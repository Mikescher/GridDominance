using System;
using Microsoft.Xna.Framework.Content.Pipeline;

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
	[ContentProcessor(DisplayName = "TexturePacker Processor - MonoGame.Extended")]
	public class TexturePackerProcessor : ContentProcessor<TexturePackerFile, TexturePackerProcessorResult>
	{
		public override TexturePackerProcessorResult Process(TexturePackerFile input, ContentProcessorContext context)
		{
			try
			{
				var output = new TexturePackerProcessorResult { Data = input };
				return output;
			}
			catch (Exception ex)
			{
				context.Logger.LogMessage("Error {0}", ex);
				throw;
			}
		}
	}
}