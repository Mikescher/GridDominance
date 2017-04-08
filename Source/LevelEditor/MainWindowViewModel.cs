using GridDominance.Levelformat.Parser;
using Leveleditor;
using LevelEditor.Properties;
using MSHC.WPF.MVVM;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace LevelEditor
{
	public class MainWindowViewModel : ObservableObject
	{
		private const int TIMER_COOLDOWN = 33;

		public ICommand ReloadCommand => new RelayCommand(Reload);
		public ICommand SaveCommand => new RelayCommand(Save);
		public ICommand CompileCommand => new RelayCommand(Compile); 
		public ICommand RepaintCommand => new RelayCommand(Repaint);
		public ICommand UUIDCommand => new RelayCommand(InsertUUID);
		public ICommand ClosingCommand => new RelayCommand<CancelEventArgs>(FormClosing);
		public ICommand EditorChangedCommand => new RelayCommand(ResetTimer);

		public ICommand DropCommand => new RelayCommand<DragEventArgs>(Drop); 
		public ICommand DragCommand => new RelayCommand<DragEventArgs>(DragEnter);

		public ObservableCollection<string> Log { get; } = new ObservableCollection<string>();

		private string _filePath = "";
		public string FilePath { get { return _filePath; } set {if (value != _filePath) {_filePath = value; OnPropertyChanged();} } }

		private string _code = "";
		public string Code { get { return _code; } set { if (value != _code) { _code = value; OnPropertyChanged(); ResetTimer(); } } }

		private int _progressValue = 0;
		public int ProgressValue { get { return _progressValue; } set { _progressValue = value; OnPropertyChanged(); } }

		private int _progressMaximum = 0;
		public int ProgressMaximum { get { return _progressMaximum; } set { _progressMaximum = value; OnPropertyChanged(); } }

		private int _selectionStartLastValid = -1;
		private int _selectionStart = 0;
		public int SelectionStart { get { return _selectionStart; } set { _selectionStart = value; if (value!= -1) { _selectionStartLastValid = value; } OnPropertyChanged();} }

		private int _selectionLength = 0;
		public int SelectionLength { get { return _selectionLength; } set { _selectionLength = value; OnPropertyChanged(); } }

		private ImageSource _previewImage;
		public ImageSource PreviewImage { get { return _previewImage; } set { _previewImage = value; OnPropertyChanged(); } }
		
		private readonly PreviewPainter painter = new PreviewPainter();

		private readonly DispatcherTimer repaintTimer = new DispatcherTimer();
		private int timerCountDown = TIMER_COOLDOWN;

		public MainWindowViewModel()
		{
			if (Environment.GetCommandLineArgs().Count() > 1 && File.Exists(Environment.GetCommandLineArgs()[1]))
			{
				FilePath = Environment.GetCommandLineArgs()[1];
				Code = File.ReadAllText(FilePath);
			}
			else
			{
				Code = Resources.example;
			}

			repaintTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);
			repaintTimer.Tick += (o, te) =>
			{
				ProgressMaximum = TIMER_COOLDOWN;
				ProgressValue = Math.Max(0, Math.Min(TIMER_COOLDOWN, timerCountDown));

				if (--timerCountDown == 0) Reparse();
			};
			repaintTimer.Start();

			//###########################

			Reparse();
		}

		private void Reparse()
		{
			try
			{
				long tmr = Environment.TickCount;
				var lp = ParseLevelFile();

				Log.Clear();

				PreviewImage = ImageHelper.CreateImageSource(painter.Draw(lp));

				RecreateMap(lp);

				Log.Add("File parsed and map drawn in " + (Environment.TickCount - tmr) + "ms");
			}
			catch (ParsingException pe)
			{
				Log.Add(pe.ToOutput());
				Console.Out.WriteLine(pe.ToString());

				PreviewImage = ImageHelper.CreateImageSource(painter.Draw(null));
			}
			catch (Exception pe)
			{
				Log.Add(pe.Message);
				Console.Out.WriteLine(pe.ToString());

				PreviewImage = ImageHelper.CreateImageSource(painter.Draw(null));
			}
		}
		
		private void Reload()
		{
			try
			{
				Code = File.ReadAllText(FilePath);
				Reparse();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		private void Save()
		{
			try
			{
				string iold = Code;
				string inew = ReplaceMagicConstants(Code);
				if (iold != inew) Code = inew;
				File.WriteAllText(FilePath, Code, Encoding.UTF8);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		private void Compile()
		{
			try
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
			catch (Exception exc)
			{
				Log.Add(exc.Message);
				Console.Out.WriteLine(exc.ToString());
			}
		}

		private void Repaint()
		{
			Reparse();
		}

		private void InsertUUID()
		{
			if (SelectionStart == -1)
			{
				Code = Code.Substring(0, _selectionStartLastValid) + "::UUID::" + Code.Substring(_selectionStartLastValid);
				SelectionStart = _selectionStartLastValid;
				SelectionLength = 0;
			}
			else
			{
				Code = Code.Substring(0, SelectionStart) + "::UUID::" + Code.Substring(SelectionStart + SelectionLength);
			}
		}

		private void ResetTimer()
		{
			timerCountDown = TIMER_COOLDOWN;
		}

		private string ReplaceMagicConstants(string s)
		{
			while (s.Contains("::UUID::"))
			{
				s = new Regex(Regex.Escape("::UUID::")).Replace(s, Guid.NewGuid().ToString("N").ToUpper(), 1);
			}

			return s;
		}

		private LevelFile ParseLevelFile()
		{
			var input = Code;
			input = ReplaceMagicConstants(input);

			Func<string, string> includesFunc = x => null;
			if (File.Exists(FilePath))
			{
				var path = Path.GetDirectoryName(FilePath) ?? "";
				var pattern = "*" + Path.GetExtension(FilePath);

				var includes = Directory.EnumerateFiles(path, pattern).ToDictionary(p => Path.GetFileName(p) ?? p, p => File.ReadAllText(p, Encoding.UTF8));

				includesFunc = x => includes.FirstOrDefault(p => LevelFile.IsIncludeMatch(p.Key, x)).Value;
			}

			var lp = new LevelFile(input, includesFunc);
			lp.Parse();

			return lp;
		}

		private void Drop(DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

			if (files != null)
			{
				foreach (string file in files)
				{
					FilePath = file;
					Reload();
				}
			}
		}

		private void DragEnter(DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;
		}

		private void FormClosing(CancelEventArgs e)
		{
			if (File.Exists(FilePath) && File.ReadAllText(FilePath) != Code)
			{
				switch (MessageBox.Show("Save changes ?", "Save?", MessageBoxButton.YesNoCancel))
				{
					case MessageBoxResult.None:
					case MessageBoxResult.Cancel:
						e.Cancel = true;
						return;
					case MessageBoxResult.Yes:
						Save();
						e.Cancel = false;
						return;
					case MessageBoxResult.No:
						e.Cancel = false;
						return;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		private void RecreateMap(LevelFile lp)
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
	}
}
