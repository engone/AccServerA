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
	public class TransactionLogService
	{
		public bool CheckDSLink()
		{
			bool flag = this.m_oDataLink != null;
			return flag && this.m_oDataLink.TableExists("acAction");
		}
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
		public static object ACACTION_FriendlyLabel()
		{
			return "Information d'action";
		}
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
		public static object ACSESSION_FriendlyLabel()
		{
			return "Information de Session";
		}
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
		private string m_Name;
		private string m_WorkData;
		private SqlDataServices m_oDataLink;
		private string m_ACACTIONSelectFieldsList;
		private string m_ACACTIONInsertFieldsList;
		private string m_ACACTIONSelectJoins;
		private string m_ACSESSIONSelectFieldsList;
		private string m_ACSESSIONInsertFieldsList;
		private string m_ACSESSIONSelectJoins;
		private string _sqlInstanceName;
		private Scripter _scripter;
	}
}
