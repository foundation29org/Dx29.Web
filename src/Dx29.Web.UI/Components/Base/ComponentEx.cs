using System;

using Microsoft.AspNetCore.Components;

using Dx29.Web.Services;

namespace Dx29.Web.UI.Components
{
    public class ComponentEx : ComponentBase, IDisposable
    {
        [Inject]
        public LocalizationService Localize { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected override void OnInitialized()
        {
            Localize.CultureChanged += OnCultureChanged;
        }

        private void OnCultureChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }

        public void NavigateTo(string uri, bool forceLoad = false)
        {
            NavigationManager.NavigateTo(uri, forceLoad);
        }

        virtual public void Dispose()
        {
            Localize.CultureChanged -= OnCultureChanged;
        }
    }
}
