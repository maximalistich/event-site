﻿using System.Web.Mvc;
using EventSite.Domain;
using EventSite.Domain.Model;
using EventSite.Domain.Queries;
using EventSite.Infrastructure.Controllers;
using EventSite.ViewModels.RegisteredUsers;

namespace EventSite.Controllers {
    public class AttendeesController : BaseController {
        const int PageSize = 24; //we show six per row, so we'll show 4 rows

        [HttpGet]
        public ActionResult Index(string eventSlug = null, int page = 1) {
            if(string.IsNullOrEmpty(eventSlug) && State.NoEventScheduled()) {
                return View("NoEventScheduled");
            }

            var eventId = string.IsNullOrEmpty(eventSlug)
                ? State.CurrentEvent.Id
                : Event.IdFrom(eventSlug);

            var attendees = Bus.Query(new AttendeesForEvent(eventId, page, PageSize));

            return View(new IndexOutput<Attendee>(attendees, null, eventSlug));
        }
    }
}