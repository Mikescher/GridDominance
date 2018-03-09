using GridDominance.DSLEditor.Drawing;
using GridDominance.DSLEditor.Helper;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.SAMScriptParser;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
		private readonly ConcurrentDictionary<int, Tuple<ImageSource, Image>> _imageCache = new ConcurrentDictionary<int, Tuple<ImageSource, Image>>();
		private readonly LevelPreviewPainter levelPainter = new LevelPreviewPainter();

		private ImageSource ReparseLevelFile(string input, bool sim)
		{
			try
			{
				var sw = Stopwatch.StartNew();

				ClearLog();
				AddLog("Start parsing");

				var lp = ParseLevelFile(input, sim);

				_imageCache.Clear();
				_imageCache[-1] = ImageHelper.CreateImageSource(levelPainter.Draw(lp, -1));

				if (sim)
				{
					foreach (var cid in lp.AllCannons.Select(c => c.CannonID))
					{
						_imageCache[cid] = ImageHelper.CreateImageSource(levelPainter.Draw(lp, cid));
					}
				}
				_currentDisplayLevel = lp;

				var img = _imageCache.ContainsKey(_currentHighlightedCannon) ? _imageCache[_currentHighlightedCannon] : _imageCache[-1];

				Application.Current.Dispatcher.Invoke(() => RecreateMapForLevelFile(lp));

				AddLog("File parsed and map drawn in " + sw.ElapsedMilliseconds + "ms");

				return img.Item1;
			}
			catch (ParsingException pe)
			{
				AddLog(pe.ToOutput());
				Console.Out.WriteLine(pe.ToString());
				_currentDisplayLevel = null;

				if (_imageCache.ContainsKey(-1))
				{
					return ImageHelper.CreateImageSource(levelPainter.DrawErrorOverlay(_imageCache[-1].Item2)).Item1;
				}
				
				return ImageHelper.CreateImageSource(levelPainter.Draw(null, -1)).Item1;
			}
			catch (Exception pe)
			{
				AddLog(pe.Message);
				Console.Out.WriteLine(pe.ToString());
				_currentDisplayLevel = null;

				return ImageHelper.CreateImageSource(levelPainter.DrawError()).Item1;
			}
		}

		private void CompileLevel()
		{
			if (!File.Exists(FilePath)) throw new FileNotFoundException(FilePath);

			var lp = ParseLevelFile(Code, true);

			var dir = Path.GetDirectoryName(FilePath);
			var name = Path.GetFileNameWithoutExtension(FilePath) + ".xnb";

			if (string.IsNullOrWhiteSpace(dir)) throw new Exception("dir == null");
			if (string.IsNullOrWhiteSpace(name)) throw new Exception("name == null");

			var outPath = Path.Combine(dir, name);

			byte[] binData;
			using (var ms = new MemoryStream())
			using (var bw = new BinaryWriter(ms))
			{
				lp.BinarySerialize(bw, false, 0, -1, 0);
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

		private LevelBlueprint ParseLevelFile(string input, bool sim)
		{
			input = ReplaceMagicConstantsInLevelFile(input);

			var includesFunc = DSLUtil.GetIncludesFunc(FilePath);

			return DSLUtil.ParseLevelFromString(input, includesFunc, sim);
		}

		private LevelBlueprint ParseLevelFileSafe(string input, bool sim)
		{
			try
			{
				return ParseLevelFile(input, sim);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private LevelBlueprint ParseSpecificLevelFile(string f, bool sim)
		{
			return DSLUtil.ParseLevelFromFile(f, sim);
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
				int tc = TimerCountDown;

				Code = newCode;

				TimerCountDown = tc;
				SelectionStart = selS;
				SelectionLength = selL;
				if (coff >= 0) CaretOffset?.Set(coff);

				Console.WriteLine("Regenerate map");
			}
		}

		private Bitmap CreateOverviewSafe(string path, LevelBlueprint bp)
		{
			var lpp = new LevelPreviewPainter();

			try
			{
				return lpp.DrawOverview(bp);
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

			var maps = Directory
				.EnumerateFiles(folder)
				.Where(p => Path.GetExtension(p).ToLower() == ".gsgraph")
				.Select(p => Tuple.Create(p, ParseGraphFileSafe(File.ReadAllText(p), p)))
				.Where(p => p.Item2 != null)
				.ToList();

			var alllevels = Directory
				.EnumerateFiles(folder)
				.Where(p => Path.GetExtension(p).ToLower() == ".gslevel")
				.Select(p => Tuple.Create(p, ParseLevelFileSafe(File.ReadAllText(p), false)))
				.ToList();

			foreach (var map in maps)
			{
				var levels = alllevels.Where(l => map.Item2.LevelNodes.Any(n => n.LevelID == l.Item2.UniqueID)).ToList();

				OutputOverview(levels, Path.Combine(folder, @"..\..\..\..\Data\overview_"+Path.GetFileNameWithoutExtension(map.Item1)+".png"));

				try
				{
					var fileOut = Path.Combine(folder, @"..\..\..\..\Data\layout_" + Path.GetFileNameWithoutExtension(map.Item1) + ".png");
					var bmp = graphPainter.Draw(map.Item2, FilePath, (s) => { }, null);

					if (!Directory.Exists(Path.GetDirectoryName(fileOut)))
					{
						MessageBox.Show("FolderNotFound: " + fileOut);
						return;
					}

					bmp.Save(fileOut);
				}
				catch (Exception e)
				{
					MessageBox.Show(e.ToString());
					throw;
				}
			}
		}

		private void OutputOverview(List<Tuple<string, LevelBlueprint>> levels, string fileOut)
		{
			var imgs = levels.Select(p => CreateOverviewSafe(p.Item1, p.Item2)).ToList();

			var iw = 0;
			var ih = 0;

			int sx = 0;
			int cy = 0;

			int x = 0;
			int y = 0;
			for (int i = 0; i < imgs.Count; i++)
			{
				if (i % 4 == 0) cy++;

				if (i>0 && i%4==0)
				{
					sx += x;

					x = 0;
					y += IMax(i > 0 ? imgs[i - 1].Height : 0, i > 1 ? imgs[i - 2].Height : 0, i > 2 ? imgs[i - 3].Height : 0, i > 3 ? imgs[i - 4].Height : 0);
					y += 48;
				}

				//g.DrawImageUnscaled(imgs[i], x, y);

				x += imgs[i].Width;

				iw = Math.Max(iw, x);
				ih = Math.Max(ih, y+ imgs[i].Height);

				x += 48;
			}
			sx += x;
			var avg = sx / cy;

			x = 0;
			y = 0;
			for (int i = 0; i < imgs.Count; i++)
			{
				if (i > 0 && i % 4 == 0 || x + 64 > avg)
				{
					x = 0;
					y += IMax(i>0 ? imgs[i - 1].Height : 0, i > 1 ? imgs[i - 2].Height : 0, i > 2 ? imgs[i - 3].Height : 0, i > 3 ? imgs[i - 4].Height : 0);
					y += 48;
				}

				//g.DrawImageUnscaled(imgs[i], x, y);

				x += imgs[i].Width;

				iw = Math.Max(iw, x);
				ih = Math.Max(ih, y + imgs[i].Height);

				x += 48;
			}

			GC.Collect();

			var bmp = new Bitmap(iw, ih);
			x = 0;
			y = 0;
			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.Clear(Color.White);
				for (int i = 0; i < imgs.Count; i++)
				{
					if (i > 0 && i % 4 == 0 || x+64 > avg)
					{
						x = 0;
						y += IMax(i > 0 ? imgs[i - 1].Height : 0, i > 1 ? imgs[i - 2].Height : 0, i > 2 ? imgs[i - 3].Height : 0, i > 3 ? imgs[i - 4].Height : 0);
						y += 48;
					}

					g.DrawImageUnscaled(imgs[i], x, y);

					x += imgs[i].Width;
					x += 48;
				}
			}

			if (!Directory.Exists(Path.GetDirectoryName(fileOut)))
			{
				MessageBox.Show("FolderNotFound: " + fileOut);
				return;
			}

			bmp.Save(fileOut);
		}

		private int IMax(int a, int b, int c, int d) => Math.Max(Math.Max(a, b), Math.Max(c, d));

		public string GenerateASCIIMap(LevelBlueprint l)
		{
			var d = new LevelAsciiDrawer(l);
			d.Calc();
			return d.Get();
		}

		private void OnLevelHover(Point mousePos, double displWidth, double displHeight)
		{
			if (_currentDisplayLevel == null) return;

			double pX = (mousePos.X / displWidth) * _currentDisplayLevel.LevelWidth;
			double pY = (mousePos.Y / displHeight) * _currentDisplayLevel.LevelHeight;

			var vm = new Vector2((float)pX, (float)pY);

			int newHighlight = -1;
			foreach (var cannon in _currentDisplayLevel.AllCannons)
			{
				var vc = new Vector2(cannon.X, cannon.Y);

				if ((vm - vc).Length() < 1.7f * cannon.Diameter / 2f) newHighlight = cannon.CannonID;
			}

			if (newHighlight != _currentHighlightedCannon)
			{
				_currentHighlightedCannon = newHighlight;
				PreviewImage = _imageCache.ContainsKey(_currentHighlightedCannon) ? _imageCache[_currentHighlightedCannon].Item1 : _imageCache[-1].Item1;
			}
		}
	}
}
