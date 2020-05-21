using System;
using System.Collections.Generic;
using System.Linq;
using PharmaACE.Utility;

namespace PharmaACE.NLP.Framework
{
    public class Query
    {
        public Select Select { get; set; }
        public From From { get; set; }
        public Join Join { get; set; }
        public Where Where { get; set; }
        public GroupBy GroupBy { get; set; }
        public OrderBy OrderBy { get; set; }

        public void PrintJson(string jsonOutputPath)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            //TODO: configure selectall as and when needed
            //Select.SelectAll = true;
            return "\n" + Select?.ToString() + From?.ToString() + Join?.ToString() + Where?.ToString() + GroupBy?.ToString() + OrderBy?.ToString() + ";\n";
        }
    }

    public class Select
    {
        public List<Tuple<string, List<string>>> Columns { get; set; }
        public bool SelectAll { get; set; }

        public Select()
        {
            Columns = new List<Tuple<string, List<string>>>();
        }

        string PrintColumn(Tuple<string, List<string>> selection)
        {
            var column = selection.Item1;
            var columnType = selection.Item2;
            if (String.IsNullOrEmpty(column))
            {
                if (columnType != null && columnType.Count > 0)
                    return "COUNT(*)";
                else
                    return "*";
            }
            else
            {
                if (columnType.Contains("DISTINCT"))
                {
                    if (columnType.Contains("COUNT"))
                        return String.Format("COUNT(DISTINCT {0})", column);
                    else
                        return String.Format("DISTINCT {0}", column);
                }
                if (columnType.Contains("COUNT"))
                    return String.Format("COUNT ({0})", column);
                else if (columnType.Contains("AVG"))
                    return String.Format("AVG({0})", column);
                else if (columnType.Contains("SUM"))
                    return String.Format("SUM({0})", column);
                else if (columnType.Contains("MAX"))
                    return String.Format("MAX({0})", column);
                else if (columnType.Contains("MIN"))
                    return String.Format("MIN({0})", column);
                else
                    return column;
            }
        }

        public override string ToString()
        {
            string selectString = String.Empty;
            if (this.SelectAll)
                selectString = "*";
            else
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    if (i == Columns.Count - 1)
                        selectString += PrintColumn(Columns[i]);
                    else
                        selectString += PrintColumn(Columns[i]) + ", ";
                }
            }

            return "SELECT " + selectString;
        }

        public void AddColumn(string column, List<string> columnType)
        {
            bool isColumnExisting = Columns.Any(c => String.Compare(c.Item1, column, true) == 0 && String.Compare(String.Join(" ", c.Item2), String.Join(" ", columnType), true) == 0);
            if (!isColumnExisting)
                Columns.Add(new Tuple<string, List<string>>(column, columnType));
        }

        string GetJustColumnName(string column)
        {
            if (!String.IsNullOrEmpty(column))
                return column.Split(new char[] { '.' }).Last();
            else
                return column;
        }
    }

    public class From
    {
        public string Table { get; set; }

        public override string ToString()
        {
            return String.Format("\nFrom {0}", Table);
        }
    }

    public class Join
    {
        public List<string> Tables { get; set; }
        public List<List<Tuple<string, string>>> Links { get; set; }

        public Join()
        {
            Tables = new List<string>();
            Links = new List<List<Tuple<string, string>>>();
        }

        void AddTable(string table)
        {
            if (!Tables.Contains(table))
                Tables.Add(table);
        }

        public override string ToString()
        {
            if(Links.Count > 0)
            {
                string str = String.Empty;
                for(int i = 0; i < Links.Count; i++)
                {
                    str += String.Format("\nINNER JOIN {0}\nON {1}.{2} = {0}.{3}", Links[i][0].Item1, Links[i][0].Item2, Links[i][1].Item1, Links[i][1].Item2);
                }
                return str;
            }
            else if(Tables.Count > 0)
            {
                if(Tables.Count == 1)
                {
                    return String.Format("\nNATURAL JOIN {1}", Tables[0]);
                }
                else
                {
                    string str = "\nNATURAL JOIN ";
                    for(int i = 0; i < Tables.Count; i++)
                    {
                        if (i == Tables.Count - 1)
                            str += Tables[i];
                        else
                            str += Tables[i] + ",";
                    }
                    return str;
                }
            }

            return null;
        }
    }

    public class Condition
    {
        public string Column { get; set; }
        public string ColumnType { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }

        public Condition()
        {
            Column = String.Empty;
            ColumnType = String.Empty;
            Operator = String.Empty;
            Value = String.Empty;
        }

        string GetColumnWithTypeOperation(string column, string columnType)
        {
            if (String.IsNullOrEmpty(ColumnType))
                return column;
            else
                return columnType + "(" + column + ")";
        }

        string GetPrettyOperator(string oprtr)
        {
            if (String.Compare(oprtr, "BETWEEN") == 0)
                return "BETWEEN OOV AND";
            else
                return oprtr;
        }

        public override string ToString()
        {
            //replace phrase linker with space and put single quote
            return GetColumnWithTypeOperation(Column, ColumnType) + " " + GetPrettyOperator(Operator) + " " 
                + String.Format("'{0}'", Value.Replace(Util.PHRASE_MAKER, " "));
        }
    }

    public class Where
    {
        public List<Tuple<string, Condition>> Conditions { get; set; }

        public Where()
        {
            Conditions = new List<Tuple<string, Condition>>();
        }

        public override string ToString()
        {
            string str = String.Empty;

            if(Conditions != null && Conditions.Count > 0)
            {
                for(int i = 0; i < Conditions.Count; i++)
                {
                    if (i == 0)
                        str += String.Format("{0}", Conditions[i].Item2.ToString());
                    else
                        str += String.Format("\n{0} {1}", Conditions[i].Item1.ToString(), Conditions[i].Item2.ToString());
                }
            }

            //TODO: can we make the following logic better? theis thing currently will not be extendible in future when there are many mix and match 
            //of difference clauses, =, !=, join, orderby, like, in... etc..
            string negationPart = null;
            var conditionalSplit = str.Split(new string[] { "\nOR", " OR " }, StringSplitOptions.RemoveEmptyEntries);
            string lastItem = conditionalSplit[conditionalSplit.Length - 1];
            var notSplit = lastItem.Split(new string[] { "!=" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (notSplit.Count > 1)
            {
                var andSplitWithoutNot = notSplit[0].Split(new string[] { "AND" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                notSplit.RemoveAt(0);
                string andSplitWithoutNotLastitem = andSplitWithoutNot.Last();
                andSplitWithoutNot.RemoveAt(andSplitWithoutNot.Count - 1);
                conditionalSplit[conditionalSplit.Length - 1] = String.Join("AND", andSplitWithoutNot);
                negationPart = andSplitWithoutNotLastitem + "!=" + String.Join("!=", notSplit);
            }
            if(string.IsNullOrWhiteSpace(negationPart))
                str = String.Format("\nWHERE ({0})", String.Join(" OR ", conditionalSplit));
            else
                str = String.Format("\nWHERE ({0}) AND ({1})", String.Join(" OR ", conditionalSplit), negationPart);

            return str;
        }
    }

    public class GroupBy
    {
        public override string ToString()
        {
            return "";
        }
    }

    public class OrderBy
    {
        public override string ToString()
        {
            return "";
        }
    }

}
