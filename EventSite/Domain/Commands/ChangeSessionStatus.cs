﻿using System.Linq;
using EventSite.Domain.Infrastructure;
using EventSite.Domain.Model;
using EventSite.Domain.Queries;
using EventSite.Domain.WorkItems;

namespace EventSite.Domain.Commands {
    public class ChangeSessionStatus : Command<Result>.AdminOnly {
        readonly SessionStatus status;
        readonly string sessionId;

        public ChangeSessionStatus(string sessionId, SessionStatus status) {
            this.status = status;
            this.sessionId = sessionId;
        }

        protected override Result Execute() {
            var session = DocSession.Load<Session>(sessionId);
            if(session == null) {
                return NotFound();
            }

            session.Status = status;

            if(status == SessionStatus.Approved) {
                setSpeakerStatus(session.Event.Id, session.User.Id, true);
            } else {
                var sessionsForSpeaker = Bus.Query(new SubmittedSessions(session.Event.Id, session.User.Id));
                if(sessionsForSpeaker.All(s => s.Status != SessionStatus.Approved)) {
                    setSpeakerStatus(session.Event.Id, session.User.Id, false);
                }
            }

            var message = string.Format("The session \"{0}\" has been given a status of {1}.", session.Name, status);

            if(status == SessionStatus.Approved || status == SessionStatus.Rejected) {
                var user = DocSession.Load<User>(session.User.Id);
                Bus.Enqueue(new SendSessionStatusChangeEmail(State.CurrentEvent, session, user));
                message += " An email has been sent to the submitter to inform them.";
            }

            return Success(message);
        }

        void setSpeakerStatus(string eventId, string userId, bool isSpeaker) {
            var reg = Bus.Query(new GetUserRegistration(eventId, userId));
            reg.IsSpeaker = isSpeaker;
        }
    }
}