// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearBitVocabulary.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the ClearBitVocabulary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CluedIn.ExternalSearch.Providers.ClearBit.Vocabularies
{
    /// <summary>The clear bit vocabulary.</summary>
    public static class ClearBitVocabulary
    {
        /// <summary>
        /// Initializes static members of the <see cref="ClearBitVocabulary" /> class.
        /// </summary>
        static ClearBitVocabulary()
        {
            Organization    = new ClearBitOrganizationVocabulary();
            Person    = new ClearBitPersonVocabulary();
        }

        /// <summary>Gets the organization.</summary>
        /// <value>The organization.</value>
        public static ClearBitOrganizationVocabulary Organization { get; private set; }
        public static ClearBitPersonVocabulary Person { get; private set; }
    }
}