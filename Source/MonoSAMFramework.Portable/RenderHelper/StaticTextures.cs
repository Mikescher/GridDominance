using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using System.Diagnostics;

namespace MonoSAMFramework.Portable.RenderHelper
{
	public static class StaticTextures
	{
		// Guaranteed Icons

		public static TextureRegion2D SinglePixel;
		public static TextureRegion2D PanelBlurEdge;
		public static TextureRegion2D PanelBlurCorner;
		public static TextureRegion2D PanelCorner;

		public static bool Initialized => SinglePixel != null && PanelBlurEdge != null && PanelBlurCorner != null && PanelCorner != null;

		// Additional Icons

		public static TextureRegion2D KeyboardBackspace;
		public static TextureRegion2D KeyboardCaps;
		public static TextureRegion2D KeyboardEnter;
		public static TextureRegion2D KeyboardCircle;

		[Conditional("DEBUG")]
		public static void ThrowIfNotInitialized()
		{
			if (!Initialized) throw new StaticTexturesInitializationException();
		}
	}
}
