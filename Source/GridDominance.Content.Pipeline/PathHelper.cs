namespace GridDominance.Content.Pipeline
{
	internal static class PathHelper
	{
		/// <summary>
		/// 
		/// This class comes originally from [MonoGame.Extended](https://github.com/craftworkgames/MonoGame.Extended/commit/f62467f)
		/// (MIT License)
		/// 
		/// @author MonoGame.Extended
		/// 
		/// </summary>
		public static string RemoveExtension(string path)
		{
			var dotIndex = path.LastIndexOf('.');
			return dotIndex > 0 ? path.Substring(0, dotIndex) : path;
		}
	}
}
