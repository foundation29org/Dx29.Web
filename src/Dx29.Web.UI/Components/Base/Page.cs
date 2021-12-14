using System;

using Microsoft.AspNetCore.Components;

using Dx29.Web.UI.Services;

namespace Dx29.Web.UI.Components
{
    public class Page : ComponentEx
    {
        [Inject]
        public AppState AppState { get; set; }

        [Inject]
        public NavigationService Navigation { get; set; }

        public void GoBack(string alternateUrl)
        {
            if (Navigation.CanGoBack)
            {
                Navigation.GoBack();
            }
            else
            {
                NavigateTo(alternateUrl);
            }
        }
    }
}
