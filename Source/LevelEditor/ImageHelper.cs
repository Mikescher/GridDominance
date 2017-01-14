using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;

namespace LevelEditor
{
	public static class ImageHelper
	{
		// http://stackoverflow.com/a/3427387/1761622
		public static ImageSource CreateImageSource(System.Drawing.Image image)
		{
			var bitmap = new System.Windows.Media.Imaging.BitmapImage();
			bitmap.BeginInit();
			MemoryStream memoryStream = new MemoryStream();
			image.Save(memoryStream, ImageFormat.Bmp);
			memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
			bitmap.StreamSource = memoryStream;
			bitmap.EndInit();
			return bitmap;
		}
	}
}
