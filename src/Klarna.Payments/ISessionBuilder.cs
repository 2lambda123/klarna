﻿using System.Collections.Generic;
using EPiServer.Commerce.Order;
using Klarna.Common.Configuration;
using Klarna.Payments.Models;

namespace Klarna.Payments
{
    public interface ISessionBuilder
    {
        Session Build(Session session, ICart cart, PaymentsConfiguration paymentsConfiguration, IDictionary<string, object> dic = null, bool includePersonalInformation = false);
    }
}
