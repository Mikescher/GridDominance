using MonoSAMFramework.Portable.LogProtocol;
using System.Threading.Tasks;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class TaskExtension
	{
		public static void EnsureNoError(this Task task)
		{
			task.ContinueWith(t =>
			{
				if (t.Exception != null && t.Exception.InnerExceptions.Count==1)
					SAMLog.Error("EnsureNoError", t.Exception.InnerExceptions[0]);
				else
					SAMLog.Error("EnsureNoError", t.Exception);
			}, 
			TaskContinuationOptions.OnlyOnFaulted);
		}

		public static void RunAsync(this Task task)
		{
			Task.Run(() => task).EnsureNoError();
		}
	}
}
