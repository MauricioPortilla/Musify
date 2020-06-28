using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musify.Models {
    public class Subscription {
        /// <summary>
        /// Explains how to pass JSON data to an object of this type.
        /// </summary>
        public static Dictionary<string, string> JSON_EQUIVALENTS = new Dictionary<string, string>() {
            { "subscription_id", "SubscriptionId" },
            { "account_id", "AccountId" },
            { "cost", "Cost" },
            { "start_date", "StartDate" },
            { "end_date", "EndDate" }
        };

        private int subscriptionId;
        public int SubscriptionId {
            get => subscriptionId;
            set => subscriptionId = value;
        }
        private int accountId;
        public int AccountId {
            get => accountId;
            set => accountId = value;
        }
        private float cost;
        public float Cost {
            get => cost;
            set => cost = value;
        }
        private DateTime startDate;
        public DateTime StartDate {
            get => startDate;
            set => startDate = value;
        }
        private DateTime endDate;
        public DateTime EndDate {
            get => endDate;
            set => endDate = value;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public Subscription() {
        }
    }
}
