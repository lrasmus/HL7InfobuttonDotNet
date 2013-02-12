// -----------------------------------------------------------------------
// <copyright file="Observation.cs" company="Microsoft">
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
    public class Observation
    {
        public CodeType Code { get; set; }
        public ObservationValue Value { get; set; }
    }
}
