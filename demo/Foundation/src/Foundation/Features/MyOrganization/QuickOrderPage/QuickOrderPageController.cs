﻿using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Foundation.Features.MyOrganization.QuickOrderPage
{
    [Authorize]
    public class QuickOrderPageController : PageController<QuickOrderPage>
    {
        public ActionResult Index(QuickOrderPage currentPage)
        {
            return View(new QuickOrderPageViewModel
            {
                CurrentContent = currentPage
            });
        }
    }
}