using Newtonsoft.Json;

namespace GridDominance.Content.Pipeline.TextureAtlases
{
	/// <summary>
	/// 
	/// This class comes originally from [MonoGame.Extended](https://github.com/craftworkgames/MonoGame.Extended/commit/f62467f)
	/// (MIT License)
	/// 
	/// @author MonoGame.Extended
	/// 
	/// </summary>
	public class TexturePackerPoint
	{
		[JsonProperty("x")]
		public double X { get; set; }

		[JsonProperty("y")]
		public double Y { get; set; }

		public override string ToString()
		{
			return string.Format("{0} {1}", X, Y);
		}

	}
}