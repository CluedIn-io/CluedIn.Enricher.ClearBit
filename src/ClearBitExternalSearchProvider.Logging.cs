using CluedIn.Core;
using CluedIn.Core.Data.Relational;
using CluedIn.ExternalSearch.DataStore;
using CluedIn.ExternalSearch.Providers.ClearBit.Model;

namespace CluedIn.ExternalSearch.Providers.ClearBit
{
    public partial class ClearBitExternalSearchProvider
    {
        public void LogResult(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result)
        {
            using (var systemContext = context.ApplicationContext.System.CreateExecutionContext())
            {
                var dataStore   = systemContext.Organization.DataStores.GetDataStore<ExternalSearchClearBitLogRecord>();
                var resultItem  = result.As<CompanyAutocompleteResult>();
                var record      = this.CreateRecord(context, query, result, resultItem.Data);

                dataStore.InsertOrUpdate(systemContext, record);
            }
        }

        private ExternalSearchClearBitLogRecord CreateRecord(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, CompanyAutocompleteResult resultItem)
        {
            var record = new ExternalSearchClearBitLogRecord();
            record.Id               = ExternalSearchLogIdGenerator.GenerateId(query.ProviderId, query.EntityType, resultItem.Name, resultItem.Domain);
            record.ProviderId       = query.ProviderId;
            record.EntityType       = query.EntityType;

            record.Name     = resultItem.Name;
            record.Domain   = resultItem.Domain;
            record.Logo     = resultItem.Logo;

            return record;
        }
    }
}
