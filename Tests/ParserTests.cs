using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HL7InfobuttonAPI.Models;
using HL7InfobuttonAPI;
using System.Collections.Specialized;

namespace Tests
{
    [TestClass]
    public class ParserTests
    {
        private const string FullMessageString =
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

        [TestMethod]
        public void ParseRequest_Full()
        {
            var parser = new Parser();
            KnowledgeRequestNotification requestNotification = parser.ParseRequest(FullMessageString);
            ValidateFullRequestNotification(requestNotification);

            // TODO: WRITE TESTS FOR SCENARIOS WHERE THE CASING IS DIFFERENT.  I.E. "healthcareprovider" and "HealthcareProvider" should match "healthCareProvider"

            // TODO: Look at Infobutton_IG.pdf for examples of tests to write
        }

        // Test that the order of the parameters doesn't matter by going in reverse of the full API call
        [TestMethod]
        public void ParseRequest_FullReverse()
        {
            var parser = new Parser();
            NameValueCollection parameters = parser.SplitStringParameters(FullMessageString);
            var reverseParameters = new NameValueCollection();
            foreach (var parameter in parameters.AllKeys.Reverse())
            {
                reverseParameters.Add(parameter, parameters[parameter]);
            }

            KnowledgeRequestNotification requestNotification = parser.ParseRequest(reverseParameters);
            ValidateFullRequestNotification(requestNotification);
        }

        [TestMethod]
        public void ParseRequest_CaseAgnostic()
        {
            var parser = new Parser();
            NameValueCollection parameters = parser.SplitStringParameters(FullMessageString);
            var caseParameters = new NameValueCollection();
            foreach (var parameter in parameters.AllKeys.Reverse())
            {
                caseParameters.Add(parameter.ToLower(), parameters[parameter]);
            }
            KnowledgeRequestNotification requestNotification = parser.ParseRequest(caseParameters);
            ValidateFullRequestNotification(requestNotification);

            parameters = parser.SplitStringParameters(FullMessageString);
            caseParameters = new NameValueCollection();
            foreach (var parameter in parameters.AllKeys.Reverse())
            {
                caseParameters.Add(parameter.ToUpper(), parameters[parameter]);
            }
            requestNotification = parser.ParseRequest(caseParameters);
            ValidateFullRequestNotification(requestNotification);
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

        /// <summary>
        /// Validates all of the values in what we consider a "full" request (meaning it specifies all parameters)
        /// </summary>
        /// <param name="requestNotification"></param>
        private void ValidateFullRequestNotification(KnowledgeRequestNotification requestNotification)
        {
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
        }

        [TestMethod]
        public void HL7Guide_Example1()
        {
            const string Example1 = "knowledgeRequestNotification.effectiveTime.v=20060706001023&holder.assignedEntity.n=Organizationusername&holder.assignedEntity.certificateText=organizationpassword&patientPerson.administrativeGenderCode.c=M&patientPerson.administrativeGenderCode.dn=Male&"
            + "&age.v.v=77&age.v.u=a&ageGroup.v.c=D000368&taskContext.c.c=PROBLISTREV&subTopic.v.c=Q000628&subTopic.v.cs=2.16.840.1.113883.6.177&subTopic.v.dn=therapy&mainSearchCriteria.v.c=D018410&mainSearchCriteria.v.cs=2.16.840.1.113883.6.177"
            + "&mainSearchCriteria.v.dn=Bacterial+Pneumonia&mainSearchCriteria.v.ot=Pneumonia";

            var parser = new Parser();
            var requestNotification = parser.ParseRequest(Example1);
            Assert.AreEqual("20060706001023", requestNotification.EffectiveTime.Value);
            Assert.AreEqual("Organizationusername", requestNotification.Holder.AssignedEntity.Name);
            Assert.AreEqual("organizationpassword", requestNotification.Holder.AssignedEntity.CertificateText);
            Assert.AreEqual("M", requestNotification.PatientContext.PatientPerson.AdministrativeGenderCode.Code);
            Assert.AreEqual("Male", requestNotification.PatientContext.PatientPerson.AdministrativeGenderCode.DisplayName);
            Assert.AreEqual("77", requestNotification.PatientContext.Age.Value.Value);
            Assert.AreEqual("a", requestNotification.PatientContext.Age.Value.Unit);
            Assert.AreEqual("D000368", requestNotification.PatientContext.AgeGroup.Value.Code);
            Assert.AreEqual("PROBLISTREV", requestNotification.TaskContext.Code.Code);
            Assert.AreEqual("Q000628", requestNotification.SubTopic.Value.Code);
            Assert.AreEqual("2.16.840.1.113883.6.177", requestNotification.SubTopic.Value.CodeSystem);
            Assert.AreEqual("therapy", requestNotification.SubTopic.Value.DisplayName);
            Assert.AreEqual("D018410", requestNotification.MainSearchCriteria.Value.Code);
            Assert.AreEqual("2.16.840.1.113883.6.177", requestNotification.MainSearchCriteria.Value.CodeSystem);
            Assert.AreEqual("Bacterial Pneumonia", requestNotification.MainSearchCriteria.Value.DisplayName);
            Assert.AreEqual("Pneumonia", requestNotification.MainSearchCriteria.Value.OriginalText);
        }

        [TestMethod]
        public void HL7Guide_Example2()
        {
            const string Example2 = "knowledgeRequestNotification.effectiveTime.v=20060706001023&holder.assignedEntity.n=OrganizationUsername"
             + "&holder.assignedEntity.certificateText=organizationpassword&patientPerson.administrativeGenderCode.c=F&patientPerson.administrativeGenderCode.cs=aGCCS"
             + "&age.v.v=8&age.v.u=a&taskContext.c.c=MEDLISTREV"
             + "&performer=PROV&informationRecipient=PAT&performer.languageCode.c=en&informationRecipient.languageCode.c=es"
             + "&performer.healthCareProvider.c.c=163W00000X&performer.healthCareProvider.c.dn=Registered Nurse&mainSearchCriteria.v.c=49502-693-03"
             + "&mainSearchCriteria.v.cs=2.16.840.1.113883.6.69&mainSearchCriteria.v.dn=Albuterol+sulfate+inhalation+solution+1.25+mg&mainSearchCriteria.v.ot=Albuterol+sulfate";

            var parser = new Parser();
            var requestNotification = parser.ParseRequest(Example2);
            Assert.AreEqual("20060706001023", requestNotification.EffectiveTime.Value);
            Assert.AreEqual("OrganizationUsername", requestNotification.Holder.AssignedEntity.Name);
            Assert.AreEqual("organizationpassword", requestNotification.Holder.AssignedEntity.CertificateText);
            Assert.AreEqual("F", requestNotification.PatientContext.PatientPerson.AdministrativeGenderCode.Code);
            Assert.AreEqual("aGCCS", requestNotification.PatientContext.PatientPerson.AdministrativeGenderCode.CodeSystem);
            Assert.AreEqual("8", requestNotification.PatientContext.Age.Value.Value);
            Assert.AreEqual("a", requestNotification.PatientContext.Age.Value.Unit);
            Assert.AreEqual("MEDLISTREV", requestNotification.TaskContext.Code.Code);
            Assert.AreEqual("PROV", requestNotification.Performer.Value);
            Assert.AreEqual("PAT", requestNotification.InformationRecipient.Value);
            Assert.AreEqual("en", requestNotification.Performer.LanguageCode.Code);
            Assert.AreEqual("es", requestNotification.InformationRecipient.LanguageCode.Code);
            Assert.AreEqual("163W00000X", requestNotification.Performer.HealthCareProvider.Code.Code);
            Assert.AreEqual("Registered Nurse", requestNotification.Performer.HealthCareProvider.Code.DisplayName);
            Assert.AreEqual("49502-693-03", requestNotification.MainSearchCriteria.Value.Code);
            Assert.AreEqual("2.16.840.1.113883.6.69", requestNotification.MainSearchCriteria.Value.CodeSystem);
            Assert.AreEqual("Albuterol sulfate inhalation solution 1.25 mg", requestNotification.MainSearchCriteria.Value.DisplayName);
            Assert.AreEqual("Albuterol sulfate", requestNotification.MainSearchCriteria.Value.OriginalText);
        }

        [TestMethod]
        public void HL7Guide_Example3a()
        {
            const string Example3a = "knowledgeRequestNotification.effectiveTime.v=20120706001023&"
                + "patientPerson.administrativeGenderCode.c=F&age.v.v=28&age.v.u=a&taskContext.c.c=MEDOE&performer=PROV&performer.healthCareProvider.c.c=163W00000X&"
                + "mainSearchCriteria.v.c=38341003&mainSearchCriteria.v.cs=2.16.840.1.113883.6.96&mainSearchCriteria.v.dn=Hypertensive%20disorder&"
                + "mainSearchCriteria.v.ot=Systemic+arterial+hypertension&observation.v.c=77386008&observation.v.cs=2.16.840.1.113883.6.96&subtopic.v.c=Q000628&"
                + "subtopic.v.cs=2.16.840.1.113883.6.177&subtopic.v.dn=therapy";

            var parser = new Parser();
            var requestNotification = parser.ParseRequest(Example3a);
            Assert.AreEqual("20120706001023", requestNotification.EffectiveTime.Value);
            Assert.AreEqual("F", requestNotification.PatientContext.PatientPerson.AdministrativeGenderCode.Code);
            Assert.AreEqual("28", requestNotification.PatientContext.Age.Value.Value);
            Assert.AreEqual("a", requestNotification.PatientContext.Age.Value.Unit);
            Assert.AreEqual("MEDOE", requestNotification.TaskContext.Code.Code);
            Assert.AreEqual("PROV", requestNotification.Performer.Value);
            Assert.AreEqual("163W00000X", requestNotification.Performer.HealthCareProvider.Code.Code);
            Assert.AreEqual("38341003", requestNotification.MainSearchCriteria.Value.Code);
            Assert.AreEqual("2.16.840.1.113883.6.96", requestNotification.MainSearchCriteria.Value.CodeSystem);
            Assert.AreEqual("Hypertensive disorder", requestNotification.MainSearchCriteria.Value.DisplayName);
            Assert.AreEqual("Systemic arterial hypertension", requestNotification.MainSearchCriteria.Value.OriginalText);
            Assert.AreEqual("77386008", requestNotification.Observation.Value.Code);
            Assert.AreEqual("2.16.840.1.113883.6.96", requestNotification.Observation.Value.CodeSystem);
            Assert.AreEqual("Q000628", requestNotification.SubTopic.Value.Code);
            Assert.AreEqual("2.16.840.1.113883.6.177", requestNotification.SubTopic.Value.CodeSystem);
            Assert.AreEqual("therapy", requestNotification.SubTopic.Value.DisplayName);
        }

        public void HL7Guide_Example3b()
        {
            const string Example3b = "knowledgeRequestNotification.effectiveTime.v=20120706001023& patientPerson.administrativeGenderCode.c=F&"
                + "age.v.v=67&age.v.u=a&taskContext.c.c=MEDOE&performer=PROV&performer.healthCareProvider.c.c=200000000X&encounter.c.c=AMB&"
                + "mainSearchCriteria.v.c=197379&mainSearchCriteria.v.cs=2.16.840.1.113883.6.88&mainSearchCriteria.v.dn=Atenolol 100 mg Oral Tablet&mainSearchCriteria.v.ot=Atenolol&"
                + "observation.v.c=102811001&observation.v.cs=2.16.840.1.113883.6.96&observation.v.v=65&observation.v.u=mL/min&subtopic.v.c=Q000008&"
                + "subtopic.v.cs=2.16.840.1.113883.6.177&subtopic.v.dn=administration+and+dosage";

            var parser = new Parser();
            var requestNotification = parser.ParseRequest(Example3b);
            Assert.AreEqual("20120706001023", requestNotification.EffectiveTime.Value);
            Assert.AreEqual("F", requestNotification.PatientContext.PatientPerson.AdministrativeGenderCode.Code);
            Assert.AreEqual("67", requestNotification.PatientContext.Age.Value.Value);
            Assert.AreEqual("a", requestNotification.PatientContext.Age.Value.Unit);
            Assert.AreEqual("MEDOE", requestNotification.TaskContext.Code.Code);
            Assert.AreEqual("PROV", requestNotification.Performer.Value);
            Assert.AreEqual("200000000X", requestNotification.Performer.HealthCareProvider.Code.Code);
            Assert.AreEqual("AMB", requestNotification.Encounter.Code.Code);
            Assert.AreEqual("197379", requestNotification.MainSearchCriteria.Value.Code);
            Assert.AreEqual("2.16.840.1.113883.6.96", requestNotification.MainSearchCriteria.Value.CodeSystem);
            Assert.AreEqual("Atenolol 100 mg Oral Tablet", requestNotification.MainSearchCriteria.Value.DisplayName);
            Assert.AreEqual("Atenolol", requestNotification.MainSearchCriteria.Value.OriginalText);
            Assert.AreEqual("102811001", requestNotification.Observation.Value.Code);
            Assert.AreEqual("2.16.840.1.113883.6.96", requestNotification.Observation.Value.CodeSystem);
            Assert.AreEqual("65", requestNotification.Observation.Value.Value);
            Assert.AreEqual("mL/min", requestNotification.Observation.Value.Unit);
            Assert.AreEqual("Q000008", requestNotification.SubTopic.Value.Code);
            Assert.AreEqual("2.16.840.1.113883.6.177", requestNotification.SubTopic.Value.CodeSystem);
            Assert.AreEqual("administration and dosage", requestNotification.SubTopic.Value.DisplayName);
        }

        [TestMethod]
        public void ParseRequest_EmptyAndMissingValues()
        {
            const string testString =
                @"subTopic.c.c=sTCC&subTopic.c.cs=sTCCS&subTopic.c.dn=sTCDN&&&&"
                + "&mainSearchCriteria.c.c=&&&&";
            var parser = new Parser();
            KnowledgeRequestNotification requestNotification = parser.ParseRequest(testString);
            Assert.AreEqual("sTCC", requestNotification.SubTopic.Value.Code);
            Assert.AreEqual("sTCCS", requestNotification.SubTopic.Value.CodeSystem);
            Assert.AreEqual("sTCDN", requestNotification.SubTopic.Value.DisplayName);
            Assert.AreEqual("", requestNotification.MainSearchCriteria.Value.Code);
        }
    }
}
