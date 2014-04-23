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
using System.Reactive.Subjects;

namespace BitcoinChartsWP.ViewModels
{
	public class MainViewModel : ReactiveObject
	{
		public IReactiveDerivedList<Candle> Candles { get; set; }

		public MainViewModel(IConnectableObservable<Trade> source)
		{
			var scheduler = new HistoricalScheduler(DateTimeOffset.UtcNow.AddMinutes(-61));
			source.Subscribe(t => scheduler.AdvanceTo(t.Timestamp));

			var windows = source
				.Select(t => t.Price)
				.Window(TimeSpan.FromMinutes(5), scheduler);

			var candles = windows
				.Select(w => new Candle(
					lo: w.Scan(0m, (min, current) => min > current ? current : min),
					hi: w.Scan(0m, (max, current) => max < current ? current : max),
					open: w.Take(1),
					close: w
				));

			this.Candles = candles.CreateCollection();

			source.Connect();
		}
	}
}