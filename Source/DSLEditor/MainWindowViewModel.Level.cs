using GridDominance.DSLEditor.Drawing;
using GridDominance.DSLEditor.Helper;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.SAMScriptParser;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;

namespace GridDominance.DSLEditor
{
	public partial class MainWindowViewModel
	{
		private int _currentHighlightedCannon = -1;
		private LevelBlueprint _currentDisplayLevel = null;
		private readonly ConcurrentDictionary<int, ImageSource> _imageCache = new ConcurrentDictionary<int, ImageSource>();
		private readonly LevelPreviewPainter levelPainter = new LevelPreviewPainter();

		private ImageSource ReparseLevelFile(string input)
		{
			try
			{
				var sw = Stopwatch.StartNew();

				ClearLog();
				AddLog("Start parsing");

				var lp = ParseLevelFile(input);

				_imageCache.Clear();
				_imageCache[-1] = ImageHelper.CreateImageSource(levelPainter.Draw(lp, -1));
				foreach (var cid in lp.BlueprintCannons.Select(c => c.CannonID))
				{
					_imageCache[cid] = ImageHelper.CreateImageSource(levelPainter.Draw(lp, cid));
				}
				_currentDisplayLevel = lp;

				var img = _imageCache.ContainsKey(_currentHighlightedCannon) ? _imageCache[_currentHighlightedCannon] : _imageCache[-1];

				Application.Current.Dispatcher.Invoke(() => RecreateMapForLevelFile(lp));

				AddLog("File parsed and map drawn in " + sw.ElapsedMilliseconds + "ms");

				return img;
			}
			catch (ParsingException pe)
			{
				AddLog(pe.ToOutput());
				Console.Out.WriteLine(pe.ToString());

				return ImageHelper.CreateImageSource(levelPainter.Draw(null, -1));
			}
			catch (Exception pe)
			{
				AddLog(pe.Message);
				Console.Out.WriteLine(pe.ToString());

				return ImageHelper.CreateImageSource(levelPainter.Draw(null, -1));
			}
		}

		private void CompileLevel()
		{
			if (!File.Exists(FilePath)) throw new FileNotFoundException(FilePath);

			var lp = ParseLevelFile(Code);

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
				bw.Write("GridDominance.Levelfileformat.Pipeline.GDLevelReader, GridDominance.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
				bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });

				bw.Write(binData);

				bw.Write(new byte[] { 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00, 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00 });
				bw.Write(new byte[] { 0x9B, 0x00, 0x00, 0x00, 0x01 });

				bw.Write("GridDominance.Levelfileformat.Pipeline.GDLevelReader, GridDominance.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

				bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });
				bw.Write(new byte[] { 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00, 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00 });
				bw.Write(new byte[] { 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00, 0xA1 });
				bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x01 });

				bw.Write("GridDominance.Levelfileformat.Pipeline.GDLevelReader, GridDominance.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

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

		private LevelBlueprint ParseLevelFile(string input)
		{
			input = ReplaceMagicConstantsInLevelFile(input);

			Func<string, string> includesFunc = x => null;
			if (File.Exists(FilePath))
			{
				var path = Path.GetDirectoryName(FilePath) ?? "";
				var pattern = "*.gsheader";

				var includes = Directory.EnumerateFiles(path, pattern).ToDictionary(p => Path.GetFileName(p) ?? p, p => File.ReadAllText(p, Encoding.UTF8));

				includesFunc = x => includes.FirstOrDefault(p => LevelBlueprint.IsIncludeMatch(p.Key, x)).Value;
			}

			return DSLUtil.ParseLevelFromString(input, includesFunc);
		}

		private LevelBlueprint ParseSpecificLevelFile(string f)
		{
			return DSLUtil.ParseLevelFromFile(f);
		}

		private void RecreateMapForLevelFile(LevelBlueprint lp)
		{
			var rex = new Regex(@"^#<map>.*^#</map>", RegexOptions.Multiline | RegexOptions.Singleline);

			var newCode = rex.Replace(Code, GenerateASCIIMap(lp));
			int selS = SelectionStart;
			int selL = SelectionLength;
			int coff = CaretOffset?.Get() ?? -1;

			if (newCode != Code)
			{
				int tc = timerCountDown;

				Code = newCode;

				timerCountDown = tc;
				SelectionStart = selS;
				SelectionLength = selL;
				if (coff >= 0) CaretOffset?.Set(coff);

				Console.WriteLine("Regenerate map");
			}
		}

		private Bitmap CreateOverviewSafe(string path)
		{
			var lpp = new LevelPreviewPainter();

			try
			{
				return lpp.DrawOverview(ParseSpecificLevelFile(path));
			}
			catch (Exception)
			{
				return lpp.DrawOverviewError(Path.GetFileName(path));
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
				.Select(CreateOverviewSafe)
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


			var fileOut = Path.Combine(folder, @"..\..\..\..\Data\overview.png");

			if (!File.Exists(fileOut))
			{
				MessageBox.Show("FileNotFound: " + fileOut);
				return;
			}

			bmp.Save(fileOut);
		}

		public string GenerateASCIIMap(LevelBlueprint l)
		{
			var d = new LevelAsciiDrawer(l);
			d.Calc();
			return d.Get();
		}

		private void OnLevelHover(Point mousePos, double displWidth, double displHeight)
		{
			if (_currentDisplayLevel == null) return;

			double pX = (mousePos.X / displWidth) * 1024;
			double pY = (mousePos.Y / displHeight) * 640;

			var vm = new Vector2((float)pX, (float)pY);

			int newHighlight = -1;
			foreach (var cannon in _currentDisplayLevel.BlueprintCannons)
			{
				var vc = new Vector2(cannon.X, cannon.Y);

				if ((vm - vc).Length() < 1.7f * cannon.Diameter / 2f) newHighlight = cannon.CannonID;
			}

			if (newHighlight != _currentHighlightedCannon)
			{
				_currentHighlightedCannon = newHighlight;
				PreviewImage = _imageCache.ContainsKey(_currentHighlightedCannon) ? _imageCache[_currentHighlightedCannon] : _imageCache[-1];
			}
		}
	}
}
