using BitcoinChartsWP.Models;
using BitcoinChartsWP.ViewModels;
using Microsoft.Phone.Controls;

namespace BitcoinChartsWP
{
	public partial class MainPage : PhoneApplicationPage
	{
		public MainPage()
		{
			InitializeComponent();
			DataContext = new MainViewModel(new Bitstamp());

			//DataContext = new MainViewModel(TestData.Create());
		}
	}
}