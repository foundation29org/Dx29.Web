using System;
using System.Collections.Generic;

namespace Dx29.Data
{
    public enum SharedByCases
    {
        PatientToPhysician,
        BetweenPhysicians,
        ErrorCase
    }

    public enum OwnerRoles
    {
        Patient,
        Physician
    }

    public class SharedCheck
    {
        public bool SharedCheckOk { get; set; }
        public string Action { get; set; }
        public bool InternalSharedUnShared { get; set; }
        public SharedByCases Case { get; set; }
        public InfoSharedChecked Info { get; set; }
    }

    public class InfoSharedChecked
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserRole { get; set; }
        public string PatientName { get; set; }
        public string CaseName { get; set; }
        public string OwnerName { get; set; } = null;
        public string OwnerEmail { get; set; } = null;
        public string UserRequestShareUnShareName { get; set; } = null;
        public string UserRequestShareUnShareEmail { get; set; }
        public string Language { get; set; }
    }
}
