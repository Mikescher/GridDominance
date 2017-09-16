using MonoSAMFramework.Portable.LogProtocol;
using System.Threading.Tasks;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class TaskExtension
    {
        public static void EnsureNoError(this Task task)
        {
            task.ContinueWith(t => { SAMLog.Error("EnsureNoError", t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public static void RunAsync(this Task task)
		{
			Task.Run(() => task).EnsureNoError();
		}
	}
}
