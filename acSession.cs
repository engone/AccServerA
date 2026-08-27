using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Infosciences.Utility.Attributes;

namespace Infosciences.Sage
{
    [DBTableName("acSession")]
    [Serializable]
    public class acSession
    {
        public static readonly string[] a_Captions = { "SessionKey", "SessionID", "StartTime", "SessionUser", "SessionMachine", "SessionClientMachine" };
        public static readonly TypeCode[] a_Types = { TypeCode.String, TypeCode.Int32, TypeCode.DateTime, TypeCode.String, TypeCode.String, TypeCode.String };

        public bool Modified { get; private set; }
        public bool ObjectIsLocked { get; private set; }

        [StringLength(128)]
        public string SessionKey
        {
            get { return _SessionKey; }
            set { _SessionKey = value; Modified = true; }
        }
        [Key]
        public int SessionID
        {
            get { return _SessionID; }
            set { _SessionID = value; Modified = true; }
        }
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; Modified = true; }
        }
        [StringLength(24)]
        [ZoneLibelle]
        public string SessionUser
        {
            get { return _SessionUser; }
            set { _SessionUser = value; Modified = true; }
        }
        [StringLength(24)]
        public string SessionMachine
        {
            get { return _SessionMachine; }
            set { _SessionMachine = value; Modified = true; }
        }
        [StringLength(24)]
        [ZoneLibelle]
        public string SessionClientMachine
        {
            get { return _SessionClientMachine; }
            set { _SessionClientMachine = value; Modified = true; }
        }

        public acSession()
        {
            _SessionKey = string.Empty;
            _SessionID = 0;
            _StartTime = DateTime.MinValue;
            _SessionUser = string.Empty;
            _SessionMachine = string.Empty;
            _SessionClientMachine = string.Empty;
        }

        public acSession(string sessionKey, int sessionID, DateTime startTime, string sessionUser, string sessionMachine, string sessionClientMachine)
        {
            _SessionKey = sessionKey;
            _SessionID = sessionID;
            _StartTime = startTime;
            _SessionUser = sessionUser;
            _SessionMachine = sessionMachine;
            _SessionClientMachine = sessionClientMachine;
        }

        public object[] GetValues() { return new object[] { _SessionKey, _SessionID, _StartTime, _SessionUser, _SessionMachine, _SessionClientMachine }; }

        public void ImportFromObject(object source)
        {
            if (source == null) return;
            TryImport(source, "SessionKey", value => _SessionKey = Convert.ToString(value));
            TryImport(source, "SessionID", value => _SessionID = Convert.ToInt32(value));
            TryImport(source, "StartTime", value => _StartTime = Convert.ToDateTime(value));
            TryImport(source, "SessionUser", value => _SessionUser = Convert.ToString(value));
            TryImport(source, "SessionMachine", value => _SessionMachine = Convert.ToString(value));
            TryImport(source, "SessionClientMachine", value => _SessionClientMachine = Convert.ToString(value));
        }

        public void SetByName(string propertyName, object value)
        {
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            switch (propertyName.Trim().ToUpperInvariant())
            {
                case "SESSIONKEY": SessionKey = (string)Convert.ChangeType(value, typeof(string)); break;
                case "SESSIONID": SessionID = (int)Convert.ChangeType(value, typeof(int)); break;
                case "STARTTIME": StartTime = (DateTime)Convert.ChangeType(value, typeof(DateTime)); break;
                case "SESSIONUSER": SessionUser = (string)Convert.ChangeType(value, typeof(string)); break;
                case "SESSIONMACHINE": SessionMachine = (string)Convert.ChangeType(value, typeof(string)); break;
                case "SESSIONCLIENTMACHINE": SessionClientMachine = (string)Convert.ChangeType(value, typeof(string)); break;
                default: return;
            }
        }

        public object GetByName(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            switch (propertyName.Trim().ToUpperInvariant())
            {
                case "SESSIONKEY": return SessionKey;
                case "SESSIONID": return SessionID;
                case "STARTTIME": return StartTime;
                case "SESSIONUSER": return SessionUser;
                case "SESSIONMACHINE": return SessionMachine;
                case "SESSIONCLIENTMACHINE": return SessionClientMachine;
                default: return null;
            }
        }

        public int GetID() { return SessionID; }

        public override string ToString()
        {
            return (SessionUser ?? string.Empty).Trim() + (SessionClientMachine ?? string.Empty).Trim();
        }

        private static void TryImport(object source, string propertyName, Action<object> assign)
        {
            try
            {
                PropertyInfo property = source.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null) assign(property.GetValue(source, null));
            }
            catch (Exception)
            {
                // Le code VB d'origine ignorait les propriétés absentes ou incompatibles.
            }
        }

        private string _SessionKey;
        private int _SessionID;
        private DateTime _StartTime;
        private string _SessionUser;
        private string _SessionMachine;
        private string _SessionClientMachine;
    }
}
