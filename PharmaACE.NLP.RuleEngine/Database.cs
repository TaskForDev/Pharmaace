using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PharmaACE.Utility;

namespace PharmaACE.NLP.Framework
{
    public class Database
    {
        public List<Table> Tables { get; set; }
        public Thesaurus Thesaurus { get; set; }

        public Database()
        {
            Tables = new List<Table>();
        }

        public bool IsSingleTable { get { return Tables.Count == 1; } }

        public Column GetColumnByName(string name)
        {
            foreach (var table in Tables)
            {
                foreach (var column in table.Columns)
                {
                    if (String.Compare(column.Name, name, true) == 0)
                        return column;
                }
            }

            return null;
        }

        public Column GetColumnByDisplayName(string displayName)
        {
            foreach (var table in Tables)
            {
                foreach (var column in table.Columns)
                {
                    if (String.Compare(column.DisplayName, displayName, true) == 0)
                        return column;
                }
            }

            return null;
        }

        public Table GetTableByName(string name)
        {
            foreach (var table in Tables)
            {
                if(String.Compare(table.Name, name, true) == 0)
                {
                    return table;
                }
            }

            return null;
        }

        public Dictionary<string, List<string>> GetTablesIntoDictionary()
        {
            var data = new Dictionary<string, List<string>>();
            foreach (var table in Tables)
            {
                data.Add(table.Name, table.Columns.Select(col => col.Name).ToList());
            }

            return data;
        }

        public Dictionary<string, List<Column>> GetPrimaryKeysByTable()
        {
            var data = new Dictionary<string, List<Column>>();
            foreach (var table in Tables)
            {
                data.Add(table.Name, table.GetPrimaryKeys());
            }

            return data;
        }

        public Dictionary<string, List<Column>> GetForeignKeysByTable()
        {
            var data = new Dictionary<string, List<Column>>();
            foreach (var table in Tables)
            {
                data.Add(table.Name, table.GetForeignKeys());
            }

            return data;
        }

        public List<Column> GetForeignKeysOfTable(string tableName)
        {
            foreach (var table in Tables)
            {
                if (String.Compare(table.Name, tableName, true) == 0)
                    return table.GetForeignKeys();
            }

            return null;
        }


        static string GeneratePath(string path)
        {
            //get the full location of the assembly with DaoTests in it
            string fullPath = System.Reflection.Assembly.GetAssembly(typeof(Thesaurus)).Location;

            //get the folder that's in
            string theDirectory = Path.GetDirectoryName(fullPath);

            string fileName = Path.Combine(theDirectory, path);
            return fileName;
        }

        public void Load(string path)
        {
            var content = File.ReadAllText(GeneratePath(path));
            List<string> tableStrings = new List<string>();
            foreach (var p in content.Split(new string[] { "CREATE" }, StringSplitOptions.None))
            {
                if (p.Contains(";"))
                    tableStrings.Add(p.Split(new char[] { ';' })[0]);
            }
            foreach (var tableString in tableStrings)
            {
                if (tableString.Contains("TABLE"))
                {
                    Table table = CreateTable(tableString);
                    Tables.Add(table);
                }
            }

            List<string> alterTableStrings = new List<string>();
            foreach (var p in content.Split(new string[] { "ALTER" }, StringSplitOptions.None))
            {
                if (p.Contains(";"))
                    tableStrings.Add(p.Split(new char[] { ';' })[0]);
            }
            foreach (var tableString in tableStrings)
            {
                if (tableString.Contains("TABLE"))
                {
                    AlterTable(tableString);
                }
            }

            //TO DO : matchcandidates should be loaded eagerly or lazily? should be configurable. currently it's always eager
            PopulateMatchCandidates();
        }

        private void PopulateMatchCandidates()
        {
            foreach (var table in Tables)
            {
                string query = String.Empty;
                foreach (var col in table.Columns)
                {
                    if (col.Types.Contains("string"))
                        query += String.Format("Select distinct {0}, LEN({0}) from {1} order by LEN({0}) desc\n", col.Name, table.Name);
                    //col.MatchCandidates.AddOrUpdate(Thesaurus.Dictionary[col.Name]); //number, datetime etc.
                }
                if (!String.IsNullOrWhiteSpace(query))
                {
                    string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ToString();
                    try
                    {
                        using (DataSet ds = new DataSet())
                        {
                            using (SqlConnection conn = new SqlConnection(conStr))
                            {
                                using (SqlCommand cmd = new SqlCommand(query, conn))
                                {
                                    cmd.CommandType = CommandType.Text;
                                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                                    {
                                        adp.Fill(ds);
                                        foreach (DataTable dt in ds.Tables)
                                        {
                                            var columnName = dt.Columns[0].ColumnName;
                                            Column column = table.GetColumnByName(columnName);
                                            if (column != null)
                                            {
                                                foreach (DataRow row in dt.Rows)
                                                {
                                                    string match = row[columnName].SafeTrim();
                                                    if (!String.IsNullOrWhiteSpace(match) && !column.MatchCandidates.Contains(match))
                                                        column.MatchCandidates.Add(match);
                                                }
                                            }
                                        }
                                    }
                                }
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

        string PredictType(string str)
        {
            string lowerCaseStr = str.ToLower();
            if (lowerCaseStr.Contains("int"))
                return "int";
            else if (lowerCaseStr.Contains("char") || lowerCaseStr.Contains("text"))
                return "string";
            else if (lowerCaseStr.Contains("date"))
                return "date";
            else
                return "unknown";
        }

        private Table CreateTable(string tableString)
        {
            var lines = tableString.Split(new char[] { '\n' });
            var table = new Table();

            foreach (var line in lines)
            {
                if (line.Contains("TABLE"))
                {
                    var tableNameMatch = Regex.Match(line, @"`(\w+)`");
                    table.Name = tableNameMatch.Groups[1].Value;
                    if (Thesaurus != null && Thesaurus.Dictionary.ContainsKey(table.Name))
                        table.Equivalences = Thesaurus.Dictionary[table.Name];
                }
                else if (line.Contains("PRIMARY KEY"))
                {
                    var primaryKeyColumns = Regex.Matches(line, @"`(\w+)`");
                    foreach (Match primaryKeyColumn in primaryKeyColumns)
                        table.AddPrimaryKey(primaryKeyColumn.Value);
                }
                else
                {
                    var columnNameMatch = Regex.Match(line, @"`(\w+)`");
                    if (columnNameMatch != null)
                    {
                        string columnType = PredictType(line);
                        List<string> equivalences = null;
                        string columnName = columnNameMatch.Groups[1].Value;
                        if (!String.IsNullOrWhiteSpace(columnName))
                        {
                            if (Thesaurus != null)
                            {
                                if (Thesaurus != null && Thesaurus.Dictionary.ContainsKey(columnName))
                                    equivalences = Thesaurus.Dictionary[columnName];
                                else
                                    equivalences = new List<string>();
                            }
                            table.Columns.Add(new Column { Name = columnName, Types = new List<string> { columnType },
                                MatchCandidates = new HashSet<string>(), Equivalences = equivalences });
                        }
                    }
                }
            }

            return table;
        }

        private void AlterTable(string alterString)
        {
            var lines = alterString.Replace('\n', ' ').Split(new char[] { ';' });
            foreach (var line in lines)
            {
                if (line.Contains("PRIMARY KEY"))
                {
                    string tableName = Regex.Match(line, @"TABLE `(\w+)`").Groups[1].Value;
                    var table = GetTableByName(tableName);
                    var primaryKeyColumnMatches = Regex.Matches(line, @"PRIMARY KEY \(`(\w+)`\)");
                    foreach (Match primaryKeyColumnMatch in primaryKeyColumnMatches)
                    {
                        table.AddPrimaryKey(primaryKeyColumnMatch.Value);
                    }
                }
                if (line.Contains("FOREIGN KEY"))
                {
                    var tableName = Regex.Match(line, @"TABLE `(\w+)`").Groups[1].Value;
                    var table = GetTableByName(tableName);
                    var foreignKeyMatches = Regex.Matches(line, @"FOREIGN KEY \(`(\w+)`\) REFERENCES `(\w+)` \(`(\w+)`\)");
                    foreach (Match foreignKeyMatch in foreignKeyMatches)
                    {
                        string column = foreignKeyMatch.Groups[0].Value;
                        string foreignTable = foreignKeyMatch.Groups[1].Value;
                        string foreignColumn = foreignKeyMatch.Groups[2].Value;
                        table.AddForeignKey(column, foreignTable, foreignColumn);
                    }
                }
            }
        }
    }
}
