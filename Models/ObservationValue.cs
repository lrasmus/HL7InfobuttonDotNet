﻿// -----------------------------------------------------------------------
// <copyright file="ObservationValue.cs" company="Microsoft">
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
    public class ObservationValue : Value
    {
        public string Value { get; set; }
        public string Unit { get; set; }
    }
}
