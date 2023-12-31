﻿using Foundation.Features.MyOrganization.SubOrganization;
using Foundation.Features.Shared;
using Foundation.Infrastructure.Commerce.Customer;
using System.Collections.Generic;

namespace Foundation.Features.MyOrganization.Users
{
    public class UsersPageViewModel : ContentViewModel<UsersPage>
    {
        public List<FoundationContact> Users { get; set; }
        public FoundationContact Contact { get; set; }
        public List<FoundationOrganization> Organizations { get; set; }
        public SubFoundationOrganizationModel SubOrganization { get; set; }
    }
}
