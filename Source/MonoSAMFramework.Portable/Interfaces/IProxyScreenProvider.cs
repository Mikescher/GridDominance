using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.Interfaces
{
	public interface IProxyScreenProvider
	{
		FRectangle ProxyTargetBounds { get; } // device units
		GameScreen Proxy { get; }
	}
}
