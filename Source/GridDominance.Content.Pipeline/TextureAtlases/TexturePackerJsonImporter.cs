using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
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
	[ContentImporter(".json", DefaultProcessor = "TexturePackerProcessor", DisplayName = "TexturePacker JSON Importer - MonoGame.Extended")]
	public class TexturePackerJsonImporter : ContentImporter<TexturePackerFile>
	{
		public override TexturePackerFile Import(string filename, ContentImporterContext context)
		{
			using (var streamReader = new StreamReader(filename))
			using (var jsonReader = new JsonTextReader(streamReader))
			{
				var serializer = new JsonSerializer();
				return serializer.Deserialize<TexturePackerFile>(jsonReader);
			}
		}
	}
}
