using LMG.Infrastructure.Analytics.Daemon.Objects.Types;
using LMG.Infrastructure.Analytics.Helpers;
using LMG.Infrastructure.Analytics.Objects;
using LMG.Infrastructure.Analytics.Objects.DB;
using LMG.Infrastructure.Analytics.Objects.DB.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LMG.Infrastructure.Analytics.Daemon.Objects.Helpers
{
    public static class MetadataImportHelper
    {
        public static void ImportMetadataFromKnownFilesToDatabase()
        {

            string folderName = "MetadataSource";
            string baseFolder = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\";
            List<string> errors = new List<string>();

            errors.Add(ImportMetadataFromFileToDatabase(baseFolder + "CourseInstance.csv", LogMetadataLoadType.CourseInstance));
            errors.Add(ImportMetadataFromFileToDatabase(baseFolder + "CourseInstanceTimeBlock.csv", LogMetadataLoadType.CourseInstanceTimeBlock));
            errors.Add(ImportMetadataFromFileToDatabase(baseFolder + "ActivityInCourse.csv", LogMetadataLoadType.ActivityInCourse));
            errors.Add(ImportMetadataFromFileToDatabase(baseFolder + "CourseInstanceClass.csv", LogMetadataLoadType.CourseInstanceClass));
            errors.Add(ImportMetadataFromFileToDatabase(baseFolder + "AgentInCourseInstance.csv", LogMetadataLoadType.AgentInCourseInstance));
            errors.Add(ImportMetadataFromFileToDatabase(baseFolder + "Agent.csv", LogMetadataLoadType.Agent));
            
            int extensionFileCount = 1;
            while (true)
            {
                string ext_file = baseFolder + "Extension" + (extensionFileCount == 1 ? "" : "-" + extensionFileCount) + ".csv";
                if (File.Exists(ext_file))
                {
                    errors.Add(ImportMetadataFromFileToDatabase(ext_file, LogMetadataLoadType.Extension));
                    extensionFileCount++;
                }
                else
                    break;
            }

            if (errors.Count > 0)
            {//There were some errors. Print them to file.
                File.WriteAllLines(baseFolder + "ImportLog (" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + ").log", errors);
            }
        }

        public static string ImportMetadataFromFileToDatabase(string metadataInputFile, LogMetadataLoadType type)
        {
            try
            {
                if (!File.Exists(metadataInputFile))
                    throw new Exception("Metadata file not found for type '"+type+"'. Expected file '"+metadataInputFile+"'.");

                CsvFileReader reader = new CsvFileReader(metadataInputFile);
                reader.Delimiter = ';';
                List<string> columns = new List<string>();
                List<string> values = new List<string>();
                bool headerProcessed = false;

                int row_index = 1;
                while (reader.ReadRow(values))
                {
                    if (!headerProcessed)
                    {
                        columns.AddRange(values);
                        headerProcessed = true;
                        continue;
                    }

                    string errorDetails = ImportMetadataRow(columns, values, type);
                    if(!string.IsNullOrWhiteSpace(errorDetails))
                    {
                        throw new Exception("Row " + row_index + ": Error: " + errorDetails);
                    }

                    row_index++;

                    #region Commented
                    //if(false)
                    //{
                    //    for (int i = 0; i < values.Count; i++)
                    //    {
                    //        if (string.IsNullOrWhiteSpace(values[i]))
                    //            continue;//No value in this column for this row

                    //        if (!columns[i].StartsWith("{") && !columns[i].StartsWith("["))
                    //            continue;//Skip this column. It's probably there for information/validation purposes.

                    //        if (columns[i].StartsWith("[") && columns[i].EndsWith("]"))
                    //        {
                    //            string column_name = columns[i].Substring(1, columns[i].Length - 2);
                    //            bool splitValues = column_name.StartsWith(",");//Split on ',' if the column name starts with ','
                    //            if (splitValues) column_name = column_name.Substring(1);//Cut away the separator ','



                    //            List<string> individual_values = new List<string>();
                    //            if (splitValues)
                    //                individual_values = new List<string>(values[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    //            else
                    //                individual_values.Add(values[i]);//Add the single value

                    //            foreach (string individual_value in individual_values)
                    //            {
                    //                if (string.IsNullOrWhiteSpace(individual_value))
                    //                    continue;//Skip empty values

                    //                try
                    //                {
                    //                    Kernel.TransactionBegin();

                    //                    LogMetadata metadata = new LogMetadata();
                    //                    metadata.LogMetadataTypeId = CacheHelper.GetOrCreateLogMetadataTypeId(column_name);
                    //                    metadata.Value = individual_value;
                    //                    metadata.OnInsert();

                    //                    if (type == LogMetadataLoadType.Agent)
                    //                    {
                    //                        LogAgentMetadata logagentmetadata = new LogAgentMetadata();
                    //                        logagentmetadata.LogAgentId = row_key_logagentid;
                    //                        logagentmetadata.LogMetadataId = metadata.LogMetadataId;
                    //                        logagentmetadata.OnInsert();
                    //                    }
                    //                    else
                    //                    {
                    //                        LogExtensionMetadata logExtensionMetadata = new LogExtensionMetadata();
                    //                        logExtensionMetadata.LogExtensionUri = key_logextensionuri;
                    //                        logExtensionMetadata.LogExtensionToken = row_key_logextension_token;
                    //                        logExtensionMetadata.LogMetadataId = metadata.LogMetadataId;
                    //                        logExtensionMetadata.OnInsert();
                    //                    }

                    //                    Kernel.TransactionCommit();
                    //                }
                    //                catch
                    //                {
                    //                    Kernel.TransactionRollback();
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                }
                return "" + metadataInputFile + " : OK (" + (row_index-1) + " rows)";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }

        private static string ImportMetadataRow(List<string> columns, List<string> values, LogMetadataLoadType type)
        {
            if(type != LogMetadataLoadType.Extension && type != LogMetadataLoadType.Agent)
                for (int i = 0; i < columns.Count; i++)//Set columns to lowercase for the other LoadTypes. We know which columns must be there, so lower case if easier to check.
                    columns[i] = columns[i].ToLower();

            string errorDetails = null;
            switch (type)
            {
                case LogMetadataLoadType.Agent:                     errorDetails = ImportMetadataRow_Agent(columns, values); break;
                case LogMetadataLoadType.Extension:                 errorDetails = ImportMetadataRow_Extension(columns, values); break;
                case LogMetadataLoadType.CourseInstance:            errorDetails = ImportMetadataRow_CourseInstance(columns, values); break;
                case LogMetadataLoadType.CourseInstanceTimeBlock:   errorDetails = ImportMetadataRow_CourseInstanceTimeBlock(columns, values); break;
                case LogMetadataLoadType.ActivityInCourse:          errorDetails = ImportMetadataRow_ActivityInCourse(columns, values); break;
                case LogMetadataLoadType.CourseInstanceClass:       errorDetails = ImportMetadataRow_CourseInstanceClass(columns, values); break;
                case LogMetadataLoadType.AgentInCourseInstance:     errorDetails = ImportMetadataRow_AgentInCourseInstance(columns, values); break;
                case LogMetadataLoadType.Unknown: break;
                default: break;
            }

            return errorDetails;
        }
        private static string ImportMetadataRow_Agent(List<string> columns, List<string> values)
        {
            try
            {
                Kernel.TransactionBegin();

                if(columns[0].ToLower() != "[agentid]")
                    throw new Exception("Invalid first columns: must be AgentId");

                string agentId_str = values[0];
                long agentId;
                if (!long.TryParse(agentId_str, out agentId))
                    throw new Exception("Invalid AgentId");

                for (int i = 1; i < columns.Count; i++)
                {//Loop through the rest of the columns (agentId must be at index 0)
                    if (string.IsNullOrWhiteSpace(values[i]))
                        continue;//No value in this column for this row

                    if (columns[i].StartsWith("[") && columns[i].EndsWith("]"))
                    {
                        string column_name = columns[i].Substring(1, columns[i].Length - 2);
                        bool splitValues = column_name.StartsWith(",");//Split on ',' if the column name starts with ','
                        if (splitValues) column_name = column_name.Substring(1);//Cut away the separator ','

                        LogMetadata metadata = new LogMetadata();
                        metadata.LogMetadataTypeId = CacheHelper.GetOrCreateLogMetadataTypeId(column_name);
                        metadata.Value = values[i];
                        metadata.OnInsert();

                        //if (type == LogMetadataLoadType.Agent)
                        {
                            LogAgentMetadata logagentmetadata = new LogAgentMetadata();
                            logagentmetadata.LogAgentId = agentId;
                            logagentmetadata.LogMetadataId = metadata.LogMetadataId;
                            logagentmetadata.OnInsert();
                        }
                        //else
                        {
                            //LogExtensionMetadata logExtensionMetadata = new LogExtensionMetadata();
                            //logExtensionMetadata.LogExtensionUri = key_logextensionuri;
                            //logExtensionMetadata.LogExtensionToken = row_key_logextension_token;
                            //logExtensionMetadata.LogMetadataId = metadata.LogMetadataId;
                            //logExtensionMetadata.OnInsert();
                        }
                    }
                }
                Kernel.TransactionCommit();
            }
            catch (Exception)
            {
                Kernel.TransactionRollback();
                throw;
            }
            return null;
        }
        private static string ImportMetadataRow_Extension(List<string> columns, List<string> values)
        {
            try
            {
                Kernel.TransactionBegin();

                #region Check for valid format in key column
                if (!columns[0].StartsWith("{") || !columns[0].EndsWith("}"))
                    throw new Exception("KeyHeader of first column is invalid. Must be in format {http(s)://...URI...}");//The key must be in the first column and must be indicated with {}
                #endregion

                string key_logextensionuri = columns[0].Substring(1, columns[0].Length - 2);
                string row_key_logextension_token = values[0];

                for (int i = 1; i < columns.Count; i++)
                {//Loop through the rest of the columns (agentId must be at index 0)
                    if (string.IsNullOrWhiteSpace(values[i]))
                        continue;//No value in this column for this row

                    

                    if (columns[i].StartsWith("[") && columns[i].EndsWith("]"))
                    {
                        string column_name = columns[i].Substring(1, columns[i].Length - 2);
                        bool splitValues = column_name.StartsWith(",");//Split on ',' if the column name starts with ','
                        if (splitValues) column_name = column_name.Substring(1);//Cut away the separator ','

                        LogMetadata metadata = new LogMetadata();
                        metadata.LogMetadataTypeId = CacheHelper.GetOrCreateLogMetadataTypeId(column_name);
                        metadata.Value = values[i];
                        metadata.OnInsert();

                        LogExtensionMetadata logExtensionMetadata = new LogExtensionMetadata();
                        logExtensionMetadata.LogExtensionUri = key_logextensionuri;
                        logExtensionMetadata.LogExtensionToken = row_key_logextension_token;
                        logExtensionMetadata.LogMetadataId = metadata.LogMetadataId;
                        logExtensionMetadata.OnInsert();
                    }
                }
                Kernel.TransactionCommit();
            }
            catch (Exception)
            {
                Kernel.TransactionRollback();
                throw;
            }
            return null;
        }
        private static string ImportMetadataRow_CourseInstance(List<string> columns, List<string> values)
        {
            try
            {
                Kernel.TransactionBegin();

                string importId = values[columns.IndexOf("[importid]")];
                string courseName = values[columns.IndexOf("[course]")];
                string academicYear = values[columns.IndexOf("[academicyear]")];
                string fromDate_str = values[columns.IndexOf("[fromdate]")];
                DateTime fromDate;
                if (!DateTime.TryParse(fromDate_str, out fromDate))
                    throw new Exception("Invalid FromDate");
                string untilDate_str = values[columns.IndexOf("[untildate]")];
                DateTime untilDate;
                if (!DateTime.TryParse(untilDate_str, out untilDate))
                    throw new Exception("Invalid UntilDate");

                LogMetadataCourseInstance obj = new LogMetadataCourseInstance();
                obj.ImportId = importId;
                obj.LogMetadataCourseId = GetCourseByName(courseName);
                obj.AcademicYear = academicYear;
                obj.FromDate = fromDate;
                obj.UntilDate = untilDate;
                ErrorHelper.ThrowErrorIfNotNull(obj.OnInsert());

                Kernel.TransactionCommit();
            }
            catch (Exception)
            {
                Kernel.TransactionRollback();
                throw;
            }
            return null;
        }
        private static string ImportMetadataRow_CourseInstanceTimeBlock(List<string> columns, List<string> values)
        {
            try
            {
                Kernel.TransactionBegin();

                string courseInstanceName = values[columns.IndexOf("[courseinstance]")];
                string timeBlock_str = values[columns.IndexOf("[timeblock]")];
                int timeBlock;
                if (!int.TryParse(timeBlock_str, out timeBlock))
                    throw new Exception("Invalid TimeBlock");
                string fromDate_str = values[columns.IndexOf("[fromdate]")];
                DateTime fromDate;
                if (!DateTime.TryParse(fromDate_str, out fromDate))
                    throw new Exception("Invalid FromDate"); ;
                string untilDate_str = values[columns.IndexOf("[untildate]")];
                DateTime untilDate;
                if (!DateTime.TryParse(untilDate_str, out untilDate))
                    throw new Exception("Invalid UntilDate");
                untilDate = untilDate.AddDays(1);//Interpret "Until" as "Until and including". So add 1 day to get to midnight of the intended day.

                LogMetadataCourseInstanceTimeBlock obj = new LogMetadataCourseInstanceTimeBlock();
                obj.LogMetadataCourseInstanceId = GetCourseInstanceByName(courseInstanceName);
                obj.TimeBlock = timeBlock;
                obj.FromDate = fromDate;
                obj.UntilDate = untilDate;
                ErrorHelper.ThrowErrorIfNotNull(obj.OnInsert());

                Kernel.TransactionCommit();
            }
            catch (Exception)
            {
                Kernel.TransactionRollback();
                throw;
            }
            return null;
        }
        private static string ImportMetadataRow_ActivityInCourse(List<string> columns, List<string> values)
        {
            try
            {
                Kernel.TransactionBegin();

                string logActivityUrl = values[columns.IndexOf("[logactivityurl]")];
                string name = values[columns.IndexOf("[name]")];
                string timeBlock_str = values[columns.IndexOf("[timeblock]")];
                string courseName = values[columns.IndexOf("[course]")];
                string chapter = values[columns.IndexOf("[chapter]")];
                string activityType = values[columns.IndexOf("[activitytype]")];
                string objectId = values[columns.IndexOf("[objectid]")];

                List<string> individual_timeblocks = new List<string>();
                if (timeBlock_str.Contains(','))
                    individual_timeblocks = new List<string>(timeBlock_str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                else
                    individual_timeblocks.Add(timeBlock_str);//Add the single value
                
                foreach (string timeBlock_record in individual_timeblocks)
                {
                    int? timeBlock = null;
                    int timeBlock_tmp;
                    if (int.TryParse(timeBlock_record, out timeBlock_tmp))
                        timeBlock = timeBlock_tmp;//throw new Exception("Invalid TimeBlock");

                    LogMetadataActivityInCourse obj = new LogMetadataActivityInCourse();
                    obj.LogActivityUrl = logActivityUrl;
                    obj.Name = name;
                    obj.TimeBlock = timeBlock;
                    obj.LogMetadataCourseId = GetCourseByName(courseName);
                    obj.Chapter = chapter;
                    obj.ActivityType = activityType;
                    obj.ObjectId = objectId;
                    ErrorHelper.ThrowErrorIfNotNull(obj.OnInsert());
                }

                Kernel.TransactionCommit();
            }
            catch (Exception)
            {
                Kernel.TransactionRollback();
                throw;
            }
            return null;
        }
        private static string ImportMetadataRow_CourseInstanceClass(List<string> columns, List<string> values)
        {
            try
            {
                Kernel.TransactionBegin();

                string courseInstanceName = values[columns.IndexOf("[courseinstance]")];
                string courseprogramme = values[columns.IndexOf("[courseprogramme]")];
                string group = values[columns.IndexOf("[group]")];
                string classtype = values[columns.IndexOf("[type]")];
                string teacherName = values[columns.IndexOf("[teacher]")];
                string fromDate_str = values[columns.IndexOf("[fromdate]")];
                DateTime fromDate;
                if (!DateTime.TryParse(fromDate_str, out fromDate))
                    throw new Exception("Invalid FromDate");
                string untilDate_str = values[columns.IndexOf("[untildate]")];
                DateTime untilDate;
                if (!DateTime.TryParse(untilDate_str, out untilDate))
                    throw new Exception("Invalid UntilDate");

                
                List<string> individual_courseprogrammes = new List<string>();
                if(!string.IsNullOrWhiteSpace(courseprogramme) && courseprogramme.Contains(","))
                    individual_courseprogrammes = new List<string>(courseprogramme.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                else
                    individual_courseprogrammes.Add(courseprogramme);//Add the single value

                List<string> individual_groups = new List<string>();
                if (!string.IsNullOrWhiteSpace(group) && group.Contains(","))
                    individual_groups = new List<string>(group.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                else
                    individual_groups.Add(group);//Add the single value

                foreach (string courseprogramme_record in individual_courseprogrammes)
                {
                    foreach (string group_record in individual_groups)
                    {
                        LogMetadataCourseInstanceClass obj = new LogMetadataCourseInstanceClass();
                        obj.LogMetadataCourseInstanceId = GetCourseInstanceByName(courseInstanceName);
                        obj.LogMetadataCourseProgrammeId = GetCourseProgrammeByName(courseprogramme_record);
                        obj.Group = (string.IsNullOrWhiteSpace(group_record) ? null : group_record);
                        obj.LogMetadataCourseInstanceClassTypeId = GetCourseInstanceClassTypeByName(classtype);
                        obj.LogMetadataTeacherId = GetTeacherByName(teacherName);
                        obj.FromDate = fromDate;
                        obj.UntilDate = untilDate;
                        ErrorHelper.ThrowErrorIfNotNull(obj.OnInsert());
                    }
                }

                Kernel.TransactionCommit();
            }
            catch (Exception)
            {
                Kernel.TransactionRollback();
                throw;
            }
            return null;
        }
        private static string ImportMetadataRow_AgentInCourseInstance(List<string> columns, List<string> values)
        {
            try
            {
                Kernel.TransactionBegin();

                string agentId_str = values[columns.IndexOf("[agentid]")];
                long agentId;
                if (!long.TryParse(agentId_str, out agentId))
                    throw new Exception("Invalid AgentId");
                string courseInstance = values[columns.IndexOf("[courseinstance]")];
                string courseprogramme = values[columns.IndexOf("[courseprogramme]")];
                string group = values[columns.IndexOf("[group]")];
                string score_str = values[columns.IndexOf("[score]")];
                string scoremax_str = values[columns.IndexOf("[scoremax]")];
                decimal? score = null;
                decimal tmp_score;
                if (!string.IsNullOrWhiteSpace(score_str))
                {
                    if (decimal.TryParse(score_str, out tmp_score))
                        score = tmp_score;
                    else
                        throw new Exception("Invalid Score");
                }
                decimal? scoremax = null;
                if (!string.IsNullOrWhiteSpace(scoremax_str))
                {
                    if (decimal.TryParse(scoremax_str, out tmp_score))
                        scoremax = tmp_score;
                    else
                        throw new Exception("Invalid ScoreMax");
                }
                LogMetadataAgentInCourseInstance obj = new LogMetadataAgentInCourseInstance();
                obj.LogAgentId = agentId;
                obj.LogMetadataCourseInstanceId = GetCourseInstanceByName(courseInstance);
                obj.LogMetadataCourseProgrammeId = GetCourseProgrammeByName(courseprogramme); ;
                obj.Group = (string.IsNullOrWhiteSpace(group) ? null : group);
                obj.Score = score;
                obj.ScoreMax = scoremax;
                ErrorHelper.ThrowErrorIfNotNull(obj.OnInsert());

                Kernel.TransactionCommit();
            }
            catch (Exception)
            {
                Kernel.TransactionRollback();
                throw;
            }
            return null;
        }

        #region Helpers
        #region Helpers to lookup Primary Keys for values of tables the import only has names for.
        private static Dictionary<string, long> m_courseNameToId = new Dictionary<string, long>();
        private static long GetCourseByName(string courseName)
        {
            try
            {
                string originalCourseName = courseName;
                courseName = courseName.ToLower();
                if (m_courseNameToId.ContainsKey(courseName))
                    return m_courseNameToId[courseName];

                long courseId = BaseObject.InvalidPrimaryKey;
                LogMetadataCourseCollection coll = new LogMetadataCourseCollection();
                coll.FillCollection("lower(name) = " + Kernel.MakeSqlSafe(courseName), "LogMetadataCourseId asc");
                if (coll.Count > 0)
                {
                    courseId = coll[0].LogMetadataCourseId;
                }
                else
                {//The course does not exist yet
                    LogMetadataCourse course = new LogMetadataCourse();
                    course.Name = originalCourseName;
                    course.OnInsert();
                    if (course.LogMetadataCourseId != BaseObject.InvalidPrimaryKey)
                        courseId = course.LogMetadataCourseId;
                    else
                        throw new Exception("Course '" + courseName + "' could not be created.");
                }

                m_courseNameToId.Add(courseName, courseId);
                return courseId;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private static Dictionary<string, long> m_courseInstanceNameToId = new Dictionary<string, long>();
        private static long GetCourseInstanceByName(string courseInstanceName)
        {
            try
            {
                courseInstanceName = courseInstanceName.ToLower();
                if (m_courseInstanceNameToId.ContainsKey(courseInstanceName))
                    return m_courseInstanceNameToId[courseInstanceName];

                long courseInstanceId = BaseObject.InvalidPrimaryKey;
                LogMetadataCourseInstanceCollection coll = new LogMetadataCourseInstanceCollection();
                coll.FillCollection("lower(importid) = " + Kernel.MakeSqlSafe(courseInstanceName), "LogMetadataCourseInstanceId asc");
                if (coll.Count > 0)
                {
                    courseInstanceId = coll[0].LogMetadataCourseInstanceId;
                }
                else
                {//The course does not exist
                    throw new Exception("Course Instance '" + courseInstanceName + "' does not exist.");
                }

                m_courseInstanceNameToId.Add(courseInstanceName, courseInstanceId);
                return courseInstanceId;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private static Dictionary<string, long> m_courseInstanceClassTypeNameToId = new Dictionary<string, long>();
        private static long GetCourseInstanceClassTypeByName(string classType)
        {
            try
            {
                string originalClassType = classType;
                classType = classType.ToLower();
                if (m_courseInstanceClassTypeNameToId.ContainsKey(classType))
                    return m_courseInstanceClassTypeNameToId[classType];

                long classTypeId = BaseObject.InvalidPrimaryKey;
                LogMetadataCourseInstanceClassTypeCollection coll = new LogMetadataCourseInstanceClassTypeCollection();
                coll.FillCollection("lower(name) = " + Kernel.MakeSqlSafe(classType), "LogMetadataCourseInstanceClassTypeId asc");
                if (coll.Count > 0)
                {
                    classTypeId = coll[0].LogMetadataCourseInstanceClassTypeId;
                }
                else
                {//The class type does not exist yet
                    LogMetadataCourseInstanceClassType type = new LogMetadataCourseInstanceClassType();
                    type.Name = originalClassType;
                    type.OnInsert();
                    if (type.LogMetadataCourseInstanceClassTypeId != BaseObject.InvalidPrimaryKey)
                        classTypeId = type.LogMetadataCourseInstanceClassTypeId;
                    else
                        throw new Exception("Course Class Type '" + classType + "' could not be created.");
                }

                m_courseInstanceClassTypeNameToId.Add(classType, classTypeId);
                return classTypeId;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private static Dictionary<string, long> m_courseProgrammeNameToId = new Dictionary<string, long>();
        private static long GetCourseProgrammeByName(string courseprogramme)
        {
            try
            {
                string originalCourseProgrammeName = courseprogramme;
                courseprogramme = courseprogramme.ToLower();
                if (m_courseProgrammeNameToId.ContainsKey(courseprogramme))
                    return m_courseProgrammeNameToId[courseprogramme];

                long courseProgrammeId = BaseObject.InvalidPrimaryKey;
                LogMetadataCourseProgrammeCollection coll = new LogMetadataCourseProgrammeCollection();
                coll.FillCollection("lower(name) = " + Kernel.MakeSqlSafe(courseprogramme), "LogMetadataCourseProgrammeId asc");
                if (coll.Count > 0)
                {
                    courseProgrammeId = coll[0].LogMetadataCourseProgrammeId;
                }
                else
                {//The class type does not exist yet
                    LogMetadataCourseProgramme programme = new LogMetadataCourseProgramme();
                    programme.Name = originalCourseProgrammeName;
                    programme.OnInsert();
                    if (programme.LogMetadataCourseProgrammeId != BaseObject.InvalidPrimaryKey)
                        courseProgrammeId = programme.LogMetadataCourseProgrammeId;
                    else
                        throw new Exception("Course Programme '" + courseprogramme + "' could not be created.");
                }

                m_courseProgrammeNameToId.Add(courseprogramme, courseProgrammeId);
                return courseProgrammeId;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private static Dictionary<string, long> m_teacherNameToId = new Dictionary<string, long>();
        private static long? GetTeacherByName(string teacherName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(teacherName))
                    return null;

                string originalTeacherName = teacherName;
                teacherName = teacherName.ToLower();
                if (m_teacherNameToId.ContainsKey(teacherName))
                    return m_teacherNameToId[teacherName];

                long teacherId = BaseObject.InvalidPrimaryKey;
                LogMetadataTeacherCollection coll = new LogMetadataTeacherCollection();
                coll.FillCollection("lower(name) = " + Kernel.MakeSqlSafe(teacherName), "LogMetadataTeacherId asc");
                if (coll.Count > 0)
                {
                    teacherId = coll[0].LogMetadataTeacherId;
                }
                else
                {//The teacher does not exist yet
                    LogMetadataTeacher teacher = new LogMetadataTeacher();
                    teacher.Name = originalTeacherName;
                    teacher.OnInsert();
                    if (teacher.LogMetadataTeacherId != BaseObject.InvalidPrimaryKey)
                        teacherId = teacher.LogMetadataTeacherId;
                    else
                        throw new Exception("Teacher '" + teacherName + "' could not be created.");
                }

                m_teacherNameToId.Add(teacherName, teacherId);
                return teacherId;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        #endregion
    }
}
