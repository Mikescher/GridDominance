using GridDominance.DSLEditor.Drawing;
using GridDominance.DSLEditor.Helper;
using GridDominance.Levelfileformat.Parser;
using GridDominance.Levelformat.Parser;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GridDominance.DSLEditor
{
	public partial class MainWindowViewModel
	{
		private readonly LevelPreviewPainter levelPainter = new LevelPreviewPainter();

		private void ReparseLevelFile()
		{
			try
			{
				var sw = Stopwatch.StartNew();
				var lp = ParseLevelFile();

				Log.Clear();

				PreviewImage = ImageHelper.CreateImageSource(levelPainter.Draw(lp));

				RecreateMapForLevelFile(lp);

				Log.Add("File parsed and map drawn in " + sw.ElapsedMilliseconds + "ms");
			}
			catch (ParsingException pe)
			{
				Log.Add(pe.ToOutput());
				Console.Out.WriteLine(pe.ToString());

				PreviewImage = ImageHelper.CreateImageSource(levelPainter.Draw(null));
			}
			catch (Exception pe)
			{
				Log.Add(pe.Message);
				Console.Out.WriteLine(pe.ToString());

				PreviewImage = ImageHelper.CreateImageSource(levelPainter.Draw(null));
			}
		}

		private void CompileLevel()
		{
			if (!File.Exists(FilePath)) throw new FileNotFoundException(FilePath);

			var lp = ParseLevelFile();

			var dir = Path.GetDirectoryName(FilePath);
			var name = Path.GetFileNameWithoutExtension(FilePath) + ".xnb";

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
				bw.Write((byte)5);    // XNB Version
				bw.Write((byte)0);    // Flags


				bw.Write((UInt32)0x95);

				bw.Write((byte)0x01);
				bw.Write("GridDominance.Levelfileformat.Pipeline.GDLevelReader, GridDominance.Levelformat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
				bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });

				bw.Write(binData);

				bw.Write(new byte[] { 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00, 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00 });
				bw.Write(new byte[] { 0x9B, 0x00, 0x00, 0x00, 0x01 });

				bw.Write("GridDominance.Levelfileformat.Pipeline.GDLevelReader, GridDominance.Levelformat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

				bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });
				bw.Write(new byte[] { 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00, 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00 });
				bw.Write(new byte[] { 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00, 0xA1 });
				bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x01 });

				bw.Write("GridDominance.Levelfileformat.Pipeline.GDLevelReader, GridDominance.Levelformat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

				bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });
			}
		}

		private string ReplaceMagicConstantsInLevelFile(string s)
		{
			while (s.Contains("::UUID::"))
			{
				s = new Regex(Regex.Escape("::UUID::")).Replace(s, Guid.NewGuid().ToString("B").ToUpper(), 1);
			}

			return s;
		}

		private LevelFile ParseLevelFile()
		{
			var input = Code;
			input = ReplaceMagicConstantsInLevelFile(input);

			Func<string, string> includesFunc = x => null;
			if (File.Exists(FilePath))
			{
				var path = Path.GetDirectoryName(FilePath) ?? "";
				var pattern = "*.gsheader";

				var includes = Directory.EnumerateFiles(path, pattern).ToDictionary(p => Path.GetFileName(p) ?? p, p => File.ReadAllText(p, Encoding.UTF8));

				includesFunc = x => includes.FirstOrDefault(p => LevelFile.IsIncludeMatch(p.Key, x)).Value;
			}

			return new LevelFileParser(input, includesFunc).Parse();
		}

		private LevelFile ParseSpecificLevelFile(string f)
		{
			var input = File.ReadAllText(f);
			input = ReplaceMagicConstantsInLevelFile(input);

			Func<string, string> includesFunc = x => null;
			if (File.Exists(FilePath))
			{
				var path = Path.GetDirectoryName(f) ?? "";
				var pattern = "*.gsheader";

				var includes = Directory.EnumerateFiles(path, pattern).ToDictionary(p => Path.GetFileName(p) ?? p, p => File.ReadAllText(p, Encoding.UTF8));

				includesFunc = x => includes.FirstOrDefault(p => LevelFile.IsIncludeMatch(p.Key, x)).Value;
			}

			return new LevelFileParser(input, includesFunc).Parse();
		}

		private void RecreateMapForLevelFile(LevelFile lp)
		{
			var rex = new Regex(@"^#<map>.*^#</map>", RegexOptions.Multiline | RegexOptions.Singleline);

			var newCode = rex.Replace(Code, lp.GenerateASCIIMap());
			int selS = SelectionStart;
			int selL = SelectionLength;

			if (newCode != Code)
			{
				int tc = timerCountDown;

				Code = newCode;

				timerCountDown = tc;
				SelectionStart = selS;
				SelectionLength = selL;

				Console.WriteLine("Regenerate map");
			}
		}

		private void LevelOverview()
		{
			if (!File.Exists(FilePath)) return;
			var folder = Path.GetDirectoryName(FilePath);
			if (!Directory.Exists(folder)) return;

			var imgs = Directory
				.EnumerateFiles(folder)
				.Where(p => Path.GetExtension(p).ToLower() == ".gslevel")
				.Select(f => new LevelPreviewPainter().DrawOverview(ParseSpecificLevelFile(f)))
				.ToList();

			var sw = imgs[0].Width;
			var sh = imgs[0].Height;

			var rc = (imgs.Count + 3) / 4;

			var w = sw * 4 + 48 * 5;
			var h = rc * sh + rc * 48;

			var bmp = new Bitmap(w, h);
			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.Clear(Color.White);
				for (int i = 0; i < imgs.Count; i++)
				{
					var x = (i % 4) * (48 + sw);
					var y = (i / 4) * (48 + sh);

					g.DrawImageUnscaled(imgs[i], x, y);
				}
			}

			FileDialog sfd = new SaveFileDialog
			{
				DefaultExt = ".png",
				InitialDirectory = folder,
				FileName = "overview.png",
			};
			if (sfd.ShowDialog() == true)
			{
				bmp.Save(sfd.FileName);
			}
		}
	}
}
