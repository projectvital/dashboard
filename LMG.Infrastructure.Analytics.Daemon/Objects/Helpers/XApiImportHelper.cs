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

using LMG.Infrastructure.Analytics.Objects;
using LMG.Infrastructure.Analytics.Objects.DB;
using LMG.Infrastructure.Analytics.Objects.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LMG.Infrastructure.Analytics.Daemon.Objects.Helpers
{
    public static class XApiImportHelper
    {

        public static List<string> ImportJsonStatementsInFolder(string folderName)
        {
            Kernel.EnsureConnection();

            string localJsonFolder = AppDomain.CurrentDomain.BaseDirectory + folderName;
            string logFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\import.log";

            DirectoryInfo dir = new DirectoryInfo(localJsonFolder);
            if (!dir.Exists)
                return new List<string>();

            List<FileInfo> files = new List<FileInfo>(dir.GetFiles("*.json", SearchOption.TopDirectoryOnly));
            if (files == null && files.Count == 0)
                return new List<string>();

            List<string> status = new List<string>();
            File.WriteAllText(logFile, GetTimestampForLog() + " : Import started" + System.Environment.NewLine);
            foreach (FileInfo file in files)
            {
                List<string> partialStatus = ImportJsonStatements(file.FullName);
                status.AddRange(partialStatus);
                File.AppendAllLines(logFile, partialStatus);
            }
            File.AppendAllText(logFile, GetTimestampForLog() + " : Import ended (" + status.Count + " warnings/errors)");

            return status;
        }
        public static List<string> ImportJsonStatements(string file)
        {
            IEnumerable<string> lines = File.ReadLines(file);
            if (lines.FirstOrDefault().StartsWith("["))
            {
                if (!lines.LastOrDefault().EndsWith("]"))
                {
                    File.AppendAllText(file, "]");
                }
            }


            List<string> status = new List<string>();
            string rawFileContent = File.ReadAllText(file);
            Newtonsoft.Json.Linq.JArray objects = Newtonsoft.Json.Linq.JArray.Parse(rawFileContent);
            for (int i = 0; i < objects.Count; i++)
            {
                try
                {
                    TinCan.Statement obj = new TinCan.Statement(new TinCan.Json.StringOfJSON(objects[i].ToString()));
                    ConvertStatement(obj);
                }
                catch (Warning warning)
                {
                    status.Add(GetTimestampForLog() + " : Warning : " + file + " : #"+i+" : " + warning.Message);
                }
                catch (ErrorLog ex)
                {
                    status.Add(GetTimestampForLog() + " : Error : " + file + " : #" + i + " : " + ex.Message);
                }
                catch (Exception ex)
                {
                    status.Add(GetTimestampForLog() + " : Error : " + file + " : #" + i + " : " + ex.Message);
                }
            }
            return status;
        }

        public static LogStatement ConvertStatement(TinCan.Statement record)
        {
            if (record == null)
                return null;

            LogStatement db_obj = new LogStatement();
            db_obj.LogAgentId = ConvertAgent(record.actor);
            db_obj.AuthorityLogAgentId = ConvertAgent(record.authority, true);
            db_obj.LogContextId = ConvertContext(record.context);
            db_obj.LogStatementId = record.id ?? Guid.Empty;
            db_obj.LogResultId = ConvertResult(record.result);
            if (record.target.ObjectType == TinCan.Activity.OBJECT_TYPE)
                db_obj.TargetLogActivityId = ConvertActivity(record.target);
            else if (record.target.ObjectType == TinCan.Agent.OBJECT_TYPE)
                db_obj.TargetLogAgentId = ConvertAgent(record.target);
            else if (record.target.ObjectType == TinCan.StatementRef.OBJECT_TYPE)
                db_obj.TargetLogStatementId = ConvertStatementRef(record.target);
            db_obj.Timestamp = record.timestamp;
            db_obj.StoredTimestamp = record.stored;
            db_obj.LogVerbId = ConvertVerb(record.verb);

            ErrorHelper.ThrowErrorIfNotNull(db_obj.OnInsert());

            return db_obj;
        }

        private static long? ConvertResult(TinCan.Result obj)
        {
            if (obj == null)
                return null;

            LogResult db_obj = new LogResult();
            if (obj.duration.HasValue)
                db_obj.DurationTicks = obj.duration.Value.Ticks;
            db_obj.IsCompleted = obj.completion;
            db_obj.IsSuccess = obj.success;
            db_obj.LogScoreId = ConvertScore(obj.score);
            db_obj.Response = obj.response;

            ErrorHelper.ThrowErrorIfNotNull(db_obj.OnInsert());

            ConvertExtensions(db_obj, obj.extensions);

            return db_obj.LogResultId;
        }
        private static long? ConvertScore(TinCan.Score obj)
        {
            if (obj == null)
                return null;

            LogScore db_obj = new LogScore();
            db_obj.Max = obj.max;
            db_obj.Min = obj.min;
            db_obj.Raw = obj.raw;
            db_obj.Scaled = obj.scaled;

            ErrorHelper.ThrowErrorIfNotNull(db_obj.OnInsert());

            return db_obj.LogScoreId;
        }
        private static long? ConvertContext(TinCan.Context obj)
        {
            if (obj == null)
                return null;

            LogContext db_obj = new LogContext();
            db_obj.InstructorLogAgentId = ConvertAgent(obj.instructor);
            db_obj.Language = obj.language;
            db_obj.Platform = obj.platform;
            db_obj.RefLogStatementId = ConvertStatementRef(obj.statement);
            db_obj.Registration = obj.registration;
            db_obj.Revision = obj.revision;
            db_obj.TeamLogAgentId = ConvertAgent(obj.team);

            ErrorHelper.ThrowErrorIfNotNull(db_obj.OnInsert());

            ConvertExtensions(db_obj, obj.extensions);
            ConvertContextActivities(db_obj, obj.contextActivities);
            
            return db_obj.LogContextId;
        }
        private static void ConvertContextActivities(LogContext db_obj, TinCan.ContextActivities contextActivities)
        {
            if (contextActivities == null)
                return;

            if (contextActivities.category != null)
            {
                foreach (TinCan.Activity cat in contextActivities.category)
                {
                    LogContextActivity act = new LogContextActivity();
                    act.LogActivityId = (long)ConvertActivity(cat);
                    act.LogContextActivityTypeId = (long)LogContextActivityTypes.Category;
                    act.LogContextId = db_obj.LogContextId;
                    ErrorHelper.ThrowErrorIfNotNull(act.OnInsert());
                }
            }

            if (contextActivities.grouping != null)
            {
                foreach (TinCan.Activity cat in contextActivities.grouping)
                {
                    LogContextActivity act = new LogContextActivity();
                    act.LogActivityId = (long)ConvertActivity(cat);
                    act.LogContextActivityTypeId = (long)LogContextActivityTypes.Grouping;
                    act.LogContextId = db_obj.LogContextId;
                    ErrorHelper.ThrowErrorIfNotNull(act.OnInsert());
                }
            }

            if (contextActivities.other != null)
            {
                foreach (TinCan.Activity cat in contextActivities.other)
                {
                    LogContextActivity act = new LogContextActivity();
                    act.LogActivityId = (long)ConvertActivity(cat);
                    act.LogContextActivityTypeId = (long)LogContextActivityTypes.Other;
                    act.LogContextId = db_obj.LogContextId;
                    ErrorHelper.ThrowErrorIfNotNull(act.OnInsert());
                }
            }

            if (contextActivities.parent != null)
            {
                foreach (TinCan.Activity cat in contextActivities.parent)
                {
                    LogContextActivity act = new LogContextActivity();
                    act.LogActivityId = (long)ConvertActivity(cat);
                    act.LogContextActivityTypeId = (long)LogContextActivityTypes.Parent;
                    act.LogContextId = db_obj.LogContextId;
                    ErrorHelper.ThrowErrorIfNotNull(act.OnInsert());
                }
            }
        }
        private static long? ConvertActivity(TinCan.StatementTarget statementTarget)
        {
            if (statementTarget is TinCan.Activity)
            {
                TinCan.Activity obj = (statementTarget as TinCan.Activity);

                LogActivity db_obj = new LogActivity();
                db_obj.Id = ""+obj.id;
                db_obj.LogActivityDefinitionId = ConvertActivityDefinition(obj.definition);

                db_obj.OnInsert();

                ConvertActivityDefinitionDetail(db_obj, obj.definition);

                return db_obj.LogActivityId;
            }
            return null;
        }
        private static void ConvertActivityDefinitionDetail(LogActivity db_obj, TinCan.ActivityDefinition activityDefinition)
        {
            Dictionary<string, string> language_map = ConvertLanguageMapToDictionary(activityDefinition.name);
            foreach (string key in language_map.Keys)
            {
                LogActivityDefinitionDetail db_label = new LogActivityDefinitionDetail();
                db_label.Language = key;
                db_label.Label = "" + language_map[key];
                db_label.LogActivityId = db_obj.LogActivityId;
                db_label.LogActivityDefinitionDetailTypeId = (long)LogActivityDefinitionDetailTypes.Name;

                ErrorHelper.ThrowErrorIfNotNull(db_label.OnInsert());
            }

            language_map = ConvertLanguageMapToDictionary(activityDefinition.description);
            foreach (string key in language_map.Keys)
            {
                LogActivityDefinitionDetail db_label = new LogActivityDefinitionDetail();
                db_label.Language = key;
                db_label.Label = "" + language_map[key];
                db_label.LogActivityId = db_obj.LogActivityId;
                db_label.LogActivityDefinitionDetailTypeId = (long)LogActivityDefinitionDetailTypes.Description;

                ErrorHelper.ThrowErrorIfNotNull(db_label.OnInsert());
            }
        }
        private static Dictionary<string, long> m_activityDefinitionIds = new Dictionary<string, long>();
        private static long? ConvertActivityDefinition(TinCan.ActivityDefinition obj)
        {
            if (obj == null)
                return null;

            string key = "" + obj.type + "|||" + obj.moreInfo;
            if (m_activityDefinitionIds.ContainsKey(key))
                return m_activityDefinitionIds[key];

            if (string.IsNullOrWhiteSpace(""+obj.moreInfo))
                obj.moreInfo = null;

            LogActivityDefinitionCollection check = new LogActivityDefinitionCollection();
            check.FillCollection("Type=" + Kernel.MakeSqlSafe(""+obj.type) + " AND MoreInfo=" + Kernel.MakeSqlSafe(""+obj.moreInfo), "");
            if (check.Count > 0)//LogActivityDefinition already exists, so use existing record.
            {
                m_activityDefinitionIds.Add(key, check[0].LogActivityDefinitionId);
                return check[0].LogActivityDefinitionId;
            }

            LogActivityDefinition db_obj = new LogActivityDefinition();
            db_obj.MoreInfo = ""+obj.moreInfo;
            db_obj.Type = "" + obj.type;

            ErrorHelper.ThrowErrorIfNotNull(db_obj.OnInsert());
            m_verbIds.Add(key, db_obj.LogActivityDefinitionId);

            ConvertExtensions(db_obj, obj.extensions);

            return db_obj.LogActivityDefinitionId;
        }
        private static void ConvertExtensions(object db_obj, TinCan.Extensions extensions)
        {
            Dictionary<string, string> exts = ConvertExtensionsToDictionary(extensions);
            foreach(string key in exts.Keys)
            {
                LogExtension ext = new LogExtension();
                if (db_obj is LogActivityDefinition)
                    ext.LogActivityDefinitionId = (db_obj as LogActivityDefinition).LogActivityDefinitionId;
                else if (db_obj is LogActivity)
                    ext.LogActivityId = (db_obj as LogActivity).LogActivityId;
                else if (db_obj is LogContext)
                    ext.LogContextId = (db_obj as LogContext).LogContextId;
                else if (db_obj is LogResult)
                    ext.LogResultId = (db_obj as LogResult).LogResultId;
                else
                    ErrorHelper.ThrowErrorIfNotNull(new LMG.Infrastructure.Analytics.Objects.ErrorLog("Unsupported object for LogExtension"));

                ext.Uri = key;
                ext.Token = exts[key];

                ErrorHelper.ThrowErrorIfNotNull(ext.OnInsert());
            }
        }
        private static List<long> m_agentIds = new List<long>();
        private static long? ConvertAgent(TinCan.StatementTarget statementTarget, bool isAuthority = false)
        {
            if (statementTarget is TinCan.Agent)
            {
                TinCan.Agent obj = (statementTarget as TinCan.Agent);

                long id;
                if (isAuthority)
                {
                    id = 0;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(obj.name))
                    {
                        //Fallback for UCLan logs: see if there's a name in the actor.account
                        if (obj.account != null && !string.IsNullOrWhiteSpace(obj.account.name))
                            obj.name = obj.account.name;//Take name from account, and see if we can get through the rest of the checks.
                        else
                            throw new Warning("Empty agent name.");
                    }

                    if (!long.TryParse(obj.name, out id))
                        throw new Warning("Agent name could not be converted to a number (" + obj.name + ").");
                }

                if (m_agentIds.Contains(id))
                    return id;

                LogAgentCollection check = new LogAgentCollection();
                check.FillCollection("LogAgentId=" + Kernel.MakeSqlSafe(id), "");
                if (check.Count > 0)//Agent already exists; flag as such.
                {
                    m_agentIds.Add(id);
                    return id;
                }

                LogAgent db_obj = new LogAgent();
                db_obj.Name = obj.name;
                db_obj.LogAgentId = id;
                db_obj.Mbox = obj.mbox;
                db_obj.Mbox_sha1sum = obj.mbox_sha1sum;
                db_obj.OpenId = obj.openid;
                db_obj.LogAgentAccountId = ConvertAgentAccount(obj.account);
                

                ErrorHelper.ThrowErrorIfNotNull(db_obj.OnInsert());
                m_agentIds.Add(db_obj.LogAgentId);

                return db_obj.LogAgentId;
            }
            return null;
        }
        private static long? ConvertAgentAccount(TinCan.AgentAccount obj)
        {
            if (obj == null)
                return null;

            LogAgentAccount db_obj = new LogAgentAccount();
            db_obj.Name = obj.name;
            db_obj.Homepage = ""+obj.homePage;
            
            ErrorHelper.ThrowErrorIfNotNull(db_obj.OnInsert());

            return db_obj.LogAgentAccountId;
        }
        private static Guid? ConvertStatementRef(TinCan.StatementTarget statementTarget)
        {
            if (statementTarget is TinCan.StatementRef)
            {
                TinCan.StatementRef obj = (statementTarget as TinCan.StatementRef);

                return obj.id;
            }
            return null;
        }
        private static Dictionary<string, long> m_verbIds = new Dictionary<string, long>();
        private static long? ConvertVerb(TinCan.Verb obj)
        {
            if (obj == null)
                return null;

            string lookup_key = (""+obj.id).ToLower();
            if(m_verbIds.ContainsKey(lookup_key))
                return m_verbIds[lookup_key];

            LogVerbCollection check = new LogVerbCollection();
            check.FillCollection("lower(uri)=" + Kernel.MakeSqlSafe(lookup_key), "");
            if (check.Count > 0)//Verb already exists; get existing value.
            {
                m_verbIds[lookup_key] = check[0].LogVerbId;
                return check[0].LogVerbId;
            }

            LogVerb db_obj = new LogVerb();
            db_obj.Uri = ""+obj.id;

            ErrorHelper.ThrowErrorIfNotNull(db_obj.OnInsert());
            m_verbIds.Add(lookup_key, db_obj.LogVerbId);

            Dictionary<string, string> language_map = ConvertLanguageMapToDictionary(obj.display);
            foreach (string key in language_map.Keys)//KeyValuePair<String, JToken> entry in obj.display.ToJObject())
            {
                LogVerbLabel db_label = new LogVerbLabel();
                db_label.Language = key;
                db_label.Label = "" + language_map[key];
                db_label.LogVerbId = db_obj.LogVerbId;

                ErrorHelper.ThrowErrorIfNotNull(db_label.OnInsert());
            }
            
            return db_obj.LogVerbId;
        }

        private static Dictionary<string, string> ConvertLanguageMapToDictionary(TinCan.LanguageMap map)
        {
            if (map == null)
                return new Dictionary<string, string>();
            return ConvertJObjectToDictionary(map.ToJObject());
        }
        private static Dictionary<string, string> ConvertExtensionsToDictionary(TinCan.Extensions extensions)
        {
            if (extensions == null)
                return new Dictionary<string, string>();
            return ConvertJObjectToDictionary(extensions.ToJObject());
        }
        private static Dictionary<string, string> ConvertJObjectToDictionary(JObject jobject)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (KeyValuePair<string, JToken> entry in jobject)
            {
                dict.Add(entry.Key, "" + entry.Value);
            }
            return dict;
        }
        private static string GetTimestampForLog()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
