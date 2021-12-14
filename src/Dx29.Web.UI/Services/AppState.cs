using System;
using System.Threading.Tasks;

using Dx29.Web.Models;

namespace Dx29.Web.UI.Services
{
    public class AppState
    {
        public AppState(Dx29Client dx29Client, IMessageService messageService)
        {
            Dx29Client = dx29Client;
            MessageService = messageService;
        }

        public Dx29Client Dx29Client { get; }
        public IMessageService MessageService { get; }

        private string _userName = null;
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    MessageService.Send(this, "UserNameChanged", _userName);
                }
            }
        }

        private PatientModel _currentCase = null;
        public PatientModel CurrentCase
        {
            get { return _currentCase; }
            set
            {
                if (_currentCase != value)
                {
                    _currentCase = value;
                    MessageService.Send(this, "CurrentCaseChanged", _currentCase);
                }
            }
        }

        public async Task<PatientModel> EnsureCaseAsync(string caseId)
        {
            CurrentCase = await Dx29Client.GetPatientAsync(caseId);
            return CurrentCase;
        }

        public async Task RefreshCurrentCaseAsync()
        {
            var caseId = CurrentCase?.Id;
            if (caseId != null)
            {
                var model = await Dx29Client.GetPatientAsync(caseId);
                CurrentCase = model;
            }
        }
    }
}
