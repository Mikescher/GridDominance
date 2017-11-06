<Query Kind="Program">
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
  <Namespace>System.Drawing.Drawing2D</Namespace>
</Query>

readonly string[] INPUTS = new[]
{
	@"full\base.png",
	@"full_flat\base.png",
	@"IAB\base.png",
};

readonly int[] SIZES = new[] 
{
	20, 24, 29, 33, 36, 40, 44, 48, 
	50, 58, 60, 62, 70, 71, 72, 76, 
	80, 87, 96, 99, 100, 106, 114, 
	150, 152, 167, 170, 180, 192, 
	210, 300,360, 512, 1024, 2048,
};

void Main()
{
	foreach (var rinput in INPUTS)
	{
		var absinput = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), rinput);
		var folder = Path.GetDirectoryName(absinput);
		
		var img = Image.FromFile(absinput);
		
		foreach (var size in SIZES)
		{
			var output = Path.Combine(folder, $"icon_{size:0000}.png");

			var img2 = FixedSize(img, size, size);
			img2.Save(output);

			output.Dump();
		}
		"".Dump();
		"--------".Dump();
		"".Dump();
	}
}

static Image FixedSize(Image imgPhoto, int Width, int Height)
{
	int sourceWidth = imgPhoto.Width;
	int sourceHeight = imgPhoto.Height;
	int sourceX = 0;
	int sourceY = 0;
	int destX = 0;
	int destY = 0;

	float nPercent = 0;
	float nPercentW = 0;
	float nPercentH = 0;

	nPercentW = ((float)Width / (float)sourceWidth);
	nPercentH = ((float)Height / (float)sourceHeight);
	if (nPercentH < nPercentW)
	{
		nPercent = nPercentH;
		destX = System.Convert.ToInt16((Width - (sourceWidth * nPercent)) / 2);
	}
	else
	{
		nPercent = nPercentW;
		destY = System.Convert.ToInt16((Height - (sourceHeight * nPercent)) / 2);
	}

	int destWidth = (int)(sourceWidth * nPercent);
	int destHeight = (int)(sourceHeight * nPercent);

	Bitmap bmPhoto = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
	bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

	Graphics grPhoto = Graphics.FromImage(bmPhoto);
	grPhoto.Clear(Color.Transparent);
	grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

	grPhoto.DrawImage(imgPhoto, new Rectangle(destX, destY, destWidth, destHeight), new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), GraphicsUnit.Pixel);

	grPhoto.Dispose();
	return bmPhoto;
}