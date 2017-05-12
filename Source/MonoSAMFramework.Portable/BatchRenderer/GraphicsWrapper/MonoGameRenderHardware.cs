using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSAMFramework.Portable.BatchRenderer.GraphicsWrapper
{
	public class MonoGameRenderHardware : IRenderHardwareInterface
	{
		private readonly GraphicsDeviceManager _gdm;

		public MonoGameRenderHardware(Game g)
		{
			_gdm = new GraphicsDeviceManager(g);
		}

		public bool IsFullScreen { get => _gdm.IsFullScreen; set => _gdm.IsFullScreen = value; }

		public int PreferredBackBufferWidth { get => _gdm.PreferredBackBufferWidth; set => _gdm.PreferredBackBufferWidth = value; }
		public int PreferredBackBufferHeight { get => _gdm.PreferredBackBufferHeight; set => _gdm.PreferredBackBufferHeight = value; }

		public bool SynchronizeWithVerticalRetrace { get => _gdm.SynchronizeWithVerticalRetrace; set => _gdm.SynchronizeWithVerticalRetrace = value; }
		public DisplayOrientation SupportedOrientations { get => _gdm.SupportedOrientations; set => _gdm.SupportedOrientations = value; }

		public bool IsDummy => false;
		public GraphicsDevice GraphicsDevice => _gdm.GraphicsDevice;
		public ISecureGraphicsDeviceWrapper SecureGraphicsDevice => new MonoGameGraphicsDeviceWrapper(_gdm.GraphicsDevice);

		public void ApplyChanges() => _gdm.ApplyChanges();
	}
}
