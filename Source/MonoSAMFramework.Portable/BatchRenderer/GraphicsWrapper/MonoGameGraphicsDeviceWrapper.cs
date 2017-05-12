using Microsoft.Xna.Framework.Graphics;

namespace MonoSAMFramework.Portable.BatchRenderer.GraphicsWrapper
{
	public class MonoGameGraphicsDeviceWrapper : ISecureGraphicsDeviceWrapper
	{
		private readonly GraphicsDevice _gd;

		public MonoGameGraphicsDeviceWrapper(GraphicsDevice gd)
		{
			_gd = gd;
		}

		public Viewport Viewport { get => _gd.Viewport; set => _gd.Viewport = value; }
	}
}
