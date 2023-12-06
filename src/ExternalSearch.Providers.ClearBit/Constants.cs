using System;
using System.Collections.Generic;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.Providers;

namespace CluedIn.ExternalSearch.Providers.ClearBit
{
    public static class Constants
    {
        public const string ComponentName = "Clearbit";
        public const string ProviderName = "Clearbit";
        public static readonly Guid ProviderId = Core.Constants.ExternalSearchProviders.ClearBitId;

        public struct KeyName
        {
            public const string AcceptedEntityType = "acceptedEntityType";
            public const string WebsiteKey = "websiteKey";
            public const string OrgNameKey = "orgNameKey";
            public const string EmailDomainKey = "emailDomainKey";
        }

        public static string About { get; set; } = "Clearbit is an enrichment service for company data.";
        public static string Icon { get; set; } = "Resources.clearbit-vector-logo.svg";
        public static string Domain { get; set; } = "N/A";

        public static AuthMethods AuthMethods { get; set; } = new AuthMethods
        {
            token = new List<Control>()
            {
                new Control()
                {
                    displayName = "Accepted Entity Type",
                    type = "input",
                    isRequired = false,
                    name = KeyName.AcceptedEntityType
                },
                new Control()
                {
                    displayName = "Website vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.WebsiteKey
                },
                new Control()
                {
                    displayName = "Organization Name vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.OrgNameKey
                },
                new Control()
                {
                    displayName = "Email Domain vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.EmailDomainKey
                }
            }
        };

        public static IEnumerable<Control> Properties { get; set; } = new List<Control>()
        {
            // NOTE: Leaving this commented as an example - BF
            //new()
            //{
            //    displayName = "Some Data",
            //    type = "input",
            //    isRequired = true,
            //    name = "someData"
            //}
        };

        public static Guide Guide { get; set; } = null;
        public static IntegrationType IntegrationType { get; set; } = IntegrationType.Enrichment;
    }
}
