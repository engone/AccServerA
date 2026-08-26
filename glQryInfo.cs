using System;

namespace Infosciences.Sage
{
	// Token: 0x02000011 RID: 17
	public class glQryInfo
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600005F RID: 95 RVA: 0x000040AD File Offset: 0x000022AD
		// (set) Token: 0x06000060 RID: 96 RVA: 0x000040B7 File Offset: 0x000022B7
		public DateTime debut { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000061 RID: 97 RVA: 0x000040C0 File Offset: 0x000022C0
		// (set) Token: 0x06000062 RID: 98 RVA: 0x000040CA File Offset: 0x000022CA
		public DateTime fin { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000063 RID: 99 RVA: 0x000040D3 File Offset: 0x000022D3
		// (set) Token: 0x06000064 RID: 100 RVA: 0x000040DD File Offset: 0x000022DD
		public string acct { get; set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000065 RID: 101 RVA: 0x000040E6 File Offset: 0x000022E6
		// (set) Token: 0x06000066 RID: 102 RVA: 0x000040F0 File Offset: 0x000022F0
		public string acctEnd { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000067 RID: 103 RVA: 0x000040F9 File Offset: 0x000022F9
		// (set) Token: 0x06000068 RID: 104 RVA: 0x00004103 File Offset: 0x00002303
		public int accTiersType { get; set; }
	}
}
