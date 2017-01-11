using LevelEditor.Properties;
using MSHC.MVVM;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
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

		public ObservableCollection<string> Log { get; } = new ObservableCollection<string>();

		private string _filePath = "";
		public string FilePath { get { return _filePath; } set {if (value != _filePath) {_filePath = value; OnPropertyChanged();} } }

		private string _code = "";
		public string Code { get { return _code; } set { if (value != _code) { _code = value; OnPropertyChanged(); ResetTimer(); } } }

		private PreviewPainter painter = new PreviewPainter();

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

			repaintTimer.Interval = 20;
			repaintTimer.Tick += (o, te) =>
			{
				pbRefreshTimer.Maximum = TIMER_COOLDOWN;
				pbRefreshTimer.Value = Math.Max(0, Math.Min(TIMER_COOLDOWN, timerCountDown));

				if (--timerCountDown == 0) Reparse();
			};
			repaintTimer.Start();

			//###########################

			Reparse();
		}

		private void Reload()
		{
			
		}

		private void Save()
		{

		}

		private void Compile()
		{

		}

		private void Repaint()
		{

		}

		private void InsertUUID()
		{

		}

		private void ResetTimer()
		{
			timerCountDown = TIMER_COOLDOWN;
		}
	}
}
