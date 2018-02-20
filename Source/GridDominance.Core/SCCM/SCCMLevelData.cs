using GridDominance.Shared.Screens.NormalGameScreen;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;

namespace GridDominance.Shared.SCCM
{
	public class SCCMLevelData
	{
		public static readonly DSize[] SIZES = {new DSize(16, 10), new DSize(24, 15), new DSize(24, 10), new DSize(16, 20), new DSize(32, 20)};

		public int OnlineID = -1;
		public string Name = "";
		public DSize Size = SIZES[0];
		public FlatAlign9 View = FlatAlign9.CENTER;
		public GameWrapMode Geometry = GameWrapMode.Death;

		public int Width  => Size.Width;
		public int Height => Size.Height;
	}
}
