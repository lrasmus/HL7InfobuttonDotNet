// -----------------------------------------------------------------------
// <copyright file="InformationRecipient.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace HL7InfobuttonAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class InformationRecipient
    {
        public string Value { get; set; }
        public HealthcareProvider HealthCareProvider { get; set; }
        public PatientPerson Patient { get; set; }
        public CodeType LanguageCode { get; set; }
    }
}
