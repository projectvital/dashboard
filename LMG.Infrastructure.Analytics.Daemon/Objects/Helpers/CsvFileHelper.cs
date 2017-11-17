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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LMG.Infrastructure.Analytics.Daemon.Objects.Helpers
{
    public static class CsvFileHelper
    {

        public static void ClearDuplicatesFromCsv()
        {
            try
            {

                string folderName = "ConvertSource";
                string localJsonFolder = AppDomain.CurrentDomain.BaseDirectory + folderName;
                string csvInputFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\output.csv";
                string csvOutputFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\output-clean.csv";
                //string csvColumnFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\output-columns.conf";
                DirectoryInfo dir = new DirectoryInfo(localJsonFolder);
                if (!dir.Exists)
                    return;

                CsvFileWriter csvWriter = new CsvFileWriter(csvOutputFile);
                csvWriter.Delimiter = ';';

                List<string> columns = new List<string>();
                //if (File.Exists(csvColumnFile))
                //{
                //    columns = new List<string>(File.ReadAllLines(csvColumnFile));
                //}

                List<Guid> processedStatementIds = new List<Guid>();
                CsvFileReader reader = new CsvFileReader(csvInputFile);
                reader.Delimiter = ';';
                List<string> values = new List<string>();
                bool headerProcessed = false;
                while (reader.ReadRow(values))
                {
                    if (!headerProcessed)
                    {
                        columns.AddRange(values);
                        csvWriter.WriteRow(columns);
                        headerProcessed = true;
                        continue;
                    }

                    Guid statementId;
                    if (Guid.TryParse(values[columns.IndexOf("id")], out statementId))
                    {
                        if (!processedStatementIds.Contains(statementId))
                        {
                            csvWriter.WriteRow(values);

                            processedStatementIds.Add(statementId);
                        }
                        else
                        {
                            //Duplicate was found and skipped!
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void RemoveLogAgentFromCsv(string logagentid)
        {
            try
            {

                string folderName = "ConvertSource";
                string localJsonFolder = AppDomain.CurrentDomain.BaseDirectory + folderName;
                string csvInputFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\output.csv";
                string csvOutputFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\output-clean.csv";

                DirectoryInfo dir = new DirectoryInfo(localJsonFolder);
                if (!dir.Exists)
                    return;

                CsvFileWriter csvWriter = new CsvFileWriter(csvOutputFile);
                csvWriter.Delimiter = ';';

                List<string> columns = new List<string>();
                List<Guid> processedStatementIds = new List<Guid>();
                CsvFileReader reader = new CsvFileReader(csvInputFile);
                reader.Delimiter = ';';
                List<string> values = new List<string>();
                bool headerProcessed = false;
                while (reader.ReadRow(values))
                {
                    if (!headerProcessed)
                    {
                        columns.AddRange(values);
                        csvWriter.WriteRow(columns);
                        headerProcessed = true;
                        continue;
                    }

                    if (values[columns.IndexOf("actor.name")] != logagentid)
                    {
                        csvWriter.WriteRow(values);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void RemoveLearningModulesFromCsv(List<string> learningModuleIds)
        {
            try
            {

                string folderName = "ConvertSource";
                string localJsonFolder = AppDomain.CurrentDomain.BaseDirectory + folderName;
                string csvInputFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\output.csv";
                string csvOutputFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\output-clean.csv";

                DirectoryInfo dir = new DirectoryInfo(localJsonFolder);
                if (!dir.Exists)
                    return;

                CsvFileWriter csvWriter = new CsvFileWriter(csvOutputFile);
                csvWriter.Delimiter = ';';

                List<string> columns = new List<string>();
                List<Guid> processedStatementIds = new List<Guid>();
                CsvFileReader reader = new CsvFileReader(csvInputFile);
                reader.Delimiter = ';';
                List<string> values = new List<string>();
                bool headerProcessed = false;
                while (reader.ReadRow(values))
                {
                    if (!headerProcessed)
                    {
                        columns.AddRange(values);
                        csvWriter.WriteRow(columns);
                        headerProcessed = true;
                        continue;
                    }

                    string learningModuleId = values[columns.IndexOf("context.extensions['http://www.project-vital.eu/xapi/extension/link-LearningModule']")];
                    if (!learningModuleIds.Contains(learningModuleId))
                    {
                        csvWriter.WriteRow(values);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void SendCsvDataToTemporaryTable()
        {
            try
            {
                string folderName = "ConvertSource";
                string csvInputFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\input.csv";

                AnalyticsWorker worker = new AnalyticsWorker();

                CsvFileReader reader = new CsvFileReader(csvInputFile);
                List<string> values = new List<string>();
                while (reader.ReadRow(values))
                {
                    Guid dummy;
                    if (values.Count == 1 && Guid.TryParse(values[0], out dummy))
                    {
                        string q = "insert into LogStatement_ExportComparison Values ('" + values[0] + "')";
                        worker.ExecuteQuery(q);

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static void GetDistinctLogAgentIdsFromCsv()
        {
            try
            {

                string folderName = "ConvertSource";
                string localJsonFolder = AppDomain.CurrentDomain.BaseDirectory + folderName;
                string csvInputFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\output.csv";
                string csvOutputFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\output-logagents.csv";
                //string csvColumnFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\output-columns.conf";
                DirectoryInfo dir = new DirectoryInfo(localJsonFolder);
                if (!dir.Exists)
                    return;

                CsvFileWriter csvWriter = new CsvFileWriter(csvOutputFile);
                csvWriter.Delimiter = ';';

                List<string> columns = new List<string>();
                //if (File.Exists(csvColumnFile))
                //{
                //    columns = new List<string>(File.ReadAllLines(csvColumnFile));
                //}

                List<string> foundItems = new List<string>();
                CsvFileReader reader = new CsvFileReader(csvInputFile);
                reader.Delimiter = ';';
                List<string> values = new List<string>();
                bool headerProcessed = false;
                while (reader.ReadRow(values))
                {
                    if (!headerProcessed)
                    {
                        columns.AddRange(values);
                        //csvWriter.WriteRow(columns);
                        headerProcessed = true;
                        continue;
                    }

                    string logagentId = values[columns.IndexOf("actor.name")];
                    if(!foundItems.Contains(logagentId))
                    {
                        foundItems.Add(logagentId);
                        values = new List<string>();
                        values.Add(logagentId);
                        csvWriter.WriteRow(values);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string csv_link_exercise_key = "context.extensions['http://www.project-vital.eu/xapi/extension/link-Exercise']";
        private static string csv_link_theorypage_key = "context.extensions['http://www.project-vital.eu/xapi/extension/link-TheoryPage']";
        private static string csv_object_id_key = "object.id";
        private static int csv_link_exercise_index = -1;
        private static int csv_link_theorypage_index = -1;
        private static int csv_object_id_index = -1;
        public static void SupplementCsvData(List<string> columns, List<string> values)
        {
            if (csv_object_id_index == -1)
            {
                csv_object_id_index = columns.IndexOf(csv_object_id_key);
                csv_link_exercise_index = columns.IndexOf(csv_link_exercise_key);
                csv_link_theorypage_index = columns.IndexOf(csv_link_theorypage_key);

                if (csv_object_id_index == -1 || csv_link_exercise_index == -1 || csv_link_theorypage_index == -1)
                    return;
            }

            string id = values[csv_object_id_index];
            //id format == http://student.commart.eu/Content/Show/1381/Exercise/21084?theorypageContext=3761
            List<string> parts = new List<string>(id.Split(new string[] { "Content/Show/1381/Exercise/", "?theorypageContext=" }, StringSplitOptions.RemoveEmptyEntries));
            //Ignore parts[0]. It's the prefix of the website: http://student.commart.eu/
            if (parts.Count != 3)//Not a statement that needs this fix!
                return;

            string exerciseId = parts[1];
            string theoryPageId = parts[2];
            if (theoryPageId.Contains("#"))
                theoryPageId = theoryPageId.Substring(0, theoryPageId.IndexOf("#"));

            values[csv_link_theorypage_index] = theoryPageId;
        }


        internal static void FilterLogAgentsFromCsv()
        {
            List<string> agents = new List<string>();
            agents.Add("50191");
            agents.Add("49231");
            agents.Add("28441");
            agents.Add("49351");
            agents.Add("28081");
            agents.Add("48811");
            agents.Add("34051");
            agents.Add("48871");
            agents.Add("57511");
            agents.Add("50071");
            agents.Add("48961");
            agents.Add("49321");
            agents.Add("16981");
            agents.Add("27691");
            agents.Add("28531");
            agents.Add("48991");
            agents.Add("50341");
            agents.Add("52111");
            agents.Add("74581");
            agents.Add("49981");
            agents.Add("50791");
            agents.Add("27961");
            agents.Add("49471");
            agents.Add("39211");
            agents.Add("49771");
            agents.Add("52171");
            agents.Add("48931");
            agents.Add("49261");
            agents.Add("49711");
            agents.Add("27871");
            agents.Add("26311");
            agents.Add("49141");
            agents.Add("49921");
            agents.Add("49531");
            agents.Add("48721");
            agents.Add("50131");
            agents.Add("26281");
            agents.Add("48781");
            agents.Add("28651");
            agents.Add("49801");
            agents.Add("48841");
            agents.Add("50251");
            agents.Add("25891");
            agents.Add("28351");
            agents.Add("49201");
            agents.Add("49051");
            agents.Add("49621");
            agents.Add("50011");
            agents.Add("50221");
            agents.Add("27271");
            agents.Add("49741");
            agents.Add("49081");
            agents.Add("49381");
            agents.Add("66991");
            agents.Add("30511");
            agents.Add("48751");
            agents.Add("49861");
            agents.Add("49951");
            agents.Add("49021");
            agents.Add("50761");
            agents.Add("28831");
            agents.Add("48901");
            agents.Add("27001");
            agents.Add("49291");
            agents.Add("50371");
            agents.Add("49441");
            agents.Add("51001");
            agents.Add("49831");
            agents.Add("28591");
            agents.Add("50161");
            agents.Add("27601");
            agents.Add("30601");
            agents.Add("28681");
            agents.Add("49411");
            agents.Add("50431");
            agents.Add("50851");
            agents.Add("28411");
            agents.Add("34201");
            agents.Add("33781");
            agents.Add("34891");
            agents.Add("32701");
            agents.Add("34171");
            agents.Add("39031");
            agents.Add("22651");
            agents.Add("20851");
            agents.Add("34771");
            agents.Add("39151");
            agents.Add("34531");
            agents.Add("39391");
            agents.Add("34741");
            agents.Add("34681");
            agents.Add("34321");
            agents.Add("34921");
            agents.Add("33751");
            agents.Add("33871");
            agents.Add("33811");
            agents.Add("18901");
            agents.Add("35101");
            agents.Add("39991");
            agents.Add("34621");
            agents.Add("35041");
            agents.Add("32671");
            agents.Add("34591");
            agents.Add("33901");
            agents.Add("40291");
            agents.Add("34711");
            agents.Add("35131");
            agents.Add("22861");
            agents.Add("34561");
            agents.Add("39931");
            agents.Add("33841");
            agents.Add("39061");
            agents.Add("17161");
            agents.Add("38881");
            agents.Add("35071");
            agents.Add("39121");
            agents.Add("17431");
            agents.Add("39841");
            agents.Add("33961");
            agents.Add("35011");
            agents.Add("34291");
            agents.Add("39781");
            agents.Add("19261");
            agents.Add("34231");
            agents.Add("33931");
            agents.Add("33601");
            agents.Add("73321");
            agents.Add("33991");
            agents.Add("39181");
            agents.Add("34471");
            agents.Add("33721");
            agents.Add("34651");
            agents.Add("15601");
            agents.Add("39001");
            agents.Add("39661");
            agents.Add("32731");
            agents.Add("33571");
            agents.Add("34261");
            agents.Add("23041");
            agents.Add("23071");
            agents.Add("39421");
            agents.Add("34081");
            agents.Add("34351");
            agents.Add("39751");
            agents.Add("39091");
            agents.Add("34141");
            agents.Add("34411");
            agents.Add("38911");
            agents.Add("34441");

            FilterLogAgentsFromCsv(agents);
        }
        public static void FilterLogAgentsFromCsv(List<string> wantedAgents)
        {
            try
            {
                string folderName = "ConvertSource";
                string localJsonFolder = AppDomain.CurrentDomain.BaseDirectory + folderName;
                string csvInputFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\output.csv";
                string csvOutputFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\output-filteredAgents.csv";

                DirectoryInfo dir = new DirectoryInfo(localJsonFolder);
                if (!dir.Exists)
                    return;

                CsvFileWriter csvWriter = new CsvFileWriter(csvOutputFile);
                csvWriter.Delimiter = ';';

                List<string> columns = new List<string>();
                List<Guid> processedStatementIds = new List<Guid>();
                CsvFileReader reader = new CsvFileReader(csvInputFile);
                reader.Delimiter = ';';
                List<string> values = new List<string>();
                bool headerProcessed = false;
                while (reader.ReadRow(values))
                {
                    if (!headerProcessed)
                    {
                        columns.AddRange(values);
                        csvWriter.WriteRow(columns);
                        headerProcessed = true;
                        continue;
                    }

                    if (wantedAgents.Contains(values[columns.IndexOf("actor.name")]))
                    {
                        csvWriter.WriteRow(values);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal static void RemoveGramVocRecords()
        {
            try
            {
                string folderName = "ConvertSource";
                string localJsonFolder = AppDomain.CurrentDomain.BaseDirectory + folderName;
                string csvInputFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\output.csv";
                string csvOutputFile = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\output-filtered-gramvoc.csv";

                DirectoryInfo dir = new DirectoryInfo(localJsonFolder);
                if (!dir.Exists)
                    return;

                CsvFileWriter csvWriter = new CsvFileWriter(csvOutputFile);
                csvWriter.Delimiter = ';';

                List<string> columns = new List<string>();
                List<Guid> processedStatementIds = new List<Guid>();
                CsvFileReader reader = new CsvFileReader(csvInputFile);
                reader.Delimiter = ';';
                List<string> values = new List<string>();
                bool headerProcessed = false;
                while (reader.ReadRow(values))
                {
                    if (!headerProcessed)
                    {
                        columns.AddRange(values);
                        csvWriter.WriteRow(columns);
                        headerProcessed = true;
                        continue;
                    }

                    bool recordIsGramVoc = false;
                    for (int i = 0 ; i < columns.Count ; i++)
                    {
                        string column = columns[i];
                        if (column.StartsWith("object.definition.name."))
                        {
                            if (values[i].StartsWith("GR_") || values[i].StartsWith("VOC_"))
                            {
                                recordIsGramVoc = true;
                                break;
                            }
                        }
                    }
                    if(!recordIsGramVoc)
                        csvWriter.WriteRow(values);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
