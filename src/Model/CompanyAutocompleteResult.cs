// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyAutocompleteResult.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the CompanyAutocompleteResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.ClearBit.Model
{
    /// <summary>The company autocomplete result.</summary>
    public class CompanyAutocompleteResult
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }
    }

    public class Name
    {
        public string fullName { get; set; }
        public string givenName { get; set; }
        public string familyName { get; set; }
    }

    public class Geo
    {
        public string city { get; set; }
        public string state { get; set; }
        public string stateCode { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class Employment
    {
        public string domain { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string role { get; set; }
        public string seniority { get; set; }
    }

    public class Facebook
    {
        public string handle { get; set; }
    }

    public class Github
    {
        public string handle { get; set; }
        public string id { get; set; }
        public string avatar { get; set; }
        public string company { get; set; }
        public string blog { get; set; }
        public string followers { get; set; }
        public string following { get; set; }
    }

    public class Twitter
    {
        public string handle { get; set; }
        public string id { get; set; }
        public string bio { get; set; }
        public string followers { get; set; }
        public string following { get; set; }
        public string statuses { get; set; }
        public string favorites { get; set; }
        public string location { get; set; }
        public string site { get; set; }
        public string avatar { get; set; }
    }

    public class Linkedin
    {
        public string handle { get; set; }
    }

    public class Googleplus
    {
        public object handle { get; set; }
    }

    public class Aboutme
    {
        public string handle { get; set; }
        public string bio { get; set; }
        public object avatar { get; set; }
    }

    public class Url
    {
        public string value { get; set; }
        public string title { get; set; }
    }

    public class Avatar
    {
        public string url { get; set; }
        public string type { get; set; }
    }

    public class Gravatar
    {
        public string handle { get; set; }
        public List<Url> urls { get; set; }
        public string avatar { get; set; }
        public List<Avatar> avatars { get; set; }
    }

    public class Person
    {
        public string id { get; set; }
        public Name name { get; set; }
        public string email { get; set; }
        public object gender { get; set; }
        public string location { get; set; }
        public string timeZone { get; set; }
        public string utcOffset { get; set; }
        public Geo geo { get; set; }
        public string bio { get; set; }
        public string site { get; set; }
        public string avatar { get; set; }
        public Employment employment { get; set; }
        public Facebook facebook { get; set; }
        public Github github { get; set; }
        public Twitter twitter { get; set; }
        public Linkedin linkedin { get; set; }
        public Googleplus googleplus { get; set; }
        public Aboutme aboutme { get; set; }
        public Gravatar gravatar { get; set; }
        public bool fuzzy { get; set; }
        public bool emailProvider { get; set; }
        public string indexedAt { get; set; }
    }

    public class Site
    {
        public string url { get; set; }
        public string title { get; set; }
        public string h1 { get; set; }
        public string metaDescription { get; set; }
        public string metaAuthor { get; set; }
        public List<object> phoneNumbers { get; set; }
        public List<object> emailAddresses { get; set; }
    }

    public class Category
    {
        public string sector { get; set; }
        public string industryGroup { get; set; }
        public string industry { get; set; }
        public string subIndustry { get; set; }
    }

    public class Geo2
    {
        public object streetNumber { get; set; }
        public object streetName { get; set; }
        public object subPremise { get; set; }
        public object city { get; set; }
        public object postalCode { get; set; }
        public object state { get; set; }
        public object stateCode { get; set; }
        public object country { get; set; }
        public object countryCode { get; set; }
        public object lat { get; set; }
        public object lng { get; set; }
    }

    public class Facebook2
    {
        public object handle { get; set; }
    }

    public class Linkedin2
    {
        public object handle { get; set; }
    }

    public class Twitter2
    {
        public string handle { get; set; }
        public string id { get; set; }
        public string bio { get; set; }
        public string followers { get; set; }
        public string following { get; set; }
        public string location { get; set; }
        public string site { get; set; }
        public string avatar { get; set; }
    }

    public class Crunchbase
    {
        public object handle { get; set; }
    }

    public class Metrics
    {
        public object alexaUsRank { get; set; }
        public string alexaGlobalRank { get; set; }
        public string googleRank { get; set; }
        public object employees { get; set; }
        public object employeesRange { get; set; }
        public object marketCap { get; set; }
        public object raised { get; set; }
        public object annualRevenue { get; set; }
    }

    public class Company
    {
        public string id { get; set; }
        public string name { get; set; }
        public object legalName { get; set; }
        public string domain { get; set; }
        public List<object> domainAliases { get; set; }
        public string url { get; set; }
        public Site site { get; set; }
        public Category category { get; set; }
        public List<string> tags { get; set; }
        public string description { get; set; }
        public object foundedYear { get; set; }
        public object location { get; set; }
        public object timeZone { get; set; }
        public object utcOffset { get; set; }
        public Geo2 geo { get; set; }
        public string logo { get; set; }
        public Facebook2 facebook { get; set; }
        public Linkedin2 linkedin { get; set; }
        public Twitter2 twitter { get; set; }
        public Crunchbase crunchbase { get; set; }
        public bool emailProvider { get; set; }
        public string type { get; set; }
        public object ticker { get; set; }
        public object phone { get; set; }
        public Metrics metrics { get; set; }
        public string indexedAt { get; set; }
        public List<string> tech { get; set; }
    }

    public class ClearbitResponse
    {
        public Person person { get; set; }
        public Company company { get; set; }
    }
}
