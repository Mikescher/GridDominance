using GridDominance.DSLEditor.Helper;
using GridDominance.DSLEditor.Properties;
using GridDominance.Levelfileformat.Blueprint;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
using MSHC.WPF.Extensions.BindingProxies;
using MSHC.WPF.MVVM;
using Ookii.Dialogs.Wpf;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace GridDominance.DSLEditor
{
	public partial class MainWindowViewModel : ObservableObject
	{
		private const int TIMER_COOLDOWN = 33;

		public ICommand ReloadCommand        => new RelayCommand(r => Reload());
		public ICommand SaveCommand          => new RelayCommand(Save);
		public ICommand CompileCommand       => new RelayCommand(Compile); 
		public ICommand PackCommand          => new RelayCommand(Pack); 
		public ICommand RepaintCommand       => new RelayCommand(Repaint);
		public ICommand UUIDCommand          => new RelayCommand(InsertUUID);
		public ICommand ClosingCommand       => new RelayCommand<CancelEventArgs>(FormClosing);
		public ICommand EditorChangedCommand => new RelayCommand(ResetTimer);
		public ICommand HoverCommand         => new RelayCommand<MouseEventArgs>(Hover);
		public ICommand ScreenshotsCommand   => new RelayCommand(CreateScreenshots);
		public ICommand DropCommand          => new RelayCommand<DragEventArgs>(Drop); 
		public ICommand DragCommand          => new RelayCommand<DragEventArgs>(DragEnter);

		public ObservableCollection<string> Log { get; } = new ObservableCollection<string>();

		public TabControl TabCtrl; 

		private string _filePath = "";
		public string FilePath { get { return _filePath; } set {if (value != _filePath) {_filePath = value; OnPropertyChanged();} } }

		private bool _codeDirty = false;
		private string _code = "";
		public string Code { get { return _code; } set { if (value != _code) { _code = value; OnPropertyChanged(); ResetTimer(); _codeDirty = true; } } }

		public string GrammarText { get { return Resources.grammar; } set { /**/ } }

		private int _progressValue = 0;
		public int ProgressValue { get { return _progressValue; } set { _progressValue = value; OnPropertyChanged(); } }

		private int _progressMaximum = 0;
		public int ProgressMaximum { get { return _progressMaximum; } set { _progressMaximum = value; OnPropertyChanged(); } }

		private int _selectionStartLastValid = -1;
		private int _selectionStart = 0;
		public int SelectionStart { get { return _selectionStart; } set { _selectionStart = value; if (value!= -1) { _selectionStartLastValid = value; } OnPropertyChanged();} }
		
		private int _selectionLength = 0;
		public int SelectionLength { get { return _selectionLength; } set { _selectionLength = value; OnPropertyChanged(); } }

		private bool _fullSimulatedRepaint = true;
		public bool FullSimulatedRepaint { get { return _fullSimulatedRepaint; } set { _fullSimulatedRepaint = value; OnPropertyChanged(); } }

		private ImageSource _previewImage;
		public ImageSource PreviewImage { get { return _previewImage; } set { _previewImage = value; OnPropertyChanged(); } }

		private IndirectProperty<int> _caretOffset;
		public IndirectProperty<int> CaretOffset { get { return _caretOffset; } set { _caretOffset = value; OnPropertyChanged(); } }

		private readonly DispatcherTimer repaintTimer = new DispatcherTimer();
		public int TimerCountDown = TIMER_COOLDOWN;

		private bool IsFilePathLevel  => FilePath.ToLower().EndsWith(".gslevel");
		private bool IsFilePathGraph  => FilePath.ToLower().EndsWith(".gsgraph");

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
				ProgressValue = Math.Max(0, Math.Min(TIMER_COOLDOWN, TimerCountDown));

				if (--TimerCountDown == 0) Reparse(true);
			};
			repaintTimer.Start();
		}

		private Thread parseThread;
		private bool isInAsyncParse = false;

		private void Reparse(bool async)
		{
			if (isInAsyncParse && parseThread != null && parseThread.IsAlive)
			{
				AddLog("Abort thread and rerun");
				parseThread.Abort();
				for (int i = 0; i < 100; i++)
				{
					if (!isInAsyncParse) break;
					Thread.Sleep(10);
				}
				if (isInAsyncParse)
				{
					AddLog("Parsing failed - other thread is still running");
					return;
				}
			}

			if (async)
			{
				string code = Code;

				parseThread = new Thread(() =>
				{
					try
					{
						isInAsyncParse = true;

						ImageSource img;
						if (IsFilePathLevel)
							img = ReparseLevelFile(code, FullSimulatedRepaint);
						else if (IsFilePathGraph)
							img = ReparseGraphFile(code);
						else
							throw new Exception("Unknown filetype");

						Application.Current.Dispatcher.BeginInvoke(new Action(() => { PreviewImage = img;  }));
						Application.Current.Dispatcher.BeginInvoke(new Action(UpdateTabsControl));
					}
					catch (Exception exc)
					{
						AddLog("Exception in parsing thread:" + exc.Message);
						Console.Out.WriteLine(exc.ToString());
					}
					finally
					{
						isInAsyncParse = false;
					}
				});
				parseThread.Start();
			}
			else
			{
				try
				{
					if (IsFilePathLevel)
						PreviewImage = ReparseLevelFile(Code, FullSimulatedRepaint);
					else if (IsFilePathGraph)
						PreviewImage = ReparseGraphFile(Code);
					else
						throw new Exception("Unknown filetype");

					Application.Current.Dispatcher.BeginInvoke(new Action(UpdateTabsControl));
				}
				catch (Exception exc)
				{
					Log.Add(exc.Message);
					Console.Out.WriteLine(exc.ToString());
				}
			}
		}

		private void UpdateTabsControl()
		{
			var tabs = new List<Tuple<string, string>>();

			if (DSLUtil.LastParsedIncludedSources != null)
			{
				tabs.AddRange(DSLUtil.LastParsedIncludedSources);
			}

			if (IsFilePathLevel && _currentDisplayLevel != null)
			{
				byte[] binData;
				using (var ms = new MemoryStream())
				using (var bw = new BinaryWriter(ms))
				{
					_currentDisplayLevel.BinarySerialize(bw, false, 0, -1, 0);
					binData = ms.ToArray();
				}

				var str = binData
					.Select(b => $"{b:X2}")
					.Select((s, i) => (i % 16 == 15) ? (s + "\r\n") : (s + " "))
					.Aggregate((a, b) => a + b);

				tabs.Add(Tuple.Create("Binary", str));

			}
			else if (IsFilePathGraph && _currentDisplayGraph != null)
			{
				byte[] binData;
				using (var ms = new MemoryStream())
				using (var bw = new BinaryWriter(ms))
				{
					_currentDisplayGraph.BinarySerialize(bw);
					binData = ms.ToArray();
				}

				var str = binData
					.Select(b => $"{b:X2}")
					.Select((s, i) => (i % 16 == 15) ? (s + "\r\n") : (s + " "))
					.Aggregate((a, b) => a + b);

				tabs.Add(Tuple.Create("Binary", str));
			}

			if (TabCtrl.Items.Count != tabs.Count + 2)
			{
				while (TabCtrl.Items.Count > 2) TabCtrl.Items.RemoveAt(TabCtrl.Items.Count - 1);

				for (int i = 0; i < tabs.Count; i++)
				{
					TabCtrl.Items.Add(new TabItem()
					{
						Header = tabs[i].Item1,
						Content = new ScrollViewer()
						{
							VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
							HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
							Content = new TextBox()
							{
								FontFamily = new FontFamily("Consolas"),
								FontSize = 14,
								IsReadOnly = true,
								IsReadOnlyCaretVisible = true,
								Text = tabs[i].Item2,
								AcceptsReturn = true,
							}
						}
					});
				}
				TabCtrl.SelectedIndex = 0;
			}
			else
			{
				for (int i = 0; i < tabs.Count; i++)
				{
					((TabItem)TabCtrl.Items[i + 2]).Header = tabs[i].Item1;
					((TextBox)((ScrollViewer)((TabItem)TabCtrl.Items[i + 2]).Content).Content).Text = tabs[i].Item2;
				}
			}
		}

		private void AddLog(string msg)
		{
			var app = Application.Current;
			if (app == null) return;

			if (app.Dispatcher.CheckAccess())
			{
				Log.Add(msg);
			}
			else
			{
				app.Dispatcher.Invoke(() => Log.Add(msg));
			}
		}

		private void ClearLog()
		{
			if (Application.Current == null) return;

			if (Application.Current.Dispatcher.CheckAccess())
			{
				Log.Clear();
			}
			else
			{
				Application.Current.Dispatcher.Invoke(() => Log.Clear());
			}
		}

		private void Reload(bool ask = true, bool async = false)
		{
			if (ask && !ConditionalSave()) return;
			try
			{
				Code = File.ReadAllText(FilePath);
				_codeDirty = false;
				Reparse(async);
				TimerCountDown = -99;
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
			Reparse(false);
		}

		private void InsertUUID()
		{
			int coff = CaretOffset?.Get() ?? -1;

			if (SelectionStart <= 0)
			{
				if (coff != -1)
				{
					Code = Code.Substring(0, coff) + "::UUID::" + Code.Substring(coff);
					SelectionStart = coff;
					SelectionLength = 0;
				}
				else
				{
					Code = Code.Substring(0, _selectionStartLastValid) + "::UUID::" + Code.Substring(_selectionStartLastValid);
					SelectionStart = _selectionStartLastValid;
					SelectionLength = 0;
				}
			}
			else
			{
				Code = Code.Substring(0, SelectionStart) + "::UUID::" + Code.Substring(SelectionStart + SelectionLength);
			}

			if (coff >= 0) CaretOffset?.Set(coff);
		}

		private void ResetTimer()
		{
			TimerCountDown = TIMER_COOLDOWN;
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
					PreviewImage = null;
					Reload(false, true);
					return;
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
					case MessageBoxResult.OK:
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

		private void Pack()
		{
			try
			{
				LevelOverview();
			}
			catch (Exception e)
			{
				MessageBox.Show("LevelOverview failed:\r\n" + e);
			}

			try
			{
				UpdateSourceFiles();
			}
			catch (Exception e)
			{
				MessageBox.Show("UpdateSourceFiles failed:\r\n" + e);
			}
		}

		private void Hover(MouseEventArgs a)
		{
			var src = a.Source as Image;
			if (IsFilePathLevel && src != null)
			{
				OnLevelHover(a.GetPosition(src), src.ActualWidth, src.ActualHeight);
			}
		}

		private void CreateScreenshots(object obj)
		{
			try
			{
				var ofd = new OpenFileDialog();
				ofd.Filter = "Userlevels Backup (*.tgz)|*.tgz";
				ofd.Title = "Open Userlevels backup";
				if (ofd.ShowDialog() != true) return;

				var sfd = new VistaFolderBrowserDialog();
				sfd.UseDescriptionForTitle = true;
				sfd.Description = "Save screenshots to directory";
				if (sfd.ShowDialog() != true) return;
				
				List<Tuple<string, LevelBlueprint>> prints = new List<Tuple<string, LevelBlueprint>>();

				using (Stream stream = File.OpenRead(ofd.FileName))
				{
					var reader = ReaderFactory.Open(stream);
					while (reader.MoveToNextEntry())
					{
						if (!reader.Entry.IsDirectory)
						{
							reader.WriteEntryToDirectory(sfd.SelectedPath, new ExtractionOptions() { ExtractFullPath = false, Overwrite = true });

							var file = Path.Combine(sfd.SelectedPath, Path.GetFileName(reader.Entry.Key.Replace('/', '\\')));
							var bp = new LevelBlueprint();
					
							using (var ms = new MemoryStream(File.ReadAllBytes(file)))
							{
								using (var br = new BinaryReader(ms))
								{
									bp.BinaryDeserialize(br);
								}
							}

							prints.Add(Tuple.Create(file, bp));

							File.Delete(file);
						}
					}
				}

				foreach (var lvl in prints)
				{
					var bmp = CreateOverviewSafe(Path.GetFileName(lvl.Item1), lvl.Item2);
					bmp.Save(lvl.Item1+".png");
				}

				MessageBox.Show("Success");
			}
			catch (Exception e)
			{
				MessageBox.Show("LevelOverview failed:\r\n" + e);
			}
		}
		
		static IEnumerable<string> EnumerateFilesDeep(string path)
		{
			Queue<string> queue = new Queue<string>();
			queue.Enqueue(path);
			while (queue.Count > 0)
			{
				path = queue.Dequeue();
				
				foreach (string subDir in Directory.GetDirectories(path))
				{
					queue.Enqueue(subDir);
				}

				string[] files = Directory.GetFiles(path);

				if (files != null)
				{
					for (int i = 0; i < files.Length; i++)
					{
						yield return files[i];
					}
				}
			}
		}
	}
}
