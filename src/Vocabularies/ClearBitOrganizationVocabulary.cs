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
    public class ClearBitOrganizationVocabulary : SimpleVocabulary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClearBitOrganizationVocabulary"/> class.
        /// </summary>
        public ClearBitOrganizationVocabulary()
        {
            this.VocabularyName = "ClearBit Autocomplete Organization";
            this.KeyPrefix      = "clearbit.organization";
            this.KeySeparator   = ".";
            this.Grouping       = EntityType.Organization;

            this.Domain         = this.Add(new VocabularyKey("domain", VocabularyKeyDataType.Uri));
            this.Logo           = this.Add(new VocabularyKey("logo", VocabularyKeyDataType.Uri, VocabularyKeyVisibility.Hidden));

            this.AddMapping(this.Domain, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Website);
        }

        public VocabularyKey Domain { get; protected set; }
        public VocabularyKey Logo { get; protected set; }
    }
}
