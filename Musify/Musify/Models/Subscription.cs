using System;
using System.Collections.Generic;

namespace Musify.Models {
    public class Subscription {
        /// <summary>
        /// Explains how to pass JSON data to an object of this type.
        /// </summary>
        public static Dictionary<string, string> JSON_EQUIVALENTS { get; } = new Dictionary<string, string>() {
            { "subscription_id", "SubscriptionId" },
            { "account_id", "AccountId" },
            { "cost", "Cost" },
            { "start_date", "StartDate" },
            { "end_date", "EndDate" }
        };

        public int SubscriptionId { get; set; }
        public int AccountId { get; set; }
        public float Cost { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public Subscription() {
        }
    }
}
