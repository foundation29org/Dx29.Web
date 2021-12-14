using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

using Dx29.Data;
using Dx29.Web.Models;
using System.Linq;

namespace Dx29.Web.Services
{
    public partial class PatientService
    {
        public async Task<MedicalCase> ShareMedicalCaseAsync(string userId, string caseId, string email, string message, string action)
        {
            SharedCheck conditionsSharedChecked = await CheckConditionsForShareAsync(userId, caseId, email, action);
            if (conditionsSharedChecked.SharedCheckOk)
            {
                var medicalCase = await MedicalHistoryClient.GetMedicalCaseAsync(userId, caseId);
                if (medicalCase != null)
                {
                    bool sendEmail = await SendEmailShareCasesAsync(conditionsSharedChecked, userId, caseId, message);
                    if (sendEmail)
                    {
                        return await MedicalHistoryClient.ShareMedicalCaseAsync(userId, caseId, email, action);
                    }
                }
            }
            else
            {
                if (!(await CanShareAsync(userId, caseId, email, action)))
                {
                    return await MedicalHistoryClient.ShareMedicalCaseAsync(userId, caseId, email, action);
                }
            }
            return null;
        }

        public async Task StopSharingMedicalCaseAsync(string userId, string caseId, string email)
        {
            var medicalCase = await MedicalHistoryClient.GetMedicalCaseAsync(userId, caseId);
            if (medicalCase != null)
            {
                await MedicalHistoryClient.StopSharingMedicalCaseAsync(userId, caseId, email);
            }
        }

        private async Task<SharedCheck> CheckConditionsForShareAsync(string userId, string caseId, string email, string action)
        {
            SharedCheck sharedCheck = new SharedCheck();
            sharedCheck.Action = action;
            sharedCheck.SharedCheckOk = false;
            if (IsSharedCase(caseId))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                string ownerId = sharedBy.UserId;
                string ownerCaseId = sharedBy.CaseId;

                bool canShareUnshare = await CanShareAsync(ownerId, ownerCaseId, email, action);
                if (canShareUnshare)
                { 
                    if ((action == "create") || (action == "delete"))
                    {
                        return await RequestApprovalAsync(userId, caseId, email, action);
                    }
                }
            }
            else
            {
                bool canShareUnshare = await CanShareAsync(userId,caseId,email,action);
                if (canShareUnshare)
                {
                    return await ShareUnShareAsync(userId, caseId, email, action);
                }
            }
            return sharedCheck;
        }
        

        private static bool IsSharedCase(string caseId)
        {
            return caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<bool> CanShareAsync(string userId, string caseId, string email, string action)
        {
            bool canShareUnshare = true;
            if (action == "create")
            {
                canShareUnshare = !(await OwnerHasCaseAsync(userId, caseId, email));
            }
            return canShareUnshare;
        }

        private async Task<bool> OwnerHasCaseAsync(string userId, string caseId, string email)
        {
            var medicalCase = await MedicalHistoryClient.GetMedicalCaseAsync(userId, caseId);
            if (medicalCase != null)
            {
                SharedWith OwnerHasCase = medicalCase.SharedWith.Where(r => r.UserId == email).FirstOrDefault();
                if(OwnerHasCase != null)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
