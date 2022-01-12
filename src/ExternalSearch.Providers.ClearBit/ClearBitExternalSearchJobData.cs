using System.Collections.Generic;
using CluedIn.Core.Crawling;

namespace CluedIn.ExternalSearch.Providers.ClearBit
{
    public abstract class ClearBitExternalSearchJobData : CrawlJobData
    {
        public ClearBitExternalSearchJobData(IDictionary<string, object> configuration)
        {
            ApiToken = GetValue<string>(configuration, ClearBitConstants.KeyName.ApiToken);
        }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object> {
                { ClearBitConstants.KeyName.ApiToken, ApiToken }
            };
        }

        public string ApiToken { get; set; }
    }
}
