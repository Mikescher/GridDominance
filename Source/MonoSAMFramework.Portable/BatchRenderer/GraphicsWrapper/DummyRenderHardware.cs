using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSAMFramework.Portable.BatchRenderer.GraphicsWrapper
{
	public class DummyRenderHardware : IRenderHardwareInterface
	{
		public bool IsFullScreen { get; set; }

		public int PreferredBackBufferWidth { get; set; }
		public int PreferredBackBufferHeight { get; set; }

		public bool SynchronizeWithVerticalRetrace { get; set; }

		public DisplayOrientation SupportedOrientations { get; set; }

		public bool IsDummy => true;
		public GraphicsDevice GraphicsDevice => null;
		public ISecureGraphicsDeviceWrapper SecureGraphicsDevice => new DummyGraphicsDeviceWrapper();

		public void ApplyChanges()
		{
			//
		}
	}
}
