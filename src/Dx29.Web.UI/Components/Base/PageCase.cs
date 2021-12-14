using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using Dx29.Web.UI.Services;
using Dx29.Web.Models;

namespace Dx29.Web.UI.Components
{
    public class PageCase : Page
    {
        [Parameter]
        public string CaseId { get; set; }

        public PatientModel CurrentCase = null;

        protected override async Task OnParametersSetAsync()
        {
            if (CaseId != null)
            {
                CurrentCase = await AppState.EnsureCaseAsync(CaseId);
                if (CurrentCase == null)
                {
                    NavigateTo("/CaseNotFound");
                }
            }
        }
    }
}
