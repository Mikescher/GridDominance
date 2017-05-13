using System.Windows;
using System.Windows.Input;

namespace GridDominance.DSLEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			var vm = new MainWindowViewModel();
			DataContext = vm;
			InitializeComponent();
			vm.TimerCountDown = 2;
		}
	}
}
