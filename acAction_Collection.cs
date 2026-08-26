using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Infosciences.Sage
{
	// Token: 0x0200000B RID: 11
	[Serializable]
	public class acAction_Collection : SortableBindingList<acAction>
	{
		// Token: 0x0600002D RID: 45 RVA: 0x00002BC4 File Offset: 0x00000DC4
		public acAction Add(string m_ActionKey, string m_ActionType, string m_ActionPiece, bool m_ActionStatus, int m_SessionID, int m_ActionID, int m_ActionRetVal)
		{
			acAction acAction = new acAction();
			acAction.ActionKey = m_ActionKey;
			acAction.ActionType = m_ActionType;
			acAction.ActionPiece = m_ActionPiece;
			acAction.ActionStatus = m_ActionStatus;
			acAction.SessionID = m_SessionID;
			acAction.ActionID = m_ActionID;
			acAction.ActionRetVal = m_ActionRetVal;
			base.Add(acAction);
			return acAction;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002C21 File Offset: 0x00000E21
		public acAction_Collection()
		{
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002C2C File Offset: 0x00000E2C
		public acAction_Collection(IList<acAction> lst)
		{
			bool flag = lst != null;
			if (flag)
			{
				try
				{
					foreach (acAction item in lst)
					{
						base.Add(item);
					}
				}
				finally
				{
					IEnumerator<acAction> enumerator;
					if (enumerator != null)
					{
						enumerator.Dispose();
					}
				}
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002C8C File Offset: 0x00000E8C
		public acAction_Collection(acAction[] aItems)
		{
			bool flag = aItems != null;
			if (flag)
			{
				foreach (acAction item in aItems)
				{
					base.Add(item);
				}
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002CD0 File Offset: 0x00000ED0
		public acAction[] ToArray()
		{
			Array array = Array.CreateInstance(typeof(acAction), base.Count);
			int num = -1;
			checked
			{
				try
				{
					foreach (acAction acAction in this)
					{
						num++;
						NewLateBinding.LateIndexSet(array, new object[]
						{
							num,
							acAction
						}, null);
					}
				}
				finally
				{
					IEnumerator<acAction> enumerator;
					if (enumerator != null)
					{
						enumerator.Dispose();
					}
				}
				return (acAction[])array;
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002D5C File Offset: 0x00000F5C
		public List<acAction> ToList()
		{
			List<acAction> list = new List<acAction>();
			try
			{
				foreach (acAction item in this)
				{
					list.Add(item);
				}
			}
			finally
			{
				IEnumerator<acAction> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
			return list;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002DB8 File Offset: 0x00000FB8
		public Dictionary<string, acAction> GetDictionary()
		{
			Dictionary<string, acAction> dictionary = new Dictionary<string, acAction>();
			try
			{
				foreach (acAction acAction in this)
				{
					dictionary.Add(Conversions.ToString(acAction.ActionID), acAction);
				}
			}
			finally
			{
				IEnumerator<acAction> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
			return dictionary;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002E20 File Offset: 0x00001020
		public object[] GetByNameArray(string m_PropertyName)
		{
			checked
			{
				object[] array = new object[base.Count + 1];
				int num = 0;
				try
				{
					foreach (acAction acAction in this)
					{
						array[num] = RuntimeHelpers.GetObjectValue(acAction.GetByName(m_PropertyName));
						num++;
					}
				}
				finally
				{
					IEnumerator<acAction> enumerator;
					if (enumerator != null)
					{
						enumerator.Dispose();
					}
				}
				return array;
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002E94 File Offset: 0x00001094
		public void LoadData(DataTable m_tb)
		{
			acAction acAction = new acAction();
			bool flag = m_tb == null;
			if (!flag)
			{
				try
				{
					foreach (object obj in m_tb.Rows)
					{
						DataRow dataRow = (DataRow)obj;
						acAction = new acAction();
						bool flag2 = m_tb.Columns.Contains("ActionKey");
						if (flag2)
						{
							bool flag3 = !Information.IsDBNull(RuntimeHelpers.GetObjectValue(dataRow["ActionKey"]));
							if (flag3)
							{
								acAction.ActionKey = Conversions.ToString(dataRow["ActionKey"]);
							}
						}
						bool flag4 = m_tb.Columns.Contains("ActionType");
						if (flag4)
						{
							bool flag5 = !Information.IsDBNull(RuntimeHelpers.GetObjectValue(dataRow["ActionType"]));
							if (flag5)
							{
								acAction.ActionType = Conversions.ToString(dataRow["ActionType"]);
							}
						}
						bool flag6 = m_tb.Columns.Contains("ActionPiece");
						if (flag6)
						{
							bool flag7 = !Information.IsDBNull(RuntimeHelpers.GetObjectValue(dataRow["ActionPiece"]));
							if (flag7)
							{
								acAction.ActionPiece = Conversions.ToString(dataRow["ActionPiece"]);
							}
						}
						bool flag8 = m_tb.Columns.Contains("ActionStatus");
						if (flag8)
						{
							bool flag9 = !Information.IsDBNull(RuntimeHelpers.GetObjectValue(dataRow["ActionStatus"]));
							if (flag9)
							{
								acAction.ActionStatus = Conversions.ToBoolean(dataRow["ActionStatus"]);
							}
						}
						bool flag10 = m_tb.Columns.Contains("SessionID");
						if (flag10)
						{
							bool flag11 = !Information.IsDBNull(RuntimeHelpers.GetObjectValue(dataRow["SessionID"]));
							if (flag11)
							{
								acAction.SessionID = Conversions.ToInteger(dataRow["SessionID"]);
							}
						}
						bool flag12 = m_tb.Columns.Contains("SessionID_Libelle");
						if (flag12)
						{
							bool flag13 = !Information.IsDBNull(RuntimeHelpers.GetObjectValue(dataRow["SessionID_Libelle"]));
							if (flag13)
							{
								acAction.SessionID_Libelle = Conversions.ToString(dataRow["SessionID_Libelle"]);
							}
						}
						bool flag14 = m_tb.Columns.Contains("ActionID");
						if (flag14)
						{
							bool flag15 = !Information.IsDBNull(RuntimeHelpers.GetObjectValue(dataRow["ActionID"]));
							if (flag15)
							{
								acAction.ActionID = Conversions.ToInteger(dataRow["ActionID"]);
							}
						}
						bool flag16 = m_tb.Columns.Contains("ActionRetVal");
						if (flag16)
						{
							bool flag17 = !Information.IsDBNull(RuntimeHelpers.GetObjectValue(dataRow["ActionRetVal"]));
							if (flag17)
							{
								acAction.ActionRetVal = Conversions.ToInteger(dataRow["ActionRetVal"]);
							}
						}
						base.Add(acAction);
					}
				}
				finally
				{
					IEnumerator enumerator;
					if (enumerator is IDisposable)
					{
						(enumerator as IDisposable).Dispose();
					}
				}
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000317C File Offset: 0x0000137C
		public DataTable GetTable()
		{
			acAction_Table acAction_Table = new acAction_Table();
			try
			{
				foreach (acAction acAction in this)
				{
					DataRow dataRow = acAction_Table.NewRow();
					dataRow["ActionKey"] = acAction.ActionKey;
					dataRow["ActionType"] = acAction.ActionType;
					dataRow["ActionPiece"] = acAction.ActionPiece;
					dataRow["ActionStatus"] = acAction.ActionStatus;
					dataRow["SessionID"] = acAction.SessionID;
					dataRow["ActionID"] = acAction.ActionID;
					dataRow["ActionRetVal"] = acAction.ActionRetVal;
					acAction_Table.Rows.Add(dataRow);
				}
			}
			finally
			{
				IEnumerator<acAction> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
			return acAction_Table;
		}
	}
}
