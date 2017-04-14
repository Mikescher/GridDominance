using GridDominance.DSLEditor.Properties;
using MSHC.WPF.MVVM;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace GridDominance.DSLEditor
{
	public partial class MainWindowViewModel : ObservableObject
	{
		private const int TIMER_COOLDOWN = 33;

		public ICommand ReloadCommand => new RelayCommand(r => Reload());
		public ICommand SaveCommand => new RelayCommand(Save);
		public ICommand CompileCommand => new RelayCommand(Compile); 
		public ICommand OverviewCommand => new RelayCommand(LevelOverview);
		public ICommand RepaintCommand => new RelayCommand(Repaint);
		public ICommand UUIDCommand => new RelayCommand(InsertUUID);
		public ICommand ClosingCommand => new RelayCommand<CancelEventArgs>(FormClosing);
		public ICommand EditorChangedCommand => new RelayCommand(ResetTimer);

		public ICommand DropCommand => new RelayCommand<DragEventArgs>(Drop); 
		public ICommand DragCommand => new RelayCommand<DragEventArgs>(DragEnter);

		public ObservableCollection<string> Log { get; } = new ObservableCollection<string>();

		private string _filePath = "";
		public string FilePath { get { return _filePath; } set {if (value != _filePath) {_filePath = value; OnPropertyChanged();} } }

		private bool _codeDirty = false;
		private string _code = "";
		public string Code { get { return _code; } set { if (value != _code) { _code = value; OnPropertyChanged(); ResetTimer(); _codeDirty = true; } } }

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

		private readonly DispatcherTimer repaintTimer = new DispatcherTimer();
		private int timerCountDown = TIMER_COOLDOWN;

		private bool IsFilePathLevel => FilePath.ToLower().EndsWith(".gslevel");
		private bool IsFilePathGraph => FilePath.ToLower().EndsWith(".gsgraph");

		public MainWindowViewModel()
		{
			if (Environment.GetCommandLineArgs().Count() > 1 && File.Exists(Environment.GetCommandLineArgs()[1]))
			{
				FilePath = Environment.GetCommandLineArgs()[1];
				Code = File.ReadAllText(FilePath);
			}
			else
			{
				
				//Code = Resources.example_graph; FilePath = "example.gsgraph";
				Code = Resources.example_level; FilePath = "example.gslevel";
			}
			_codeDirty = false;

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
			Log.Clear();

			try
			{
				if (IsFilePathLevel)
					ReparseLevelFile();
				else if (IsFilePathGraph)
					ReparseGraphFile();
				else
					throw new Exception("Unknown filetype");
			}
			catch (Exception exc)
			{
				Log.Add(exc.Message);
				Console.Out.WriteLine(exc.ToString());
			}
		}

		private void Reload(bool ask = true)
		{
			if (ask && !ConditionalSave()) return;
			try
			{
				Code = File.ReadAllText(FilePath);
				_codeDirty = false;
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
				if (!File.Exists(FilePath))
				{
					MessageBox.Show("Filepath does not exist");
					return;
				}

				string iold = Code;
				string inew = ReplaceMagicConstantsInLevelFile(Code);
				if (iold != inew) Code = inew;
				File.WriteAllText(FilePath, Code, Encoding.UTF8);
				_codeDirty = false;
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
				if (IsFilePathLevel)
					CompileLevel();
				else if (IsFilePathGraph)
					CompileGraph();
				else
					throw new Exception("Unknown filetype");
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

		private void Drop(DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

			if (files != null)
			{
				foreach (string file in files)
				{
					if (!ConditionalSave()) return;
					FilePath = file;
					Reload(false);
				}
			}
		}

		private void DragEnter(DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;
		}

		private void FormClosing(CancelEventArgs e)
		{
			e.Cancel = !ConditionalSave();
		}

		private bool ConditionalSave()
		{
			if (File.Exists(FilePath) && _codeDirty)
			{
				switch (MessageBox.Show($"Save changes to {Path.GetFileName(FilePath)} ?", "Save?", MessageBoxButton.YesNoCancel))
				{
					case MessageBoxResult.None:
					case MessageBoxResult.Cancel:
						return false;
					case MessageBoxResult.Yes:
						Save();
						return true;
					case MessageBoxResult.No:
						return true;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			return true;
		}
	}
}
