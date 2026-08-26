using System;
using Infosciences.Auth.Portable;
using Microsoft.VisualBasic.CompilerServices;

namespace Infosciences.Sage
{
	// Token: 0x02000010 RID: 16
	public class AuthManager
	{
		// Token: 0x06000059 RID: 89 RVA: 0x00003FE8 File Offset: 0x000021E8
		public AuthManager()
		{
			this.m_AppSubDir = "AccountsCentral";
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00003FFC File Offset: 0x000021FC
		public PortableLoginService oLib
		{
			get
			{
				return this.m_Lib;
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004014 File Offset: 0x00002214
		private PortableLoginService GetBObject()
		{
			string text = "c:\\Infoscienceservers\\AccountsCentral";
			return new PortableLoginService(text);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004034 File Offset: 0x00002234
		private bool LinkIsValid()
		{
			return this.m_Lib != null && Operators.CompareString(this.m_Lib.Comment(), string.Empty, false) != 0;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00004074 File Offset: 0x00002274
		public bool CheckBO()
		{
			bool flag = !this.LinkIsValid();
			if (flag)
			{
				this.m_Lib = this.GetBObject();
			}
			return this.LinkIsValid();
		}

		// Token: 0x0400002E RID: 46
		protected PortableLoginService m_Lib;

		// Token: 0x0400002F RID: 47
		protected string m_AppSubDir;
	}
}
