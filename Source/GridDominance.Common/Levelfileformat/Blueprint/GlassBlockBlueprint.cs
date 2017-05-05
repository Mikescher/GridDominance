namespace GridDominance.Levelfileformat.Blueprint
{
	public struct GlassBlockBlueprint
	{
		public const float DEFAULT_WIDTH = 24; // TILE_WIDTH * 0.333f

		public readonly float X; // center
		public readonly float Y;
		public readonly float Width;
		public readonly float Height;

		public GlassBlockBlueprint(float x, float y, float w, float h)
		{
			X = x;
			Y = y;
			Width = w;
			Height = h;
		}
	}
}
