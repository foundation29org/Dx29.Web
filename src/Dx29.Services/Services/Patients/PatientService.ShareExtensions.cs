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
        private async Task<SharedCheck> ShareUnShareAsync(string userId, string caseId, string email, string action)
        {
            SharedCheck sharedCheck = new SharedCheck();
            sharedCheck.SharedCheckOk = false;
            sharedCheck.Action = action;

            sharedCheck = await completeUserInfo(sharedCheck, userId, caseId);
            if (sharedCheck.Info != null)
            {
                sharedCheck = await completeUserRequestInfo(sharedCheck, email);
                sharedCheck = await completeUserRelationshipsInfo(sharedCheck, email);
            }
            return sharedCheck;
        }

        private async Task<SharedCheck> RequestApprovalAsync(string userId, string caseId, string email, string action)
        {
            SharedCheck sharedCheck = new SharedCheck();
            sharedCheck.SharedCheckOk = false;
            sharedCheck.Action = action;

            sharedCheck = await completeUserInfo(sharedCheck, userId, caseId);
            if (sharedCheck.Info != null)
            {
                sharedCheck = await completeOwnerInfo(sharedCheck, userId, caseId);
                sharedCheck = await completeUserRequestInfo(sharedCheck, email);
                sharedCheck = await completeUserRelationshipsInfo(sharedCheck, email);
            }
            return sharedCheck;
        }

        private async Task<SharedCheck> completeUserInfo(SharedCheck sharedCheck, string userId, string caseId)
        {
            string userEmail = ContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user != null)
            {
                var medicalCase = await MedicalHistoryClient.GetMedicalCaseAsync(userId, caseId);
                var patientName = medicalCase.AsPatientModel().PatientInfo.Name;
                var language = user.Language;
                sharedCheck.Info = new InfoSharedChecked
                {
                    UserName = user.FirstName,
                    UserEmail = user.Email,
                    PatientName = patientName,
                    CaseName = patientName,
                    Language = language
                };
            }
            return sharedCheck;
        }

        private async Task<SharedCheck> completeOwnerInfo(SharedCheck sharedCheck, string userId, string caseId)
        {
            var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
            var ownerId = sharedBy.UserId;
            var ownerCaseId = sharedBy.CaseId;

            string ownerEmail = getOwnerEmail(ownerId);
            if (ownerEmail != null)
            {
                var owner = await _userManager.FindByEmailAsync(ownerEmail);
                if (owner != null)
                {
                    var medicalCase = await MedicalHistoryClient.GetMedicalCaseAsync(ownerId, ownerCaseId);
                    var patientName = medicalCase.AsPatientModel().PatientInfo.Name;

                    sharedCheck.Info.PatientName = patientName;
                    sharedCheck.Info.Language = owner.Language;
                    sharedCheck.Info.OwnerEmail = owner.Email;
                    sharedCheck.Info.OwnerName = owner.FirstName;
                }
            }
            return sharedCheck;
        }

        private string getOwnerEmail(string ownerId)
        {
            string email = null;
            foreach (var user in _userManager.Users)
            {
                string hashId = AccountHashService.GetHash(user.Email);
                if (ownerId == hashId)
                {
                    email = user.Email;
                }
            }
            return email;
        }

        private async Task<SharedCheck> completeUserRequestInfo(SharedCheck sharedCheck, string email)
        {
            var userToShareInternal = await _userManager.FindByEmailAsync(email);
            var language = sharedCheck.Info.Language;
            if ((userToShareInternal != null) && (sharedCheck.Info.OwnerEmail == null))
            {
                language = userToShareInternal.Language;
            }
            sharedCheck.Info.Language = language;

            bool isInternal = false;
            if (userToShareInternal != null)
            {
                sharedCheck.Info.UserRequestShareUnShareName = userToShareInternal.FirstName;
                isInternal = true;
            }
            else
            {
                isInternal = false;

            }
            sharedCheck.Info.UserRequestShareUnShareEmail = email;
            sharedCheck.InternalSharedUnShared = isInternal;

            return sharedCheck;
        }

        private async Task<SharedCheck> completeUserRelationshipsInfo(SharedCheck sharedCheck, string email)
        {
            string userEmail = ContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            var roles = await _userManager.GetRolesAsync(user);

            var userToShareInternal = await _userManager.FindByEmailAsync(email);

            IList<string> rolesToShare = new List<string>();
            if (sharedCheck.InternalSharedUnShared)
            {
                rolesToShare = await _userManager.GetRolesAsync(userToShareInternal);
            }
            else
            {
                rolesToShare.Add("Physician");
            }

            foreach (var roleUser in roles)
            {
                if (roleUser == "Patient")
                {
                    foreach (var roleShared in rolesToShare)
                    {
                        if (roleShared == "Physician")
                        {
                            sharedCheck.Info.UserRole = roleUser;
                            sharedCheck.Case = SharedByCases.PatientToPhysician;
                            sharedCheck.SharedCheckOk = true;
                            break;
                        }
                    }
                    if (sharedCheck.SharedCheckOk)
                    {
                        break;
                    }
                }
                else if (roleUser == "Physician")
                {
                    foreach (var roleShared in rolesToShare)
                    {
                        if (roleShared == "Physician")
                        {
                            sharedCheck.Info.UserRole = roleUser;
                            sharedCheck.Case = SharedByCases.BetweenPhysicians;
                            sharedCheck.SharedCheckOk = true;
                            break;
                        }
                    }
                    if (sharedCheck.SharedCheckOk)
                    {
                        break;
                    }
                }
            }
            if (!sharedCheck.SharedCheckOk)
            {
                sharedCheck.Case = SharedByCases.ErrorCase;
            }
            return sharedCheck;
        }

        private async Task<bool> SendEmailShareCasesAsync(SharedCheck sharedCheck, string userId, string caseId, string message)
        {
            bool sendEmailOk = false;

            string userName = sharedCheck.Info.UserName;
            string userEmail = sharedCheck.Info.UserEmail;
            string userRole = sharedCheck.Info.UserRole;
            string caseName = sharedCheck.Info.CaseName;
            string ownerName = sharedCheck.Info.OwnerName;
            string ownerEmail = sharedCheck.Info.OwnerEmail;
            string userRequestShareUnshareName = sharedCheck.Info.UserRequestShareUnShareName;
            string userRequestShareUnshareEmail = sharedCheck.Info.UserRequestShareUnShareEmail;


            string language = sharedCheck.Info.Language;

            if ((sharedCheck.Action.ToLower() == "create") && (!IsSharedCase(caseId)) || ((sharedCheck.Action.ToLower() == "accept") && (!IsSharedCase(caseId))))
            {
                if (sharedCheck.InternalSharedUnShared)
                {
                    sendEmailOk = await _emailHelper.SendShareInternalAsync(userName, userEmail, caseName, userRole, ownerEmail, userRequestShareUnshareName, userRequestShareUnshareEmail, language, message);
                }
                else
                {
                    sendEmailOk = await _emailHelper.SendShareExternalAsync(userName, userEmail, caseName, userRole, ownerEmail, userRequestShareUnshareName, userRequestShareUnshareEmail, language, message);
                }
            }
            else if ((sharedCheck.Action.ToLower() == "create") && (IsSharedCase(caseId)))
            {
                if (sharedCheck.InternalSharedUnShared)
                {
                    sendEmailOk = await _emailHelper.SendRequestApprovalInternalAsync(userName, userEmail, caseName, userRole, ownerEmail, userRequestShareUnshareName, userRequestShareUnshareEmail, language, message);
                }
                else
                {
                    sendEmailOk = await _emailHelper.SendRequestApprovalExternalAsync(userName, userEmail, caseName, userRole, ownerEmail, userRequestShareUnshareName, userRequestShareUnshareEmail, language, message); ;
                }
            }
            else if ((sharedCheck.Action.ToLower() == "revoke") && (!(IsSharedCase(caseId))))
            {
                // Check previous accepted: requestUserEmail has the case in shared  state and send email unshared
                bool previousOwnerShared = await CheckPreviousOnOwnerStateSharedAsync(userId, caseId, userRequestShareUnshareEmail);
                if (previousOwnerShared)
                {
                    if (sharedCheck.InternalSharedUnShared)
                    {
                        sendEmailOk = await _emailHelper.SendUnShareInternalAsync(userName, userEmail, caseName, userRole, ownerEmail, userRequestShareUnshareName, userRequestShareUnshareEmail, language, true, message);
                    }
                    else
                    {
                        sendEmailOk = await _emailHelper.SendUnShareExternalAsync(userName, userEmail, caseName, userRole, ownerEmail, userRequestShareUnshareName, userRequestShareUnshareEmail, language, true, message);
                    }
                }
                else
                {
                    return true;
                }

            }
            else if ((sharedCheck.Action.ToLower() == "delete") && (!IsSharedCase(caseId)))
            {
                sendEmailOk = true;
            }
            else if ((sharedCheck.Action.ToLower() == "delete") && (IsSharedCase(caseId)))
            {
                bool previousOwnerShared = await CheckPreviousOnOwnerStateSharedAsync(userId, caseId, userRequestShareUnshareEmail);
                bool previousOwnerPending = await CheckPreviousOnOwnerStatePendingAsync(userId, caseId, userRequestShareUnshareEmail);

                if (previousOwnerShared || previousOwnerPending)
                {
                    if (sharedCheck.InternalSharedUnShared)
                    {
                        sendEmailOk = await _emailHelper.SendInformationToOwnerInternalAsync(userName, userEmail, caseName, userRole, ownerEmail, userRequestShareUnshareName, userRequestShareUnshareEmail, language, message);
                    }
                    else
                    {
                        sendEmailOk = await _emailHelper.SendInformationToOwnerExternalAsync(userName, userEmail, caseName, userRole, ownerEmail, userRequestShareUnshareName, userRequestShareUnshareEmail, language, message);
                    }
                    userName = ownerName;
                    userEmail = ownerEmail;

                    if (sharedCheck.InternalSharedUnShared)
                    {
                        sendEmailOk = await _emailHelper.SendUnShareInternalAsync(userName, userEmail, caseName, userRole, ownerEmail, userRequestShareUnshareName, userRequestShareUnshareEmail, language, false, message);
                    }
                    else
                    {
                        sendEmailOk = await _emailHelper.SendUnShareExternalAsync(userName, userEmail, caseName, userRole, ownerEmail, userRequestShareUnshareName, userRequestShareUnshareEmail, language, false, message);
                    }
                }
                else
                {
                    return true;
                }

            }
            return sendEmailOk;
        }

        private async Task<bool> CheckPreviousOnOwnerStateSharedAsync(string userId, string caseId, string userRequestShareUnshareEmail)
        {
            Console.WriteLine("CheckPreviousStateSharedAsync");
            Console.WriteLine(userRequestShareUnshareEmail);
            bool previousShared = false;

            var ownerId = userId;
            var ownerCaseId = caseId;
            if (IsSharedCase(caseId))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                if (sharedBy != null)
                {
                    ownerId = sharedBy.UserId;
                    ownerCaseId = sharedBy.CaseId;
                }
                else
                {
                    ownerId = null;
                    ownerCaseId = null;
                } 
            }
            
            Console.WriteLine(ownerId);
            if (ownerId != null)
            {
                var medicalCaseOwner = await MedicalHistoryClient.GetMedicalCaseAsync(ownerId, ownerCaseId);
                if (medicalCaseOwner != null)
                {
                    SharedWith medicalCaseSharedWithRequest = medicalCaseOwner.SharedWith.Where(r => r.UserId == userRequestShareUnshareEmail).FirstOrDefault();
                    Console.WriteLine(medicalCaseSharedWithRequest);
                    if (medicalCaseSharedWithRequest != null)
                    {
                        Console.WriteLine(medicalCaseSharedWithRequest.IsShared());
                        if (medicalCaseSharedWithRequest.IsShared())
                        {
                            previousShared = true;
                        }
                    }
                }
            }
            return previousShared;
        }
        private async Task<bool> CheckPreviousOnOwnerStatePendingAsync(string userId, string caseId, string userRequestShareUnshareEmail)
        {
            bool previousPending = false;

            var ownerId = userId;
            var ownerCaseId = caseId;
            if (IsSharedCase(caseId))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                if (sharedBy != null)
                {
                    ownerId = sharedBy.UserId;
                    ownerCaseId = sharedBy.CaseId;
                }
                else
                {
                    ownerId = null;
                    ownerCaseId = null;
                }
            }

            Console.WriteLine(ownerId);
            if(ownerId != null)
            {
                var medicalCaseOwner = await MedicalHistoryClient.GetMedicalCaseAsync(ownerId, ownerCaseId);
                if (medicalCaseOwner != null)
                {
                    SharedWith medicalCaseSharedWithRequest = medicalCaseOwner.SharedWith.Where(r => r.UserId == userRequestShareUnshareEmail).FirstOrDefault();
                    Console.WriteLine(medicalCaseSharedWithRequest);
                    if (medicalCaseSharedWithRequest != null)
                    {
                        Console.WriteLine(medicalCaseSharedWithRequest.IsPending());
                        if (medicalCaseSharedWithRequest.IsShared())
                        {
                            previousPending = true;
                        }
                    }
                }
            }           
            return previousPending;
        }
    }

}
