﻿using System.Web.Mvc;
using CodeCamp.Infrastructure.Controllers;
using CodeCamp.ViewModels.Account;

namespace CodeCamp.Controllers {
    public class AccountController : BaseController {
        [HttpGet]
        public ActionResult Login() {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateAccountViewModel input) {
            return Execute(input)
                .OnSuccess(x => {
                    State.Login(x.Subject, input.Persist);
                    return Redirect(input.ReturnUrl);
                })
                .OnFailure(x => View(input));
        }
    }
}