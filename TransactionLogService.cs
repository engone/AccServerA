using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using InfoSciences;
using InfoSciences.DataLayers;
using Infosciences.Sage.My;
using Infosciences.Sage.My.Resources;
using IniParser;
using IniParser.Model;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualBasic.CompilerServices;

namespace Infosciences.Sage
{
	// Token: 0x02000015 RID: 21
	public class TransactionLogService
	{
		// Token: 0x06000112 RID: 274 RVA: 0x0000B3C8 File Offset: 0x000095C8
		public bool CheckDSLink()
		{
			bool flag = this.m_oDataLink != null;
			return flag && this.m_oDataLink.TableExists("acAction");
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000B3FC File Offset: 0x000095FC
		private string BuildValueListString(object m_it, string[] maFields)
		{
			string text = maFields[0];
			SqlDataServices oDataLink = this.m_oDataLink;
			object[] array;
			bool[] array2;
			object obj = NewLateBinding.LateGet(m_it, null, "getbyname", array = new object[]
			{
				text
			}, null, null, array2 = new bool[]
			{
				true
			});
			if (array2[0])
			{
				text = (string)Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array[0]), typeof(string));
			}
			string text2 = oDataLink.ExprForFilters(RuntimeHelpers.GetObjectValue(obj));
			checked
			{
				int num = maFields.GetLength(0) - 1;
				for (int i = 1; i <= num; i++)
				{
					text = maFields[i];
					string str = text2;
					string str2 = ",";
					SqlDataServices oDataLink2 = this.m_oDataLink;
					obj = NewLateBinding.LateGet(m_it, null, "getbyname", array = new object[]
					{
						text
					}, null, null, array2 = new bool[]
					{
						true
					});
					if (array2[0])
					{
						text = (string)Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array[0]), typeof(string));
					}
					text2 = str + str2 + oDataLink2.ExprForFilters(RuntimeHelpers.GetObjectValue(obj));
				}
				return text2;
			}
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000B50C File Offset: 0x0000970C
		private string BuildValueListString(object m_it, string FieldList)
		{
			string[] array = FieldList.Split(new char[]
			{
				',',
				';',
				'|'
			});
			string text = array[0];
			SqlDataServices oDataLink = this.m_oDataLink;
			object[] array2;
			bool[] array3;
			object obj = NewLateBinding.LateGet(m_it, null, "getbyname", array2 = new object[]
			{
				text
			}, null, null, array3 = new bool[]
			{
				true
			});
			if (array3[0])
			{
				text = (string)Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2[0]), typeof(string));
			}
			string text2 = oDataLink.ExprForFilters(RuntimeHelpers.GetObjectValue(obj));
			checked
			{
				int num = array.GetLength(0) - 1;
				for (int i = 1; i <= num; i++)
				{
					text = array[i];
					string str = text2;
					string str2 = ",";
					SqlDataServices oDataLink2 = this.m_oDataLink;
					obj = NewLateBinding.LateGet(m_it, null, "getbyname", array2 = new object[]
					{
						text
					}, null, null, array3 = new bool[]
					{
						true
					});
					if (array3[0])
					{
						text = (string)Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2[0]), typeof(string));
					}
					text2 = str + str2 + oDataLink2.ExprForFilters(RuntimeHelpers.GetObjectValue(obj));
				}
				return text2;
			}
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000B63C File Offset: 0x0000983C
		private string BuildUpdateValueListString(object m_it, string FieldList)
		{
			string[] array = FieldList.Split(new char[]
			{
				',',
				';',
				'|'
			});
			string text = array[0];
			string str = text;
			string str2 = "=";
			SqlDataServices oDataLink = this.m_oDataLink;
			object[] array2;
			bool[] array3;
			object obj = NewLateBinding.LateGet(m_it, null, "getbyname", array2 = new object[]
			{
				text
			}, null, null, array3 = new bool[]
			{
				true
			});
			if (array3[0])
			{
				text = (string)Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2[0]), typeof(string));
			}
			string text2 = str + str2 + oDataLink.ExprForFilters(RuntimeHelpers.GetObjectValue(obj));
			checked
			{
				int num = array.GetLength(0) - 1;
				for (int i = 1; i <= num; i++)
				{
					text = array[i];
					string[] array4 = new string[5];
					array4[0] = text2;
					array4[1] = ",";
					array4[2] = text;
					array4[3] = "=";
					int num2 = 4;
					SqlDataServices oDataLink2 = this.m_oDataLink;
					obj = NewLateBinding.LateGet(m_it, null, "getbyname", array2 = new object[]
					{
						text
					}, null, null, array3 = new bool[]
					{
						true
					});
					if (array3[0])
					{
						text = (string)Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2[0]), typeof(string));
					}
					array4[num2] = oDataLink2.ExprForFilters(RuntimeHelpers.GetObjectValue(obj));
					text2 = string.Concat(array4);
				}
				return text2;
			}
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000B794 File Offset: 0x00009994
		public static object ACACTION_FriendlyLabel()
		{
			return "Information d'action";
		}

		// Token: 0x06000117 RID: 279 RVA: 0x0000B7AC File Offset: 0x000099AC
		public acAction_Collection ACACTION_LoadCollection()
		{
			string text = ("Select " + this.m_ACACTIONSelectFieldsList + " From acAction" + this.m_ACACTIONSelectJoins) ?? "";
			DataTable dataTable = this.m_oDataLink.retNativeSqlResults(text);
			bool flag = dataTable == null;
			acAction_Collection result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = dataTable.Rows.Count == 0;
				if (flag2)
				{
					result = null;
				}
				else
				{
					acAction_Collection acAction_Collection = new acAction_Collection();
					acAction_Collection.LoadData(dataTable);
					result = acAction_Collection;
				}
			}
			return result;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000B828 File Offset: 0x00009A28
		public acAction_Collection ACACTION_LoadCollectionStartingWith(string startWith)
		{
			string text = string.Concat(new string[]
			{
				"Select ",
				this.m_ACACTIONSelectFieldsList,
				" From acAction",
				this.m_ACACTIONSelectJoins,
				"  Where (ACACTION.ActionType like '",
				startWith,
				"%')m_tb=m_oDatalink.RetNativesqlResults(stSql)"
			});
			DataTable dataTable;
			bool flag = dataTable == null;
			acAction_Collection result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = dataTable.Rows.Count == 0;
				if (flag2)
				{
					result = null;
				}
				else
				{
					acAction_Collection acAction_Collection = new acAction_Collection();
					acAction_Collection.LoadData(dataTable);
					result = acAction_Collection;
				}
			}
			return result;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000B8B4 File Offset: 0x00009AB4
		public acAction ACACTION_LoadItem(int m_KeyVal)
		{
			string text = string.Concat(new string[]
			{
				"Select ",
				this.m_ACACTIONSelectFieldsList,
				" From acAction",
				this.m_ACACTIONSelectJoins,
				" WHERE ActionID = ",
				this.m_oDataLink.ExprForFilters(m_KeyVal)
			});
			DataTable dataTable = this.m_oDataLink.retNativeSqlResults(text);
			bool flag = dataTable == null;
			acAction result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = dataTable.Rows.Count == 0;
				if (flag2)
				{
					result = null;
				}
				else
				{
					acAction_Collection acAction_Collection = new acAction_Collection();
					acAction_Collection.LoadData(dataTable);
					result = acAction_Collection[0];
				}
			}
			return result;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000B958 File Offset: 0x00009B58
		public acAction_Collection ACACTION_LoadSessionIDLinkedItems(int SessionID_Value)
		{
			string text = string.Concat(new string[]
			{
				"Select ",
				this.m_ACACTIONSelectFieldsList,
				" From acAction",
				this.m_ACACTIONSelectJoins,
				" WHERE SessionID= ",
				this.m_oDataLink.ExprForFilters(SessionID_Value)
			});
			DataTable dataTable = this.m_oDataLink.retNativeSqlResults(text);
			bool flag = dataTable == null;
			acAction_Collection result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = dataTable.Rows.Count == 0;
				if (flag2)
				{
					result = null;
				}
				else
				{
					acAction_Collection acAction_Collection = new acAction_Collection();
					acAction_Collection.LoadData(dataTable);
					result = acAction_Collection;
				}
			}
			return result;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000B9F8 File Offset: 0x00009BF8
		public bool ACACTION_SaveCollection(acAction_Collection oDetails)
		{
			bool flag = true;
			try
			{
				foreach (acAction it in oDetails)
				{
					string text = string.Concat(new string[]
					{
						"Insert into acAction(",
						this.m_ACACTIONInsertFieldsList,
						") Values (",
						this.BuildValueListString(it, this.m_ACACTIONInsertFieldsList),
						")"
					});
					try
					{
						this.m_oDataLink.ExecuteNativeSQL(text);
					}
					catch (Exception ex)
					{
						flag = false;
					}
					bool flag2 = !flag;
					if (flag2)
					{
						break;
					}
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
			return flag;
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000BAC8 File Offset: 0x00009CC8
		public bool ACACTION_UpdateCollection(acAction_Collection oDetails)
		{
			bool flag = true;
			try
			{
				foreach (acAction acAction in oDetails)
				{
					string text = "Update acAction Set " + this.BuildUpdateValueListString(acAction, this.m_ACACTIONInsertFieldsList) + " WHERE ActionID=" + this.m_oDataLink.ExprForFilters(acAction.ActionID);
					try
					{
						this.m_oDataLink.ExecuteNativeSQL(text);
					}
					catch (Exception ex)
					{
						flag = false;
					}
					bool flag2 = !flag;
					if (flag2)
					{
						break;
					}
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
			return flag;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000BB88 File Offset: 0x00009D88
		public bool ACACTION_SaveItem(acAction m_it)
		{
			acAction acAction = this.ACACTION_LoadItem(m_it.ActionID);
			bool flag = acAction == null;
			bool result;
			if (flag)
			{
				result = (this.ACACTION_CreateItem(m_it) != 0);
			}
			else
			{
				m_it.ActionID = acAction.ActionID;
				result = this.ACACTION_UpdateItem(m_it);
			}
			return result;
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000BBD8 File Offset: 0x00009DD8
		public bool ACACTION_SaveCollection2(acAction_Collection oDetails)
		{
			bool flag = true;
			try
			{
				foreach (acAction it in oDetails)
				{
					flag = this.ACACTION_SaveItem(it);
					bool flag2 = !flag;
					if (flag2)
					{
						return false;
					}
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
			return flag;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x0000BC40 File Offset: 0x00009E40
		public bool ACACTION_APPENDCollection(acAction_Collection oDetails)
		{
			bool flag = true;
			try
			{
				foreach (acAction acAction in oDetails)
				{
					string text = string.Concat(new string[]
					{
						"Insert into acAction(ActionKey,ActionType,ActionPiece,ActionStatus,SessionID,ActionID,ActionRetVal) Values (",
						this.m_oDataLink.ExprForFilters(acAction.ActionKey),
						",",
						this.m_oDataLink.ExprForFilters(acAction.ActionType),
						",",
						this.m_oDataLink.ExprForFilters(acAction.ActionPiece),
						",",
						this.m_oDataLink.ExprForFilters(acAction.ActionStatus),
						",",
						this.m_oDataLink.ExprForFilters(acAction.SessionID),
						",",
						this.m_oDataLink.ExprForFilters(acAction.ActionID),
						") "
					});
					try
					{
						this.m_oDataLink.ExecuteNativeSQL(text);
					}
					catch (Exception ex)
					{
						flag = false;
					}
					bool flag2 = !flag;
					if (flag2)
					{
						break;
					}
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
			return flag;
		}

		// Token: 0x06000120 RID: 288 RVA: 0x0000BDBC File Offset: 0x00009FBC
		public bool ACACTION_DELETECollection()
		{
			bool result = true;
			string text = "DELETE FROM  acAction";
			try
			{
				this.m_oDataLink.ExecuteNativeSQL(text);
			}
			catch (Exception ex)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000BE0C File Offset: 0x0000A00C
		public bool ACACTION_DELETEItem(int m_KeyVal)
		{
			bool flag = true;
			string text = "DELETE FROM acAction WHERE ActionID=" + this.m_oDataLink.ExprForFilters(m_KeyVal);
			try
			{
				this.m_oDataLink.ExecuteNativeSQL(text);
			}
			catch (Exception ex)
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = this.ACACTION_LoadItem(m_KeyVal) != null;
				if (flag3)
				{
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000BE8C File Offset: 0x0000A08C
		public bool ACACTION_DELETECollection(acAction_Collection m_col)
		{
			bool result = true;
			try
			{
				foreach (acAction acAction in m_col)
				{
					result = this.ACACTION_DELETEItem(acAction.ActionID);
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
			return result;
		}

		// Token: 0x06000123 RID: 291 RVA: 0x0000BEE8 File Offset: 0x0000A0E8
		public bool ACACTION_DeleteSessionIDLinkedItems(int SessionID_Value)
		{
			bool flag = true;
			string text = "DELETE FROM acAction WHERE SessionID=" + this.m_oDataLink.ExprForFilters(SessionID_Value);
			this.m_oDataLink.ExecuteNativeSQL(text);
			string text2 = " Select count(*) From acAction WHERE SessionID= " + this.m_oDataLink.ExprForFilters(SessionID_Value);
			DataTable dataTable = this.m_oDataLink.retNativeSqlResults(text2);
			bool flag2 = dataTable == null;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = dataTable.Rows.Count > 0;
				result = (!flag3 && flag);
			}
			return result;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x0000BF7C File Offset: 0x0000A17C
		public int ACACTION_FindId(acAction m_it)
		{
			string text = string.Concat(new string[]
			{
				"Select ActionID FROM acAction WHERE ActionKey=",
				this.m_oDataLink.ExprForFilters(m_it.ActionKey),
				" AND ActionType=",
				this.m_oDataLink.ExprForFilters(m_it.ActionType),
				" AND ActionPiece=",
				this.m_oDataLink.ExprForFilters(m_it.ActionPiece),
				" AND ActionStatus=",
				this.m_oDataLink.ExprForFilters(m_it.ActionStatus),
				" AND SessionID=",
				this.m_oDataLink.ExprForFilters(m_it.SessionID)
			});
			DataTable dataTable;
			try
			{
				dataTable = this.m_oDataLink.retNativeSqlResults(text);
			}
			catch (Exception ex)
			{
			}
			bool flag = dataTable == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = dataTable.Rows.Count == 0;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					result = Conversions.ToInteger(dataTable.Rows[0][0]);
				}
			}
			return result;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000C0A0 File Offset: 0x0000A2A0
		public bool ACACTION_SaveSessionIDLinkedCollection(acAction_Collection oDetails, int SessionID_Value)
		{
			string text = "DELETE FROM acAction WHERE SessionID=" + this.m_oDataLink.ExprForFilters(SessionID_Value);
			this.m_oDataLink.ExecuteNativeSQL(text);
			try
			{
				foreach (acAction acAction in oDetails)
				{
					acAction.SessionID = SessionID_Value;
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
			bool flag = this.ACACTION_SaveCollection(oDetails);
			bool result;
			return result;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000C12C File Offset: 0x0000A32C
		public int ACACTION_CreateItem(acAction m_it)
		{
			bool flag = true;
			string text = string.Concat(new string[]
			{
				"Insert into acAction(",
				this.m_ACACTIONInsertFieldsList,
				") Values (",
				this.BuildValueListString(m_it, this.m_ACACTIONInsertFieldsList),
				")"
			});
			try
			{
				this.m_oDataLink.ExecuteNativeSQL(text);
			}
			catch (Exception ex)
			{
				flag = false;
			}
			bool flag2 = flag;
			int num;
			if (flag2)
			{
				num = Conversions.ToInteger(this.m_oDataLink.RetNativeSqlScalar("Select Isnull(IDENT_CURRENT( 'acAction' ),0) as CURID"));
				acAction acAction = this.ACACTION_LoadItem(num);
				bool flag3 = acAction != null;
				if (!flag3)
				{
					flag = false;
					num = 0;
				}
			}
			return num;
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000C1F0 File Offset: 0x0000A3F0
		public bool ACACTION_UpdateItem(acAction m_it)
		{
			bool result = true;
			string text = string.Concat(new string[]
			{
				"ActionKey=",
				this.m_oDataLink.ExprForFilters(m_it.ActionKey),
				",ActionType=",
				this.m_oDataLink.ExprForFilters(m_it.ActionType),
				",ActionPiece=",
				this.m_oDataLink.ExprForFilters(m_it.ActionPiece),
				",ActionStatus=",
				this.m_oDataLink.ExprForFilters(m_it.ActionStatus),
				",SessionID=",
				this.m_oDataLink.ExprForFilters(m_it.SessionID)
			});
			string text2 = "Update acAction Set " + this.BuildUpdateValueListString(m_it, this.m_ACACTIONInsertFieldsList) + " WHERE ActionID=" + this.m_oDataLink.ExprForFilters(m_it.ActionID);
			try
			{
				this.m_oDataLink.ExecuteNativeSQL(text2);
			}
			catch (Exception ex)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0000C310 File Offset: 0x0000A510
		public static object ACSESSION_FriendlyLabel()
		{
			return "Information de Session";
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0000C328 File Offset: 0x0000A528
		public acSession_Collection ACSESSION_LoadCollection()
		{
			string text = ("Select " + this.m_ACSESSIONSelectFieldsList + " From acSession" + this.m_ACSESSIONSelectJoins) ?? "";
			DataTable dataTable = this.m_oDataLink.retNativeSqlResults(text);
			bool flag = dataTable == null;
			acSession_Collection result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = dataTable.Rows.Count == 0;
				if (flag2)
				{
					result = null;
				}
				else
				{
					acSession_Collection acSession_Collection = new acSession_Collection();
					acSession_Collection.LoadData(dataTable);
					result = acSession_Collection;
				}
			}
			return result;
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000C3A4 File Offset: 0x0000A5A4
		public acSession_Collection ACSESSION_LoadCollectionStartingWith(string startWith)
		{
			string text = string.Concat(new string[]
			{
				"Select ",
				this.m_ACSESSIONSelectFieldsList,
				" From acSession",
				this.m_ACSESSIONSelectJoins,
				" Where (ACSESSION.SessionUser like '",
				startWith,
				"%') "
			});
			DataTable dataTable = this.m_oDataLink.retNativeSqlResults(text);
			bool flag = dataTable == null;
			acSession_Collection result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = dataTable.Rows.Count == 0;
				if (flag2)
				{
					result = null;
				}
				else
				{
					acSession_Collection acSession_Collection = new acSession_Collection();
					acSession_Collection.LoadData(dataTable);
					result = acSession_Collection;
				}
			}
			return result;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000C43C File Offset: 0x0000A63C
		public acSession ACSESSION_LoadItem(int m_KeyVal)
		{
			string text = string.Concat(new string[]
			{
				"Select ",
				this.m_ACSESSIONSelectFieldsList,
				" From acSession",
				this.m_ACSESSIONSelectJoins,
				" WHERE SessionID = ",
				this.m_oDataLink.ExprForFilters(m_KeyVal)
			});
			DataTable dataTable = this.m_oDataLink.retNativeSqlResults(text);
			bool flag = dataTable == null;
			acSession result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = dataTable.Rows.Count == 0;
				if (flag2)
				{
					result = null;
				}
				else
				{
					acSession_Collection acSession_Collection = new acSession_Collection();
					acSession_Collection.LoadData(dataTable);
					result = acSession_Collection[0];
				}
			}
			return result;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000C4E0 File Offset: 0x0000A6E0
		public bool ACSESSION_SaveCollection(acSession_Collection oDetails)
		{
			bool flag = true;
			try
			{
				foreach (acSession it in oDetails)
				{
					string text = string.Concat(new string[]
					{
						"Insert into acSession(",
						this.m_ACSESSIONInsertFieldsList,
						") Values (",
						this.BuildValueListString(it, this.m_ACSESSIONInsertFieldsList),
						")"
					});
					try
					{
						this.m_oDataLink.ExecuteNativeSQL(text);
					}
					catch (Exception ex)
					{
						flag = false;
					}
					bool flag2 = !flag;
					if (flag2)
					{
						break;
					}
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
			return flag;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000C5B0 File Offset: 0x0000A7B0
		public bool ACSESSION_UpdateCollection(acSession_Collection oDetails)
		{
			bool flag = true;
			try
			{
				foreach (acSession acSession in oDetails)
				{
					string text = "Update acSession Set " + this.BuildUpdateValueListString(acSession, this.m_ACSESSIONInsertFieldsList) + " WHERE SessionID=" + this.m_oDataLink.ExprForFilters(acSession.SessionID);
					try
					{
						this.m_oDataLink.ExecuteNativeSQL(text);
					}
					catch (Exception ex)
					{
						flag = false;
					}
					bool flag2 = !flag;
					if (flag2)
					{
						break;
					}
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
			return flag;
		}

		// Token: 0x0600012E RID: 302 RVA: 0x0000C670 File Offset: 0x0000A870
		public bool ACSESSION_SaveItem(acSession m_it)
		{
			acSession acSession = this.ACSESSION_LoadItem(m_it.SessionID);
			bool flag = acSession == null;
			bool result;
			if (flag)
			{
				result = (this.ACSESSION_CreateItem(m_it) != 0);
			}
			else
			{
				m_it.SessionID = acSession.SessionID;
				result = this.ACSESSION_UpdateItem(m_it);
			}
			return result;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000C6C0 File Offset: 0x0000A8C0
		public bool ACSESSION_SaveCollection2(acSession_Collection oDetails)
		{
			bool flag = true;
			try
			{
				foreach (acSession it in oDetails)
				{
					flag = this.ACSESSION_SaveItem(it);
					bool flag2 = !flag;
					if (flag2)
					{
						return false;
					}
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
			return flag;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x0000C728 File Offset: 0x0000A928
		public bool ACSESSION_APPENDCollection(acSession_Collection oDetails)
		{
			bool flag = true;
			try
			{
				foreach (acSession acSession in oDetails)
				{
					bool flag2 = acSession.SessionID > 0;
					if (flag2)
					{
						string text = "SET IDENTITY_INSERT acSession ON ";
						this.m_oDataLink.ExecuteNativeSQL(text);
					}
					string text2 = string.Concat(new string[]
					{
						"Insert into acSession(SessionKey,SessionID,StartTime,SessionUser,SessionMachine,SessionClientMachine) Values (",
						this.m_oDataLink.ExprForFilters(acSession.SessionKey),
						",",
						this.m_oDataLink.ExprForFilters(acSession.SessionID),
						",",
						this.m_oDataLink.ExprForFilters(acSession.StartTime),
						",",
						this.m_oDataLink.ExprForFilters(acSession.SessionUser),
						",",
						this.m_oDataLink.ExprForFilters(acSession.SessionMachine),
						",",
						this.m_oDataLink.ExprForFilters(acSession.SessionClientMachine),
						") "
					});
					try
					{
						this.m_oDataLink.ExecuteNativeSQL(text2);
					}
					catch (Exception ex)
					{
						flag = false;
					}
					bool flag3 = !flag;
					if (flag3)
					{
						break;
					}
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
			return flag;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x0000C8C4 File Offset: 0x0000AAC4
		public bool ACSESSION_DELETECollection()
		{
			bool result = true;
			string text = "DELETE FROM  acSession";
			try
			{
				this.m_oDataLink.ExecuteNativeSQL(text);
			}
			catch (Exception ex)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0000C914 File Offset: 0x0000AB14
		public bool ACSESSION_DELETEItem(int m_KeyVal)
		{
			bool flag = true;
			string text = "DELETE FROM acSession WHERE SessionID=" + this.m_oDataLink.ExprForFilters(m_KeyVal);
			try
			{
				this.m_oDataLink.ExecuteNativeSQL(text);
			}
			catch (Exception ex)
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = this.ACSESSION_LoadItem(m_KeyVal) != null;
				if (flag3)
				{
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0000C994 File Offset: 0x0000AB94
		public bool ACSESSION_DELETECollection(acSession_Collection m_col)
		{
			bool result = true;
			try
			{
				foreach (acSession acSession in m_col)
				{
					result = this.ACSESSION_DELETEItem(acSession.SessionID);
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
			return result;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x0000C9F0 File Offset: 0x0000ABF0
		public int ACSESSION_FindId(acSession m_it)
		{
			string text = string.Concat(new string[]
			{
				"Select SessionID FROM acSession WHERE SessionKey=",
				this.m_oDataLink.ExprForFilters(m_it.SessionKey),
				" AND StartTime=",
				this.m_oDataLink.ExprForFilters(m_it.StartTime),
				" AND SessionUser=",
				this.m_oDataLink.ExprForFilters(m_it.SessionUser),
				" AND SessionMachine=",
				this.m_oDataLink.ExprForFilters(m_it.SessionMachine),
				" AND SessionClientMachine=",
				this.m_oDataLink.ExprForFilters(m_it.SessionClientMachine)
			});
			DataTable dataTable;
			try
			{
				dataTable = this.m_oDataLink.retNativeSqlResults(text);
			}
			catch (Exception ex)
			{
			}
			bool flag = dataTable == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = dataTable.Rows.Count == 0;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					result = Conversions.ToInteger(dataTable.Rows[0][0]);
				}
			}
			return result;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000CB10 File Offset: 0x0000AD10
		public int ACSESSION_CreateItem(acSession m_it)
		{
			bool flag = true;
			string text = string.Concat(new string[]
			{
				"Insert into acSession(",
				this.m_ACSESSIONInsertFieldsList,
				") Values (",
				this.BuildValueListString(m_it, this.m_ACSESSIONInsertFieldsList),
				")"
			});
			try
			{
				this.m_oDataLink.ExecuteNativeSQL(text);
			}
			catch (Exception ex)
			{
				flag = false;
			}
			bool flag2 = flag;
			int num;
			if (flag2)
			{
				num = Conversions.ToInteger(this.m_oDataLink.RetNativeSqlScalar("Select Isnull(IDENT_CURRENT( 'acSession' ),0) as CURID"));
				acSession acSession = this.ACSESSION_LoadItem(num);
				bool flag3 = acSession != null;
				if (!flag3)
				{
					flag = false;
					num = 0;
				}
			}
			return num;
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0000CBD4 File Offset: 0x0000ADD4
		public bool ACSESSION_UpdateItem(acSession m_it)
		{
			bool result = true;
			string text = string.Concat(new string[]
			{
				"SessionKey=",
				this.m_oDataLink.ExprForFilters(m_it.SessionKey),
				",StartTime=",
				this.m_oDataLink.ExprForFilters(m_it.StartTime),
				",SessionUser=",
				this.m_oDataLink.ExprForFilters(m_it.SessionUser),
				",SessionMachine=",
				this.m_oDataLink.ExprForFilters(m_it.SessionMachine),
				",SessionClientMachine=",
				this.m_oDataLink.ExprForFilters(m_it.SessionClientMachine)
			});
			string text2 = "Update acSession Set " + this.BuildUpdateValueListString(m_it, this.m_ACSESSIONInsertFieldsList) + " WHERE SessionID=" + this.m_oDataLink.ExprForFilters(m_it.SessionID);
			try
			{
				this.m_oDataLink.ExecuteNativeSQL(text2);
			}
			catch (Exception ex)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x0000CCF0 File Offset: 0x0000AEF0
		private string _ValidateServerName(string m_ServerName)
		{
			Server server;
			try
			{
				bool flag = string.IsNullOrEmpty(m_ServerName);
				if (flag)
				{
					server = new Server();
				}
				else
				{
					server = new Server(m_ServerName);
				}
			}
			catch (Exception ex)
			{
				server = null;
			}
			bool flag2 = server == null;
			string result;
			if (flag2)
			{
				result = string.Empty;
			}
			else
			{
				result = m_ServerName + "\\" + server.InstanceName;
			}
			return result;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x0000CD68 File Offset: 0x0000AF68
		private string getAvailableLocalSQLInstance()
		{
			string text = this._ValidateServerName(".");
			bool flag = string.IsNullOrEmpty(text);
			if (flag)
			{
				text = this._ValidateServerName(".\\sqlexpress");
			}
			return text;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000CDA0 File Offset: 0x0000AFA0
		private void __writeIniOption(string section, string key, string value)
		{
			string path = MyProject.Computer.FileSystem.SpecialDirectories.AllUsersApplicationData + "\\_acctransactionLogService.ini";
			FileIniDataParser fileIniDataParser = new FileIniDataParser();
			IniData iniData = new IniData();
			SectionData sectionData = new SectionData(section);
			KeyData keyData = new KeyData(key);
			keyData.Value = value;
			sectionData.Keys.AddKey(keyData);
			iniData.Sections.Add(sectionData);
			try
			{
				StreamWriter streamWriter = new StreamWriter(path);
				fileIniDataParser.WriteData(streamWriter, iniData);
				streamWriter.Flush();
			}
			catch (Exception ex)
			{
			}
			finally
			{
				StreamWriter streamWriter;
				bool flag = streamWriter != null;
				if (flag)
				{
					streamWriter.Close();
				}
			}
		}

		// Token: 0x0600013A RID: 314 RVA: 0x0000CE6C File Offset: 0x0000B06C
		private string __ReadIniOption(string section, string key, string DefaultValue = "")
		{
			string path = MyProject.Computer.FileSystem.SpecialDirectories.AllUsersApplicationData + "\\_acctransactionLogService.ini";
			FileIniDataParser fileIniDataParser = new FileIniDataParser();
			IniData iniData;
			try
			{
				StreamReader streamReader = new StreamReader(path);
				iniData = fileIniDataParser.ReadData(streamReader);
				streamReader.ReadToEnd();
			}
			catch (Exception ex)
			{
				iniData = null;
			}
			finally
			{
				StreamReader streamReader;
				bool flag = streamReader != null;
				if (flag)
				{
					streamReader.Close();
				}
			}
			bool flag2 = iniData != null;
			if (flag2)
			{
				bool flag3 = iniData[section] != null;
				if (flag3)
				{
					bool flag4 = iniData[section].ContainsKey(key);
					if (flag4)
					{
						return iniData.Sections[section].GetKeyData(key).Value;
					}
				}
			}
			return DefaultValue;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x0000CF58 File Offset: 0x0000B158
		public TransactionLogService()
		{
			this.m_Name = "dataservices";
			this.m_WorkData = "";
			this.m_ACACTIONSelectFieldsList = "acAction.ActionKey As ActionKey,acAction.ActionType As ActionType,acAction.ActionPiece As ActionPiece,acAction.ActionStatus As ActionStatus,acAction.SessionID As SessionID,acSession.SessionUser As SessionID_Libelle,ActionID,acSession.ActionRetVal";
			this.m_ACACTIONInsertFieldsList = "ActionKey,ActionType,ActionPiece,ActionStatus,SessionID,ActionRetVal";
			this.m_ACACTIONSelectJoins = " Inner Join acSession on acSession.SessionID=acAction.SessionID";
			this.m_ACSESSIONSelectFieldsList = "acSession.SessionKey As SessionKey,acSession.SessionID As SessionID,acSession.StartTime As StartTime,acSession.SessionUser As SessionUser,acSession.SessionMachine As SessionMachine,SessionClientMachine";
			this.m_ACSESSIONInsertFieldsList = "SessionKey,StartTime,SessionUser,SessionMachine,SessionClientMachine";
			this.m_ACSESSIONSelectJoins = "";
			this._sqlInstanceName = ".\\sqlexpress";
			SQLParams sqlparams = new SQLParams
			{
				ServerInstanceName = this._sqlInstanceName,
				DbName = "accLogDb",
				db_User = "islnk",
				db_User_Pwd = "2205$brico",
				DateFormat = 2
			};
			this.m_oDataLink = new SqlDataServices(sqlparams);
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000D01C File Offset: 0x0000B21C
		public TransactionLogService(string svrInstance)
		{
			this.m_Name = "dataservices";
			this.m_WorkData = "";
			this.m_ACACTIONSelectFieldsList = "acAction.ActionKey As ActionKey,acAction.ActionType As ActionType,acAction.ActionPiece As ActionPiece,acAction.ActionStatus As ActionStatus,acAction.SessionID As SessionID,acSession.SessionUser As SessionID_Libelle,ActionID,acSession.ActionRetVal";
			this.m_ACACTIONInsertFieldsList = "ActionKey,ActionType,ActionPiece,ActionStatus,SessionID,ActionRetVal";
			this.m_ACACTIONSelectJoins = " Inner Join acSession on acSession.SessionID=acAction.SessionID";
			this.m_ACSESSIONSelectFieldsList = "acSession.SessionKey As SessionKey,acSession.SessionID As SessionID,acSession.StartTime As StartTime,acSession.SessionUser As SessionUser,acSession.SessionMachine As SessionMachine,SessionClientMachine";
			this.m_ACSESSIONInsertFieldsList = "SessionKey,StartTime,SessionUser,SessionMachine,SessionClientMachine";
			this.m_ACSESSIONSelectJoins = "";
			this._sqlInstanceName = svrInstance;
			bool flag = this.CheckDB(this._sqlInstanceName);
			if (flag)
			{
				SQLParams sqlparams = new SQLParams
				{
					ServerInstanceName = this._sqlInstanceName,
					ParamDescr = "accLogDb"
				};
				this.m_oDataLink = new SqlDataServices(sqlparams);
			}
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000D0D0 File Offset: 0x0000B2D0
		private bool CheckDB(string svr)
		{
			bool flag = this._scripter == null;
			if (flag)
			{
				this._scripter = new Scripter(svr);
			}
			bool flag2 = string.IsNullOrEmpty(this._scripter.ServerInstance);
			bool result;
			if (flag2)
			{
				this.__writeIniOption("sqlinstance", "name", "");
				result = false;
			}
			else
			{
				bool flag3 = !this._scripter.dbExists("accLogDb");
				if (flag3)
				{
					string script = Resources.CreateDb;
					this._scripter.ExecuteGlobalScript(script);
					script = Resources.AccLogDb;
					this._scripter.ExecuteGlobalScript(script);
				}
				result = this._scripter.dbExists("accLogDb");
			}
			return result;
		}

		// Token: 0x04000077 RID: 119
		private string m_Name;

		// Token: 0x04000078 RID: 120
		private string m_WorkData;

		// Token: 0x04000079 RID: 121
		private SqlDataServices m_oDataLink;

		// Token: 0x0400007A RID: 122
		private string m_ACACTIONSelectFieldsList;

		// Token: 0x0400007B RID: 123
		private string m_ACACTIONInsertFieldsList;

		// Token: 0x0400007C RID: 124
		private string m_ACACTIONSelectJoins;

		// Token: 0x0400007D RID: 125
		private string m_ACSESSIONSelectFieldsList;

		// Token: 0x0400007E RID: 126
		private string m_ACSESSIONInsertFieldsList;

		// Token: 0x0400007F RID: 127
		private string m_ACSESSIONSelectJoins;

		// Token: 0x04000080 RID: 128
		private string _sqlInstanceName;

		// Token: 0x04000081 RID: 129
		private Scripter _scripter;
	}
}
