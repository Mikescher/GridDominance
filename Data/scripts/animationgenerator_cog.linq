<Query Kind="Program">
  <Namespace>System.Drawing</Namespace>
</Query>

string FILE_IN  = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), @"..\graphics\cannoncog.png");
string FILE_OUT = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), @"..\graphics\cannoncog_animated\cannoncog_{0:000}.png");

const int PIECES = 128;

void Main()
{
	var b = Image.FromFile(FILE_IN);
	var w = b.Width;
	var h = b.Height;

	for (int i = 0; i < PIECES; i++)
	{
		var bp = Generate((Math.PI * 2 * i) / (PIECES-1), b);

		Invert(bp).Dump();

		bp.Save(string.Format(FILE_OUT, i));
    }
}

private Image Generate(double angle, Image source)
{
	var r = new Bitmap(source);

	for (int x = 0; x < source.Width; x++)
	{
		for (int y = 0; y < source.Height; y++)
		{
			var a = Math.Atan2(y - source.Height / 2, x - source.Width / 2);
			a +=  Math.PI;
			a += (3*Math.PI) / 2;
			while (a > Math.PI*2) a -= Math.PI*2;
		
			if (a >= angle && !(Math.PI * 2 == angle))
			{
				r.SetPixel(x, y, Color.Transparent);
			}
		}
	}
	
	return r;
}

public Image Invert(Image bitmapImage0)
{
	byte A, R, G, B;
	Color pixelColor;

	var bitmapImage = new Bitmap(bitmapImage0);

	for (int y = 0; y < bitmapImage.Height; y++)
	{
		for (int x = 0; x < bitmapImage.Width; x++)
		{
			pixelColor = bitmapImage.GetPixel(x, y);
			A = pixelColor.A;
			R = (byte)(255 - pixelColor.R);
			G = (byte)(255 - pixelColor.G);
			B = (byte)(255 - pixelColor.B);
			bitmapImage.SetPixel(x, y, Color.FromArgb((int)A, (int)R, (int)G, (int)B));
		}
	}
	
	return bitmapImage;
}