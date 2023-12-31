﻿using System.Collections.Generic;
using System.Text.Json.Serialization;
using Klarna.Common.Models;

namespace Klarna.Checkout.Models
{
    public class CallbackAddressUpdateRequest
    {
        /// <summary>
        /// Non-negative, minor units. Total total amount of the order, including tax and any discounts.
        /// </summary>
        /// <remarks>Required</remarks>
        [JsonPropertyName("order_amount")]
        public int OrderAmount { get; set; }
        /// <summary>
        /// Non-negative, minor units. The total tax amount of the order.
        /// </summary>
        /// <remarks>Required</remarks>
        [JsonPropertyName("order_tax_amount")]
        public int OrderTaxAmount { get; set; }
        /// <summary>
        /// The applicable order lines (max 1000)
        /// </summary>
        /// <remarks>Required</remarks>
        [JsonPropertyName("order_lines")]
        public ICollection<OrderLine> OrderLines { get; set; }
        /// <summary>
        /// Customer's billing address
        /// </summary>
        /// <remarks>Read only</remarks>
        [JsonPropertyName("billing_address")]
        public CheckoutAddressInfo BillingCheckoutAddress { get; set; }
        /// <summary>
        /// Customer's shipping address
        /// </summary>
        /// <remarks>Read only</remarks>
        [JsonPropertyName("shipping_address")]
        public CheckoutAddressInfo ShippingCheckoutAddress { get; set; }
        /// <summary>
        /// Current shipping option selected by the customer.
        /// </summary>s
        /// <remarks>Read only</remarks>
        [JsonPropertyName("selected_shipping_option")]
        public ShippingOption SelectedShippingOption { get; set; }
        /// <summary>
        /// ISO 4217 purchase currency.
        /// </summary>
        /// <remarks>Required</remarks>
        [JsonPropertyName("purchase_currency")]
        public string PurchaseCurrency { get; set; }
    }
}
