using System;
using System.IO;
using IniParser;
using IniParser.Model;
using Microsoft.VisualBasic.CompilerServices;

namespace Infosciences.Sage
{
	// Token: 0x02000016 RID: 22
	[StandardModule]
	internal sealed class Utils
	{
		// Token: 0x0600013F RID: 319 RVA: 0x0000D1A0 File Offset: 0x0000B3A0
		public static void SaveMaxRets(int _docmaxrets)
		{
			bool flag = !Directory.Exists(Utils.ServerRepository);
			if (flag)
			{
				Directory.CreateDirectory(Utils.ServerRepository);
			}
			string text = Path.Combine(Utils.ServerRepository, Utils.ApplicationRepository);
			bool flag2 = !Directory.Exists(text);
			if (flag2)
			{
				Directory.CreateDirectory(text);
			}
			string text2 = Path.Combine(text, Utils.obeClientfile);
			FileIniDataParser fileIniDataParser = new FileIniDataParser();
			IniData iniData = new IniData();
			iniData.Sections.Add(new SectionData("Currents"));
			iniData["Currents"]["docmaxrets"] = _docmaxrets.ToString();
			fileIniDataParser.WriteFile(text2, iniData, null);
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000D248 File Offset: 0x0000B448
		public static int LoadMaxRet()
		{
			bool flag = !Directory.Exists(Utils.ServerRepository);
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				string text = Path.Combine(Utils.ServerRepository, Utils.ApplicationRepository);
				bool flag2 = !Directory.Exists(text);
				if (flag2)
				{
					result = 0;
				}
				else
				{
					string text2 = Path.Combine(text, Utils.obeClientfile);
					bool flag3 = !File.Exists(text2);
					if (flag3)
					{
						result = 0;
					}
					else
					{
						FileIniDataParser fileIniDataParser = new FileIniDataParser();
						IniData iniData;
						try
						{
							iniData = fileIniDataParser.ReadFile(text2);
						}
						catch (Exception ex)
						{
							iniData = null;
						}
						bool flag4 = iniData != null;
						if (flag4)
						{
							int result2 = 0;
							string s = iniData["Currents"]["docmaxrets"];
							bool flag5 = int.TryParse(s, out result2);
							if (flag5)
							{
								return result2;
							}
						}
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x04000082 RID: 130
		private static string ServerRepository = "c:\\Infoscienceservers";

		// Token: 0x04000083 RID: 131
		private static string ApplicationRepository = "accountscentral";

		// Token: 0x04000084 RID: 132
		private static string obeClientfile = "transactionsettings.ini";
	}
}
