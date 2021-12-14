using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Dx29.Web.UI.Components
{
    public class KnownIcon : ComponentBase
    {
        public Dictionary<string, string> IconNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Patients", "flaticon-users-1" },
            { "Dashboard", "flaticon2-layers" },
            { "Symptoms", "fas fa-hand-holding-medical" },
            { "Diagnosis", "flaticon2-cardiogram" },
            { "Notes", "flaticon2-file-1" },
            { "Reports", "flaticon2-crisp-icons" },
            { "Genotype", "fas fa-dna" },
            { "Summary", "fas fa-paperclip" },
            { "Share", "fas fa-share-alt" },
            { "Alert", "flaticon2-warning" },
            { "Timeline", "flaticon-calendar-3"},

            { "Symptoms-Item", "far fa-check-circle" },
            { "Diagnosis-Item", "icon-nm flaticon2-cardiogram" },
            { "Notes-Item", "icon-lg flaticon2-file-1" },
            { "Reports-Item", "icon-lg flaticon2-crisp-icons" },
            { "Genotype-Item", "icon-lg fas fa-dna" },
        };

        [Parameter]
        public string CssClass { get; set; }

        [Parameter]
        public string Style { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string Type { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "i");
            builder.AddAttribute(1, "class", GetClass());
            var iconStyle = GetIconStyle();
            if (Style != null || iconStyle != null)
            {
                builder.AddAttribute(2, "style", $"{iconStyle}{Style}");
            }
            builder.CloseElement();
        }

        private string GetClass()
        {
            string str = null;
            switch (Type?.ToLowerInvariant())
            {
                case "menu":
                    str = $"menu-icon {GetIconNameClass(Name)}";
                    break;
                case "header":
                    str = $"icon-2x {GetIconNameClass(Name)} {GetColorClass()}";
                    break;
                case "section":
                    str = $"icon-xl {GetIconNameClass(Name)} {GetColorClass()}";
                    break;
                case "item":
                    str = $"{GetIconNameClass(Name)}";
                    break;
                default:
                    str = $"{Type} {GetIconNameClass(Name)}";
                    break;
            }

            if (CssClass != null)
            {
                str = $"{str} {CssClass}";
            }
            return str;
        }

        private string GetIconNameClass(string name)
        {
            if (IconNames.TryGetValue($"{name}-{Type}", out string iconName))
            {
                return iconName;
            }
            if (IconNames.TryGetValue(name, out iconName))
            {
                return iconName;
            }
            return name;
        }

        private string GetColorClass()
        {
            switch (Name?.ToLowerInvariant())
            {
                case "symptoms":
                    return "text-danger opacity-80";
                case "diagnosis":
                    return "text-success";
                case "summary":
                    return "text-success";
                case "share":
                    return "text-success";
                case "alert":
                    return "text-danger";

                case "notes":
                    return "";
                case "reports":
                    return "text-primary opacity-80";
                case "genotype":
                    return "text-info opacity-70";

                default:
                    return "";
            }
        }

        private string GetIconStyle()
        {
            switch (Name.ToLowerInvariant())
            {
                case "patients":
                    return "color: rosybrown;";

                default:
                    return null;
            }
        }
    }
}
