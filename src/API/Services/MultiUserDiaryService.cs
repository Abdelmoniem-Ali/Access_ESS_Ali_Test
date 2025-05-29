using ARCRM.Pidgets.MultiUserDiaryAPI.Helpers;
using ARCRM.Pidgets.MultiUserDiaryAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Xml;

namespace ARCRM.Pidgets.MultiUserDiaryAPI.Services
{
    public class MultiUserDiaryService: BaseService
    {
        public MultiUserDiaryService(string softwareKey, string token, string email) : base(softwareKey, token, email)
        {
        }

        public List<string> GetRegistrySettings()
        {
            var registrySettings = new List<string>();
            var registryInfo = GetRegistryInfo("DiaryCheckState");
            if(registryInfo != null)
            {
                if(!string.IsNullOrEmpty(registryInfo.XmlText))
                {
                    Dictionary<string, string> saveList = GetRegistryEntryHashTable(registryInfo.XmlText);

                    foreach (KeyValuePair<string, string> kvp in saveList)
                    {
                        if (!registrySettings.Contains(kvp.Value))
                        {
                            registrySettings.Add(kvp.Value);
                        }
                    }
                }
            }
            return registrySettings;
        }

        public SharedDiaryInfo[] GetOwners()
        {
            var owners = new List<SharedDiaryInfo>();
            var parameters = new List<SqlParameter>() { new SqlParameter("@AW_USERID", UserId) };
            var userDetailsDT = DataAccess.ExecStoredProcedureReturnResultsDT("dbo.USER_SELECT_BY_ID", parameters);
            if (userDetailsDT != null && userDetailsDT.Rows.Count > 0)
            {
                owners.Add(new SharedDiaryInfo()
                {
                    UserId = UserId ?? 0,
                    KeyId = (UserId ?? 0).ToString(),
                    KeyName = userDetailsDT.Rows[0]["UserFullName"].ToString()
                });
            }

            parameters = new List<SqlParameter>() { new SqlParameter("@AW_USERID", UserId) };
            var sharedDiaries = DataAccess.ExecStoredProcedureReturnResultsDT("dbo.DIARY_SHARED_LINK_SELECT",parameters).ToList<SharedDiaryInfo>();
            foreach (var sharedDiary in sharedDiaries)
            {
                parameters = new List<SqlParameter>() { new SqlParameter("@AW_USERID", sharedDiary.UserId), new("@AW_LINKEDUSERID", UserId) };
                var diaryLinkDetailsDT = DataAccess.ExecStoredProcedureReturnResultsDT("dbo.DIARY_SHARED_PERMISSION_SELECT", parameters);
                if (diaryLinkDetailsDT != null && diaryLinkDetailsDT.Rows.Count > 0)
                {
                    var shareMode = string.IsNullOrEmpty(diaryLinkDetailsDT.Rows[0]["ShareMode"].ToString())? 0 : int.Parse(diaryLinkDetailsDT.Rows[0]["ShareMode"].ToString());
                    var expiresOn = diaryLinkDetailsDT.Rows[0]["ExpiresOn"].ToString();
                    DateTime? ExpireDate = string.IsNullOrEmpty(diaryLinkDetailsDT.Rows[0]["ExpireDate"].ToString())? null: DateTime.Parse(diaryLinkDetailsDT.Rows[0]["ExpireDate"].ToString());
                    if (shareMode == 0 || (expiresOn == "Y" && (!ExpireDate.HasValue || ExpireDate < DateTime.Today)))
                    {
                        continue;
                    }
                    owners.Add(new SharedDiaryInfo()
                    {
                        UserId = sharedDiary.UserId,
                        UserName = sharedDiary.UserName,
                        KeyId = (sharedDiary.UserId??0).ToString(),
                        KeyName = sharedDiary.UserName

                    });

                }
            }

            parameters = new List<SqlParameter>() { new SqlParameter("@AW_USERID", UserId) };
            var resourceDiaries = DataAccess.ExecStoredProcedureReturnResultsDT("dbo.RESOURCE_DIARY_SELECT", parameters);
            if (resourceDiaries != null && resourceDiaries.Rows.Count > 0) {
                foreach (DataRow resourceDiary in resourceDiaries.Rows)
                {
                    parameters = new List<SqlParameter>() { new SqlParameter("@AW_RESOURCEDIARYID", int.Parse(resourceDiary["ResourceDiaryId"].ToString())), new("@AW_LINKEDUSERID", UserId) };
                    var diaryLinkDetailsDT = DataAccess.ExecStoredProcedureReturnResultsDT("dbo.RESOURCE_DIARY_PERMISSION_SELECT", parameters);
                    if (diaryLinkDetailsDT != null && diaryLinkDetailsDT.Rows.Count > 0)
                    {
                        var shareMode = string.IsNullOrEmpty(diaryLinkDetailsDT.Rows[0]["ShareMode"].ToString()) ? 0 : int.Parse(diaryLinkDetailsDT.Rows[0]["ShareMode"].ToString());
                        var expiresOn = diaryLinkDetailsDT.Rows[0]["ExpiresOn"].ToString();
                        DateTime? ExpireDate = string.IsNullOrEmpty(diaryLinkDetailsDT.Rows[0]["ExpireDate"].ToString()) ? null : DateTime.Parse(diaryLinkDetailsDT.Rows[0]["ExpireDate"].ToString());
                        if (shareMode == 0 || (expiresOn == "Y" && (!ExpireDate.HasValue || ExpireDate < DateTime.Today)))
                        {
                            continue;
                        }
                        owners.Add(new SharedDiaryInfo()
                        {
                            KeyId = resourceDiary["RESOURCEDIARYNAME"].ToString(),
                            KeyName = resourceDiary["RESOURCEDIARYNAME"].ToString(),
                            ResourceDiaryId = int.Parse(resourceDiary["ResourceDiaryId"].ToString())
                        });
                    }
                }
            }

            return owners.ToArray();

        }

        public DiaryEventInfo[] GetDiaryEvents(DateTime pStartDate, DateTime pEndDate, SharedDiaryInfo[] pOwners)
        {
            var diaryEvents = new List<DiaryEventInfo>();
           foreach(var owner in pOwners)
            {
                if(owner.ResourceDiaryId.HasValue)
                {
                    var parameters = new List<SqlParameter>() { new SqlParameter("@AW_STARTTIME", pStartDate), new SqlParameter("@AW_ENDTIME", pEndDate), new SqlParameter("@AW_RESOURCEDIARYID", owner.ResourceDiaryId) };
                    var diaryEventList = DataAccess.ExecStoredProcedureReturnResultsDT("dbo.DIARY_EVENT_SELECT_BY_RESOURCE_DIARY_ID", parameters).ToList<DiaryEventInfo>();
                    diaryEventList.ForEach(x =>
                    {
                        x.KeyId = owner.KeyId;
                    });
                    diaryEvents.AddRange(diaryEventList);
                }
                else
                {
                    var parameters = new List<SqlParameter>() { new SqlParameter("@AW_USERID",owner.UserId), new SqlParameter("@AW_STARTTIME", pStartDate), new SqlParameter("@AW_ENDTIME", pEndDate) };
                    var diaryEventList = DataAccess.ExecStoredProcedureReturnResultsDT("dbo.DIARY_EVENT_SELECT_BY_DATE_RANGE", parameters).ToList<DiaryEventInfo>();
                    diaryEventList.ForEach(x => 
                    {
                        x.KeyId = owner.UserId.ToString();
                    });
                    diaryEvents.AddRange(diaryEventList);
                }
            }
           foreach(var diaryEvent in diaryEvents)
            {
                var parameters = new List<SqlParameter>() { new SqlParameter("@AW_DIARYEVENTID", diaryEvent.DiaryEventId) };
                var recurrenceDetails = DataAccess.ExecStoredProcedureReturnResultsDT("dbo.DIARY_RECURRENCE_SELECT_BY_ID", parameters);
                if(recurrenceDetails != null && recurrenceDetails.Rows.Count > 0)
                {
                    diaryEvent.PatternFrequency = recurrenceDetails.Rows[0]["PatternFrequency"] == DBNull.Value ? null : (int?)recurrenceDetails.Rows[0]["PatternFrequency"];
                    diaryEvent.PatternDaysOfWeek = recurrenceDetails.Rows[0]["PatternDaysOfWeek"] == DBNull.Value ? null : (int?)recurrenceDetails.Rows[0]["PatternDaysOfWeek"];
                    diaryEvent.RangeLimit = recurrenceDetails.Rows[0]["RangeLimit"] == DBNull.Value ? null : (int?)recurrenceDetails.Rows[0]["RangeLimit"];
                    diaryEvent.RangeMaxOccurrences = recurrenceDetails.Rows[0]["RangeMaxOccurrences"] == DBNull.Value ? null : (int?)recurrenceDetails.Rows[0]["RangeMaxOccurrences"];
                    diaryEvent.RangeEndDate = recurrenceDetails.Rows[0]["RangeEndDate"] == DBNull.Value ? null : (DateTime?)recurrenceDetails.Rows[0]["RangeEndDate"];
                    diaryEvent.RangeStartDate = recurrenceDetails.Rows[0]["RangeStartDate"] == DBNull.Value ? null : (DateTime?)recurrenceDetails.Rows[0]["RangeStartDate"];
                    diaryEvent.PatternInterval = recurrenceDetails.Rows[0]["PatternInterval"] == DBNull.Value ? null : (int?)recurrenceDetails.Rows[0]["PatternInterval"];
                    diaryEvent.PatternType = recurrenceDetails.Rows[0]["PatternType"] == DBNull.Value ? null : (int?)recurrenceDetails.Rows[0]["PatternType"];
                    diaryEvent.PatternOccurrenceOfDayInMonth = recurrenceDetails.Rows[0]["PatternOccurrenceOfDayInMonth"] == DBNull.Value ? null : (int?)recurrenceDetails.Rows[0]["PatternOccurrenceOfDayInMonth"];
                    diaryEvent.PatternMonthOfYear = recurrenceDetails.Rows[0]["PatternMonthOfYear"] == DBNull.Value ? null : (int?)recurrenceDetails.Rows[0]["PatternMonthOfYear"];
                    diaryEvent.PatternDayOfMonth = recurrenceDetails.Rows[0]["PatternDayOfMonth"] == DBNull.Value ? null : (int?)recurrenceDetails.Rows[0]["PatternDayOfMonth"];
                }
                
            }
            SaveRegistry(pOwners.Select(o => o.KeyId).ToArray());
           return diaryEvents.ToArray();

        }


        public TaskInfo[] GetTasks()
        {
            var parameters = new List<SqlParameter>() { new SqlParameter("@AW_USERID", UserId), new SqlParameter("@AW_SYSTEMCODE", "COMPL") };

            return DataAccess.ExecStoredProcedureReturnResultsDT("dbo.TASK_SELECT_BY_STATUS", parameters).ToList<TaskInfo>().ToArray();
        }

        public TaskInfo? GetTaskById(int pTaskId)
        {
            var parameters = new List<SqlParameter>() { new SqlParameter("@AW_TASKID", pTaskId) };
            var taskInfo = DataAccess.ExecStoredProcedureReturnResultsDT("dbo.TASK_SELECT_BY_ID", parameters).ToList<TaskInfo>().FirstOrDefault();

            if(taskInfo != null)
            {
                parameters = new List<SqlParameter>() { new SqlParameter("@AW_TASKID", pTaskId), new SqlParameter("AW_USERID",UserId) };
                taskInfo.TaskAssignment = DataAccess.ExecStoredProcedureReturnResultsDT("dbo.TASK_ASSIGNMENT_SELECT_OF_USER",parameters).ToList<TaskAssignmentInfo>().FirstOrDefault();

                parameters = new List<SqlParameter>() { new SqlParameter("@AW_TASKID", pTaskId) };
                var taskNotesDT = DataAccess.ExecStoredProcedureReturnResultsDT("dbo.TASK_NOTE_SELECT_BY_ID", parameters);
                if(taskNotesDT != null && taskNotesDT.Rows.Count > 0)
                {
                    taskInfo.Notes = taskNotesDT.Rows[0]["Notes"].ToString();
                }

                taskInfo.RecurrenceInfo = DataAccess.ExecStoredProcedureReturnResultsDT("dbo.TASK_RECURRENCE_SELECT_BY_ID",parameters).ToList<TaskRecurrenceInfo>().FirstOrDefault();
            }

            return taskInfo;
        }

        private RegistryInfo? GetRegistryInfo(string pSystemCode)
        {
            var parameters = new List<SqlParameter>() { new SqlParameter("@AW_REGISTRYUSERID", UserId), new SqlParameter("@AW_SYSTEMCODE", "DiaryCheckState") };

           return DataAccess.ExecStoredProcedureReturnResultsDT("dbo.REGISTRY_SELECT", parameters).ToList<RegistryInfo>().FirstOrDefault();
        }

        private void SaveRegistry(string[] pVisibleOwners)
        {
            Dictionary<string, string> saveList = new Dictionary<string, string>();
            foreach (string value in pVisibleOwners)
            {
                saveList.Add("id" + value.Replace(" ", ""), value); //Ensure there are no spaces in the key element
            }
            var registryInfo = GetRegistryInfo("DiaryCheckState");
            if(registryInfo == null)
            {
                registryInfo = new RegistryInfo()
                {
                    SystemCode = "DiaryCheckState",
                    RegistryUserId = UserId,
                    CreatedUserId = UserId,
                    UpdatedUserId = UserId
                };
            }
            registryInfo.XmlText = GetXmlText(saveList);

            if (registryInfo.RegistryId.HasValue)
            {
                var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@AV_REGISTRYUSERID", UserId),
                    new SqlParameter("@AV_SYSTEMCODE", "DiaryCheckState"),
                    new SqlParameter("@AV_XMLTEXT", string.IsNullOrEmpty(registryInfo.XmlText)? DBNull.Value : registryInfo.XmlText),
                    new SqlParameter("@AW_REGISTRYID", registryInfo.RegistryId),
                    new SqlParameter("@AV_UPDATEDUSERID", registryInfo.UpdatedUserId),
                    new SqlParameter("@AW_UPDATEDTIMESTAMP", registryInfo.UpdatedTimestamp)
                };
                DataAccess.ExecStoredProcedure("dbo.REGISTRY_UPDATE", parameters);
            }
            else
            {
                var parameters = new List<SqlParameter>(){ new SqlParameter("@AV_REGISTRYUSERID", UserId),
                    new SqlParameter("@AV_SYSTEMCODE", "DiaryCheckState"),
                    new SqlParameter("@AV_XMLTEXT", string.IsNullOrEmpty(registryInfo.XmlText) ? DBNull.Value : registryInfo.XmlText),
                    new SqlParameter("@AV_UPDATEDUSERID", registryInfo.UpdatedUserId),
                    new SqlParameter("@AV_CREATEDUSERID", registryInfo.CreatedUserId)
                };
                DataAccess.ExecStoredProcedure("dbo.REGISTRY_INSERT", parameters);
            }
        }

        private string GetXmlText(Dictionary<string, string> pRegistryEntryList)
        {
            string result = string.Empty;

            using (System.IO.StringWriter xmlText = new StringWriter())
            {
                using (System.Xml.XmlTextWriter xmlSettings = new System.Xml.XmlTextWriter(xmlText))
                {

                    xmlSettings.WriteStartDocument();

                    xmlSettings.WriteStartElement("Settings");

                    xmlSettings.WriteStartElement("fieldValues");

                    foreach (KeyValuePair<string, string> kvp in pRegistryEntryList)
                    {
                        xmlSettings.WriteStartElement(kvp.Key.ToString());
                        xmlSettings.WriteString(kvp.Value.ToString());
                        xmlSettings.WriteEndElement();
                    }

                    xmlSettings.WriteEndElement();
                    xmlSettings.WriteEndElement();

                    xmlSettings.WriteEndDocument();

                    xmlSettings.Flush();
                    result = xmlText.ToString();
                }
            }
            return result;
        }

        private Dictionary<string, string> GetRegistryEntryHashTable(string pXml)
        {
            var result = new Dictionary<string, string>();
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.InnerXml = pXml;

                XmlElement element = xmlDoc.DocumentElement;
                XmlNodeList fieldValuesList = element.GetElementsByTagName("fieldValues");

                for (int i = 0; i < fieldValuesList.Count; i++)
                {
                    XmlNode xmlnode = fieldValuesList[i];
                    foreach (XmlNode node in xmlnode.ChildNodes)
                    {
                        result.Add(node.Name, node.InnerText);
                    }
                }
            }
            catch { }
            return result;
        }


    }
}
