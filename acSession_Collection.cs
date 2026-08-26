using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Infosciences.Sage
{
	// Token: 0x0200000F RID: 15
	[Serializable]
	public class acSession_Collection : SortableBindingList<acSession>
	{
		// Token: 0x0600004F RID: 79 RVA: 0x000039E4 File Offset: 0x00001BE4
		public acSession Add(string m_SessionKey, int m_SessionID, DateTime m_StartTime, string m_SessionUser, string m_SessionMachine, string m_SessionClientMachine)
		{
			acSession acSession = new acSession();
			acSession.SessionKey = m_SessionKey;
			acSession.SessionID = m_SessionID;
			acSession.StartTime = m_StartTime;
			acSession.SessionUser = m_SessionUser;
			acSession.SessionMachine = m_SessionMachine;
			acSession.SessionClientMachine = m_SessionClientMachine;
			base.Add(acSession);
			return acSession;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003A38 File Offset: 0x00001C38
		public acSession_Collection()
		{
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003A44 File Offset: 0x00001C44
		public acSession_Collection(IList<acSession> lst)
		{
			bool flag = lst != null;
			if (flag)
			{
				try
				{
					foreach (acSession item in lst)
					{
						base.Add(item);
					}
				}
				finally
				{
					IEnumerator<acSession> enumerator;
					if (enumerator != null)
					{
						enumerator.Dispose();
					}
				}
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003AA4 File Offset: 0x00001CA4
		public acSession_Collection(acSession[] aItems)
		{
			bool flag = aItems != null;
			if (flag)
			{
				foreach (acSession item in aItems)
				{
					base.Add(item);
				}
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003AE8 File Offset: 0x00001CE8
		public acSession[] ToArray()
		{
			Array array = Array.CreateInstance(typeof(acSession), base.Count);
			int num = -1;
			checked
			{
				try
				{
					foreach (acSession acSession in this)
					{
						num++;
						NewLateBinding.LateIndexSet(array, new object[]
						{
							num,
							acSession
						}, null);
					}
				}
				finally
				{
					IEnumerator<acSession> enumerator;
					if (enumerator != null)
					{
						enumerator.Dispose();
					}
				}
				return (acSession[])array;
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003B74 File Offset: 0x00001D74
		public List<acSession> ToList()
		{
			List<acSession> list = new List<acSession>();
			try
			{
				foreach (acSession item in this)
				{
					list.Add(item);
				}
			}
			finally
			{
				IEnumerator<acSession> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
			return list;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003BD0 File Offset: 0x00001DD0
		public Dictionary<int, acSession> GetDictionary()
		{
			Dictionary<int, acSession> dictionary = new Dictionary<int, acSession>();
			try
			{
				foreach (acSession acSession in this)
				{
					dictionary.Add(acSession.SessionID, acSession);
				}
			}
			finally
			{
				IEnumerator<acSession> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
			return dictionary;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003C34 File Offset: 0x00001E34
		public object[] GetByNameArray(string m_PropertyName)
		{
			checked
			{
				object[] array = new object[base.Count + 1];
				int num = 0;
				try
				{
					foreach (acSession acSession in this)
					{
						array[num] = RuntimeHelpers.GetObjectValue(acSession.GetByName(m_PropertyName));
						num++;
					}
				}
				finally
				{
					IEnumerator<acSession> enumerator;
					if (enumerator != null)
					{
						enumerator.Dispose();
					}
				}
				return array;
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003CA8 File Offset: 0x00001EA8
		public void LoadData(DataTable m_tb)
		{
			acSession acSession = new acSession();
			bool flag = m_tb == null;
			if (!flag)
			{
				try
				{
					foreach (object obj in m_tb.Rows)
					{
						DataRow dataRow = (DataRow)obj;
						acSession = new acSession();
						bool flag2 = m_tb.Columns.Contains("SessionKey");
						if (flag2)
						{
							bool flag3 = !Information.IsDBNull(RuntimeHelpers.GetObjectValue(dataRow["SessionKey"]));
							if (flag3)
							{
								acSession.SessionKey = Conversions.ToString(dataRow["SessionKey"]);
							}
						}
						bool flag4 = m_tb.Columns.Contains("SessionID");
						if (flag4)
						{
							bool flag5 = !Information.IsDBNull(RuntimeHelpers.GetObjectValue(dataRow["SessionID"]));
							if (flag5)
							{
								acSession.SessionID = Conversions.ToInteger(dataRow["SessionID"]);
							}
						}
						bool flag6 = m_tb.Columns.Contains("StartTime");
						if (flag6)
						{
							bool flag7 = !Information.IsDBNull(RuntimeHelpers.GetObjectValue(dataRow["StartTime"]));
							if (flag7)
							{
								acSession.StartTime = Conversions.ToDate(dataRow["StartTime"]);
							}
						}
						bool flag8 = m_tb.Columns.Contains("SessionUser");
						if (flag8)
						{
							bool flag9 = !Information.IsDBNull(RuntimeHelpers.GetObjectValue(dataRow["SessionUser"]));
							if (flag9)
							{
								acSession.SessionUser = Conversions.ToString(dataRow["SessionUser"]);
							}
						}
						bool flag10 = m_tb.Columns.Contains("SessionMachine");
						if (flag10)
						{
							bool flag11 = !Information.IsDBNull(RuntimeHelpers.GetObjectValue(dataRow["SessionMachine"]));
							if (flag11)
							{
								acSession.SessionMachine = Conversions.ToString(dataRow["SessionMachine"]);
							}
						}
						bool flag12 = m_tb.Columns.Contains("SessionClientMachine");
						if (flag12)
						{
							bool flag13 = !Information.IsDBNull(RuntimeHelpers.GetObjectValue(dataRow["SessionClientMachine"]));
							if (flag13)
							{
								acSession.SessionClientMachine = Conversions.ToString(dataRow["SessionClientMachine"]);
							}
						}
						base.Add(acSession);
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

		// Token: 0x06000058 RID: 88 RVA: 0x00003EFC File Offset: 0x000020FC
		public DataTable GetTable()
		{
			acSession_Table acSession_Table = new acSession_Table();
			try
			{
				foreach (acSession acSession in this)
				{
					DataRow dataRow = acSession_Table.NewRow();
					dataRow["SessionKey"] = acSession.SessionKey;
					dataRow["SessionID"] = acSession.SessionID;
					dataRow["StartTime"] = acSession.StartTime;
					dataRow["SessionUser"] = acSession.SessionUser;
					dataRow["SessionMachine"] = acSession.SessionMachine;
					dataRow["SessionClientMachine"] = acSession.SessionClientMachine;
					acSession_Table.Rows.Add(dataRow);
				}
			}
			finally
			{
				IEnumerator<acSession> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
			return acSession_Table;
		}
	}
}
