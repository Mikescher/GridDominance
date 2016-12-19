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
	public class TexturePackerSize
	{
		[JsonProperty("w")]
		public int Width { get; set; }

		[JsonProperty("h")]
		public int Height { get; set; }

		public override string ToString()
		{
			return string.Format("{0} {1}", Width, Height);
		}
	}
}