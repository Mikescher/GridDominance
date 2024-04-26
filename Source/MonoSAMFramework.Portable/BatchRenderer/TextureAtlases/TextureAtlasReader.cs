using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.BatchRenderer.TextureAtlases
{
	/// <summary>
	/// 
	/// This class comes originally from [MonoGame.Extended](https://github.com/craftworkgames/MonoGame.Extended/commit/f62467f)
	/// (MIT License)
	/// 
	/// @author MonoGame.Extended
	/// 
	/// </summary>
	public class TextureAtlasReader : ContentTypeReader<TextureAtlas>
	{
		protected override TextureAtlas Read(ContentReader reader, TextureAtlas existingInstance)
		{
			var assetName = GetRelativeAssetPath(reader, reader.ReadString());
			var texture = reader.ContentManager.Load<Texture2D>(assetName);
			var atlas = new TextureAtlas(texture);

			var regionCount = reader.ReadInt32();

			for (var i = 0; i < regionCount; i++)
			{
				atlas.CreateRegion(
					name: reader.ReadString(),
					x: reader.ReadInt32(),
					y: reader.ReadInt32(),
					width: reader.ReadInt32(),
					height: reader.ReadInt32());
			}

			return atlas;
		}

		private string GetRelativeAssetPath(ContentReader contentReader, string relativePath)
		{
			var assetName = contentReader.AssetName;
			var assetNodes = assetName.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
			var relativeNodes = relativePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
			var relativeIndex = assetNodes.Length - 1;
			var newPathNodes = new List<string>();

			foreach (var relativeNode in relativeNodes)
			{
				if (relativeNode == "..")
					relativeIndex--;
				else
					newPathNodes.Add(relativeNode);
			}

			var values = assetNodes
				.Take(relativeIndex)
				.Concat(newPathNodes)
				.ToArray();

			return string.Join("/", values);
		}
	}
}