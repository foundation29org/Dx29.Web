using System;
using System.Collections.Generic;
using System.Linq;

namespace Dx29.Data
{
    public class ClinicalTrials
    {
        public ClinicalTrialsInfo FullStudiesResponse { get; set; }
    }

    public class ClinicalTrialsInfo
    {
        public ClinicalTrialsInfo()
        {
            FullStudies = new List<FullStudiesInfo>();
        }
        public List<FullStudiesInfo> FullStudies { get; set; }
    }

    public class FullStudiesInfo
    {
        public Study Study { get; set; }
    }

    public class Study
    {
        public ProtocolSection ProtocolSection { get; set; }
    }
    public class ProtocolSection
    {
        public IdentificationModule IdentificationModule { get; set; }
        public StatusModule StatusModule { get; set; }
        
        public SponsorCollaboratorsModule SponsorCollaboratorsModule { get; set; }

        public ContactsLocationsModule ContactsLocationsModule { get; set; }
    }

    public class IdentificationModule
    {
        public string NCTId { get; set; }
        public string NCTIdLink => $"https://clinicaltrials.gov/ct2/show/{NCTId}";
        public Organization Organization { get; set; }
        public string BriefTitle { get; set; }
    }

    public class Organization
    {
        public string OrgFullName { get; set; }
    }

    public class StatusModule
    {
        public string OverallStatus { get; set; }
        public string StudyFirstSubmitDate { get; set; }
    }
    
    public class SponsorCollaboratorsModule
    {
        public ResponsibleParty ResponsibleParty { get; set; }
    }

    public class ResponsibleParty
    {
        public string ResponsiblePartyInvestigatorFullName { get; set; }
    }
    public class ContactsLocationsModule
    {
        public LocationList LocationList { get; set; }
        public CentralContactList CentralContactList { get; set; }
    }

    public class LocationList
    {
        public LocationList()
        {
            Location = new List<Locations>();
        }
        public List<Locations> Location { get; set; }
    }

    public class Locations
    {
        public string LocationCountry { get; set; }
    }

    public class CentralContactList
    {
        public CentralContactList()
        {
            CentralContact = new List<CentralContact>();
        }
        public List<CentralContact> CentralContact { get; set; }
    }

    public class CentralContact
    {
        public string CentralContactName { get; set; }
        public string CentralContactEMail { get; set; }
    }
}
