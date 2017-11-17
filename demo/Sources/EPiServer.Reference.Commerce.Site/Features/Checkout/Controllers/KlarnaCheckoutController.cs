﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EPiServer.Commerce.Order;
using EPiServer.Logging;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Services;
using Klarna.Checkout;
using Klarna.Checkout.Models;
using Klarna.Common.Models;
using Newtonsoft.Json;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.Controllers
{
    [RoutePrefix("klarnacheckout")]
    public class KlarnaCheckoutController : ApiController
    {
        private ILogger _log = LogManager.GetLogger(typeof(KlarnaPaymentController));
        private readonly IKlarnaCheckoutService _klarnaCheckoutService;
        private readonly IOrderRepository _orderRepository;
        private readonly ILineItemValidator _lineItemValidator;
        private readonly ICartService _cartService;
        private readonly CheckoutService _checkoutService;


        public KlarnaCheckoutController(
            IKlarnaCheckoutService klarnaCheckoutService,
            IOrderRepository orderRepository, 
            ILineItemValidator lineItemValidator, 
            ICartService cartService, 
            CheckoutService checkoutService)
        {
            _klarnaCheckoutService = klarnaCheckoutService;
            _orderRepository = orderRepository;
            _lineItemValidator = lineItemValidator;
            _cartService = cartService;
            _checkoutService = checkoutService;
        }
        
        [Route("cart/{orderGroupId}/shippingoptionupdate")]
        [AcceptVerbs("POST")]
        [HttpPost]
        [ResponseType(typeof(ShippingOptionUpdateResponse))]
        public IHttpActionResult ShippingOptionUpdate(int orderGroupId, [FromBody]ShippingOptionUpdateRequest shippingOptionUpdateRequest)
        {
            var cart = _orderRepository.Load<ICart>(orderGroupId);

            var response = _klarnaCheckoutService.UpdateShippingMethod(cart, shippingOptionUpdateRequest);

            return Ok(response);
        }

        [Route("cart/{orderGroupId}/addressupdate")]
        [AcceptVerbs("POST")]
        [HttpPost]
        [ResponseType(typeof(AddressUpdateResponse))]
        public IHttpActionResult AddressUpdate(int orderGroupId, [FromBody]AddressUpdateRequest addressUpdateRequest)
        {
            var cart = _orderRepository.Load<ICart>(orderGroupId);
            var response = _klarnaCheckoutService.UpdateAddress(cart, addressUpdateRequest);
            return Ok(response);
        }

        [Route("cart/{orderGroupId}/ordervalidation")]
        [AcceptVerbs("POST")]
        [HttpPost]
        public IHttpActionResult OrderValidation(int orderGroupId, [FromBody]PatchedCheckoutOrderData checkoutData)
        {
            var cart = _orderRepository.Load<ICart>(orderGroupId);

            // Validate cart lineitems
            var validationIssues = _cartService.ValidateCart(cart);
            if (validationIssues.Any())
            {
                // check validation issues and redirect to a page to display the error
                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.RedirectMethod);
                httpResponseMessage.Headers.Location = new Uri("http://klarna.geta.no/en/error-pages/checkout-something-went-wrong/");
                return ResponseMessage(httpResponseMessage);
            }

            // Validate billing address if necessary (this is just an example)
            // To return an error like this you need require_validate_callback_success set to true
            if (checkoutData.BillingAddress.PostalCode.Equals("94108-2704"))
            {
                var errorResult = new ErrorResult
                {
                    ErrorType = ErrorType.address_error,
                    ErrorText = "Can't ship to postalcode 94108-2704"
                };
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, errorResult));
            }

            // Validate order amount, shipping address
            if (!_klarnaCheckoutService.ValidateOrder(cart, checkoutData))
            {
                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.RedirectMethod);
                httpResponseMessage.Headers.Location = new Uri("http://klarna.geta.no/en/error-pages/checkout-something-went-wrong/");
                return ResponseMessage(httpResponseMessage);
            }

            return Ok();
        }

        [Route("cart/{orderGroupId}/fraud")]
        [AcceptVerbs("POST")]
        [HttpPost]
        public IHttpActionResult FraudNotification(int orderGroupId, string klarna_order_id)
        {
            var purchaseOrder = GetOrCreatePurchaseOrder(orderGroupId, klarna_order_id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            var requestParams = Request.Content.ReadAsStringAsync().Result;
            if (!string.IsNullOrEmpty(requestParams))
            {
                var notification = JsonConvert.DeserializeObject<NotificationModel>(requestParams);
                _klarnaCheckoutService.FraudUpdate(notification);
            }
            return Ok();
        }

        [Route("cart/{orderGroupId}/push")]
        [AcceptVerbs("POST")]
        [HttpPost]
        public IHttpActionResult Push(int orderGroupId, string klarna_order_id)
        {
            if (klarna_order_id == null)
            {
                return BadRequest();
            }
            var purchaseOrder = GetOrCreatePurchaseOrder(orderGroupId, klarna_order_id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            // Update merchant reference
            _klarnaCheckoutService.UpdateMerchantReference1(purchaseOrder);

            // Acknowledge the order through the order management API
            _klarnaCheckoutService.AcknowledgeOrder(purchaseOrder);

            return Ok();
        }

        private IPurchaseOrder GetOrCreatePurchaseOrder(int orderGroupId, string klarnaOrderId)
        {
            // Check if the order has been created already
            var purchaseOrder = _klarnaCheckoutService.GetPurchaseOrderByKlarnaOrderId(klarnaOrderId);
            if (purchaseOrder != null)
            {
                return purchaseOrder;
            }
            
            // Check if we still have a cart and can create an order
            var cart = _orderRepository.Load<ICart>(orderGroupId);
            var cartKlarnaOrderId = cart.Properties[Constants.KlarnaCheckoutOrderIdCartField]?.ToString();
            if (cartKlarnaOrderId == null || !cartKlarnaOrderId.Equals(klarnaOrderId))
            {
                return null;
            }

            var order = _klarnaCheckoutService.GetOrder(klarnaOrderId, cart.Market);
            if (!order.Status.Equals("checkout_complete"))
            {
                // Won't create order, Klarna checkout not complete
                return null;
            }
            purchaseOrder = _checkoutService.CreatePurchaseOrderForKlarna(klarnaOrderId, order, cart);
            return purchaseOrder;
        }
    }
}