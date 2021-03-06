﻿using System;
using System.Collections.Generic;
using EventSite.Domain.Model;

namespace EventSite.Domain {
    public static class ExtensionMethods {
        public static bool UserIsLoggedIn(this IApplicationState state) {
            return state.User != null;
        }

        public static bool UserCanManageSponsors(this IApplicationState state) {
            return state.UserIsLoggedIn() && UserHasPermission(state.User, Roles.ManageSponsors);
        }

        public static bool UserIsAdmin(this IApplicationState state) {
            return state.UserIsLoggedIn() && UserHasPermission(state.User, Roles.Admin);
        }

        private static bool UserHasPermission(User user, string role) {
            if (user.Roles.Contains(Roles.Admin)) {
                return true;
            }

            return user.Roles.Contains(role);
        }

        public static bool RunningInProduction(this IApplicationState state) {
            return state.Settings.RunningInProduction();
        }

        public static bool RunningInProduction(this ISettings settings) {
            return string.Compare(settings.Environment, "Production", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool CanSubmitSessions(this IApplicationState state) {
            return state.UserIsLoggedIn()
                   && state.RegistrationStatus != RegistrationStatus.NoEventScheduled
                   && state.CurrentEvent.IsSessionSubmissionOpen;
        }

        public static bool EventScheduled(this IApplicationState state) {
            return state.CurrentEvent != null;
        }

        public static bool NoEventScheduled(this IApplicationState state) {
            return state.CurrentEvent == null;
        }

        public static bool RegisteredForEvent(this IApplicationState state) {
            return state.RegistrationStatus == RegistrationStatus.Registered;
        }

        public static string CurrentEventSlug(this IApplicationState state) {
            if(!state.EventScheduled()) {
                return null;
            }

            return Event.SlugFromId(state.CurrentEvent.Id);
        }

        public static string UserSlug(this IApplicationState state) {
            if(!state.UserIsLoggedIn()) {
                return null;
            }

            return User.SlugFromId(state.User.Id);
        }

        public static void Apply<T>(this IEnumerable<T> enumerable, Action<T> action) {
            foreach(var item in enumerable) {
                action(item);
            }
        }
    }
}