// -----------------------------------------------------------------------
// <copyright file="KnowledgeRequestNotification.cs" company="Microsoft">
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
    public class KnowledgeRequestNotification
    {
        public EffectiveTime EffectiveTime { get; set; }
        public MainSearchCriteria MainSearchCriteria { get; set; }
        public TaskContext TaskContext { get; set; }
        public SubTopic SubTopic { get; set; }

        public Performer Performer { get; set; }
        public PatientContext PatientContext { get; set; }
        public InformationRecipient InformationRecipient { get; set; }

        public Encounter Encounter { get; set; }
        public Observation Observation { get; set; }
        public LocationOfInterest LocationOfInterest { get; set; }
    }
}
