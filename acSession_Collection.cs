using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Infosciences.Sage
{
    [Serializable]
    public class acSession_Collection : SortableBindingList<acSession>
    {
        public acSession_Collection() { }

        public acSession_Collection(IList<acSession> items)
        {
            if (items == null) return;
            foreach (acSession item in items) Add(item);
        }

        public acSession_Collection(acSession[] items) : this((IList<acSession>)items) { }

        public acSession[] ToArray() { return this.AsEnumerable().ToArray(); }
        public List<acSession> ToList() { return this.AsEnumerable().ToList(); }

        public Dictionary<string, acSession> GetDictionary()
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
                var item = new acSession();
                    if (table.Columns.Contains("SessionKey") && row["SessionKey"] != DBNull.Value)
                        item.SessionKey = (string)Convert.ChangeType(row["SessionKey"], typeof(string));
                    if (table.Columns.Contains("SessionID") && row["SessionID"] != DBNull.Value)
                        item.SessionID = (int)Convert.ChangeType(row["SessionID"], typeof(int));
                    if (table.Columns.Contains("StartTime") && row["StartTime"] != DBNull.Value)
                        item.StartTime = (DateTime)Convert.ChangeType(row["StartTime"], typeof(DateTime));
                    if (table.Columns.Contains("SessionUser") && row["SessionUser"] != DBNull.Value)
                        item.SessionUser = (string)Convert.ChangeType(row["SessionUser"], typeof(string));
                    if (table.Columns.Contains("SessionMachine") && row["SessionMachine"] != DBNull.Value)
                        item.SessionMachine = (string)Convert.ChangeType(row["SessionMachine"], typeof(string));
                    if (table.Columns.Contains("SessionClientMachine") && row["SessionClientMachine"] != DBNull.Value)
                        item.SessionClientMachine = (string)Convert.ChangeType(row["SessionClientMachine"], typeof(string));
                Add(item);
            }
        }

        public DataTable GetTable()
        {
            var table = new acSession_Table();
            foreach (acSession item in this)
            {
                DataRow row = table.NewRow();
                row["SessionKey"] = (object)item.SessionKey ?? DBNull.Value;
                row["SessionID"] = item.SessionID;
                row["StartTime"] = item.StartTime;
                row["SessionUser"] = (object)item.SessionUser ?? DBNull.Value;
                row["SessionMachine"] = (object)item.SessionMachine ?? DBNull.Value;
                row["SessionClientMachine"] = (object)item.SessionClientMachine ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
