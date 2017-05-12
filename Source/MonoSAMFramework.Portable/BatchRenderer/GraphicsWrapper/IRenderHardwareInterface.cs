
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSAMFramework.Portable.BatchRenderer.GraphicsWrapper
{
	public interface IRenderHardwareInterface
	{
		bool IsFullScreen { get; set; }
		bool IsDummy { get; }

		int PreferredBackBufferWidth { get; set; }
		int PreferredBackBufferHeight { get; set; }

		bool SynchronizeWithVerticalRetrace { get; set; }

		DisplayOrientation SupportedOrientations { get; set; }

		GraphicsDevice GraphicsDevice { get; }
		ISecureGraphicsDeviceWrapper SecureGraphicsDevice { get; }

		void ApplyChanges();
	}
}
