using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BitcoinChartsWP.Resources;
using ReactiveUI;
using BitcoinChartsWP.Models;

namespace BitcoinChartsWP.ViewModels
{
	public class MainViewModel : ReactiveObject
	{
		public MainViewModel()
		{
			new Bitstamp().Subscribe(x =>
				{
					return;
				});
		}
	}
}