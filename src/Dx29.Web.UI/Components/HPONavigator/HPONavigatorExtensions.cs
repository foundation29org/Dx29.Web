using System;
using System.Linq;

using Dx29.Data;

namespace Dx29.Web.UI
{
    static public class HPONavigatorExtensions
    {
        static public string GetSynonyms(this Term term)
        {
            var item = term.Synonyms;
            if (item != null)
            {
                return String.Join(" | ", item.Select(r => r.Label));
            }
            return null;
        }

        static public string GetPubMeds(this Term term)
        {
            var items = term.PubMeds;
            if (items != null)
            {
                return String.Join(", ", items);
            }
            return null;
        }
    }
}
