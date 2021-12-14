using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

namespace Dx29.Web.UI.Components
{
    partial class Wizard
    {
        [Parameter]
        public RenderFragment WizardSteps { get; set; }

        [Parameter]
        public RenderFragment WizardStepContents { get; set; }

        [Parameter]
        public EventCallback<WizardNavigationArgs> OnNavigation { get; set; }

        [Parameter]
        public EventCallback OnFinish { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        public int Steps { get; set; } = 0;
        public int Index { get; set; } = 0;

        private string GetState()
        {
            if (Index == 0) return "first";
            if (Index == Steps - 1) return "last";
            return "between";
        }

        private async void Next()
        {
            int newIndex = Math.Min(Steps - 1, Index + 1);
            if (await ValidateStepAsync(Index, newIndex))
            {
                Index = newIndex;
            }
            StateHasChanged();
        }

        private async void Prev()
        {
            int newIndex = Math.Max(0, Index - 1);
            if (await ValidateStepAsync(Index, newIndex))
            {
                Index = newIndex;
            }
            StateHasChanged();
        }

        private async void Finish()
        {
            await OnFinish.InvokeAsync();
        }

        private async Task<bool> ValidateStepAsync(int currentIndex, int newIndex)
        {
            var args = new WizardNavigationArgs(currentIndex, newIndex);
            await OnNavigation.InvokeAsync(args);
            return !args.Cancel;
        }
    }

    public class WizardNavigationArgs
    {
        public WizardNavigationArgs(int currentIndex, int newIndex)
        {
            CurrentIndex = currentIndex;
            NewIndex = newIndex;
            Cancel = false;
        }

        public int CurrentIndex { get; }
        public int NewIndex { get; }
        public bool IForward => CurrentIndex < NewIndex;
        public bool IsBackward => CurrentIndex > NewIndex;
        public bool Cancel { get; set; }
    }
}
