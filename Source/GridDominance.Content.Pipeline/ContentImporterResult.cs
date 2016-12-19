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
	public class ContentImporterResult<T>
	{
		public ContentImporterResult(string filePath, T data)
		{
			FilePath = filePath;
			Data = data;
		}

		public string FilePath { get; private set; }
		public T Data { get; private set; }
	}
}