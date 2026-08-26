using System;
using System.Data;

namespace Infosciences.Sage
{
	public class acAction_Table : DataTable
	{
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
			base.Columns.Add(column);
			column = new DataColumn("ActionID", typeof(int));
			base.Columns.Add(column);
			column = new DataColumn("ActionRetVal", typeof(int));
			base.Columns.Add(column);
		}
	}
}
