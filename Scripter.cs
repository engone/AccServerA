using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.Smo;

namespace Infosciences.Sage
{
    public class Scripter
    {
        private static readonly string[] BatchSeparators = { "GO", "go", "Go", "gO" };
        private Server _server;
        private Database _database;
        private string _script;
        private string[] _commandScripts;
        private StringBuilder _log;

        public Scripter() { }

        public Scripter(string instanceName)
        {
            ValidateServerName(instanceName);
        }

        public string ServerInstance
        {
            get
            {
                if (_server == null) return string.Empty;
                return string.IsNullOrEmpty(_server.InstanceName)
                    ? _server.Name
                    : _server.Name + "\\" + _server.InstanceName;
            }
            set { ValidateServerName(value); }
        }

        public static int ExecuterScript(string serverInstanceName, string databaseName, string scriptText)
        {
            try
            {
                var server = new Server(serverInstanceName);
                Database database = server.Databases[databaseName];
                if (database == null || !database.IsDbOwner) return 0;

                string script = JoinBatches(scriptText);
                if (script.Length > 0) database.ExecuteNonQuery(script, 2);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool dbExists(string databaseName)
        {
            return _server != null && _server.Databases.Contains(databaseName);
        }

        public void ExecuterScript(string script)
        {
            _script = script;
            _log = new StringBuilder();
            if (string.IsNullOrEmpty(_script)) return;

            try
            {
                _database.ExecuteNonQuery(CleanScript(_script), 2);
            }
            catch (Exception)
            {
                _log.AppendLine("Echec execution script.");
            }
        }

        public void ExecuteGlobalScript(string script)
        {
            _script = script;
            _log = new StringBuilder();
            if (string.IsNullOrEmpty(_script)) return;
            if (!ActivateDb("Master")) return;

            try
            {
                _server.ConnectionContext.ExecuteNonQuery(CleanScript(_script), 2);
            }
            catch (Exception)
            {
                _log.AppendLine("Echec execution script.");
            }
        }

        private void ValidateServerName(string serverName)
        {
            try
            {
                _server = string.IsNullOrEmpty(serverName) ? new Server() : new Server(serverName);
                int unused = _server.Databases.Count;
            }
            catch (Exception)
            {
                _server = null;
            }
        }

        private bool ActivateDb(string databaseName)
        {
            try
            {
                _database = _server.Databases[databaseName];
                if (!_database.IsDbOwner && !_database.IsDbDdlAdmin)
                {
                    MessageBox.Show("L'utilisateur courant ne dispose pas de droits suffisants pour cette opération");
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                _database = null;
                return false;
            }
        }

        private void BuildCommandList()
        {
            _commandScripts = _script.Split(BatchSeparators, StringSplitOptions.RemoveEmptyEntries);
        }

        private static string CleanScript(string script)
        {
            return string.Join("\r", script.Split(BatchSeparators, StringSplitOptions.RemoveEmptyEntries));
        }

        private static string JoinBatches(string script)
        {
            if (string.IsNullOrEmpty(script)) return string.Empty;
            return string.Join(" ", script.Split(BatchSeparators, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
