using System;
using System.IO;

namespace GridDominance.Content.Pipeline
{
	/// <summary>
	/// 
	/// This class comes originally from [MonoGame.Extended](https://github.com/craftworkgames/MonoGame.Extended/commit/f62467f)
	/// (MIT License)
	/// 
	/// @author MonoGame.Extended
	/// 
	/// </summary>
	public static class PathExtensions
	{
		public static string GetApplicationFullPath(string path)
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
		}
	}
}
