﻿using System.Web.Mvc;
using EventSite.Infrastructure.Controllers;

namespace EventSite.Controllers {
    public class ErrorController : LowLevelController {
        public ActionResult Error(int errorCode = 500, string message = null, string url = null) {
            return CreateErrorActionResult(errorCode, message, url);
        }
    }
}