using Leveleditor.Parser;
using Leveleditor.Properties;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace Leveleditor
{
	public partial class MainForm : Form
	{
		private const int TIMER_COOLDOWN = 33;
		private readonly Color[] CANNON_COLORS = new Color[] { Color.LightGray, Color.Green, Color.Red, Color.Blue, Color.Yellow, Color.Cyan, Color.Orange, Color.Pink };

		readonly Bitmap graphicsBuffer = new Bitmap(1024, 640);

		private readonly Timer refreshTimer = new Timer();
		private int timerCountDown = TIMER_COOLDOWN;

		public MainForm()
		{
			InitializeComponent();
		}

		private void btnRefresh_Click(object sender, EventArgs e)
		{
			Reparse();
		}

		private void RecreateBuffer(Levelparser level)
		{
			timerCountDown = -1;

			using (Graphics g = Graphics.FromImage(graphicsBuffer))
			{
				if (level == null)
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.Clear(Color.OrangeRed);

					g.DrawLine(new Pen(Color.DarkRed, 32), 0, 0, 1024, 640);
					g.DrawLine(new Pen(Color.DarkRed, 32), 1024, 0, 0, 640);
				}
				else
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.Clear(Color.Black);

					for (int x = 0; x < 8; x++)
					{
						g.DrawLine(Pens.DarkGray, x * 128, 0, x * 128, 640);
					}
					for (int y = 0; y < 5; y++)
					{
						g.DrawLine(Pens.DarkGray, 0, y * 128, 1024, y * 128);
					}

					foreach (var c in level.BlueprintCannons)
					{
						g.FillEllipse(new SolidBrush(CANNON_COLORS[c.Player]), new Rectangle((int) (c.X - c.Radius), (int)(c.Y - c.Radius),(int)(c.Radius * 2), (int)(c.Radius * 2)));
					}

				}
			}

			canvas?.BeginInvoke(new Action(() => canvas.Invalidate()));
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

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			timerCountDown = TIMER_COOLDOWN;
		}

		private void Reparse()
		{
			var lp = new Levelparser(edCode.Text);

			try
			{
				lp.Parse();

				edError.Text = string.Empty;

				RecreateBuffer(lp);
			}
			catch (ParsingException pe)
			{
				edError.Text = pe.ToOutput();
				Console.Out.WriteLine(pe.ToString());

				RecreateBuffer(null);
			}
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			edCode.Text = Encoding.UTF8.GetString(Resources.example);

			//###########################

			refreshTimer.Interval = 20;
			refreshTimer.Tick += (o, te) =>
			{
				pbRefreshTimer.Maximum = TIMER_COOLDOWN;
				pbRefreshTimer.Value = Math.Max(0, Math.Min(TIMER_COOLDOWN, timerCountDown));

				if (--timerCountDown == 0) Reparse();
			};
			refreshTimer.Start();

			//###########################

			Reparse();
		}
	}
}
