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
	public class TexturePackerRegion
	{
		[JsonProperty("filename")]
		public string Filename { get; set; }

		[JsonProperty("frame")]
		public TexturePackerRectangle Frame { get; set; }

		[JsonProperty("rotated")]
		public bool IsRotated { get; set; }

		[JsonProperty("trimmed")]
		public bool IsTrimmed { get; set; }

		[JsonProperty("spriteSourceSize")]
		public TexturePackerRectangle SourceRectangle { get; set; }

		[JsonProperty("sourceSize")]
		public TexturePackerSize SourceSize { get; set; }

		[JsonProperty("pivot")]
		public TexturePackerPoint PivotPoint { get; set; }

		public override string ToString()
		{
			return string.Format("{0} {1}", Filename, Frame);
		}
	}
}