﻿using System.IO;
using System.Web;
using System.Web.Mvc;
using EventSite.Domain;
using EventSite.Domain.Commands;
using EventSite.Domain.Model;
using EventSite.Domain.Queries;
using EventSite.Infrastructure.Controllers;
using EventSite.Infrastructure.Filters;

namespace EventSite.Controllers {
    public class SponsorsController : BaseController {
        [HttpGet]
        public ActionResult Index(string eventSlug = null) {
            if(string.IsNullOrEmpty(eventSlug) && State.NoEventScheduled()) {
                return View("NoEventScheduled");
            }

            var eventId = string.IsNullOrEmpty(eventSlug)
                ? State.CurrentEvent.Id
                : Event.IdFrom(eventSlug);

            var sponsors = Bus.Query(new SponsorsForEvent(eventId));

            return View(sponsors);
        }

        [HttpGet]
        [LoggedIn(Roles = Roles.Admin)]
        public ActionResult Create(string eventSlug) {
            return View("CreateOrUpdate", new CreateOrUpdateSponsor());
        }

        [HttpPost]
        [LoggedIn(Roles = Roles.Admin)]
        public ActionResult Index(string eventSlug, CreateOrUpdateSponsor input, HttpPostedFileBase file) {
            return Execute(input)
                .OnSuccess(x => {
                    if(file == null || file.ContentLength == 0) {
                        DisplayErrorMessage("You must provide an image.");
                        return View("CreateOrUpdate", input);
                    }

                    saveSponsorImage(x.Subject, file);

                    return RedirectToAction("Detail", new {
                        eventSlug,
                        sponsorSlug = Sponsor.SlugFromId(x.Subject.Id)
                    });
                })
                .OnFailure(x => View("CreateOrUpdate", input));
        }

        [HttpGet]
        [LoggedIn(Roles = Roles.Admin)]
        public ActionResult Detail(string eventSlug, string sponsorSlug) {
            var sponsor = DocSession.Load<Sponsor>(Sponsor.IdFrom(eventSlug, sponsorSlug));
            if(sponsor == null) {
                return NotFound();
            }

            return View("CreateOrUpdate", new CreateOrUpdateSponsor {
                Sponsor = sponsor
            });
        }

        [HttpPost]
        [LoggedIn(Roles = Roles.Admin)]
        public ActionResult Detail(string eventSlug, string sponsorSlug, CreateOrUpdateSponsor input, HttpPostedFileBase file = null) {
            input.Sponsor.Id = Sponsor.IdFrom(eventSlug, sponsorSlug);
            return Execute(input)
                .OnSuccess(x => {
                    if(file != null && file.ContentLength > 0) {
                        saveSponsorImage(x.Subject, file);
                    }

                    return RedirectToAction("Detail", new {eventSlug, sponsorSlug});
                })
                .OnFailure(x => View("CreateOrUpdate", input));
        }

        void saveSponsorImage(Sponsor sponsor, HttpPostedFileBase file) {
            var extension = Path.GetExtension(file.FileName);
            sponsor.ImageFileName = Sponsor.SlugFromId(sponsor.Id) + extension;

            var savePath = Server.MapPath("~" + sponsor.GetImageServerPath());

            createFolderIfNeeded(savePath);
            file.SaveAs(savePath);
        }

        static void createFolderIfNeeded(string filename) {
            var folder = Path.GetDirectoryName(filename);
            if(!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
        }
    }
}