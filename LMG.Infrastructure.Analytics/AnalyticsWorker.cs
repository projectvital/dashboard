using LMG.Infrastructure.Analytics.Helpers;
using LMG.Infrastructure.Analytics.Objects;
using LMG.Infrastructure.Analytics.Objects.DB;
using LMG.Infrastructure.Analytics.Objects.Types;
using OriginDatabaseLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMG.Infrastructure.Analytics
{
    public class AnalyticsWorker
    {
        public const string ERROR_REQUIRED_FIELD_MISSING = "Missing required field";
        private string m_language = "nl-be";
        public string Language
        {
            get { return m_language; }
            set { m_language = value; }
        }
        private string m_instruction_language = null;
        public string InstructionLanguage
        {
            get 
            {
                if (!string.IsNullOrWhiteSpace(m_instruction_language))
                    return m_instruction_language;
                else
                    return m_language;
            }
            set { m_instruction_language = value; }
        }

        public AnalyticsWorker(string language = null)
        {
            SqlValue.setCurrentDriver(SqlODBCDriver.Driver.SQLServer);

            //CacheHelper.InstallDatabaseConstants();

            if (language != null)
                m_language = language;
        }

        public ErrorLog LogUserLogin(LogParameters parameters)
        {
            ErrorLog error = null;
            try
            {
                if (!CheckCommonParametersAndThrowExceptionForMissingFields(parameters))
                    return null;//Return null if logging is disabled
                
                Kernel.Connection.TransactionBegin();

                LogStatement statement = CreateNewStatement();

                statement.LogAgentId = CacheHelper.GetAgent(parameters.StudentId.Value).LogAgentId;
                statement.LogVerbId = CacheHelper.GetVerb(Verbs.LoggedIn).LogVerbId;

                LogActivity activity = new LogActivity();
                activity.LogActivityDefinitionId = CacheHelper.GetActivityDefinition(LogActivityDefinitions.Application).LogActivityDefinitionId;
                if (parameters.AccessedUri != null)
                    activity.Id = parameters.AccessedUri.ToString();
                ThrowError(activity.OnInsert());

                AddActivityName(activity, "User Login");
                AddActivityDescription(activity, parameters.AccessedContentDescription);

                LogContext context = new LogContext();
                context = CreateNewContext(parameters.Registration.Value);
                ThrowError(context.OnInsert());

                //LogExtension ext = CreateLogExtensionForInstanceId(context, parameters);
                //if(ext != null) ThrowError(ext.OnInsert());

                statement.TargetLogActivityId = activity.LogActivityId;
                statement.LogContextId = context.LogContextId;
                ThrowError(statement.OnInsert());
            }
            catch (Exception ex)
            {
                //Log error
                error = new ErrorLog(ex);
            }
            finally
            {
                if (Kernel.Connection.TransactionActive())
                {
                    if (error == null)
                        Kernel.Connection.TransactionCommit();
                    else
                        Kernel.Connection.TransactionRollback();
                }
            }
            return error;
        }
        public ErrorLog LogUserLogout(LogParameters parameters)
        {
            ErrorLog error = null;
            try
            {
                if (!CheckCommonParametersAndThrowExceptionForMissingFields(parameters))
                    return null;//Return null if logging is disabled

                Kernel.Connection.TransactionBegin();
                
                LogStatement statement = CreateNewStatement();

                statement.LogAgentId = CacheHelper.GetAgent(parameters.StudentId.Value).LogAgentId;
                statement.LogVerbId = CacheHelper.GetVerb(Verbs.LoggedOut).LogVerbId;

                LogActivity activity = new LogActivity();
                activity.LogActivityDefinitionId = CacheHelper.GetActivityDefinition(LogActivityDefinitions.Application).LogActivityDefinitionId;
                activity.Id = parameters.AccessedUri.ToString();
                ThrowError(activity.OnInsert());

                AddActivityName(activity, "User logout");
                AddActivityDescription(activity, parameters.AccessedContentDescription);

                LogContext context = CreateNewContext(parameters.Registration.Value);
                ThrowError(context.OnInsert());

                //LogExtension ext = CreateLogExtensionForInstanceId(context, parameters);
                //if (ext != null) ThrowError(ext.OnInsert());

                statement.LogContextId = context.LogContextId;
                statement.TargetLogActivityId = activity.LogActivityId;
                ThrowError(statement.OnInsert());
            }
            catch (Exception ex)
            {
                //Log error
                error = new ErrorLog(ex);
            }
            finally
            {
                if (Kernel.Connection.TransactionActive())
                {
                    if (error == null)
                        Kernel.Connection.TransactionCommit();
                    else
                        Kernel.Connection.TransactionRollback();
                }
            }
            return error;
        }
        public ErrorLog LogResourceAccess(LogParametersForAccess parameters)
        {
            ErrorLog error = null;
            try
            {
                if (!CheckCommonParametersAndThrowExceptionForMissingFields(parameters))
                    return null;//Return null if logging is disabled

                if ((parameters.AccessType == AccessTypes.Exercise && parameters.ExerciseId.HasValue)
                    || 
                    (parameters.AccessType == AccessTypes.TheoryPage && parameters.TheoryPageId.HasValue)
                    )
                {//rmenten 2016-04-21: Get actual title from exercise/theoryPage, instead of the treeItem (which gets it out of metadata). The actual Title is probably more informative.
                    try
                    {
                        SqlQuerySelect sel = new SqlQuerySelect();
                        if (parameters.AccessType == AccessTypes.Exercise && parameters.ExerciseId.HasValue)
                            sel.Select("Title").From("Exercise").Where("ExerciseId", SqlValue.Convert(parameters.ExerciseId.Value));
                        else if (parameters.AccessType == AccessTypes.TheoryPage && parameters.TheoryPageId.HasValue)
                            sel.Select("Title").From("TheoryPage").Where("TheoryPageId", SqlValue.Convert(parameters.TheoryPageId.Value));
                        else if (parameters.AccessType == AccessTypes.ExerciseTreeItem && parameters.ExerciseTreeItemId.HasValue)
                            sel.Select("Title").From("ExerciseTreeItem").Where("ExerciseTreeItemId", SqlValue.Convert(parameters.ExerciseTreeItemId.Value));
                        else if (parameters.AccessType == AccessTypes.TheoryPageTreeItem && parameters.TheoryPageTreeItemId.HasValue)
                            sel.Select("Title").From("TheoryPageTreeItem").Where("TheoryPageTreeItemId", SqlValue.Convert(parameters.TheoryPageTreeItemId.Value));
                        SqlResult res = Kernel.Connection.ExecuteQuery(sel);
                        string title = res.GetString(0, 0, null);
                        if (!string.IsNullOrWhiteSpace(title))
                            parameters.AccessedContentTitle = title;
                    }
                    catch
                    {//Renaming is not that important. If it fails, we can still proceed with the logging.
                    }
                }

                Kernel.Connection.TransactionBegin();

                LogStatement statement = CreateNewStatement();

                statement.LogAgentId = CacheHelper.GetAgent(parameters.StudentId.Value).LogAgentId;
                statement.LogVerbId = CacheHelper.GetVerb(Verbs.Accessed).LogVerbId;
                LogContext context = CreateNewContext(parameters.Registration.Value);
                ThrowError(context.OnInsert());

                LogExtension ext = CreateLogExtensionForInstanceId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForExerciseType(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForReferrer(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForHintId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForSolutionId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForFeedbackId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForDictionaryId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForNavigationType(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForExploreHotspotId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                //ext = CreateLogExtensionForContentTitle(context, parameters);
                //if (ext != null) throwError(ext.OnInsert());
                //ext = CreateLogExtensionForParentUri(context, parameters);
                //if (ext != null) throwError(ext.OnInsert());
                //ext = CreateLogExtensionForParentTitle(context, parameters);
                //if (ext != null) throwError(ext.OnInsert());
                   
                

                LogActivity activity = new LogActivity();
                activity.LogActivityDefinitionId = CacheHelper.GetActivityDefinition(CacheHelper.GetLogActivityDefinitionForAccessType(parameters.AccessType)).LogActivityDefinitionId;
                activity.Id = parameters.AccessedUri.ToString();
                ThrowError(activity.OnInsert());

                ext = CreateLogExtensionForAccessType(activity, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                if (parameters.AccessType == AccessTypes.Dictionary)
                {
                    ext = CreateLogExtensionForDictionaryId(activity, parameters);
                    if (ext != null) ThrowError(ext.OnInsert());
                }
                ext = CreateLogExtensionForTabTitle(activity, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForExploreHotspotTabId(activity, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForExploreHotspotTabCount(activity, parameters);
                if (ext != null) ThrowError(ext.OnInsert());

                AddActivityName(activity, parameters.AccessedContentTitle);
                AddActivityDescription(activity, parameters.AccessedContentDescription);

                //if (parameters.TheoryPageId.HasValue)
                if (parameters.ParentUri != null)
                {//User accessed a theory page. Add TheoryPageNode as parent (if provided)
                    //TODO: look up the parent if it is not provided?

                    LogActivity parentActivity = new LogActivity();
                    parentActivity.LogActivityDefinitionId = CacheHelper.GetActivityDefinition(
                        ((parameters.ParentAccessType.HasValue)?
                        CacheHelper.GetLogActivityDefinitionForAccessType(parameters.ParentAccessType.Value):
                        LogActivityDefinitions.Module)).LogActivityDefinitionId;
                    parentActivity.Id = parameters.ParentUri.ToString();
                    ThrowError(parentActivity.OnInsert());

                    AddActivityName(parentActivity, parameters.ParentTitle);
                    //AddActivityDescription(activity, parameters.AccessedContentDescription);
                    ext = CreateLogExtensionForParentAccessType(parentActivity, parameters);
                    if (ext != null) ThrowError(ext.OnInsert());

                    LogContextActivity contextAct = new LogContextActivity();
                    contextAct.LogActivityId = parentActivity.LogActivityId;
                    contextAct.LogContextActivityTypeId = (long)LogContextActivityTypes.Parent;
                    contextAct.LogContextId = context.LogContextId;
                    ThrowError(contextAct.OnInsert());

                }


                statement.LogContextId = context.LogContextId;
                statement.TargetLogActivityId = activity.LogActivityId;
                ThrowError(statement.OnInsert());


                AddCommonLogStatementLinks(parameters, statement);
                
            }
            catch (Exception ex)
            {
                //Log error
                error = new ErrorLog(ex);
            }
            finally
            {
                //SaveToEventLog("LogResourceAccess() finished. Error = " + ((error == null)?"null":error.ToExtendedString()), 0);

                if (Kernel.Connection.TransactionActive())
                {
                    if (error == null)
                        Kernel.Connection.TransactionCommit();
                    else
                        Kernel.Connection.TransactionRollback();
                }
            }
            return error;
        }
        public ErrorLog LogExerciseAttempt(LogParametersForExerciseResult parameters)
        {
            ErrorLog error = null;
            try
            {
                if (!CheckCommonParametersAndThrowExceptionForMissingFields(parameters))
                    return null;//Return null if logging is disabled

                bool isMatchCheck = (parameters is LogParametersForExerciseInteraction);

                Kernel.Connection.TransactionBegin();

                LogStatement statement = CreateNewStatement();

                statement.LogAgentId = CacheHelper.GetAgent(parameters.StudentId.Value).LogAgentId;
                if (isMatchCheck)
                    statement.LogVerbId = CacheHelper.GetVerb(Verbs.Interacted).LogVerbId;
                else
                    statement.LogVerbId = CacheHelper.GetVerb(Verbs.Attempted).LogVerbId;
                LogContext context = CreateNewContext(parameters.Registration.Value);
                ThrowError(context.OnInsert());

                LogExtension ext = CreateLogExtensionForInstanceId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForExerciseType(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());

                LogActivity activity = new LogActivity();
                activity.LogActivityDefinitionId = CacheHelper.GetActivityDefinition(CacheHelper.GetLogActivityDefinitionForAccessType(AccessTypes.Exercise)).LogActivityDefinitionId;
                activity.Id = parameters.AccessedUri.ToString();
                ThrowError(activity.OnInsert());
                
                AddActivityName(activity, parameters.AccessedContentTitle);
                AddActivityDescription(activity, parameters.AccessedContentDescription);

                //if (parameters.ExerciseId.HasValue)
                //{
                //    LogActivity parentActivity = new LogActivity();
                //    parentActivity.LogActivityDefinitionId = CacheHelper.GetActivityDefinition(CacheHelper.GetLogActivityDefinitionForAccessType(AccessTypes.Exercise)).LogActivityDefinitionId;
                //    parentActivity.Id = accessedUri.ToString() + "/TODO/USE/PARENT/INSTEAD";
                //    error = parentActivity.OnInsert();
                //    if (error != null) throwError(error);

                //    LogContextActivity contextAct = new LogContextActivity();
                //    contextAct.LogActivityId = parentActivity.LogActivityId;
                //    contextAct.LogContextActivityTypeId = (long)LogContextActivityTypes.Parent;
                //    contextAct.LogContextId = context.LogContextId;
                //    error = contextAct.OnInsert();
                //    if (error != null) throwError(error);
                //}

                LogScore score = new LogScore();
                score.Min = parameters.Min;
                score.Raw = parameters.Raw;
                score.Max = parameters.Max;
                score.Scaled = parameters.Percentage;
                ThrowError(score.OnInsert());

                LogResult result = new LogResult();
                result.IsCompleted = parameters.IsCompleted;
                result.IsSuccess = parameters.IsSuccess;
                result.LogScoreId = score.LogScoreId;
                if(!string.IsNullOrWhiteSpace(parameters.Response))
                    result.Response = parameters.Response;
                else if (parameters.UserAnswers != null && parameters.UserAnswers.Count > 0)
                    result.Response = Newtonsoft.Json.JsonConvert.SerializeObject(parameters.UserAnswers);
                result.DurationTicks = parameters.DurationTicks;
                ThrowError(result.OnInsert());
                
                ext = CreateLogExtensionForAttempt(result, parameters);
                if(ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForAttemptMax(result, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForCorrectAnswers(result, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                if (isMatchCheck)
                {
                    ext = CreateLogExtensionForMatchCheckDragId(result, (parameters as LogParametersForExerciseInteraction));
                    if(ext != null) ThrowError(ext.OnInsert());
                    ext = CreateLogExtensionForMatchCheckDropTargetId(result, (parameters as LogParametersForExerciseInteraction));
                    if (ext != null) ThrowError(ext.OnInsert());
                }
                ext = CreateLogExtensionForSentenceId(result, parameters);
                if (ext != null) ThrowError(ext.OnInsert());

                statement.LogResultId = result.LogResultId;
                statement.LogContextId = context.LogContextId;
                statement.TargetLogActivityId = activity.LogActivityId;
                ThrowError(statement.OnInsert());


                AddCommonLogStatementLinks(parameters, statement);
            }
            catch (Exception ex)
            {
                //Log error
                error = new ErrorLog(ex);
            }
            finally
            {
                if (Kernel.Connection.TransactionActive())
                {
                    if (error == null)
                        Kernel.Connection.TransactionCommit();
                    else
                        Kernel.Connection.TransactionRollback();
                }
            }
            return error;
        }
        public ErrorLog LogExerciseComplete(LogParametersForExerciseResult parameters)
        {
            ErrorLog error = null;
            try
            {
                if (!CheckCommonParametersAndThrowExceptionForMissingFields(parameters))
                    return null;//Return null if logging is disabled
                
                Kernel.Connection.TransactionBegin();

                LogStatement statement = CreateNewStatement();

                statement.LogAgentId = CacheHelper.GetAgent(parameters.StudentId.Value).LogAgentId;
                statement.LogVerbId = CacheHelper.GetVerb(Verbs.Completed).LogVerbId;
                LogContext context = CreateNewContext(parameters.Registration.Value);
                ThrowError(context.OnInsert());

                LogExtension ext = CreateLogExtensionForInstanceId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForExerciseType(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());

                LogActivity activity = new LogActivity();
                activity.LogActivityDefinitionId = CacheHelper.GetActivityDefinition(CacheHelper.GetLogActivityDefinitionForAccessType(AccessTypes.Exercise)).LogActivityDefinitionId;
                activity.Id = parameters.AccessedUri.ToString();
                ThrowError(activity.OnInsert());
                
                AddActivityName(activity, parameters.AccessedContentTitle);
                AddActivityDescription(activity, parameters.AccessedContentDescription);

                //if (parameters.ExerciseId.HasValue)
                //{
                //    LogActivity parentActivity = new LogActivity();
                //    parentActivity.LogActivityDefinitionId = CacheHelper.GetActivityDefinition(CacheHelper.GetLogActivityDefinitionForAccessType(AccessTypes.Exercise)).LogActivityDefinitionId;
                //    parentActivity.Id = accessedUri.ToString() + "/TODO/USE/PARENT/INSTEAD";
                //    error = parentActivity.OnInsert();
                //    if (error != null) throwError(error);

                //    LogContextActivity contextAct = new LogContextActivity();
                //    contextAct.LogActivityId = parentActivity.LogActivityId;
                //    contextAct.LogContextActivityTypeId = (long)LogContextActivityTypes.Parent;
                //    contextAct.LogContextId = context.LogContextId;
                //    error = contextAct.OnInsert();
                //    if (error != null) throwError(error);
                //}

                LogScore score = new LogScore();
                score.Min = parameters.Min;
                score.Raw = parameters.Raw;
                score.Max = parameters.Max;
                score.Scaled = parameters.Percentage;
                ThrowError(score.OnInsert());

                LogResult result = new LogResult();
                result.IsCompleted = parameters.IsCompleted;
                result.IsSuccess = parameters.IsSuccess;
                result.LogScoreId = score.LogScoreId;
                result.Response = parameters.Response;
                result.DurationTicks = parameters.DurationTicks;
                ThrowError(result.OnInsert());

                ext = CreateLogExtensionForAttempt(result, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForAttemptMax(result, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                
                statement.LogResultId = result.LogResultId;
                statement.LogContextId = context.LogContextId;
                statement.TargetLogActivityId = activity.LogActivityId;
                ThrowError(statement.OnInsert());


                AddCommonLogStatementLinks(parameters, statement);
            }
            catch (Exception ex)
            {
                //Log error
                error = new ErrorLog(ex);
            }
            finally
            {
                if (Kernel.Connection.TransactionActive())
                {
                    if (error == null)
                        Kernel.Connection.TransactionCommit();
                    else
                        Kernel.Connection.TransactionRollback();
                }
            }
            return error;
        }
        public ErrorLog LogMediaInteraction(LogParametersForMediaInteraction parameters)
        {
            ErrorLog error = null;
            try
            {
                if (!CheckCommonParametersAndThrowExceptionForMissingFields(parameters))
                    return null;//Return null if logging is disabled

                Kernel.Connection.TransactionBegin();

                LogStatement statement = CreateNewStatement();

                Verbs? verb = null;
                if (parameters.MediaInteractionType == Objects.Types.MediaInteractionTypes.Play)
                    verb = Verbs.Played;
                else if (parameters.MediaInteractionType == Objects.Types.MediaInteractionTypes.Pause)
                    verb = Verbs.Paused;
                else if (parameters.MediaInteractionType == Objects.Types.MediaInteractionTypes.Complete)
                    verb = Verbs.Completed;
                else if (parameters.MediaInteractionType == Objects.Types.MediaInteractionTypes.Skip)
                    verb = Verbs.Skipped;
                else if (parameters.MediaInteractionType == Objects.Types.MediaInteractionTypes.Record)
                    verb = Verbs.Recorded;
                else ThrowMissingFieldException("MediaInteractionType");

                statement.LogAgentId = parameters.StudentId.Value;
                statement.LogVerbId = CacheHelper.GetVerb(verb.Value).LogVerbId;
                LogContext context = CreateNewContext(parameters.Registration.Value);
                ThrowError(context.OnInsert());

                LogExtension ext = CreateLogExtensionForInstanceId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForExerciseType(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForMediaStartingPoint(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForMediaEndingPoint(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForSolutionId(context, parameters);//Primarily for RecordCompare
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForSentenceId(context, parameters);//Primarily for ListenWrite
                if (ext != null) ThrowError(ext.OnInsert());

                LogActivity activity = new LogActivity();
                activity.LogActivityDefinitionId = CacheHelper.GetActivityDefinition(LogActivityDefinitions.Media).LogActivityDefinitionId;
                activity.Id = parameters.AccessedUri.ToString();
                ThrowError(activity.OnInsert());

                ext = CreateLogExtensionForMimeType(activity, parameters, GetMimeTypeForUrl(parameters.AccessedUri));
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForIsVoiceRecording(activity, parameters);
                if (ext != null) ThrowError(ext.OnInsert());

                if (parameters.ParentUri != null)
                {
                    //TODO: look up the parent if it is not provided?

                    LogActivityDefinitions? type = null;
                    if (parameters.ExerciseId.HasValue)
                        type = LogActivityDefinitions.Interaction;
                    else if (parameters.TheoryPageId.HasValue)
                        type = LogActivityDefinitions.Page_Theory;
                    //todo rmenten 2016-04-06: also provide support for media in a dictionary item?

                    if (type.HasValue)
                    {
                        LogActivity parentActivity = new LogActivity();
                        parentActivity.LogActivityDefinitionId = CacheHelper.GetActivityDefinition(type).LogActivityDefinitionId;
                        parentActivity.Id = parameters.ParentUri.ToString();
                        ThrowError(parentActivity.OnInsert());

                        AddActivityName(parentActivity, parameters.ParentTitle);
                        //AddActivityDescription(activity, parameters.AccessedContentDescription);

                        LogContextActivity contextAct = new LogContextActivity();
                        contextAct.LogActivityId = parentActivity.LogActivityId;
                        contextAct.LogContextActivityTypeId = (long)LogContextActivityTypes.Parent;
                        contextAct.LogContextId = context.LogContextId;
                        ThrowError(contextAct.OnInsert());
                    }
                }

                statement.LogContextId = context.LogContextId;
                statement.TargetLogActivityId = activity.LogActivityId;
                ThrowError(statement.OnInsert());

                AddCommonLogStatementLinks(parameters, statement);
            }
            catch (Exception ex)
            {
                //Log error
                error = new ErrorLog(ex);
            }
            finally
            {
                if (Kernel.Connection.TransactionActive())
                {
                    if (error == null)
                        Kernel.Connection.TransactionCommit();
                    else
                        Kernel.Connection.TransactionRollback();
                }
            }
            return error;
        }
        public ErrorLog LogTheoryPdfRequested(LogParametersForAccess parameters)
        {
            ErrorLog error = null;
            try
            {
                if (!CheckCommonParametersAndThrowExceptionForMissingFields(parameters))
                    return null;//Return null if logging is disabled
                
                Kernel.Connection.TransactionBegin();

                LogStatement statement = CreateNewStatement();

                statement.LogAgentId = parameters.StudentId.Value;
                statement.LogVerbId = CacheHelper.GetVerb(Verbs.Printed).LogVerbId;
                LogContext context = CreateNewContext(parameters.Registration.Value);
                ThrowError(context.OnInsert());

                LogExtension ext = CreateLogExtensionForReferrer(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());

                LogActivity activity = new LogActivity();
                activity.LogActivityDefinitionId = CacheHelper.GetActivityDefinition(LogActivityDefinitions.File).LogActivityDefinitionId;
                activity.Id = parameters.AccessedUri.ToString();
                ThrowError(activity.OnInsert());

                ext = CreateLogExtensionForMimeType(activity, parameters, "application/pdf");
                if (ext != null) ThrowError(ext.OnInsert());

                if (parameters.ParentUri != null)
                {
                    //TODO: look up the parent if it is not provided?

                    LogActivityDefinitions? type = null;
                    /*if (parameters.ExerciseId.HasValue)
                        type = LogActivityDefinitions.Interaction;
                    else */if (parameters.TheoryPageId.HasValue)
                        type = LogActivityDefinitions.Page_Theory;
                    //todo rmenten 2016-04-06: also provide support for media in a dictionary item?

                    if (type.HasValue)
                    {
                        LogActivity parentActivity = new LogActivity();
                        parentActivity.LogActivityDefinitionId = CacheHelper.GetActivityDefinition(type).LogActivityDefinitionId;
                        parentActivity.Id = parameters.ParentUri.ToString();
                        ThrowError(parentActivity.OnInsert());

                        AddActivityName(parentActivity, parameters.ParentTitle);
                        //AddActivityDescription(activity, parameters.AccessedContentDescription);

                        LogContextActivity contextAct = new LogContextActivity();
                        contextAct.LogActivityId = parentActivity.LogActivityId;
                        contextAct.LogContextActivityTypeId = (long)LogContextActivityTypes.Parent;
                        contextAct.LogContextId = context.LogContextId;
                        ThrowError(contextAct.OnInsert());
                    }
                }

                statement.LogContextId = context.LogContextId;
                statement.TargetLogActivityId = activity.LogActivityId;
                ThrowError(statement.OnInsert());

                AddCommonLogStatementLinks(parameters, statement);
            }
            catch (Exception ex)
            {
                //Log error
                error = new ErrorLog(ex);
            }
            finally
            {
                if (Kernel.Connection.TransactionActive())
                {
                    if (error == null)
                        Kernel.Connection.TransactionCommit();
                    else
                        Kernel.Connection.TransactionRollback();
                }
            }
            return error;
        }
        public ErrorLog LogDictionaryEntrySearched(LogParametersForDictionaryEntrySearch parameters)
        {
            ErrorLog error = null;
            try
            {
                if (string.IsNullOrWhiteSpace(parameters.DictionarySearchTerm))
                    return null;//User did not really search for anything, so we don't need to log it.

                if (!CheckCommonParametersAndThrowExceptionForMissingFields(parameters))
                    return null;//Return null if logging is disabled
                
                Kernel.Connection.TransactionBegin();

                LogStatement statement = CreateNewStatement();

                statement.LogAgentId = parameters.StudentId.Value;
                statement.LogVerbId = CacheHelper.GetVerb(Verbs.Searched).LogVerbId;
                LogContext context = CreateNewContext(parameters.Registration.Value);
                ThrowError(context.OnInsert());

                LogExtension ext = CreateLogExtensionForDictionaryId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForDictionaryEntryId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForDictionarySearchTerm(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForDictionarySearchOption(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                
                
                LogActivity activity = new LogActivity();
                activity.LogActivityDefinitionId = CacheHelper.GetActivityDefinition(LogActivityDefinitions.Media).LogActivityDefinitionId;
                activity.Id = parameters.AccessedUri.ToString();
                ThrowError(activity.OnInsert());

                if (parameters.DictionarySearchResultCount.HasValue)
                {
                    LogResult result = new LogResult();
                    result.IsSuccess = parameters.DictionarySearchResultCount.Value > 0;
                    result.Response = "" + parameters.DictionarySearchResultCount.Value;
                    ThrowError(result.OnInsert());

                    ext = CreateLogExtensionForDictionarySearchResultCount(result, parameters);
                    if (ext != null) ThrowError(ext.OnInsert());

                    statement.LogResultId = result.LogResultId;
                }

                if (parameters.ParentUri != null)
                {
                    //TODO: look up the parent if it is not provided?

                    LogActivityDefinitions? type = null;
                    if (parameters.ExerciseId.HasValue)
                        type = LogActivityDefinitions.Interaction;
                    else if (parameters.TheoryPageId.HasValue)
                        type = LogActivityDefinitions.Page_Theory;
                    //todo rmenten 2016-04-06: also provide support for media in a dictionary item?

                    if (type.HasValue)
                    {
                        LogActivity parentActivity = new LogActivity();
                        parentActivity.LogActivityDefinitionId = CacheHelper.GetActivityDefinition(type).LogActivityDefinitionId;
                        parentActivity.Id = parameters.ParentUri.ToString();
                        ThrowError(parentActivity.OnInsert());

                        AddActivityName(parentActivity, parameters.ParentTitle);
                        //AddActivityDescription(activity, parameters.AccessedContentDescription);
                        
                        LogContextActivity contextAct = new LogContextActivity();
                        contextAct.LogActivityId = parentActivity.LogActivityId;
                        contextAct.LogContextActivityTypeId = (long)LogContextActivityTypes.Parent;
                        contextAct.LogContextId = context.LogContextId;
                        ThrowError(contextAct.OnInsert());
                    }
                }

                statement.LogContextId = context.LogContextId;
                statement.TargetLogActivityId = activity.LogActivityId;
                ThrowError(statement.OnInsert());

                AddCommonLogStatementLinks(parameters, statement);
            }
            catch (Exception ex)
            {
                //Log error
                error = new ErrorLog(ex);
            }
            finally
            {
                if (Kernel.Connection.TransactionActive())
                {
                    if (error == null)
                        Kernel.Connection.TransactionCommit();
                    else
                        Kernel.Connection.TransactionRollback();
                }
            }
            return error;
        }
        public ErrorLog LogExerciseInteraction(LogParametersForExerciseInteraction parameters)
        {
            ErrorLog error = null;
            try
            {
                if (!parameters.ExerciseInteractionType.HasValue) ThrowMissingFieldException("ExerciseInteractionType");

                if (parameters.ExerciseInteractionType == ExerciseInteractionTypes.DragDrop)
                {
                    return LogExerciseMatchCheck(parameters);
                }
                else
                {
                    //todo rmenten 2016-04-28
                }
            }
            catch (Exception ex)
            {
                //Log error
                error = new ErrorLog(ex);
            }
            return error;
        }
        public ErrorLog LogExerciseMatchCheck(LogParametersForExerciseInteraction parameters)
        {
            //todo rmenten 2016-04-25: add differentiation for parameters of type LogParametersForMatchCheck
            return LogExerciseAttempt(parameters);
        }
        public ErrorLog LogSessionAbandoned(LogParametersForAccess parameters)
        {
            ErrorLog error = null;
            try
            {
                if (!CheckCommonParametersAndThrowExceptionForMissingFields(parameters))
                    return null;//Return null if logging is disabled

                Kernel.Connection.TransactionBegin();

                LogStatement statement = CreateNewStatement();

                statement.LogAgentId = CacheHelper.GetAgent(parameters.StudentId.Value).LogAgentId;
                statement.LogVerbId = CacheHelper.GetVerb(Verbs.SessionAbandoned).LogVerbId;
                LogContext context = CreateNewContext(parameters.Registration.Value);
                ThrowError(context.OnInsert());

                LogExtension ext = CreateLogExtensionForInstanceId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForExerciseType(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForReferrer(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForHintId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForSolutionId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForFeedbackId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForDictionaryId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForNavigationType(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForExploreHotspotId(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                //ext = CreateLogExtensionForContentTitle(context, parameters);
                //if (ext != null) throwError(ext.OnInsert());
                //ext = CreateLogExtensionForParentUri(context, parameters);
                //if (ext != null) throwError(ext.OnInsert());
                //ext = CreateLogExtensionForParentTitle(context, parameters);
                //if (ext != null) throwError(ext.OnInsert());
                ext = CreateLogExtensionForMigrateSessionTo(context, parameters);
                if (ext != null) ThrowError(ext.OnInsert());



                LogActivity activity = new LogActivity();
                activity.LogActivityDefinitionId = CacheHelper.GetActivityDefinition(CacheHelper.GetLogActivityDefinitionForAccessType(parameters.AccessType)).LogActivityDefinitionId;
                activity.Id = parameters.AccessedUri.ToString();
                ThrowError(activity.OnInsert());

                ext = CreateLogExtensionForAccessType(activity, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                if (parameters.AccessType == AccessTypes.Dictionary)
                {
                    ext = CreateLogExtensionForDictionaryId(activity, parameters);
                    if (ext != null) ThrowError(ext.OnInsert());
                }
                ext = CreateLogExtensionForTabTitle(activity, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForExploreHotspotTabId(activity, parameters);
                if (ext != null) ThrowError(ext.OnInsert());
                ext = CreateLogExtensionForExploreHotspotTabCount(activity, parameters);
                if (ext != null) ThrowError(ext.OnInsert());

                AddActivityName(activity, parameters.AccessedContentTitle);
                AddActivityDescription(activity, parameters.AccessedContentDescription);

                if (parameters.ParentUri != null)
                {
                    //TODO: look up the parent if it is not provided?

                    LogActivity parentActivity = new LogActivity();
                    parentActivity.LogActivityDefinitionId = CacheHelper.GetActivityDefinition(
                        ((parameters.ParentAccessType.HasValue) ?
                        CacheHelper.GetLogActivityDefinitionForAccessType(parameters.ParentAccessType.Value) :
                        LogActivityDefinitions.Module)).LogActivityDefinitionId;
                    parentActivity.Id = parameters.ParentUri.ToString();
                    ThrowError(parentActivity.OnInsert());

                    AddActivityName(parentActivity, parameters.ParentTitle);
                    //AddActivityDescription(activity, parameters.AccessedContentDescription);
                    ext = CreateLogExtensionForParentAccessType(parentActivity, parameters);
                    if (ext != null) ThrowError(ext.OnInsert());

                    LogContextActivity contextAct = new LogContextActivity();
                    contextAct.LogActivityId = parentActivity.LogActivityId;
                    contextAct.LogContextActivityTypeId = (long)LogContextActivityTypes.Parent;
                    contextAct.LogContextId = context.LogContextId;
                    ThrowError(contextAct.OnInsert());

                }


                statement.LogContextId = context.LogContextId;
                statement.TargetLogActivityId = activity.LogActivityId;
                ThrowError(statement.OnInsert());


                AddCommonLogStatementLinks(parameters, statement);

            }
            catch (Exception ex)
            {
                //Log error
                error = new ErrorLog(ex);
            }
            finally
            {
                //SaveToEventLog("LogResourceAccess() finished. Error = " + ((error == null)?"null":error.ToExtendedString()), 0);

                if (Kernel.Connection.TransactionActive())
                {
                    if (error == null)
                        Kernel.Connection.TransactionCommit();
                    else
                        Kernel.Connection.TransactionRollback();
                }
            }
            return error;
        }

        public ErrorLog UpdateLogAgentPassword(long logAgentId, string password)
        {
            if (logAgentId == LMG.Infrastructure.Analytics.Objects.DB.Base.BaseObject.InvalidPrimaryKey)
                return null;

            try
            {
                SqlQueryUpdate up = new SqlQueryUpdate("LogAgent");
                up.Set("Password", SqlValue.Convert(password));
                up.Where("LogAgentId", SqlValue.Convert(logAgentId));
                SqlResult res = Kernel.Connection.ExecuteQuery(up);
            }
            catch (Exception ex)
            {
                return new ErrorLog(ex);
            }
            return null;
        }
        public ErrorLog UpdateLogStatementWithStoredTimestamp(Guid? statement_id, DateTime? stored)
        {
            if (!statement_id.HasValue)
                return null;

            try
            {
                SqlQueryUpdate up = new SqlQueryUpdate("LogStatement");
                up.Set("StoredTimestamp", SqlValue.Convert(stored));
                up.Where("LogStatementId", "'"+statement_id.Value.ToString()+"'");
                SqlResult res = Kernel.Connection.ExecuteQuery(up);
            }
            catch (Exception ex)
            {
                return new ErrorLog(ex);
            }
            return null;
        }

        

        public bool CheckLogPreference(long? learningModuleId, long? studentId)
        {
            bool globalAllows = ("" + ConfigurationManager.AppSettings["DisableAnalytics"]).ToLower() != "true";
            if (!globalAllows)
                return false;

            bool studentAllows = true;
            if (studentId.HasValue)
            {
                StudentsCollection students = new StudentsCollection();
                students.FillCollection("Id = " + SqlValue.Convert(studentId), "");
                if (students.Count == 1)
                {
                    studentAllows = true;//todo rmenten 2016-04-15: add column to table:  Students.TrackResults;
                }
            }
            if (!studentAllows)
                return false;
            //Student allows it. Check other levels as well.


            bool learningModuleAllows = true;
            if (learningModuleId.HasValue)
            {
                SqlQuerySelect sel = new SqlQuerySelect();
                sel.Select("TrackResults").From("LearningModule").Where("LearningModuleId", SqlValue.Convert(learningModuleId));
                SqlResult res = Kernel.Connection.ExecuteQuery(sel);
                learningModuleAllows = res.GetBoolean(0, 0, false);
            }
            if (!learningModuleAllows)
                return false;
            //LearningModule allows it. Check other levels as well.

            if (false)
            {
                bool studentGroupAllows = true;
                if (studentId.HasValue)
                {
                    //todo rmenten 2016-04-15: add column to table:  StudentGroup.TrackResults;
                    SqlQuerySelect sel = new SqlQuerySelect();
                    sel.Select("StudentGroup.TrackResults")
                        .From("Student_x_StudentGroup")
                        .InnerJoin("StudentGroup", "StudentGroup.StudentGroupId = Student_x_StudentGroup.StudentGroupId")
                        .Where("Student_x_StudentGroup.StudentId", SqlValue.Convert(studentId));
                    SqlResult res = Kernel.Connection.ExecuteQuery(sel);

                    studentGroupAllows = res.GetBoolean(0, 0, false);
                }
                if (!studentGroupAllows)
                    return false;
            }

            return true;
        }


        public LogStatementCollection GetUnprocessedLogStatements(DateTime? startDate = null, DateTime? endDate = null, List<string> studentGroups = null, bool? onlyUnsentStatements = true, int index_from = -1, int index_offset = -1, bool filterStudents = false)
        {
            List<long> studentGroupIds = new List<long>();
            if (studentGroups != null && studentGroups.Count > 0)
            {
                string sql = @"SELECT StudentGroupId from StudentGroup WHERE Name in ('"+string.Join("','", studentGroups)+@"')";

                SqlQueryCustom sel = new SqlQueryCustom(sql);
                SqlResult res = Kernel.Connection.ExecuteQuery(sql);
                if (res.RowCount > 0)
                {
                    for(int i = 0 ; i < res.RowCount ; i++)
                        studentGroupIds.Add(res.GetInt64(i, 0, -1));
                } 
                
            }


            string limitAppendix = "";
            if (index_from != -1 || index_offset != -1)
            {
                //offset 0 rows fetch next 100 rows only
                limitAppendix = " OFFSET " + ((index_from == -1) ? 0 : index_from) + " rows";
                if (index_offset != -1)
                    limitAppendix += " fetch next " + index_offset + " rows only";
            }


            LogStatementCollection list = new LogStatementCollection();
            list.FillCollection("1 = 1"
                + ((onlyUnsentStatements == true)?" AND StoredTimestamp is null":"")
                + ((startDate.HasValue) ? " AND CAST(Timestamp as DATE) >= " +SqlValue.Convert(startDate) : "")
                + ((endDate.HasValue) ? " AND CAST(Timestamp as DATE) <= " + SqlValue.Convert(endDate) : "")
                + ((studentGroupIds.Count > 0) ? " AND EXISTS (SELECT * FROM LogAgent INNER JOIN Student_x_StudentGroup ON LogAgent.StudentId = Student_x_StudentGroup.StudentId WHERE LogAgent.LogAgentId = LogStatement.LogAgentId AND Student_x_StudentGroup.StudentGroupId IN (" + string.Join(",", studentGroupIds) + "))" : "")
                + ((filterStudents) ? " AND EXISTS (SELECT * FROM LogAgentLRSFilter WHERE LogAgentLRSFilter.LogAgentId = LogStatement.LogAgentId)" : "")
                //+ ((showOnlyLocalhost == true) ? "AND AuthorityLogAgentId = LogAgentId" : "")
                //+ ((showOnlyLocalhost == false) ? "AND AuthorityLogAgentId IS NULL" : "")
                , "Timestamp asc, TargetLogActivityId asc, LogContextId asc" + limitAppendix);

            foreach (LogStatement record in list)
            {
                ResolveIDsToObjects(record);
            }
            return list;
        }

     
        /// <summary>
        /// The provided should only return the list of logstatementids. No other fields are expected!
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public LogStatementCollection GetLogStatements(string query)
        {
            LogStatementCollection list = new LogStatementCollection();
            list.FillCollection("1 = 1"
               + ((!string.IsNullOrWhiteSpace(query)) ? " AND logstatementid in ("+query+")" : "")
                , "Timestamp asc, TargetLogActivityId asc, LogContextId asc");

            foreach (LogStatement record in list)
            {
                ResolveIDsToObjects(record);
            }
            return list;
        }
        
        public static void ResolveIDsToObjects(LogStatement record)
        {
            if (record.AuthorityLogAgentId.HasValue)
                record.AuthorityLogAgentIdResolved = CacheHelper.GetAgent(record.AuthorityLogAgentId.Value);
            if (record.LogAgentId.HasValue)
                record.LogAgentIdResolved = CacheHelper.GetAgent(record.LogAgentId.Value);
            if (record.LogContextId.HasValue)
                record.LogContextIdResolved = CacheHelper.GetContext(record.LogContextId.Value);
            if (record.LogResultId.HasValue)
                record.LogResultIdResolved = CacheHelper.GetResult(record.LogResultId.Value);
            if (record.LogVerbId.HasValue)
                record.LogVerbIdResolved = CacheHelper.GetVerb(record.LogVerbId.Value);
            if (record.TargetLogActivityId.HasValue)
                record.TargetLogActivityIdResolved = CacheHelper.GetActivity(record.TargetLogActivityId.Value);
            if (record.TargetLogAgentId.HasValue)
                record.TargetLogAgentIdResolved = CacheHelper.GetAgent(record.TargetLogAgentId.Value);
            //if (record.TargetLogStatementId.HasValue)
            //    record.TargetLogStatementIdResolved = CacheHelper.GetVerb(record.TargetLogStatementId.Value);
        }

        public static double? GetHighScore(long? logAgentId = null, long? exerciseId = null)
        {
            try
            {
                string sql = @"select top 1 LogScore.Scaled
                from LogStatement
                inner join LogStatementLink on LogStatement.LogStatementId = LogStatementLink.LogStatementId and LogStatementLink.TableName = 'Exercise'
                inner join LogResult on LogResult.LogResultId = LogStatement.LogResultId
                inner join LogScore on LogScore.LogScoreId = LogResult.LogScoreId
                where LogVerbId = " + (int)Verbs.Completed;
                if (logAgentId.HasValue)
                    sql += " and LogStatement.LogAgentId = " + logAgentId.Value + " ";
                if (exerciseId.HasValue)
                    sql += " and LogStatementLink.TableId = '" + exerciseId.Value + "' ";
                //sql += "order by timestamp desc";
                sql += "order by LogScore.Scaled desc";

                //inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
                //left outer join LogExtension on LogExtension.LogContextId = LogStatement.LogContextId and LogExtension.Uri = 'http://www.project-vital.eu/xapi/extension/instance-id'

                SqlQueryCustom sel = new SqlQueryCustom(sql);
                SqlResult res = Kernel.Connection.ExecuteQuery(sql);
                if (res.RowCount > 0)
                {
                    return res.GetNullableFloat64(0, 0);
                }
            }
            catch
            {
            }
            return null;
        }
        public static string GetPreviousSessionLastAccessedUrl(long? logAgentId = null, long? learningModuleId = null)
        {
            try
            {
                string sql = @"select top 1 LogActivity.Id from logstatement
                    left join LogAgent on LogAgent.LogAgentId = LogStatement.LogAgentId
                    left join LogActivity on LogActivity.LogActivityId = LogStatement.TargetLogActivityId
                    left join LogActivityDefinition on LogActivityDefinition.LogActivityDefinitionId = LogActivity.LogActivityDefinitionId";
                if(learningModuleId.HasValue)
                    sql += " inner join LogStatementLink on LogStatement.LogStatementId = LogStatementLink.LogStatementId and LogStatementLink.TableName = 'LearningModule'";
                sql += " where LogStatement.LogVerbId = " + (int)Verbs.Accessed; 
                if(logAgentId.HasValue)
                    sql += " AND logagent.logagentid = " + logAgentId;
                if (learningModuleId.HasValue)
                    sql += " AND LogStatementLink.TableId = '" + learningModuleId + "'";
                sql += @" and LogActivity.LogActivityDefinitionId in (" + (int)LogActivityDefinitions.Course + "," + (int)LogActivityDefinitions.Module + ")";
                sql += " order by timestamp desc";

                SqlQueryCustom sel = new SqlQueryCustom(sql);
                SqlResult res = Kernel.Connection.ExecuteQuery(sql);
                if (res.RowCount > 0)
                {
                    return res.GetString(0, 0, null);
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        #region Object helpers
        private void AddActivityName(LogActivity activity, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;
            LogActivityDefinitionDetail activityDetail = new LogActivityDefinitionDetail();
            activityDetail.LogActivityDefinitionDetailTypeId = (long)LogActivityDefinitionDetailTypes.Name;
            activityDetail.Language = InstructionLanguage;
            activityDetail.Label = name;
            activityDetail.LogActivityId = activity.LogActivityId;
            ThrowError(activityDetail.OnInsert());
        }
        private void AddActivityDescription(LogActivity activity, string descr)
        {
            if (string.IsNullOrWhiteSpace(descr))
                return;
            LogActivityDefinitionDetail activityDetail = new LogActivityDefinitionDetail();
            activityDetail.LogActivityDefinitionDetailTypeId = (long)LogActivityDefinitionDetailTypes.Description;
            activityDetail.Language = InstructionLanguage;
            activityDetail.Label = descr;
            activityDetail.LogActivityId = activity.LogActivityId;
            ThrowError(activityDetail.OnInsert());
        }
        private void AddLogStatementLink(Guid logStatementId, EMCGTables table, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                LogStatementLink link = new LogStatementLink();
                link.LogStatementId = logStatementId;
                link.TableName = table.ToString();
                link.TableId = value;
                ThrowError(link.OnInsert());
            }
        }
        private void AddCommonLogStatementLinks(LogParameters parameters, LogStatement statement)
        {
            AddLogStatementLink(statement.LogStatementId, EMCGTables.LearningModule, "" + parameters.LearningModuleId);
            AddLogStatementLink(statement.LogStatementId, EMCGTables.LearningModuleProfile, "" + parameters.LearningModuleProfileId);
            AddLogStatementLink(statement.LogStatementId, EMCGTables.Exercise, "" + parameters.ExerciseId);
            AddLogStatementLink(statement.LogStatementId, EMCGTables.ExerciseTreeItem, "" + parameters.ExerciseTreeItemId);
            AddLogStatementLink(statement.LogStatementId, EMCGTables.TheoryPage, "" + parameters.TheoryPageId);
            AddLogStatementLink(statement.LogStatementId, EMCGTables.TheoryPageTreeItem, "" + parameters.TheoryPageTreeItemId);
            if (parameters is LogParametersForDictionaryEntrySearch)
            {
                AddLogStatementLink(statement.LogStatementId, EMCGTables.Dictionary, "" + (parameters as LogParametersForDictionaryEntrySearch).DictionaryId);
                AddLogStatementLink(statement.LogStatementId, EMCGTables.DictionaryEntry, "" + (parameters as LogParametersForDictionaryEntrySearch).DictionaryEntryId);
            }
        }
        private void ThrowError(ErrorLog error)
        {
            if (error != null)
                throw new Exception(error.Message, error.Exception);
        }
        private void ThrowMissingFieldException(string field_name)
        {
            string message = ERROR_REQUIRED_FIELD_MISSING + ": " + field_name;
            //SaveToEventLog(message, 0);
            throw new Exception(message);
        }
        private void SaveToEventLog(string message, int eventID)
        {
            try
            {
                string sSource;
                string sLog;
                string sEvent;

                sSource = "LMG.Infrastructure.Analytics::AnalyticsWorker";
                sLog = "Application";
                sEvent = message;

                if (!System.Diagnostics.EventLog.SourceExists(sSource))
                    System.Diagnostics.EventLog.CreateEventSource(sSource, sLog);
                System.Diagnostics.EventLog.WriteEntry(sSource, sEvent, System.Diagnostics.EventLogEntryType.Error, eventID);
            }
            catch
            {
            }
        }
        private bool CheckCommonParametersAndThrowExceptionForMissingFields(LogParameters parameters)
        {
            LogAgent student = null;
            if(parameters.StudentId.HasValue)
                student = CacheHelper.GetAgent(parameters.StudentId.Value);
            if (student == null) ThrowMissingFieldException("StudentId");

            Uri mediaUri = parameters.AccessedUri;
            if (mediaUri == null) ThrowMissingFieldException("AccessedUri");

            if (!parameters.Registration.HasValue) ThrowMissingFieldException("Registration");

            if (parameters.LearningModuleId.HasValue)
            {//Get instruction language from learningmodule
                string instrLanguage = CacheHelper.GetLearningModuleInstructionLanguage(parameters.LearningModuleId.Value);
                if (!string.IsNullOrWhiteSpace(instrLanguage))
                {
                    m_instruction_language = instrLanguage;
                }
            }

            if (CheckLogPreference(parameters.LearningModuleId, parameters.StudentId) == false)
                return false;
            else
                return true;
        }

        private string GetMimeTypeForUrl(Uri uri)
        {
            if (uri == null)
                return null;
            return GetMimeTypeForUrl(uri.ToString());
        }
        private string GetMimeTypeForUrl(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
                return null;
            uri = uri.ToLower();

            if (uri.EndsWith(".mp3"))
            {
                return "audio/mpeg";
            }
            else if (uri.EndsWith(".mp4"))
            {
                return "video/mp4";
            }

            return null;
            //Audio
            //     audio/aac .aac
            //     audio/mp4 .mp4 .m4a
            //     audio/mpeg .mp1 .mp2 .mp3 .mpg .mpeg
            //     audio/ogg .oga .ogg
            //     audio/wav .wav
            //     audio/webm .webm
            //Video
            //     video/mp4 .mp4 .m4v
            //     video/ogg .ogv
            //     video/webm .webm

        }

        private LogContext CreateNewContext(Guid registration)
        {
            LogContext context = new LogContext();
            context.Registration = registration;
            context.Platform = "EMCG";
            context.Language = m_language;

            return context;
        }
        private LogStatement CreateNewStatement()
        {
            LogStatement statement = new LogStatement();
            statement.LogStatementId = Guid.NewGuid();
            statement.Timestamp = DateTime.Now;

            //statement.LogAgentId
            //statement.LogVerbId = CacheHelper.GetVerb(Verbs.Completed).LogVerbId;

            return statement;
        }
        private LogExtension CreateLogExtensionForInstanceId(LogContext context, LogParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.InstanceId))
                return null;
            return CreateLogExtension(context, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.InstanceId), parameters.InstanceId);
        }
        private LogExtension CreateLogExtensionForAttempt(LogResult result, LogParametersForExerciseResult parameters)
        {
            if (!parameters.Attempt.HasValue)
                return null;
            //CacheHelper.GetXAPIDefinitionsBaseURL("extensions/attempt")
            return CreateLogExtension(result, parameters, "http://id.tincanapi.com/extension/attempt-id", "" + parameters.Attempt);
        }
        private LogExtension CreateLogExtensionForCorrectAnswers(LogResult result, LogParametersForExerciseResult parameters)
        {
            if (parameters.CorrectAnswers == null || parameters.CorrectAnswers.Count == 0)
                return null;

            string correctAnswers = Newtonsoft.Json.JsonConvert.SerializeObject(parameters.CorrectAnswers);
            //Newtonsoft.Json.Linq.JObject job = new Newtonsoft.Json.Linq.JObject();
            //foreach (string key in parameters.CorrectAnswers.Keys)
            //{
            //    string value = parameters.CorrectAnswers[key];
            //}

            return CreateLogExtension(result, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.CorrectAnswers), correctAnswers);
        }
        private LogExtension CreateLogExtensionForAttemptMax(LogResult result, LogParametersForExerciseResult parameters)
        {
            if (!parameters.MaxAttempts.HasValue)
                return null;
            return CreateLogExtension(result, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.AttemptMax), "" + parameters.MaxAttempts);
        }
        private LogExtension CreateLogExtensionForSentenceId(LogResult result, LogParametersForExerciseResult parameters)
        {
            if (!parameters.SentenceId.HasValue)
                return null;
            return CreateLogExtension(result, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.SentenceId), "" + parameters.SentenceId);
        }
        private LogExtension CreateLogExtensionForDictionarySearchResultCount(LogResult result, LogParametersForDictionaryEntrySearch parameters)
        {
            if (!parameters.DictionarySearchResultCount.HasValue)
                return null;
            return CreateLogExtension(result, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.DictionarySearchResultCount), "" + parameters.DictionarySearchResultCount.Value);
        }
        private LogExtension CreateLogExtensionForMatchCheckDragId(LogResult result, LogParametersForExerciseInteraction parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.MatchDragId))
                return null;
            return CreateLogExtension(result, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.DragId), "" + parameters.MatchDragId);
        }
        private LogExtension CreateLogExtensionForMatchCheckDropTargetId(LogResult result, LogParametersForExerciseInteraction parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.MatchDropTargetId))
                return null;
            return CreateLogExtension(result, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.DropTargetId), "" + parameters.MatchDropTargetId);
        }
        private LogExtension CreateLogExtensionForExerciseType(LogContext result, LogParameters parameters)
        {
            if (!parameters.ExerciseType.HasValue)
                return null;
            return CreateLogExtension(result, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.ExerciseType), "" + parameters.ExerciseType);
        }
        private LogExtension CreateLogExtensionForAccessType(LogActivity activity, LogParametersForAccess parameters)
        {
            return CreateLogExtension(activity, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.AccessType), "" + parameters.AccessType);
        }
        private LogExtension CreateLogExtensionForParentAccessType(LogActivity result, LogParametersForAccess parameters)
        {
            if (!parameters.ParentAccessType.HasValue)
                return null;
            return CreateLogExtension(result, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.AccessType), "" + parameters.ParentAccessType.Value);
        }
        private LogExtension CreateLogExtensionForMediaStartingPoint(LogContext result, LogParametersForMediaInteraction parameters)
        {
            if (!parameters.MediaStartingPoint.HasValue)
                return null;
            return CreateLogExtension(result, parameters, "http://id.tincanapi.com/extension/starting-point", "" + parameters.MediaStartingPoint.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
        private LogExtension CreateLogExtensionForMediaEndingPoint(LogContext result, LogParametersForMediaInteraction parameters)
        {
            if (!parameters.MediaEndingPoint.HasValue)
                return null;
            return CreateLogExtension(result, parameters, "http://id.tincanapi.com/extension/ending-point", "" + parameters.MediaEndingPoint.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
        private LogExtension CreateLogExtensionForReferrer(LogContext result, LogParametersForAccess parameters)
        {
            if (parameters.LinkReferrer == null)
                return null;
            return CreateLogExtension(result, parameters, "http://id.tincanapi.com/extension/referrer", "" + parameters.LinkReferrer);
        }
        private LogExtension CreateLogExtensionForMimeType(LogContext result, LogParameters parameters, string mimetype)
        {
            if (string.IsNullOrWhiteSpace(mimetype))
                return null;
            return CreateLogExtension(result, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.MimeType), mimetype);
        }
        private LogExtension CreateLogExtensionForMimeType(LogActivity activity, LogParameters parameters, string mimetype)
        {
            if (string.IsNullOrWhiteSpace(mimetype))
                return null;
            return CreateLogExtension(activity, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.MimeType), mimetype);
        }
        private LogExtension CreateLogExtensionForDictionaryId(LogActivity activity, LogParameters parameters)
        {
            if (!parameters.DictionaryId.HasValue)
                return null;
            return CreateLogExtension(activity, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.DictionaryId), "" + parameters.DictionaryId.Value);
        }
        private LogExtension CreateLogExtensionForTabTitle(LogActivity activity, LogParametersForAccess parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.TabTitle))
                return null;
            return CreateLogExtension(activity, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.TabTitle), parameters.TabTitle);
        }
        private LogExtension CreateLogExtensionForExploreHotspotId(LogContext context, LogParametersForAccess parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.ExploreHotspotId))
                return null;
            return CreateLogExtension(context, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.ExploreHotspotId), parameters.ExploreHotspotId);
        }
        private LogExtension CreateLogExtensionForExploreHotspotTabId(LogActivity activity, LogParametersForAccess parameters)
        {
            if (!parameters.ExploreHotspotTabId.HasValue)
                return null;
            return CreateLogExtension(activity, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.ExploreHotspotTabId), "" + parameters.ExploreHotspotTabId.Value);
        }
        private LogExtension CreateLogExtensionForExploreHotspotTabCount(LogActivity activity, LogParametersForAccess parameters)
        {
            if (!parameters.ExploreHotspotTabCount.HasValue)
                return null;
            return CreateLogExtension(activity, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.ExploreHotspotTabCount), "" + parameters.ExploreHotspotTabCount.Value);
        }
        private LogExtension CreateLogExtensionForIsVoiceRecording(LogActivity activity, LogParametersForMediaInteraction parameters)
        {
            if (!parameters.IsVoiceRecording.HasValue)
                return null;
            return CreateLogExtension(activity, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.IsVoiceRecording), ""+parameters.IsVoiceRecording.Value);
        }
        private LogExtension CreateLogExtensionForDictionaryId(LogContext context, LogParameters parameters)
        {
            if (!parameters.DictionaryId.HasValue)
                return null;
            return CreateLogExtension(context, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.DictionaryId), "" + parameters.DictionaryId.Value);
        }
        private LogExtension CreateLogExtensionForDictionaryEntryId(LogContext context, LogParametersForDictionaryEntrySearch parameters)
        {
            if (!parameters.DictionaryEntryId.HasValue)
                return null;
            return CreateLogExtension(context, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.DictionaryEntryId), "" + parameters.DictionaryEntryId.Value);
        }
        private LogExtension CreateLogExtensionForDictionarySearchOption(LogContext context, LogParametersForDictionaryEntrySearch parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.DictionarySearchOption))
                return null;
            return CreateLogExtension(context, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.DictionarySearchOption), "" + parameters.DictionarySearchOption);
        }
        private LogExtension CreateLogExtensionForDictionarySearchTerm(LogContext context, LogParametersForDictionaryEntrySearch parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.DictionarySearchTerm))
                return null;
            return CreateLogExtension(context, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.DictionarySearchTerm), "" + parameters.DictionarySearchTerm);
        }
        private LogExtension CreateLogExtensionForNavigationType(LogContext result, LogParametersForAccess parameters)
        {
            if (!parameters.NavigationType.HasValue)
                return null;
            return CreateLogExtension(result, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.NavigationType), "" + parameters.NavigationType);
        }
        private LogExtension CreateLogExtension(LogContext context, LogParameters parameters, string uri, string token)
        {
            if (token == null)
                return null;

            LogExtension ext = CreateLogExtensionBase(context, null, null, null);
            ext.Uri = uri;
            ext.Token = token;
            return ext;
        }
        private LogExtension CreateLogExtensionForHintId(LogContext context, LogParametersForAccess parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.HintId))
                return null;
            return CreateLogExtension(context, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.HintId), parameters.HintId);
        }
        private LogExtension CreateLogExtensionForSolutionId(LogContext context, LogParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.SolutionId))
                return null;
            return CreateLogExtension(context, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.SolutionId), parameters.SolutionId);
        }
        private LogExtension CreateLogExtensionForSentenceId(LogContext context, LogParameters parameters)
        {
            if (!parameters.SentenceId.HasValue)
                return null;
            return CreateLogExtension(context, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.SentenceId), ""+parameters.SentenceId.Value);
        }
        private LogExtension CreateLogExtensionForFeedbackId(LogContext context, LogParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.FeedbackId))
                return null;
            return CreateLogExtension(context, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.FeedbackId), parameters.FeedbackId);
        }
        private LogExtension CreateLogExtensionForMigrateSessionTo(LogContext context, LogParametersForAccess parameters)
        {
            if (!parameters.SessionMigrationTo.HasValue)
                return null;
            return CreateLogExtension(context, parameters, CacheHelper.GetXAPIDefinitionsBaseURL(CustomXAPIDefinitionExtensions.MigrateSessionTo), ""+parameters.SessionMigrationTo.Value);
        }
        private LogExtension CreateLogExtension(LogResult result, LogParameters parameters, string uri, string token)
        {
            if (token == null)
                return null;

            LogExtension ext = CreateLogExtensionBase(null, result, null, null);
            ext.Uri = uri;
            ext.Token = token;
            return ext;
        }
        private LogExtension CreateLogExtension(LogActivityDefinition activityDefinition, LogParameters parameters, string uri, string token)
        {
            if (token == null)
                return null;

            LogExtension ext = CreateLogExtensionBase(null, null, activityDefinition, null);
            ext.Uri = uri;
            ext.Token = token;
            return ext;
        }
        private LogExtension CreateLogExtension(LogActivity activity, LogParameters parameters, string uri, string token)
        {
            if (token == null)
                return null;

            LogExtension ext = CreateLogExtensionBase(null, null, null, activity);
            ext.Uri = uri;
            ext.Token = token;
            return ext;
        }
        private LogExtension CreateLogExtensionBase(LogContext context, LogResult result, LogActivityDefinition activityDefinition, LogActivity activity)
        {
            LogExtension ext = new LogExtension();
            if(context != null)
                ext.LogContextId = context.LogContextId;
            else if (result != null)
                ext.LogResultId = result.LogResultId;
            else if (activityDefinition != null)
                ext.LogActivityDefinitionId = activityDefinition.LogActivityDefinitionId;
            else if (activity != null)
                ext.LogActivityId = activity.LogActivityId;
            return ext;
        }
        #endregion



        public void ExecuteQuery(string q)
        {
            Kernel.Connection.ExecuteQuery(q);
        }

        private Dictionary<int, DateTime> m_CalculateStudentPerformanceGroup = new Dictionary<int, DateTime>();
        public void CalculateStudentPerformanceGroup()
        {
            decimal median_session_count = 29;
            //decimal median_activity_type_exercise_count = 388.5m;
            decimal median_exercise_count = 526m;
            decimal median_theory_page_count = 69m;
            decimal median_assessment_score = 0;
            
            decimal lower_quartile_session_count = 21;
            //decimal lower_quartile_activity_type_exercise_count = 173m;
            decimal lower_quartile_exercise_count = 305m;
            decimal lower_quartile_theory_page_count = 52.5m;

            int courseInstanceId = 13;//FdA1

            bool doForOtherPredefinedCourseInstance = true;
            if (doForOtherPredefinedCourseInstance)
            {
                //Numbers provided by Anouk / R
                median_session_count = 33;
                median_exercise_count = 653m;
                median_theory_page_count = 200;

                lower_quartile_session_count = 25;
                lower_quartile_exercise_count = 274;
                lower_quartile_theory_page_count = 109.5m;


                //Numbers provided by function CalculateStudentPerformanceGroupThresholds
                //median_session_count = 31;
                //median_exercise_count = 563.5m;
                //median_theory_page_count = 105;

                //lower_quartile_session_count = 16;
                //lower_quartile_exercise_count = 134;
                //lower_quartile_theory_page_count = 32m;

                courseInstanceId = 15;//FdA2
            }
            else if (Kernel.Connection.directOdbcConnection.Connection.Database == "VITAL.UvA")
            {
                median_session_count = 22;
                median_exercise_count = 247.5m;
                median_theory_page_count = 105;
                median_assessment_score = 0.60m;

                lower_quartile_session_count = 16;
                lower_quartile_exercise_count = 91;
                lower_quartile_theory_page_count = 60;

                

                courseInstanceId = 9;
            }
            else if (Kernel.Connection.directOdbcConnection.Connection.Database == "VITAL.UCLan")
            {
                median_session_count = 22;
                median_exercise_count = 247.5m;
                median_theory_page_count = 105;
                median_assessment_score = 0.60m;

                lower_quartile_session_count = 16;
                lower_quartile_exercise_count = 91;
                lower_quartile_theory_page_count = 60;

                

                courseInstanceId = 5;
            }

            CalculateStudentPerformanceGroupThresholds(courseInstanceId);

            //int current_week_number = 0;
            //int max_week_number = 0;
            //if (GetWeeknumberProgress(courseInstanceId, ref current_week_number, ref max_week_number))
            int current_day_number = 0;
            int max_day_number = 0;
            if (GetDayNumberProgress(courseInstanceId, ref current_day_number, ref max_day_number))
            {
                decimal day_factor = (decimal)current_day_number / (decimal)max_day_number;

                List<long> flagged_students = new List<long>();
                List<long> students_matching_at_least_one = new List<long>();

                List<Tuple<long, decimal>> student_session_count = ListStudentsBySessionCountThreshold(courseInstanceId);
                CalculateStudentPerformanceGroup_HandleCriterion(student_session_count, median_session_count * day_factor, lower_quartile_session_count * day_factor, flagged_students, students_matching_at_least_one);

                List<Tuple<long, decimal>> student_exercise_count = ListStudentsByExerciseCountThreshold(courseInstanceId);
                CalculateStudentPerformanceGroup_HandleCriterion(student_exercise_count, median_exercise_count * day_factor, lower_quartile_exercise_count * day_factor, flagged_students, students_matching_at_least_one);

                List<Tuple<long, decimal>> student_theory_page_count = ListStudentsByTheoryPageCountThreshold(courseInstanceId);
                CalculateStudentPerformanceGroup_HandleCriterion(student_theory_page_count, median_theory_page_count * day_factor, lower_quartile_theory_page_count * day_factor, flagged_students, students_matching_at_least_one);

                List<Tuple<long, decimal>> student_with_last_login_long_time_ago = ListStudentsByLoginLongTimeAgo(courseInstanceId);
                CalculateStudentPerformanceGroup_HandleCriterion(student_with_last_login_long_time_ago, 0, 0, flagged_students, students_matching_at_least_one);

                if (Kernel.Connection.directOdbcConnection.Connection.Database == "VITAL.UvA")
                {
                    List<Tuple<long, decimal>> student_assessment_score = ListStudentsByAssessmentScore(courseInstanceId);
                    CalculateStudentPerformanceGroup_HandleCriterion(student_assessment_score, median_assessment_score, median_assessment_score, flagged_students, students_matching_at_least_one);
                }

                UpdateStudentPerformanceGroup(courseInstanceId, flagged_students);

                /*//Check validity by comparing with the scores
                 
                   SELECT calculatedperformancegroup, score
                      FROM [LogMetadataAgentInCourseInstance]
                      where LogMetadataCourseInstanceId = 13
                      --and calculatedperformancegroup = 'A' AND score < 10
                      and calculatedperformancegroup = 'B' AND score >= 10
                      order by calculatedperformancegroup
                 
                 
                 */


            }
        }
        public void CalculateStudentPerformanceGroupThresholds(int courseInstanceId)
        {
            decimal median_session_count = 0;
            decimal median_exercise_count = 0;
            decimal median_theory_page_count = 0;

            decimal lower_quartile_session_count = 0;
            decimal lower_quartile_exercise_count = 0;
            decimal lower_quartile_theory_page_count = 0;


            //int current_week_number = 0;
            //int max_week_number = 0;
            //if (GetWeeknumberProgress(courseInstanceId, ref current_week_number, ref max_week_number))
            int current_day_number = 0;
            int max_day_number = 0;
            if (GetDayNumberProgress(courseInstanceId, ref current_day_number, ref max_day_number))
            {
                decimal day_factor = (decimal)current_day_number / (decimal)max_day_number;

                List<long> students_who_passed = ListStudentsWhoPassedCourse(courseInstanceId);

                List<Tuple<long, decimal>> student_session_count = ListStudentsBySessionCountThreshold(courseInstanceId);
                List<Tuple<long, decimal>> student_exercise_count = ListStudentsByExerciseCountThreshold(courseInstanceId);
                List<Tuple<long, decimal>> student_theory_page_count = ListStudentsByTheoryPageCountThreshold(courseInstanceId);
                //List<Tuple<long, decimal>> student_with_last_login_long_time_ago = ListStudentsByLoginLongTimeAgo(courseInstanceId);

                List<decimal> session_counts = GetCountForPassedStudents(student_session_count, students_who_passed);
                median_session_count = GetMedian(session_counts);
                lower_quartile_session_count = GetMedian(session_counts.Where(x => x < median_session_count).ToList());

                List<decimal> exercise_counts = GetCountForPassedStudents(student_exercise_count, students_who_passed);
                median_exercise_count = GetMedian(exercise_counts);
                lower_quartile_exercise_count = GetMedian(exercise_counts.Where(x => x < median_exercise_count).ToList());

                List<decimal> theory_page_counts = GetCountForPassedStudents(student_theory_page_count, students_who_passed);
                median_theory_page_count = GetMedian(theory_page_counts);
                lower_quartile_theory_page_count = GetMedian(theory_page_counts.Where(x => x < median_theory_page_count).ToList());
                //session_counts.Sort();
            }
        }
        private List<decimal> GetCountForPassedStudents(List<Tuple<long, decimal>> list_to_filter, List<long> students_who_passed)
        {
            List<decimal> counts = new List<decimal>();
            foreach (Tuple<long, decimal> t in list_to_filter)
            {
                //if (students_who_passed.Contains(t.Item1))
                    counts.Add(t.Item2);
            }
            return counts;
        }
        private List<long> ListStudentsWhoPassedCourse(int courseInstanceId)
        {
            decimal threshold = 0.5m;
            if (Kernel.Connection.directOdbcConnection.Connection.Database == "VITAL.UvA")
                threshold = 0.55m;

            string sql = @"select * from LogMetadataAgentInCourseInstance
                where logmetadatacourseinstanceid = " + courseInstanceId + @" and Score/ScoreMax >= " + SqlValue.Convert(threshold);

            SqlQueryCustom sel = new SqlQueryCustom(sql);

            List<long> students = new List<long>();
            SqlResult res = Kernel.Connection.ExecuteQuery(sel);
            if (res.RowCount > 0)
            {
                for(int i = 0 ; i < res.RowCount ; i++)
                    students.Add(res.GetInt64(i, 0, 0));
            }
            return students;
        }
        private decimal GetMedian(List<decimal> source)
        {
            // Create a copy of the input, and sort the copy
            decimal[] temp = source.ToArray();
            Array.Sort(temp);

            int count = temp.Length;
            if (count == 0)
            {
                return 0;
                //throw new InvalidOperationException("Empty collection");
            }
            else if (count % 2 == 0)
            {
                // count is even, average two middle elements
                decimal a = temp[count / 2 + 1];
                decimal b = temp[count / 2];
                return (a + b) / 2m;
            }
            else
            {
                // count is odd, return the middle element
                return temp[count / 2];
            }
        }

        private void UpdateStudentPerformanceGroup(int courseInstanceId, List<long> flagged_students)
        {
            try
            {
                SqlQueryUpdate up = new SqlQueryUpdate();
                up.Table("LogMetadataAgentInCourseInstance");
                up.Set("CalculatedPerformanceGroup", SqlValue.Convert("A"));
                up.Where("LogMetadataCourseInstanceId", SqlValue.Convert(courseInstanceId));
                SqlResult res = Kernel.Connection.ExecuteQuery(up);

                foreach (long logAgentId in flagged_students)
                {
                    up = new SqlQueryUpdate();
                    up.Table("LogMetadataAgentInCourseInstance");
                    up.Set("CalculatedPerformanceGroup", SqlValue.Convert("B"));
                    up.Where("LogAgentId", SqlValue.Convert(logAgentId));
                    up.Where("LogMetadataCourseInstanceId", SqlValue.Convert(courseInstanceId));
                    res = Kernel.Connection.ExecuteQuery(up);
                }
            }
            catch (Exception)
            {
            }
        }

        private static void CalculateStudentPerformanceGroup_HandleCriterion(List<Tuple<long, decimal>> student_session_count, decimal median_session_count, decimal lower_quartile_session_count, List<long> flagged_students, List<long> students_matching_at_least_one)
        {
            foreach (Tuple<long, decimal> record in student_session_count)
            {
                if (record.Item2 < median_session_count)
                {
                    if (students_matching_at_least_one.Contains(record.Item1) || record.Item2 < lower_quartile_session_count)
                    {
                        if(!flagged_students.Contains(record.Item1))
                            flagged_students.Add(record.Item1);
                    }
                    else
                        students_matching_at_least_one.Add(record.Item1);
                }
            }
        }
        /// <summary>
        /// The returned decimal for a logAgentId is -1 for students for which the last login was too long ago. 
        /// </summary>
        /// <param name="courseInstanceId"></param>
        /// <returns></returns>
        private List<Tuple<long, decimal>> ListStudentsByLoginLongTimeAgo(int courseInstanceId)
        {
            DateTime today = CalculateStudentPerformanceGroup_GetTodayOrEndDateForCourseInstance(courseInstanceId);

            string sql = @"select logstatement.logagentid, max(timestamp)
                from logstatement
                inner join LogActivity on LogActivity.LogActivityId = LogStatement.TargetLogActivityId
                inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId AND LogVerb.Uri = 'https://w3id.org/xapi/adl/verbs/logged-in'
                inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId AND LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = " + courseInstanceId + @"
                inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseInstanceId + @"
                where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < " + SqlValue.Convert(today) + @"
                group by logstatement.logagentid";

            SqlQueryCustom sel = new SqlQueryCustom(sql);

            Dictionary<long, DateTime> results = new Dictionary<long, DateTime>();
            List<Tuple<long, decimal>> matching_students = new List<Tuple<long, decimal>>();
            SqlResult res = Kernel.Connection.ExecuteQuery(sel);
            if (res.RowCount > 0)
            {
                for (int i = 0; i < res.RowCount; i++)
                {
                    long logAgentId = res.GetInt64(i, 0, 0);
                    DateTime dt = res.GetDateTime(i, 1, DateTime.MinValue);

                    TimeSpan timeSinceLastLogin = today.Subtract(dt);
                    if(timeSinceLastLogin.Days >= 14)
                        matching_students.Add(new Tuple<long, decimal>(logAgentId, -1));
                }

                    
            }
            return matching_students;
        }
        private List<Tuple<long, decimal>> ListStudentsByExerciseCountThreshold(int courseInstanceId)
        {
            DateTime today = CalculateStudentPerformanceGroup_GetTodayOrEndDateForCourseInstance(courseInstanceId);

            string sql = @"select logstatement.logagentid, count(*)
                from logstatement
                inner join LogActivity on LogActivity.LogActivityId = LogStatement.TargetLogActivityId
                inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId AND LogVerb.Uri = 'http://adlnet.gov/expapi/verbs/attempted'
                inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId AND LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = " + courseInstanceId + @"
                inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseInstanceId + @"
                inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id AND LogMetadataActivityInCourse.ActivityType in ('Exercise', 'Test exercise','Practice exercise')
                where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < " + SqlValue.Convert(today) + @"
                group by logstatement.logagentid";

            SqlQueryCustom sel = new SqlQueryCustom(sql);

            List<Tuple<long, decimal>> matching_students = new List<Tuple<long, decimal>>();
            SqlResult res = Kernel.Connection.ExecuteQuery(sel);
            if (res.RowCount > 0)
            {
                for (int i = 0; i < res.RowCount; i++)
                    matching_students.Add(new Tuple<long, decimal>(res.GetInt64(i, 0, 0), res.GetInt32(i, 1, 0)));
            }
            return matching_students;
        }
        private List<Tuple<long, decimal>> ListStudentsByAssessmentScore(int courseInstanceId)
        {
            DateTime today = CalculateStudentPerformanceGroup_GetTodayOrEndDateForCourseInstance(courseInstanceId);

            string sql = @"select LogStatement.LogAgentId, avg(LogScore.Scaled)
                from LogStatement
                inner join LogActivity on LogActivity.LogActivityId = LogStatement.TargetLogActivityId
                inner join LogActivityDefinition on LogActivityDefinition.LogActivityDefinitionId = LogActivity.LogActivityDefinitionId AND LogActivityDefinition.Type = 'http://adlnet.gov/expapi/activities/assessment'
                inner join LogResult on LogResult.LogResultId = LogStatement.LogResultId
                inner join LogScore on LogScore.LogScoreId = LogResult.LogScoreId
                inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId AND LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = " + courseInstanceId + @"
                inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseInstanceId = LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId
                where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < " + SqlValue.Convert(today) + @"
                group by LogStatement.LogAgentId";
            
            SqlQueryCustom sel = new SqlQueryCustom(sql);

            List<Tuple<long, decimal>> matching_students = new List<Tuple<long, decimal>>();
            SqlResult res = Kernel.Connection.ExecuteQuery(sel);
            if (res.RowCount > 0)
            {
                for (int i = 0; i < res.RowCount; i++)
                    matching_students.Add(new Tuple<long, decimal>(res.GetInt64(i, 0, 0), (decimal)res.GetFloat64(i, 1, 0)));
            }
            return matching_students;
        }
        private List<Tuple<long, decimal>> ListStudentsByTheoryPageCountThreshold(int courseInstanceId)
        {
            DateTime today = CalculateStudentPerformanceGroup_GetTodayOrEndDateForCourseInstance(courseInstanceId);

            string sql = @"select logstatement.logagentid, count(*)
                from logstatement
                inner join LogActivity on LogActivity.LogActivityId = LogStatement.TargetLogActivityId
                inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId AND LogVerb.Uri != 'https://w3id.org/xapi/adl/verbs/logged-out'
                inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId AND LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = " + courseInstanceId + @"
                inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseInstanceId + @"
                inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id AND LogMetadataActivityInCourse.ActivityType = 'TheoryPage'
                where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < " + SqlValue.Convert(today) + @"
                group by logstatement.logagentid
                ";
            
            SqlQueryCustom sel = new SqlQueryCustom(sql);

            List<Tuple<long, decimal>> matching_students = new List<Tuple<long, decimal>>();
            SqlResult res = Kernel.Connection.ExecuteQuery(sel);
            if (res.RowCount > 0)
            {
                for (int i = 0; i < res.RowCount; i++)
                    matching_students.Add(new Tuple<long, decimal>(res.GetInt64(i, 0, 0), res.GetInt32(i, 1, 0)));
            }
            return matching_students;
        }
        private List<Tuple<long, decimal>> ListStudentsBySessionCountThreshold(int courseInstanceId)
        {
            DateTime today = CalculateStudentPerformanceGroup_GetTodayOrEndDateForCourseInstance(courseInstanceId);

            string sql = @"select logstatement.logagentid, count(distinct Registration)
                from logstatement
                inner join logcontext on LogContext.LogContextId = LogStatement.LogContextId
                inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId AND LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = " + courseInstanceId /*   in (13,14) "  */ + @"
                inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseInstanceId = LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId
                where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < " + SqlValue.Convert(today) + @"
                group by logstatement.logagentid";
                //having count(distinct Registration) < " + upper_threshold;

            SqlQueryCustom sel = new SqlQueryCustom(sql);

            List<Tuple<long, decimal>> matching_students = new List<Tuple<long, decimal>>();
            SqlResult res = Kernel.Connection.ExecuteQuery(sel);
            if (res.RowCount > 0)
            {
                for(int i = 0 ; i < res.RowCount ; i++)
                    matching_students.Add(new Tuple<long, decimal>(res.GetInt64(i, 0, 0), res.GetInt32(i, 1, 0)));
            }
            return matching_students;
        }
        private bool GetWeeknumberProgress(int courseInstanceId, ref int current_week_number, ref int max_week_number)
        {
            DateTime now = DateTime.Now;

            string sql = @"select timeblock, (select max(timeblock) from LogMetadataCourseInstanceTimeBlock where LogMetadataCourseInstanceTimeBlock.LogMetadataCourseInstanceId = 13)
                from LogMetadataCourseInstanceTimeBlock where LogMetadataCourseInstanceTimeBlock.FromDate < " + SqlValue.Convert(now) + @" and LogMetadataCourseInstanceTimeBlock.UntilDate >= " + SqlValue.Convert(now) + @"
                and LogMetadataCourseInstanceTimeBlock.LogMetadataCourseInstanceId = " + courseInstanceId;

            SqlQueryCustom sel = new SqlQueryCustom(sql);

            SqlResult res = Kernel.Connection.ExecuteQuery(sel);
            if (res.RowCount > 0)
            {
                current_week_number = res.GetInt32(0, 0, 0);
                max_week_number = res.GetInt32(0, 1, 0);

                return (max_week_number > 0 && current_week_number <= max_week_number);
            }
            else
                return false;
        }
        private bool GetDayNumberProgress(int courseInstanceId, ref int current_day_number, ref int max_day_number)
        {
            DateTime now = DateTime.Now;

            //min(LogMetadataCourseInstanceTimeBlock.FromDate), , 
            string sql = @"select datediff(DAY, min(LogMetadataCourseInstanceTimeBlock.FromDate), max(LogMetadataCourseInstanceTimeBlock.UntilDate)) as 'current',
                datediff(DAY, min(LogMetadataCourseInstanceTimeBlock.FromDate), case when " + SqlValue.Convert(now) + @" > max(LogMetadataCourseInstanceTimeBlock.UntilDate) THEN max(LogMetadataCourseInstanceTimeBlock.UntilDate) ELSE " + SqlValue.Convert(now) + @" END) as 'max',
                max(LogMetadataCourseInstanceTimeBlock.UntilDate)
                from LogMetadataCourseInstanceTimeBlock
                where LogMetadataCourseInstanceTimeBlock.LogMetadataCourseInstanceId = " + courseInstanceId;

            SqlQueryCustom sel = new SqlQueryCustom(sql);

            SqlResult res = Kernel.Connection.ExecuteQuery(sel);
            if (res.RowCount > 0)
            {
                current_day_number = res.GetInt32(0, 0, 0);
                max_day_number = res.GetInt32(0, 1, 0);
                DateTime end_date = res.GetDateTime(0, 2, DateTime.MinValue);

                if(!m_CalculateStudentPerformanceGroup.ContainsKey(courseInstanceId))
                    m_CalculateStudentPerformanceGroup.Add(courseInstanceId, end_date);

                if (current_day_number > max_day_number)
                    current_day_number = max_day_number;

                return (max_day_number > 0);
            }
            else
                return false;
        }
        private DateTime CalculateStudentPerformanceGroup_GetTodayOrEndDateForCourseInstance(int courseInstanceId)
        {
            if (m_CalculateStudentPerformanceGroup.ContainsKey(courseInstanceId))
            {
                DateTime tmp = m_CalculateStudentPerformanceGroup[courseInstanceId];
                if (tmp < DateTime.Today)
                    return tmp;
                else
                    return DateTime.Today;
            }
            else
                return DateTime.Today;
        }
    }
}
