using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Infosciences.Sage
{
    [Serializable]
    public class acAction_Collection : SortableBindingList<acAction>
    {
        public acAction_Collection() { }

        public acAction_Collection(IList<acAction> items)
        {
            if (items == null) return;
            foreach (acAction item in items) Add(item);
        }

        public acAction_Collection(acAction[] items) : this((IList<acAction>)items) { }

        public acAction[] ToArray() { return this.AsEnumerable().ToArray(); }
        public List<acAction> ToList() { return this.AsEnumerable().ToList(); }

        public Dictionary<string, acAction> GetDictionary()
        {
            return this.ToDictionary(item => item.GetID().ToString(), item => item);
        }

        public object[] GetByNameArray(string propertyName)
        {
            var values = new object[Count + 1];
            for (int index = 0; index < Count; index++) values[index] = this[index].GetByName(propertyName);
            return values;
        }

        public void LoadData(DataTable table)
        {
            if (table == null) return;
            foreach (DataRow row in table.Rows)
            {
                var item = new acAction();
                    if (table.Columns.Contains("ActionKey") && row["ActionKey"] != DBNull.Value)
                        item.ActionKey = (string)Convert.ChangeType(row["ActionKey"], typeof(string));
                    if (table.Columns.Contains("ActionType") && row["ActionType"] != DBNull.Value)
                        item.ActionType = (string)Convert.ChangeType(row["ActionType"], typeof(string));
                    if (table.Columns.Contains("ActionPiece") && row["ActionPiece"] != DBNull.Value)
                        item.ActionPiece = (string)Convert.ChangeType(row["ActionPiece"], typeof(string));
                    if (table.Columns.Contains("ActionStatus") && row["ActionStatus"] != DBNull.Value)
                        item.ActionStatus = (bool)Convert.ChangeType(row["ActionStatus"], typeof(bool));
                    if (table.Columns.Contains("SessionID") && row["SessionID"] != DBNull.Value)
                        item.SessionID = (int)Convert.ChangeType(row["SessionID"], typeof(int));
                    if (table.Columns.Contains("ActionID") && row["ActionID"] != DBNull.Value)
                        item.ActionID = (int)Convert.ChangeType(row["ActionID"], typeof(int));
                    if (table.Columns.Contains("ActionRetVal") && row["ActionRetVal"] != DBNull.Value)
                        item.ActionRetVal = (int)Convert.ChangeType(row["ActionRetVal"], typeof(int));
                Add(item);
            }
        }

        public DataTable GetTable()
        {
            var table = new acAction_Table();
            foreach (acAction item in this)
            {
                DataRow row = table.NewRow();
                row["ActionKey"] = (object)item.ActionKey ?? DBNull.Value;
                row["ActionType"] = (object)item.ActionType ?? DBNull.Value;
                row["ActionPiece"] = (object)item.ActionPiece ?? DBNull.Value;
                row["ActionStatus"] = item.ActionStatus;
                row["SessionID"] = item.SessionID;
                row["ActionID"] = item.ActionID;
                row["ActionRetVal"] = item.ActionRetVal;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
