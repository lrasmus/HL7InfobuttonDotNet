// -----------------------------------------------------------------------
// <copyright file="Encounter.cs" company="Microsoft">
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
    public class Encounter
    {
        public CodeType Code { get; set; }
        public ServiceDeliveryLocation ServiceDeliveryLocation { get; set; }
    }
}
