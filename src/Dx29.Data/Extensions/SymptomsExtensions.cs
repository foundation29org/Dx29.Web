using System;
using System.Linq;

using Dx29.Data;

namespace Dx29.Web.Models
{
    static public class SymptomsExtensions
    {
        static public string GetSourcesDesc(this Symptom symptom)
        {
            if (symptom.Sources.Count == 1)
            {
                if (symptom.Sources.First().Name == "Manual")
                {
                    return "Manual";
                }
                return "Report";
            }
            return "Multiple";
        }
    }
}
