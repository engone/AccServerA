using System;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Infosciences.Sage
{
	public class Scripter
	{
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
		public bool dbExists(string m_dbName)
		{
			bool flag = this.m_SMOServer != null;
			return flag && this.m_SMOServer.Databases.Contains(m_dbName);
		}
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
		public Scripter()
		{
		}
		public Scripter(string instanceName)
		{
			this.ValidateServerName(instanceName);
		}
		private SmoApplication m_SMO;
		private DataTable m_SMOServers;
		private Server m_SMOServer;
		private Database m_SMODB;
		protected string m_Script;
		protected string[] CommandScripts;
		private StringBuilder _log;
	}
}
