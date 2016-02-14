using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Leveleditor
{
	public partial class MainForm : Form
	{
		readonly Bitmap graphicsBuffer = new Bitmap(1024, 640);

		public MainForm()
		{
			RecreateBuffer();

			InitializeComponent();
		}

		private void btnRefresh_Click(object sender, EventArgs e)
		{
			RecreateBuffer();
			canvas.Invalidate();
		}

		private void RecreateBuffer()
		{
			using (Graphics g = Graphics.FromImage(graphicsBuffer))
			{
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.Clear(Color.Black);

				for (int x = 0; x < 8; x++)
				{

					g.DrawLine(Pens.DarkGray, x*128, 0, x*128, 640);
				}
				for (int y = 0; y < 5; y++)
				{
					g.DrawLine(Pens.DarkGray, 0, y * 128, 1024, y * 128);
				}

				g.FillEllipse(Brushes.Red, new Rectangle(10, 10, 32, 32));
			}
		}

		private void canvas_Paint(object sender, PaintEventArgs e)
		{
			Rectangle dest;

			Rectangle src = new Rectangle(0, 0, 1024, 640);
			Rectangle disp = canvas.DisplayRectangle;
			disp.Inflate(-10, -10);

			if (src.Width*1.0/src.Height < disp.Width*1.0/ disp.Height)
				dest = new Rectangle(0, 0, (src.Width * disp.Height) / src.Height, disp.Height);
			else
				dest = new Rectangle(0, 0, disp.Width, (src.Height * disp.Width )/ src.Width);

			dest.X = (canvas.DisplayRectangle.Width - dest.Width)/2;
			dest.Y = (canvas.DisplayRectangle.Height - dest.Height) /2;

			e.Graphics.DrawImage(graphicsBuffer, dest, src, GraphicsUnit.Pixel);
		}
	}
}
