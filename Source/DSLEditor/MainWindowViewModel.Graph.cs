using GridDominance.DSLEditor.Drawing;
using GridDominance.DSLEditor.Helper;
using GridDominance.Graphfileformat.Blueprint;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using GridDominance.Graphfileformat;
using GridDominance.SAMScriptParser;

namespace GridDominance.DSLEditor
{
	public partial class MainWindowViewModel
	{
		private readonly GraphPreviewPainter graphPainter = new GraphPreviewPainter();

		private void ReparseGraphFile()
		{
			try
			{
				var sw = Stopwatch.StartNew();
				var lp = ParseGraphFile();

				Log.Clear();

				PreviewImage = ImageHelper.CreateImageSource(graphPainter.Draw(lp, FilePath));

				Log.Add("File parsed  in " + sw.ElapsedMilliseconds + "ms");
			}
			catch (ParsingException pe)
			{
				Log.Add(pe.ToOutput());
				Console.Out.WriteLine(pe.ToString());

				PreviewImage = ImageHelper.CreateImageSource(graphPainter.Draw(null, null));
			}
			catch (Exception pe)
			{
				Log.Add(pe.Message);
				Console.Out.WriteLine(pe.ToString());

				PreviewImage = ImageHelper.CreateImageSource(graphPainter.Draw(null, null));
			}
		}

		private void CompileGraph()
		{
			if (!File.Exists(FilePath)) throw new FileNotFoundException(FilePath);

			var lp = ParseGraphFile();

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
				bw.Write("GridDominance.Graphfileformat.Pipeline.GDLevelReader, GridDominance.Levelformat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
				bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });

				bw.Write(binData);

				bw.Write(new byte[] { 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00, 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00 });
				bw.Write(new byte[] { 0x9B, 0x00, 0x00, 0x00, 0x01 });

				bw.Write("GridDominance.Graphfileformat.Pipeline.GDLevelReader, GridDominance.Levelformat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

				bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });
				bw.Write(new byte[] { 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00, 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00 });
				bw.Write(new byte[] { 0x58, 0x4E, 0x42, 0x67, 0x05, 0x00, 0xA1 });
				bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x01 });

				bw.Write("GridDominance.Graphfileformat.Pipeline.GDLevelReader, GridDominance.Levelformat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

				bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });
			}
		}

		private GraphBlueprint ParseGraphFile()
		{
			return new GraphParser(Code).Parse();
		}

		private void UpdateGuids()
		{
			if (!File.Exists(FilePath)) { MessageBox.Show("No root folder"); return; }
			var folder = Path.GetDirectoryName(FilePath);
			if (!Directory.Exists(folder)) { MessageBox.Show("No root folder"); return; }
			if (!folder.ToLower().Trim('\\').EndsWith("GridDominance.Shared\\Content\\levels".ToLower())) { MessageBox.Show("Invalid root folder"); return; }

			var files = Directory.EnumerateFiles(folder).Where(p => Path.GetExtension(p).ToLower() == ".gslevel").ToList();
			var levls = files.Select(ParseSpecificLevelFile).ToList();

			{
				var f0 = Path.Combine(folder, @"..\..\..\GridDominance.Shared\Content\Content.mgcb");

				if (File.Exists(f0))
				{
					var txt0 = File.ReadAllText(f0);
					foreach (var f in files)
					{
						if (!txt0.Contains($"#begin levels/{Path.GetFileName(f)}"))
						{
							txt0 += $"#begin levels/{Path.GetFileName(f)}\r\n";
							txt0 += $"/importer:GDLevelImporter\r\n";
							txt0 += $"/processor:GDLevelProcessor\r\n";
							txt0 += $"/build:levels/{Path.GetFileName(f)}\r\n";
							txt0 += $"\r\n";
						}
					}
					if (File.ReadAllText(f0) != txt0) File.WriteAllText(f0, txt0);
				}
				else
				{
					MessageBox.Show("FileNotFound: " + f0);
				}
			}

			{
				var f1 = Path.Combine(folder, @"..\..\Resources\Levels.cs");

				if (File.Exists(f1))
				{
					var txt1 = File.ReadAllText(f1);
					foreach (var loadstr in files.Select(f => $"LoadLevel(content, \"levels/{Path.GetFileNameWithoutExtension(f)}\");"))
					{
						if (!txt1.Contains(loadstr))
						{
							txt1 = txt1.Replace("/* [MARK_LOAD_LEVEL] */", $"{loadstr}\r\n\t\t\t/* [MARK_LOAD_LEVEL] */");
						}
					}
					if (File.ReadAllText(f1) != txt1) File.WriteAllText(f1, txt1);
				}
				else
				{
					MessageBox.Show("FileNotFound: " + f1);
				}
			}

			{
				var f2 = Path.Combine(folder, @"..\..\..\GridDominance.Server\internals\config_levelids.php");
				StringBuilder txt2 = new StringBuilder();
				txt2.AppendLine("<?php");
				txt2.AppendLine();
				txt2.AppendLine("if(count(get_included_files()) ==1) exit(\"Direct access not permitted.\");");
				txt2.AppendLine();
				txt2.AppendLine("return [");
				foreach (var l in levls) txt2.AppendLine($"\t'{l.UniqueID:B}', // {l.Name, -8} | {l.FullName}");
				txt2.AppendLine("];");

				if (File.Exists(f2))
				{
					File.WriteAllText(f2, txt2.ToString());
				}
				else
				{
					MessageBox.Show("FileNotFound: " + f2);
				}

			}
		}
	}
}
