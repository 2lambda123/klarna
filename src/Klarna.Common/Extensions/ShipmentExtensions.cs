using System.Linq;
using EPiServer.Commerce.Order;
using EPiServer.ServiceLocation;
using Klarna.Common.Helpers;
using Klarna.Common.Models;
using Mediachase.Commerce.Markets;

namespace Klarna.Common.Extensions
{
    public static class ShipmentExtensions
    {
#pragma warning disable 649
        private static Injected<IShippingCalculator> _shippingCalculator;
        private static Injected<IMarketService> _marketService;
#pragma warning restore 649

        public static OrderLine GetOrderLine(this IShipment shipment, IOrderGroup cart, OrderGroupTotals totals, bool includeTaxes)
        {
            var total = AmountHelper.GetAmount(totals.ShippingTotal);
            var totalTaxAmount = 0;
            var taxRate = 0;

            if (includeTaxes)
            {
                var market = _marketService.Service.GetMarket(cart.MarketId);
                var shippingTaxTotal = _shippingCalculator.Service.GetShippingTax(shipment, market, cart.Currency);

                if (shippingTaxTotal.Amount > 0)
                {
                    totalTaxAmount = AmountHelper.GetAmount(shippingTaxTotal.Amount);

                    var shippingTotalExcludingTax = market.PricesIncludeTax
                        ? totals.ShippingTotal.Amount - shippingTaxTotal.Amount
                        : totals.ShippingTotal.Amount;
                    taxRate = AmountHelper.GetAmount(shippingTaxTotal.Amount * 100 / shippingTotalExcludingTax);

                    if (!market.PricesIncludeTax)
                    {
                        total = total + totalTaxAmount;
                    }
                }
            }

            var shipmentOrderLine = new OrderLine
            {
                Name = shipment.ShippingMethodName,
                Quantity = 1,
                UnitPrice = total,
                TotalAmount = total,
                TaxRate = taxRate,
                TotalTaxAmount = totalTaxAmount,
                Type = OrderLineType.shipping_fee
            };
            if (string.IsNullOrEmpty(shipmentOrderLine.Name))
            {
                var shipmentMethod = Mediachase.Commerce.Orders.Managers.ShippingManager.GetShippingMethod(shipment.ShippingMethodId)
                    .ShippingMethod.FirstOrDefault();
                if (shipmentMethod != null)
                {
                    shipmentOrderLine.Name = shipmentMethod.DisplayName;
                }
            }
            return shipmentOrderLine;
        }
    }
}