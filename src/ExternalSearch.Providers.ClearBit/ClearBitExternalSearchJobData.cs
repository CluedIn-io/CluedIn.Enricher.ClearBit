using System.Collections.Generic;
using CluedIn.Core.Crawling;

namespace CluedIn.ExternalSearch.Providers.ClearBit
{
    public class ClearBitExternalSearchJobData : CrawlJobData
    {
        public ClearBitExternalSearchJobData(IDictionary<string, object> configuration)
        {
           
        }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object> {
                
            };
        }
        
    }
}
