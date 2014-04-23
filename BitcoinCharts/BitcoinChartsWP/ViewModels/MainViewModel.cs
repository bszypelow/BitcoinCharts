using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BitcoinChartsWP.Resources;
using ReactiveUI;
using BitcoinChartsWP.Models;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading;
using System.Reactive.Concurrency;

namespace BitcoinChartsWP.ViewModels
{
	public class MainViewModel : ReactiveObject
	{
		public ObservableCollection<Candle> Candles { get; set; }

		public MainViewModel(IObservable<Trade> source)
		{
			this.Candles = new ObservableCollection<Candle>();

			var scheduler = new HistoricalScheduler(DateTimeOffset.UtcNow.AddHours(-1).AddMinutes(-1));
			source.Subscribe(t => scheduler.AdvanceTo(t.Timestamp));

			var windows = source
				.Select(t => t.Price)
				.Window(TimeSpan.FromMinutes(5), scheduler);

			var candles = windows.Select(w => new
				{
					Lo = w.Scan(0m, (min, current) => min > current ? current : min),
					Hi = w.Scan(0m, (max, current) => max < current ? current : max),
					Open = w.Take(1),
					Close = w
				});

			candles.Subscribe(c =>
				{
					this.Candles.Add(new Candle(c.Hi, c.Lo, c.Open, c.Close));
				});
		}
	}
}