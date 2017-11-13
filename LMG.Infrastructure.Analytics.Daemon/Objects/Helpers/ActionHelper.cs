using TinCan;
using LMG.Infrastructure.Analytics.Daemon.Objects.Types;
using LMG.Infrastructure.Analytics.Helpers;
using LMG.Infrastructure.Analytics.Objects;
using LMG.Infrastructure.Analytics.Objects.DB;
using LMG.Infrastructure.Analytics.Objects.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LMG.Infrastructure.Analytics.Daemon.Objects.Helpers
{
    public static class ActionHelper
    {
        public static void GeneratePasswordsForLogAgents()
        {
            AnalyticsWorker worker = new AnalyticsWorker();

            LogAgentCollection agents = new LogAgentCollection();
            agents.FillCollection("", "logagentid");

            foreach (LogAgent agent in agents)
            {
                if (string.IsNullOrWhiteSpace(agent.Password))
                {//Add new unencrypted password
                    agent.Password = StringHelper.GenerateRandomString(8);
                    worker.UpdateLogAgentPassword(agent.LogAgentId, agent.Password);
                }
            }
        }
        public static void SupplementLinkDataInDatabaseForUvA()
        {
            try
            {
                AnalyticsWorker worker = new AnalyticsWorker();

                //Get all statements for which there is no content id in the LogStatementLink table.
                //  Then insert the necessary rows.

                //, LogActivity.Id
                string query = @"select top 500 LogStatement.LogStatementId from logstatement
                    inner join LogActivity on LogStatement.TargetLogActivityId=LogActivity.LogActivityId
                    left outer join LogStatementLink exLink on LogStatement.LogStatementId=exLink.LogStatementId and exLink.TableName='PracticePackage'
                    left outer join LogStatementLink tpLink on LogStatement.LogStatementId=tpLink.LogStatementId and tpLink.TableName='Theory'
                    left outer join LogStatementLink testLink on LogStatement.LogStatementId=tpLink.LogStatementId and tpLink.TableName='TestPackage'
                    where exLink.tableid is null AND tplink.tableid is null AND testLink.tableid is null";

                LogStatementCollection statements = worker.GetLogStatements(query);

                foreach (LogStatement statement in statements)
                {
                    if (statement.TargetLogActivityIdResolved == null)
                        continue;

                    string id = statement.TargetLogActivityIdResolved.Id;

                    List<string> prefix_theory = new List<string>() 
                    {
                        "https://uva.sowiso.nl/content/theory/",
                        "https://uva.sowiso.nl/content_ajax/theory"
                    };

                    List<string> prefix_practice_package = new List<string>() 
                    {
                        "https://uva.sowiso.nl/content_ajax/theory_exercise/",
                        "https://uva.sowiso.nl/evaluate_ajax/line/",
                        "https://uva.sowiso.nl/evaluate_ajax/maxima/",
                        "https://uva.sowiso.nl/evaluate_ajax/open_free/",
                        "https://uva.sowiso.nl/evaluate_ajax/sc/",
                        "https://uva.sowiso.nl/evaluate_ajax/solution/",
                        "https://uva.sowiso.nl/evaluate_ajax/text/",
                        "https://uva.sowiso.nl/content/package/",
                        "https://uva.sowiso.nl/test_ajax/did_access_interaction/",
                    };

                    List<string> prefix_test_package = new List<string>() 
                    {
                        "https://uva.sowiso.nl/content/test/",
                        "https://uva.sowiso.nl/test_ajax/test_result/"
                    };

                    List<string> prefix_category = new List<string>() 
                    {
                        "https://uva.sowiso.nl/home_ajax/did_open_category/",
                        "https://uva.sowiso.nl/home/index/",
                     	"https://uva.sowiso.nl/home/course/"
                    };

                    string content_type = "";
                    string content_id = "";
                    foreach (string prefix in prefix_theory)
                    {
                        if (id.StartsWith(prefix))
                        {
                            id = id.Replace(prefix, "");
                            content_id = id;
                            content_type = "Theory";
                            break;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(content_id))
                    {
                        foreach (string prefix in prefix_practice_package)
                        {
                            if (id.StartsWith(prefix))
                            {
                                id = id.Replace(prefix, "");
                                content_id = id;
                                content_type = "Practice package";
                                break;
                            }
                        }
                    }
                    if (string.IsNullOrWhiteSpace(content_id))
                    {
                        foreach (string prefix in prefix_test_package)
                        {
                            if (id.StartsWith(prefix))
                            {
                                id = id.Replace(prefix, "");
                                content_id = id;
                                content_type = "Test package";
                                break;
                            }
                        }
                    }
                    if (string.IsNullOrWhiteSpace(content_id))
                    {
                        foreach (string prefix in prefix_category)
                        {
                            if (id.StartsWith(prefix))
                            {
                                id = id.Replace(prefix, "");
                                content_id = id;
                                content_type = "Category";
                                break;
                            }
                        }
                    }


                    ////id format == http://student.commart.eu/Content/Show/1381/Exercise/21084?theorypageContext=3761
                    //List<string> parts = new List<string>(id.Split(new string[] {"Content/Show/1381/Exercise/", "?theorypageContext="}, StringSplitOptions.RemoveEmptyEntries));
                    ////Ignore parts[0]. It's the prefix of the website: http://student.commart.eu/
                    //string exerciseId = parts[1];
                    //string theoryPageId = parts[2];
                    //if(theoryPageId.Contains("#"))
                    //    theoryPageId = theoryPageId.Substring(0, theoryPageId.IndexOf("#"));

                    if (string.IsNullOrWhiteSpace(content_id))
                        continue;

                    //bool foundTPLink = false;
                    //LogStatementLinkCollection links = CacheHelper.GetLogStatementLinks(statement.LogStatementId);
                    //foreach (LogStatementLink link in links)
                    //{
                    //    if (link.TableName == "TheoryPage")
                    //        foundTPLink = true;
                    //}
                    //if (!foundTPLink)
                    {
                        LogStatementLink tpLink = new LogStatementLink();
                        tpLink.TableName = content_type;
                        tpLink.TableId = content_id;
                        tpLink.LogStatementId = statement.LogStatementId;
                        tpLink.OnInsert();
                    }
                    //ConvertLogExtensions(

                    //parts = parts;
                }
            }
            catch (Exception ex)
            {
            }
        }
        public static void SupplementDataInDatabase()
        {
            try
            {
                AnalyticsWorker worker = new AnalyticsWorker();

                //Get all theory to exercise navigations where there is no theorypage id in the LogStatementLink table.
                //  Then insert the necessary rows.

                string query = @"select LogStatement.LogStatementId from logstatement
                    inner join LogStatementLink on LogStatement.LogStatementId=LogStatementLink.LogStatementId and LogStatementLink.TableName='LearningModule' and LogStatementLink.tableId='1381'
                    inner join LogContext on LogStatement.LogContextId=LogContext.LogContextId
                    inner join LogActivity on LogStatement.TargetLogActivityId=LogActivity.LogActivityId
                    left outer join LogStatementLink exLink on LogStatement.LogStatementId=exLink.LogStatementId and exLink.TableName='Exercise'
                    left outer join LogStatementLink tpLink on LogStatement.LogStatementId=tpLink.LogStatementId and tpLink.TableName='TheoryPage'
                    where lower(LogActivity.id) like '%theorypagecontext%'
                                            and tplink.tableid is null";

                LogStatementCollection statements = worker.GetLogStatements(query);

                foreach (LogStatement statement in statements)
                {
                    if (statement.TargetLogActivityIdResolved == null)
                        continue;

                    string id = statement.TargetLogActivityIdResolved.Id;
                    //id format == http://student.commart.eu/Content/Show/1381/Exercise/21084?theorypageContext=3761
                    List<string> parts = new List<string>(id.Split(new string[] { "Content/Show/1381/Exercise/", "?theorypageContext=" }, StringSplitOptions.RemoveEmptyEntries));
                    //Ignore parts[0]. It's the prefix of the website: http://student.commart.eu/
                    string exerciseId = parts[1];
                    string theoryPageId = parts[2];
                    if (theoryPageId.Contains("#"))
                        theoryPageId = theoryPageId.Substring(0, theoryPageId.IndexOf("#"));

                    bool foundTPLink = false;
                    LogStatementLinkCollection links = CacheHelper.GetLogStatementLinks(statement.LogStatementId);
                    foreach (LogStatementLink link in links)
                    {
                        if (link.TableName == "TheoryPage")
                            foundTPLink = true;
                    }
                    if (!foundTPLink)
                    {
                        LogStatementLink tpLink = new LogStatementLink();
                        tpLink.TableName = "TheoryPage";
                        tpLink.TableId = theoryPageId;
                        tpLink.LogStatementId = statement.LogStatementId;
                        tpLink.OnInsert();
                    }
                    //ConvertLogExtensions(

                    //parts = parts;
                }
            }
            catch (Exception ex)
            {
            }
        }


        public static void Main_Push()
        {
            AnalyticsWorker worker = new AnalyticsWorker();

            bool skipRafTests = false;
            bool onlyRafTests = true;
            bool saveToFile = true;
            bool saveToLRS = !saveToFile;
            int startIndex = -1;// -1 for no limit
            int max_count = -1;
            DateTime startDate = DateTime.Today;
            DateTime? endDate = null;
            bool? onlyUnsentStatements = null;
            List<string> studentGroups = null;
            bool enableStudentFilter = ConfigurationManager.AppSettings["EnableStudentFilter"] == "true";
            bool anonimizeUsers = false;

            bool publish = true;

            if (publish)
            {
                skipRafTests = true;
                onlyRafTests = false;
                saveToFile = ConfigurationManager.AppSettings["OutputToFile"] == "true";
                saveToLRS = ConfigurationManager.AppSettings["OutputToLRS"] == "true";
                anonimizeUsers = ConfigurationManager.AppSettings["AnonimizeUsers"] == "true";
                studentGroups = ConfigurationHelper.GetStudentGroups();
                if (!ConfigurationHelper.GetSubmissionPeriod(out startDate, out endDate))
                    return;
                onlyUnsentStatements = true;
                int.TryParse(ConfigurationManager.AppSettings["LimitStartIndex"], out startIndex);
                int.TryParse(ConfigurationManager.AppSettings["LimitOffset"], out max_count);
            }

            Dictionary<string, List<LogStatement>> dict_user_to_logstatement = new Dictionary<string, List<LogStatement>>();
            Dictionary<string, List<Statement>> dict_user_to_statement = new Dictionary<string, List<Statement>>();



            LogStatementCollection statements = worker.GetUnprocessedLogStatements(startDate, endDate, studentGroups, onlyUnsentStatements, startIndex, max_count, enableStudentFilter);
            //                string query = @"select LogStatementId from logstatement 
            //where CAST(storedTimestamp as DATE) >= '2016-11-08' and CAST(storedTimestamp as DATE) <= '2017-02-27'
            //and logagentid in (select Student_x_StudentGroup.StudentId from Student_x_StudentGroup where studentgroupid = 901 and StudentId = logstatement.logagentid)
            //and logagentid in (select logagentid from logagentlrsfilter)
            //and LogStatementId not in (select id from LogStatement_ExportImport)";
            //                LogStatementCollection statements = worker.GetLogStatements(query);
            List<Statement> LrsStatements = new List<Statement>();
            foreach (LogStatement record in statements)
            {
                if (skipRafTests && record.LogAgentId == 22141)
                {
                    continue;
                }
                if (onlyRafTests && record.LogAgentId != 22141)
                {
                    continue;
                }

                Statement state = DataConversionHelper.ConvertLogStatement(record);

                string strExerciseId = null;
                string strTheoryPageId = null;
                string strLearningModuleId = null;
                string strLearningModuleProfileId = null;

                LogStatementLinkCollection links = CacheHelper.GetLogStatementLinks(record.LogStatementId);
                foreach (LogStatementLink link in links)
                {
                    if (state.context == null)
                        state.context = new Context();
                    if (state.context.extensions == null)
                        state.context.extensions = new TinCan.Extensions();

                    //if(link.TableName == ""+EMCGTables.Exercise)
                    {
                        var list = state.context.extensions.ToJObject();
                        list.Add(CacheHelper.GetXAPIDefinitionsBaseURL("extension/link-" + link.TableName), link.TableId);
                        state.context.extensions = new TinCan.Extensions(list);

                        if (link.TableName == "Exercise")
                            strExerciseId = link.TableId;
                        else if (link.TableName == "TheoryPage")
                            strTheoryPageId = link.TableId;
                        else if (link.TableName == "LearningModule")
                            strLearningModuleId = link.TableId;
                        else if (link.TableName == "LearningModuleProfile")
                            strLearningModuleProfileId = link.TableId;

                    }

                }

                long? exerciseId = null;
                long? theoryPageId = null;
                long? learningModuleId = null;
                long? learningModuleProfileId = null;
                long tmp;
                if (!string.IsNullOrWhiteSpace(strExerciseId) && long.TryParse(strExerciseId, out tmp))
                    exerciseId = tmp;
                if (!string.IsNullOrWhiteSpace(strTheoryPageId) && long.TryParse(strTheoryPageId, out tmp))
                    theoryPageId = tmp;
                if (!string.IsNullOrWhiteSpace(strLearningModuleId) && long.TryParse(strLearningModuleId, out tmp))
                    learningModuleId = tmp;
                if (!string.IsNullOrWhiteSpace(strLearningModuleProfileId) && long.TryParse(strLearningModuleProfileId, out tmp))
                    learningModuleProfileId = tmp;

                #region Check target (type Activity)
                if (state.target is TinCan.Activity)
                {
                    Activity act = (state.target as TinCan.Activity);
                    if (IsTypeExercise(act.definition.type))
                    {//It's an exercise
                        if (act.definition.description.isEmpty()
                            && !string.IsNullOrWhiteSpace(strExerciseId))
                        {//Lookup description in metadata.
                            string descr = CacheHelper.GetExerciseDescriptionFromMetadata(strExerciseId);
                            if (!string.IsNullOrWhiteSpace(descr))
                            {
                                act.definition.description = DataConversionHelper.ConvertStringToLanguageMap(worker.Language, descr);
                            }
                        }

                        if (act.definition.name.isEmpty()
                            && !string.IsNullOrWhiteSpace(strExerciseId))
                        {//Lookup description in metadata.
                            string name = CacheHelper.GetExerciseName(strExerciseId);
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                act.definition.name = DataConversionHelper.ConvertStringToLanguageMap(worker.Language, name);
                            }
                        }
                    }
                    else if (act.definition.type.ToString() == "http://activitystrea.ms/schema/1.0/page")
                    {//It's a theory page
                        //////////if (act.definition.name.isEmpty()
                        ////////// && !string.IsNullOrWhiteSpace(strTheoryPageId))
                        //////////{//Lookup description in metadata.
                        //////////    string name = CacheHelper.GetTheoryPageName(strTheoryPageId);
                        //////////    if (!string.IsNullOrWhiteSpace(name))
                        //////////    {
                        //////////        act.definition.name = ConvertStringToLanguageMap(worker.Language, name);
                        //////////    }
                        //////////}
                    }
                    else if (act.definition.type.ToString() == "http://adlnet.gov/expapi/activities/course")
                    {//It's learning module

                        if (learningModuleId.HasValue)//!string.IsNullOrWhiteSpace(strLearningModuleId))
                        {
                            string learningModuleInstructionLanguage = null;
                            if (act.definition.name.isEmpty() || act.definition.description.isEmpty())
                            {
                                learningModuleInstructionLanguage = CacheHelper.GetLearningModuleInstructionLanguage(learningModuleId.Value);
                                if (string.IsNullOrWhiteSpace(learningModuleInstructionLanguage))
                                    learningModuleInstructionLanguage = worker.Language;
                            }

                            if (act.definition.name.isEmpty())
                            {//Lookup name.
                                string name = CacheHelper.GetLearningModuleName(strLearningModuleId);
                                if (!string.IsNullOrWhiteSpace(name))
                                {
                                    act.definition.name = DataConversionHelper.ConvertStringToLanguageMap(learningModuleInstructionLanguage, name);
                                }
                            }

                            if (act.definition.description.isEmpty())
                            {//Lookup description.
                                string descr = CacheHelper.GetLearningModuleDescription(strLearningModuleId);
                                if (!string.IsNullOrWhiteSpace(descr))
                                {
                                    act.definition.description = DataConversionHelper.ConvertStringToLanguageMap(learningModuleInstructionLanguage, descr);
                                }
                                else
                                {//no description found. Clear the record.
                                    act.definition.description = null;
                                }
                            }
                        }
                    }
                    else if (act.definition.type.ToString() == "http://adlnet.gov/expapi/activities/module"
                        && learningModuleProfileId.HasValue)
                    {//It's learning module profile
                        //act.definition.description = null;//Clear description, since it does not add value.
                    }
                }
                #endregion

                #region Check context.extensions
                string exerciseType = null;
                string instanceid = null;
                if (state.context != null && state.context.extensions != null)
                {
                    bool exerciseTypeFound = false;
                    Newtonsoft.Json.Linq.JObject job = state.context.extensions.ToJObject();
                    foreach (Newtonsoft.Json.Linq.JProperty x in (Newtonsoft.Json.Linq.JToken)job)
                    {
                        string name = x.Name;
                        Newtonsoft.Json.Linq.JToken value = x.Value;

                        if (name == CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.ExerciseType))
                        {
                            exerciseTypeFound = true;
                            exerciseType = value.ToString();
                        }
                        else if (name == CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.InstanceId))
                        {
                            instanceid = value.ToString();
                        }
                    }

                    if (exerciseId.HasValue && !exerciseTypeFound)
                    {
                        string type = CacheHelper.GetExerciseType(exerciseId);
                        if (!string.IsNullOrWhiteSpace(type))
                        {
                            job.Add(CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.ExerciseType), type);
                            exerciseType = type;
                        }
                    }
                    state.context.extensions = new TinCan.Extensions(job);
                }
                #endregion

                #region Check result.extensions
                if (state.result != null && state.result.extensions != null)
                {
                    if (exerciseId.HasValue && (exerciseType == "Locate" || exerciseType == "Discover"))
                    {
                        string dragId = null;
                        string dropTargetId = null;

                        Newtonsoft.Json.Linq.JObject job_re = state.result.extensions.ToJObject();
                        foreach (Newtonsoft.Json.Linq.JProperty x in (Newtonsoft.Json.Linq.JToken)job_re)
                        {
                            string name = x.Name;
                            string value = x.Value.ToString();

                            if (name == CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.DragId))
                            {
                                dragId = value;
                            }
                            else if (name == CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.DropTargetId))
                            {
                                dropTargetId = value;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(dragId))
                        {
                            string valueType = null;
                            string captionName = "";
                            if (exerciseType == "Locate")
                                captionName = CacheHelper.GetLocateCaptionName(exerciseId.Value, dragId, ref valueType);
                            else if (exerciseType == "Discover")
                                captionName = CacheHelper.GetDiscoverItemName(exerciseId.Value, dragId, ref valueType);

                            if (!string.IsNullOrWhiteSpace(captionName))
                            {
                                job_re.Add(CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.DragCaption), captionName);
                                if (!string.IsNullOrWhiteSpace(valueType))
                                    job_re.Add(CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.DragCaptionValueType), valueType);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(dropTargetId))
                        {
                            string hotspotName = "";
                            if (exerciseType == "Locate")
                                hotspotName = CacheHelper.GetLocateHotspotName(exerciseId.Value, dropTargetId);
                            else if (exerciseType == "Discover")
                                hotspotName = CacheHelper.GetDiscoverItemName(exerciseId.Value, dropTargetId);
                            if (!string.IsNullOrWhiteSpace(hotspotName))
                            {
                                job_re.Add(CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.DropTargetHotspotName), hotspotName);
                            }
                        }
                        state.result.extensions = new TinCan.Extensions(job_re);
                    }
                }
                #endregion

                #region HOTFIX for 2016-05-02 dataset
                //if (prepareExportForBenoit)
                //{
                //    if (state.target is TinCan.Activity)
                //    {
                //        Activity act = (state.target as TinCan.Activity);
                //        #region Try to restore Navigation extension
                //        if (IsTypeExercise(act.definition.type))
                //        {
                //            if (!act.definition.name.isEmpty())
                //            {
                //                string exname = CacheHelper.GetExerciseName(strExerciseId);
                //                if (!string.IsNullOrWhiteSpace(exname) && !act.id.ToString().EndsWith("/Exercise/" + strExerciseId))
                //                {
                //                    #region add navigation extension

                //                    Newtonsoft.Json.Linq.JObject job = state.context.extensions.ToJObject();
                //                    if (exerciseId.HasValue)
                //                    {
                //                        string type = "SomeKindOfNavigation";//CacheHelper.GetExerciseType(exerciseId);
                //                        //if (!string.IsNullOrWhiteSpace(type))
                //                        {
                //                            job.Add(CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.NavigationType), type);
                //                        }
                //                    }
                //                    state.context.extensions = new TinCan.Extensions(job);


                //                    #endregion
                //                }
                //            }
                //        }
                //        else
                //        {
                //            if (act.definition.type.ToString() == "http://activitystrea.ms/schema/1.0/page"
                //                && theoryPageId.HasValue)
                //            {
                //                if (!act.definition.name.isEmpty())
                //                {
                //                    string name = CacheHelper.GetTheoryPageName(strTheoryPageId);
                //                    if (!string.IsNullOrWhiteSpace(name) && !act.id.ToString().EndsWith("/Theory/" + strTheoryPageId))
                //                    {
                //                        #region add navigation extension

                //                        Newtonsoft.Json.Linq.JObject job = state.context.extensions.ToJObject();
                //                        if (exerciseId.HasValue)
                //                        {
                //                            string type = "SomeKindOfNavigation";//CacheHelper.GetExerciseType(exerciseId);
                //                            //if (!string.IsNullOrWhiteSpace(type))
                //                            {
                //                                job.Add(CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.NavigationType), type);
                //                            }
                //                        }
                //                        state.context.extensions = new TinCan.Extensions(job);


                //                        #endregion
                //                    }
                //                }
                //            }
                //        }
                //        #endregion

                //        #region Try to restore the Access Type for the main activity
                //        string id = act.id.ToString();
                //        AccessTypes? at = null;
                //        at = GuessAccessTypeForUrl(id);

                //        if (at.HasValue)
                //        {
                //            Newtonsoft.Json.Linq.JObject job = act.definition.extensions.ToJObject();
                //            job.Add(CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.AccessType), "" + at.Value);
                //            act.definition.extensions = new TinCan.Extensions(job);
                //        }
                //        #endregion
                //        #region Try to restore the Access Type for the parent activity
                //        if (state.context != null
                //            && state.context.contextActivities != null
                //            && state.context.contextActivities.parent != null
                //            && state.context.contextActivities.parent.Count > 0)
                //        {
                //            string parentid = state.context.contextActivities.parent[0].id.ToString();
                //            AccessTypes? pat = null;
                //            pat = GuessAccessTypeForUrl(parentid);

                //            if (pat.HasValue)
                //            {
                //                Newtonsoft.Json.Linq.JObject job = state.context.contextActivities.parent[0].definition.extensions.ToJObject();
                //                job.Add(CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.AccessType), "" + pat.Value);
                //                state.context.contextActivities.parent[0].definition.extensions = new TinCan.Extensions(job);
                //            }
                //        }
                //        #endregion
                //    }
                //}

                #endregion

                if (false)
                {
                    #region Process monitoring

                    //Check if last known statement for this user was SessionAbandoned
                    //If true: Is this an Accessed statement?
                    //         If true: Continue
                    //         If false: Get last Accessed statement before SessionAbandoned. 
                    //                   If it has the same Instance as the current statement: copy, adjust and insert that old statement first.
                    //                   If not: Continue
                    //If false: Continue

                    string userId = "" + record.LogAgentId;//state.actor.mbox;

                    if (!dict_user_to_logstatement.ContainsKey(userId))
                    {
                        dict_user_to_logstatement.Add(userId, new List<LogStatement>());
                        dict_user_to_statement.Add(userId, new List<Statement>());

                        //prefill with a few historic (database) statements (e.g. last SessionAbandoned, and last Accessed).
                        //TODO get more historic items
                        LogStatement historicLogStatement = CacheHelper.GetLastLogStatement(record.LogAgentId);
                        Statement historicStatement = null;


                        if (historicLogStatement.LogVerbId == (int)Verbs.SessionAbandoned)
                        {//Last Session ended abnormally.
                            if (historicLogStatement != null)
                                historicStatement = DataConversionHelper.ConvertLogStatement(historicLogStatement);

                            dict_user_to_logstatement[userId].Add(historicLogStatement);
                            dict_user_to_statement[userId].Add(historicStatement);
                        }


                    }

                    dict_user_to_logstatement[userId].Add(record);
                    dict_user_to_statement[userId].Add(state);

                    int userStatementCount = dict_user_to_logstatement[userId].Count;

                    //if (!string.IsNullOrWhiteSpace(instanceid))
                    {
                        //_process_dictionary[userId, instanceid].Add(state);

                        LogStatement previousRecord = null;
                        Statement previousRecordStatement = null;
                        if (userStatementCount > 1)
                        {
                            previousRecord = dict_user_to_logstatement[userId][userStatementCount - 2];
                            previousRecordStatement = dict_user_to_statement[userId][userStatementCount - 2];
                        }

                        if (previousRecord != null && previousRecord.LogVerbId == (int)Verbs.SessionAbandoned)
                        {
                            //Remember that the previous Session has been terminated for this user (and Instance).
                            //If the current statement is not of an Accessed-type: copy that Accessed-statement from the previous Session (for the given InstanceId).
                            //First record for this user in the current batch is not Accessed or LoggedIn -> User is probably continuing on the previous Session.
                            if (record.LogVerbId != (int)Verbs.Accessed
                                && record.LogVerbId != (int)Verbs.LoggedIn)
                            {
                                if (dict_user_to_logstatement[userId].Count == 2)
                                {//Only the SessionAbandoned statement was found before the current record.
                                    //-> Lookup last Accessed statement before the session was abandoned

                                    LogStatement lastAccessedStatementOfPreviousSession = CacheHelper.GetLastLogStatement(record.LogAgentId, "LogVerbId = " + (int)Verbs.Accessed);
                                    if (lastAccessedStatementOfPreviousSession != null)
                                    {
                                        dict_user_to_logstatement[userId].Insert(0, lastAccessedStatementOfPreviousSession);
                                        dict_user_to_statement[userId].Insert(0, DataConversionHelper.ConvertLogStatement(lastAccessedStatementOfPreviousSession));
                                    }
                                }

                                //Find a corresponding Accessed statement, by looking back in the logged list
                                for (int i = dict_user_to_logstatement[userId].Count - 3; i >= 0; i--)// "-3" skips the current record (not Accessed/LoggedIn) and the previous (SessionAbandoned)
                                {
                                    Statement recordStatement = dict_user_to_statement[userId][i];
                                    LogStatement recordLogStatement = dict_user_to_logstatement[userId][i];

                                    if (recordLogStatement.LogVerbId == (int)Verbs.Accessed)
                                    {
                                        string recordInstanceId = GetContextExtensionValue(recordStatement, CustomXAPIDefinitionExtensions.InstanceId);
                                        if (recordInstanceId == instanceid
                                            && recordInstanceId != null//rmenten 2017-03-27: Only add it to have a starting record for exercises (which have a valid instanceid).
                                        )
                                        {//Same instance as the current Statement or (in case of double null) just the previous Accessed statement.
                                            recordStatement = new Statement(recordStatement.ToJObject(TinCan.TCAPIVersion.latest()));//Clone historic statement
                                            recordStatement.timestamp = record.Timestamp.Value.AddMilliseconds(-1);//Place artificial object slightly before the current statement

                                            Newtonsoft.Json.Linq.JObject job = recordStatement.context.extensions.ToJObject();
                                            job.Add(CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.IsAutogenerated), "true");
                                            recordStatement.context.extensions = new TinCan.Extensions(job);

                                            recordStatement.id = Guid.NewGuid();//Generate new random guid to differentiate between the autogenerated and the original.

                                            LrsStatements.Add(recordStatement);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }

                LrsStatements.Add(state);
            }

            string suffix = "";
            if (publish)
                suffix = "-XXh" + ("" + DateTime.Now.Minute).PadLeft(2, '0');
            if (true)
                suffix += " (" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss.f") + ")";

            if (saveToFile)
            {

                string localLrsFile = AppDomain.CurrentDomain.BaseDirectory + "LRS-output" + suffix + ".json";
                SaveStatementsToJSONFile(LrsStatements, localLrsFile);
            }
            if (saveToLRS)
            {
                //string suffix = "";
                //if (publish)
                //    suffix = "-XXh" + ("" + DateTime.Now.Minute).PadLeft(2, '0');
                string logLrsFile = AppDomain.CurrentDomain.BaseDirectory + "LRS-response" + suffix + ".json";
                SaveStatementsToLRS(LrsStatements, worker, logLrsFile);
            }
        }
        public static void Main_Pull()
        {
            DaemonModePullType pullType;
            bool daemonShouldPullToDatabase = ("" + ConfigurationManager.AppSettings["DaemonModePullType"]).ToLower() == "database";
            if (daemonShouldPullToDatabase)
            {
                pullType = DaemonModePullType.Database;
            }
            else
            {
                pullType = DaemonModePullType.File;
            }
            PullStoredStatementsFromLRS(pullType);
        }


        private static void SaveStatementsToJSONFile(List<Statement> LrsStatements, string localLrsFile)
        {
            var jarray = new Newtonsoft.Json.Linq.JArray();
            foreach (Statement st in LrsStatements)
            {
                jarray.Add(st.ToJObject(TinCan.TCAPIVersion.latest()));
            }
            File.WriteAllBytes(localLrsFile, Encoding.UTF8.GetBytes(jarray.ToString()));
        }
        private static void SaveStatementsToLRS(List<Statement> LrsStatements, AnalyticsWorker worker, string logLrsFile)
        {
            string status = null;
            if (LrsStatements.Count > 0)
            {
                TinCan.RemoteLRS lrss = new TinCan.RemoteLRS(ConfigurationManager.AppSettings["LRS_URI"], ConfigurationManager.AppSettings["LRS_User"], ConfigurationManager.AppSettings["LRS_Password"]);
                //TinCan.RemoteLRS lrss = new TinCan.RemoteLRS("http://lrs.learninglocker.net/data/xAPI/", "4e03204fb43a56f0856f965884846c2b9ff52954", "b98b2781c99e47bed49b362230ebec57c56f286d");
                //TinCan.LRSResponses.StatementLRSResponse responses = lrss.SaveStatement(state);
                status = "Sending " + LrsStatements.Count + " in one batch." + System.Environment.NewLine;
                TinCan.LRSResponses.StatementsResultLRSResponse responses = lrss.SaveStatements(LrsStatements);

                if (responses.success == true)
                {
                    status += "  Batch sent successfully." + System.Environment.NewLine;

                    DateTime storedTime = DateTime.Now;//Approximate time (not server time). Good enough for our logging. The server does not return the actual stored time.
                    status += "  Setting StoredTimestamp in database to " + storedTime.ToShortDateString() + " " + storedTime.ToShortTimeString() + "." + System.Environment.NewLine;
                    //if(responses.content != null)
                    {
                        foreach (Statement statement in LrsStatements)//responses.content.statements
                        {//Update our local copies with the newly set StoredTimestamp
                            ErrorLog error = worker.UpdateLogStatementWithStoredTimestamp(statement.id, storedTime/*statement.stored*/);
                            if (error != null)
                            {
                                status += "  Error for statement " + statement.id + ": " + System.Environment.NewLine;
                                status += "<<<<<" + System.Environment.NewLine;
                                status += "" + error.Message + "." + System.Environment.NewLine;
                                status += ">>>>>" + System.Environment.NewLine;
                            }
                        }
                    }
                }
                else
                {//rmenten 2016-05-19: retry individually if the batch fails.
                    //In case of duplicate messages, the server sends following Message:
                    //          The remote server returned an error: (409) Conflict.

                    status += "  Batch send failed." + System.Environment.NewLine;

                    foreach (Statement statement in LrsStatements)
                    {
                        bool markAsSent = false;
                        TinCan.LRSResponses.StatementLRSResponse response = lrss.SaveStatement(statement);
                        if (response.success)
                        {
                            markAsSent = true;
                        }
                        else
                        {
                            if (response.errMsg.Contains("\"code\":409")
                                && response.errMsg.Contains("message\":\"Conflicts")
                                && response.httpException != null
                                && response.httpException.Message.Contains("409")
                                && response.httpException.Message.Contains("Conflict"))
                            {
                                //We can assume it's a duplicate. Mark it as sent in de database.
                                markAsSent = true;
                            }
                        }

                        if (markAsSent)
                        {
                            DateTime storedTime = DateTime.Now;
                            ErrorLog error = worker.UpdateLogStatementWithStoredTimestamp(statement.id, storedTime/*statement.stored*/);
                        }
                    }
                }


                status += "Success = " + responses.success + System.Environment.NewLine;
                status += "Count = " + LrsStatements.Count + System.Environment.NewLine;
                if (!responses.success)
                {
                    status += "ErrorMessage = " + responses.errMsg + System.Environment.NewLine;
                    if (responses.httpException != null)
                    {
                        status += "Exception = " + responses.httpException.Message + System.Environment.NewLine + responses.httpException.StackTrace;
                    }
                }
            }
            else
            {
                status = "OK. No statements needed to be sent.";
            }

            File.WriteAllBytes(logLrsFile, Encoding.UTF8.GetBytes(status));
        }
        private static void PullStoredStatementsFromLRS(DaemonModePullType pullType)//AnalyticsWorker worker, string logLrsFile)
        {
            DateTime exportStartDate;
            DateTime? exportStopDate;

            if (!ConfigurationHelper.GetExtractionPeriod(out exportStartDate, out exportStopDate))
                return;

            TinCan.RemoteLRS lrss = new TinCan.RemoteLRS(ConfigurationManager.AppSettings["LRS_URI"], ConfigurationManager.AppSettings["LRS_User"], ConfigurationManager.AppSettings["LRS_Password"]);
            //status = "Sending " + LrsStatements.Count + " in one batch." + System.Environment.NewLine;

            //lrss.VoidStatement(new Guid("f8593417-3c0c-43b0-b9ad-d13b5cd75cde"), new Agent() { name = "sowiso", mbox = "mailto:dev@sowiso.com" });

            List<string> writtenExportFiles = new List<string>();

            DateTime currentDate = exportStartDate;
            while (currentDate < exportStopDate)
            {
                StatementsQuery q = new StatementsQuery();
                q.since = currentDate;// new DateTime(2017, 01, 07, 0, 0, 0, DateTimeKind.Utc);
                //q.until = q.since.Value.AddHours(6);// new DateTime(2017, 01, 07, 0, 0, 0, DateTimeKind.Utc);
                //q.limit = 1000;
                q.until = q.since.Value.AddHours(24);
                q.limit = 2000;

                try
                {
                    if (pullType == DaemonModePullType.File)
                        SavePulledStatementsToFileByLogDate(lrss, q, writtenExportFiles);
                    else
                        SavePulledStatementsToDatabase(lrss, q);
                }
                catch (Exception ex)
                {

                }

                currentDate = q.until.Value;//Set value for next loop
            }

            if (pullType == DaemonModePullType.File)
            {
                foreach (string writtenFile in writtenExportFiles)
                {
                    File.AppendAllText(writtenFile, "]");//End JSON Array

                    ////////////if (("" + ConfigurationManager.AppSettings["KeepEmptyPullFiles"]).ToLower() != "true")
                    ////////////{
                    ////////////    try
                    ////////////    {
                    ////////////        //Clean up empty data files
                    ////////////        FileInfo fi = new FileInfo(localLrsFile);
                    ////////////        if (fi.Length == 0)
                    ////////////            fi.Delete();
                    ////////////    }
                    ////////////    catch { }
                    ////////////}
                }
            }
        }

        private static void SavePulledStatementsToDatabase(RemoteLRS lrss, StatementsQuery q)
        {
            //TODO: Not yet implemented.
            throw new NotImplementedException();
        }
        private static void SavePulledStatementsToFileByLogDate(TinCan.RemoteLRS lrss, StatementsQuery q, List<string> writtenExportFiles)
        {
            //string localLrsFile = AppDomain.CurrentDomain.BaseDirectory + "LRS-export-" + q.since.Value.ToString("yyyy-MM-dd-HH") + ".json";
            //File.WriteAllText(localLrsFile, "");//Create the file (or make sure it's empty if it already exists).

            TinCan.LRSResponses.StatementsResultLRSResponse resultResponse = lrss.QueryStatements(q);
            //bool skip = false;
            bool somethingWasWritten = false;
            while (resultResponse != null)
            {
                //string export = "";
                StatementsResult result = resultResponse.content;

                if (result == null || result.statements == null || result.statements.Count == 0)
                {
                    //skip = true;
                    break;
                }

                foreach (Statement statement in result.statements)
                {
                    string export = "";
                    string localLrsFile = AppDomain.CurrentDomain.BaseDirectory + "Export\\LRS-export-" + statement.timestamp.Value.ToString("yyyy-MM-dd") + ".json";
                    FileInfo fi = new FileInfo(localLrsFile);
                    bool fileIsEmpty = true;
                    if (fi.Exists && fi.Length > 0)
                        fileIsEmpty = false;

                    if (fileIsEmpty)
                        export += "[";//Open JArray
                    else
                        export += ",";//Separate from previous JObject
                    export += statement.ToJSON(true);

                    File.AppendAllText(localLrsFile, export);

                    if (!writtenExportFiles.Contains(localLrsFile))
                        writtenExportFiles.Add(localLrsFile);
                }

                try
                {
                    if (!string.IsNullOrWhiteSpace(result.more))
                    {
                        resultResponse = lrss.MoreStatements(result);
                    }
                    else
                    {
                        resultResponse = null;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                //                File.AppendAllText(localLrsFile, export);
                somethingWasWritten = true;
            }

            //if (somethingWasWritten)
            //    File.AppendAllText(localLrsFile, "]");//Close JArray

            ////////////if (("" + ConfigurationManager.AppSettings["KeepEmptyPullFiles"]).ToLower() != "true")
            ////////////{
            ////////////    try
            ////////////    {
            ////////////        //Clean up empty data files
            ////////////        FileInfo fi = new FileInfo(localLrsFile);
            ////////////        if (fi.Length == 0)
            ////////////            fi.Delete();
            ////////////    }
            ////////////    catch { }
            ////////////}

            //if (skip)
            //    continue;


            //File.WriteAllBytes(logLrsFile, Encoding.UTF8.GetBytes(status));
        }
        private static void SavePulledStatementsToFileByStoredDate(TinCan.RemoteLRS lrss, StatementsQuery q)
        {
            string localLrsFile = AppDomain.CurrentDomain.BaseDirectory + "LRS-export-" + q.since.Value.ToString("yyyy-MM-dd-HH") + ".json";
            File.WriteAllText(localLrsFile, "");//Create the file (or make sure it's empty if it already exists).

            TinCan.LRSResponses.StatementsResultLRSResponse resultResponse = lrss.QueryStatements(q);
            //bool skip = false;
            bool somethingWasWritten = false;
            while (resultResponse != null)
            {
                string export = "";
                StatementsResult result = resultResponse.content;

                if (result == null || result.statements == null || result.statements.Count == 0)
                {
                    //skip = true;
                    break;
                }

                foreach (Statement statement in result.statements)
                {
                    if (string.IsNullOrWhiteSpace(export))
                        export += "[";//Open JArray
                    else
                        export += ",";//Separate from previous JObject
                    export += statement.ToJSON(true);
                }

                if (!string.IsNullOrWhiteSpace(result.more))
                {
                    resultResponse = lrss.MoreStatements(result);
                }
                else
                {
                    resultResponse = null;
                }

                File.AppendAllText(localLrsFile, export);
                somethingWasWritten = true;
            }

            if (somethingWasWritten)
                File.AppendAllText(localLrsFile, "]");//Close JArray

            if (("" + ConfigurationManager.AppSettings["KeepEmptyPullFiles"]).ToLower() != "true")
            {
                try
                {
                    //Clean up empty data files
                    FileInfo fi = new FileInfo(localLrsFile);
                    if (fi.Length == 0)
                        fi.Delete();
                }
                catch { }
            }

            //if (skip)
            //    continue;


            //File.WriteAllBytes(logLrsFile, Encoding.UTF8.GetBytes(status));
        }


        private static string GetContextExtensionValue(Statement state, CustomXAPIDefinitionExtensions field)
        {
            string value = null;
            if (state.context != null && state.context.extensions != null)
            {
                Newtonsoft.Json.Linq.JObject job = state.context.extensions.ToJObject();
                foreach (Newtonsoft.Json.Linq.JProperty x in (Newtonsoft.Json.Linq.JToken)job)
                {
                    string name = x.Name;
                    Newtonsoft.Json.Linq.JToken tokenvalue = x.Value;

                    if (name == CacheHelper.GetXAPIDefinitionsBaseURL(field))
                    {
                        value = tokenvalue.ToString();
                    }
                }
            }
            return value;
        }
        private static AccessTypes? GuessAccessTypeForUrl(string id)
        {
            AccessTypes? at = null;
            if (/*id.EndsWith("/Exercise/" + strExerciseId)
                || (id.Contains("/Exercise/" + strExerciseId) && id.Contains("?theorypageContext="))
                || */(id.Contains("/Exercise/"))
                )
                at = AccessTypes.Exercise;
            else if (/*id.EndsWith("/Theory/" + strTheoryPageId)
                || (id.Contains("/Theory/" + strTheoryPageId) && id.Contains("?exerciseContext="))
                || */(id.Contains("/Theory/"))
                )
                at = AccessTypes.TheoryPage;
            else if (id.Contains("/ENode/"))
                at = AccessTypes.ExerciseTreeItem;
            else if (id.Contains("/TNode/"))
                at = AccessTypes.TheoryPageTreeItem;
            else if (id.Contains("/Profile/"))
                at = AccessTypes.LearningModuleProfile;
            else if (id.EndsWith("/Root"))
                at = AccessTypes.LearningModule;
            else if (id.Contains("/GetDictionaryEntry/") || id.Contains("/dictionary/"))
                at = AccessTypes.Dictionary;
            return at;
        }
        private static bool IsTypeExercise(Uri uri)
        {
            return uri.ToString() == "http://adlnet.gov/expapi/activities/interaction";
        }
    }
}
