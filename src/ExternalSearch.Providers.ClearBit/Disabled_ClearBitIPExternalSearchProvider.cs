// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearBitExternalSearchProvider.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the ClearBitExternalSearchProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

using CluedIn.Core;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.ExternalSearch.Providers.ClearBit.Model;
using CluedIn.ExternalSearch.Providers.ClearBit.Vocabularies;

using RestSharp;

namespace CluedIn.ExternalSearch.Providers.ClearBit
{
    /// <summary>The clear bit external search provider.</summary>
    /// <seealso cref="CluedIn.ExternalSearch.ExternalSearchProviderBase" />
    public class ClearBitIPExternalSearchProvider : ExternalSearchProviderBase
    {
        /**********************************************************************************************************
         * CONSTRUCTORS
         **********************************************************************************************************/

        /// <summary>
        /// Initializes a new instance of the <see cref="ClearBitIPExternalSearchProvider" /> class.
        /// </summary>
        public ClearBitIPExternalSearchProvider()
            : base(Constants.ExternalSearchProviders.ClearBitIpId, EntityType.Organization)
        {
        }

        /**********************************************************************************************************
         * METHODS
         **********************************************************************************************************/

        /// <summary>Builds the queries.</summary>
        /// <param name="context">The context.</param>
        /// <param name="request">The request.</param>
        /// <returns>The search queries.</returns>
        public override IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request)
        {
            //Disable
            yield break;

            #region TODO ClearBitIPExternalSearchProvider.BuildQueries(ExecutionContext context, IExternalSearchRequest request) is disabled in code ... review

            //if (!this.Accepts(request.EntityMetaData.EntityType))
            //    yield break;

            //var existingResults = request.GetQueryResults<Company>(this).ToList();

            //Func<string, bool> ipFilter = value => existingResults.Any(r => string.Equals(r.Data.id, value, StringComparison.InvariantCultureIgnoreCase));

            //// Query Input
            //var entityType = request.EntityMetaData.EntityType;
            //var ipAddress = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.IpAddress, null);

            //if (ipAddress != null)
            //{
            //    var values = ipAddress;

            //    foreach (var value in values.Where(v => !ipFilter(v)))
            //        yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Identifier, value);
            //}

            #endregion
        }

        /// <summary>Executes the search.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <returns>The results.</returns>
        public override IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query)
        {
            var name = query.QueryParameters[ExternalSearchQueryParameter.Identifier].FirstOrDefault();

            if (string.IsNullOrEmpty(name))
                yield break;

            var sharedApiToken = ConfigurationManagerEx.AppSettings["Providers.ExternalSearch.ClearBit.ApiToken"];

            var client = new RestClient("https://reveal.clearbit.com");
            var request = new RestRequest(string.Format("/v1/companies/find?ip={0}", name), Method.GET);
            request.AddHeader("Authorization", "Bearer " + sharedApiToken);

            var response = client.ExecuteTaskAsync<ClearbitResponse>(request).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                yield return new ExternalSearchQueryResult<Company>(query, response.Data.company);
            }
            else if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound)
                yield break;
            else if (response.ErrorException != null)
                throw new AggregateException(response.ErrorException.Message, response.ErrorException);
            else
                throw new ApplicationException("Could not execute external search query - StatusCode:" + response.StatusCode);
        }

        /// <summary>Builds the clues.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The clues.</returns>
        public override IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<Company>();

            var code = this.GetOriginEntityCode(resultItem);

            var clue = new Clue(code, context.Organization);
            clue.Data.OriginProviderDefinitionId = this.Id;

            this.PopulateMetadata(clue.Data.EntityData, resultItem);
            this.DownloadPreviewImage(context, resultItem.Data.logo, clue);

            return new[] { clue };
        }

        /// <summary>Gets the primary entity metadata.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The primary entity metadata.</returns>
        public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<Company>();
            return this.CreateMetadata(resultItem);
        }

        /// <summary>Gets the preview image.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The preview image.</returns>
        public override IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            return this.DownloadPreviewImageBlob<Company>(context, result, r => r.Data.logo);
        }

        /// <summary>Creates the metadata.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The metadata.</returns>
        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<Company> resultItem)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateMetadata(metadata, resultItem);

            return metadata;
        }

        /// <summary>Gets the origin entity code.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The origin entity code.</returns>
        private EntityCode GetOriginEntityCode(IExternalSearchQueryResult<Company> resultItem)
        {
            return new EntityCode(EntityType.Organization, CodeOrigin.CluedIn.CreateSpecific("clearBit"), resultItem.Data.id);
        }

        /// <summary>Populates the metadata.</summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="resultItem">The result item.</param>
        private void PopulateMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<Company> resultItem)
        {
            var code = this.GetOriginEntityCode(resultItem);

            metadata.EntityType = EntityType.Organization;
            metadata.Name = resultItem.Data.name;
            metadata.OriginEntityCode = code;

            metadata.Codes.Add(code);

            metadata.Description = resultItem.Data.description;
            //metadata.Properties[ClearBitVocabulary.Organization.LastName] = resultItem.Data.domain;
            //metadata.Properties[ClearBitVocabulary.Organization.Avatar] = resultItem.Data.indexedAt;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.logo;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.type;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.url;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.category.industry;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.category.industryGroup;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.category.sector;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.category.subIndustry;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.crunchbase.handle.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = string.Join(",", resultItem.Data.domainAliases);
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.emailProvider.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.facebook.handle.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.foundedYear.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.geo.city.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.geo.country.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.geo.countryCode.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.geo.lat.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.geo.lng.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.geo.postalCode.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.geo.state.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.geo.stateCode.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.geo.streetName.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.geo.streetNumber.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.geo.subPremise.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.legalName.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.linkedin.handle.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.location.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.metrics.alexaGlobalRank;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.metrics.googleRank;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.metrics.alexaUsRank.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.metrics.annualRevenue.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.metrics.employees.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.metrics.employeesRange.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.metrics.marketCap.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.metrics.raised.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.phone.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.site.h1;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.site.metaAuthor;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.site.metaDescription;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.site.title;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.site.url;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = string.Join(",", resultItem.Data.site.emailAddresses);
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = string.Join(",", resultItem.Data.site.phoneNumbers);
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = string.Join(",", resultItem.Data.tags);
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = string.Join(",", resultItem.Data.tech);
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.ticker.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.timeZone.ToString();
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.twitter.avatar;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.twitter.bio;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.twitter.followers;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.twitter.following;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.twitter.handle;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.twitter.id;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.twitter.location;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.twitter.site;
            //metadata.Properties[ClearBitVocabulary.Organization.Bio] = resultItem.Data.utcOffset.ToString();
        }
    }
}
