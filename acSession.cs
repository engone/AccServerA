using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Infosciences.Utility.Attributes;
using Microsoft.VisualBasic.CompilerServices;

namespace Infosciences.Sage
{
	// Token: 0x0200000E RID: 14
	[DBTableName("acSession")]
	[Serializable]
	public class acSession
	{
		// Token: 0x06000038 RID: 56 RVA: 0x00003374 File Offset: 0x00001574
		// Note: this type is marked as 'beforefieldinit'.
		static acSession()
		{
			TypeCode[] array = new TypeCode[6];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.91B1293B5F47F1DE94E9927281471FAA9E1FDDE1F92626CE0BF9B0903D49E588).FieldHandle);
			acSession.a_Types = array;
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000039 RID: 57 RVA: 0x000033D4 File Offset: 0x000015D4
		public bool Modified
		{
			get
			{
				return this.m_Modified;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600003A RID: 58 RVA: 0x000033EC File Offset: 0x000015EC
		public bool ObjectIsLocked
		{
			get
			{
				return this.m_ObjectIsLocked;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00003404 File Offset: 0x00001604
		// (set) Token: 0x0600003C RID: 60 RVA: 0x0000341C File Offset: 0x0000161C
		[StringLength(128, ErrorMessage = "La zone  ne peut depasser 128 caractere(s)")]
		public string SessionKey
		{
			get
			{
				return this.m_SessionKey;
			}
			set
			{
				this.m_SessionKey = value;
				this.m_Modified = true;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00003430 File Offset: 0x00001630
		// (set) Token: 0x0600003E RID: 62 RVA: 0x00003448 File Offset: 0x00001648
		[Key]
		public int SessionID
		{
			get
			{
				return this.m_SessionID;
			}
			set
			{
				this.m_SessionID = value;
				this.m_Modified = true;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600003F RID: 63 RVA: 0x0000345C File Offset: 0x0000165C
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00003474 File Offset: 0x00001674
		public DateTime StartTime
		{
			get
			{
				return this.m_StartTime;
			}
			set
			{
				this.m_StartTime = value;
				this.m_Modified = true;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00003488 File Offset: 0x00001688
		// (set) Token: 0x06000042 RID: 66 RVA: 0x000034A0 File Offset: 0x000016A0
		[StringLength(24, ErrorMessage = "La zone SessionUser ne peut depasser 24 caractere(s)")]
		[ZoneLibelle]
		public string SessionUser
		{
			get
			{
				return this.m_SessionUser;
			}
			set
			{
				this.m_SessionUser = value;
				this.m_Modified = true;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000043 RID: 67 RVA: 0x000034B4 File Offset: 0x000016B4
		// (set) Token: 0x06000044 RID: 68 RVA: 0x000034CC File Offset: 0x000016CC
		[StringLength(24, ErrorMessage = "La zone  ne peut depasser 24 caractere(s)")]
		public string SessionMachine
		{
			get
			{
				return this.m_SessionMachine;
			}
			set
			{
				this.m_SessionMachine = value;
				this.m_Modified = true;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000045 RID: 69 RVA: 0x000034E0 File Offset: 0x000016E0
		// (set) Token: 0x06000046 RID: 70 RVA: 0x000034F8 File Offset: 0x000016F8
		[StringLength(24, ErrorMessage = "La zone SessionClientMachine ne peut depasser 24 caractere(s)")]
		[ZoneLibelle]
		public string SessionClientMachine
		{
			get
			{
				return this.m_SessionClientMachine;
			}
			set
			{
				this.m_SessionClientMachine = value;
				this.m_Modified = true;
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x0000350C File Offset: 0x0000170C
		public acSession()
		{
			this.m_Modified = false;
			this.m_ObjectIsLocked = false;
			this.m_SessionKey = "";
			this.m_SessionID = 0;
			this.m_StartTime = DateTime.MinValue;
			this.m_SessionUser = "";
			this.m_SessionMachine = "";
			this.m_SessionClientMachine = "";
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003570 File Offset: 0x00001770
		public acSession(string p_SessionKey, int p_SessionID, DateTime p_StartTime, string p_SessionUser, string p_SessionMachine, string p_SessionClientMachine)
		{
			this.m_Modified = false;
			this.m_ObjectIsLocked = false;
			this.m_SessionKey = "";
			this.m_SessionID = 0;
			this.m_StartTime = DateTime.MinValue;
			this.m_SessionUser = "";
			this.m_SessionMachine = "";
			this.m_SessionClientMachine = "";
			this.m_SessionKey = p_SessionKey;
			this.m_SessionID = p_SessionID;
			this.m_StartTime = p_StartTime;
			this.m_SessionUser = p_SessionUser;
			this.m_SessionMachine = p_SessionMachine;
			this.m_SessionClientMachine = p_SessionClientMachine;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003600 File Offset: 0x00001800
		public object[] GetValues()
		{
			return new object[]
			{
				this.m_SessionKey,
				this.m_SessionID,
				this.m_StartTime,
				this.m_SessionUser,
				this.m_SessionMachine,
				this.m_SessionClientMachine
			};
		}

		// Token: 0x0600004A RID: 74 RVA: 0x0000365C File Offset: 0x0000185C
		public void ImportFromObject(object m_ObjValue)
		{
			try
			{
				this.m_SessionKey = Conversions.ToString(NewLateBinding.LateGet(m_ObjValue, null, "SessionKey", new object[0], null, null, null));
			}
			catch (Exception ex)
			{
			}
			try
			{
				this.m_SessionID = Conversions.ToInteger(NewLateBinding.LateGet(m_ObjValue, null, "SessionID", new object[0], null, null, null));
			}
			catch (Exception ex2)
			{
			}
			try
			{
				this.m_StartTime = Conversions.ToDate(NewLateBinding.LateGet(m_ObjValue, null, "StartTime", new object[0], null, null, null));
			}
			catch (Exception ex3)
			{
			}
			try
			{
				this.m_SessionUser = Conversions.ToString(NewLateBinding.LateGet(m_ObjValue, null, "SessionUser", new object[0], null, null, null));
			}
			catch (Exception ex4)
			{
			}
			try
			{
				this.m_SessionMachine = Conversions.ToString(NewLateBinding.LateGet(m_ObjValue, null, "SessionMachine", new object[0], null, null, null));
			}
			catch (Exception ex5)
			{
			}
			try
			{
				this.m_SessionClientMachine = Conversions.ToString(NewLateBinding.LateGet(m_ObjValue, null, "SessionClientMachine", new object[0], null, null, null));
			}
			catch (Exception ex6)
			{
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000037FC File Offset: 0x000019FC
		public void SetByName(string m_PropertyName, object m_Value)
		{
			string left = m_PropertyName.Trim().ToUpper();
			if (Operators.CompareString(left, "SESSIONKEY", false) != 0)
			{
				if (Operators.CompareString(left, "SESSIONID", false) != 0)
				{
					if (Operators.CompareString(left, "STARTTIME", false) != 0)
					{
						if (Operators.CompareString(left, "SESSIONUSER", false) != 0)
						{
							if (Operators.CompareString(left, "SESSIONMACHINE", false) != 0)
							{
								if (Operators.CompareString(left, "SESSIONCLIENTMACHINE", false) == 0)
								{
									this.m_SessionClientMachine = Conversions.ToString(m_Value);
								}
							}
							else
							{
								this.m_SessionMachine = Conversions.ToString(m_Value);
							}
						}
						else
						{
							this.m_SessionUser = Conversions.ToString(m_Value);
						}
					}
					else
					{
						this.m_StartTime = Conversions.ToDate(m_Value);
					}
				}
				else
				{
					this.m_SessionID = Conversions.ToInteger(m_Value);
				}
			}
			else
			{
				this.m_SessionKey = Conversions.ToString(m_Value);
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000038C8 File Offset: 0x00001AC8
		public object GetByName(string m_PropertyName)
		{
			string left = m_PropertyName.Trim().ToUpper();
			object result;
			if (Operators.CompareString(left, "SESSIONKEY", false) != 0)
			{
				if (Operators.CompareString(left, "SESSIONID", false) != 0)
				{
					if (Operators.CompareString(left, "STARTTIME", false) != 0)
					{
						if (Operators.CompareString(left, "SESSIONUSER", false) != 0)
						{
							if (Operators.CompareString(left, "SESSIONMACHINE", false) != 0)
							{
								if (Operators.CompareString(left, "SESSIONCLIENTMACHINE", false) == 0)
								{
									result = this.m_SessionClientMachine;
								}
							}
							else
							{
								result = this.m_SessionMachine;
							}
						}
						else
						{
							result = this.m_SessionUser;
						}
					}
					else
					{
						result = this.m_StartTime;
					}
				}
				else
				{
					result = this.m_SessionID;
				}
			}
			else
			{
				result = this.m_SessionKey;
			}
			return result;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003984 File Offset: 0x00001B84
		public int GetID()
		{
			return this.m_SessionID;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x0000399C File Offset: 0x00001B9C
		public override string ToString()
		{
			string text = string.Empty;
			text = this.m_SessionUser.Trim() + this.m_SessionClientMachine.Trim();
			bool flag = string.IsNullOrEmpty(text);
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = text;
			}
			return result;
		}

		// Token: 0x04000024 RID: 36
		private bool m_Modified;

		// Token: 0x04000025 RID: 37
		private bool m_ObjectIsLocked;

		// Token: 0x04000026 RID: 38
		private string m_SessionKey;

		// Token: 0x04000027 RID: 39
		private int m_SessionID;

		// Token: 0x04000028 RID: 40
		private DateTime m_StartTime;

		// Token: 0x04000029 RID: 41
		private string m_SessionUser;

		// Token: 0x0400002A RID: 42
		private string m_SessionMachine;

		// Token: 0x0400002B RID: 43
		private string m_SessionClientMachine;

		// Token: 0x0400002C RID: 44
		public static string[] a_Captions = new string[]
		{
			"SessionKey",
			"SessionID",
			"StartTime",
			"SessionUser",
			"SessionMachine",
			"SessionClientMachine"
		};

		// Token: 0x0400002D RID: 45
		public static TypeCode[] a_Types;
	}
}
