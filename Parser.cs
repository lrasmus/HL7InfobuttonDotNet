using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HL7InfobuttonAPI.Models;
using System.Collections.Specialized;
using System.Net;

namespace HL7InfobuttonAPI
{
    public class Parser
    {
        public KnowledgeRequestNotification ParseRequest(string parameters)
        {
            return ParseRequest(SplitStringParameters(parameters));
        }

        public NameValueCollection SplitStringParameters(string parameters)
        {
            var collection = new NameValueCollection();
            var pairs = parameters.Split('&');
            foreach (var pair in pairs)
            {
                var item = pair.Split('=');
                if (item.Length >= 2)
                {
                    collection.Add(item[0], item[1]);
                }
            }
            return collection;
        }

        public KnowledgeRequestNotification ParseRequest(NameValueCollection parameters)
        {
            KnowledgeRequestNotification requestNotification = new KnowledgeRequestNotification();
            foreach (var parameter in parameters.AllKeys)
            {
                if (parameter == null)
                {
                    continue;
                }

                string[] keyParts = parameter.Split('.');
                AddPart(requestNotification, keyParts, parameters[parameter]);
            }

            return requestNotification;

            // v = value, n = name, c = code, dn = displayName, u = unit, cs = codeSystem, ot = originalText
            //knowledgeRequestNotification.effectiveTime.v       x

            //holder.assignedEntity.n                             x
            //holder.assignedEntity.certificateText

            //assignedAuthorizedPerson.id.root                    x

            //representedOrganization.id.root                      x

            //assignedEntity.representedOrganization.n              x

            //patientPerson.administrativeGenderCode.c               x
            //patientPerson.administrativeGenderCode.dn

            //age.v.v  <-- is this an error?  spec says code.value, should it be age.c.v?
            //age.v.u  <-- same question about error

            //ageGroup.v.c                                     x
            //ageGroup.v.cs
            //ageGroup.v.dn

            //taskContext.c.c                                 x
            //taskContext.c.dn

            //subTopic.v.c                                      x
            //subTopic.v.cs
            //subTopic.v.dn
            //subTopic.v.ot
            //subTopic.c.c (deprecated)
            //subTopic.c.cs (deprecated)
            //subTopic.c.dn (deprecated)

            //mainSearchCriteria.v.c                           x
            //mainSearchCriteria.v.cs
            //mainSearchCriteria.v.dn
            //mainSearchCriteria.v.ot
            //mainSearchCriteria.c.c (deprecated)
            //mainSearchCriteria.c.cs (deprecated)
            //mainSearchCriteria.c.dn (deprecated)
            //mainSearchCriteria.c.ot (deprecated)

            //severityObservation.interpretationCode.c        x
            //severityObservation.interpretationCode.cs
            //severityObservation.interpretationCode.dn

            //informationRecipient=PAT|PROV|PAYOR                  x
            //informationRecipient.healthCareProvider.c.c
            //informationRecipient.healthCareProvider.c.cs
            //informationRecipient.healthCareProvider.c.dn
            //informationRecipient.languageCode.c

            //performer=PAT|PROV|PAYOR                        x
            //performer.healthCareProvider.c.c
            //performer.healthCareProvider.c.cs
            //performer.healthCareProvider.c.dn
            //performer.languageCode.c

            //encounter.c.c                              x
            //encounter.c.cs
            //encounter.c.dn

            //serviceDeliveryLocation.id.root             x

            //observation.c.c                              x
            //observation.c.cs
            //observation.c.dn
            //observation.v.c
            //observation.v.cs
            //observation.v.dn
            //observation.v.v
            //observation.v.u

            //locationOfInterest.addr.postalCode              x
            //locationOfInterest.addr.city
            //locationOfInterest.addr.state
        }

        private bool HasProperty(object container, string property)
        {
            var type = container.GetType();
            return type.GetProperty(property) != null;
        }

        private void SetProperty(object container, string property, object value)
        {
            var type = container.GetType();
            if (value is string)
            {
                value = Uri.UnescapeDataString((value as string).Replace("+", "%20"));
                //value = WebUtility.HtmlDecode((value as string).Replace("+", "%20"));
            }
            type.GetProperty(property).SetValue(container, value, null);
        }

        private object GetProperty(object container, string property)
        {
            var type = container.GetType();
            var propertyObject = type.GetProperty(property);
            if (propertyObject == null)
            {
                return null;
            }
            var propertyValue = propertyObject.GetValue(container, null);
            if (propertyValue == null)
            {
                propertyValue = Activator.CreateInstance(propertyObject.PropertyType);
                SetProperty(container, property, propertyValue);
            }

            return propertyValue;
        }

        private static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        private void AddPart(object container, IEnumerable<string> keyParts, string value)
        {
            if (container == null || keyParts == null || keyParts.Count() == 0)
            {
                return;
            }

            string partName = MapParameterNameToStandardizedName(keyParts.First());
            switch (partName)
            {
                case "knowledgeRequestNotification":
                    AddPart(container, keyParts.Skip(1), value);
                    break;
                    //case "assignedAuthorizedPerson":
                    //case "representedOrganization":
                    //case "assignedEntity":
                case "administrativeGenderCode":
                case "taskContext":
                case "effectiveTime":
                case "interpretationCode":
                case "healthCareProvider":
                case "languageCode":
                case "encounter":
                case "observation":
                case "locationOfInterest":
                case "addr":
                case "holder":
                case "assignedEntity":
                    AddPart(GetProperty(container, UppercaseFirst(partName)), keyParts.Skip(1), value);
                    break;

                    // Both subTopic and mainSearchCriteria have deprecated values that previously used "code", and now use "value".
                    // Instead of having both structured in those data models, we are going to map "code" references to "value".
                case "subTopic":
                case "mainSearchCriteria":
                    if (keyParts.Count() > 2 && keyParts.Skip(1).First() == "c")
                    {
                        var newParts = new List<string>(new string[] {partName, "v"});
                        newParts.AddRange(keyParts.Skip(2));
                        keyParts = newParts;
                        partName = keyParts.First();
                    }
                    AddPart(GetProperty(container, UppercaseFirst(partName)), keyParts.Skip(1), value);
                    break;

                case "id":
                    AddPart(GetProperty(container, "ID"), keyParts.Skip(1), value);
                    break;

                case "severityObservation":
                    {
                        var notification = container as KnowledgeRequestNotification;
                        if (notification.MainSearchCriteria == null)
                        {
                            notification.MainSearchCriteria = new MainSearchCriteria();
                        }
                        AddPart(GetProperty(notification.MainSearchCriteria, "SeverityObservation"), keyParts.Skip(1),
                                value);
                        break;
                    }

                case "serviceDeliveryLocation":
                    {
                        var notification = container as KnowledgeRequestNotification;
                        if (notification.Encounter == null)
                        {
                            notification.Encounter = new Encounter();
                        }
                        AddPart(GetProperty(notification.Encounter, "ServiceDeliveryLocation"), keyParts.Skip(1), value);
                        break;
                    }

                case "informationRecipient":
                    if (keyParts.Count() > 1)
                    {
                        AddPart(GetProperty(container, UppercaseFirst(partName)), keyParts.Skip(1), value);
                    }
                    else if (container is KnowledgeRequestNotification)
                    {
                        var notification = container as KnowledgeRequestNotification;
                        if (notification.InformationRecipient == null)
                        {
                            notification.InformationRecipient = new InformationRecipient();
                        }
                        notification.InformationRecipient.Value = value;
                    }
                    break;

                    // TODO - refactor this w/ informationRecipient case above
                case "performer":
                    if (keyParts.Count() > 1)
                    {
                        AddPart(GetProperty(container, UppercaseFirst(partName)), keyParts.Skip(1), value);
                    }
                    else if (container is KnowledgeRequestNotification)
                    {
                        KnowledgeRequestNotification notification = container as KnowledgeRequestNotification;
                        if (notification.Performer == null)
                        {
                            notification.Performer = new Performer();
                        }
                        notification.Performer.Value = value;
                    }
                    break;

                case "patientPerson":
                case "ageGroup":
                case "age":
                    var patientContext = GetProperty(container, "PatientContext");
                    AddPart(GetProperty(patientContext, UppercaseFirst(partName)), keyParts.Skip(1), value);
                    break;
                    //case "holder":
                    //    AddPart(??, keyParts.Skip(1), value);
                    //    break;
                    //case "assignedEntity":
                    //    AddPart(??, keyParts.Skip(1), value);
                    //    break;
                    break;
                case "n":
                    SetProperty(container, "Name", value);
                    break;
                case "certificateText":
                    SetProperty(container, "CertificateText", value);
                    break;
                case "root":
                    SetProperty(container, "Root", value);
                    break;
                case "c":
                    if (keyParts.Count() > 1)
                    {
                        AddPart(GetProperty(container, "Code"), keyParts.Skip(1), value);
                    }
                    else
                    {
                        SetProperty(container, "Code", value);
                    }
                    break;
                case "cs":
                    SetProperty(container, "CodeSystem", value);
                    break;
                case "dn":
                    SetProperty(container, "DisplayName", value);
                    break;
                case "v":
                    if (keyParts.Count() > 1)
                    {
                        AddPart(GetProperty(container, "Value"), keyParts.Skip(1), value);
                    }
                    else
                    {
                        SetProperty(container, "Value", value);
                    }
                    break;
                case "u":
                    SetProperty(container, "Unit", value);
                    break;
                case "ot":
                    SetProperty(container, "OriginalText", value);
                    break;
                case "postalCode":
                    SetProperty(container, "PostalCode", value);
                    break;
                case "city":
                    SetProperty(container, "City", value);
                    break;
                case "state":
                    SetProperty(container, "State", value);
                    break;

                    //informationRecipient=PAT|PROV|PAYOR                  x
                    //informationRecipient.healthCareProvider.c.c
                    //informationRecipient.healthCareProvider.c.cs
                    //informationRecipient.healthCareProvider.c.dn
                    //informationRecipient.languageCode.c

                    //performer=PAT|PROV|PAYOR                        x
                    //performer.healthCareProvider.c.c
                    //performer.healthCareProvider.c.cs
                    //performer.healthCareProvider.c.dn
                    //performer.languageCode.c

                    //encounter.c.c                              x
                    //encounter.c.cs
                    //encounter.c.dn

                    //serviceDeliveryLocation.id.root             x

                    //observation.c.c                              x
                    //observation.c.cs
                    //observation.c.dn
                    //observation.v.c
                    //observation.v.cs
                    //observation.v.dn
                    //observation.v.v
                    //observation.v.u

                    //locationOfInterest.addr.postalCode              x
                    //locationOfInterest.addr.city
                    //locationOfInterest.addr.state
            }
        }

        private string MapParameterNameToStandardizedName(string parameter)
        {
            if (parameter.Equals("knowledgeRequestNotification", StringComparison.CurrentCultureIgnoreCase))
            {
                return "knowledgeRequestNotification";
            }
            else if (parameter.Equals("administrativeGenderCode", StringComparison.CurrentCultureIgnoreCase))
            {
                return "administrativeGenderCode";
            }
            else if (parameter.Equals("taskContext", StringComparison.CurrentCultureIgnoreCase))
            {
                return "taskContext";
            }
            else if (parameter.Equals("effectiveTime", StringComparison.CurrentCultureIgnoreCase))
            {
                return "effectiveTime";
            }
            else if (parameter.Equals("interpretationCode", StringComparison.CurrentCultureIgnoreCase))
            {
                return "interpretationCode";
            }
            else if (parameter.Equals("healthCareProvider", StringComparison.CurrentCultureIgnoreCase))
            {
                return "healthCareProvider";
            }
            else if (parameter.Equals("languageCode", StringComparison.CurrentCultureIgnoreCase))
            {
                return "languageCode";
            }
            else if (parameter.Equals("encounter", StringComparison.CurrentCultureIgnoreCase))
            {
                return "encounter";
            }
            else if (parameter.Equals("observation", StringComparison.CurrentCultureIgnoreCase))
            {
                return "observation";
            }
            else if (parameter.Equals("locationOfInterest", StringComparison.CurrentCultureIgnoreCase))
            {
                return "locationOfInterest";
            }
            else if (parameter.Equals("addr", StringComparison.CurrentCultureIgnoreCase))
            {
                return "addr";
            }
            else if (parameter.Equals("subTopic", StringComparison.CurrentCultureIgnoreCase))
            {
                return "subTopic";
            }
            else if (parameter.Equals("mainSearchCriteria", StringComparison.CurrentCultureIgnoreCase))
            {
                return "mainSearchCriteria";
            }
            else if (parameter.Equals("id", StringComparison.CurrentCultureIgnoreCase))
            {
                return "id";
            }
            else if (parameter.Equals("severityObservation", StringComparison.CurrentCultureIgnoreCase))
            {
                return "severityObservation";
            }
            else if (parameter.Equals("serviceDeliveryLocation", StringComparison.CurrentCultureIgnoreCase))
            {
                return "serviceDeliveryLocation";
            }
            else if (parameter.Equals("informationRecipient", StringComparison.CurrentCultureIgnoreCase))
            {
                return "informationRecipient";
            }
            else if (parameter.Equals("performer", StringComparison.CurrentCultureIgnoreCase))
            {
                return "performer";
            }
            else if (parameter.Equals("patientPerson", StringComparison.CurrentCultureIgnoreCase))
            {
                return "patientPerson";
            }
            else if (parameter.Equals("ageGroup", StringComparison.CurrentCultureIgnoreCase))
            {
                return "ageGroup";
            }
            else if (parameter.Equals("age", StringComparison.CurrentCultureIgnoreCase))
            {
                return "age";
            }
            else if (parameter.Equals("n", StringComparison.CurrentCultureIgnoreCase))
            {
                return "n";
            }
            else if (parameter.Equals("certificateText", StringComparison.CurrentCultureIgnoreCase))
            {
                return "certificateText";
            }
            else if (parameter.Equals("root", StringComparison.CurrentCultureIgnoreCase))
            {
                return "root";
            }
            else if (parameter.Equals("c", StringComparison.CurrentCultureIgnoreCase))
            {
                return "c";
            }
            else if (parameter.Equals("cs", StringComparison.CurrentCultureIgnoreCase))
            {
                return "cs";
            }
            else if (parameter.Equals("dn", StringComparison.CurrentCultureIgnoreCase))
            {
                return "dn";
            }
            else if (parameter.Equals("v", StringComparison.CurrentCultureIgnoreCase))
            {
                return "v";
            }
            else if (parameter.Equals("u", StringComparison.CurrentCultureIgnoreCase))
            {
                return "u";
            }
            else if (parameter.Equals("ot", StringComparison.CurrentCultureIgnoreCase))
            {
                return "ot";
            }
            else if (parameter.Equals("postalCode", StringComparison.CurrentCultureIgnoreCase))
            {
                return "postalCode";
            }
            else if (parameter.Equals("city", StringComparison.CurrentCultureIgnoreCase))
            {
                return "city";
            }
            else if (parameter.Equals("state", StringComparison.CurrentCultureIgnoreCase))
            {
                return "state";
            }
            else if (parameter.Equals("holder", StringComparison.CurrentCultureIgnoreCase))
            {
                return "holder";
            }
            else if (parameter.Equals("assignedEntity", StringComparison.CurrentCultureIgnoreCase))
            {
                return "assignedEntity";
            }
            else
            {
                return parameter;
            }
        }
    }
}
