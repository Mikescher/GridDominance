using System;

namespace MonoSAMFramework.Portable.BatchRenderer
{
	public interface IDebugBatchRenderer : IDisposable
	{
#if DEBUG
		int LastRenderSpriteCount { get;}
		int LastRenderTextCount { get; }
#endif
	}
}
