﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using EPiServer.Commerce.Order;
using Klarna.Common.Helpers;
using Klarna.Payments.Models;

namespace Klarna.Payments.Extensions
{
    public static class CartExtensions
    {
        public static string GetKlarnaSessionId(this ICart cart)
        {
            return cart.Properties[Constants.KlarnaSessionIdCartField]?.ToString();
        }

        public static void SetKlarnaSessionId(this ICart cart, string sessionId)
        {
            cart.Properties[Constants.KlarnaSessionIdCartField] = sessionId;
        }

        public static string GetKlarnaClientToken(this ICart cart)
        {
            return cart.Properties[Constants.KlarnaClientTokenCartField]?.ToString();
        }

        public static void SetKlarnaClientToken(this ICart cart, string clientToken)
        {
            cart.Properties[Constants.KlarnaClientTokenCartField] = clientToken;
        }

        public static Descriptor GetKlarnaPaymentsDescriptor(this ICart cart)
        {
            var value = cart.Properties[Constants.KlarnaPaymentsDescriptorCartField]?.ToString() ?? string.Empty;
            return JsonSerializer.Deserialize<Descriptor>(value);
        }

        public static void SetKlarnaPaymentsDescriptor(this ICart cart, Descriptor descriptor)
        {
            var serialized = JsonSerializer.Serialize(descriptor);
            cart.Properties[Constants.KlarnaPaymentsDescriptorCartField] = serialized;
        }

        public static IEnumerable<PaymentMethodCategory> GetKlarnaPaymentMethodCategories(this ICart cart)
        {
            var value = cart.Properties[Constants.KlarnaPaymentMethodCategoriesCartField]?.ToString() ?? string.Empty;
            return JsonSerializer.Deserialize<PaymentMethodCategory[]>(value);
        }

        public static void SetKlarnaPaymentMethodCategories(
            this ICart cart,
            IEnumerable<PaymentMethodCategory> paymentMethodCategories)
        {
            var serialized = JsonSerializer.Serialize(paymentMethodCategories.ToArray());
            cart.Properties[Constants.KlarnaPaymentMethodCategoriesCartField] = serialized;
        }

        public static Uri GetSiteUrl(this ICart cart)
        {
            var url = cart.Properties[Constants.KlarnaSiteUrlCartField]?.ToString() ?? string.Empty;
            return Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri : SiteUrlHelper.GetCurrentSiteUrl();
        }

        public static void SetSiteUrl(
            this ICart cart, Uri siteUrl)
        {
            cart.Properties[Constants.KlarnaSiteUrlCartField] = siteUrl?.ToString() ?? string.Empty;
        }
    }
}