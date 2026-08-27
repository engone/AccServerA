using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfoSciences;
using Infosciences.Accounting.objects;
using Infosciences.Accounting.Server;
using Infosciences.Auth.Objects;
using Infosciences.Auth.Portable;
using Infosciences.TPV35;

namespace Infosciences.Sage
{
	public class SageAccService : IAccService
	{
		private virtual EventLog m_EventLog { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }
		public string ConnectionKey { get; set; }
		public int MAXRET
		{
			get
			{
				return this._MAXRET;
			}
			set
			{
				this._MAXRET = value;
			}
		}
		// (add) Token: 0x0600006F RID: 111 RVA: 0x00004158 File Offset: 0x00002358
		// (remove) Token: 0x06000070 RID: 112 RVA: 0x00004190 File Offset: 0x00002390
		public event SageAccService.DSLinkAvailabilityChangedEventHandler DSLinkAvailabilityChanged;
		private string clientIP()
		{
			OperationContext operationContext = OperationContext.Current;
			MessageProperties incomingMessageProperties = operationContext.IncomingMessageProperties;
			RemoteEndpointMessageProperty remoteEndpointMessageProperty = (RemoteEndpointMessageProperty)incomingMessageProperties[RemoteEndpointMessageProperty.Name];
			return remoteEndpointMessageProperty.Address;
		}
		private void LoadSettings()
		{
			int num = Utils.LoadMaxRet();
			bool flag = num > 0;
			if (flag)
			{
				this._DOCMAXRET = num;
			}
			else
			{
				Utils.SaveMaxRets(this._DOCMAXRET);
			}
		}
		public string SiteVersionNo()
		{
			return this.m_SiteVersion;
		}
		public void InitiateService()
		{
			this.m_EventLog = new EventLog();
			bool flag = !EventLog.SourceExists("AccServices_Source");
			if (flag)
			{
				EventLog.CreateEventSource("AccServices_Source", "accServer_Log");
			}
			this.m_EventLog.Source = "AccServices_Source";
			this.m_EventLog.Log = "accServer_Log";
			this.LoadSettings();
		}
		private bool _sessionLoggerAvailable()
		{
			return false;
		}
		private void _ConnectSessionLogger()
		{
		}
		public bool IsReady()
		{
			return this.m_oCat.ISCPTADataLinkReady();
		}
		private SageNetServices Connect()
		{
			char value = '\r';
			this._resetLogs();
			bool flag = this.m_EventLog == null;
			if (flag)
			{
				this.InitiateService();
			}
			this.m_EventLog.WriteEntry("Initialisation de SageNetServices Middleware ...");
			this._JobLog = new StringBuilder("Initialisation de SageNetServices Middleware ..." + Convert.ToString(value));
			this._Dossier_Courant = this.m_DossierSage;
			this.m_oCat = new SageNetServices();
			this.m_EventLog.WriteEntry("Connection à [" + this.m_DossierSage + " ...");
			this._JobLog.AppendLine("Connection à [" + this.m_DossierSage + " ...");
			bool flag2 = !this.m_oCat.InitLink(this.m_DossierSage);
			SageNetServices result;
			if (flag2)
			{
				this.m_EventLog.WriteEntry("Echec Connexion \r" + this.m_oCat.ConnectLog);
				this._JobLog.AppendLine("Echec Connexion \r" + this.m_oCat.ConnectLog);
				this.m_oCat = null;
				result = null;
			}
			else
			{
				this.m_EventLog.WriteEntry(string.Concat(
					"Connection à [",
					this.m_DossierSage,
					"] Workdata=[",
					this.m_oCat.WorkData,
					"]"
				));
				this._JobLog.AppendLine(string.Concat(
					"Connection à [",
					this.m_DossierSage,
					"] Workdata=[",
					this.m_oCat.WorkData,
					"]"
				));
				this.m_EventLog.WriteEntry(string.Format("Format de date  {0}  {1}", this.m_oCat.getDateFormat(), this.m_oCat.DbMachine));
				this._JobLog.AppendLine(string.Format("Format de date  {0}  {1}", this.m_oCat.getDateFormat(), this.m_oCat.DbMachine));
				bool flag3 = this._sessionLoggerAvailable();
				if (flag3)
				{
					this._acSession = new acSession
					{
						SessionKey = this.m_DossierSage,
						StartTime = DateAndTime.Now,
						SessionMachine = Environment.MachineName,
						SessionClientMachine = this.clientIP(),
						SessionUser = this._instanceUserKey
					};
					this._sessionID = this._sessionLogger.ACSESSION_CreateItem(this._acSession);
				}
				this.m_oCat.EnableInstantLogging(false);
				result = this.m_oCat;
			}
			return result;
		}
		private bool CheckDSLink()
		{
			bool flag = this.m_oCat == null && !string.IsNullOrWhiteSpace(this._Dossier_Courant);
			if (flag)
			{
				this.TryLinkServer(this._Dossier_Courant);
			}
			bool flag2 = this.m_oCat == null;
			bool result;
			if (flag2)
			{
				SageAccService.DSLinkAvailabilityChangedEventHandler dslinkAvailabilityChangedEvent = this.DSLinkAvailabilityChangedEvent;
				if (dslinkAvailabilityChangedEvent != null)
				{
					dslinkAvailabilityChangedEvent(false);
				}
				result = false;
			}
			else
			{
				SageAccService.DSLinkAvailabilityChangedEventHandler dslinkAvailabilityChangedEvent2 = this.DSLinkAvailabilityChangedEvent;
				if (dslinkAvailabilityChangedEvent2 != null)
				{
					dslinkAvailabilityChangedEvent2(true);
				}
				InfoDossier_Collection infoDossier_Collection = this.m_oCat.INFO_DOSSIER_LoadCollection();
				bool flag3 = infoDossier_Collection != null && infoDossier_Collection.Count > 0;
				if (flag3)
				{
					this.m_LastComment = "Accounts Server - [" + infoDossier_Collection[0].D_RAISONSOC + "]";
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}
		public string TransmissionLog
		{
			get
			{
				bool flag = this._transmissionLog != null;
				string result;
				if (flag)
				{
					result = this._transmissionLog.ToString();
				}
				else
				{
					result = string.Empty;
				}
				return result;
			}
		}
		private void _resetLogs()
		{
			this._JobLog = null;
			this._transmissionLog = null;
		}
		private StringBuilder _getLivingLog()
		{
			bool flag = this._JobLog != null;
			StringBuilder result;
			if (flag)
			{
				result = this._JobLog;
			}
			else
			{
				bool flag2 = this._transmissionLog != null;
				if (flag2)
				{
					result = this._transmissionLog;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}
		public string LastComment()
		{
			bool flag = this._getLivingLog() == null;
			string result;
			if (flag)
			{
				result = this.m_LastComment;
			}
			else
			{
				result = this._getLivingLog().ToString();
			}
			return result;
		}
		public List<string> getDBLog()
		{
			return this.m_oCat.getCurrentSQLTextLogs();
		}
		public Imputation[] InterrogerCompteTiers(string m_Acct, DateTime m_Date1, DateTime m_Date2, int m_CatEcr)
		{
			this.m_ImputationBag = new List<Imputation>();
			clsCbEcritureComptable_Collection clsCbEcritureComptable_Collection = this.m_oCat.ECRITURE_LoadTiersCollection(m_Acct, m_Date1, m_Date2, false);
			bool flag = clsCbEcritureComptable_Collection == null;
			checked
			{
				Imputation[] result;
				if (flag)
				{
					result = null;
				}
				else
				{
					List<Imputation> list = new List<Imputation>();
					int num = 0;
					try
					{
						foreach (clsCbEcritureComptable clsCbEcritureComptable in clsCbEcritureComptable_Collection)
						{
							bool flag2 = m_CatEcr == 0;
							if (flag2)
							{
								bool flag3 = clsCbEcritureComptable.EC_LETTRE == 0;
								if (flag3)
								{
									num++;
									bool flag4 = num <= this._DOCMAXRET;
									if (flag4)
									{
										list.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(clsCbEcritureComptable));
									}
									else
									{
										this.m_ImputationBag.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(clsCbEcritureComptable));
									}
								}
							}
							else
							{
								bool flag5 = m_CatEcr == 1;
								if (flag5)
								{
									bool flag6 = clsCbEcritureComptable.EC_LETTRE == 0 & clsCbEcritureComptable.EC_POINT == 1;
									if (flag6)
									{
										num++;
										bool flag7 = num <= this._DOCMAXRET;
										if (flag7)
										{
											list.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(clsCbEcritureComptable));
										}
										else
										{
											this.m_ImputationBag.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(clsCbEcritureComptable));
										}
									}
								}
								else
								{
									bool flag8 = m_CatEcr == 2;
									if (flag8)
									{
										num++;
										bool flag9 = num <= this._DOCMAXRET;
										if (flag9)
										{
											list.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(clsCbEcritureComptable));
										}
										else
										{
											this.m_ImputationBag.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(clsCbEcritureComptable));
										}
									}
								}
							}
						}
					}
					finally
					{
						IEnumerator<clsCbEcritureComptable> enumerator;
						if (enumerator != null)
						{
							enumerator.Dispose();
						}
					}
					result = list.ToArray();
				}
				return result;
			}
		}
		public Imputation[] InterrogerBKCompteTiers(string m_bkAcct, DateTime m_Date1, DateTime m_Date2, int m_CatEcr)
		{
			this.m_ImputationBag = new List<Imputation>();
			clsCbEcritureComptable_Collection clsCbEcritureComptable_Collection = this.m_oCat.ECRITURE_LoadCBTiersCollection(m_bkAcct, m_Date1, m_Date2, false);
			bool flag = clsCbEcritureComptable_Collection == null;
			checked
			{
				Imputation[] result;
				if (flag)
				{
					result = null;
				}
				else
				{
					List<Imputation> list = new List<Imputation>();
					int num = 0;
					try
					{
						foreach (clsCbEcritureComptable clsCbEcritureComptable in clsCbEcritureComptable_Collection)
						{
							bool flag2 = m_CatEcr == 0;
							if (flag2)
							{
								bool flag3 = clsCbEcritureComptable.EC_LETTRE == 0;
								if (flag3)
								{
									num++;
									bool flag4 = num <= this._DOCMAXRET;
									if (flag4)
									{
										list.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(clsCbEcritureComptable));
									}
									else
									{
										this.m_ImputationBag.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(clsCbEcritureComptable));
									}
								}
							}
							else
							{
								bool flag5 = m_CatEcr == 1;
								if (flag5)
								{
									bool flag6 = clsCbEcritureComptable.EC_LETTRE == 0 & clsCbEcritureComptable.EC_POINT == 1;
									if (flag6)
									{
										num++;
										bool flag7 = num <= this._DOCMAXRET;
										if (flag7)
										{
											list.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(clsCbEcritureComptable));
										}
										else
										{
											this.m_ImputationBag.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(clsCbEcritureComptable));
										}
									}
								}
							}
						}
					}
					finally
					{
						IEnumerator<clsCbEcritureComptable> enumerator;
						if (enumerator != null)
						{
							enumerator.Dispose();
						}
					}
					result = list.ToArray();
				}
				return result;
			}
		}
		public Imputation[] InterrogerCompteGerneral(string m_Acct, DateTime m_Date1, DateTime m_Date2, int m_CatEcr)
		{
			this.m_ImputationBag = new List<Imputation>();
			clsCbEcritureComptable_Collection clsCbEcritureComptable_Collection = this.m_oCat.ECRITURE_COMPTELoadCollection(m_Acct, m_Date1, m_Date2, false);
			bool flag = clsCbEcritureComptable_Collection == null;
			checked
			{
				Imputation[] result;
				if (flag)
				{
					result = null;
				}
				else
				{
					List<Imputation> list = new List<Imputation>();
					int num = 0;
					try
					{
						foreach (clsCbEcritureComptable oItem in clsCbEcritureComptable_Collection)
						{
							num++;
							bool flag2 = num <= this._DOCMAXRET;
							if (flag2)
							{
								list.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(oItem));
							}
							else
							{
								this.m_ImputationBag.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(oItem));
							}
						}
					}
					finally
					{
						IEnumerator<clsCbEcritureComptable> enumerator;
						if (enumerator != null)
						{
							enumerator.Dispose();
						}
					}
					result = list.ToArray();
				}
				return result;
			}
		}
		public ImputationTransmission IMPUTATIONTRANSMISSIOM_LoadItem(string m_id)
		{
			ImputationTransmission result;
			return result;
		}
		private void EndEcrLaod(IAsyncResult ar)
		{
			bool flag = this.fnLoadEcr != null;
			if (flag)
			{
				clsCbEcritureComptable_Collection clsCbEcritureComptable_Collection = this.fnLoadEcr.EndInvoke(ar);
				bool flag2 = clsCbEcritureComptable_Collection != null;
				if (flag2)
				{
					Dictionary<string, CompteGeneral> dictionary = new Dictionary<string, CompteGeneral>();
					ImputationTransmission imputationTransmission = new ImputationTransmission();
					List<Imputation> list = new List<Imputation>();
					this.m_ImputationBag = new List<Imputation>();
					try
					{
						foreach (clsCbEcritureComptable oItem in clsCbEcritureComptable_Collection)
						{
							this.m_ImputationBag.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(oItem));
						}
					}
					finally
					{
						IEnumerator<clsCbEcritureComptable> enumerator;
						if (enumerator != null)
						{
							enumerator.Dispose();
						}
					}
					this.bFnExecuting = false;
				}
			}
		}
		public ImputationTransmission IMPUTATIONTRANSMISSIOM_JnalLookup(string m_CodeJnal, string srch, DateTime m_date1, DateTime m_date2)
		{
			this.m_EventLog.WriteEntry(string.Format("# {4} Requete journal {0} du {1:d} au {2:d} filtre {3}", new object[]
			{
				m_CodeJnal,
				m_date1,
				m_date2,
				srch,
				DateAndTime.Now
			}));
			ImputationTransmission imputationTransmission = new ImputationTransmission();
			bool flag = this.bFnExecuting;
			ImputationTransmission result;
			if (flag)
			{
				this.m_EventLog.WriteEntry(string.Format("=> En attente du resultat. ", new object[0]));
				result = new ImputationTransmission
				{
					Description = "Waiting for execution"
				};
			}
			else
			{
				bool flag2 = this.m_ImputationBag != null && this.m_ImputationBag.Count > 0;
				if (flag2)
				{
					Imputation[] collection = this.IMPUTATION_BagFetch();
					this.m_EventLog.WriteEntry(string.Format("Requete terminee. {0} enregistrememts ", this.m_ImputationBag.Count));
					imputationTransmission = new ImputationTransmission();
					imputationTransmission.Imputations.AddRange(collection);
					result = imputationTransmission;
				}
				else
				{
					this.m_EventLog.WriteEntry(string.Format("# {0} calcul intetrrogation ", DateAndTime.Now));
					bool flag3 = !this.CheckDSLink();
					if (flag3)
					{
						result = null;
					}
					else
					{
						SageNetServices $VB$NonLocal_2 = this.m_oCat;
						this.fnLoadEcr = ((string a0, string a1, DateTime a2, DateTime a3) => $VB$NonLocal_2.ECRITURE_LoadCollection(a0, a1, a2, a3, false, 0));
						this.clbkLaodEcr = new AsyncCallback(this.EndEcrLaod);
						this.fnLoadEcr.BeginInvoke(m_CodeJnal, srch, m_date1, m_date2, this.clbkLaodEcr, this.cbEcrObj);
						this.bFnExecuting = true;
						imputationTransmission = new ImputationTransmission
						{
							Description = "Waiting for execution"
						};
						result = imputationTransmission;
					}
				}
			}
			return result;
		}
		private int __ImputationTransmissionloadJnal(string m_CodeJnal, DateTime m_date1, DateTime m_date2)
		{
			bool flag = !this.CheckDSLink();
			checked
			{
				int result;
				if (flag)
				{
					this.__bImputationLoadComplete = true;
					this.__bLoadErrStat = false;
					result = 0;
				}
				else
				{
					this.__bLoadErrStat = true;
					clsCbEcritureComptable_Collection clsCbEcritureComptable_Collection = this.m_oCat.ECRITURE_LoadCollection(m_CodeJnal, m_date1, m_date2, false, "");
					bool flag2 = clsCbEcritureComptable_Collection == null;
					if (flag2)
					{
						this.__bImputationLoadComplete = true;
						this.__bLoadErrStat = false;
						result = 0;
					}
					else
					{
						Dictionary<string, CompteGeneral> dictionary = new Dictionary<string, CompteGeneral>();
						this.__imputationTransResults = new ImputationTransmission();
						int num = 0;
						List<Imputation> list = new List<Imputation>();
						this.m_ImputationBag = new List<Imputation>();
						try
						{
							foreach (clsCbEcritureComptable clsCbEcritureComptable in clsCbEcritureComptable_Collection)
							{
								CompteGeneral compteGeneral = this.m_oCat.COMPTEG_LoadItem(clsCbEcritureComptable.CG_NUM);
								bool flag3 = compteGeneral != null;
								if (flag3)
								{
									bool flag4 = !dictionary.ContainsKey(clsCbEcritureComptable.CG_NUM);
									if (flag4)
									{
										dictionary.Add(clsCbEcritureComptable.CG_NUM, this.CONVERT_CompteGeneral_2_CompteGeneral(compteGeneral));
										this.__imputationTransResults.ComptesGeneraux.Add(this.CONVERT_CompteGeneral_2_CompteGeneral(compteGeneral));
									}
								}
								Dictionary<string, TiersComptable> dictionary2 = new Dictionary<string, TiersComptable>();
								clsClient clsClient = this.m_oCat.TIERS_LoadItem(clsCbEcritureComptable.CT_NUM, CB_TYPE_TIERS.CB_TYPE_TIERS_CLIENT);
								bool flag5 = clsClient != null;
								if (flag5)
								{
									bool flag6 = !dictionary2.ContainsKey(clsCbEcritureComptable.CT_NUM.Trim());
									if (flag6)
									{
										dictionary2.Add(clsCbEcritureComptable.CT_NUM.Trim(), this.CONVERT_clsCLIENTS_2_TiersComptable(clsClient));
										this.__imputationTransResults.ComptesTiers.Add(this.CONVERT_clsCLIENTS_2_TiersComptable(clsClient));
									}
								}
								num++;
								this.__imputationTransResults.SectionAnalytique = this.m_SectionAnalityt;
								bool flag7 = num <= this._DOCMAXRET;
								if (flag7)
								{
									list.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(clsCbEcritureComptable));
								}
								else
								{
									this.m_ImputationBag.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(clsCbEcritureComptable));
								}
							}
						}
						finally
						{
							IEnumerator<clsCbEcritureComptable> enumerator;
							if (enumerator != null)
							{
								enumerator.Dispose();
							}
						}
						this.__imputationTransResults.Imputations = list;
						this.__bImputationLoadComplete = true;
						this.__bLoadErrStat = false;
						result = this.__imputationTransResults.Imputations.Count;
					}
				}
				return result;
			}
		}
		private void __endLoadImputationTransmissionJnal(IAsyncResult ar)
		{
			this.__bImputationLoading = false;
			bool isCompleted = ar.IsCompleted;
			if (isCompleted)
			{
				int num = this._fnLoadJnal.EndInvoke(ar);
				bool flag = num > 0;
				if (flag)
				{
				}
				this.__bLoadErrStat = false;
			}
		}
		public ImputationTransmission IMPUTATIONTRANSMISSIOM_LoadJnal(string m_CodeJnal, DateTime m_date1, DateTime m_date2)
		{
			bool _bImputationLoading = this.__bImputationLoading;
			ImputationTransmission result;
			if (_bImputationLoading)
			{
				ImputationTransmission imputationTransmission = new ImputationTransmission
				{
					Description = "Waiting for result"
				};
				result = imputationTransmission;
			}
			else
			{
				bool flag = !this.__bImputationLoading && this.__bImputationLoadComplete;
				if (flag)
				{
					this.__bImputationLoading = false;
					this.__bImputationLoadComplete = false;
					this.__bLoadErrStat = false;
					result = this.__imputationTransResults;
				}
				else
				{
					bool flag2 = !this.__bImputationLoading && this.__bLoadErrStat;
					if (flag2)
					{
						this.__bImputationLoading = false;
						this.__bImputationLoadComplete = false;
						this.__bLoadErrStat = false;
						result = null;
					}
					else
					{
						this.__imputationTransResults = null;
						this.__bImputationLoadComplete = false;
						this._fnLoadJnal = new SageAccService.dlgt_ImputationLoadJnal(this.__ImputationTransmissionloadJnal);
						this._clbkLoadJnal = new AsyncCallback(this.__endLoadImputationTransmissionJnal);
						this._fnLoadJnal.BeginInvoke(m_CodeJnal, m_date1, m_date2, this._clbkLoadJnal, this._loadJnalBag);
						this.__bImputationLoading = true;
						ImputationTransmission imputationTransmission2 = new ImputationTransmission
						{
							Description = "Waiting for result"
						};
						result = imputationTransmission2;
					}
				}
			}
			return result;
		}
		private void _resetAccLogs()
		{
			this._accjobLog = accJobLog.CreateLog();
		}
		public ImputationTransmission IMPUTATIONTRANSMISSION_Build2(Imputation[] m_List)
		{
			this._resetAccLogs();
			bool flag = m_List == null;
			ImputationTransmission result;
			if (flag)
			{
				result = null;
			}
			else
			{
				this._accjobLog.AppendLine(string.Format("Buiding Transmission for {0} imputations...", m_List.GetLength(0)), "IMPUTATIONTRANSMISSION_Build2", 1, true);
				bool flag2 = !this.CheckDSLink();
				if (flag2)
				{
					this._accjobLog.AppendLine("DS Link Unavailable.", "IMPUTATIONTRANSMISSION_Build2", 1, true);
					ImputationTransmission imputationTransmission = new ImputationTransmission();
					imputationTransmission.Imputations.AddRange(m_List);
					result = imputationTransmission;
				}
				else
				{
					Dictionary<string, CompteGeneral> dictionary = new Dictionary<string, CompteGeneral>();
					Dictionary<string, TiersComptable> dictionary2 = new Dictionary<string, TiersComptable>();
					ImputationTransmission imputationTransmission = new ImputationTransmission();
					this._accjobLog.AppendLine("Checking G And T  Accounts ...", "IMPUTATIONTRANSMISSION_Build2", 1, true);
					foreach (Imputation imputation in m_List)
					{
						this._accjobLog.AppendLine(string.Format("Checking CG {0} ...{0}", imputation.CompteGeneral), "IMPUTATIONTRANSMISSION_Build2", 1, true);
						CompteGeneral compteGeneral = this.m_oCat.COMPTEG_LoadItem(imputation.CompteGeneral);
						bool flag3 = compteGeneral != null;
						if (flag3)
						{
							bool flag4 = !dictionary.ContainsKey(imputation.CompteGeneral);
							if (flag4)
							{
								dictionary.Add(imputation.CompteGeneral, this.CONVERT_CompteGeneral_2_CompteGeneral(compteGeneral));
								imputationTransmission.ComptesGeneraux.Add(this.CONVERT_CompteGeneral_2_CompteGeneral(compteGeneral));
							}
						}
						bool flag5 = !string.IsNullOrEmpty(imputation.CompteTiers);
						if (flag5)
						{
							string text = imputation.CompteTiers.Trim();
							this._accjobLog.AppendLine(string.Format("Checking CT {0} ...{0}", text), "IMPUTATIONTRANSMISSION_Build2", 1, true);
							clsClient clsClient = this.m_oCat.TIERS_LoadItem(text, CB_TYPE_TIERS.CB_TYPE_TIERS_CLIENT);
							bool flag6 = clsClient != null;
							if (flag6)
							{
								bool flag7 = !dictionary2.ContainsKey(text);
								if (flag7)
								{
									dictionary2.Add(text, this.CONVERT_clsCLIENTS_2_TiersComptable(clsClient));
									imputationTransmission.ComptesTiers.Add(this.CONVERT_clsCLIENTS_2_TiersComptable(clsClient));
								}
							}
						}
						this._accjobLog.AppendLine("adding Imp ...", "IMPUTATIONTRANSMISSION_Build2", 1, true);
						imputationTransmission.SectionAnalytique = this.m_SectionAnalityt;
						imputationTransmission.Imputations.Add(imputation);
					}
					this._accjobLog.AppendLine(string.Format("Transmission built {0} G-Accounts,{1} T-Accounts {2} Imputations {3}", new object[]
					{
						imputationTransmission.ComptesGeneraux.Count,
						imputationTransmission.ComptesTiers.Count,
						imputationTransmission.Imputations.Count,
						"\r\n"
					}), "IMPUTATIONTRANSMISSION_Build2", 1, true);
					result = imputationTransmission;
				}
			}
			return result;
		}
		List<Imputation> IAccService.CreateImputation(EnteteDocument m_Vente)
		{
			bool flag = !this.CheckDSLink();
			List<Imputation> result;
			if (flag)
			{
				result = null;
			}
			else
			{
				List<Imputation> list = new List<Imputation>();
				Imputation imputation = new Imputation();
				Imputation imputation2 = imputation;
				imputation2.CompteGeneral = this.m_CPTASales.CompteVente;
				imputation2.CompteContrePartie = this.m_CPTASales.CompteGenClient;
				imputation2.CompteTiers = this.m_CPTASales.TiersClient;
				imputation2.JournalComptable = this.m_CPTASales.JournalVente;
				imputation2.MontantImputation = m_Vente.MONTANT_HT;
				imputation2.SensImputation = 1;
				imputation2.LibelleImputation = "Vente " + m_Vente.NODOC;
				imputation2.NoPiece = m_Vente.NODOC;
				list.Add(imputation);
				imputation = new Imputation();
				Imputation imputation3 = imputation;
				imputation3.CompteGeneral = this.m_CPTASales.CompteGeneralTVA;
				imputation3.CompteContrePartie = this.m_CPTASales.CompteGenClient;
				imputation3.JournalComptable = this.m_CPTASales.JournalVente;
				imputation3.MontantImputation = m_Vente.MONTANT_TVA;
				imputation3.SensImputation = 1;
				imputation3.LibelleImputation = "TVA sur Vente " + m_Vente.NODOC;
				imputation3.NoPiece = m_Vente.NODOC;
				list.Add(imputation);
				imputation = new Imputation();
				Imputation imputation4 = imputation;
				imputation4.CompteGeneral = this.m_CPTASales.CompteGenClient;
				imputation4.JournalComptable = this.m_CPTASales.JournalVente;
				imputation4.MontantImputation = m_Vente.MONTANT_TTC;
				imputation4.SensImputation = 2;
				imputation4.LibelleImputation = "Ventes  " + m_Vente.NODOC;
				imputation4.NoPiece = m_Vente.NODOC;
				list.Add(imputation);
				result = list;
			}
			return result;
		}
		public bool ImputerVente(EnteteDocument m_Vente)
		{
			bool flag = !this.CheckDSLink();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				List<Imputation> list = this.CreateImputation(m_Vente);
				ImputationTransmission imputationTransmission = this.IMPUTATIONTRANSMISSION_Build(list);
				int num = 0;
				bool flag2 = imputationTransmission != null;
				if (flag2)
				{
					num = this.IMPUTATIONTRANSMISSIOM_CreateItem(imputationTransmission);
				}
				result = (num > 0);
			}
			return result;
		}
		public ImputationTransmission IMPUTATIONTRANSMISSION_Build(List<Imputation> m_List)
		{
			bool flag = !this.CheckDSLink();
			ImputationTransmission result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = m_List == null;
				if (flag2)
				{
					result = null;
				}
				else
				{
					Dictionary<string, CompteGeneral> dictionary = new Dictionary<string, CompteGeneral>();
					ImputationTransmission imputationTransmission = new ImputationTransmission();
					try
					{
						foreach (Imputation imputation in m_List)
						{
							CompteGeneral compteGeneral = this.m_oCat.COMPTEG_LoadItem(imputation.CompteGeneral);
							bool flag3 = compteGeneral != null;
							if (flag3)
							{
								bool flag4 = !dictionary.ContainsKey(imputation.CompteGeneral);
								if (flag4)
								{
									dictionary.Add(imputation.CompteGeneral, this.CONVERT_CompteGeneral_2_CompteGeneral(compteGeneral));
									imputationTransmission.ComptesGeneraux.Add(this.CONVERT_CompteGeneral_2_CompteGeneral(compteGeneral));
								}
							}
							Dictionary<string, TiersComptable> dictionary2 = new Dictionary<string, TiersComptable>();
							clsClient clsClient = this.m_oCat.TIERS_LoadItem(imputation.CompteTiers.Trim(), CB_TYPE_TIERS.CB_TYPE_TIERS_CLIENT);
							bool flag5 = clsClient != null;
							if (flag5)
							{
								bool flag6 = !dictionary2.ContainsKey(imputation.CompteTiers.Trim());
								if (flag6)
								{
									dictionary2.Add(imputation.CompteTiers.Trim(), this.CONVERT_clsCLIENTS_2_TiersComptable(clsClient));
									imputationTransmission.ComptesTiers.Add(this.CONVERT_clsCLIENTS_2_TiersComptable(clsClient));
								}
							}
							imputationTransmission.SectionAnalytique = this.m_SectionAnalityt;
							imputationTransmission.Imputations.Add(imputation);
						}
					}
					finally
					{
						List<Imputation>.Enumerator enumerator;
						((IDisposable)enumerator).Dispose();
					}
					result = imputationTransmission;
				}
			}
			return result;
		}
		public void IMPUTATIONTRANSMISSIOM_DeleteItem(ImputationTransmission m_Transmission)
		{
		}
		private acAction __getSuccessLog(string key)
		{
			bool flag = this._sessionLoggerAvailable();
			if (flag)
			{
				bool flag2 = this.__actions == null;
				if (flag2)
				{
					this.__actions = this._sessionLogger.ACACTION_LoadCollection();
				}
				bool flag3 = this.__actions != null;
				if (flag3)
				{
					acAction acAction = this.__actions.First((acAction m) => string.Equals(m.ActionKey, key, StringComparison.Ordinal) & m.ActionStatus);
					bool flag4 = acAction != null;
					if (flag4)
					{
						return acAction;
					}
				}
			}
			return null;
		}
		public int IMPUTATIONTRANSMISSIOM_CreateItem(ImputationTransmission m_ItemObject)
		{
			this._resetLogs();
			bool flag = !this.CheckDSLink();
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = m_ItemObject.ComptesGeneraux != null;
				if (flag2)
				{
					this.m_EventLog.WriteEntry("TRANSMISSION IMPUTATIONs: COMPTES GENERAUX ->" + m_ItemObject.ComptesGeneraux.Count.ToString());
					bool flag3 = m_ItemObject.ComptesGeneraux != null;
					if (flag3)
					{
						try
						{
							foreach (CompteGeneral compteGeneral in m_ItemObject.ComptesGeneraux)
							{
								bool flag4 = this.COMPTEGEN_READ(compteGeneral.NumeroCompte) == null;
								if (flag4)
								{
									this.COMPTEGEN_WRITE(compteGeneral);
								}
							}
						}
						finally
						{
							List<CompteGeneral>.Enumerator enumerator;
							((IDisposable)enumerator).Dispose();
						}
					}
				}
				bool flag5 = m_ItemObject.ComptesTiers != null;
				if (flag5)
				{
					this.m_EventLog.WriteEntry("TRANSMISSION IMPUTATIONs: COMPTES TIERS ->" + m_ItemObject.ComptesTiers.Count.ToString());
					bool flag6 = m_ItemObject.ComptesTiers != null;
					if (flag6)
					{
						try
						{
							foreach (TiersComptable tiersComptable in m_ItemObject.ComptesTiers)
							{
								bool flag7 = this.TIERSCOMPTABLE_LoadItem(tiersComptable.CodeTiers, (int)tiersComptable.TypeTiers) == null;
								if (flag7)
								{
									this.TIERSCOMPTABLE_CreateItem(tiersComptable);
								}
							}
						}
						finally
						{
							List<TiersComptable>.Enumerator enumerator2;
							((IDisposable)enumerator2).Dispose();
						}
					}
				}
				bool flag8 = m_ItemObject.Imputations != null;
				if (flag8)
				{
					this.m_EventLog.WriteEntry("TRANSMISSION IMPUTATIONs: IMPUTATIONS ->" + m_ItemObject.Imputations.Count.ToString() + "  ...");
					clsCbEcritureComptable_Collection clsCbEcritureComptable_Collection = this.CONVERTCOLLECTION_Imputation_2_clsCbEcritureComptable(m_ItemObject.Imputations);
					bool flag9 = false;
					acAction acAction2;
					try
					{
						foreach (clsCbEcritureComptable clsCbEcritureComptable in clsCbEcritureComptable_Collection)
						{
							string key = string.Concat(
								clsCbEcritureComptable.JO_NUM.Trim(),
								"\\",
								clsCbEcritureComptable.CG_NUM.Trim(),
								"\\",
								clsCbEcritureComptable.EC_PIECE,
								"\\",
								clsCbEcritureComptable.EC_SENS.ToString(),
								"\\",
								clsCbEcritureComptable.EC_MONTANT.ToString()
							);
							acAction acAction = this.__getSuccessLog(key);
							bool flag10 = acAction != null;
							if (flag10)
							{
								flag9 = true;
								acAction2 = acAction;
								break;
							}
						}
					}
					finally
					{
						IEnumerator<clsCbEcritureComptable> enumerator3;
						if (enumerator3 != null)
						{
							enumerator3.Dispose();
						}
					}
					this.__actions = null;
					bool flag11 = flag9;
					if (flag11)
					{
						this._transmissionLog.AppendLine("Ecriture déja transféré session [" + Convert.ToString(acAction2.SessionID) + "]");
						result = -1;
					}
					else
					{
						bool flag12 = !string.Equals(m_ItemObject.SectionAnalytique, string.Empty, StringComparison.Ordinal);
						if (flag12)
						{
							try
							{
								foreach (clsCbEcritureComptable clsCbEcritureComptable2 in clsCbEcritureComptable_Collection)
								{
									bool flag13 = this.m_oCat.JOURNAL_VentilAnalyt(clsCbEcritureComptable2.JO_NUM);
									if (flag13)
									{
										clsCbEcritureComptable2.RepartAnalyt = new ECRITUREA_Collection();
										clsCbEcritureComptable2.RepartAnalyt.Add(0, 1, 0, m_ItemObject.SectionAnalytique, clsCbEcritureComptable2.EC_MONTANT, 0m, "ISI", 0);
									}
								}
							}
							finally
							{
								IEnumerator<clsCbEcritureComptable> enumerator4;
								if (enumerator4 != null)
								{
									enumerator4.Dispose();
								}
							}
						}
						this.m_EventLog.WriteEntry("TRANSMISSION IMPUTATIONS: DEBUT ECRITURE   ...");
						try
						{
							foreach (clsCbEcritureComptable clsCbEcritureComptable3 in clsCbEcritureComptable_Collection)
							{
								this.m_EventLog.WriteEntry(string.Concat(
									"TRANSMISSION IMPUTATIONS: ",
									clsCbEcritureComptable3.CT_NUM,
									":",
									clsCbEcritureComptable3.EC_INTITULE,
									"-->",
									clsCbEcritureComptable3.EC_MONTANT.ToString(),
									"   ..."
								));
								int num = this.m_oCat.ECRITURE_INSERT(clsCbEcritureComptable3);
								bool flag14 = this._sessionLoggerAvailable();
								if (flag14)
								{
									bool actionStatus = false;
									bool flag15 = this.m_oCat.ECRITURE_LoadItem(num) != null;
									if (flag15)
									{
										actionStatus = true;
									}
									string actionKey = string.Concat(
										clsCbEcritureComptable3.JO_NUM.Trim(),
										"\\",
										clsCbEcritureComptable3.CG_NUM.Trim(),
										"\\",
										clsCbEcritureComptable3.EC_PIECE,
										"\\",
										clsCbEcritureComptable3.EC_SENS.ToString(),
										"\\",
										clsCbEcritureComptable3.EC_MONTANT.ToString()
									);
									acAction it = new acAction
									{
										SessionID = this._sessionID,
										ActionPiece = clsCbEcritureComptable3.EC_PIECE,
										ActionType = "INSERT",
										ActionKey = actionKey,
										ActionStatus = actionStatus,
										ActionRetVal = num
									};
									this._sessionLogger.ACACTION_CreateItem(it);
								}
							}
						}
						finally
						{
							IEnumerator<clsCbEcritureComptable> enumerator5;
							if (enumerator5 != null)
							{
								enumerator5.Dispose();
							}
						}
						this.m_EventLog.WriteEntry("TRANSMISSION IMPUTATIONS: FIN ECRITURE.");
						result = m_ItemObject.Imputations.Count;
					}
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}
		public void IMPUTATIONTRANSMISSIOM_UpdateItem(ImputationTransmission m_ItemObject)
		{
		}
		public List<CompteGeneral> RecurseGeneralAccounts(string m_Jnal)
		{
			List<CompteGeneral> result;
			return result;
		}
		private CompteGeneral CONVERT_CompteGeneral_2_CompteGeneral(CompteGeneral m_oItem)
		{
			return checked(new CompteGeneral
			{
				CG_NUM = m_oItem.NumeroCompte,
				CG_INTITULE = m_oItem.IntituleCompte,
				CG_TYPE = (byte)m_oItem.TypeCompte,
				N_NATURE = (byte)m_oItem.NatureCompte
			});
		}
		private CompteGeneral CONVERT_CompteGeneral_2_CompteGeneral(CompteGeneral m_oItem)
		{
			return new CompteGeneral
			{
				NumeroCompte = m_oItem.CG_NUM,
				IntituleCompte = m_oItem.CG_INTITULE,
				TypeCompte = (int)m_oItem.CG_TYPE,
				NatureCompte = (NatureCompteEnum)m_oItem.N_NATURE
			};
		}
		private CompteGeneral_Collection CONVERTCOLLECTION_CompteGeneral_2_CompteGeneral(List<CompteGeneral> m_oItemCol)
		{
			CompteGeneral_Collection compteGeneral_Collection = new CompteGeneral_Collection();
			try
			{
				foreach (CompteGeneral oItem in m_oItemCol)
				{
					CompteGeneral item = this.CONVERT_CompteGeneral_2_CompteGeneral(oItem);
					compteGeneral_Collection.Add(item);
				}
			}
			finally
			{
				List<CompteGeneral>.Enumerator enumerator;
				((IDisposable)enumerator).Dispose();
			}
			return compteGeneral_Collection;
		}
		private List<CompteGeneral> CONVERTCOLLECTION_CompteGeneral_2_CompteGeneral(CompteGeneral_Collection m_oItemCol)
		{
			List<CompteGeneral> list = new List<CompteGeneral>();
			try
			{
				foreach (CompteGeneral oItem in m_oItemCol)
				{
					CompteGeneral item = this.CONVERT_CompteGeneral_2_CompteGeneral(oItem);
					list.Add(item);
				}
			}
			finally
			{
				IEnumerator<CompteGeneral> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
			return list;
		}
		private clsClient CONVERT_TiersComptable_2_clsCLIENTS(TiersComptable m_oItem)
		{
			return new clsClient
			{
				CT_NUM = m_oItem.CodeTiers,
				CT_INTITULE = m_oItem.NomTiers,
				CG_NUMPRINC = m_oItem.CompteGeneral,
				CT_TYPE = checked((byte)m_oItem.TypeTiers),
				CBCT_NUM = m_oItem.cbKey
			};
		}
		private TiersComptable CONVERT_clsCLIENTS_2_TiersComptable(clsClient m_oItem)
		{
			return new TiersComptable
			{
				CodeTiers = m_oItem.CT_NUM,
				NomTiers = m_oItem.CT_INTITULE,
				CompteGeneral = m_oItem.CG_NUMPRINC,
				TypeTiers = (TiersComptable.TYPE_TIERS_COMPTABLE)m_oItem.CT_TYPE,
				cbKey = m_oItem.CBCT_NUM
			};
		}
		private clsClient_Collection CONVERTCOLLECTION_TiersComptable_2_clsCLIENTS(List<TiersComptable> m_oItemCol)
		{
			clsClient_Collection clsClient_Collection = new clsClient_Collection();
			try
			{
				foreach (TiersComptable oItem in m_oItemCol)
				{
					clsClient oItem2 = this.CONVERT_TiersComptable_2_clsCLIENTS(oItem);
					clsClient_Collection.Add(oItem2);
				}
			}
			finally
			{
				List<TiersComptable>.Enumerator enumerator;
				((IDisposable)enumerator).Dispose();
			}
			return clsClient_Collection;
		}
		private List<TiersComptable> CONVERTCOLLECTION_clsCLIENTS_2_TiersComptable(clsClient_Collection m_oItemCol)
		{
			List<TiersComptable> list = new List<TiersComptable>();
			try
			{
				foreach (object obj in m_oItemCol)
				{
					clsClient oItem = (clsClient)obj;
					TiersComptable item = this.CONVERT_clsCLIENTS_2_TiersComptable(oItem);
					list.Add(item);
				}
			}
			finally
			{
				IEnumerator enumerator;
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			return list;
		}
		private clsCbEcritureComptable CONVERT_Imputation_2_clsCbEcritureComptable(Imputation m_oItem)
		{
			return checked(new clsCbEcritureComptable
			{
				EC_PIECE = m_oItem.NoPiece,
				JO_NUM = m_oItem.JournalComptable,
				EC_INTITULE = m_oItem.LibelleImputation,
				EC_SENS = (byte)m_oItem.SensImputation,
				EC_MONTANT = m_oItem.MontantImputation,
				EC_DATE = m_oItem.DateImputation,
				EC_JOUR = (byte)m_oItem.DateImputation.Day,
				JM_DATE = DateAndTime.DateSerial(m_oItem.DateImputation.Year, m_oItem.DateImputation.Month, 1),
				CG_NUM = m_oItem.CompteGeneral,
				CG_NUMCONT = m_oItem.CompteContrePartie,
				CT_NUM = m_oItem.CompteTiers,
				EC_NO = m_oItem.IdImputation
			});
		}
		private Imputation CONVERT_clsCbEcritureComptable_2_Imputation(clsCbEcritureComptable m_oItem)
		{
			Imputation imputation = new Imputation();
			imputation.NoPiece = m_oItem.EC_PIECE;
			imputation.JournalComptable = m_oItem.JO_NUM;
			imputation.LibelleImputation = m_oItem.EC_INTITULE;
			imputation.SensImputation = (int)m_oItem.EC_SENS;
			imputation.MontantImputation = m_oItem.EC_MONTANT;
			imputation.DateImputation = DateAndTime.DateSerial(m_oItem.JM_DATE.Year, m_oItem.JM_DATE.Month, (int)m_oItem.EC_JOUR);
			imputation.CompteGeneral = m_oItem.CG_NUM;
			imputation.CompteContrePartie = m_oItem.CG_NUMCONT;
			imputation.CompteTiers = m_oItem.CT_NUM;
			imputation.IdImputation = m_oItem.EC_NO;
			bool flag = m_oItem.EC_CLOTURE > 0;
			if (flag)
			{
				imputation.StatutImputation = STATUT_IMPUTATION.CLOTURE;
			}
			else
			{
				imputation.StatutImputation = STATUT_IMPUTATION.VALIDE;
			}
			return imputation;
		}
		private clsCbEcritureComptable_Collection CONVERTCOLLECTION_Imputation_2_clsCbEcritureComptable(List<Imputation> m_oItemCol)
		{
			clsCbEcritureComptable_Collection clsCbEcritureComptable_Collection = new clsCbEcritureComptable_Collection();
			try
			{
				foreach (Imputation oItem in m_oItemCol)
				{
					clsCbEcritureComptable item = this.CONVERT_Imputation_2_clsCbEcritureComptable(oItem);
					clsCbEcritureComptable_Collection.Add(item);
				}
			}
			finally
			{
				List<Imputation>.Enumerator enumerator;
				((IDisposable)enumerator).Dispose();
			}
			return clsCbEcritureComptable_Collection;
		}
		private List<Imputation> CONVERTCOLLECTION_clsCbEcritureComptable_2_Imputation(clsCbEcritureComptable_Collection m_oItemCol)
		{
			List<Imputation> list = new List<Imputation>();
			try
			{
				foreach (clsCbEcritureComptable oItem in m_oItemCol)
				{
					Imputation item = this.CONVERT_clsCbEcritureComptable_2_Imputation(oItem);
					list.Add(item);
				}
			}
			finally
			{
				IEnumerator<clsCbEcritureComptable> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
			return list;
		}
		public CompteGeneral COMPTEGEN_READ(string m_code)
		{
			CompteGeneral compteGeneral = this.m_oCat.COMPTEG_LoadItem(m_code);
			bool flag = compteGeneral == null;
			CompteGeneral result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = new CompteGeneral
				{
					IntituleCompte = compteGeneral.CG_INTITULE,
					NumeroCompte = compteGeneral.CG_NUM,
					NatureCompte = (NatureCompteEnum)compteGeneral.N_NATURE,
					TypeCompte = (int)compteGeneral.CG_TYPE
				};
			}
			return result;
		}
		public bool COMPTEGEN_WRITE(CompteGeneral m_ItemObject)
		{
			bool result = true;
			CompteGeneral it = this.CONVERT_CompteGeneral_2_CompteGeneral(m_ItemObject);
			this.m_oCat.COMPTEG_CreateItem(it);
			return result;
		}
		private int __createAcsAction(List<CompteGeneral> m_ItemObjects)
		{
			bool flag = false;
			int num = 0;
			checked
			{
				try
				{
					foreach (CompteGeneral oItem in m_ItemObjects)
					{
						try
						{
							CompteGeneral it = this.CONVERT_CompteGeneral_2_CompteGeneral(oItem);
							this.m_oCat.COMPTEG_CreateItem(it);
							num++;
						}
						catch (Exception ex)
						{
							flag = true;
						}
						bool flag2 = flag;
						if (flag2)
						{
							break;
						}
					}
				}
				finally
				{
					List<CompteGeneral>.Enumerator enumerator;
					((IDisposable)enumerator).Dispose();
				}
				return num;
			}
		}
		public jobResult COMPTEGEN_WRITEX(List<CompteGeneral> m_ItemObjects, string jobkey)
		{
			jobResult jobResult = new jobResult();
			bool flag = string.Equals(jobkey, string.Empty, StringComparison.Ordinal);
			if (flag)
			{
				jobResult.jobKey = Guid.NewGuid().ToString();
				this._tsk = Task.Run<int>(() => this.__createAcsAction(m_ItemObjects));
				jobResult.jobComplete = false;
				jobResult.jobStarted = true;
			}
			else
			{
				jobResult.jobKey = jobkey;
				bool flag2 = this._tsk != null;
				if (flag2)
				{
					bool isCompleted = this._tsk.IsCompleted;
					if (isCompleted)
					{
						jobResult.jobComplete = true;
						jobResult.jobIntegerValue = this._tsk.Result;
					}
					else
					{
						jobResult.jobComplete = false;
					}
				}
				else
				{
					jobResult.jobComplete = false;
					jobResult.jobTerminated = true;
				}
			}
			return jobResult;
		}
		public bool COMPTEGEN_DELETE(CompteGeneral m_ItemObject)
		{
			return true;
		}
		public bool COMPTEGEN_EDIT(CompteGeneral m_ItemObject)
		{
			CompteGeneral it = this.CONVERT_CompteGeneral_2_CompteGeneral(m_ItemObject);
			return this.m_oCat.COMPTEG_UpdateItem(it);
		}
		public TiersComptable[] TIERSCOMPTABLE_BagFetch()
		{
			bool flag = this.m_TiersBag.Count == 0;
			checked
			{
				TiersComptable[] result;
				if (flag)
				{
					result = null;
				}
				else
				{
					int num = 0;
					List<TiersComptable> list = new List<TiersComptable>();
					while (this.m_TiersBag.Count > 0)
					{
						bool flag2 = num <= this._DOCMAXRET;
						if (!flag2)
						{
							break;
						}
						num++;
						list.Add(this.m_TiersBag[0]);
						this.m_TiersBag.RemoveAt(0);
					}
					result = list.ToArray();
				}
				return result;
			}
		}
		public TiersComptable[] TIERSCOMPTABLE_LoadItems()
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			checked
			{
				TiersComptable[] result;
				if (flag2)
				{
					result = null;
				}
				else
				{
					clsClient_Collection clsClient_Collection = this.m_oCat.TIERS_LoadCollection();
					bool flag3 = clsClient_Collection == null;
					if (flag3)
					{
						result = null;
					}
					else
					{
						List<TiersComptable> list = new List<TiersComptable>();
						this.m_TiersBag = new List<TiersComptable>();
						int num = 0;
						try
						{
							foreach (object obj in clsClient_Collection)
							{
								clsClient oItem = (clsClient)obj;
								num++;
								bool flag4 = num <= this._MAXRET;
								if (flag4)
								{
									list.Add(this.CONVERT_clsCLIENTS_2_TiersComptable(oItem));
								}
								else
								{
									this.m_TiersBag.Add(this.CONVERT_clsCLIENTS_2_TiersComptable(oItem));
								}
							}
						}
						finally
						{
							IEnumerator enumerator;
							if (enumerator is IDisposable)
							{
								(enumerator as IDisposable).Dispose();
							}
						}
						TiersComptable[] array = list.ToArray();
						result = array;
					}
				}
				return result;
			}
		}
		public TiersComptable[] TIERSCOMPTABLE_LoadCbItems()
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			checked
			{
				TiersComptable[] result;
				if (flag2)
				{
					result = null;
				}
				else
				{
					clsClient_Collection clsClient_Collection = this.m_oCat.TIERS_LoadCBCollection();
					bool flag3 = clsClient_Collection == null;
					if (flag3)
					{
						result = null;
					}
					else
					{
						List<TiersComptable> list = new List<TiersComptable>();
						this.m_TiersBag = new List<TiersComptable>();
						int num = 0;
						try
						{
							foreach (object obj in clsClient_Collection)
							{
								clsClient oItem = (clsClient)obj;
								num++;
								bool flag4 = num <= this._MAXRET;
								if (flag4)
								{
									list.Add(this.CONVERT_clsCLIENTS_2_TiersComptable(oItem));
								}
								else
								{
									this.m_TiersBag.Add(this.CONVERT_clsCLIENTS_2_TiersComptable(oItem));
								}
							}
						}
						finally
						{
							IEnumerator enumerator;
							if (enumerator is IDisposable)
							{
								(enumerator as IDisposable).Dispose();
							}
						}
						TiersComptable[] array = list.ToArray();
						result = array;
					}
				}
				return result;
			}
		}
		public TiersComptable TIERSCOMPTABLE_LoadItem(string m_itID, int m_Type)
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			TiersComptable result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				clsClient clsClient = this.m_oCat.TIERS_LoadItem(m_itID, (CB_TYPE_TIERS)m_Type);
				bool flag3 = clsClient == null;
				if (flag3)
				{
					result = null;
				}
				else
				{
					TiersComptable tiersComptable = this.CONVERT_clsCLIENTS_2_TiersComptable(clsClient);
					result = tiersComptable;
				}
			}
			return result;
		}
		public bool TIERSCOMPTABLE_CreateItem(TiersComptable m_ItemObject)
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = Convert.IsDBNull(m_ItemObject.TypeTiers);
				if (flag3)
				{
					result = false;
				}
				else
				{
					clsClient clsClient = this.CONVERT_TiersComptable_2_clsCLIENTS(m_ItemObject);
					bool flag4 = !string.IsNullOrEmpty(clsClient.CT_CLASSEMENT) && clsClient.CT_CLASSEMENT.Length > 17;
					if (flag4)
					{
						clsClient.CT_CLASSEMENT = clsClient.CT_CLASSEMENT.Substring(1, 17);
					}
					clsClient.CT_SOMMEIL = 0;
					bool flag5 = this.m_oCat.TIERS_Create(clsClient);
					result = flag5;
				}
			}
			return result;
		}
		private int __createAcsAction(List<TiersComptable> m_ItemObjects)
		{
			bool flag = false;
			int num = 0;
			checked
			{
				try
				{
					foreach (TiersComptable oItem in m_ItemObjects)
					{
						try
						{
							clsClient tiers = this.CONVERT_TiersComptable_2_clsCLIENTS(oItem);
							this.m_oCat.TIERS_Create(tiers);
							num++;
						}
						catch (Exception ex)
						{
							flag = true;
						}
						bool flag2 = flag;
						if (flag2)
						{
							break;
						}
					}
				}
				finally
				{
					List<TiersComptable>.Enumerator enumerator;
					((IDisposable)enumerator).Dispose();
				}
				return num;
			}
		}
		public jobResult TIERSCOMPTABLE_CREATEITEMX(List<TiersComptable> m_ItemObjects, string jobkey)
		{
			jobResult jobResult = new jobResult();
			bool flag = string.Equals(jobkey, string.Empty, StringComparison.Ordinal);
			if (flag)
			{
				jobResult.jobKey = Guid.NewGuid().ToString();
				this._tsk = Task.Run<int>(() => this.__createAcsAction(m_ItemObjects));
				jobResult.jobComplete = false;
				jobResult.jobStarted = true;
			}
			else
			{
				jobResult.jobKey = jobkey;
				bool flag2 = this._tsk != null;
				if (flag2)
				{
					bool isCompleted = this._tsk.IsCompleted;
					if (isCompleted)
					{
						jobResult.jobComplete = true;
						jobResult.jobIntegerValue = this._tsk.Result;
					}
					else
					{
						jobResult.jobComplete = false;
					}
				}
				else
				{
					jobResult.jobComplete = false;
					jobResult.jobTerminated = true;
				}
			}
			return jobResult;
		}
		public bool TIERSCOMPTABLE_DeleteItem(TiersComptable m_ItemObject)
		{
			bool result = true;
			clsClient clsClient = this.CONVERT_TiersComptable_2_clsCLIENTS(m_ItemObject);
			return result;
		}
		public bool TIERSCOMPTABLE_UpdateItem(TiersComptable m_ItemObject)
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = Convert.IsDBNull(m_ItemObject.TypeTiers);
				clsClient clsClient;
				if (flag3)
				{
					clsClient = this.m_oCat.TIERS_LoadItem(m_ItemObject.CodeTiers, CB_TYPE_TIERS.CB_TYPE_TIERS_FOURNISSEUR);
				}
				else
				{
					clsClient = this.m_oCat.TIERS_LoadItem(m_ItemObject.CodeTiers, (CB_TYPE_TIERS)m_ItemObject.TypeTiers);
				}
				bool flag4 = clsClient != null;
				if (flag4)
				{
					clsClient.CT_TYPE = checked((byte)m_ItemObject.TypeTiers);
					clsClient.CT_INTITULE = m_ItemObject.NomTiers;
					bool flag5 = !string.IsNullOrEmpty(m_ItemObject.CompteGeneral);
					if (flag5)
					{
						clsClient.CG_NUMPRINC = m_ItemObject.CompteGeneral;
					}
				}
				this.m_oCat.TIERS_UpdateItem(clsClient);
				bool flag6 = true;
				result = flag6;
			}
			return result;
		}
		private JOURNAL CONVERT_Journal_2_JOURNAL(Journal m_oItem)
		{
			return new JOURNAL
			{
				JO_NUM = m_oItem.Code,
				JO_INTITULE = m_oItem.Intitule,
				JO_TYPE = checked((byte)m_oItem.Type),
				CG_NUM = m_oItem.CompteLie
			};
		}
		private Journal CONVERT_Journal_2_JOURNAL(JOURNAL m_oItem)
		{
			return new Journal
			{
				Code = m_oItem.JO_NUM,
				Intitule = m_oItem.JO_INTITULE,
				Type = (int)m_oItem.JO_TYPE,
				CompteLie = m_oItem.CG_NUM
			};
		}
		private JOURNAL_Collection CONVERTCOLLECTION_Journal_2_JOURNAL(List<Journal> m_oItemCol)
		{
			JOURNAL_Collection journal_Collection = new JOURNAL_Collection();
			try
			{
				foreach (Journal oItem in m_oItemCol)
				{
					JOURNAL item = this.CONVERT_Journal_2_JOURNAL(oItem);
					journal_Collection.Add(item);
				}
			}
			finally
			{
				List<Journal>.Enumerator enumerator;
				((IDisposable)enumerator).Dispose();
			}
			return journal_Collection;
		}
		private List<Journal> CONVERTCOLLECTION_Journal_2_JOURNAL(JOURNAL_Collection m_oItemCol)
		{
			List<Journal> list = new List<Journal>();
			try
			{
				foreach (JOURNAL oItem in m_oItemCol)
				{
					Journal item = this.CONVERT_Journal_2_JOURNAL(oItem);
					list.Add(item);
				}
			}
			finally
			{
				IEnumerator<JOURNAL> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
			return list;
		}
		public List<Journal> JOURNAL_LoadItems()
		{
			this._JobLog = new StringBuilder("Chargement journaux");
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			List<Journal> result;
			if (flag2)
			{
				this._JobLog.AppendLine("Pas de connexion à Sage [" + this.m_DossierSage + "]");
				result = null;
			}
			else
			{
				JOURNAL_Collection journal_Collection = this.m_oCat.JOURNAL_LoadCollection();
				bool flag3 = journal_Collection == null;
				if (flag3)
				{
					bool flag4 = this.m_oCat.ISDataLinkReady();
					if (flag4)
					{
						result = new List<Journal>();
					}
					else
					{
						result = null;
					}
				}
				else
				{
					List<Journal> list = new List<Journal>();
					try
					{
						foreach (JOURNAL oItem in journal_Collection)
						{
							list.Add(this.CONVERT_Journal_2_JOURNAL(oItem));
						}
					}
					finally
					{
						IEnumerator<JOURNAL> enumerator;
						if (enumerator != null)
						{
							enumerator.Dispose();
						}
					}
					result = list;
				}
			}
			return result;
		}
		public int JOURNAL_DeCloturer(string jnal, DateTime jour)
		{
			return this.m_oCat.ECRITURE_DeCloturer(jnal, jour);
		}
		public int JOURNAL_Cloturer(string jnal, DateTime jour)
		{
			return 0;
		}
		public Journal JOURNAL_LoadItem(string m_itID)
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			Journal result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				JOURNAL journal = this.m_oCat.JOURNAL_LoadItem(m_itID);
				bool flag3 = journal == null;
				if (flag3)
				{
					result = null;
				}
				else
				{
					Journal journal2 = this.CONVERT_Journal_2_JOURNAL(journal);
					result = journal2;
				}
			}
			return result;
		}
		public bool JOURNAL_CreateItem(Journal m_ItemObject)
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				JOURNAL journal = this.CONVERT_Journal_2_JOURNAL(m_ItemObject);
				journal.JO_SAISANAL = 1;
				bool flag3 = this.m_oCat.JOURNAL_CreateItem(journal);
				result = flag3;
			}
			return result;
		}
		public bool JOURNAL_DeleteItem(Journal m_ItemObject)
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				JOURNAL journal = this.CONVERT_Journal_2_JOURNAL(m_ItemObject);
				bool flag3 = this.m_oCat.JOURNAL_DELETEItem(journal.JO_NUM);
				result = flag3;
			}
			return result;
		}
		public bool JOURNAL_UpdateItem(Journal m_ItemObject)
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				JOURNAL it = this.CONVERT_Journal_2_JOURNAL(m_ItemObject);
				bool flag3 = this.m_oCat.JOURNAL_UpdateItem(it);
				result = flag3;
			}
			return result;
		}
		public LedgerMetrix JOURNAL_GetTFMetrix(string jnal, DateTime d1, DateTime d2)
		{
			LedgerMetrix ledgerMetrix = new LedgerMetrix();
			Tuple<int, int> tuple = this.m_oCat.JOURNAL_Ecrs(jnal, d1, d2);
			bool flag = tuple != null;
			if (flag)
			{
				ledgerMetrix.elementCount = tuple.Item2;
				ledgerMetrix.entryCount = tuple.Item1;
			}
			Tuple<decimal, decimal> tuple2 = this.m_oCat.JOURNAL_TTL(jnal, d1, d2);
			bool flag2 = tuple2 != null;
			if (flag2)
			{
				ledgerMetrix.DebitTotal = tuple2.Item1;
				ledgerMetrix.CreditTotal = tuple2.Item2;
			}
			return ledgerMetrix;
		}
		public List<SoldeGeneral> JOURNAL_GAccounts_Totals(string jnal, DateTime d1, DateTime d2)
		{
			List<Tuple<string, decimal, decimal>> list = this.m_oCat.JOURNAL_Accounts_TTls(jnal, d1, d2);
			bool flag = list != null;
			List<SoldeGeneral> result;
			if (flag)
			{
				List<SoldeGeneral> list2 = new List<SoldeGeneral>();
				try
				{
					foreach (Tuple<string, decimal, decimal> tuple in list)
					{
						list2.Add(new SoldeGeneral
						{
							NoCompte = tuple.Item1,
							MontantMouvementsDebiteurs = tuple.Item2,
							MontantMouvementsCrediteurs = tuple.Item3
						});
					}
				}
				finally
				{
					List<Tuple<string, decimal, decimal>>.Enumerator enumerator;
					((IDisposable)enumerator).Dispose();
				}
				result = list2;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public SageAccService(string svcUserLoginOrKey)
		{
			this.m_SiteVersion = "14";
			this._MAXRET = 300;
			this._DOCMAXRET = 100;
			this.m_ApplicationFamily = SageAccService.APPLICATION_FAMILY.SAGESQL;
			this.m_DossierSage = "test.gcm";
			this.m_Nodepot = 0;
			this.m_SectionAnalityt = "";
			this.m_LastComment = "Infosciences Accounts Mediation Server " + this.m_SiteVersion;
			this.bFnExecuting = false;
			this.__bImputationLoading = false;
			this.__bImputationLoadComplete = false;
			this.__bLoadErrStat = false;
			this.__imputationTransResults = null;
			this._tskValue = 0;
			this.m_TiersBag = new List<TiersComptable>();
			this.m_ImputationBag = new List<Imputation>();
			this.m_WriteLockedState = -1;
			this._IsInLogOnlyMode = false;
			this.Dossier_Courant = "";
			this.SecManager = new AuthManager();
			this._GLLGBag = new List<LigneGL>();
			this.__bGLLoading = false;
			this.__bGLLoadComplete = false;
			this.__bGLLoadErrStat = false;
			this._ObeapiUrl = "http://www.infosciences.net/obe";
			this._instanceUserKey = svcUserLoginOrKey;
		}
		public SageAccService()
		{
			this.m_SiteVersion = "14";
			this._MAXRET = 300;
			this._DOCMAXRET = 100;
			this.m_ApplicationFamily = SageAccService.APPLICATION_FAMILY.SAGESQL;
			this.m_DossierSage = "test.gcm";
			this.m_Nodepot = 0;
			this.m_SectionAnalityt = "";
			this.m_LastComment = "Infosciences Accounts Mediation Server " + this.m_SiteVersion;
			this.bFnExecuting = false;
			this.__bImputationLoading = false;
			this.__bImputationLoadComplete = false;
			this.__bLoadErrStat = false;
			this.__imputationTransResults = null;
			this._tskValue = 0;
			this.m_TiersBag = new List<TiersComptable>();
			this.m_ImputationBag = new List<Imputation>();
			this.m_WriteLockedState = -1;
			this._IsInLogOnlyMode = false;
			this.Dossier_Courant = "";
			this.SecManager = new AuthManager();
			this._GLLGBag = new List<LigneGL>();
			this.__bGLLoading = false;
			this.__bGLLoadComplete = false;
			this.__bGLLoadErrStat = false;
			this._ObeapiUrl = "http://www.infosciences.net/obe";
		}
		public Imputation[] IMPUTATION_BagFetch()
		{
			bool flag = this.m_ImputationBag.Count == 0;
			checked
			{
				Imputation[] result;
				if (flag)
				{
					result = null;
				}
				else
				{
					int num = 0;
					List<Imputation> list = new List<Imputation>();
					while (this.m_ImputationBag.Count > 0)
					{
						bool flag2 = num <= this._DOCMAXRET;
						if (!flag2)
						{
							break;
						}
						num++;
						list.Add(this.m_ImputationBag[0]);
						this.m_ImputationBag.RemoveAt(0);
					}
					result = list.ToArray();
				}
				return result;
			}
		}
		public Imputation[] IMPUTATION_LoadItems(string m_CodeJnal)
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			checked
			{
				Imputation[] result;
				if (flag2)
				{
					result = null;
				}
				else
				{
					Imputation[] array = null;
					DateTime date = DateAndTime.DateSerial(DateAndTime.Today.Year, DateAndTime.Today.Month, 1);
					DateTime date2 = DateAndTime.Today.Date;
					clsCbEcritureComptable_Collection clsCbEcritureComptable_Collection = this.m_oCat.ECRITURE_LoadCollection(m_CodeJnal, date, date2, false, "");
					bool flag3 = clsCbEcritureComptable_Collection != null;
					if (flag3)
					{
						array = (Imputation[])Array.CreateInstance(typeof(Imputation), clsCbEcritureComptable_Collection.Count);
						int num = 0;
						List<Imputation> list = new List<Imputation>();
						this.m_ImputationBag = new List<Imputation>();
						try
						{
							foreach (clsCbEcritureComptable oItem in clsCbEcritureComptable_Collection)
							{
								num++;
								bool flag4 = num <= this._DOCMAXRET;
								if (flag4)
								{
									list.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(oItem));
								}
								else
								{
									this.m_ImputationBag.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(oItem));
								}
							}
						}
						finally
						{
							IEnumerator<clsCbEcritureComptable> enumerator;
							if (enumerator != null)
							{
								enumerator.Dispose();
							}
						}
						array = list.ToArray();
					}
					result = array;
				}
				return result;
			}
		}
		public int IMPUTATION_SearchDelete(EcrMultiCritQuery qry)
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			int result;
			if (flag2)
			{
				result = -1;
			}
			else
			{
				CBEcSearchQry cbecSearchQry = new CBEcSearchQry();
				CBEcSearchQry cbecSearchQry2 = cbecSearchQry;
				cbecSearchQry2.Journee = qry.Journee;
				cbecSearchQry2.Journal = qry.Journal;
				cbecSearchQry2.Journee2 = qry.Journee2;
				cbecSearchQry2.Journee2Defined = qry.Journee2Defined;
				cbecSearchQry2.JourneeMatch = (CBDATEMATCH_MODE)qry.JourneeMatch;
				cbecSearchQry2.JourneeDefined = qry.JourneeDefined;
				cbecSearchQry2.CompteAux = qry.CompteAux;
				cbecSearchQry2.CompteGeneral = qry.CompteGeneral;
				cbecSearchQry2.CompteGeneralMatchExact = qry.CompteGeneralMatchExact;
				cbecSearchQry2.Createur = qry.Createur;
				cbecSearchQry2.CompteContrePartie = qry.CompteContrePartie;
				cbecSearchQry2.CompteContrepartieMatchExact = qry.CompteContrePartieMatchExact;
				bool sensEcritureDefined = qry.SensEcritureDefined;
				if (sensEcritureDefined)
				{
					cbecSearchQry2.SensImputation = (SENS_ECR)qry.SensEcriture;
				}
				int num = this.m_oCat.ECRITURE_SearchDelete(cbecSearchQry);
				result = num;
			}
			return result;
		}
		public Imputation[] IMPUTATION_SearchItems(EcrMultiCritQuery qry)
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			checked
			{
				Imputation[] result;
				if (flag2)
				{
					result = null;
				}
				else
				{
					Imputation[] array = null;
					CBEcSearchQry cbecSearchQry = new CBEcSearchQry();
					CBEcSearchQry cbecSearchQry2 = cbecSearchQry;
					cbecSearchQry2.Journee = qry.Journee;
					cbecSearchQry2.Journal = qry.Journal;
					cbecSearchQry2.Journee2 = qry.Journee2;
					cbecSearchQry2.Journee2Defined = qry.Journee2Defined;
					cbecSearchQry2.JourneeMatch = (CBDATEMATCH_MODE)qry.JourneeMatch;
					cbecSearchQry2.JourneeDefined = qry.JourneeDefined;
					cbecSearchQry2.CompteAux = qry.CompteAux;
					cbecSearchQry2.CompteGeneral = qry.CompteGeneral;
					cbecSearchQry2.CompteGeneralMatchExact = qry.CompteGeneralMatchExact;
					cbecSearchQry2.Createur = qry.Createur;
					cbecSearchQry2.CompteContrePartie = qry.CompteContrePartie;
					cbecSearchQry2.CompteContrepartieMatchExact = qry.CompteContrePartieMatchExact;
					bool sensEcritureDefined = qry.SensEcritureDefined;
					if (sensEcritureDefined)
					{
						cbecSearchQry2.SensImputation = (SENS_ECR)qry.SensEcriture;
					}
					clsCbEcritureComptable_Collection clsCbEcritureComptable_Collection = this.m_oCat.ECRITURE_Search(cbecSearchQry);
					bool flag3 = clsCbEcritureComptable_Collection != null;
					if (flag3)
					{
						array = (Imputation[])Array.CreateInstance(typeof(Imputation), clsCbEcritureComptable_Collection.Count);
						int num = 0;
						List<Imputation> list = new List<Imputation>();
						this.m_ImputationBag = new List<Imputation>();
						try
						{
							foreach (clsCbEcritureComptable oItem in clsCbEcritureComptable_Collection)
							{
								num++;
								bool flag4 = num <= this._DOCMAXRET;
								if (flag4)
								{
									list.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(oItem));
								}
								else
								{
									this.m_ImputationBag.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(oItem));
								}
							}
						}
						finally
						{
							IEnumerator<clsCbEcritureComptable> enumerator;
							if (enumerator != null)
							{
								enumerator.Dispose();
							}
						}
						array = list.ToArray();
					}
					result = array;
				}
				return result;
			}
		}
		public decimal[] IMPUTATION_LoadTimeFramedSummary(string m_CodeJnal, DateTime m_Date1, DateTime m_Date2)
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			decimal[] result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				decimal[] array = new decimal[3];
				clsCbEcritureComptable_Collection clsCbEcritureComptable_Collection = this.m_oCat.ECRITURE_LoadCollection(m_CodeJnal, m_Date1, m_Date2, false, "");
				bool flag3 = clsCbEcritureComptable_Collection != null;
				if (flag3)
				{
					array[0] = new decimal(clsCbEcritureComptable_Collection.Count);
					try
					{
						foreach (clsCbEcritureComptable clsCbEcritureComptable in clsCbEcritureComptable_Collection)
						{
							bool flag4 = clsCbEcritureComptable.EC_SENS == 0;
							if (flag4)
							{
								decimal[] array2 = array;
								int num = 1;
								ref decimal ptr = ref array2[num];
								array2[num] = decimal.Add(ptr, clsCbEcritureComptable.EC_MONTANT);
							}
							bool flag5 = clsCbEcritureComptable.EC_SENS == 1;
							if (flag5)
							{
								decimal[] array3 = array;
								int num2 = 2;
								ref decimal ptr = ref array3[num2];
								array3[num2] = decimal.Add(ptr, clsCbEcritureComptable.EC_MONTANT);
							}
						}
					}
					finally
					{
						IEnumerator<clsCbEcritureComptable> enumerator;
						if (enumerator != null)
						{
							enumerator.Dispose();
						}
					}
				}
				result = array;
			}
			return result;
		}
		public Imputation[] IMPUTATION_ItemsLookup(string m_CodeJnal, string Srch, DateTime m_Date1, DateTime m_Date2)
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			checked
			{
				Imputation[] result;
				if (flag2)
				{
					result = null;
				}
				else
				{
					Imputation[] array = null;
					clsCbEcritureComptable_Collection clsCbEcritureComptable_Collection = this.m_oCat.ECRITURE_LoadCollection(m_CodeJnal, Srch, m_Date1, m_Date2, false, 0);
					bool flag3 = clsCbEcritureComptable_Collection != null;
					if (flag3)
					{
						array = (Imputation[])Array.CreateInstance(typeof(Imputation), clsCbEcritureComptable_Collection.Count);
						int count = clsCbEcritureComptable_Collection.Count;
						for (int i = 1; i <= count; i++)
						{
							array[i - 1] = this.CONVERT_clsCbEcritureComptable_2_Imputation(clsCbEcritureComptable_Collection[i - 1]);
						}
					}
					result = array;
				}
				return result;
			}
		}
		public Imputation[] IMPUTATION_LoadTimeFramedItems(string m_CodeJnal, DateTime m_Date1, DateTime m_Date2)
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			checked
			{
				Imputation[] result;
				if (flag2)
				{
					result = null;
				}
				else
				{
					Imputation[] array = null;
					clsCbEcritureComptable_Collection clsCbEcritureComptable_Collection = this.m_oCat.ECRITURE_LoadCollection(m_CodeJnal, m_Date1, m_Date2, false, "");
					bool flag3 = clsCbEcritureComptable_Collection != null;
					if (flag3)
					{
						array = (Imputation[])Array.CreateInstance(typeof(Imputation), clsCbEcritureComptable_Collection.Count);
						int count = clsCbEcritureComptable_Collection.Count;
						for (int i = 1; i <= count; i++)
						{
							array[i - 1] = this.CONVERT_clsCbEcritureComptable_2_Imputation(clsCbEcritureComptable_Collection[i - 1]);
						}
					}
					result = array;
				}
				return result;
			}
		}
		public Imputation IMPUTATION_LoadItem(int m_EcNo)
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			Imputation result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				Imputation imputation = null;
				result = imputation;
			}
			return result;
		}
		private void _endImputationsCreation(IAsyncResult ar)
		{
			this._jobImputationResult.jobCompleteTime = DateAndTime.Now;
			int jobIntegerValue = -1;
			try
			{
				jobIntegerValue = this.fnImputationsCreationJob.EndInvoke(ar);
				this._jobImputationResult.jobComplete = true;
			}
			catch (Exception ex)
			{
				jobIntegerValue = -1;
				this._jobImputationResult.jobComplete = false;
			}
			finally
			{
				this._jobImputationResult.jobTerminated = true;
			}
			this._jobImputationResult.jobIntegerValue = jobIntegerValue;
		}
		public jobResult IMPUTATION_CreateItems(Imputation[] imps)
		{
			bool flag = imps == null;
			jobResult jobImputationResult;
			if (flag)
			{
				jobImputationResult = this._jobImputationResult;
			}
			else
			{
				string jobKey = "";
				this.fnImputationsCreationJob = new SageAccService.dlgt_ECRSJob(this._imputation_Creation_Job);
				this.clbkImpsCreation = new AsyncCallback(this._endImputationsCreation);
				this._jobImputationResult = new jobResult
				{
					jobStarted = true,
					jobTerminated = false,
					jobStartTime = DateAndTime.Now,
					jobComplete = false,
					jobKey = jobKey
				};
				this.fnImputationsCreationJob.BeginInvoke(imps, this.clbkImpsCreation, this.objImpCr);
				Thread.Sleep(5);
				jobImputationResult = this._jobImputationResult;
			}
			return jobImputationResult;
		}
		private int _imputation_Creation_Job(IEnumerable<Imputation> imps)
		{
			List<clsCbEcritureComptable> list = new List<clsCbEcritureComptable>();
			try
			{
				foreach (Imputation oItem in imps)
				{
					list.Add(this.CONVERT_Imputation_2_clsCbEcritureComptable(oItem));
				}
			}
			finally
			{
				IEnumerator<Imputation> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
			return this.m_oCat.InsererEcrituresComptables(list);
		}
		public int IMPUTATION_CreateItem(Imputation m_ItemObject)
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			int result;
			if (flag2)
			{
				result = 0;
			}
			else
			{
				bool flag3 = this.m_WriteLockedState == 1;
				if (flag3)
				{
					result = -1;
				}
				else
				{
					bool flag4 = this.m_WriteLockedState == -1;
					if (flag4)
					{
						this.m_WriteLockedState = 0;
						bool flag5 = this.m_oCat.CheckJnalWriteLock();
						if (flag5)
						{
							this.m_WriteLockedState = 1;
						}
					}
					bool flag6 = m_ItemObject == null;
					if (flag6)
					{
						result = 0;
					}
					else
					{
						clsCbEcritureComptable clsCbEcritureComptable = this.CONVERT_Imputation_2_clsCbEcritureComptable(m_ItemObject);
						string text = string.Concat(
							clsCbEcritureComptable.JO_NUM.Trim(),
							"\\",
							clsCbEcritureComptable.CG_NUM.Trim(),
							"\\",
							clsCbEcritureComptable.EC_PIECE,
							"\\",
							clsCbEcritureComptable.EC_SENS.ToString(),
							"\\",
							clsCbEcritureComptable.EC_MONTANT.ToString(),
							"\\",
							clsCbEcritureComptable.JM_DATE.GetHashCode().ToString(),
							"\\",
							clsCbEcritureComptable.EC_JOUR.ToString()
						);
						acAction acAction = this.__getSuccessLog(text);
						bool flag7 = acAction != null;
						if (flag7)
						{
							result = -2;
						}
						else
						{
							bool flag8 = m_ItemObject.StatutImputation == STATUT_IMPUTATION.POSTE;
							if (flag8)
							{
								bool flag9 = !this._IsInLogOnlyMode;
								if (flag9)
								{
									this.m_oCat.EnableInstantLogging(true);
									this._IsInLogOnlyMode = true;
								}
							}
							int num = this.m_oCat.ECRITURE_INSERT(clsCbEcritureComptable);
							bool flag10 = this._sessionLoggerAvailable();
							if (flag10)
							{
								bool actionStatus = false;
								bool flag11 = this.m_oCat.ECRITURE_LoadItem(num) != null;
								if (flag11)
								{
									actionStatus = true;
								}
								acAction it = new acAction
								{
									SessionID = this._sessionID,
									ActionPiece = clsCbEcritureComptable.EC_PIECE,
									ActionType = "INSERT",
									ActionKey = text,
									ActionStatus = actionStatus,
									ActionRetVal = num
								};
								this._sessionLogger.ACACTION_CreateItem(it);
							}
							else
							{
								bool flag12 = num == 0;
								if (flag12)
								{
								}
							}
							bool isInLogOnlyMode = this._IsInLogOnlyMode;
							if (isInLogOnlyMode)
							{
								result = 1;
							}
							else
							{
								result = num;
							}
						}
					}
				}
			}
			return result;
		}
		public bool IMPUTATION_DeleteItem(Imputation m_ItemObject)
		{
			bool flag = m_ItemObject.IdImputation > 0;
			checked
			{
				bool result;
				if (flag)
				{
					this.m_oCat.ECRITURE_DecloturerEc(m_ItemObject.IdImputation);
					result = this.m_oCat.ECRITURE_Delete(m_ItemObject.IdImputation);
				}
				else
				{
					clsCbEcritureComptable clsCbEcritureComptable = new clsCbEcritureComptable();
					clsCbEcritureComptable.CG_NUM = m_ItemObject.CompteGeneral;
					clsCbEcritureComptable.EC_PIECE = m_ItemObject.NoPiece;
					clsCbEcritureComptable.JO_NUM = m_ItemObject.JournalComptable;
					clsCbEcritureComptable.EC_MONTANT = m_ItemObject.MontantImputation;
					clsCbEcritureComptable.EC_JOUR = (byte)m_ItemObject.DateImputation.Day;
					clsCbEcritureComptable.JM_DATE = new DateTime(m_ItemObject.DateImputation.Year, m_ItemObject.DateImputation.Month, 1);
					clsCbEcritureComptable.EC_SENS = (byte)m_ItemObject.SensImputation;
					bool flag2 = this.m_oCat.ECRITURE_Delete(clsCbEcritureComptable);
					result = flag2;
				}
				return result;
			}
		}
		public bool Imputation_DeletePiece(int mExercice, string noPiece, string Jnal)
		{
			return this.m_oCat.ECRITURE_DeletePiece(mExercice, Jnal, noPiece);
		}
		public bool Imputation_DeletePieceTiers(int mExercice, string noPiece, string Jnal, string tiers)
		{
			return this.m_oCat.ECRITURE_DeletePieceCT(mExercice, Jnal, noPiece, tiers);
		}
		public bool Imputation_ReImputerPiece(string noPiece, string jnal, Imputation[] imputations)
		{
			bool flag = this.m_oCat == null;
			if (flag)
			{
				this.m_oCat = this.Connect();
			}
			bool flag2 = this.m_oCat == null;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = this.m_WriteLockedState == 1;
				if (flag3)
				{
					throw new Exception("Les journaux sont en utilisation. Re-prendre plus tard");
				}
				bool flag4 = this.m_WriteLockedState == -1;
				if (flag4)
				{
					this.m_WriteLockedState = 0;
					bool flag5 = this.m_oCat.CheckJnalWriteLock();
					if (flag5)
					{
						this.m_WriteLockedState = 1;
					}
				}
				int year = imputations[0].DateImputation.Year;
				bool flag6 = this.m_oCat.ECRITURE_DeletePiece(year, noPiece, jnal);
				if (flag6)
				{
					bool flag7 = true;
					foreach (Imputation oItem in imputations)
					{
						clsCbEcritureComptable ec = this.CONVERT_Imputation_2_clsCbEcritureComptable(oItem);
						int num = this.m_oCat.ECRITURE_INSERT(ec);
						bool flag8 = num == 0;
						if (flag8)
						{
							flag7 = false;
							break;
						}
					}
					result = flag7;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}
		public bool IMPUTATION_UpdateItem(Imputation m_ItemObject)
		{
			bool flag = m_ItemObject.IdImputation > 0;
			checked
			{
				bool result;
				if (flag)
				{
					clsCbEcritureComptable clsCbEcritureComptable = this.m_oCat.ECRITURE_LoadItem(m_ItemObject.IdImputation);
					bool flag2 = clsCbEcritureComptable != null;
					if (flag2)
					{
						clsCbEcritureComptable.CG_NUM = m_ItemObject.CompteGeneral;
						clsCbEcritureComptable.EC_PIECE = m_ItemObject.NoPiece;
						clsCbEcritureComptable.JO_NUM = m_ItemObject.JournalComptable;
						clsCbEcritureComptable.EC_MONTANT = m_ItemObject.MontantImputation;
						clsCbEcritureComptable.EC_JOUR = (byte)m_ItemObject.DateImputation.Day;
						clsCbEcritureComptable.JM_DATE = new DateTime(m_ItemObject.DateImputation.Year, m_ItemObject.DateImputation.Month, 1);
						clsCbEcritureComptable.EC_SENS = (byte)m_ItemObject.SensImputation;
						clsCbEcritureComptable.EC_NO = m_ItemObject.IdImputation;
						clsCbEcritureComptable.CT_NUM = m_ItemObject.CompteTiers;
						clsCbEcritureComptable.CG_NUMCONT = m_ItemObject.CompteContrePartie;
						clsCbEcritureComptable.EC_INTITULE = m_ItemObject.LibelleImputation;
						result = this.m_oCat.ECRITURE_Update(clsCbEcritureComptable);
					}
					else
					{
						bool flag3 = false;
						result = flag3;
					}
				}
				else
				{
					result = false;
				}
				return result;
			}
		}
		public bool IMPUTATION_UpdateItemAmount(int idEc, decimal Amt)
		{
			return this.m_oCat.ECRITURE_UpdateAmount(idEc, Amt);
		}
		public string Dossier_Courant { get; set; }
		public bool TryLinkServer(string m_NomDossier)
		{
			bool result;
			return result;
		}
		public bool TryLinkServer(string m_NomDossier, int noVersion)
		{
			bool result;
			return result;
		}
		public bool TryXLinkServer(string m_NomDossier)
		{
			this.m_DossierSage = m_NomDossier;
			this.Connect();
			bool flag = this.m_oCat != null;
			bool result;
			if (flag)
			{
				this.InitSecurity();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public bool TryXLinkServer(string m_NomDossier, int noVersion)
		{
			this.m_DossierSage = m_NomDossier;
			this.Connect();
			bool flag = this.m_oCat != null;
			bool result;
			if (flag)
			{
				this.m_oCat.CurreCBVersion = new decimal(noVersion);
				this.InitSecurity();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public string IMPUTATION_GetId(Imputation m_ItemObject)
		{
			bool flag = m_ItemObject == null;
			string result;
			if (flag)
			{
				result = Convert.ToString(0);
			}
			else
			{
				clsCbEcritureComptable ecr = this.CONVERT_Imputation_2_clsCbEcritureComptable(m_ItemObject);
				bool flag2 = this.m_oCat == null;
				if (flag2)
				{
					this.m_oCat = this.Connect();
				}
				bool flag3 = this.m_oCat == null;
				if (flag3)
				{
					result = "-1";
				}
				else
				{
					int value = this.m_oCat.ECRITURE_FindID(ecr);
					result = Convert.ToString(value);
				}
			}
			return result;
		}
		public bool IMPUTATION_DeleteID(string m_Id)
		{
			int num = 0;
			bool flag = false;
			bool flag2 = !int.TryParse(m_Id, out num);
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = this.m_oCat == null;
				if (flag3)
				{
					this.m_oCat = this.Connect();
				}
				bool flag4 = this._sessionLoggerAvailable();
				if (flag4)
				{
					clsCbEcritureComptable clsCbEcritureComptable = this.m_oCat.ECRITURE_LoadItem(num);
					bool flag5 = clsCbEcritureComptable != null;
					if (flag5)
					{
						flag = this.m_oCat.ECRITURE_Delete(num);
						acAction it = new acAction
						{
							SessionID = this._sessionID,
							ActionPiece = clsCbEcritureComptable.EC_PIECE,
							ActionType = "DELETE",
							ActionKey = string.Concat(
								clsCbEcritureComptable.JO_NUM.Trim(),
								"\\",
								clsCbEcritureComptable.CG_NUM.Trim(),
								"\\",
								clsCbEcritureComptable.EC_PIECE
							),
							ActionStatus = flag,
							ActionRetVal = num
						};
						this._sessionLogger.ACACTION_CreateItem(it);
					}
				}
				result = flag;
			}
			return result;
		}
		public List<CompteA> COMPTEA_LoadALL()
		{
			CbCodeAnalylique_Collection cbCodeAnalylique_Collection = this.m_oCat.CBCODEANALYTIQUE_LoadCollection();
			bool flag = cbCodeAnalylique_Collection != null;
			List<CompteA> result;
			if (flag)
			{
				List<CompteA> list = new List<CompteA>();
				try
				{
					foreach (CbCodeAnalylique cbCodeAnalylique in cbCodeAnalylique_Collection)
					{
						list.Add(new CompteA
						{
							Code = cbCodeAnalylique.JA_NUM,
							Intitule = cbCodeAnalylique.JA_INTITULE
						});
					}
				}
				finally
				{
					IEnumerator<CbCodeAnalylique> enumerator;
					if (enumerator != null)
					{
						enumerator.Dispose();
					}
				}
				result = list;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public bool COMPTEA_Write(CompteA ca)
		{
			CbCodeAnalylique it = new CbCodeAnalylique
			{
				JA_IFRS = 0,
				JA_INTITULE = ca.Intitule,
				JA_NUM = ca.Code
			};
			CbCodeAnalylique cbCodeAnalylique = this.m_oCat.CBCODEANALYTIQUE_LoadItem(ca.Code);
			bool flag = cbCodeAnalylique == null;
			bool result;
			if (flag)
			{
				result = this.m_oCat.CBCODEANALYTIQUE_CreateItem(it);
			}
			else
			{
				result = this.m_oCat.CBCODEANALYTIQUE_UpdateItem(it);
			}
			return result;
		}
		public bool COMPTEA_Delete(string c)
		{
			return this.m_oCat.CBCODEANALYTIQUE_DELETEItem(c);
		}
		public List<ImputationA> IMPUTATIONA_LoadItems(int ecNo)
		{
			ECRITUREA_Collection ecriturea_Collection = this.m_oCat.ECRITUREA_LoadEC_NOLinkedItems(ecNo);
			bool flag = ecriturea_Collection != null;
			List<ImputationA> result;
			if (flag)
			{
				List<ImputationA> list = new List<ImputationA>();
				try
				{
					foreach (ECRITUREA ecriturea in ecriturea_Collection)
					{
						list.Add(new ImputationA
						{
							CompteA = ecriturea.CODE_SECTION,
							ImpALigne = (int)ecriturea.LGN_ECA,
							ImpID = ecriturea.NO_ECR,
							Montant = ecriturea.MONTANT,
							ImpAQte = ecriturea.QUANTITE
						});
					}
				}
				finally
				{
					IEnumerator<ECRITUREA> enumerator;
					if (enumerator != null)
					{
						enumerator.Dispose();
					}
				}
				result = list;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public int IMPUTATIONA_WriteLines(int ecNo, List<ImputationA> lines)
		{
			clsCbEcritureComptable clsCbEcritureComptable = this.m_oCat.ECRITURE_LoadItem(ecNo);
			bool flag = clsCbEcritureComptable == null;
			checked
			{
				int result;
				if (flag)
				{
					result = 0;
				}
				else
				{
					int num = 0;
					decimal d = 0m;
					List<ECRITUREA> list = new List<ECRITUREA>();
					try
					{
						foreach (ImputationA imputationA in lines)
						{
							bool flag2 = imputationA != null;
							if (flag2)
							{
								num++;
								d = decimal.Add(d, imputationA.Montant);
								list.Add(new ECRITUREA
								{
									QUANTITE = imputationA.ImpAQte,
									NO_ECR = ecNo,
									MONTANT = imputationA.Montant,
									LGN_ECA = (byte)num,
									CODE_SECTION = imputationA.CompteA
								});
							}
						}
					}
					finally
					{
						List<ImputationA>.Enumerator enumerator;
						((IDisposable)enumerator).Dispose();
					}
					bool flag3 = decimal.Compare(d, clsCbEcritureComptable.EC_MONTANT) != 0;
					if (flag3)
					{
						result = 0;
					}
					else
					{
						try
						{
							foreach (ECRITUREA it in list)
							{
								this.m_oCat.ECRITUREA_CreateItem(it);
							}
						}
						finally
						{
							List<ECRITUREA>.Enumerator enumerator2;
							((IDisposable)enumerator2).Dispose();
						}
						result = list.Count;
					}
				}
				return result;
			}
		}
		public List<CompteGeneral> COMPTEGEN_GETITEMS(string m_Radic)
		{
			CompteGeneral_Collection compteGeneral_Collection = this.m_oCat.COMPTEG_LoadCollection();
			bool flag = compteGeneral_Collection == null;
			List<CompteGeneral> result;
			if (flag)
			{
				result = null;
			}
			else
			{
				List<CompteGeneral> list = new List<CompteGeneral>();
				try
				{
					foreach (CompteGeneral compteGeneral in compteGeneral_Collection)
					{
						list.Add(new CompteGeneral
						{
							IntituleCompte = compteGeneral.CG_INTITULE,
							NumeroCompte = compteGeneral.CG_NUM,
							NatureCompte = (NatureCompteEnum)compteGeneral.N_NATURE,
							TypeCompte = (int)compteGeneral.CG_TYPE
						});
					}
				}
				finally
				{
					IEnumerator<CompteGeneral> enumerator;
					if (enumerator != null)
					{
						enumerator.Dispose();
					}
				}
				result = list;
			}
			return result;
		}
		public CompteGeneral[] COMPTEGEN_GETTypeITEMS(int acType)
		{
			CompteGeneral_Collection compteGeneral_Collection = this.m_oCat.COMPTEG_LoadCG_TYPELinkedItems(checked((byte)acType));
			bool flag = compteGeneral_Collection == null;
			CompteGeneral[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				List<CompteGeneral> list = new List<CompteGeneral>();
				try
				{
					foreach (CompteGeneral compteGeneral in compteGeneral_Collection)
					{
						list.Add(new CompteGeneral
						{
							IntituleCompte = compteGeneral.CG_INTITULE,
							NumeroCompte = compteGeneral.CG_NUM
						});
					}
				}
				finally
				{
					IEnumerator<CompteGeneral> enumerator;
					if (enumerator != null)
					{
						enumerator.Dispose();
					}
				}
				result = list.ToArray();
			}
			return result;
		}
		private SoldeGeneral _SoldesGenerauxCompteT(string m_Acct, DateTime m_Date1, DateTime m_Date2, int m_CatEcr)
		{
			DataTable dataTable = this.m_oCat.ECRITURE_CalculerTotauxCompte(m_Date1, m_Date2, m_Acct, true, SENS_ECR.AUCUN);
			bool flag = dataTable == null;
			SoldeGeneral result;
			if (flag)
			{
				result = null;
			}
			else
			{
				SoldeGeneral soldeGeneral = new SoldeGeneral();
				DataRow[] array = dataTable.Select("EC_ANTYPE=1 And EC_SENS=0");
				bool flag2 = array != null && array.GetLength(0) >= 1;
				if (flag2)
				{
					soldeGeneral.ReportDebiteur = Convert.ToDecimal(array[0][0]);
				}
				DataRow[] array2 = dataTable.Select("EC_ANTYPE=1 And EC_SENS=1");
				bool flag3 = array2 != null && array2.GetLength(0) >= 1;
				if (flag3)
				{
					soldeGeneral.REportCrediteur = Convert.ToDecimal(array2[0][0]);
				}
				DataRow[] array3 = dataTable.Select("EC_ANTYPE=0 And EC_SENS=0");
				bool flag4 = array3 != null && array3.GetLength(0) >= 1;
				if (flag4)
				{
					soldeGeneral.MontantMouvementsDebiteurs = Convert.ToDecimal(array3[0][0]);
				}
				DataRow[] array4 = dataTable.Select("EC_ANTYPE=0 And EC_SENS=1");
				bool flag5 = array4 != null && array4.GetLength(0) >= 1;
				if (flag5)
				{
					soldeGeneral.MontantMouvementsCrediteurs = Convert.ToDecimal(array4[0][0]);
				}
				decimal num = decimal.Add(soldeGeneral.ReportDebiteur, soldeGeneral.MontantMouvementsDebiteurs);
				decimal num2 = decimal.Add(soldeGeneral.REportCrediteur, soldeGeneral.MontantMouvementsCrediteurs);
				bool flag6 = decimal.Compare(num, num2) > 0;
				if (flag6)
				{
					soldeGeneral.AReporterDebiteur = decimal.Subtract(num, num2);
				}
				else
				{
					soldeGeneral.AReporterCrediteur = decimal.Subtract(num2, num);
				}
				result = soldeGeneral;
			}
			return result;
		}
		private List<SoldeGeneral> _marshallResults(DataTable m_tb)
		{
			Dictionary<string, DataRow> dictionary = new Dictionary<string, DataRow>();
			try
			{
				foreach (object obj in m_tb.Rows)
				{
					DataRow dataRow = (DataRow)obj;
					bool flag = !dictionary.ContainsKey(Convert.ToString(dataRow[1]).Trim());
					if (flag)
					{
						dictionary.Add(Convert.ToString(dataRow[1]).Trim(), dataRow);
					}
				}
			}
			finally
			{
				IEnumerator enumerator;
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			List<SoldeGeneral> list = new List<SoldeGeneral>();
			bool flag2 = false;
			this._resetLogs();
			this._JobLog = new StringBuilder(" Marshalling " + dictionary.Keys.Count.ToString() + " ...");
			try
			{
				foreach (string text in dictionary.Keys)
				{
					SoldeGeneral soldeGeneral = new SoldeGeneral();
					soldeGeneral.NoCompte = text;
					try
					{
						DataRow[] array = m_tb.Select("COMPTE='" + text + "'  And (EC_ANTYPE>0 OR JO_NUM='RAN') And EC_SENS=0");
						bool flag3 = array != null && array.GetLength(0) >= 1;
						if (flag3)
						{
							bool flag4 = string.IsNullOrEmpty(soldeGeneral.NomCompte);
							if (flag4)
							{
								bool flag5 = m_tb.Columns.Count >= 7;
								if (flag5)
								{
									soldeGeneral.NomCompte = Convert.ToString(array[0][6]);
								}
								bool flag6 = m_tb.Columns.Count >= 8;
								if (flag6)
								{
									soldeGeneral.PhoneCompte = Convert.ToString(array[0][7]);
								}
								bool flag7 = m_tb.Columns.Count >= 9;
								if (flag7)
								{
									soldeGeneral.emailCompte = Convert.ToString(array[0][8]);
								}
							}
							foreach (DataRow dataRow2 in array)
							{
								bool flag8 = dataRow2.Table.Columns.Count >= 4;
								if (flag8)
								{
									SoldeGeneral soldeGeneral2;
									(soldeGeneral2 = soldeGeneral).ReportDebiteur = soldeGeneral2.ReportDebiteur + Convert.ToDecimal(dataRow2[3]);
								}
							}
						}
						DataRow[] array3 = m_tb.Select("COMPTE='" + text + "' AND   (EC_ANTYPE>0 OR JO_NUM='RAN')  And EC_SENS=1");
						bool flag9 = array3 != null && array3.GetLength(0) >= 1;
						if (flag9)
						{
							bool flag10 = string.IsNullOrEmpty(soldeGeneral.NomCompte);
							if (flag10)
							{
								bool flag11 = m_tb.Columns.Count >= 7;
								if (flag11)
								{
									soldeGeneral.NomCompte = Convert.ToString(array3[0][6]);
								}
								bool flag12 = m_tb.Columns.Count >= 8;
								if (flag12)
								{
									soldeGeneral.PhoneCompte = Convert.ToString(array3[0][7]);
								}
								bool flag13 = m_tb.Columns.Count >= 9;
								if (flag13)
								{
									soldeGeneral.emailCompte = Convert.ToString(array3[0][8]);
								}
							}
						}
						foreach (DataRow dataRow3 in array3)
						{
							bool flag14 = dataRow3.Table.Columns.Count >= 4;
							if (flag14)
							{
								SoldeGeneral soldeGeneral2;
								(soldeGeneral2 = soldeGeneral).REportCrediteur = soldeGeneral2.REportCrediteur + Convert.ToDecimal(dataRow3[3]);
							}
						}
						DataRow[] array5 = m_tb.Select("COMPTE='" + text + "' AND (EC_ANTYPE=0 AND JO_NUM<>'RAN') And EC_SENS=0");
						bool flag15 = array5 != null && array5.GetLength(0) >= 1;
						if (flag15)
						{
							foreach (DataRow dataRow4 in array5)
							{
								bool flag16 = dataRow4.Table.Columns.Count >= 4;
								if (flag16)
								{
									SoldeGeneral soldeGeneral2;
									(soldeGeneral2 = soldeGeneral).MontantMouvementsDebiteurs = soldeGeneral2.MontantMouvementsDebiteurs + Convert.ToDecimal(dataRow4[3]);
								}
							}
							bool flag17 = string.IsNullOrEmpty(soldeGeneral.NomCompte);
							if (flag17)
							{
								bool flag18 = m_tb.Columns.Count >= 7;
								if (flag18)
								{
									soldeGeneral.NomCompte = Convert.ToString(array5[0][6]);
								}
								bool flag19 = m_tb.Columns.Count >= 8;
								if (flag19)
								{
									soldeGeneral.PhoneCompte = Convert.ToString(array5[0][7]);
								}
								bool flag20 = m_tb.Columns.Count >= 9;
								if (flag20)
								{
									soldeGeneral.emailCompte = Convert.ToString(array5[0][8]);
								}
							}
						}
						DataRow[] array7 = m_tb.Select("COMPTE='" + text + "' AND (EC_ANTYPE=0 AND JO_NUM<>'RAN') And EC_SENS=1");
						bool flag21 = array7 != null && array7.GetLength(0) >= 1;
						if (flag21)
						{
							bool flag22 = string.IsNullOrEmpty(soldeGeneral.NomCompte);
							if (flag22)
							{
								bool flag23 = m_tb.Columns.Count >= 7;
								if (flag23)
								{
									soldeGeneral.NomCompte = Convert.ToString(array7[0][6]);
								}
								bool flag24 = m_tb.Columns.Count >= 8;
								if (flag24)
								{
									soldeGeneral.PhoneCompte = Convert.ToString(array7[0][7]);
								}
								bool flag25 = m_tb.Columns.Count >= 9;
								if (flag25)
								{
									soldeGeneral.emailCompte = Convert.ToString(array7[0][8]);
								}
							}
						}
						foreach (DataRow dataRow5 in array7)
						{
							bool flag26 = dataRow5.Table.Columns.Count >= 4;
							if (flag26)
							{
								SoldeGeneral soldeGeneral2;
								(soldeGeneral2 = soldeGeneral).MontantMouvementsCrediteurs = soldeGeneral2.MontantMouvementsCrediteurs + Convert.ToDecimal(dataRow5[3]);
							}
						}
						decimal num = decimal.Add(soldeGeneral.ReportDebiteur, soldeGeneral.MontantMouvementsDebiteurs);
						decimal num2 = decimal.Add(soldeGeneral.REportCrediteur, soldeGeneral.MontantMouvementsCrediteurs);
						bool flag27 = decimal.Compare(num, num2) > 0;
						if (flag27)
						{
							soldeGeneral.AReporterDebiteur = decimal.Subtract(num, num2);
						}
						else
						{
							soldeGeneral.AReporterCrediteur = decimal.Subtract(num2, num);
						}
						flag2 = true;
					}
					catch (Exception ex)
					{
						flag2 = false;
						this._JobLog.AppendLine("Erreur _marshall:\r" + ex.Message);
					}
					bool flag28 = flag2;
					if (!flag28)
					{
						list = null;
						break;
					}
					list.Add(soldeGeneral);
				}
			}
			finally
			{
				Dictionary<string, DataRow>.KeyCollection.Enumerator enumerator2;
				((IDisposable)enumerator2).Dispose();
			}
			return list;
		}
		private SoldeGeneral _SoldesGenerauxBKCompteT(byte[] m_cbAcct, DateTime m_Date1, DateTime m_Date2, int m_CatEcr)
		{
			DataTable dataTable = this.m_oCat.ECRITURE_CalculerTotauxCBCompte(m_Date1, m_Date2, m_cbAcct, true, SENS_ECR.AUCUN);
			bool flag = dataTable == null;
			SoldeGeneral result;
			if (flag)
			{
				result = null;
			}
			else
			{
				SoldeGeneral soldeGeneral = new SoldeGeneral();
				DataRow[] array = dataTable.Select("EC_ANTYPE=1 And EC_SENS=0");
				bool flag2 = array != null && array.GetLength(0) >= 1;
				if (flag2)
				{
					soldeGeneral.ReportDebiteur = Convert.ToDecimal(array[0][0]);
				}
				DataRow[] array2 = dataTable.Select("EC_ANTYPE=1 And EC_SENS=1");
				bool flag3 = array2 != null && array2.GetLength(0) >= 1;
				if (flag3)
				{
					soldeGeneral.REportCrediteur = Convert.ToDecimal(array2[0][0]);
				}
				DataRow[] array3 = dataTable.Select("EC_ANTYPE=0 And EC_SENS=0");
				bool flag4 = array3 != null && array3.GetLength(0) >= 1;
				if (flag4)
				{
					soldeGeneral.MontantMouvementsDebiteurs = Convert.ToDecimal(array3[0][0]);
				}
				DataRow[] array4 = dataTable.Select("EC_ANTYPE=0 And EC_SENS=1");
				bool flag5 = array4 != null && array4.GetLength(0) >= 1;
				if (flag5)
				{
					soldeGeneral.MontantMouvementsCrediteurs = Convert.ToDecimal(array4[0][0]);
				}
				decimal num = decimal.Add(soldeGeneral.ReportDebiteur, soldeGeneral.MontantMouvementsDebiteurs);
				decimal num2 = decimal.Add(soldeGeneral.REportCrediteur, soldeGeneral.MontantMouvementsCrediteurs);
				bool flag6 = decimal.Compare(num, num2) > 0;
				if (flag6)
				{
					soldeGeneral.AReporterDebiteur = decimal.Subtract(num, num2);
				}
				else
				{
					soldeGeneral.AReporterCrediteur = decimal.Subtract(num2, num);
				}
				result = soldeGeneral;
			}
			return result;
		}
		public SoldeGeneral SoldesGenerauxCompteT(string m_Acct, DateTime m_Date1, DateTime m_Date2, int m_CatEcr)
		{
			clsClient clsClient = this.m_oCat.TIERS_LoadItem(m_Acct, CB_TYPE_TIERS.CB_TYPE_TIERS_CLIENT);
			bool flag = clsClient == null;
			SoldeGeneral result;
			if (flag)
			{
				result = null;
			}
			else
			{
				SoldeGeneral soldeGeneral = this._SoldesGenerauxCompteT(clsClient.CT_NUM, m_Date1, m_Date2, 0);
				bool flag2 = soldeGeneral != null && (decimal.Compare(soldeGeneral.AReporterCrediteur, 0m) != 0 | decimal.Compare(soldeGeneral.AReporterDebiteur, 0m) != 0 | decimal.Compare(soldeGeneral.MontantMouvementsDebiteurs, 0m) != 0 | decimal.Compare(soldeGeneral.MontantMouvementsCrediteurs, 0m) != 0);
				if (flag2)
				{
					soldeGeneral.NoCompte = clsClient.CT_NUM;
					soldeGeneral.NomCompte = clsClient.CT_INTITULE;
					soldeGeneral.PhoneCompte = clsClient.CT_TELEPHONE;
					soldeGeneral.emailCompte = clsClient.CT_EMAIL;
					result = soldeGeneral;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}
		public SoldeGeneral SoldesGenerauxBKCompteT(string m_bkAcct, DateTime m_Date1, DateTime m_Date2, int m_CatEcr)
		{
			clsClient clsClient = this.m_oCat.TIERS_CBLoadItem(m_bkAcct, CB_TYPE_TIERS.CB_TYPE_TIERS_CLIENT);
			bool flag = clsClient == null;
			SoldeGeneral result;
			if (flag)
			{
				result = null;
			}
			else
			{
				SoldeGeneral soldeGeneral = this._SoldesGenerauxBKCompteT(clsClient.CBCT_NUM, m_Date1, m_Date2, 0);
				bool flag2 = soldeGeneral != null && (decimal.Compare(soldeGeneral.AReporterCrediteur, 0m) != 0 | decimal.Compare(soldeGeneral.AReporterDebiteur, 0m) != 0 | decimal.Compare(soldeGeneral.MontantMouvementsDebiteurs, 0m) != 0 | decimal.Compare(soldeGeneral.MontantMouvementsCrediteurs, 0m) != 0);
				if (flag2)
				{
					soldeGeneral.NoCompte = clsClient.CT_NUM;
					soldeGeneral.NomCompte = clsClient.CT_INTITULE;
					soldeGeneral.PhoneCompte = clsClient.CT_TELEPHONE;
					soldeGeneral.emailCompte = clsClient.CT_EMAIL;
					result = soldeGeneral;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}
		public List<SoldeGeneral> BalanceTiers_BagFetch()
		{
			bool flag = this._soldeBag.Count == 0;
			checked
			{
				List<SoldeGeneral> result;
				if (flag)
				{
					result = null;
				}
				else
				{
					int num = 0;
					List<SoldeGeneral> list = new List<SoldeGeneral>();
					while (this._soldeBag.Count > 0)
					{
						bool flag2 = num <= this._DOCMAXRET;
						if (!flag2)
						{
							break;
						}
						num++;
						list.Add(this._soldeBag[0]);
						this._soldeBag.RemoveAt(0);
					}
					result = list;
				}
				return result;
			}
		}
		public List<SoldeGeneral> BalanceCompteTiers(DateTime m_Date1, DateTime m_Date2, int m_typeTiers, int m_CatEcr)
		{
			this._soldeBag = new List<SoldeGeneral>();
			bool flag = m_typeTiers == -1;
			DataTable dataTable;
			if (flag)
			{
				dataTable = this.m_oCat.ECRITURE_CalculerTotaux(m_Date1, m_Date2, false, 0, SENS_ECR.AUCUN);
			}
			else
			{
				dataTable = this.m_oCat.ECRITURE_CalculerTotaux(m_Date1, m_Date2, true, m_typeTiers, SENS_ECR.AUCUN);
			}
			bool flag2 = dataTable == null;
			checked
			{
				List<SoldeGeneral> result;
				if (flag2)
				{
					result = null;
				}
				else
				{
					bool flag3 = dataTable.Rows.Count == 0;
					if (flag3)
					{
						result = null;
					}
					else
					{
						List<SoldeGeneral> list = this._marshallResults(dataTable);
						List<SoldeGeneral> list2 = new List<SoldeGeneral>();
						int num = 0;
						bool flag4 = list != null;
						if (flag4)
						{
							try
							{
								foreach (SoldeGeneral item in list)
								{
									num++;
									bool flag5 = num <= this._MAXRET;
									if (flag5)
									{
										list2.Add(item);
									}
									else
									{
										this._soldeBag.Add(item);
									}
								}
							}
							finally
							{
								List<SoldeGeneral>.Enumerator enumerator;
								((IDisposable)enumerator).Dispose();
							}
							result = list2;
						}
						else
						{
							result = null;
						}
					}
				}
				return result;
			}
		}
		public bool confirmWorkingACGateway()
		{
			bool flag = this._gw != null && this._gw.IsLinked() && this._gw.gwInstallationChecked();
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				SQLParams connectedSQLParams = this.m_oCat.connectedSQLParams;
				bool flag2 = connectedSQLParams == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					this._gw = new SecurityGateway(connectedSQLParams);
					result = this._gw.gwInstallationChecked();
				}
			}
			return result;
		}
		private void InitSecurity()
		{
			bool flag = this.confirmWorkingACGateway();
			if (flag)
			{
				bool flag2 = this._gw.INFOUSER_Load1StMatch("sys") == null;
				if (flag2)
				{
					infouser it = new infouser
					{
						pwd = "1234admin",
						usermask = "sys",
						userName = "Accounts Central System Administrator"
					};
					int num = this._gw.INFOUSER_CreateItem(it, "_APP_ADMINS");
					bool flag3 = num > 0;
					if (flag3)
					{
					}
				}
			}
			else
			{
				bool flag4 = this.SecManager.CheckBO();
				if (flag4)
				{
					ISApplicationUser isapplicationUser = this.SecManager.oLib.ISAPPLICATIONUSER_LoadItem("sys");
					bool flag5 = isapplicationUser == null;
					if (flag5)
					{
						isapplicationUser = new ISApplicationUser
						{
							USERCODE = "sys",
							USERFULLNAME = "Accounts Central System Administrator",
							USERGENERALPROFILE = "_APP_ADMINS",
							USERPASSWORD = "1234"
						};
						this.SecManager.oLib.ISAPPLICATIONUSER_CreateItem(isapplicationUser);
					}
				}
			}
		}
		public bool AuthenticateWithPassword(string m_pwd)
		{
			bool flag = this.confirmWorkingACGateway();
			bool result;
			if (flag)
			{
				infouser infouser = this._gw.INFOUSER_Load1StPWDMatch(m_pwd);
				bool flag2 = infouser != null;
				if (flag2)
				{
					this._UserLogged = true;
					this._instanceUserKey = infouser.usermask;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			else
			{
				bool flag3 = this.SecManager.CheckBO();
				if (flag3)
				{
					PortableLoginService oLib = this.SecManager.oLib;
					string text = oLib.User_from_psswd(m_pwd);
					bool flag4 = !string.Equals(text, string.Empty, StringComparison.Ordinal);
					if (flag4)
					{
						this._UserLogged = true;
					}
					else
					{
						this._UserLogged = false;
					}
					this._instanceUserKey = text;
					result = this._UserLogged;
				}
				else
				{
					this._UserLogged = false;
					result = false;
				}
			}
			return result;
		}
		public ISApplicationUser getAuthenticatedUser()
		{
			bool userLogged = this._UserLogged;
			if (userLogged)
			{
				bool flag = this.confirmWorkingACGateway();
				if (flag)
				{
					infouser infouser = this._gw.INFOUSER_Load1StMatch(this._instanceUserKey);
					bool flag2 = infouser != null;
					if (flag2)
					{
						return this._createNewApplicationUser(infouser);
					}
				}
			}
			return null;
		}
		public bool CheckUserLooged()
		{
			return this._UserLogged;
		}
		// (add) Token: 0x060000E9 RID: 233 RVA: 0x00009A10 File Offset: 0x00007C10
		// (remove) Token: 0x060000EA RID: 234 RVA: 0x00009A48 File Offset: 0x00007C48
		public event SageAccService.EchecOperation_ADEventHandler EchecOperation_AD;
		public List<ISApplicationUser> downLoadSystemUsers()
		{
			this.m_EventLog.WriteEntry("downLoadSystemUsers: domain   ...");
			string text = "";
			string text2 = "";
			try
			{
				text2 = IPGlobalProperties.GetIPGlobalProperties().DomainName;
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				text2 = "";
			}
			bool flag = string.Equals(text2, string.Empty, StringComparison.Ordinal);
			if (flag)
			{
				text = "WinNT://" + Environment.MachineName;
				text2 = "(local)";
				this.m_EventLog.WriteEntry(string.Format("downLoadSystemUsers: Not Found domain {0} ", text2));
			}
			else
			{
				text = "LDAP://" + text2;
				this.m_EventLog.WriteEntry(string.Format("downLoadSystemUsers: Found domain {0} ", text2));
			}
			SearchResultCollection searchResultCollection = null;
			DirectorySearcher directorySearcher = null;
			try
			{
				DirectoryEntry searchRoot = new DirectoryEntry(text);
				directorySearcher = new DirectorySearcher(searchRoot);
			}
			catch (Exception ex2)
			{
				SageAccService.EchecOperation_ADEventHandler echecOperation_ADEvent = this.EchecOperation_ADEvent;
				if (echecOperation_ADEvent != null)
				{
					echecOperation_ADEvent(ex2.Message);
				}
				searchResultCollection = null;
			}
			directorySearcher.Filter = "(objectClass=user)";
			this.m_EventLog.WriteEntry(string.Format("downLoadSystemUsers: searching user entries on  {0} ...", text));
			try
			{
				searchResultCollection = directorySearcher.FindAll();
			}
			catch (Exception ex3)
			{
				searchResultCollection = null;
				this.m_EventLog.WriteEntry(string.Format("downLoadSystemUsers: search fail {0}", ex3.Message));
			}
			bool flag2 = searchResultCollection == null;
			List<ISApplicationUser> result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				this.m_EventLog.WriteEntry(string.Format("downLoadSystemUsers: {0} entries found", searchResultCollection.Count));
				List<ISApplicationUser> list = new List<ISApplicationUser>();
				try
				{
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						ISApplicationUser isapplicationUser = new ISApplicationUser();
						DirectoryEntry directoryEntry = searchResult.GetDirectoryEntry();
						isapplicationUser.USERCODE = Convert.ToString(directoryEntry.Properties["SAMAccountName"].Value);
						isapplicationUser.USERFULLNAME = Convert.ToString(directoryEntry.Properties["sn"].Value);
						list.Add(isapplicationUser);
					}
				}
				finally
				{
					IEnumerator enumerator;
					if (enumerator is IDisposable)
					{
						(enumerator as IDisposable).Dispose();
					}
				}
				result = list;
			}
			return result;
		}
		public void DeleteSiteUser(string userCode)
		{
			bool flag = this.confirmWorkingACGateway();
			if (flag)
			{
			}
			bool flag2 = this.SecManager.CheckBO();
			if (flag2)
			{
				PortableLoginService oLib = this.SecManager.oLib;
				oLib.ISAPPLICATIONUSER_DELETEItem(userCode);
			}
		}
		public List<ISApplicationUser> DownLoadSiteUsers()
		{
			bool flag = this.confirmWorkingACGateway();
			if (flag)
			{
				infouser_Collection infouser_Collection = this._gw.INFOUSER_LoadCollection();
				bool flag2 = infouser_Collection != null;
				if (flag2)
				{
					List<ISApplicationUser> list = new List<ISApplicationUser>();
					try
					{
						foreach (infouser infouser in infouser_Collection)
						{
							ISApplicationUser isapplicationUser = new ISApplicationUser
							{
								USERCODE = infouser.usermask,
								USERFULLNAME = infouser.userName,
								USERPASSWORD = infouser.pwd
							};
							userAccRole_Collection userAccRole_Collection = this._gw.USERACCROLE_LoaduseridLinkedItems(infouser.userid);
							bool flag3 = userAccRole_Collection != null && userAccRole_Collection.Count > 0;
							if (flag3)
							{
								isapplicationUser.USERGENERALPROFILE = userAccRole_Collection[checked(userAccRole_Collection.Count - 1)].userAccRole;
							}
							list.Add(isapplicationUser);
						}
					}
					finally
					{
						List<infouser>.Enumerator enumerator;
						((IDisposable)enumerator).Dispose();
					}
					return list;
				}
			}
			bool flag4 = this.SecManager.CheckBO();
			if (flag4)
			{
				PortableLoginService oLib = this.SecManager.oLib;
				ISApplicationUser_Collection isapplicationUser_Collection = oLib.ISApplicationUser_LoadCollection();
				bool flag5 = isapplicationUser_Collection != null;
				if (flag5)
				{
					List<ISApplicationUser> list2 = new List<ISApplicationUser>();
					try
					{
						foreach (ISApplicationUser item in isapplicationUser_Collection)
						{
							list2.Add(item);
						}
					}
					finally
					{
						List<ISApplicationUser>.Enumerator enumerator2;
						((IDisposable)enumerator2).Dispose();
					}
					return list2;
				}
			}
			return null;
		}
		public bool SaveSiteUser(ISApplicationUser SiteUserInfo)
		{
			bool flag = this.confirmWorkingACGateway();
			bool result;
			if (flag)
			{
				infouser it = new infouser
				{
					pwd = SiteUserInfo.USERPASSWORD,
					usermask = SiteUserInfo.USERCODE,
					userName = SiteUserInfo.USERFULLNAME
				};
				result = this._gw.INFOUSER_SaveItem(it, SiteUserInfo.USERGENERALPROFILE);
			}
			else
			{
				bool flag2 = this.SecManager.CheckBO();
				if (flag2)
				{
					PortableLoginService oLib = this.SecManager.oLib;
					bool flag3 = oLib.ISAPPLICATIONUSER_LoadItem(SiteUserInfo.USERCODE) == null;
					if (flag3)
					{
						result = oLib.ISAPPLICATIONUSER_CreateItem(SiteUserInfo);
					}
					else
					{
						result = oLib.ISAPPLICATIONUSER_UpdateItem(SiteUserInfo);
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}
		public string SiteUserName()
		{
			return Thread.CurrentPrincipal.Identity.Name;
		}
		public string SiteUserRole()
		{
			string result;
			return result;
		}
		private string _getUserRoleForApplication(string s)
		{
			bool flag = this.confirmWorkingACGateway();
			string result;
			if (flag)
			{
				infouser infouser = this._gw.INFOUSER_Load1StMatch(s);
				bool flag2 = infouser != null;
				if (flag2)
				{
					userAccRole_Collection userAccRole_Collection = this._gw.USERACCROLE_LoaduseridLinkedItems(infouser.userid);
					this._UserLogged = true;
					this._instanceUserKey = infouser.usermask;
					bool flag3 = userAccRole_Collection != null && userAccRole_Collection.Count > 0;
					if (flag3)
					{
						return userAccRole_Collection[0].userAccRole;
					}
				}
				result = string.Empty;
			}
			else
			{
				ISApplicationUser isapplicationUser = this.SecManager.oLib.ISAPPLICATIONUSER_LoadItem(s);
				bool flag4 = isapplicationUser != null;
				if (flag4)
				{
					result = isapplicationUser.USERGENERALPROFILE;
				}
				else
				{
					result = string.Empty;
				}
			}
			return result;
		}
		public bool[][] SiteUserExecutionProfile(string userKey)
		{
			string right = WindowsIdentity.GetCurrent().Name.ToString();
			bool flag = string.IsNullOrWhiteSpace(userKey);
			bool[][] result;
			if (flag)
			{
				result = new bool[][]
				{
					new bool[2],
					new bool[4]
				};
			}
			else
			{
				bool flag2 = string.Equals(userKey, right, StringComparison.Ordinal);
				if (flag2)
				{
					result = new bool[][]
					{
						new bool[]
						{
							true,
							true,
							true
						},
						new bool[]
						{
							true,
							true,
							true,
							true
						}
					};
				}
				else
				{
					string left = this._getUserRoleForApplication(userKey);
					if (!string.Equals(left, "_APP_ADMINS", StringComparison.Ordinal))
					{
						if (!string.Equals(left, "_APP_ALL_DOMAINS", StringComparison.Ordinal))
						{
							if (!string.Equals(left, "APP_CLIENT_DOMAIN", StringComparison.Ordinal))
							{
								result = new bool[][]
								{
									new bool[3],
									new bool[4]
								};
							}
							else
							{
								bool[][] array = new bool[2][];
								array[0] = new bool[3];
								int num = 1;
								bool[] array2 = new bool[4];
								array2[1] = true;
								array[num] = array2;
								result = array;
							}
						}
						else
						{
							bool[][] array3 = new bool[2][];
							int num2 = 0;
							bool[] array4 = new bool[3];
							array4[0] = true;
							array4[1] = true;
							array3[num2] = array4;
							array3[1] = new bool[]
							{
								true,
								true,
								true,
								true
							};
							result = array3;
						}
					}
					else
					{
						result = new bool[][]
						{
							new bool[]
							{
								true,
								true,
								true
							},
							new bool[]
							{
								true,
								true,
								true,
								true
							}
						};
					}
				}
			}
			return result;
		}
		private ISApplicationUser _createNewApplicationUser(infouser u)
		{
			bool flag = u == null;
			checked
			{
				ISApplicationUser result;
				if (flag)
				{
					result = null;
				}
				else
				{
					ISApplicationUser isapplicationUser = new ISApplicationUser
					{
						USERCODE = u.usermask,
						USERFULLNAME = u.userName,
						USERPASSWORD = u.pwd,
						USERACCESSLEVEL = 0
					};
					string[] array = u.securityFlags.Split(new char[]
					{
						'|'
					});
					bool flag2 = array.GetLength(0) == 3;
					if (flag2)
					{
						int useraccesslevel = 0;
						int num = 0;
						int num2 = 2;
						for (;;)
						{
							bool flag3 = int.TryParse(array[num2], out num);
							if (flag3)
							{
								bool flag4 = num == 1;
								if (flag4)
								{
									break;
								}
							}
							num2 += -1;
							if (num2 < 0)
							{
								goto IL_A7;
							}
						}
						useraccesslevel = num2 + 1;
						IL_A7:
						isapplicationUser.USERACCESSLEVEL = useraccesslevel;
					}
					result = isapplicationUser;
				}
				return result;
			}
		}
		private void _callback(object state)
		{
			CompletedAsyncResult<glQryInfo> completedAsyncResult = new CompletedAsyncResult<glQryInfo>((glQryInfo)state);
			try
			{
			}
			catch (Exception ex)
			{
			}
		}
		private void _gl_calculer(int m_typeTiers, DateTime m_Date1, DateTime m_Date2)
		{
			bool flag = true;
			bool flag2 = m_typeTiers == 1;
			if (flag2)
			{
				flag = false;
			}
			bool flag3 = !this.CheckDSLink();
			checked
			{
				if (flag3)
				{
					this.__bGLLoadComplete = true;
					this.__bGLLoadErrStat = false;
				}
				else
				{
					this.__bGLLoadErrStat = true;
					clsCbEcritureComptable_Collection clsCbEcritureComptable_Collection = this.m_oCat.ECRITURE_LoadCollection(m_Date1, m_Date2, "0", "ZZZZZZZ", flag, SENS_ECR.AUCUN);
					bool flag4 = clsCbEcritureComptable_Collection != null;
					if (flag4)
					{
						List<LigneGL> list = new List<LigneGL>();
						int num = 0;
						try
						{
							foreach (clsCbEcritureComptable clsCbEcritureComptable in clsCbEcritureComptable_Collection)
							{
								LigneGL ligneGL = new LigneGL
								{
									AppEc = "SAGE.CPTA.100.SQL",
									DateModifEc = clsCbEcritureComptable.DATE_MODIF
								};
								LigneGL ligneGL2 = ligneGL;
								ligneGL2.CreditEC = clsCbEcritureComptable.getCredit();
								ligneGL2.DateEc = clsCbEcritureComptable.DATEComptable;
								ligneGL2.DebitEc = clsCbEcritureComptable.getDebit();
								ligneGL2.IntituleCompte = clsCbEcritureComptable.INTITULE_COMPTE;
								ligneGL2.JnalEc = clsCbEcritureComptable.JO_NUM;
								ligneGL2.LettrageEc = clsCbEcritureComptable.EC_LETTRAGE;
								ligneGL2.LibelleEc = clsCbEcritureComptable.EC_INTITULE;
								bool flag5 = flag;
								if (flag5)
								{
									ligneGL2.NoCompte = clsCbEcritureComptable.CT_NUM;
								}
								else
								{
									ligneGL2.NoCompte = clsCbEcritureComptable.CG_NUM;
								}
								ligneGL2.PieceEc = clsCbEcritureComptable.EC_PIECE;
								num++;
								bool flag6 = num <= this.MAXRET;
								if (flag6)
								{
									list.Add(ligneGL);
								}
								else
								{
									this._GLLGBag.Add(ligneGL);
								}
							}
						}
						finally
						{
							IEnumerator<clsCbEcritureComptable> enumerator;
							if (enumerator != null)
							{
								enumerator.Dispose();
							}
						}
						this._glResults = new GLBagInfo
						{
							BagCount = num,
							Data = list
						};
						this.__bGLLoadComplete = true;
						this.__bGLLoadErrStat = false;
					}
				}
			}
		}
		private void _gl_CalculerCompte(string m_Acct, int m_typeTiers, DateTime m_Date1, DateTime m_Date2)
		{
			bool flag = true;
			bool flag2 = m_typeTiers == 1;
			if (flag2)
			{
				flag = false;
			}
			bool flag3 = !this.CheckDSLink();
			checked
			{
				if (flag3)
				{
					this.__bGLLoadComplete = true;
					this.__bGLLoadErrStat = false;
				}
				else
				{
					this.__bGLLoadErrStat = true;
					clsCbEcritureComptable_Collection clsCbEcritureComptable_Collection = this.m_oCat.ECRITURE_LoadCollection(m_Date1, m_Date2, m_Acct, m_Acct, flag, SENS_ECR.AUCUN);
					bool flag4 = clsCbEcritureComptable_Collection != null;
					if (flag4)
					{
						List<LigneGL> list = new List<LigneGL>();
						int num = 0;
						try
						{
							foreach (clsCbEcritureComptable clsCbEcritureComptable in clsCbEcritureComptable_Collection)
							{
								LigneGL ligneGL = new LigneGL
								{
									AppEc = "SAGE.CPTA.100.SQL",
									DateModifEc = clsCbEcritureComptable.DATE_MODIF
								};
								LigneGL ligneGL2 = ligneGL;
								ligneGL2.CreditEC = clsCbEcritureComptable.getCredit();
								ligneGL2.DateEc = clsCbEcritureComptable.DATEComptable;
								ligneGL2.DebitEc = clsCbEcritureComptable.getDebit();
								ligneGL2.IntituleCompte = clsCbEcritureComptable.INTITULE_COMPTE;
								ligneGL2.JnalEc = clsCbEcritureComptable.JO_NUM;
								ligneGL2.LettrageEc = clsCbEcritureComptable.EC_LETTRAGE;
								ligneGL2.LibelleEc = clsCbEcritureComptable.EC_INTITULE;
								bool flag5 = flag;
								if (flag5)
								{
									ligneGL2.NoCompte = clsCbEcritureComptable.CT_NUM;
								}
								else
								{
									ligneGL2.NoCompte = clsCbEcritureComptable.CG_NUM;
								}
								ligneGL2.PieceEc = clsCbEcritureComptable.EC_PIECE;
								num++;
								bool flag6 = num <= this.MAXRET;
								if (flag6)
								{
									list.Add(ligneGL);
								}
								else
								{
									this._GLLGBag.Add(ligneGL);
								}
							}
						}
						finally
						{
							IEnumerator<clsCbEcritureComptable> enumerator;
							if (enumerator != null)
							{
								enumerator.Dispose();
							}
						}
						this._glResults = new GLBagInfo
						{
							BagCount = num,
							Data = list
						};
						this.__bGLLoadComplete = true;
						this.__bGLLoadErrStat = false;
					}
				}
			}
		}
		private void _endGL_Calculer(IAsyncResult ar)
		{
			this.__bGLLoading = false;
			bool isCompleted = ar.IsCompleted;
			if (isCompleted)
			{
				this._fnLoadGL.EndInvoke(ar);
				this.__bGLLoadErrStat = false;
			}
		}
		private void _endGL_CalculerCompte(IAsyncResult ar)
		{
			this.__bGLLoading = false;
			bool isCompleted = ar.IsCompleted;
			if (isCompleted)
			{
				this._fnLoadGLAcc.EndInvoke(ar);
				this.__bGLLoadErrStat = false;
			}
		}
		public List<LigneGL> gl_Calculer_Compte(string m_Acct, int m_typeTiers, DateTime m_Date1, DateTime m_Date2)
		{
			bool _bGLLoading = this.__bGLLoading;
			List<LigneGL> result;
			if (_bGLLoading)
			{
				List<LigneGL> list = new List<LigneGL>
				{
					new LigneGL
					{
						IntituleCompte = "Waiting for result"
					}
				};
				result = list;
			}
			else
			{
				bool flag = !this.__bGLLoading && this.__bGLLoadComplete;
				if (flag)
				{
					this.__bGLLoading = false;
					this.__bGLLoadComplete = false;
					this.__bLoadErrStat = false;
					result = this._glResults.Data;
				}
				else
				{
					bool flag2 = !this.__bGLLoading && this.__bLoadErrStat;
					if (flag2)
					{
						this.__bGLLoading = false;
						this.__bGLLoadComplete = false;
						this.__bLoadErrStat = false;
						result = null;
					}
					else
					{
						this.__imputationTransResults = null;
						this.__bGLLoadComplete = false;
						this._fnLoadGLAcc = new SageAccService.dlgt_GLLoadAcc(this._gl_CalculerCompte);
						this._clbkLoadJnal = new AsyncCallback(this._endGL_CalculerCompte);
						this._fnLoadGLAcc.BeginInvoke(m_Acct, m_typeTiers, m_Date1, m_Date2, this._clbkLoadGL, this._loadGLBag);
						this.__bGLLoading = true;
						List<LigneGL> list2 = new List<LigneGL>
						{
							new LigneGL
							{
								IntituleCompte = "Waiting for result"
							}
						};
						result = list2;
					}
				}
			}
			return result;
		}
		public List<LigneGL> gl_Calculer(int m_typeTiers, DateTime m_Date1, DateTime m_Date2)
		{
			bool _bGLLoading = this.__bGLLoading;
			List<LigneGL> result;
			if (_bGLLoading)
			{
				List<LigneGL> list = new List<LigneGL>
				{
					new LigneGL
					{
						IntituleCompte = "Waiting for result"
					}
				};
				result = list;
			}
			else
			{
				bool flag = !this.__bGLLoading && this.__bGLLoadComplete;
				if (flag)
				{
					this.__bGLLoading = false;
					this.__bGLLoadComplete = false;
					this.__bLoadErrStat = false;
					result = this._glResults.Data;
				}
				else
				{
					bool flag2 = !this.__bGLLoading && this.__bLoadErrStat;
					if (flag2)
					{
						this.__bGLLoading = false;
						this.__bGLLoadComplete = false;
						this.__bLoadErrStat = false;
						result = null;
					}
					else
					{
						this.__imputationTransResults = null;
						this.__bGLLoadComplete = false;
						this._fnLoadGL = new SageAccService.dlgt_GLLoad(this._gl_calculer);
						this._clbkLoadJnal = new AsyncCallback(this._endGL_Calculer);
						this._fnLoadGL.BeginInvoke(m_typeTiers, m_Date1, m_Date2, this._clbkLoadGL, this._loadGLBag);
						this.__bGLLoading = true;
						List<LigneGL> list2 = new List<LigneGL>
						{
							new LigneGL
							{
								IntituleCompte = "Waiting for result"
							}
						};
						result = list2;
					}
				}
			}
			return result;
		}
		public List<LigneGL> GLBagFetch()
		{
			bool flag = this._GLLGBag.Count == 0;
			checked
			{
				List<LigneGL> result;
				if (flag)
				{
					result = null;
				}
				else
				{
					int num = 0;
					List<LigneGL> list = new List<LigneGL>();
					while (this._GLLGBag.Count > 0)
					{
						bool flag2 = num <= this._DOCMAXRET;
						if (!flag2)
						{
							break;
						}
						num++;
						list.Add(this._GLLGBag[0]);
						this._GLLGBag.RemoveAt(0);
					}
					result = list;
				}
				return result;
			}
		}
		public int JOURNAL_CreateEcritureCentralisation(string mJnal, DateTime dDate, int sns)
		{
			DateTime jm_DATE = new DateTime(dDate.Year, dDate.Month, 1);
			DateTime dateTime = jm_DATE.AddMonths(1).AddDays(-1.0);
			int day = dateTime.Day;
			JOURNAL journal = this.m_oCat.JOURNAL_LoadItem(mJnal);
			bool flag = journal == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = journal.JO_TYPE != 2;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					string ec_INTITULE = string.Format("Centralisation {0}/{1}", dDate.Month, dDate.Year);
					clsCbEcritureComptable ec = checked(new clsCbEcritureComptable
					{
						JO_NUM = mJnal,
						EC_INTITULE = ec_INTITULE,
						EC_MONTANT = 1m,
						EC_SENS = (byte)sns,
						JM_DATE = jm_DATE,
						EC_JOUR = (byte)day,
						N_REGLEMENT = 0,
						EC_CTYPE = 1,
						CG_NUM = journal.CG_NUM,
						CG_NUMCONT = "",
						DATEComptable = dateTime,
						EC_DATE = dateTime
					});
					result = this.m_oCat.ECRITURE_INSERT(ec);
				}
			}
			return result;
		}
		public Imputation[] JOURNAL_LoadCentralisationsMois(string mJnal, DateTime mDate)
		{
			clsCbEcritureComptable_Collection clsCbEcritureComptable_Collection = this.m_oCat.ECRITURE_LoadCentralisationJournalTresorMois(mJnal, mDate);
			bool flag = clsCbEcritureComptable_Collection != null;
			Imputation[] result;
			if (flag)
			{
				List<Imputation> list = new List<Imputation>();
				try
				{
					foreach (clsCbEcritureComptable oItem in clsCbEcritureComptable_Collection)
					{
						list.Add(this.CONVERT_clsCbEcritureComptable_2_Imputation(oItem));
					}
				}
				finally
				{
					IEnumerator<clsCbEcritureComptable> enumerator;
					if (enumerator != null)
					{
						enumerator.Dispose();
					}
				}
				result = list.ToArray();
			}
			else
			{
				result = null;
			}
			return result;
		}
		public bool IMPUTATION_DeLettrage(int m_Id)
		{
			return this.m_oCat.ECRITURE_DecloturerEc(m_Id) > 0;
		}
		private GLBagInfo __calculerPlage(string m_Acct1, string m_Acct2, int m_typeTiers, DateTime m_Date1, DateTime m_Date2)
		{
			bool flag = true;
			bool flag2 = m_typeTiers == 1;
			if (flag2)
			{
				flag = false;
			}
			clsCbEcritureComptable_Collection clsCbEcritureComptable_Collection = this.m_oCat.ECRITURE_LoadCollection(m_Date1, m_Date2, "0000000", "ZZZZZZZ", flag, SENS_ECR.AUCUN);
			bool flag3 = clsCbEcritureComptable_Collection != null;
			checked
			{
				GLBagInfo result;
				if (flag3)
				{
					List<LigneGL> list = new List<LigneGL>();
					int num = 0;
					try
					{
						foreach (clsCbEcritureComptable clsCbEcritureComptable in clsCbEcritureComptable_Collection)
						{
							LigneGL ligneGL = new LigneGL
							{
								AppEc = "SAGE.CPTA.100.SQL",
								DateModifEc = clsCbEcritureComptable.DATE_MODIF
							};
							LigneGL ligneGL2 = ligneGL;
							ligneGL2.CreditEC = clsCbEcritureComptable.getCredit();
							ligneGL2.DateEc = clsCbEcritureComptable.DATEComptable;
							ligneGL2.DebitEc = clsCbEcritureComptable.getDebit();
							ligneGL2.IntituleCompte = clsCbEcritureComptable.INTITULE_COMPTE;
							ligneGL2.JnalEc = clsCbEcritureComptable.JO_NUM;
							ligneGL2.LettrageEc = clsCbEcritureComptable.EC_LETTRAGE;
							ligneGL2.LibelleEc = clsCbEcritureComptable.EC_INTITULE;
							bool flag4 = flag;
							if (flag4)
							{
								ligneGL2.NoCompte = clsCbEcritureComptable.CT_NUM;
							}
							else
							{
								ligneGL2.NoCompte = clsCbEcritureComptable.CG_NUM;
							}
							ligneGL2.PieceEc = clsCbEcritureComptable.EC_PIECE;
							num++;
							bool flag5 = num <= this.MAXRET;
							if (flag5)
							{
								list.Add(ligneGL);
							}
							else
							{
								this._GLLGBag.Add(ligneGL);
							}
						}
					}
					finally
					{
						IEnumerator<clsCbEcritureComptable> enumerator;
						if (enumerator != null)
						{
							enumerator.Dispose();
						}
					}
					GLBagInfo glbagInfo = new GLBagInfo
					{
						BagCount = num,
						Data = list
					};
					result = glbagInfo;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}
		private string m_SiteVersion;
		private int _MAXRET;
		private int _DOCMAXRET;
		private SageNetServices m_oCat;
		private SageAccService.APPLICATION_FAMILY m_ApplicationFamily;
		private string m_DossierSage;
		private int m_Nodepot;
		private string m_SectionAnalityt;
		private string m_LastComment;
		private CIALDefaults m_CPTASales;
		private acSession _acSession;
		private TransactionLogService _sessionLogger;
		private int _sessionID;
		protected StringBuilder _transmissionLog;
		protected StringBuilder _JobLog;
		private object cbEcrObj;
		private AsyncCallback clbkLaodEcr;
		private SageAccService.dlgt_LoadEcriture fnLoadEcr;
		private bool bFnExecuting;
		private bool __bImputationLoading;
		private bool __bImputationLoadComplete;
		private bool __bLoadErrStat;
		private ImputationTransmission __imputationTransResults;
		private SageAccService.dlgt_ImputationLoadJnal _fnLoadJnal;
		private AsyncCallback _clbkLoadJnal;
		private object _loadJnalBag;
		private accJobLogCollection _accjobLog;
		private acAction_Collection __actions;
		private Task<int> _tsk;
		private int _tskValue;
		private List<TiersComptable> m_TiersBag;
		private List<Imputation> m_ImputationBag;
		private int m_WriteLockedState;
		private bool _IsInLogOnlyMode;
		private SageAccService.dlgt_ECRSJob fnImputationsCreationJob;
		private AsyncCallback clbkImpsCreation;
		private object objImpCr;
		private jobResult _jobImputationResult;
		private List<SoldeGeneral> _soldeBag;
		private string _instanceUserKey;
		private SecurityGateway _gw;
		private AuthManager SecManager;
		private bool _UserLogged;
		private List<LigneGL> _GLLGBag;
		private GLBagInfo _glResults;
		private bool __bGLLoading;
		private bool __bGLLoadComplete;
		private bool __bGLLoadErrStat;
		private SageAccService.dlgt_GLLoadAcc _fnLoadGLAcc;
		private AsyncCallback _clbkLoadGL;
		private object _loadGLBag;
		private SageAccService.dlgt_GLLoad _fnLoadGL;
		private string _ObeapiUrl;
		public enum APPLICATION_FAMILY
		{
			TOPCOM,
			SAGESQL,
			SAGECBASE
		}
		// (Invoke) Token: 0x0600014E RID: 334
		public delegate void DSLinkAvailabilityChangedEventHandler(bool changedToGood);
		// (Invoke) Token: 0x06000152 RID: 338
		public delegate clsCbEcritureComptable_Collection dlgt_LoadEcriture(string m_CodeJnal, string srch, DateTime m_date1, DateTime m_date2);
		// (Invoke) Token: 0x06000156 RID: 342
		public delegate int dlgt_ImputationLoadJnal(string m_CodeJnal, DateTime m_date1, DateTime m_date2);
		// (Invoke) Token: 0x0600015A RID: 346
		public delegate int dlgt_ECRSJob(IEnumerable<Imputation> imps);
		public enum USER_CHECK_STATE
		{
			UNREGISTERD_ON_SITE,
			UNKNOWN = -1,
			KNOWN_AND_REGISTERED = 1
		}
		// (Invoke) Token: 0x0600015E RID: 350
		public delegate void EchecOperation_ADEventHandler(string m_Msg);
		// (Invoke) Token: 0x06000162 RID: 354
		public delegate void dlgt_GLLoadAcc(string m_Acct, int m_typeTiers, DateTime m_Date1, DateTime m_Date2);
		// (Invoke) Token: 0x06000166 RID: 358
		public delegate void dlgt_GLLoad(int m_typeTiers, DateTime m_Date1, DateTime m_Date2);
	}
}
