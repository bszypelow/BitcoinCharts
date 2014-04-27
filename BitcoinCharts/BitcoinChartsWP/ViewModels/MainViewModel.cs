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
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Diagnostics;
using System.Collections.Generic;

namespace BitcoinChartsWP.ViewModels
{
	public class MainViewModel : ReactiveObject
	{
		public PlotModel Chart { get; set; }

		public IReactiveDerivedList<Candle> Candles { get; set; }

		public MainViewModel(IConnectableObservable<Trade> source)
		{
			var scheduler = new HistoricalScheduler(DateTimeOffset.UtcNow.AddMinutes(-61));
			source.Subscribe(t => scheduler.AdvanceTo(t.Timestamp));
			
			this.Candles = source
				.Select(t => (double)t.Price)
				.Window(TimeSpan.FromMinutes(5), scheduler)
				.Select(w => new Candle(
					time: new DateTime(scheduler.Clock.Ticks),
					lo: w.Scan(double.MaxValue, (min, current) => min > current ? current : min),
					hi: w.Scan(double.MinValue, (max, current) => max < current ? current : max),
					open: w.Take(1),
					close: w))
				.CreateCollection();

			this.Chart = new PlotModel();
			this.Chart.Axes.Add(new DateTimeAxis { StringFormat = "hh:mm" });
			this.Chart.Axes.Add(new LinearAxis());
			this.Chart.Series.Add(new CandleStickSeries
			{
				ItemsSource = this.Candles,
				DataFieldX = "Time",
				DataFieldHigh = "Hi",
				DataFieldLow = "Lo",
				DataFieldOpen = "Open",
				DataFieldClose = "Close",
				CandleWidth = 12,
				StrokeThickness= 3,
				IncreasingFill = OxyColors.DarkGreen,
				DecreasingFill = OxyColors.Red,
				Color = OxyColors.Black
			});

			source
				.Sample(TimeSpan.FromSeconds(0.5))
				.Subscribe(p => this.Chart.InvalidatePlot(updateData: true));

			source.Connect();
		}
	}
}