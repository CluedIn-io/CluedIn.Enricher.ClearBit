// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearBitOrganizationVocabulary.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the ClearBitOrganizationVocabulary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.ClearBit.Vocabularies
{
    /// <summary>The clear bit organization vocabulary.</summary>
    /// <seealso cref="CluedIn.Core.Data.Vocabularies.SimpleVocabulary" />
    public class ClearBitPersonVocabulary : SimpleVocabulary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClearBitPersonVocabulary"/> class.
        /// </summary>
        public ClearBitPersonVocabulary()
        {
            this.VocabularyName = "ClearBit Autocomplete Person";
            this.KeyPrefix      = "clearbit.person";
            this.KeySeparator   = ".";
            this.Grouping       = EntityType.Person;

            this.Domain         = this.Add(new VocabularyKey("domain", VocabularyKeyDataType.Uri));
            this.Logo           = this.Add(new VocabularyKey("logo", VocabularyKeyDataType.Uri, VocabularyKeyVisibility.Hidden));


            this.FirstName = this.Add(new VocabularyKey("name"));
            this.LastName = this.Add(new VocabularyKey("lastName"));
            this.Avatar = this.Add(new VocabularyKey("avatar"));
            this.Bio = this.Add(new VocabularyKey("bio"));
            this.Email = this.Add(new VocabularyKey("email"));
            this.IndexedAt = this.Add(new VocabularyKey("indexedAt"));
            this.Location = this.Add(new VocabularyKey("location"));
            this.Site = this.Add(new VocabularyKey("site"));
            this.TimeZone = this.Add(new VocabularyKey("timeZone"));
            this.UtcOffset = this.Add(new VocabularyKey("utcOffset"));
            this.AboutMe = this.Add(new VocabularyKey("aboutMe"));
            this.EmailProvider = this.Add(new VocabularyKey("emailProvider"));
            this.Employment = this.Add(new VocabularyKey("employment"));
            this.Facebook = this.Add(new VocabularyKey("facebook"));
            this.Fuzzy = this.Add(new VocabularyKey("fuzzy"));
            this.Gender = this.Add(new VocabularyKey("gender"));
            this.Geo = this.Add(new VocabularyKey("geo"));
            this.Github = this.Add(new VocabularyKey("github"));
            this.GooglePlus = this.Add(new VocabularyKey("googlePlu"));
            this.Gravatar = this.Add(new VocabularyKey("gravatar"));
            this.LinkedIn = this.Add(new VocabularyKey("linkedIn"));
            this.Twitter = this.Add(new VocabularyKey("twitter"));
            this.AboutMeBio = this.Add(new VocabularyKey("aboutMeBio"));
            this.AboutMeAvatar = this.Add(new VocabularyKey("aboutMeAvatar"));
            this.EmploymentDomain = this.Add(new VocabularyKey("employmentDomain"));
            this.EmploymentRole = this.Add(new VocabularyKey("employmentRole"));
            this.EmploymentSeniority = this.Add(new VocabularyKey("employmentSeniority"));
            this.EmploymentTitle = this.Add(new VocabularyKey("employmentTitle"));
            this.City = this.Add(new VocabularyKey("city"));
            this.Country = this.Add(new VocabularyKey("country"));
            this.CountryCode = this.Add(new VocabularyKey("countryCode"));
            this.Lat = this.Add(new VocabularyKey("lat"));
            this.Long = this.Add(new VocabularyKey("long"));
            this.State = this.Add(new VocabularyKey("state"));
            this.StateCode = this.Add(new VocabularyKey("stateCode"));
            this.GithubAvatar = this.Add(new VocabularyKey("githubAvatar"));
            this.GithubBlog = this.Add(new VocabularyKey("githubBlog"));
            this.GithubCompany = this.Add(new VocabularyKey("githubCompany"));
            this.GithubFollowers = this.Add(new VocabularyKey("githubFollowers"));
            this.GithubFollowing = this.Add(new VocabularyKey("githubFollowing"));
            this.GithubHandle = this.Add(new VocabularyKey("githubHandle"));
            this.GravatarAvatar = this.Add(new VocabularyKey("gravatarAvatar"));
            this.GravatarAvatars = this.Add(new VocabularyKey("gravatarAvatars"));
            this.TwitterAvatar = this.Add(new VocabularyKey("twitterAvatar"));
            this.GravatarUrls = this.Add(new VocabularyKey("GravatarsUrls"));
            this.TwitterBio = this.Add(new VocabularyKey("twitterBio"));
            this.TwitterFavourites = this.Add(new VocabularyKey("twitterFavourites"));
            this.TwitterFollowers = this.Add(new VocabularyKey("twitterFollowers"));
            this.TwitterFollowing = this.Add(new VocabularyKey("twitterFollowing"));
            this.TwitterHandle = this.Add(new VocabularyKey("twitterHandle"));
            this.TwitterLocation = this.Add(new VocabularyKey("twitterLocation"));
            this.TwitterSite = this.Add(new VocabularyKey("twitterSite"));
            this.TwitterStatuses = this.Add(new VocabularyKey("twitterStatuses"));

        }

        public VocabularyKey Domain { get; protected set; }
        public VocabularyKey Logo { get; protected set; }
        public VocabularyKey FirstName { get; set; }
        public VocabularyKey LastName { get; set; }
        public VocabularyKey Avatar { get; set; }
        public VocabularyKey Bio { get; set; }
        public VocabularyKey Email { get; set; }
        public VocabularyKey IndexedAt { get; set; }
        public VocabularyKey Location { get; set; }
        public VocabularyKey Site { get; set; }
        public VocabularyKey TimeZone { get; set; }
        public VocabularyKey UtcOffset { get; set; }
        public VocabularyKey AboutMe { get; set; }
        public VocabularyKey EmailProvider { get; set; }
        public VocabularyKey Employment { get; set; }
        public VocabularyKey Facebook { get; set; }
        public VocabularyKey Fuzzy { get; set; }
        public VocabularyKey Gender { get; set; }
        public VocabularyKey Geo { get; set; }
        public VocabularyKey Github { get; set; }
        public VocabularyKey GooglePlus { get; set; }
        public VocabularyKey Gravatar { get; set; }
        public VocabularyKey LinkedIn { get; set; }
        public VocabularyKey Twitter { get; set; }
        public VocabularyKey AboutMeBio { get; set; }
        public VocabularyKey AboutMeAvatar { get; set; }
        public VocabularyKey EmploymentDomain { get; set; }
        public VocabularyKey EmploymentRole { get; set; }
        public VocabularyKey EmploymentSeniority { get; set; }
        public VocabularyKey EmploymentTitle { get; set; }
        public VocabularyKey City { get; set; }
        public VocabularyKey Country { get; set; }
        public VocabularyKey CountryCode { get; set; }
        public VocabularyKey Lat { get; set; }
        public VocabularyKey Long { get; set; }
        public VocabularyKey State { get; set; }
        public VocabularyKey StateCode { get; set; }
        public VocabularyKey GithubAvatar { get; set; }
        public VocabularyKey GithubBlog { get; set; }
        public VocabularyKey GithubCompany { get; set; }
        public VocabularyKey GithubFollowers { get; set; }
        public VocabularyKey GithubFollowing { get; set; }
        public VocabularyKey GithubHandle { get; set; }
        public VocabularyKey GravatarAvatar { get; set; }
        public VocabularyKey GravatarAvatars { get; set; }
        public VocabularyKey TwitterAvatar { get; set; }
        public VocabularyKey GravatarUrls { get; set; }
        public VocabularyKey TwitterBio { get; set; }
        public VocabularyKey TwitterFavourites { get; set; }
        public VocabularyKey TwitterFollowers { get; set; }
        public VocabularyKey TwitterFollowing { get; set; }
        public VocabularyKey TwitterHandle { get; set; }
        public VocabularyKey TwitterLocation { get; set; }
        public VocabularyKey TwitterSite { get; set; }
        public VocabularyKey TwitterStatuses { get; set; }
    }
}
