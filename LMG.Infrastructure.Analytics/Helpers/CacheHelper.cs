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
using LMG.Infrastructure.Analytics.Objects.DB.Base;
using LMG.Infrastructure.Analytics.Objects.Types;
using OriginDatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMG.Infrastructure.Analytics.Helpers
{
    public static class CacheHelper
    {
        public static LogVerb GetVerb(Verbs verb)
        {
            return GetVerb((long)verb);
        }
        public static LogVerb GetVerb(long verbid)
        {
            LogVerbCollection coll = new LogVerbCollection();
            coll.FillCollection("LogVerbId=" + verbid + "", "");
            if (coll.Count == 1)
            {
                return coll[0];
            }
            return null;
        }
        public static LogVerbLabelCollection GetVerbLabels(Verbs verb)
        {
            return GetVerbLabels((long)verb);
        }
        public static LogVerbLabelCollection GetVerbLabels(long verbid)
        {
            LogVerbLabelCollection coll = new LogVerbLabelCollection();
            coll.FillCollection("LogVerbId=" + verbid + "", "");
            return coll;
        }

        public static Dictionary<long, LogAgent> m_cached_agents = new Dictionary<long, LogAgent>();
        public static LogAgent GetAgent(long id)
        {
            if (!BaseObject.IsPrimaryKeyValid(id))
                return null;
            if (!m_cached_agents.ContainsKey(id))
            {
                LogAgentCollection col = new LogAgentCollection();
                col.FillCollection("LogAgentId=" + SqlValue.Convert(id), "");
                if (col.Count > 0)
                {
                    m_cached_agents.Add(id, col[0]);
                }
                else if (col.Count == 0)
                {//try students
                    return GetStudent(id);
                }
            }
            return m_cached_agents[id];
        }
        private static Dictionary<long, LogAgent> m_cached_students = new Dictionary<long, LogAgent>();
        private static LogAgent GetStudent(long id)
        {
            if (!BaseObject.IsPrimaryKeyValid(id))
                return null;
            if (!m_cached_agents.ContainsKey(id))
            {
                StudentsCollection col = new StudentsCollection();
                col.FillCollection("Id=" + SqlValue.Convert(id), "");
                if (col.Count > 0)
                {
                    LogAgent agent = new LogAgent();
                    agent.LogAgentId = col[0].Id;
                    agent.StudentId = col[0].Id;
                    agent.Mbox = col[0].EmailAddress;
                    agent.Name = col[0].FirstName + " " + col[0].LastName;
                    ErrorLog error = agent.OnInsert();//rmenten 2016-03-30: create the LogAgent with the student details. 
                    //TODO: rmenten 2016-03-30: check if JIT-creation of LogAgent for a Student is desired.
                    if (error != null) error.Throw();
                    m_cached_students.Add(id, agent);
                }
                else
                {
                    return null;
                }
            }
            return m_cached_students[id];
        }

        public static Dictionary<long, LogContext> m_cached_contexts = new Dictionary<long, LogContext>();
        public static LogContext GetContext(long id)
        {
            if (!BaseObject.IsPrimaryKeyValid(id))
                return null;
            if (!m_cached_contexts.ContainsKey(id))
            {
                LogContextCollection col = new LogContextCollection();
                col.FillCollection("LogContextId=" + SqlValue.Convert(id), "");
                if (col.Count > 0)
                    m_cached_contexts.Add(id, col[0]);
            }
            return m_cached_contexts[id];
        }
        public static LogResult GetResult(long id)
        {//No need to cache this, since each is unique
            if (!BaseObject.IsPrimaryKeyValid(id))
                return null;

            LogResultCollection col = new LogResultCollection();
            col.FillCollection("LogResultId=" + SqlValue.Convert(id), "");
            if (col.Count > 0)
                return (col[0]);

            return null;
        }
        //public static Dictionary<long, LogScore> m_cached_contexts = new Dictionary<long, LogScore>();
        public static LogScore GetScore(long id)
        {//No need to cache this, since each is unique
            if (!BaseObject.IsPrimaryKeyValid(id))
                return null;
            
            LogScoreCollection col = new LogScoreCollection();
            col.FillCollection("LogScoreId=" + SqlValue.Convert(id), "");
            if (col.Count > 0)
                return (col[0]);
            
            return null;
        }


        public static LogExtensionCollection GetExtensions(LogContext record)
        {
            if (record == null)
                return new LogExtensionCollection();
            return GetExtensions(record.LogContextId, null, null, null);
        }
        public static LogExtensionCollection GetExtensions(LogResult record)
        {
            if (record == null)
                return new LogExtensionCollection();
            return GetExtensions(null, record.LogResultId, null, null);
        }
        public static LogExtensionCollection GetExtensions(LogActivityDefinition record)
        {
            if (record == null)
                return new LogExtensionCollection();
            return GetExtensions(null, null, record.LogActivityDefinitionId, null);
        }
        public static LogExtensionCollection GetExtensions(LogActivity record)
        {
            if (record == null)
                return new LogExtensionCollection();
            return GetExtensions(null, null, record.LogActivityDefinitionId, record.LogActivityId);
        }
        private static LogExtensionCollection GetExtensions(long? contextId, long? resultId, long? activityDefinitionId, long? activityId)
        {
            LogExtensionCollection list = new LogExtensionCollection();
            if (!contextId.HasValue && !resultId.HasValue && !activityDefinitionId.HasValue)
                return list;

            string where = "";
            if (contextId.HasValue)
                where += "LogContextId=" + SqlValue.Convert(contextId.Value);
            else if (resultId.HasValue)
                where += "LogResultId=" + SqlValue.Convert(resultId.Value);
            else if (activityDefinitionId.HasValue || activityId.HasValue)
            {
                bool both = (activityDefinitionId.HasValue && activityId.HasValue);
                string tmp = "";
                if(activityDefinitionId.HasValue)
                    tmp += "LogActivityDefinitionId=" + SqlValue.Convert(activityDefinitionId.Value);
                if (both)
                    tmp += " OR ";
                if (activityDefinitionId.HasValue)
                    tmp += "LogActivityId=" + SqlValue.Convert(activityId.Value);
                if (both)
                    where += "("+tmp+")";
            }
            
            list.FillCollection(where, null);
            
            if (contextId.HasValue)
            {//rmenten 2016-10-19: Manually add the platform version to every context extension list
                list.Add(new LogExtension(){ 
                    LogContextId = contextId, 
                    Uri = GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.PlatformVersion), 
                    Token = Kernel.PlatformVersion });
            }

            return list;
        }

        public static Dictionary<long, LogActivity> m_cached_activities = new Dictionary<long, LogActivity>();
        public static LogActivity GetActivity(long id)
        {
            if (!BaseObject.IsPrimaryKeyValid(id))
                return null;
            if (!m_cached_activities.ContainsKey(id))
            {
                LogActivityCollection col = new LogActivityCollection();
                col.FillCollection("LogActivityId=" + SqlValue.Convert(id), "");
                if (col.Count > 0)
                    m_cached_activities.Add(id, col[0]);
            }
            return m_cached_activities[id];
        }
        public static Dictionary<long, LogActivityDefinition> m_cached_activity_definitions = new Dictionary<long, LogActivityDefinition>();
        public static LogActivityDefinition GetActivityDefinition(LogActivityDefinitions? logActivityDefinitions)
        {
            if (logActivityDefinitions.HasValue)
                return GetActivityDefinition((long)logActivityDefinitions.Value);
            else
                return null;
        }
        public static LogActivityDefinition GetActivityDefinition(long id)
        {
            if (!BaseObject.IsPrimaryKeyValid(id))
                return null;
            if (!m_cached_activity_definitions.ContainsKey(id))
            {
                LogActivityDefinitionCollection col = new LogActivityDefinitionCollection();
                col.FillCollection("LogActivityDefinitionId=" + SqlValue.Convert(id), "");
                if (col.Count > 0)
                    m_cached_activity_definitions.Add(id, col[0]);
            }
            return m_cached_activity_definitions[id];
        }
        public static LogActivityDefinitions? GetLogActivityDefinitionForAccessType(AccessTypes accessTypes)
        {
            switch (accessTypes)
            {
                case AccessTypes.LearningModule: return LogActivityDefinitions.Course;
                case AccessTypes.LearningModuleProfile: return LogActivityDefinitions.Module;
                case AccessTypes.TheoryPage: return LogActivityDefinitions.Page_Theory;
                case AccessTypes.TheoryPageTreeItem: return LogActivityDefinitions.Module;
                case AccessTypes.Exercise: return LogActivityDefinitions.Interaction;
                case AccessTypes.ExerciseTreeItem: return LogActivityDefinitions.Module;
                case AccessTypes.Dictionary: return LogActivityDefinitions.Page_Dictionary;
                case AccessTypes.LinkExternal: return LogActivityDefinitions.Link_External;
                case AccessTypes.Hint: return LogActivityDefinitions.Article_Hint;
                case AccessTypes.Solution: return LogActivityDefinitions.Article_Solution;
                case AccessTypes.Feedback: return LogActivityDefinitions.Article_Feedback;
                case AccessTypes.ExploreHotspot: return LogActivityDefinitions.Article_ExploreHotspot;
                case AccessTypes.Other:
                default: return null;
            }
        }

        public static Dictionary<long, LogActivityDefinitionDetailCollection> m_cached_activity_definition_details = new Dictionary<long, LogActivityDefinitionDetailCollection>();
        public static LogActivityDefinitionDetailCollection GetLogActivityDefinitionDetails(LogActivity record)
        {
            if (record == null)
                return null;
            long id = record.LogActivityId;
            if (!m_cached_activity_definition_details.ContainsKey(id))
            {
                LogActivityDefinitionDetailCollection col = new LogActivityDefinitionDetailCollection();
                col.FillCollection("LogActivityId=" + SqlValue.Convert(id), "");
                m_cached_activity_definition_details.Add(id, col);
            }
            return m_cached_activity_definition_details[id];
        }

        public static LogContextActivityCollection GetLogContextActivities(long contextId)
        {
            //No need to cache this, since each is unique
            if (!BaseObject.IsPrimaryKeyValid(contextId))
                return null;

            LogContextActivityCollection col = new LogContextActivityCollection();
            col.FillCollection("LogContextId=" + SqlValue.Convert(contextId), "");
            return col;
        }

        public static LogStatementLinkCollection GetLogStatementLinks(Guid logStatementId)
        {
            //No need to cache this, since each is unique
            LogStatementLinkCollection col = new LogStatementLinkCollection();
            col.FillCollection("LogStatementId=" + SqlValue.Convert(logStatementId), "");
            return col;
        }


        public static string GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions ext)
        {
            return GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensionsConverter.Convert(ext));
        }
        public static string GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionVerbs verb)
        {
            return GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionVerbsConverter.Convert(verb));
        }
        public static string GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionActivities verb)
        {
            return GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionActivitiesConverter.Convert(verb));
        }
        public static string GetXAPIDefinitionsBaseURL(string suffix = null)
        {
            return "http://www.project-vital.eu/xapi/" + suffix;
        }

        public static string GetExerciseDescriptionFromMetadata(string exerciseId, bool isTheoryPage = false)
        {
            try
            {
                //rmenten 2016-04-22: isTheoryPage not used yet. Is there an equivalent metadata field for the "Exercise number"

                SqlQuerySelect sel = new SqlQuerySelect();
                sel.Select("MetadataValue.Value");
                sel.From("MetadataValue");
                sel.InnerJoin("MetadataField", "MetadataValue.MetadataFieldId = MetadataField.MetadataFieldId");
                sel.Where("(MetadataField.FieldName = 'Exercise number' OR MetadataField.FieldName = 'Oefeningnummer')");
                sel.Where("MetadataValue.ExerciseId = " + exerciseId);
                SqlResult res = Kernel.Connection.ExecuteQuery(sel);
                if (res.RowCount > 0)
                {
                    return res.GetString(0, 0, null);
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        public static string GetExerciseName(string exerciseId)
        {
            try
            {
                SqlQuerySelect sel = new SqlQuerySelect();
                sel.Select("Title");
                sel.From("Exercise");
                sel.Where("ExerciseId = " + exerciseId);
                SqlResult res = Kernel.Connection.ExecuteQuery(sel);
                if (res.RowCount > 0)
                {
                    return res.GetString(0, 0, null);
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        public static string GetTheoryPageName(string theoryPageId)
        {
            try
            {
                SqlQuerySelect sel = new SqlQuerySelect();
                sel.Select("Title");
                sel.From("TheoryPage");
                sel.Where("TheoryPageId = " + theoryPageId);
                SqlResult res = Kernel.Connection.ExecuteQuery(sel);
                if (res.RowCount > 0)
                {
                    return res.GetString(0, 0, null);
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        public static string GetLearningModuleName(string learningModuleId)
        {
            try
            {
                SqlQuerySelect sel = new SqlQuerySelect();
                sel.Select("Title");
                sel.From("LearningModule");
                sel.Where("LearningModuleId = " + learningModuleId);
                SqlResult res = Kernel.Connection.ExecuteQuery(sel);
                if (res.RowCount > 0)
                {
                    return res.GetString(0, 0, null);
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        public static string GetLearningModuleDescription(string learningModuleId)
        {
            try
            {
                SqlQuerySelect sel = new SqlQuerySelect();
                sel.Select("SubTitle");//Description
                sel.From("LearningModule");
                sel.Where("LearningModuleId = " + learningModuleId);
                SqlResult res = Kernel.Connection.ExecuteQuery(sel);
                if (res.RowCount > 0)
                {
                    return res.GetString(0, 0, null);
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        public static string GetLearningModuleInstructionLanguage(long learningModuleId)
        {
            try
            {
                SqlQuerySelect sel = new SqlQuerySelect();
                sel.Select("InstructionLanguage");
                sel.From("LearningModule");
                sel.Where("LearningModuleId", SqlValue.Convert(learningModuleId));
                SqlResult res = Kernel.Connection.ExecuteQuery(sel);
                if (res.RowCount > 0)
                {
                    string instrLanguage = res.GetString(0, 0, "");
                    if (!string.IsNullOrWhiteSpace(instrLanguage))
                    {
                        return CleanInstructionLanguage(instrLanguage);
                    }
                }
            }
            catch
            {
            }

            return null;
        }
        private static string CleanInstructionLanguage(string lang)
        {
            lang = lang.ToLower();

            if (lang == "deutsch" || lang == "duits")
                return "de";
            if (lang == "english" || lang == "engels")
                return "en";
            if (lang == "francais"|| lang == "français" || lang == "frans")
                return "fr";
            if (lang == "nederlands")
                return "nl";
            if (lang == "arabic")
                return "ar";

            if (lang == "en-uk" || lang == "en-us" || lang == "en" || lang == "fr-fr" || lang == "fr-be" || lang == "fr" || lang == "nl-nl" || lang == "nl-be" || lang == "nl" || lang == "de-de" || lang == "de")
                return lang;

            if (System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures).Any(culture => string.Equals(culture.Name, lang, StringComparison.CurrentCultureIgnoreCase)))
            {
                return lang;
            }

            return null;//unknown language
        }
        public static Dictionary<long, string> m_cached_exercise_types = null;
        public static string GetExerciseType(long? exerciseId)
        {
            if(!exerciseId.HasValue)
                return null;

            if(m_cached_exercise_types == null)
                m_cached_exercise_types = new Dictionary<long, string>();
            else if (m_cached_exercise_types.ContainsKey(exerciseId.Value))
                return m_cached_exercise_types[exerciseId.Value];

            try
            {
                SqlQuerySelect sel = new SqlQuerySelect();
                sel.Select("ExerciseType");
                sel.From("Exercise");
                sel.Where("ExerciseId = " + exerciseId);
                SqlResult res = Kernel.Connection.ExecuteQuery(sel);
                if (res.RowCount > 0)
                {
                    string type = res.GetString(0, 0, null);
                    m_cached_exercise_types.Add(exerciseId.Value, type);
                    return type;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        public static Dictionary<string, string> m_cached_locate_caption_names = null;
        public static Dictionary<string, string> m_cached_locate_caption_value_types = null;
        public static string GetLocateCaptionName(long exerciseId, string dragId, ref string valueType)
        {
            string key = dragId + "@" + exerciseId;
            if (m_cached_locate_caption_names == null)
            {
                m_cached_locate_caption_names = new Dictionary<string, string>();
                m_cached_locate_caption_value_types = new Dictionary<string, string>();
            }
            else if (m_cached_locate_caption_names.ContainsKey(key))
            {
                if (m_cached_locate_caption_value_types.ContainsKey(key))
                    valueType = m_cached_locate_caption_value_types[key];
                return m_cached_locate_caption_names[key];
            }

            try
            {
                SqlQuerySelect sel = new SqlQuerySelect();
                sel.Select("LocateHotspotCaptions.CaptionPlainTextValue");
                sel.Select("LocateHotspotCaptions.CaptionValueType");
                sel.From("LocateHotspotCaptions");
                sel.InnerJoin("LocateHotspot", "LocateHotspot.LocateHotspotId = LocateHotspotCaptions.LocateHotspot_Id");
                sel.Where("LocateHotspot.LocateExerciseId = " + exerciseId);
                sel.Where("LocateHotspotCaptions.captionkey", SqlValue.Convert(dragId));
                SqlResult res = Kernel.Connection.ExecuteQuery(sel);
                if (res.RowCount > 0)
                {
                    string name = res.GetString(0, 0, null);
                    m_cached_locate_caption_names.Add(key, name);
                    valueType = res.GetString(0, 1, null);
                    m_cached_locate_caption_value_types.Add(key, valueType);
                    return name;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        public static Dictionary<string, string> m_cached_locate_hotspot_names = null;
        public static string GetLocateHotspotName(long exerciseId, string dropTargetId)
        {
            string key = dropTargetId + "@" + exerciseId;
            if (m_cached_locate_hotspot_names == null)
                m_cached_locate_hotspot_names = new Dictionary<string, string>();
            else if (m_cached_locate_hotspot_names.ContainsKey(key))
                return m_cached_locate_hotspot_names[key];

            try
            {
                SqlQuerySelect sel = new SqlQuerySelect();
                sel.Select("Name");
                sel.From("LocateHotspot");
                sel.Where("LocateExerciseId = " + exerciseId);
                sel.Where("HotspotKey", SqlValue.Convert(dropTargetId));
                SqlResult res = Kernel.Connection.ExecuteQuery(sel);
                if (res.RowCount > 0)
                {
                    string name = res.GetString(0, 0, null);
                    m_cached_locate_hotspot_names.Add(key, name);
                    return name;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        public static Dictionary<string, string> m_cached_discover_item_names = null;
        public static Dictionary<string, string> m_cached_discover_item_value_types = null;
        public static string GetDiscoverItemName(long exerciseId, string itemId)
        {
            string dummy = "";
            return GetDiscoverItemName(exerciseId, itemId, ref dummy);
        }
        public static string GetDiscoverItemName(long exerciseId, string itemId, ref string valueType)
        {
            string key = itemId + "@" + exerciseId;
            if (m_cached_discover_item_names == null)
            {
                m_cached_discover_item_names = new Dictionary<string, string>();
                m_cached_discover_item_value_types = new Dictionary<string, string>();
            }
            else if (m_cached_discover_item_names.ContainsKey(key))
            {
                if (m_cached_discover_item_value_types.ContainsKey(key))
                    valueType = m_cached_discover_item_value_types[key];
                return m_cached_discover_item_names[key];
            }

            try
            {
                SqlQuerySelect sel = new SqlQuerySelect();
                sel.Select("coalesce(texthint, Imagefilename)");
                sel.Select("hintmediatype");
                sel.From("DiscoverImage");
                sel.Where("DiscoverImageId = " + itemId);
                SqlResult res = Kernel.Connection.ExecuteQuery(sel);
                if (res.RowCount > 0)
                {
                    string name = res.GetString(0, 0, null);
                    m_cached_discover_item_names.Add(key, name);
                    valueType = res.GetString(0, 1, null);
                    m_cached_discover_item_value_types.Add(key, valueType);
                    return name;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        #region LogMetadata
        private static Dictionary<string, long> m_cache_GetOrCreateLogMetadataTypeId = new Dictionary<string, long>();
        public static long GetOrCreateLogMetadataTypeId(string column_name)
        {
            string lookup_column_name = ("" + column_name).ToLower();

            if (m_cache_GetOrCreateLogMetadataTypeId.ContainsKey(lookup_column_name))
            {
                return m_cache_GetOrCreateLogMetadataTypeId[lookup_column_name];
            }

            long result = -1;
            LogMetadataTypeCollection types = new LogMetadataTypeCollection();
            types.FillCollection("lower(descr) = " + Kernel.MakeSqlSafe(lookup_column_name), "");
            if (types.Count > 0)
            {
                result = types[0].LogMetadataTypeId;
            }
            else
            {
                LogMetadataType type = new LogMetadataType();
                type.Descr = column_name;
                if (type.OnInsert() == null
                    && type.LogMetadataTypeId != BaseObject.InvalidPrimaryKey)
                {
                    result = type.LogMetadataTypeId;
                }
                else
                {
                    throw new Exception("LogMetadataTypeId Insert failed.");
                }
            }

            m_cache_GetOrCreateLogMetadataTypeId.Add(lookup_column_name, result);
            return result;
        }
        #endregion

        public static void InstallDatabaseConstants(bool clearFirst = false)
        {
            if (clearFirst)
            {
                ClearDatabaseTable("LogVerb");
                ClearDatabaseTable("LogVerbLabel");
                //ClearDatabaseTable("LogActivity");
                ClearDatabaseTable("LogActivityDefinition");
                //ClearDatabaseTable("LogActivityDefinitionDetail");
                ClearDatabaseTable("LogActivityDefinitionDetailType");
                ClearDatabaseTable("LogContextActivityType");
                ClearDatabaseTable("LogAgent");
            }

            string Language_EN = "en-uk";
            string Language_NL = "nl-be";

            #region Verbs
            SqlQuerySelect sel = new SqlQuerySelect();
            sel.Select("count(*)").From("LogVerb");
            SqlResult result = Kernel.Connection.ExecuteQuery(sel);
            if (result.GetInt32(0, 0, 0) == 0)
            {
                List<LogVerb> verbs = new List<LogVerb>();
                verbs.Add(new LogVerb() { LogVerbId = 1, Uri = "https://w3id.org/xapi/adl/verbs/logged-in" });
                verbs.Add(new LogVerb() { LogVerbId = 2, Uri = "https://w3id.org/xapi/adl/verbs/logged-out" });
                verbs.Add(new LogVerb() { LogVerbId = 3, Uri = "http://activitystrea.ms/schema/1.0/access" });
                verbs.Add(new LogVerb() { LogVerbId = 4, Uri = "http://activitystrea.ms/schema/1.0/complete" });
                verbs.Add(new LogVerb() { LogVerbId = 5, Uri = "http://adlnet.gov/expapi/verbs/attempted" });
                verbs.Add(new LogVerb() { LogVerbId = 6, Uri = "http://activitystrea.ms/schema/1.0/search" });
                verbs.Add(new LogVerb() { LogVerbId = 7, Uri = "http://id.tincanapi.com/verb/viewed" });
                verbs.Add(new LogVerb() { LogVerbId = 8, Uri = GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionVerbs.VoiceRecorded) });
                verbs.Add(new LogVerb() { LogVerbId = 9, Uri = "http://activitystrea.ms/schema/1.0/play" });
                verbs.Add(new LogVerb() { LogVerbId = 10, Uri = "http://id.tincanapi.com/verb/paused" });
                verbs.Add(new LogVerb() { LogVerbId = 11, Uri = "http://activitystrea.ms/schema/1.0/watch" });
                verbs.Add(new LogVerb() { LogVerbId = 12, Uri = "http://activitystrea.ms/schema/1.0/listen" });
                verbs.Add(new LogVerb() { LogVerbId = 13, Uri = "http://adlnet.gov/expapi/activities/link" });
                verbs.Add(new LogVerb() { LogVerbId = 14, Uri = "http://id.tincanapi.com/verb/previewed" });
                verbs.Add(new LogVerb() { LogVerbId = 15, Uri = GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionVerbs.PrintedToPdf) });
                verbs.Add(new LogVerb() { LogVerbId = 16, Uri = "http://id.tincanapi.com/verb/skipped" });
                verbs.Add(new LogVerb() { LogVerbId = 17, Uri = "http://adlnet.gov/expapi/verbs/interacted" });
                
                foreach (LogVerb verb in verbs)
                {
                    verb.OnInsert();
                }
            }
            #region Verb Labels
            sel = new SqlQuerySelect();
            sel.Select("count(*)").From("LogVerbLabel");
            result = Kernel.Connection.ExecuteQuery(sel);
            if (result.GetInt32(0, 0, 0) == 0)
            {
                List<LogVerbLabel> verbLabels = new List<LogVerbLabel>();
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 1, Language = Language_EN, Label = "logged in" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 1, Language = Language_NL, Label = "ingelogd" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 2, Language = Language_EN, Label = "logged out" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 2, Language = Language_NL, Label = "uitgelogd" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 3, Language = Language_EN, Label = "accessed" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 3, Language = Language_NL, Label = "geopend" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 4, Language = Language_EN, Label = "completed" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 4, Language = Language_NL, Label = "voltooid" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 5, Language = Language_EN, Label = "attempted" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 5, Language = Language_NL, Label = "geprobeerd" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 6, Language = Language_EN, Label = "searched" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 6, Language = Language_NL, Label = "gezocht" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 7, Language = Language_EN, Label = "viewed" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 7, Language = Language_NL, Label = "bekeken" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 8, Language = Language_EN, Label = "recorded" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 8, Language = Language_NL, Label = "opgenomen" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 9, Language = Language_EN, Label = "played" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 9, Language = Language_NL, Label = "afgespeeld" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 10, Language = Language_EN, Label = "paused" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 10, Language = Language_NL, Label = "gepauzeerd" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 11, Language = Language_EN, Label = "watched" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 11, Language = Language_NL, Label = "volledig bekeken" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 12, Language = Language_EN, Label = "listened" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 12, Language = Language_NL, Label = "volledig geluisterd" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 13, Language = Language_EN, Label = "linked" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 13, Language = Language_NL, Label = "gelinkt" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 14, Language = Language_EN, Label = "previewed" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 14, Language = Language_NL, Label = "kort bekeken" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 15, Language = Language_EN, Label = "printed" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 15, Language = Language_NL, Label = "geprint" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 16, Language = Language_EN, Label = "skipped" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 16, Language = Language_NL, Label = "overgeslagen" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 17, Language = Language_EN, Label = "interacted" });
                verbLabels.Add(new LogVerbLabel() { LogVerbId = 17, Language = Language_NL, Label = "geïnterageerd" });

                foreach (LogVerbLabel verbLabel in verbLabels)
                {
                    verbLabel.OnInsert();
                }
            }
            #endregion
            #endregion
            #region Activity
            //todo rmenten 2016-03-21: determine how to specify different types of articles/pages (for hint/help and theory/dictionary)
            #region LogActivityDefinition
            //int LogActivityDefinitionId_Application = 1;
            //int LogActivityDefinitionId_Course = 2;
            //int LogActivityDefinitionId_Module = 3;
            //int LogActivityDefinitionId_Interaction = 4;
            //int LogActivityDefinitionId_Page_Theory = 5;
            //int LogActivityDefinitionId_Article_Hint = 6;
            //int LogActivityDefinitionId_Page_Dictionary = 7;
            //int LogActivityDefinitionId_Article_Help = 8;
            //int LogActivityDefinitionId_Media = 9;
            //int LogActivityDefinitionId_Link_External = 10;
            //int LogActivityDefinitionId_File = 11;
            //Article_Solution = 12
            //Article_Feedback = 13
            //Article_ExploreHotspot = 14

            sel = new SqlQuerySelect();
            sel.Select("count(*)").From("LogActivityDefinition");
            result = Kernel.Connection.ExecuteQuery(sel);
            if (result.GetInt32(0, 0, 0) == 0)
            {
                List<LogActivityDefinition> list = new List<LogActivityDefinition>();
                list.Add(new LogActivityDefinition() { /* Application             , */Type = "http://activitystrea.ms/schema/1.0/application" });
                list.Add(new LogActivityDefinition() { /* Course                  , */Type = "http://adlnet.gov/expapi/activities/course" });
                list.Add(new LogActivityDefinition() { /* Module                  , */Type = "http://adlnet.gov/expapi/activities/module" });
                list.Add(new LogActivityDefinition() { /* Interaction             , */Type = "http://adlnet.gov/expapi/activities/interaction" });
                list.Add(new LogActivityDefinition() { /* Page_Theory             , */Type = "http://activitystrea.ms/schema/1.0/page" });
                list.Add(new LogActivityDefinition() { /* Article_Hint            , */Type = "http://activitystrea.ms/schema/1.0/article" });
                list.Add(new LogActivityDefinition() { /* Page_Dictionary         , */Type = "http://activitystrea.ms/schema/1.0/page" });
                list.Add(new LogActivityDefinition() { /* Article_Help            , */Type = "http://activitystrea.ms/schema/1.0/article" });
                list.Add(new LogActivityDefinition() { /* Media                   , */Type = "http://adlnet.gov/expapi/activities/media" });
                list.Add(new LogActivityDefinition() { /* Link_External           , */Type = "http://adlnet.gov/expapi/activities/link" });
                list.Add(new LogActivityDefinition() { /* File                    , */Type = "http://activitystrea.ms/schema/1.0/file" });
                list.Add(new LogActivityDefinition() { /* Article_Solution        , */Type = "http://activitystrea.ms/schema/1.0/article" });
                list.Add(new LogActivityDefinition() { /* Article_Feedback        , */Type = "http://activitystrea.ms/schema/1.0/article" });
                list.Add(new LogActivityDefinition() { /* Article_ExploreHotspot  , */Type = "http://activitystrea.ms/schema/1.0/article" });
                                    
                foreach (LogActivityDefinition item in list)
                {
                    item.OnInsert();
                }
            }
            #endregion
            #region LogActivityDefinitionDetailType
            //int LogActivityDefinitionDetailType_nameId = 1;
            //int LogActivityDefinitionDetailType_descrId = 2;
            sel = new SqlQuerySelect();
            sel.Select("count(*)").From("LogActivityDefinitionDetailType");
            result = Kernel.Connection.ExecuteQuery(sel);
            if (result.GetInt32(0, 0, 0) == 0)
            {
                List<LogActivityDefinitionDetailType> list = new List<LogActivityDefinitionDetailType>();
                list.Add(new LogActivityDefinitionDetailType() { /*LogActivityDefinitionDetailTypeId = LogActivityDefinitionDetailType_nameId,  */  Label = "name" });
                list.Add(new LogActivityDefinitionDetailType() { /*LogActivityDefinitionDetailTypeId = LogActivityDefinitionDetailType_descrId, */  Label = "description" });

                foreach (LogActivityDefinitionDetailType item in list)
                {
                    item.OnInsert();
                }
            }
            #endregion
            #region LogActivityDefinitionDetail
            //sel = new SqlQuerySelect();
            //sel.Select("count(*)").From("LogActivityDefinitionDetail");
            //result = Kernel.Connection.ExecuteQuery(sel);
            //if (result.GetInt32(0, 0, 0) == 0)
            //{
            //    List<LogActivityDefinitionDetail> list = new List<LogActivityDefinitionDetail>();
            //    list.Add(new LogActivityDefinitionDetail() { LogActivityDefinitionId = LogActivityDefinitionId_Application, Language = Language_EN, Label = "Application", LogActivityDefinitionDetailTypeId = LogActivityDefinitionDetailType_nameId });
            //    list.Add(new LogActivityDefinitionDetail() { LogActivityDefinitionId = LogActivityDefinitionId_Course, Language = Language_EN, Label = "Course", LogActivityDefinitionDetailTypeId = LogActivityDefinitionDetailType_nameId });
            //    list.Add(new LogActivityDefinitionDetail() { LogActivityDefinitionId = LogActivityDefinitionId_Module, Language = Language_EN, Label = "Module", LogActivityDefinitionDetailTypeId = LogActivityDefinitionDetailType_nameId });
            //    list.Add(new LogActivityDefinitionDetail() { LogActivityDefinitionId = LogActivityDefinitionId_Interaction, Language = Language_EN, Label = "Interaction", LogActivityDefinitionDetailTypeId = LogActivityDefinitionDetailType_nameId });
            //    list.Add(new LogActivityDefinitionDetail() { LogActivityDefinitionId = LogActivityDefinitionId_Page_Theory, Language = Language_EN, Label = "Theory page", LogActivityDefinitionDetailTypeId = LogActivityDefinitionDetailType_nameId });
            //    list.Add(new LogActivityDefinitionDetail() { LogActivityDefinitionId = LogActivityDefinitionId_Article_Hint, Language = Language_EN, Label = "Hint article", LogActivityDefinitionDetailTypeId = LogActivityDefinitionDetailType_nameId });
            //    list.Add(new LogActivityDefinitionDetail() { LogActivityDefinitionId = LogActivityDefinitionId_Page_Dictionary, Language = Language_EN, Label = "Dictionary page", LogActivityDefinitionDetailTypeId = LogActivityDefinitionDetailType_nameId });
            //    list.Add(new LogActivityDefinitionDetail() { LogActivityDefinitionId = LogActivityDefinitionId_Article_Help, Language = Language_EN, Label = "Help article", LogActivityDefinitionDetailTypeId = LogActivityDefinitionDetailType_nameId });
            //    list.Add(new LogActivityDefinitionDetail() { LogActivityDefinitionId = LogActivityDefinitionId_Media, Language = Language_EN, Label = "Media", LogActivityDefinitionDetailTypeId = LogActivityDefinitionDetailType_nameId });

            //    foreach (LogActivityDefinitionDetail item in list)
            //    {
            //        item.OnInsert();
            //    }
            //}
            #endregion
            /*
            sel = new SqlQuerySelect();
            sel.Select("count(*)").From("LogActivity");
            result = Kernel.Connection.ExecuteQuery(sel);
            if (result.GetInt32(0, 0, 0) == 0)
            {
                List<LogActivity> list = new List<LogActivity>();
                list.Add(new LogActivity() { LogActivityDefinitionId = LogActivityDefinitionId_Application, Id = "http://activitystrea.ms/schema/1.0/application" });
                list.Add(new LogActivity() { LogActivityDefinitionId = LogActivityDefinitionId_Course, Id = "http://adlnet.gov/expapi/activities/course" });
                list.Add(new LogActivity() { LogActivityDefinitionId = LogActivityDefinitionId_Module, Id = "http://adlnet.gov/expapi/activities/module" });
                list.Add(new LogActivity() { LogActivityDefinitionId = LogActivityDefinitionId_Interaction, Id = "http://adlnet.gov/expapi/activities/interaction" });
                list.Add(new LogActivity() { LogActivityDefinitionId = LogActivityDefinitionId_Page_Theory, Id = "http://activitystrea.ms/schema/1.0/page" });
                list.Add(new LogActivity() { LogActivityDefinitionId = LogActivityDefinitionId_Article_Hint, Id = "http://activitystrea.ms/schema/1.0/article" });
                list.Add(new LogActivity() { LogActivityDefinitionId = LogActivityDefinitionId_Page_Dictionary, Id = "http://activitystrea.ms/schema/1.0/application" });
                list.Add(new LogActivity() { LogActivityDefinitionId = LogActivityDefinitionId_Article_Help, Id = "http://activitystrea.ms/schema/1.0/article" });
                list.Add(new LogActivity() { LogActivityDefinitionId = LogActivityDefinitionId_Media, Id = "http://adlnet.gov/expapi/activities/media" });

                foreach (LogActivity item in list)
                {
                    item.OnInsert();
                }
            }*/
            #endregion
            #region Context
            #region LogContextActivityType
            sel = new SqlQuerySelect();
            sel.Select("count(*)").From("LogContextActivityType");
            result = Kernel.Connection.ExecuteQuery(sel);
            if (result.GetInt32(0, 0, 0) == 0)
            {
                List<LogContextActivityType> list = new List<LogContextActivityType>();
                list.Add(new LogContextActivityType() { LogContextActivityTypeId = 1, Type = "parent" });
                list.Add(new LogContextActivityType() { LogContextActivityTypeId = 2, Type = "grouping" });
                list.Add(new LogContextActivityType() { LogContextActivityTypeId = 3, Type = "category" });
                list.Add(new LogContextActivityType() { LogContextActivityTypeId = 4, Type = "other" });

                foreach (LogContextActivityType item in list)
                {
                    item.OnInsert();
                }
            }
            #endregion
            #endregion
            #region Test users
            sel = new SqlQuerySelect();
            sel.Select("count(*)").From("LogAgent");
            result = Kernel.Connection.ExecuteQuery(sel);
            if (result.GetInt32(0, 0, 0) == 0)
            {
                LogAgent agent = new LogAgent() { Name = "Raf Menten", Mbox = "raf.menten@uhasselt.be" };
                agent.OnInsert();
            }
            #endregion
        }
        private static void ClearDatabaseTable(string tablename)
        {
            Kernel.Connection.ExecuteQuery(new SqlQueryDelete(tablename).Where("1=1"));
        }

        public static LogStatement GetLastLogStatement(long? logAgentId, string extraWhere = null)
        {
            string sqlWhere = "1=1";
            if (logAgentId.HasValue)
            {
                sqlWhere += " AND LogAgentId = " + logAgentId.Value;
            }
            if (!string.IsNullOrWhiteSpace(extraWhere))
            {
                sqlWhere += " AND " + extraWhere;
            }

            LogStatementCollection statements = new LogStatementCollection();
            statements.FillCollection(sqlWhere, "timestamp desc", 1);
            if (statements.Count > 0)
                return statements[0];
            else
                return null;

        }



        
    }
}
