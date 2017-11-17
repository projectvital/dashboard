/*
    Project VITAL.Dashboard
    Copyright (C) 2017 - Universiteit Hasselt
    This project has been funded with support from the European Commission (Project number: 2015-BE02-KA203-012317). 

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using LMG.Infrastructure.Analytics.Helpers;
using LMG.Infrastructure.Analytics.Objects.DB;
using LMG.Infrastructure.Analytics.Objects.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using TinCan;

namespace LMG.Infrastructure.Analytics.Daemon.Objects.Helpers
{
    public static class DataConversionHelper
    {
        #region Helper functions to convert DB-objects to TinCan
        public static LanguageMap ConvertStringToLanguageMap(string language, string param)
        {
            LanguageMap map = new LanguageMap();
            map.Add(language, param);
            return map;
        }

        private static StatementRef ConvertLogStatementIdToRef(Guid recordId)
        {
            StatementRef sr = new StatementRef();
            sr.id = recordId;
            return sr;
        }
        public static Statement ConvertLogStatement(LogStatement record)
        {
            if (record == null)
                return null;

            if (record.LogVerbId.HasValue && record.LogVerbIdResolved == null)
            {//Last moment check to see if all fields have been resolved. Verb should always be filled and is therefore used as a reference.
                AnalyticsWorker.ResolveIDsToObjects(record);
            }

            Statement obj = new Statement();
            obj.actor = ConvertLogAgent(record.LogAgentIdResolved);
            obj.authority = ConvertLogAgent(record.AuthorityLogAgentIdResolved);
            obj.context = ConvertLogContext(record.LogContextIdResolved);
            obj.id = record.LogStatementId;
            obj.result = ConvertResult(record.LogResultIdResolved);
            if (record.TargetLogActivityId.HasValue)
                obj.target = ConvertLogActivity(CacheHelper.GetActivity(record.TargetLogActivityId.Value));
            else if (record.TargetLogAgentId.HasValue)
                obj.target = ConvertLogAgent(CacheHelper.GetAgent(record.TargetLogAgentId.Value));
            else if (record.TargetLogStatementId.HasValue)
                obj.target = ConvertLogStatementToTarget(record.TargetLogStatementId.Value);
            obj.timestamp = record.Timestamp;
            obj.verb = ConvertVerb(record.LogVerbIdResolved);
            return obj;
        }
        private static Activity ConvertLogActivity(LogActivity record)
        {
            if (record == null)
                return null;

            Activity obj = new Activity();
            if (record.LogActivityDefinitionId.HasValue)
                obj.definition = ConvertLogActivityDefinition(record, CacheHelper.GetActivityDefinition(record.LogActivityDefinitionId.Value));
            obj.id = new Uri(record.Id);
            //obj.id = new Uri(System.Uri.EscapeUriString(record.Id));
            return obj;
        }
        private static ActivityDefinition ConvertLogActivityDefinition(LogActivity activity, LogActivityDefinition record)
        {
            if (record == null)
                return null;

            ActivityDefinition obj = new ActivityDefinition();
            obj.description = ConvertLogActivityDefinitionDetails(CacheHelper.GetLogActivityDefinitionDetails(activity), LogActivityDefinitionDetailTypes.Description);
            obj.extensions = ConvertLogExtensions(CacheHelper.GetExtensions(activity/* rmenten 2016-04-12: Also look for extensions that are activity-centered.    //record*/));
            if (!string.IsNullOrWhiteSpace(record.MoreInfo))
                obj.moreInfo = new Uri(record.MoreInfo);
            obj.name = ConvertLogActivityDefinitionDetails(CacheHelper.GetLogActivityDefinitionDetails(activity), LogActivityDefinitionDetailTypes.Name);
            if (!string.IsNullOrWhiteSpace(record.Type))
                obj.type = new Uri(record.Type);
            return obj;
        }
        private static StatementTarget ConvertLogStatementToTarget(Guid statementId)
        {
            StatementRef obj = new StatementRef();
            obj.id = statementId;
            return obj;
        }
        private static Verb ConvertVerb(LogVerb record)
        {
            if (record == null)
                return null;

            Verb obj = new Verb();
            obj.id = new Uri(CacheHelper.GetVerb(record.LogVerbId).Uri);
            obj.display = ConvertLogVerbLabels(CacheHelper.GetVerbLabels(record.LogVerbId));
            return obj;
        }
        private static Result ConvertResult(LogResult record)
        {
            if (record == null)
                return null;

            Result obj = new Result();
            obj.completion = record.IsCompleted;
            if (record.DurationTicks.HasValue)
                obj.duration = new TimeSpan(record.DurationTicks.Value);
            obj.extensions = ConvertLogExtensions(CacheHelper.GetExtensions(record));
            obj.response = record.Response;
            if (record.LogScoreId.HasValue)
                obj.score = ConverLogScore(CacheHelper.GetScore(record.LogScoreId.Value));
            obj.success = record.IsSuccess;
            return obj;
        }
        private static LanguageMap ConvertLogVerbLabels(LogVerbLabelCollection records)
        {
            if (records == null)
                return null;

            LanguageMap map = new LanguageMap();
            foreach (LogVerbLabel record in records)
            {
                map.Add(record.Language, record.Label);
            }
            return map;
        }
        private static LanguageMap ConvertLogActivityDefinitionDetails(LogActivityDefinitionDetailCollection records, LogActivityDefinitionDetailTypes type)
        {
            if (records == null)
                return null;

            LanguageMap map = new LanguageMap();
            foreach (LogActivityDefinitionDetail record in records)
            {
                if (record.LogActivityDefinitionDetailTypeId == (long)type)
                    map.Add(record.Language, record.Label);
            }
            return map;
        }
        private static ContextActivities ConvertLogContextActivities(LogContextActivityCollection records)
        {
            if (records == null)
                return null;

            ContextActivities cas = new ContextActivities();
            foreach (LogContextActivity record in records)
            {
                //if(record.LogContextActivityTypeId.HasValue && record.LogActivityId.HasValue)
                {
                    if (record.LogContextActivityTypeId == (long)LogContextActivityTypes.Parent)
                    {
                        if (cas.parent == null) cas.parent = new List<Activity>();
                        cas.parent.Add(ConvertLogActivity(CacheHelper.GetActivity(record.LogActivityId)));
                    }
                    else if (record.LogContextActivityTypeId == (long)LogContextActivityTypes.Grouping)
                    {
                        if (cas.grouping == null) cas.grouping = new List<Activity>();
                        cas.grouping.Add(ConvertLogActivity(CacheHelper.GetActivity(record.LogActivityId)));
                    }
                    else if (record.LogContextActivityTypeId == (long)LogContextActivityTypes.Category)
                    {
                        if (cas.category == null) cas.category = new List<Activity>();
                        cas.category.Add(ConvertLogActivity(CacheHelper.GetActivity(record.LogActivityId)));
                    }
                    else if (record.LogContextActivityTypeId == (long)LogContextActivityTypes.Other)
                    {
                        if (cas.other == null) cas.other = new List<Activity>();
                        cas.other.Add(ConvertLogActivity(CacheHelper.GetActivity(record.LogActivityId)));
                    }
                }
            }
            return cas;
        }
        private static TinCan.Extensions ConvertLogExtensions(LogExtensionCollection records)
        {
            if (records == null)
                return null;

            Newtonsoft.Json.Linq.JObject jobj = new Newtonsoft.Json.Linq.JObject();
            foreach (LogExtension record in records)
            {
                jobj.Add(record.Uri, record.Token);
            }
            TinCan.Extensions objs = new TinCan.Extensions(jobj);
            return objs;
        }
        private static Score ConverLogScore(LogScore record)
        {
            if (record == null)
                return null;

            Score obj = new Score();
            obj.max = record.Max;
            obj.min = record.Min;
            obj.raw = record.Raw;
            obj.scaled = record.Scaled;
            return obj;
        }
        private static Context ConvertLogContext(LogContext record)
        {
            if (record == null)
                return null;

            Context obj = new Context();
            obj.contextActivities = ConvertLogContextActivities(CacheHelper.GetLogContextActivities(record.LogContextId));
            obj.extensions = ConvertLogExtensions(CacheHelper.GetExtensions(record));
            if (record.InstructorLogAgentId.HasValue)
                obj.instructor = ConvertLogAgent(CacheHelper.GetAgent(record.InstructorLogAgentId.Value));
            obj.language = record.Language;
            obj.platform = record.Platform;
            obj.registration = record.Registration;
            obj.revision = record.Revision;
            if (record.RefLogStatementId.HasValue)
                obj.statement = ConvertLogStatementIdToRef(record.RefLogStatementId.Value);
            if (record.TeamLogAgentId.HasValue)
                obj.team = ConvertLogAgent(CacheHelper.GetAgent(record.TeamLogAgentId.Value));
            return obj;
        }
        private static Agent ConvertLogAgent(LogAgent record)
        {
            if (record == null)
                return null;

            Agent obj = new Agent();
            if (record.LogAgentAccountIdResolved != null)
                obj.account = ConvertLogAgentAccount(record.LogAgentAccountIdResolved);
            if (ConfigurationManager.AppSettings["AnonimizeUsers"] == "true")
            {
                obj.name = "" + record.LogAgentId;// AnonymousKey;
                obj.mbox = "mailto:" + record.LogAgentId/*AnonymousKey*/ + "@anonymous.uhasselt.be";
            }
            else
            {
                obj.name = record.Name;
                obj.mbox = "mailto:" + record.Mbox;
                obj.mbox_sha1sum = record.Mbox_sha1sum;
                obj.openid = record.OpenId;
            }
            return obj;
        }
        private static AgentAccount ConvertLogAgentAccount(LogAgentAccount record)
        {
            if (record == null)
                return null;

            AgentAccount obj = new AgentAccount();
            obj.homePage = new Uri(record.Homepage);
            obj.name = record.Name;
            return obj;
        }
        #endregion

        public static void ConvertJsonToCsv()
        {
            try
            {
                string folderName = "ConvertSource";
                string localJsonFolder = AppDomain.CurrentDomain.BaseDirectory + folderName;
                string csvOutputFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\output.csv";
                string csvColumnFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\output-columns.conf";
                DirectoryInfo dir = new DirectoryInfo(localJsonFolder);
                if (!dir.Exists)
                    return;

                List<FileInfo> files = new List<FileInfo>(dir.GetFiles("*.json", SearchOption.TopDirectoryOnly));
                if (files == null && files.Count == 0)
                    return;

                if (true)
                {
                    //Close arrays if they should still be open
                    foreach (FileInfo file in files)
                    {
                        IEnumerable<string> lines = File.ReadLines(file.FullName);
                        if (lines.FirstOrDefault().StartsWith("["))
                        {
                            if (!lines.LastOrDefault().EndsWith("]"))
                            {
                                File.AppendAllText(file.FullName, "]");
                            }
                        }

                    }
                }

                List<string> columns = new List<string>();
                if (File.Exists(csvColumnFile))
                {
                    columns = new List<string>(File.ReadAllLines(csvColumnFile));
                }
                else
                {
                    foreach (FileInfo file in files)
                    {
                        string rawFileContent = File.ReadAllText(file.FullName);
                        Newtonsoft.Json.Linq.JArray objects = Newtonsoft.Json.Linq.JArray.Parse(rawFileContent);

                        LMG.Infrastructure.Analytics.Daemon.Objects.Helpers.JsonHelper.GetCsvColumns(columns, objects);
                    }
                    File.WriteAllLines(csvColumnFile, columns);
                }

                CsvFileWriter csvWriter = new CsvFileWriter(csvOutputFile);
                csvWriter.Delimiter = ';';
                bool csvHeaderPrinted = false;

                foreach (FileInfo file in files)
                {
                    //new Newtonsoft.Json.JsonReader().
                    string rawFileContent = File.ReadAllText(file.FullName);
                    Newtonsoft.Json.Linq.JArray objects = Newtonsoft.Json.Linq.JArray.Parse(rawFileContent);

                    for (int i = 0; i < objects.Count; i++)
                    {
                        List<string> values = new List<string>();
                        foreach (string column in columns)
                        {
                            Newtonsoft.Json.Linq.JToken token = objects[i].SelectToken(column);
                            string val = JsonHelper.GetTokenValue(token);

                            //rmenten 2017-03-27: added to fix the dataset where some statements did not come from the LRS, and thus did not have a valid stored date.
                            //if (column == "stored" && string.IsNullOrWhiteSpace(val))
                            //{
                            //    val = "17/03/2017 00:00:00";
                            //}

                            values.Add(val);
                        }

                        if (!csvHeaderPrinted)
                        {
                            csvWriter.WriteRow(columns);
                            csvHeaderPrinted = true;
                        }

                        CsvFileHelper.SupplementCsvData(columns, values);

                        csvWriter.WriteRow(values);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
