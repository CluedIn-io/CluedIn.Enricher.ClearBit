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
            Token = new List<Control>()
            {
                new Control()
                {
                    DisplayName = "Accepted Entity Type",
                    Type = "input",
                    IsRequired = true,
                    Name = KeyName.AcceptedEntityType,
                    Help = "The entity type that defines the golden records you want to enrich (e.g., /Organization)."
                },
                new Control()
                {
                    DisplayName = "Website Vocabulary Key",
                    Type = "input",
                    IsRequired = false,
                    Name = KeyName.WebsiteKey,
                    Help = "The vocabulary key that contains the websites of companies you want to enrich (e.g., organization.website)."
                },
                new Control()
                {
                    DisplayName = "Organization Name Vocabulary Key",
                    Type = "input",
                    IsRequired = false,
                    Name = KeyName.OrgNameKey,
                    Help = "The vocabulary key that contains the names of companies you want to enrich (e.g., organization.name)."
                },
                new Control()
                {
                    DisplayName = "Email Domain Vocabulary Key",
                    Type = "input",
                    IsRequired = false,
                    Name = KeyName.EmailDomainKey,
                    Help = "The vocabulary key that contains the email domains of companies you want to enrich (e.g., organization.domain)."
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
