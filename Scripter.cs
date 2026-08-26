using System;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Infosciences.Sage
{
	// Token: 0x02000014 RID: 20
	public class Scripter
	{
		// Token: 0x06000106 RID: 262 RVA: 0x0000AE50 File Offset: 0x00009050
		public static int ExecuterScript(string ServerInstanceName, string DatabaseName, string ScriptText)
		{
			bool flag = true;
			try
			{
				SmoApplication smoApplication = new SmoApplication();
			}
			catch (Exception ex)
			{
				flag = false;
			}
			bool flag2 = flag;
			int result;
			if (flag2)
			{
				Server server = new Server(ServerInstanceName);
				bool flag3 = server != null;
				if (flag3)
				{
					Database database;
					try
					{
						database = server.Databases[DatabaseName];
					}
					catch (Exception ex2)
					{
						database = null;
					}
					bool flag4 = database != null && database.IsDbOwner;
					if (flag4)
					{
						string[] separator = new string[]
						{
							"GO",
							"go",
							"Go",
							"gO"
						};
						string[] array = ScriptText.Split(separator, StringSplitOptions.RemoveEmptyEntries);
						string text = string.Empty;
						foreach (string str in array)
						{
							text = text + " " + str;
						}
						bool flag5 = array.GetLength(0) > 0;
						if (flag5)
						{
							try
							{
								database.ExecuteNonQuery(text, 2);
							}
							catch (FailedOperationException ex3)
							{
								flag = false;
							}
							catch (Exception ex4)
							{
							}
						}
					}
				}
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000AFCC File Offset: 0x000091CC
		private void ValidateServerName(string m_ServerName)
		{
			try
			{
				bool flag = string.IsNullOrEmpty(m_ServerName);
				if (flag)
				{
					this.m_SMOServer = new Server();
				}
				else
				{
					this.m_SMOServer = new Server(m_ServerName);
				}
				int count = this.m_SMOServer.Databases.Count;
			}
			catch (Exception ex)
			{
				this.m_SMOServer = null;
			}
			bool flag2 = this.m_SMOServer == null;
			if (flag2)
			{
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000108 RID: 264 RVA: 0x0000B04C File Offset: 0x0000924C
		// (set) Token: 0x06000109 RID: 265 RVA: 0x0000B0B7 File Offset: 0x000092B7
		public string ServerInstance
		{
			get
			{
				bool flag = this.m_SMOServer != null;
				string result;
				if (flag)
				{
					bool flag2 = string.IsNullOrEmpty(this.m_SMOServer.InstanceName);
					if (flag2)
					{
						result = this.m_SMOServer.Name;
					}
					else
					{
						result = this.m_SMOServer.Name + "\\" + this.m_SMOServer.InstanceName;
					}
				}
				else
				{
					result = string.Empty;
				}
				return result;
			}
			set
			{
				this.ValidateServerName(value);
			}
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000B0C4 File Offset: 0x000092C4
		public bool dbExists(string m_dbName)
		{
			bool flag = this.m_SMOServer != null;
			return flag && this.m_SMOServer.Databases.Contains(m_dbName);
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000B0FC File Offset: 0x000092FC
		private bool ActivateDb(string m_dbName)
		{
			bool flag = true;
			try
			{
				this.m_SMODB = this.m_SMOServer.Databases[m_dbName];
			}
			catch (Exception ex)
			{
				flag = false;
				this.m_SMODB = null;
			}
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = !(this.m_SMODB.IsDbOwner | this.m_SMODB.IsDbDdlAdmin);
				if (flag3)
				{
					Interaction.MsgBox("L'utilisateur courant ne dispose pas de droits suffisants pour cette opération", MsgBoxStyle.OkOnly, null);
					return false;
				}
			}
			return flag;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000B190 File Offset: 0x00009390
		private void BuildCommandList()
		{
			string[] separator = new string[]
			{
				"GO",
				"go",
				"Go",
				"gO"
			};
			this.CommandScripts = this.m_Script.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000B1D8 File Offset: 0x000093D8
		private string getCleanScript(string st)
		{
			string[] separator = new string[]
			{
				"GO",
				"go",
				"Go",
				"gO"
			};
			string[] array = st.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			string text = "";
			foreach (string str in array)
			{
				text = text + "\r" + str;
			}
			return text;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x0000B254 File Offset: 0x00009454
		public void ExecuterScript(string Script)
		{
			this.m_Script = Script;
			this._log = new StringBuilder();
			bool flag = Operators.CompareString(this.m_Script, string.Empty, false) == 0;
			if (!flag)
			{
				try
				{
					this.m_SMODB.ExecuteNonQuery(this.getCleanScript(this.m_Script), 2);
				}
				catch (FailedOperationException ex)
				{
					this._log.AppendLine("Echec execution script.");
				}
				catch (Exception ex2)
				{
				}
			}
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000B2F8 File Offset: 0x000094F8
		public void ExecuteGlobalScript(string Script)
		{
			this.m_Script = Script;
			this._log = new StringBuilder();
			bool flag = Operators.CompareString(this.m_Script, string.Empty, false) == 0;
			if (!flag)
			{
				this.ActivateDb("Master");
				try
				{
					this.m_SMOServer.ConnectionContext.ExecuteNonQuery(this.getCleanScript(this.m_Script), 2);
				}
				catch (FailedOperationException ex)
				{
					this._log.AppendLine("Echec execution script.");
				}
				catch (Exception ex2)
				{
				}
			}
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000B3AC File Offset: 0x000095AC
		public Scripter()
		{
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000B3B6 File Offset: 0x000095B6
		public Scripter(string instanceName)
		{
			this.ValidateServerName(instanceName);
		}

		// Token: 0x04000070 RID: 112
		private SmoApplication m_SMO;

		// Token: 0x04000071 RID: 113
		private DataTable m_SMOServers;

		// Token: 0x04000072 RID: 114
		private Server m_SMOServer;

		// Token: 0x04000073 RID: 115
		private Database m_SMODB;

		// Token: 0x04000074 RID: 116
		protected string m_Script;

		// Token: 0x04000075 RID: 117
		protected string[] CommandScripts;

		// Token: 0x04000076 RID: 118
		private StringBuilder _log;
	}
}
