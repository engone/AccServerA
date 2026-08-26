using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Infosciences.Utility.Attributes;
using Microsoft.VisualBasic.CompilerServices;

namespace Infosciences.Sage
{
	// Token: 0x0200000A RID: 10
	[DBTableName("acAction")]
	[Serializable]
	public class acAction
	{
		// Token: 0x06000012 RID: 18 RVA: 0x00002324 File Offset: 0x00000524
		// Note: this type is marked as 'beforefieldinit'.
		static acAction()
		{
			TypeCode[] array = new TypeCode[7];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.AF93FF53348259661FC7793F8E0E87D25CB229F9053DD893F9413DF193F245FF).FieldHandle);
			acAction.a_Types = array;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000013 RID: 19 RVA: 0x0000238C File Offset: 0x0000058C
		public bool Modified
		{
			get
			{
				return this.m_Modified;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000014 RID: 20 RVA: 0x000023A4 File Offset: 0x000005A4
		public bool ObjectIsLocked
		{
			get
			{
				return this.m_ObjectIsLocked;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000023BC File Offset: 0x000005BC
		// (set) Token: 0x06000016 RID: 22 RVA: 0x000023D4 File Offset: 0x000005D4
		[StringLength(64, ErrorMessage = "La zone ActionKey ne peut depasser 64 caractere(s)")]
		public string ActionKey
		{
			get
			{
				return this.m_ActionKey;
			}
			set
			{
				this.m_ActionKey = value;
				this.m_Modified = true;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000017 RID: 23 RVA: 0x000023E8 File Offset: 0x000005E8
		// (set) Token: 0x06000018 RID: 24 RVA: 0x00002400 File Offset: 0x00000600
		[StringLength(10, ErrorMessage = "La zone ActionType ne peut depasser 10 caractere(s)")]
		[ZoneLibelle]
		public string ActionType
		{
			get
			{
				return this.m_ActionType;
			}
			set
			{
				this.m_ActionType = value;
				this.m_Modified = true;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002414 File Offset: 0x00000614
		// (set) Token: 0x0600001A RID: 26 RVA: 0x0000242C File Offset: 0x0000062C
		[StringLength(13, ErrorMessage = "La zone ActionPiece ne peut depasser 13 caractere(s)")]
		[ZoneLibelle]
		public string ActionPiece
		{
			get
			{
				return this.m_ActionPiece;
			}
			set
			{
				this.m_ActionPiece = value;
				this.m_Modified = true;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002440 File Offset: 0x00000640
		// (set) Token: 0x0600001C RID: 28 RVA: 0x00002458 File Offset: 0x00000658
		public bool ActionStatus
		{
			get
			{
				return this.m_ActionStatus;
			}
			set
			{
				this.m_ActionStatus = value;
				this.m_Modified = true;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600001D RID: 29 RVA: 0x0000246C File Offset: 0x0000066C
		// (set) Token: 0x0600001E RID: 30 RVA: 0x00002484 File Offset: 0x00000684
		[ExternalLink("acSession", "SessionID", "SessionUser", "acSession")]
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

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002495 File Offset: 0x00000695
		// (set) Token: 0x06000020 RID: 32 RVA: 0x0000249F File Offset: 0x0000069F
		public string SessionID_Libelle { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000021 RID: 33 RVA: 0x000024A8 File Offset: 0x000006A8
		// (set) Token: 0x06000022 RID: 34 RVA: 0x000024C0 File Offset: 0x000006C0
		[Key]
		public int ActionID
		{
			get
			{
				return this.m_ActionID;
			}
			set
			{
				this.m_ActionID = value;
				this.m_Modified = true;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000023 RID: 35 RVA: 0x000024D4 File Offset: 0x000006D4
		// (set) Token: 0x06000024 RID: 36 RVA: 0x000024EC File Offset: 0x000006EC
		public int ActionRetVal
		{
			get
			{
				return this.m_ActionRetVal;
			}
			set
			{
				this.m_ActionRetVal = value;
				this.m_Modified = true;
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002500 File Offset: 0x00000700
		public acAction()
		{
			this.m_Modified = false;
			this.m_ObjectIsLocked = false;
			this.m_ActionKey = "";
			this.m_ActionType = "";
			this.m_ActionPiece = "";
			this.m_ActionStatus = false;
			this.m_SessionID = 0;
			this.m_ActionID = 0;
			this.m_ActionRetVal = 0;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002560 File Offset: 0x00000760
		public acAction(string p_ActionKey, string p_ActionType, string p_ActionPiece, bool p_ActionStatus, int p_SessionID, int p_ActionID, int p_ActionRetVal)
		{
			this.m_Modified = false;
			this.m_ObjectIsLocked = false;
			this.m_ActionKey = "";
			this.m_ActionType = "";
			this.m_ActionPiece = "";
			this.m_ActionStatus = false;
			this.m_SessionID = 0;
			this.m_ActionID = 0;
			this.m_ActionRetVal = 0;
			this.m_ActionKey = p_ActionKey;
			this.m_ActionType = p_ActionType;
			this.m_ActionPiece = p_ActionPiece;
			this.m_ActionStatus = p_ActionStatus;
			this.m_SessionID = p_SessionID;
			this.m_ActionID = p_ActionID;
			this.m_ActionRetVal = p_ActionRetVal;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000025F8 File Offset: 0x000007F8
		public object[] GetValues()
		{
			return new object[]
			{
				this.m_ActionKey,
				this.m_ActionType,
				this.m_ActionPiece,
				this.m_ActionStatus,
				this.m_SessionID,
				this.m_ActionID,
				this.m_ActionRetVal
			};
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002668 File Offset: 0x00000868
		public void ImportFromObject(object m_ObjValue)
		{
			try
			{
				this.m_ActionKey = Conversions.ToString(NewLateBinding.LateGet(m_ObjValue, null, "ActionKey", new object[0], null, null, null));
			}
			catch (Exception ex)
			{
			}
			try
			{
				this.m_ActionType = Conversions.ToString(NewLateBinding.LateGet(m_ObjValue, null, "ActionType", new object[0], null, null, null));
			}
			catch (Exception ex2)
			{
			}
			try
			{
				this.m_ActionPiece = Conversions.ToString(NewLateBinding.LateGet(m_ObjValue, null, "ActionPiece", new object[0], null, null, null));
			}
			catch (Exception ex3)
			{
			}
			try
			{
				this.m_ActionStatus = Conversions.ToBoolean(NewLateBinding.LateGet(m_ObjValue, null, "ActionStatus", new object[0], null, null, null));
			}
			catch (Exception ex4)
			{
			}
			try
			{
				this.m_SessionID = Conversions.ToInteger(NewLateBinding.LateGet(m_ObjValue, null, "SessionID", new object[0], null, null, null));
			}
			catch (Exception ex5)
			{
			}
			try
			{
				this.SessionID_Libelle = Conversions.ToString(NewLateBinding.LateGet(m_ObjValue, null, "SessionID_Libelle", new object[0], null, null, null));
			}
			catch (Exception ex6)
			{
			}
			try
			{
				this.m_ActionID = Conversions.ToInteger(NewLateBinding.LateGet(m_ObjValue, null, "ActionID", new object[0], null, null, null));
			}
			catch (Exception ex7)
			{
			}
			try
			{
				this.m_ActionRetVal = Conversions.ToInteger(NewLateBinding.LateGet(m_ObjValue, null, "ActionRetVal", new object[0], null, null, null));
			}
			catch (Exception ex8)
			{
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x0000288C File Offset: 0x00000A8C
		public void SetByName(string m_PropertyName, object m_Value)
		{
			string text = m_PropertyName.Trim().ToUpper();
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 1823506311U)
			{
				if (num != 599207389U)
				{
					if (num != 1575727986U)
					{
						if (num == 1823506311U)
						{
							if (Operators.CompareString(text, "ACTIONTYPE", false) == 0)
							{
								this.m_ActionType = Conversions.ToString(m_Value);
							}
						}
					}
					else if (Operators.CompareString(text, "ACTIONID", false) == 0)
					{
						this.m_ActionID = Conversions.ToInteger(m_Value);
					}
				}
				else if (Operators.CompareString(text, "ACTIONRETVAL", false) == 0)
				{
					this.m_ActionRetVal = Conversions.ToInteger(m_Value);
				}
			}
			else if (num <= 3769736154U)
			{
				if (num != 2362439479U)
				{
					if (num == 3769736154U)
					{
						if (Operators.CompareString(text, "SESSIONID", false) == 0)
						{
							this.m_SessionID = Conversions.ToInteger(m_Value);
						}
					}
				}
				else if (Operators.CompareString(text, "ACTIONPIECE", false) == 0)
				{
					this.m_ActionPiece = Conversions.ToString(m_Value);
				}
			}
			else if (num != 3966381798U)
			{
				if (num == 4293367209U)
				{
					if (Operators.CompareString(text, "ACTIONSTATUS", false) == 0)
					{
						this.m_ActionStatus = Conversions.ToBoolean(m_Value);
					}
				}
			}
			else if (Operators.CompareString(text, "ACTIONKEY", false) == 0)
			{
				this.m_ActionKey = Conversions.ToString(m_Value);
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000029FC File Offset: 0x00000BFC
		public object GetByName(string m_PropertyName)
		{
			string text = m_PropertyName.Trim().ToUpper();
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			object result;
			if (num <= 1823506311U)
			{
				if (num != 599207389U)
				{
					if (num != 1575727986U)
					{
						if (num == 1823506311U)
						{
							if (Operators.CompareString(text, "ACTIONTYPE", false) == 0)
							{
								result = this.m_ActionType;
							}
						}
					}
					else if (Operators.CompareString(text, "ACTIONID", false) == 0)
					{
						result = this.m_ActionID;
					}
				}
				else if (Operators.CompareString(text, "ACTIONRETVAL", false) == 0)
				{
					result = this.m_ActionRetVal;
				}
			}
			else if (num <= 3769736154U)
			{
				if (num != 2362439479U)
				{
					if (num == 3769736154U)
					{
						if (Operators.CompareString(text, "SESSIONID", false) == 0)
						{
							result = this.m_SessionID;
						}
					}
				}
				else if (Operators.CompareString(text, "ACTIONPIECE", false) == 0)
				{
					result = this.m_ActionPiece;
				}
			}
			else if (num != 3966381798U)
			{
				if (num == 4293367209U)
				{
					if (Operators.CompareString(text, "ACTIONSTATUS", false) == 0)
					{
						result = this.m_ActionStatus;
					}
				}
			}
			else if (Operators.CompareString(text, "ACTIONKEY", false) == 0)
			{
				result = this.m_ActionKey;
			}
			return result;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002B5C File Offset: 0x00000D5C
		public string GetID()
		{
			return Conversions.ToString(this.m_ActionID);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002B7C File Offset: 0x00000D7C
		public override string ToString()
		{
			string text = string.Empty;
			text = this.m_ActionType.Trim() + this.m_ActionPiece.Trim();
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

		// Token: 0x04000011 RID: 17
		private bool m_Modified;

		// Token: 0x04000012 RID: 18
		private bool m_ObjectIsLocked;

		// Token: 0x04000013 RID: 19
		private string m_ActionKey;

		// Token: 0x04000014 RID: 20
		private string m_ActionType;

		// Token: 0x04000015 RID: 21
		private string m_ActionPiece;

		// Token: 0x04000016 RID: 22
		private bool m_ActionStatus;

		// Token: 0x04000017 RID: 23
		private int m_SessionID;

		// Token: 0x04000018 RID: 24
		private int m_ActionID;

		// Token: 0x04000019 RID: 25
		private int m_ActionRetVal;

		// Token: 0x0400001A RID: 26
		public static string[] a_Captions = new string[]
		{
			"ActionKey",
			"ActionType",
			"ActionPiece",
			"ActionStatus",
			"SessionID",
			"ActionID",
			"ActionRetVal"
		};

		// Token: 0x0400001B RID: 27
		public static TypeCode[] a_Types;
	}
}
