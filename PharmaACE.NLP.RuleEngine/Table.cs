using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.Framework
{
    public class Table
    {
        public string Name { get; set; }
        public List<Column> Columns { get; set; }
        public List<string> Equivalences { get; set; }

        public Table()
        {
            Columns = new List<Column>();
            Equivalences = new List<string>();
        }

        public Column GetColumnByName(string name)
        {
            return Columns.Where(col => String.Compare(col.Name, name, true) == 0).FirstOrDefault();
        }

        public bool IsEquivalent(string word)
        {
            return Equivalences.Contains(word);
        }

        public List<Column> GetPrimaryKeys()
        {
            List<Column> primaryKeys = new List<Column>();
            foreach (var column in Columns)
            {
                if (column.Primary)
                    primaryKeys.Add(column);
            }

            return primaryKeys;
        }

        public List<string> GetPrimaryKeyNames()
        {
            List<string> primaryKeyNames = new List<string>();
            foreach (var column in Columns)
            {
                if (column.Primary)
                    primaryKeyNames.Add(column.Name);
            }

            return primaryKeyNames;
        }

        public void AddPrimaryKey(string primaryKeyColumn)
        {
            foreach (var column in Columns)
            {
                if(String.Compare(column.Name, primaryKeyColumn, true) == 0)
                {
                    column.Primary = true;
                }
            }
        }

        public List<Column> GetForeignKeys()
        {
            List<Column> foreignKeys = new List<Column>();
            foreach (var column in Columns)
            {
                if (column.Foreign != null)
                    foreignKeys.Add(column);
            }

            return foreignKeys;
        }

        public List<string> GetForeignKeyNames()
        {
            List<string> foreignKeyNames = new List<string>();
            foreach (var column in Columns)
            {
                if (column.Foreign != null)
                    foreignKeyNames.Add(column.Name);
            }

            return foreignKeyNames;
        }

        public void AddForeignKey(string columnName, string foreignTable, string foreignColumn)
        {
            foreach (var column in Columns)
            {
                if(String.Compare(column.Name, columnName, true) == 0)
                {
                    column.Foreign = new Foreign { ForeignColumn = foreignColumn, ForeignTable = foreignTable };
                }
            }
        }
    }
}
