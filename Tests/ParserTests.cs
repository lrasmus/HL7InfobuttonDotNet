using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HL7InfobuttonAPI.Models;
using HL7InfobuttonAPI;

namespace Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ParseRequest_Full()
        {
            const string testString =
                @"knowledgeRequestNotification.effectiveTime.v=20110113"
                + "&holder.assignedEntity.n=hAEN&holder.assignedEntity.certificateText=hAECT"
                + "&assignedAuthorizedPerson.id.root=aAPIR"
                + "&representedOrganization.id.root=rOIR"
                + "&assignedEntity.representedOrganization.n=aERON"
                + "&patientPerson.administrativeGenderCode.c=pPAGC&patientPerson.administrativeGenderCode.dn=pPAGCDN"
                + "&age.v.v=aVV&age.v.u=aVU&ageGroup.v.c=aGVC&ageGroup.v.cs=aGVCS&ageGroup.v.dn=aGVDN"
                + "&taskContext.c.c=tCCC&taskContext.c.dn=tCCDN"
                + "&subTopic.v.c=sTVC&subTopic.v.cs=sTVCS&subTopic.v.dn=sTVDN&subTopic.v.ot=sTVOT"
                + "&mainSearchCriteria.v.c=mSCVC&mainSearchCriteria.v.cs=mSCVCS&mainSearchCriteria.v.dn=mSCVDN&mainSearchCriteria.v.ot=mSCVOT"
                + "&severityObservation.interpretationCode.c=sOICC&severityObservation.interpretationCode.cs=sOICCS&severityObservation.interpretationCode.dn=sOICDN"
                + "&informationRecipient=iR&informationRecipient.healthCareProvider.c.c=iRHCPCC&informationRecipient.healthCareProvider.c.cs=iRHCPCCS&informationRecipient.healthCareProvider.c.dn=iRHCPCDN&informationRecipient.languageCode.c=iRLCC"
                + "&performer=p&performer.healthCareProvider.c.c=pHCPCC&performer.healthCareProvider.c.cs=pHCPCCS&performer.healthCareProvider.c.dn=pHCPCDN&performer.languageCode.c=pLCC"
                + "&encounter.c.c=eCC&encounter.c.cs=eCCS&encounter.c.dn=eCDN"
                + "&observation.c.c=oCC&observation.c.cs=oCCS&observation.c.dn=oCDN&observation.v.c=oVC&observation.v.cs=oVCS&observation.v.dn=oVDN&observation.v.v=oVV&observation.v.u=oVU"
                + "&locationOfInterest.addr.postalCode=lOIAPC&locationOfInterest.addr.city=lOIAC&locationOfInterest.addr.state=lOIAS"
                + "&serviceDeliveryLocation.id.root=sDLIR";
            var parser = new Parser();
            KnowledgeRequestNotification requestNotification = parser.ParseRequest(testString);
            Assert.AreEqual("20110113", requestNotification.EffectiveTime.Value);
            Assert.AreEqual("pPAGC", requestNotification.PatientContext.PatientPerson.AdministrativeGenderCode.Code);
            Assert.AreEqual("pPAGCDN", requestNotification.PatientContext.PatientPerson.AdministrativeGenderCode.DisplayName);
            Assert.AreEqual("aVV", requestNotification.PatientContext.Age.Value.Value);
            Assert.AreEqual("aVU", requestNotification.PatientContext.Age.Value.Unit);
            Assert.AreEqual("aGVC", requestNotification.PatientContext.AgeGroup.Value.Code);
            Assert.AreEqual("aGVCS", requestNotification.PatientContext.AgeGroup.Value.CodeSystem);
            Assert.AreEqual("aGVDN", requestNotification.PatientContext.AgeGroup.Value.DisplayName);
            Assert.AreEqual("tCCC", requestNotification.TaskContext.Code.Code);
            Assert.AreEqual("tCCDN", requestNotification.TaskContext.Code.DisplayName);
            Assert.AreEqual("sTVC", requestNotification.SubTopic.Value.Code);
            Assert.AreEqual("sTVCS", requestNotification.SubTopic.Value.CodeSystem);
            Assert.AreEqual("sTVDN", requestNotification.SubTopic.Value.DisplayName);
            Assert.AreEqual("sTVOT", requestNotification.SubTopic.Value.OriginalText);
            Assert.AreEqual("mSCVC", requestNotification.MainSearchCriteria.Value.Code);
            Assert.AreEqual("mSCVCS", requestNotification.MainSearchCriteria.Value.CodeSystem);
            Assert.AreEqual("mSCVDN", requestNotification.MainSearchCriteria.Value.DisplayName);
            Assert.AreEqual("mSCVOT", requestNotification.MainSearchCriteria.Value.OriginalText);
            Assert.AreEqual("iR", requestNotification.InformationRecipient.Value);
            Assert.AreEqual("iRHCPCC", requestNotification.InformationRecipient.HealthCareProvider.Code.Code);
            Assert.AreEqual("iRHCPCCS", requestNotification.InformationRecipient.HealthCareProvider.Code.CodeSystem);
            Assert.AreEqual("iRHCPCDN", requestNotification.InformationRecipient.HealthCareProvider.Code.DisplayName);
            Assert.AreEqual("iRLCC", requestNotification.InformationRecipient.LanguageCode.Code);
            Assert.AreEqual("pHCPCC", requestNotification.Performer.HealthCareProvider.Code.Code);
            Assert.AreEqual("pHCPCCS", requestNotification.Performer.HealthCareProvider.Code.CodeSystem);
            Assert.AreEqual("pHCPCDN", requestNotification.Performer.HealthCareProvider.Code.DisplayName);
            Assert.AreEqual("pLCC", requestNotification.Performer.LanguageCode.Code);
            Assert.AreEqual("eCC", requestNotification.Encounter.Code.Code);
            Assert.AreEqual("eCCS", requestNotification.Encounter.Code.CodeSystem);
            Assert.AreEqual("eCDN", requestNotification.Encounter.Code.DisplayName);
            Assert.AreEqual("oCC", requestNotification.Observation.Code.Code);
            Assert.AreEqual("oCCS", requestNotification.Observation.Code.CodeSystem);
            Assert.AreEqual("oCDN", requestNotification.Observation.Code.DisplayName);
            Assert.AreEqual("oVC", requestNotification.Observation.Value.Code);
            Assert.AreEqual("oVCS", requestNotification.Observation.Value.CodeSystem);
            Assert.AreEqual("oVDN", requestNotification.Observation.Value.DisplayName);
            Assert.AreEqual("oVU", requestNotification.Observation.Value.Unit);
            Assert.AreEqual("oVV", requestNotification.Observation.Value.Value);
            Assert.AreEqual("lOIAPC", requestNotification.LocationOfInterest.Addr.PostalCode);
            Assert.AreEqual("lOIAC", requestNotification.LocationOfInterest.Addr.City);
            Assert.AreEqual("lOIAS", requestNotification.LocationOfInterest.Addr.State);
            Assert.AreEqual("sDLIR", requestNotification.Encounter.ServiceDeliveryLocation.ID.Root);
            Assert.AreEqual("sOICC", requestNotification.MainSearchCriteria.SeverityObservation.InterpretationCode.Code);
            Assert.AreEqual("sOICCS", requestNotification.MainSearchCriteria.SeverityObservation.InterpretationCode.CodeSystem);
            Assert.AreEqual("sOICDN", requestNotification.MainSearchCriteria.SeverityObservation.InterpretationCode.DisplayName);

            // FOR LATER (DEPRECATED): &subTopic.c.c=sTCC&subTopic.c.cs=sTCCS&subTopic.c.dn=sTCDN
            // FOR LATER (DEPRECATED): mainSearchCriteria.c.c=mSCCC&mainSearchCriteria.c.cs=mSCCCS&mainSearchCriteria.c.dn=mSCCDN&mainSearchCriteria.c.ot=mSCCOT

            // TODO: WRITE TESTS FOR SCENARIOS WHERE THE CASING IS DIFFERENT.  I.E. "healthcareprovider" and "HealthcareProvider" should match "healthCareProvider"

            // TODO: Look at Infobutton_IG.pdf for examples of tests to write
        }

        [TestMethod]
        public void ParseRequest_Deprecated()
        {
            const string testString =
                @"subTopic.c.c=sTCC&subTopic.c.cs=sTCCS&subTopic.c.dn=sTCDN"
                + "&mainSearchCriteria.c.c=mSCCC&mainSearchCriteria.c.cs=mSCCCS&mainSearchCriteria.c.dn=mSCCDN&mainSearchCriteria.c.ot=mSCCOT";
            var parser = new Parser();
            KnowledgeRequestNotification requestNotification = parser.ParseRequest(testString);
            Assert.AreEqual("sTCC", requestNotification.SubTopic.Value.Code);
            Assert.AreEqual("sTCCS", requestNotification.SubTopic.Value.CodeSystem);
            Assert.AreEqual("sTCDN", requestNotification.SubTopic.Value.DisplayName);
            Assert.AreEqual("mSCCC", requestNotification.MainSearchCriteria.Value.Code);
            Assert.AreEqual("mSCCCS", requestNotification.MainSearchCriteria.Value.CodeSystem);
            Assert.AreEqual("mSCCDN", requestNotification.MainSearchCriteria.Value.DisplayName);
            Assert.AreEqual("mSCCOT", requestNotification.MainSearchCriteria.Value.OriginalText);
        }

        [TestMethod]
        public void ParseRequest_MixValidAndDeprecated()
        {
            const string testString =
                @"subTopic.c.c=sTCC&subTopic.c.cs=sTCCS&subTopic.c.dn=sTCDN&subTopic.v.c=sTVC&subTopic.v.cs=sTVCS&subTopic.v.dn=sTVDN"
                + "&mainSearchCriteria.c.c=mSCCC&mainSearchCriteria.c.cs=mSCCCS&mainSearchCriteria.c.dn=mSCCDN&mainSearchCriteria.c.ot=mSCCOT&mainSearchCriteria.v.c=mSCVC&mainSearchCriteria.v.cs=mSCVCS&mainSearchCriteria.v.dn=mSCVDN&mainSearchCriteria.v.ot=mSCVOT";
            var parser = new Parser();
            KnowledgeRequestNotification requestNotification = parser.ParseRequest(testString);
            Assert.AreEqual("sTVC", requestNotification.SubTopic.Value.Code);
            Assert.AreEqual("sTVCS", requestNotification.SubTopic.Value.CodeSystem);
            Assert.AreEqual("sTVDN", requestNotification.SubTopic.Value.DisplayName);
            Assert.AreEqual("mSCVC", requestNotification.MainSearchCriteria.Value.Code);
            Assert.AreEqual("mSCVCS", requestNotification.MainSearchCriteria.Value.CodeSystem);
            Assert.AreEqual("mSCVDN", requestNotification.MainSearchCriteria.Value.DisplayName);
            Assert.AreEqual("mSCVOT", requestNotification.MainSearchCriteria.Value.OriginalText);
        }
    }
}
