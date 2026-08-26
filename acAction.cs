using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Infosciences.Utility.Attributes;

namespace Infosciences.Sage
{
    [DBTableName("acAction")]
    [Serializable]
    public class acAction
    {
        public static readonly string[] a_Captions = { "ActionKey", "ActionType", "ActionPiece", "ActionStatus", "SessionID", "ActionID", "ActionRetVal" };
        public static readonly TypeCode[] a_Types = { TypeCode.String, TypeCode.String, TypeCode.String, TypeCode.Boolean, TypeCode.Int32, TypeCode.Int32, TypeCode.Int32 };

        public bool Modified { get; private set; }
        public bool ObjectIsLocked { get; private set; }

        [StringLength(64)]
        public string ActionKey
        {
            get { return _ActionKey; }
            set { _ActionKey = value; Modified = true; }
        }
        [StringLength(10)]
        [ZoneLibelle]
        public string ActionType
        {
            get { return _ActionType; }
            set { _ActionType = value; Modified = true; }
        }
        [StringLength(13)]
        [ZoneLibelle]
        public string ActionPiece
        {
            get { return _ActionPiece; }
            set { _ActionPiece = value; Modified = true; }
        }
        public bool ActionStatus
        {
            get { return _ActionStatus; }
            set { _ActionStatus = value; Modified = true; }
        }
        [ExternalLink("acSession", "SessionID", "SessionUser", "acSession")]
        public int SessionID
        {
            get { return _SessionID; }
            set { _SessionID = value; Modified = true; }
        }
        public string SessionID_Libelle { get; set; }
        [Key]
        public int ActionID
        {
            get { return _ActionID; }
            set { _ActionID = value; Modified = true; }
        }
        public int ActionRetVal
        {
            get { return _ActionRetVal; }
            set { _ActionRetVal = value; Modified = true; }
        }

        public acAction()
        {
            _ActionKey = string.Empty;
            _ActionType = string.Empty;
            _ActionPiece = string.Empty;
            _ActionStatus = false;
            _SessionID = 0;
            _ActionID = 0;
            _ActionRetVal = 0;
        }

        public acAction(string actionKey, string actionType, string actionPiece, bool actionStatus, int sessionID, int actionID, int actionRetVal)
        {
            _ActionKey = actionKey;
            _ActionType = actionType;
            _ActionPiece = actionPiece;
            _ActionStatus = actionStatus;
            _SessionID = sessionID;
            _ActionID = actionID;
            _ActionRetVal = actionRetVal;
        }

        public object[] GetValues() { return new object[] { _ActionKey, _ActionType, _ActionPiece, _ActionStatus, _SessionID, _ActionID, _ActionRetVal }; }

        public void ImportFromObject(object source)
        {
            if (source == null) return;
            TryImport(source, "ActionKey", value => ActionKey = (string)Convert.ChangeType(value, typeof(string)));
            TryImport(source, "ActionType", value => ActionType = (string)Convert.ChangeType(value, typeof(string)));
            TryImport(source, "ActionPiece", value => ActionPiece = (string)Convert.ChangeType(value, typeof(string)));
            TryImport(source, "ActionStatus", value => ActionStatus = (bool)Convert.ChangeType(value, typeof(bool)));
            TryImport(source, "SessionID", value => SessionID = (int)Convert.ChangeType(value, typeof(int)));
            TryImport(source, "ActionID", value => ActionID = (int)Convert.ChangeType(value, typeof(int)));
            TryImport(source, "ActionRetVal", value => ActionRetVal = (int)Convert.ChangeType(value, typeof(int)));
        }

        public void SetByName(string propertyName, object value)
        {
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            switch (propertyName.Trim().ToUpperInvariant())
            {
                case "ACTIONKEY": ActionKey = (string)Convert.ChangeType(value, typeof(string)); break;
                case "ACTIONTYPE": ActionType = (string)Convert.ChangeType(value, typeof(string)); break;
                case "ACTIONPIECE": ActionPiece = (string)Convert.ChangeType(value, typeof(string)); break;
                case "ACTIONSTATUS": ActionStatus = (bool)Convert.ChangeType(value, typeof(bool)); break;
                case "SESSIONID": SessionID = (int)Convert.ChangeType(value, typeof(int)); break;
                case "ACTIONID": ActionID = (int)Convert.ChangeType(value, typeof(int)); break;
                case "ACTIONRETVAL": ActionRetVal = (int)Convert.ChangeType(value, typeof(int)); break;
                default: throw new ArgumentException("Propriété inconnue.", nameof(propertyName));
            }
        }

        public object GetByName(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            switch (propertyName.Trim().ToUpperInvariant())
            {
                case "ACTIONKEY": return ActionKey;
                case "ACTIONTYPE": return ActionType;
                case "ACTIONPIECE": return ActionPiece;
                case "ACTIONSTATUS": return ActionStatus;
                case "SESSIONID": return SessionID;
                case "ACTIONID": return ActionID;
                case "ACTIONRETVAL": return ActionRetVal;
                default: throw new ArgumentException("Propriété inconnue.", nameof(propertyName));
            }
        }

        public int GetID() { return ActionID; }

        public override string ToString()
        {
            return (ActionType ?? string.Empty).Trim() + (ActionPiece ?? string.Empty).Trim();
        }

        private static void TryImport(object source, string propertyName, Action<object> assign)
        {
            PropertyInfo property = source.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property != null) assign(property.GetValue(source, null));
        }

        private string _ActionKey;
        private string _ActionType;
        private string _ActionPiece;
        private bool _ActionStatus;
        private int _SessionID;
        private int _ActionID;
        private int _ActionRetVal;
    }
}
