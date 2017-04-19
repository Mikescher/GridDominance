using GridDominance.DSLEditor.Drawing;
using GridDominance.DSLEditor.Helper;
using GridDominance.Graphfileformat.Blueprint;
using System;
using System.Diagnostics;
using System.IO;
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
	}
}
