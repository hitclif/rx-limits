using System;
using System.Collections.Generic;
using System.Reactive.Linq;
namespace ConsoleApp1
{
    public static class Functions
    {
        public static IObservable<Data> CompleteAfter(this IObservable<Data> stream, string sessionId, int limit)
        {
            // a flag to inidicate whether there was the first occurence
            var initialized = false;

            // counter for item of other sessions
            var otherSessionIdItems = 0;

            var sessionDataStream = stream
                .Select(data =>
                {
                    initialized = initialized || data.SessionId == sessionId;
                    otherSessionIdItems = data.SessionId == sessionId || !initialized
                        ? otherSessionIdItems = 0  // keep counter on zero if was not intialized or data is from my session
                        : otherSessionIdItems + 1; // increase counter of items from other sessions

                    return new
                    {
                        otherSessionIdItems,
                        data
                    };
                })
                .TakeWhile(d => d.otherSessionIdItems < limit) // executes Complete if condition is not satisfied
                .Where(d => d.data.SessionId == sessionId)
                .Select(d => d.data);

            return sessionDataStream;
        }
    }
}
