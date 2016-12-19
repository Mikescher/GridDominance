using System.Collections.Generic;
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
	public class TexturePackerFile
	{
		[JsonProperty("frames")]
		public List<TexturePackerRegion> Regions { get; set; }

		[JsonProperty("meta")]
		public TexturePackerMeta Metadata { get; set; }
	}
}