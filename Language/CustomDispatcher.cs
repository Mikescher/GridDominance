using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace MonoSAMFramework.Portable.Language
{
	public class CustomDispatcher
	{
		private sealed class CustomDispatcherItem
		{
			public Action DispatchAction;
			public bool Executed = false;
		}

		private readonly ConcurrentQueue<CustomDispatcherItem> dispatchQueue = new ConcurrentQueue<CustomDispatcherItem>();

		public void BeginInvoke(Action a)
		{
			var item = new CustomDispatcherItem { DispatchAction = a };
			dispatchQueue.Enqueue(item);
		}

		public void Invoke(Action a)
		{
			var item = new CustomDispatcherItem { DispatchAction = a };
			dispatchQueue.Enqueue(item);
			while (!item.Executed)
			{
				MonoSAMGame.CurrentInst.Bridge.Sleep(0);
			}
		}

		public void Work()
		{
			CustomDispatcherItem item;
			while (dispatchQueue.TryDequeue(out item))
			{
				item.DispatchAction();
				item.Executed = true;
			}
		}
	}
}
