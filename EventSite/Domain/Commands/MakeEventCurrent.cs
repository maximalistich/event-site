﻿using EventSite.Domain.Infrastructure;
using EventSite.Domain.Model;
using EventSite.Domain.Queries;

namespace EventSite.Domain.Commands {
    public class MakeEventCurrent : Command<Result> {
        readonly string eventId;

        public MakeEventCurrent(string eventId) {
            this.eventId = eventId;
        }

        protected override Result Execute() {
            var newCurrent = DocSession.Load<Event>(eventId);
            if(newCurrent == null) {
                return NotFound();
            }

            var oldCurrent = Bus.Query(new CurrentEvent());
            if(oldCurrent != null) {
                oldCurrent.IsCurrent = false;
            }

            if(string.IsNullOrEmpty(newCurrent.Name)) {
                return Error("The event must have a name.");
            }

            State.ChangeCurrentEvent(newCurrent);

            return SuccessFormat("\"{0}\" is now the current event.", newCurrent.Name);
        }
    }
}