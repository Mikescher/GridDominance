using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using System.Diagnostics;

namespace MonoSAMFramework.Portable.RenderHelper
{
	public static class StaticTextures
	{
		public static TextureRegion2D SinglePixel;
		public static TextureRegion2D PanelBlurEdge;
		public static TextureRegion2D PanelBlurCorner;
		public static TextureRegion2D PanelCorner;

		public static bool Initialized => SinglePixel != null && PanelBlurEdge != null && PanelBlurCorner != null && PanelCorner != null;

		[Conditional("DEBUG")]
		public static void ThrowIfNotInitialized()
		{
			if (!Initialized) throw new StaticTexturesInitializationException();
		}
	}
}
