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
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using CluedIn.Core;
using CluedIn.Core.Connectors;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.ExternalSearch;
using CluedIn.Core.Providers;
using CluedIn.ExternalSearch.Filters;
using CluedIn.ExternalSearch.Providers.ClearBit.Model;
using CluedIn.ExternalSearch.Providers.ClearBit.Vocabularies;

using RestSharp;
using EntityType = CluedIn.Core.Data.EntityType;

namespace CluedIn.ExternalSearch.Providers.ClearBit
{
    /// <summary>The clear bit external search provider.</summary>
    /// <seealso cref="CluedIn.ExternalSearch.ExternalSearchProviderBase" />
    public partial class ClearBitExternalSearchProvider : ExternalSearchProviderBase, IExternalSearchResultLogger, IExtendedEnricherMetadata, IConfigurableExternalSearchProvider, IExternalSearchProviderWithVerifyConnection
    {
        private static readonly EntityType[] DefaultAcceptedEntityTypes = { EntityType.Organization };

        /**********************************************************************************************************
         * CONSTRUCTORS
         **********************************************************************************************************/

        /// <summary>
        /// Initializes a new instance of the <see cref="ClearBitExternalSearchProvider" /> class.
        /// </summary>
        public ClearBitExternalSearchProvider()
            : base(ExternalSearchProviderPriority.First, Core.Constants.ExternalSearchProviders.ClearBitId, DefaultAcceptedEntityTypes)
        {
        }

        /**********************************************************************************************************
         * METHODS
         **********************************************************************************************************/

        public override bool Accepts(EntityType entityType) => throw new NotSupportedException();

        public override IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request) => throw new NotSupportedException();

        public override IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query) => throw new NotSupportedException();

        public override IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request) => throw new NotSupportedException();

        public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request) => throw new NotSupportedException();

        public override IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request) => throw new NotSupportedException();

        private IEnumerable<IExternalSearchQuery> InternalBuildQueries(ExecutionContext context, IExternalSearchRequest request, IDictionary<string, object> config)
        {
            if (!Accepts(config, request.EntityMetaData.EntityType))
                yield break;

            var existingResults = request.GetQueryResults<CompanyAutocompleteResult>(this).ToList();

            Func<string, bool> domainFilter = value => existingResults.Any(r => string.Equals(r.Data.Domain, value, StringComparison.InvariantCultureIgnoreCase));
            Func<string, bool> nameFilter = value => OrganizationFilters.NameFilter(context, value);

            // Query Input
            var entityType = request.EntityMetaData.EntityType;
            var website = new HashSet<string>();
            var organizationName = new HashSet<string>();
            var emailDomainNames = new HashSet<string>();


            if (config.TryGetValue(Constants.KeyName.WebsiteKey, out var customVocabKeyWebsite) && !string.IsNullOrWhiteSpace(customVocabKeyWebsite?.ToString()))
            {
                website = request.QueryParameters.GetValue<string, HashSet<string>>(config[Constants.KeyName.WebsiteKey].ToString(), new HashSet<string>());
            }
            else
            {
                website = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Website, new HashSet<string>()).ToHashSet();
            }

            if (config.TryGetValue(Constants.KeyName.OrgNameKey, out var customVocabKeyOrgName) && !string.IsNullOrWhiteSpace(customVocabKeyOrgName?.ToString()))
            {
                organizationName = request.QueryParameters.GetValue<string, HashSet<string>>(config[Constants.KeyName.OrgNameKey].ToString(), new HashSet<string>());
            }
            else
            {
                organizationName = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName, new HashSet<string>()).ToHashSet();
            }

            if (config.TryGetValue(Constants.KeyName.EmailDomainKey, out var customVocabKeyEmailDomain) && !string.IsNullOrWhiteSpace(customVocabKeyEmailDomain?.ToString()))
            {
                emailDomainNames = request.QueryParameters.GetValue<string, HashSet<string>>(config[Constants.KeyName.EmailDomainKey].ToString(), new HashSet<string>());
            }
            else
            {
                emailDomainNames = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.EmailDomainNames, new HashSet<string>()).ToHashSet();
            }

            emailDomainNames.AddRange(website.GetDomainNamesFromUris());

            if (!string.IsNullOrEmpty(request.EntityMetaData.Name))
                organizationName.Add(request.EntityMetaData.Name);
            if (!string.IsNullOrEmpty(request.EntityMetaData.DisplayName))
                organizationName.Add(request.EntityMetaData.DisplayName);

            request.EntityMetaData.Aliases.ForEach(a => organizationName.Add(a));

            if (website.Any())
            {
                var values = website;

                foreach (var value in values)
                {
                    Uri uri;

                    if (Uri.TryCreate(value, UriKind.Absolute, out uri))
                    {
                        if (!domainFilter(uri.Host))
                            yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Domain, uri.Host);
                    }
                    else if (!domainFilter(value))
                        yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Domain, value);
                }
            }

            if (emailDomainNames.Any())
            {
                var values = emailDomainNames.SelectMany(v => v.Split(new[] { ",", ";", "|" }, StringSplitOptions.RemoveEmptyEntries)).Select(v => v.ToLowerInvariant()).ToHashSet();

                foreach (var value in values.Where(v => !domainFilter(v)))
                {
                    Uri uri;

                    if (Uri.TryCreate(value, UriKind.Absolute, out uri))
                    {
                        if (!domainFilter(uri.Host))
                            yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Domain, uri.Host);
                    }
                    else if (!domainFilter(value))
                        yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Domain, value);
                }
            }

            if (organizationName.Any())
            {
                var values = organizationName.GetOrganizationNameVariants()
                                             .Select(NameNormalization.Normalize)
                                             .ToHashSet();

                foreach (var value in values.Where(v => !nameFilter(v)))
                    yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Name, value);
            }
        }

        /// <summary>Creates the metadata.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The metadata.</returns>
        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<CompanyAutocompleteResult> resultItem, IExternalSearchRequest request)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateMetadata(metadata, resultItem, request);

            return metadata;
        }

        /// <summary>Populates the metadata.</summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="resultItem">The result item.</param>
        private void PopulateMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<CompanyAutocompleteResult> resultItem, IExternalSearchRequest request)
        {
            metadata.EntityType = request.EntityMetaData.EntityType;
            metadata.Name = request.EntityMetaData.Name;
            metadata.OriginEntityCode = request.EntityMetaData.OriginEntityCode;

            metadata.Properties[ClearBitVocabulary.Organization.Domain] = resultItem.Data.Domain;
            metadata.Properties[ClearBitVocabulary.Organization.Logo] = resultItem.Data.Logo;
        }

        public IEnumerable<EntityType> Accepts(IDictionary<string, object> config, IProvider provider) => Accepts(config);

        private IEnumerable<EntityType> Accepts(IDictionary<string, object> config)
        {
            if (config.TryGetValue(Constants.KeyName.AcceptedEntityType, out var acceptedEntityTypeObj) && acceptedEntityTypeObj is string acceptedEntityType && !string.IsNullOrWhiteSpace(acceptedEntityType))
            {
                // If configured, only accept the configured entity types
                return new EntityType[] { acceptedEntityType };
            }

            // Fallback to default accepted entity types
            return DefaultAcceptedEntityTypes;
        }

        private bool Accepts(IDictionary<string, object> config, EntityType entityTypeToEvaluate)
        {
            var configurableAcceptedEntityTypes = this.Accepts(config).ToArray();

            return configurableAcceptedEntityTypes.Any(entityTypeToEvaluate.Is);
        }

        private ConnectionVerificationResult ConstructVerifyConnectionResponse(IRestResponse response)
        {
            var errorMessageBase = $"{Constants.ProviderName} returned \"{(int)response.StatusCode} {response.StatusDescription}\".";
            if (response.ErrorException != null)
            {
                return new ConnectionVerificationResult(false, $"{errorMessageBase} {(!string.IsNullOrWhiteSpace(response.ErrorException.Message) ? response.ErrorException.Message : "This could be due to breaking changes in the external system")}.");
            }

            if (response.StatusCode is HttpStatusCode.Unauthorized)
            {
                return new ConnectionVerificationResult(false, $"{errorMessageBase} This could be due to invalid API key.");
            }

            var regex = new Regex(@"\<(html|head|body|div|span|img|p\>|a href)", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            var isHtml = regex.IsMatch(response.Content);

            var errorMessage = response.IsSuccessful ? string.Empty
                : string.IsNullOrWhiteSpace(response.Content) || isHtml
                    ? $"{errorMessageBase} This could be due to breaking changes in the external system."
                    : $"{errorMessageBase} {response.Content}.";

            return new ConnectionVerificationResult(response.IsSuccessful, errorMessage);
        }

        public IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            foreach (var externalSearchQuery in InternalBuildQueries(context, request, config))
            {
                yield return externalSearchQuery;
            }
        }

        public IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query, IDictionary<string, object> config, IProvider provider)
        {
            var name = query.QueryParameters.GetValue<string, HashSet<string>>(ExternalSearchQueryParameter.Name.ToString(), new HashSet<string>()).FirstOrDefault();
            var domain = query.QueryParameters.GetValue<string, HashSet<string>>(ExternalSearchQueryParameter.Domain.ToString(), new HashSet<string>()).FirstOrDefault();

            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(domain))
                yield break;

            var client = new RestClient("https://autocomplete.clearbit.com");
            var request = new RestRequest(string.Format("/v1/companies/suggest?query={0}", name ?? domain), Method.GET);

            var response = client.ExecuteTaskAsync<List<CompanyAutocompleteResult>>(request).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                foreach (var result in response.Data.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    yield return new ExternalSearchQueryResult<CompanyAutocompleteResult>(query, result);
                    yield break;
                }
                foreach (var result in response.Data)
                {
                    yield return new ExternalSearchQueryResult<CompanyAutocompleteResult>(query, result);
                    yield break;
                }
            }
            else if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound)
                yield break;
            else if (response.ErrorException != null)
                throw new AggregateException(response.ErrorException.Message, response.ErrorException);
            else
                throw new ApplicationException("Could not execute external search query - StatusCode:" + response.StatusCode);
        }

        public IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            var resultItem = result.As<CompanyAutocompleteResult>();

            var clue = new Clue(request.EntityMetaData.OriginEntityCode, context.Organization);
            clue.Data.OriginProviderDefinitionId = this.Id;

            this.PopulateMetadata(clue.Data.EntityData, resultItem, request);
            this.DownloadPreviewImage(context, resultItem.Data.Logo, clue);

            return new[] { clue };
        }

        public IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            var resultItem = result.As<CompanyAutocompleteResult>();
            return this.CreateMetadata(resultItem, request);
        }

        public IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            return this.DownloadPreviewImageBlob<CompanyAutocompleteResult>(context, result, r => r.Data.Logo);
        }

        public ConnectionVerificationResult VerifyConnection(ExecutionContext context, IReadOnlyDictionary<string, object> config)
        {
            var client = new RestClient("https://autocomplete.clearbit.com");
            var request = new RestRequest(string.Format("/v1/companies/suggest?query=Google"), Method.GET);

            var response = client.ExecuteAsync<List<CompanyAutocompleteResult>>(request).Result;

            return ConstructVerifyConnectionResponse(response);
        }

        public string Icon { get; } = Constants.Icon;
        public string Domain { get; } = Constants.Domain;
        public string About { get; } = Constants.About;
        public AuthMethods AuthMethods { get; } = Constants.AuthMethods;
        public IEnumerable<Control> Properties { get; } = Constants.Properties;
        public Guide Guide { get; } = Constants.Guide;
        public IntegrationType Type { get; } = Constants.IntegrationType;
    }
}
