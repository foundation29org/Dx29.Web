using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Dx29.Web.UI.Services
{
    public class NavigationService : IDisposable
    {
        public NavigationService(NavigationManager navigationManager)
        {
            NavigationStack = new Stack<string>();
            NavigationManager = navigationManager;
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        public Stack<string> NavigationStack { get; }

        public NavigationManager NavigationManager { get; }

        public void ClearNavigation()
        {
            NavigationStack.Clear();
            NavigationStack.Push(NavigationManager.Uri);
        }

        public void NavigateTo(string uri, bool forceLoad = false)
        {
            NavigationManager.NavigateTo(uri, forceLoad);
        }

        public void GoBack(bool forceLoad = false)
        {
            if (CanGoBack)
            {
                NavigationStack.Pop();
                string uri = NavigationStack.Pop();
                while ((uri.Contains("no-track", StringComparison.OrdinalIgnoreCase) || uri == NavigationManager.Uri) && NavigationStack.Count > 0)
                {
                    uri = NavigationStack.Pop();
                }
                NavigationManager.NavigateTo(uri, forceLoad);
            }
        }

        public void TryGoBack(string alternateHRef)
        {
            if (CanGoBack)
            {
                GoBack();
            }
            else
            {
                RemoveCurrent();
                NavigateTo(alternateHRef);
            }
        }

        public void RemoveCurrent()
        {
            if (NavigationStack.Count > 0)
            {
                NavigationStack.Pop();
            }
        }

        private void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            string uri = e.Location;
            if (uri.Contains("ClearNavigation", StringComparison.OrdinalIgnoreCase))
            {
                NavigationStack.Clear();
                uri = uri.Replace("?ClearNavigation", "").Replace("&ClearNavigation", "");
            }
            NavigationStack.Push(uri);
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }

        public bool CanGoBack => NavigationStack.Count > 1;
    }
}
