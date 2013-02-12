// -----------------------------------------------------------------------
// <copyright file="AssignedEntity.cs" company="Microsoft">
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
    public class AssignedEntity
    {
        public RepresentedOrganization RepresentedOrganization { get; set; }
        public string Name { get; set; }
        public string CertificateText { get; set; }
    }
}
