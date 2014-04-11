using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BitcoinChartsWP.Resources;

namespace BitcoinChartsWP.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public MainViewModel()
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}