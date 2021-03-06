﻿using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using EventSite.Controllers;

namespace EventSite.Infrastructure.Routing {
    public class RouteConfig {
        readonly RouteCollection routes;

        public RouteConfig(RouteCollection routes) {
            this.routes = routes;
        }

        public void Configure() {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new {favicon = @"(.*/)?favicon.ico(/.*)?"});

            RouteFor("home")
                .HandledBy<HomeController>(x => x.Index());

            RouteFor("contact")
                .HandledBy<HomeController>(x => x.Contact());

            RouteFor("account")
                .HandledBy<AccountController>(x => x.Index());

            RouteFor("account/login")
                .HandledBy<AccountController>(x => x.Login());

            RouteFor("account/new")
                .HandledBy<AccountController>(x => x.Create(null));

            RouteFor("account/logout")
                .HandledBy<AccountController>(x => x.Logout());

            RouteFor("events/{eventSlug}/sessions/{sessionSlug}/approve")
                .HandledBy<SessionsController>(x => x.Approve(null, null));

            RouteFor("events/{eventSlug}/sessions/{sessionSlug}/reject")
                .HandledBy<SessionsController>(x => x.Reject(null, null));

            RouteFor("events/{eventSlug}/sessions/{sessionSlug}/edit")
                .HandledBy<SessionsController>(x => x.Edit(null, null));

            RouteFor("events/{eventSlug}/sessions/new/{userSlug}")
                .HandledBy<SessionsController>(x => x.Create(null));

            RouteFor("events/{eventSlug}/sessions/{sessionSlug}")
                .HandledBy<SessionsController>(x => x.Detail(null, null, null));

            RouteFor("events/{eventSlug}/sessions")
                .HandledBy<SessionsController>(x => x.Index(null, 0, null));

            RouteFor("sessions")
                .HandledBy<SessionsController>(x => x.Index(null, 0, null));

            RouteFor("events/{eventSlug}/registrations/new")
                .HandledBy<RegistrationController>(x => x.Create());

            RouteFor("events/{eventSlug}/registrations")
                .HandledBy<RegistrationController>(x => x.Index(null));

            RouteFor("registrations/new")
                .HandledBy<RegistrationController>(x => x.Create());

            RouteFor("events/{eventSlug}/make-current")
                .HandledBy<EventsController>(x => x.MakeCurrent(null));

            RouteFor("events/{eventSlug}/location")
                .HandledBy<LocationController>(x => x.Index(null));

            RouteFor("location")
                .HandledBy<LocationController>(x => x.Index(null));

            RouteFor("events/{eventSlug}/speakers/{speakerSlug}")
                .HandledBy<SpeakersController>(x => x.Index(null, null, 0));

            RouteFor("events/{eventSlug}/speakers")
                .HandledBy<SpeakersController>(x => x.Index(null, null, 0));

            RouteFor("speakers")
                .HandledBy<SpeakersController>(x => x.Index(null, null, 0));

            RouteFor("users/update/{userSlug}")
                .HandledBy<UsersController>(x => x.Update(null));

            RouteFor("events/{eventSlug}/attendees/export")
                .HandledBy<EventsController>(x => x.ExportAttendees(null));

            RouteFor("events/{eventSlug}/attendees")
                .HandledBy<AttendeesController>(x => x.Index(null,0));

            RouteFor("attendees")
                .HandledBy<AttendeesController>(x => x.Index(null,0));

            RouteFor("events/{eventSlug}/sponsors/export")
                .HandledBy<EventsController>(x => x.ExportSponsors(null));

            RouteFor("events/{eventSlug}/sponsors")
                .HandledBy<SponsorsController>(x => x.Index(null, null));

            RouteFor("events/{eventSlug}/sponsors/new")
                .HandledBy<SponsorsController>(x => x.Create(null));

            RouteFor("events/{eventSlug}/sponsors/info")
                .HandledBy<SponsorsController>(x => x.Info(null));

            RouteFor("events/{eventSlug}/sponsors/{sponsorSlug}")
                .HandledBy<SponsorsController>(x => x.Detail(null, null));

            RouteFor("sponsors")
                .HandledBy<SponsorsController>(x => x.Index(null, null));

            RouteFor("events/new")
                .HandledBy<EventsController>(x => x.Create());

            RouteFor("events/{eventSlug}")
                .HandledBy<EventsController>(x => x.Detail(null));

            RouteFor("events")
                .HandledBy<EventsController>(x => x.Index());

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
                );

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
                );

            routes.MapRoute(
                "NotFound",
                "{*url}",
                new {controller = "Error", action = "Error", errorCode = 404, message = "Not Found"}
                );
        }

        RouteBuilder RouteFor(string route) {
            return new RouteBuilder(routes, route);
        }
    }
}