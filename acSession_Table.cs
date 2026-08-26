using System;
using System.Data;

namespace Infosciences.Sage
{
	public class acSession_Table : DataTable
	{
		public acSession_Table()
		{
			base.TableName = "acSession";
			DataColumn column = new DataColumn("SessionKey", typeof(string));
			base.Columns.Add(column);
			column = new DataColumn("SessionID", typeof(int));
			base.Columns.Add(column);
			column = new DataColumn("StartTime", typeof(DateTime));
			base.Columns.Add(column);
			column = new DataColumn("SessionUser", typeof(string));
			base.Columns.Add(column);
			column = new DataColumn("SessionMachine", typeof(string));
			base.Columns.Add(column);
			column = new DataColumn("SessionClientMachine", typeof(string));
			base.Columns.Add(column);
		}
	}
}
