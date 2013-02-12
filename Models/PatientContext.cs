// -----------------------------------------------------------------------
// <copyright file="PatientContext.cs" company="Microsoft">
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
    public class PatientContext
    {
        public PatientPerson PatientPerson { get; set; }
        public Age Age { get; set; }
        public AgeGroup AgeGroup { get; set; }
    }
}
