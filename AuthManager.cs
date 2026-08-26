using System;
using Infosciences.Auth.Portable;

namespace Infosciences.Sage
{
	public class AuthManager
	{
		public AuthManager()
		{
			this.m_AppSubDir = "AccountsCentral";
		}
		public PortableLoginService oLib
		{
			get
			{
				return this.m_Lib;
			}
		}
		private PortableLoginService GetBObject()
		{
			string text = "c:\\Infoscienceservers\\AccountsCentral";
			return new PortableLoginService(text);
		}
		private bool LinkIsValid()
		{
			return this.m_Lib != null && !string.IsNullOrEmpty(this.m_Lib.Comment());
		}
		public bool CheckBO()
		{
			bool flag = !this.LinkIsValid();
			if (flag)
			{
				this.m_Lib = this.GetBObject();
			}
			return this.LinkIsValid();
		}
		protected PortableLoginService m_Lib;
		protected string m_AppSubDir;
	}
}
