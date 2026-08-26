using System;
using System.Threading;

namespace Infosciences.Sage
{
	// Token: 0x02000013 RID: 19
	internal class CompletedAsyncResult<T> : IAsyncResult
	{
		// Token: 0x06000100 RID: 256 RVA: 0x0000ADC4 File Offset: 0x00008FC4
		public CompletedAsyncResult(T data)
		{
			this.data_Renamed = data;
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000101 RID: 257 RVA: 0x0000ADD8 File Offset: 0x00008FD8
		public T Data
		{
			get
			{
				return this.data_Renamed;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000102 RID: 258 RVA: 0x0000ADF0 File Offset: 0x00008FF0
		public object AsyncState
		{
			get
			{
				return this.data_Renamed;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000103 RID: 259 RVA: 0x0000AE10 File Offset: 0x00009010
		public WaitHandle AsyncWaitHandle
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000104 RID: 260 RVA: 0x0000AE28 File Offset: 0x00009028
		public bool CompletedSynchronously
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000105 RID: 261 RVA: 0x0000AE3C File Offset: 0x0000903C
		public bool IsCompleted
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0400006F RID: 111
		private T data_Renamed;
	}
}
