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
    public class ClearBitDomainExternalSearchProvider : ExternalSearchProviderBase
    {
        /**********************************************************************************************************
         * CONSTRUCTORS
         **********************************************************************************************************/

        /// <summary>
        /// Initializes a new instance of the <see cref="ClearBitDomainExternalSearchProvider" /> class.
        /// </summary>
        public ClearBitDomainExternalSearchProvider()
            : base(Constants.ExternalSearchProviders.ClearBitDomainId, EntityType.Person)
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

            #region TODO ClearBitDomainExternalSearchProvider.BuildQueries(ExecutionContext context, IExternalSearchRequest request) is disabled in code ... review

            //if (!this.Accepts(request.EntityMetaData.EntityType))
            //    yield break;

            //var existingResults = request.GetQueryResults<Person>(this).ToList();

            //Func<string, bool> domainFilter = value => existingResults.Any(r => string.Equals(r.Data.id, value, StringComparison.InvariantCultureIgnoreCase));
            //Func<string, bool> nameFilter = value => string.IsNullOrEmpty(value) || value.IsGuid() || value.IsNumber() || string.Equals(value, "Microsoft Office User", StringComparison.InvariantCultureIgnoreCase);

            //// Query Input
            //var entityType = request.EntityMetaData.EntityType;
            //var website = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Website, null);
            //var organizationName = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName, new HashSet<string>()).ToHashSet();
            //var emailDomainNames = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.EmailDomainNames, null);

            //if (!string.IsNullOrEmpty(request.EntityMetaData.Name))
            //    organizationName.Add(request.EntityMetaData.Name);
            //if (!string.IsNullOrEmpty(request.EntityMetaData.DisplayName))
            //    organizationName.Add(request.EntityMetaData.DisplayName);

            //request.EntityMetaData.Aliases.ForEach(a => organizationName.Add(a));

            //if (website != null)
            //{
            //    var values = website;

            //    foreach (var value in values)
            //    {
            //        Uri uri;

            //        if (Uri.TryCreate(value, UriKind.Absolute, out uri) && !domainFilter(uri.Host))
            //            yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Name, uri.Host);
            //        else if (!domainFilter(value))
            //            yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Name, value);
            //    }
            //}
            //else if (emailDomainNames != null)
            //{
            //    var values = emailDomainNames.SelectMany(v => v.Split(new[] { ",", ";", "|" }, StringSplitOptions.RemoveEmptyEntries)).ToHashSet();

            //    foreach (var value in values.Where(v => !domainFilter(v)))
            //        yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Name, value);
            //}
            //else if (organizationName != null)
            //{
            //    var values = organizationName;

            //    foreach (var value in values.Where(v => !nameFilter(v)))
            //        yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Name, value);
            //}

            #endregion
        }

        /// <summary>Executes the search.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <returns>The results.</returns>
        public override IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query)
        {
            var name = query.QueryParameters[ExternalSearchQueryParameter.Name].FirstOrDefault();

            if (string.IsNullOrEmpty(name))
                yield break;

            var sharedApiToken = ConfigurationManager.AppSettings["Providers.ExternalSearch.ClearBit.ApiToken"];

            var client = new RestClient("https://person-stream.clearbit.com");
            var request = new RestRequest(string.Format("/v2/combined/find?email={0}", name), Method.GET);
            request.AddHeader("Authorization", "Bearer " + sharedApiToken);

            var response = client.ExecuteTaskAsync<ClearbitResponse>(request).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                yield return new ExternalSearchQueryResult<Person>(query, response.Data.person);
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
            var resultItem = result.As<Person>();

            var code = this.GetOriginEntityCode(resultItem);

            var clue = new Clue(code, context.Organization);
            clue.Data.OriginProviderDefinitionId = this.Id;

            this.PopulateMetadata(clue.Data.EntityData, resultItem);
            this.DownloadPreviewImage(context, resultItem.Data.avatar, clue);

            return new[] { clue };
        }

        /// <summary>Gets the primary entity metadata.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The primary entity metadata.</returns>
        public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<Person>();
            return this.CreateMetadata(resultItem);
        }

        /// <summary>Gets the preview image.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The preview image.</returns>
        public override IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            return this.DownloadPreviewImageBlob<Person>(context, result, r => r.Data.avatar);
        }

        /// <summary>Creates the metadata.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The metadata.</returns>
        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<Person> resultItem)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateMetadata(metadata, resultItem);

            return metadata;
        }

        /// <summary>Gets the origin entity code.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The origin entity code.</returns>
        private EntityCode GetOriginEntityCode(IExternalSearchQueryResult<Person> resultItem)
        {
            return new EntityCode(EntityType.Person, CodeOrigin.CluedIn.CreateSpecific("clearBit"), resultItem.Data.id);
        }

        /// <summary>Populates the metadata.</summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="resultItem">The result item.</param>
        private void PopulateMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<Person> resultItem)
        {
            var code = this.GetOriginEntityCode(resultItem);

            metadata.EntityType = EntityType.Person;
            metadata.Name = resultItem.Data.name.fullName;
            metadata.OriginEntityCode = code;

            metadata.Codes.Add(code);
            metadata.Codes.Add(new EntityCode(EntityType.Person, CodeOrigin.CluedIn.CreateSpecific("email"), resultItem.Data.email));

            metadata.Properties[ClearBitVocabulary.Person.FirstName] = resultItem.Data.name.givenName;
            metadata.Properties[ClearBitVocabulary.Person.LastName] = resultItem.Data.name.familyName;
            metadata.Properties[ClearBitVocabulary.Person.Avatar] = resultItem.Data.avatar;
            metadata.Properties[ClearBitVocabulary.Person.Bio] = resultItem.Data.bio;
            metadata.Properties[ClearBitVocabulary.Person.Email] = resultItem.Data.email;
            metadata.Properties[ClearBitVocabulary.Person.IndexedAt] = resultItem.Data.indexedAt;
            metadata.Properties[ClearBitVocabulary.Person.Location] = resultItem.Data.location;
            metadata.Properties[ClearBitVocabulary.Person.Site] = resultItem.Data.site;
            metadata.Properties[ClearBitVocabulary.Person.TimeZone] = resultItem.Data.timeZone;
            metadata.Properties[ClearBitVocabulary.Person.UtcOffset] = resultItem.Data.utcOffset;
            metadata.Properties[ClearBitVocabulary.Person.AboutMeBio] = resultItem.Data.aboutme.bio;
            metadata.Properties[ClearBitVocabulary.Person.AboutMe] = resultItem.Data.aboutme.handle;
            metadata.Properties[ClearBitVocabulary.Person.AboutMeAvatar] = resultItem.Data.aboutme.avatar.ToString();
            metadata.Properties[ClearBitVocabulary.Person.EmailProvider] = resultItem.Data.emailProvider.ToString();
            metadata.Properties[ClearBitVocabulary.Person.EmploymentDomain] = resultItem.Data.employment.domain;
            metadata.Properties[ClearBitVocabulary.Person.Employment] = resultItem.Data.employment.name;
            metadata.Properties[ClearBitVocabulary.Person.EmploymentRole] = resultItem.Data.employment.role;
            metadata.Properties[ClearBitVocabulary.Person.EmploymentSeniority] = resultItem.Data.employment.seniority;
            metadata.Properties[ClearBitVocabulary.Person.EmploymentTitle] = resultItem.Data.employment.title;
            metadata.Properties[ClearBitVocabulary.Person.Facebook] = resultItem.Data.facebook.handle;
            metadata.Properties[ClearBitVocabulary.Person.Fuzzy] = resultItem.Data.fuzzy.ToString();
            metadata.Properties[ClearBitVocabulary.Person.Gender] = resultItem.Data.gender.ToString();
            metadata.Properties[ClearBitVocabulary.Person.City] = resultItem.Data.geo.city;
            metadata.Properties[ClearBitVocabulary.Person.Country] = resultItem.Data.geo.country;
            metadata.Properties[ClearBitVocabulary.Person.CountryCode] = resultItem.Data.geo.countryCode;
            metadata.Properties[ClearBitVocabulary.Person.Lat] = resultItem.Data.geo.lat;
            metadata.Properties[ClearBitVocabulary.Person.Long] = resultItem.Data.geo.lng;
            metadata.Properties[ClearBitVocabulary.Person.State] = resultItem.Data.geo.state;
            metadata.Properties[ClearBitVocabulary.Person.StateCode] = resultItem.Data.geo.stateCode;
            metadata.Properties[ClearBitVocabulary.Person.GithubAvatar] = resultItem.Data.github.avatar;
            metadata.Properties[ClearBitVocabulary.Person.GithubBlog] = resultItem.Data.github.blog;
            metadata.Properties[ClearBitVocabulary.Person.GithubCompany] = resultItem.Data.github.company;
            metadata.Properties[ClearBitVocabulary.Person.GithubFollowers] = resultItem.Data.github.followers;
            metadata.Properties[ClearBitVocabulary.Person.GithubFollowing] = resultItem.Data.github.following;
            metadata.Properties[ClearBitVocabulary.Person.GithubHandle] = resultItem.Data.github.handle;
            metadata.Properties[ClearBitVocabulary.Person.Github] = resultItem.Data.github.id;
            metadata.Properties[ClearBitVocabulary.Person.GooglePlus] = resultItem.Data.googleplus.handle.ToString();
            metadata.Properties[ClearBitVocabulary.Person.GravatarAvatar] = resultItem.Data.gravatar.avatar;
            metadata.Properties[ClearBitVocabulary.Person.Gravatar] = resultItem.Data.gravatar.handle;
            metadata.Properties[ClearBitVocabulary.Person.GravatarAvatars] = string.Join(",", resultItem.Data.gravatar.avatars);
            metadata.Properties[ClearBitVocabulary.Person.GravatarUrls] = string.Join(",", resultItem.Data.gravatar.urls);
            metadata.Properties[ClearBitVocabulary.Person.LinkedIn] = resultItem.Data.linkedin.handle;
            metadata.Properties[ClearBitVocabulary.Person.TwitterAvatar] = resultItem.Data.twitter.avatar;
            metadata.Properties[ClearBitVocabulary.Person.TwitterBio] = resultItem.Data.twitter.bio;
            metadata.Properties[ClearBitVocabulary.Person.TwitterFavourites] = resultItem.Data.twitter.favorites;
            metadata.Properties[ClearBitVocabulary.Person.TwitterFollowers] = resultItem.Data.twitter.followers;
            metadata.Properties[ClearBitVocabulary.Person.TwitterFollowing] = resultItem.Data.twitter.following;
            metadata.Properties[ClearBitVocabulary.Person.TwitterHandle] = resultItem.Data.twitter.handle;
            metadata.Properties[ClearBitVocabulary.Person.Twitter] = resultItem.Data.twitter.id;
            metadata.Properties[ClearBitVocabulary.Person.TwitterLocation] = resultItem.Data.twitter.location;
            metadata.Properties[ClearBitVocabulary.Person.TwitterSite] = resultItem.Data.twitter.site;
            metadata.Properties[ClearBitVocabulary.Person.TwitterStatuses] = resultItem.Data.twitter.statuses;
        }
    }
}
