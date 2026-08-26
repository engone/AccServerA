using System;
using System.Data;

namespace Infosciences.Sage
{
	// Token: 0x02000008 RID: 8
	public class acAction_Table : DataTable
	{
		// Token: 0x06000011 RID: 17 RVA: 0x00002200 File Offset: 0x00000400
		public acAction_Table()
		{
			base.TableName = "acAction";
			DataColumn column = new DataColumn("ActionKey", typeof(string));
			base.Columns.Add(column);
			column = new DataColumn("ActionType", typeof(string));
			base.Columns.Add(column);
			column = new DataColumn("ActionPiece", typeof(string));
			base.Columns.Add(column);
			column = new DataColumn("ActionStatus", typeof(bool));
			base.Columns.Add(column);
			column = new DataColumn("SessionID", typeof(int));
			base.Columns.Add(column);
			column = new DataColumn("SessionID_Libelle", typeof(string));
			column = new DataColumn("ActionID", typeof(int));
			base.Columns.Add(column);
			column = new DataColumn("ActionRetVal", typeof(int));
			base.Columns.Add(column);
		}
	}
}
