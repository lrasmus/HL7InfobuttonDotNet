// -----------------------------------------------------------------------
// <copyright file="Value.cs" company="Microsoft">
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
    public class Value
    {
        public string Code { get; set; }
        public string CodeSystem { get; set; }
        public string DisplayName { get; set; }
        public string OriginalText { get; set; }
    }
}
