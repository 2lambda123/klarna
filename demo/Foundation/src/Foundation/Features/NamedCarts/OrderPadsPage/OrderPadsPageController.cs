﻿using EPiServer.Commerce.Order;
using EPiServer.Web.Mvc;
using Foundation.Features.Checkout.Services;
using Foundation.Features.Checkout.ViewModels;
using Foundation.Features.MyOrganization.Organization;
using Foundation.Infrastructure.Cms;
using Foundation.Infrastructure.Commerce;
using Foundation.Infrastructure.Commerce.Customer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Foundation.Features.NamedCarts.OrderPadsPage
{
    [Authorize]
    public class OrderPadsPageController : PageController<OrderPadsPage>
    {
        private readonly ICustomerService _customerService;
        private readonly ICartService _cartService;
        private readonly IOrganizationService _organizationService;
        private readonly ICookieService _cookieService;

        public OrderPadsPageController(ICartService cartService, ICustomerService customerService, IOrganizationService organizationService, ICookieService cookieService)
        {
            _customerService = customerService;
            _cartService = cartService;
            _organizationService = organizationService;
            _cookieService = cookieService;
        }

        [NavigationAuthorize("Admin,Approver")]
        public ActionResult Index(OrderPadsPage currentPage)
        {
            var currentOrganization = !string.IsNullOrEmpty(_cookieService.Get(Constant.Fields.SelectedOrganization))
               ? _organizationService.GetSubFoundationOrganizationById(_cookieService.Get(Constant.Fields.SelectedOrganization))
               : _organizationService.GetCurrentFoundationOrganization();

            var viewModel = new OrderPadsPageViewModel
            {
                CurrentContent = currentPage,
                QuoteStatus = "",
                CurrentCustomer = _customerService.GetCurrentContact(),
                OrganizationOrderPadList = new List<OrganizationOrderPadViewModel>()
            };

            if (currentOrganization != null)
            {
                if (string.IsNullOrEmpty(_cookieService.Get(Constant.Fields.SelectedOrganization)))
                {
                    // Has suborganizatons. (is Organization)
                    foreach (var suborganization in currentOrganization.SubOrganizations)
                    {
                        viewModel.OrganizationOrderPadList.Add(AddSuborganizationToOrderPadList(suborganization.OrganizationId.ToString(), suborganization.Name));
                    }
                }
                else
                {
                    // Has only users. (is Suborganization)
                    viewModel.OrganizationOrderPadList.Add(AddSuborganizationToOrderPadList(currentOrganization.OrganizationId.ToString(), currentOrganization.Name));
                }
            }

            return View(viewModel);
        }

        private OrganizationOrderPadViewModel AddSuborganizationToOrderPadList(string suborganizationGuid, string suborganizationName)
        {
            var orderPadOrganization = new OrganizationOrderPadViewModel
            {
                OrganizationName = suborganizationName,
                OrganizationId = suborganizationGuid,
                UsersOrderPad = new List<UsersOrderPadViewModel>()
            };

            var organizationUsersList = _customerService.GetContactsForOrganization(_organizationService.GetFoundationOrganizationById(suborganizationGuid));
            foreach (var user in organizationUsersList)
            {
                var userOrderPad = new UsersOrderPadViewModel
                {
                    UserName = user.FullName,
                    UserId = user.ContactId.ToString()
                };
                userOrderPad.WishCartList = _cartService.LoadWishListCardByCustomerId(user.ContactId);
                if (userOrderPad.WishCartList != null)
                {
                    foreach (var lineItem in userOrderPad.WishCartList.GetAllLineItems())
                    {
                        lineItem.PlacedPrice = _cartService.GetDiscountedPrice(userOrderPad.WishCartList, lineItem).Value.Amount;
                    }
                }
                orderPadOrganization.UsersOrderPad.Add(userOrderPad);
            }

            return orderPadOrganization;
        }
    }
}