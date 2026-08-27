using System;
using System.Threading;

namespace Infosciences.Sage
{
	internal class CompletedAsyncResult<T> : IAsyncResult
	{
		public CompletedAsyncResult(T data)
		{
			this.data_Renamed = data;
		}
		public T Data
		{
			get
			{
				return this.data_Renamed;
			}
		}
		public object AsyncState
		{
			get
			{
				return this.data_Renamed;
			}
		}
		public WaitHandle AsyncWaitHandle
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}
		public bool CompletedSynchronously
		{
			get
			{
				return true;
			}
		}
		public bool IsCompleted
		{
			get
			{
				return true;
			}
		}
		private T data_Renamed;
	}
}
