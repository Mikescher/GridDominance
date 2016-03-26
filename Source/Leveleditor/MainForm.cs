using GridDominance.Levelformat.Parser;
using ICSharpCode.AvalonEdit;
using Leveleditor.Properties;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Leveleditor
{
	public partial class MainForm : Form
	{
		private const int TIMER_COOLDOWN = 33;
		private readonly Color[] CANNON_COLORS = new Color[] { Color.LightGray, Color.Green, Color.Red, Color.Blue, Color.Yellow, Color.Cyan, Color.Orange, Color.Pink };

		private TextEditor edCode;

		readonly Bitmap graphicsBuffer = new Bitmap(1024, 640);

		private readonly Timer refreshTimer = new Timer();
		private int timerCountDown = TIMER_COOLDOWN;

		public MainForm()
		{
			InitializeComponent();
			var ce = new CodeEditor();
			edCode = ce.EditMain;
			hostCode.Child = ce;
			edCode.TextChanged += textBox1_TextChanged;
		}

		private void btnRefresh_Click(object sender, EventArgs e)
		{
			Reparse();
		}

		private void RecreateBuffer(LevelFile level)
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

					for (int x = 0; x < 16; x++)
					{
						g.DrawLine((x%2==0)? Pens.DarkGray : Pens.DimGray, x * 64, 0, x * 64, 640);
					}
					for (int y = 0; y < 10; y++)
					{
						g.DrawLine((y % 2 == 0) ? Pens.DarkGray : new Pen(Color.FromArgb(88, 88, 88)), 0, y * 64, 1024, y * 64);
					}

					foreach (var c in level.BlueprintCannons)
					{
						var topleftX = (int) (c.X - c.Scale * 64);
						var topleftY = (int) (c.Y - c.Scale * 64);
						var width    = (int)(c.Scale * 128);
						var height   = (int)(c.Scale * 128);

						var rectReal   = new Rectangle(topleftX, topleftY, width, height);
						var rectCircle = new Rectangle(rectReal.Location, rectReal.Size);
						var rectOuter  = new Rectangle(rectReal.Location, rectReal.Size);
						rectCircle.Inflate((width * 48 / 64 - width) / 2, (height * 48 / 64 - height) / 2);
						rectOuter.Inflate((width * 80 / 64 - width) / 2, (height * 80 / 64 - height) / 2);

						var save = g.Save();
						{
							g.TranslateTransform(c.X, c.Y);
							g.RotateTransform(c.Rotation);

							var bHeight = height / 4;
							var bWidth  = width  / 2;
							var bPosX   = width  / 8;

							var br = new Rectangle(bPosX, -bHeight / 2, bWidth, bHeight);

							g.FillRectangle(new SolidBrush(CANNON_COLORS[c.Player]), br);
						}
						g.Restore(save);

						g.FillRectangle(new SolidBrush(Color.FromArgb(64, CANNON_COLORS[c.Player])), rectReal);
						g.FillEllipse(new SolidBrush(CANNON_COLORS[c.Player]), rectCircle);
						g.DrawEllipse(new Pen(Color.Black), rectCircle);
						g.DrawEllipse(new Pen(CANNON_COLORS[c.Player], 2), rectOuter);

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
			try
			{
				var lp = ParseLevelFile();

				edError.Text = string.Empty;

				RecreateBuffer(lp);

				RecreateMap(lp);
			}
			catch (ParsingException pe)
			{
				edError.Text = pe.ToOutput();
				Console.Out.WriteLine(pe.ToString());

				RecreateBuffer(null);
			}
		}

		private LevelFile ParseLevelFile()
		{
			Func<string, string> includesFunc = x => null;
			if (File.Exists(edPath.Text))
			{
				var path = Path.GetDirectoryName(edPath.Text) ?? "";
				var pattern = "*" + Path.GetExtension(edPath.Text);

				var includes = Directory.EnumerateFiles(path, pattern).ToDictionary(p => Path.GetFileName(p) ?? p, p => File.ReadAllText(p, Encoding.UTF8));

				includesFunc = x => includes.FirstOrDefault(p => LevelFile.IsIncludeMatch(p.Key, x)).Value;
			}

			var lp = new LevelFile(edCode.Text, includesFunc);
			lp.Parse();

			return lp;
		}

		private void RecreateMap(LevelFile lp)
		{
			var rex = new Regex(@"^#<map>.*^#</map>", RegexOptions.Multiline | RegexOptions.Singleline);
			
			var newCode = rex.Replace(edCode.Text, lp.GenerateASCIIMap());

			if (newCode != edCode.Text)
			{
				int tc = timerCountDown;
				int selS = edCode.SelectionStart;
				int selL = edCode.SelectionLength;

				edCode.Text = newCode;

				try
				{
					timerCountDown = tc;
					edCode.SelectionStart = selS;
					edCode.SelectionLength = selL;
				}
				catch (Exception) { /* wayne */ }

				Console.WriteLine("Regenerate map");
			}
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			if (Environment.GetCommandLineArgs().Count() > 1 && File.Exists(Environment.GetCommandLineArgs()[1]))
			{
				edPath.Text = Environment.GetCommandLineArgs()[1];
				edCode.Text = File.ReadAllText(edPath.Text);
			}
			else
			{
				edCode.Text = Encoding.UTF8.GetString(Resources.example);
			}

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

		private void canvas_SizeChanged(object sender, EventArgs e)
		{
			Reparse();
		}

		private void btnReload_Click(object sender, EventArgs e)
		{
			edCode.Text = File.ReadAllText(edPath.Text);
			Reparse();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			File.WriteAllText(edPath.Text, edCode.Text, Encoding.UTF8);
		}

		private void edPath_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

			foreach (string file in files)
			{
				edPath.Text = file;
				edCode.Text = File.ReadAllText(edPath.Text);
				Reparse();
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (File.Exists(edPath.Text) && File.ReadAllText(edPath.Text) != edCode.Text)
			{
				switch (MessageBox.Show("Save changes ?", "Save?", MessageBoxButtons.YesNoCancel))
				{
					case DialogResult.None:
					case DialogResult.Cancel:
						e.Cancel = true;
						return;
					case DialogResult.Yes:
						File.WriteAllText(edPath.Text, edCode.Text, Encoding.UTF8);
						e.Cancel = false;
						return;
					case DialogResult.No:
						e.Cancel = false;
						return;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		private void edPath_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
		}

		private void btnCompile_Click(object sender, EventArgs e) // too lazy to include BinaryWriter - or analyze format in depth
		{
			try
			{
				if (! File.Exists(edPath.Text)) throw new FileNotFoundException(edPath.Text);

				var lp = ParseLevelFile();

				var dir = Path.GetDirectoryName(edPath.Text);
				var name = Path.GetFileNameWithoutExtension(edPath.Text) + ".xnb";

				if (string.IsNullOrWhiteSpace(dir)) throw new Exception("dir == null");
				if (string.IsNullOrWhiteSpace(name)) throw new Exception("name == null");

				var outPath = Path.Combine(dir, name);

				byte[] binData;
				using (var ms = new MemoryStream())
				using (var bw = new BinaryWriter(ms))
				{
					lp.BinarySerialize(bw);
					binData = ms.ToArray();
				}

				using (var fs = new FileStream(outPath, FileMode.Create))
				using (var bw = new ExtendedBinaryWriter(fs))
				{
					// Header

					bw.Write('X');
					bw.Write('N');
					bw.Write('B');
					bw.Write('g');        // Target Platform
					bw.Write((byte)5); 	  // XNB Version
					bw.Write((byte)0);    // Flags


					bw.Write((UInt32)0x95);

					bw.Write((byte) 0x01);
					bw.Write("GridDominance.Levelformat.Pipeline.GDLevelReader, GridDominance.Levelformat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
					bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });

					bw.Write(binData);

					bw.Write(new byte[] { 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00, 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00 });
					bw.Write(new byte[] { 0x9B, 0x00, 0x00, 0x00, 0x01});

					bw.Write("GridDominance.Levelformat.Pipeline.GDLevelReader, GridDominance.Levelformat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

					bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });
					bw.Write(new byte[] { 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00, 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00 });
					bw.Write(new byte[] { 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00, 0xA1 });
					bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x01});

					bw.Write("GridDominance.Levelformat.Pipeline.GDLevelReader, GridDominance.Levelformat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

					bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });
				}

			}
			catch (Exception exc)
			{
				edError.Text = exc.ToString();
				Console.Out.WriteLine(exc.ToString());
			}
		}
	}
}
