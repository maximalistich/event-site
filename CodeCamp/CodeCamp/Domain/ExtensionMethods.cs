﻿using System;
using System.Collections.Generic;
using CodeCamp.Domain.Model;

namespace CodeCamp.Domain {
    public static class ExtensionMethods {
        public static bool UserIsLoggedIn(this IApplicationState state) {
            return state.User != null;
        }

        public static bool UserIsAdmin(this IApplicationState state) {
            return state.UserIsLoggedIn() && state.User.Roles.Contains(Roles.Admin);
        }

        public static bool RunningInProduction(this IApplicationState state) {
            return string.Compare(state.Environment, "production", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool CanSubmitSessions(this IApplicationState state) {
            return state.UserIsLoggedIn()
                   && state.RegistrationStatus != RegistrationStatus.NoEventScheduled
                   && state.CurrentEvent.IsSessionSubmissionOpen;
        }

        public static void Apply<T>(this IEnumerable<T> enumerable, Action<T> action) {
            foreach(var item in enumerable) {
                action(item);
            }
        }
    }
}