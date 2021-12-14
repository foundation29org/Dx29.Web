using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Dx29.Data;
using Dx29.Data.Resources;

namespace Dx29.Web.Models
{
    public enum PatientGender
    {
        Unknown,
        Male,
        Female
    }

    public partial class PatientModel
    {
        public PatientModel()
        {
            PatientInfo = new PatientInfoModel();
        }

        public string Id { get; set; }
        public string Status { get; set; }

        public PatientInfoModel PatientInfo { get; set; }

        public int SymptomsCount { get; set; }
        public int PhenotypeReportsCount { get; set; }
        public int GenotypeReportsCount { get; set; }

        public IList<SharedWith> SharedWith { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }

        public bool IsCaseOwner() => Id.StartsWith('c');

        public bool IsShared() => Status == CaseStatus.Shared.ToString();
        public bool IsSharing() => Status == CaseStatus.Sharing.ToString();
        public bool CanBeShared() => Status == CaseStatus.Private.ToString() || Status == CaseStatus.Sharing.ToString() || Status == CaseStatus.Shared.ToString();
    }

    public class PatientInfoModel
    {
        public PatientInfoModel()
        {
            Gender = Dx29.Data.Gender.Unknown.ToString();
            DiseasesIds = new List<string>();
        }
        public PatientInfoModel(string name, PatientGender gender, DateTimeOffset? birthDate, IList<string> diseasesIds = null)
        {
            Name = name;
            Gender = gender.ToString();
            BirthDate = birthDate;
            DiseasesIds = diseasesIds ?? new List<string>();
        }

        [Required(ErrorMessageResourceType = typeof(UISharedResource), ErrorMessageResourceName = "Name_is_required")]
        [StringLength(100, ErrorMessageResourceType = typeof(UISharedResource), ErrorMessageResourceName = "Name_is_too_long.")]
        public string Name { get; set; }

        public string Gender { get; set; }

        [Required(ErrorMessageResourceType = typeof(UISharedResource), ErrorMessageResourceName = "Birthdate_is_required")]
        public DateTimeOffset? BirthDate { get; set; }

        public IList<string> DiseasesIds { get; set; }
    }

    public class CreatePatitentModel
    {
        public CreatePatitentModel()
        {
        }
        public CreatePatitentModel(string name, PatientGender gender, DateTimeOffset? birthDate, IList<string> diseasesIds = null)
        {
            PatientInfo = new PatientInfoModel(name, gender, birthDate, diseasesIds);
        }

        public PatientInfoModel PatientInfo { get; set; }

        public IList<FileModel> Files { get; set; }

        public IList<string> Symptoms { get; set; }
    }
}
