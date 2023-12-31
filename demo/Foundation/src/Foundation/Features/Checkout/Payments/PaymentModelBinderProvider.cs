﻿using EPiServer.Commerce.Order;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Foundation.Features.Checkout.Payments
{
    public class PaymentModelBinderProvider : IModelBinderProvider
    {
        private static readonly IDictionary<Type, Type> ModelBinderTypeMappings = new Dictionary<Type, Type>
        {
            {typeof(IPaymentMethod), typeof(PaymentOptionViewModelBinder)},
        };

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (ModelBinderTypeMappings.ContainsKey(context.Metadata.ModelType))
            {
                return ActivatorUtilities.CreateInstance(ServiceLocator.Current, ModelBinderTypeMappings[context.Metadata.ModelType]) as IModelBinder;
            }
            return null;
        }
    }
}
