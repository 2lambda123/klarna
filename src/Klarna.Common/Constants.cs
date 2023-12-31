﻿namespace Klarna.Common
{
    public static class Constants
    {
        public const string KlarnaSystemKeyword = "Klarna";

        public const string OrderNamespace = "Mediachase.Commerce.Orders";
        public const string PurchaseOrderClass = "PurchaseOrder";
        public const string OtherPaymentClass = "OtherPayment";
        public const string CartClass = "ShoppingCart";

        // Purchase order meta fields
        public const string KlarnaOrderIdField = "KlarnaOrderId";

        // Other payment meta fields
        public const string FraudStatusPaymentField = "FraudStatusPayment";
    }
}
