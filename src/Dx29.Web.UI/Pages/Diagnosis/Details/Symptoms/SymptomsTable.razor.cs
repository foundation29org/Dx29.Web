using System;

using Dx29.Data;

namespace Dx29.Web.UI.Components
{
    partial class SymptomsTable
    {
        private string GetTextInfo(DiffSymptom item)
        {
            string text = "";
            if (item.HasPatient && item.HasDisease)
            {
                if (item.Relationship == "Equal")
                {
                    text = Localize["Symptom observed in the patient and the disease."];
                }
                else if (item.Relationship == "Successor")
                {
                    text = Localize["Symptom deduced in the patient because they also present {0}.", item.RelatedName];

                }
                else if (item.Relationship == "Predecessor")
                {
                    text = Localize["Symptom observed in patient and disease."];
                }
            }
            else if (item.HasPatient && !item.HasDisease)
            {
                text = Localize["Symptom observed in the patient."];
            }
            else if (!item.HasPatient && item.HasDisease)
            {
                text = Localize["Symptom observed in the disease."];
            }
            return text;
        }

        private string GetGroupLabel(string groupType, string groupName, bool isSorted)
        {
            if (groupName == null)
            {
                if (isSorted)
                {
                    return Localize["Unknown"];
                }
                return Localize[String.Format("Unknown {0}", groupType)];
            }
            return Localize[groupName];
        }
    }
}
