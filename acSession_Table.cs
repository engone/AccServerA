using System;
using System.Data;

namespace Infosciences.Sage
{
	// Token: 0x0200000C RID: 12
	public class acSession_Table : DataTable
	{
		// Token: 0x06000037 RID: 55 RVA: 0x00003284 File Offset: 0x00001484
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
