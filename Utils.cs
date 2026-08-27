using System;
using System.IO;
using IniParser;
using IniParser.Model;

namespace Infosciences.Sage
{
	internal sealed class Utils
	{
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
		private static string ServerRepository = "c:\\Infoscienceservers";
		private static string ApplicationRepository = "accountscentral";
		private static string obeClientfile = "transactionsettings.ini";
	}
}
