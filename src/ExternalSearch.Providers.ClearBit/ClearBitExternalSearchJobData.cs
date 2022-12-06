using System.Collections.Generic;
using CluedIn.Core.Crawling;
using CluedIn.Core.Data.Relational;

namespace CluedIn.ExternalSearch.Providers.ClearBit
{
    public class ClearBitExternalSearchJobData : CrawlJobData
    {
        public ClearBitExternalSearchJobData(IDictionary<string, object> configuration)
        {
            AcceptedEntityType = GetValue<string>(configuration, Constants.KeyName.AcceptedEntityType);
            WebsiteKey = GetValue<string>(configuration, Constants.KeyName.WebsiteKey);
            OrgNameKey = GetValue<string>(configuration, Constants.KeyName.OrgNameKey);
            EmailDomainKey = GetValue<string>(configuration, Constants.KeyName.EmailDomainKey);
        }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object> {
                { Constants.KeyName.AcceptedEntityType, AcceptedEntityType },
                { Constants.KeyName.WebsiteKey, WebsiteKey },
                { Constants.KeyName.OrgNameKey, OrgNameKey },
                { Constants.KeyName.EmailDomainKey, EmailDomainKey }
            };
        }
        public string AcceptedEntityType { get; set; }
        public string WebsiteKey { get; set; }
        public string OrgNameKey { get; set; }
        public string EmailDomainKey { get; set; }
    }
}
