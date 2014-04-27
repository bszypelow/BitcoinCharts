using BitcoinChartsWP.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ReactiveUI;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Media;

namespace BitcoinChartsWP.ViewModels
{
	public class MainViewModel : ReactiveObject
	{
		private static readonly Brush GreenBrush = new SolidColorBrush(Colors.Green);
		private static readonly Brush RedBrush = new SolidColorBrush(Colors.Red);

		private ObservableAsPropertyHelper<double> lastPrice;
		public double LastPrice { get { return this.lastPrice.Value; } }

		private ObservableAsPropertyHelper<Brush> trend;
		public Brush Trend { get { return this.trend.Value; } }

		public PlotModel Chart { get; set; }

		public IReactiveDerivedList<Candle> Candles { get; set; }

		public MainViewModel(IConnectableObservable<Trade> source)
		{
			var prices = from trade in source
						 select trade.Price;

			this.lastPrice = prices
				.Sample(TimeSpan.FromSeconds(0.2))
				.ToProperty(this, t => t.LastPrice);

			this.trend = prices
				.DistinctUntilChanged()
				.Buffer(2, 1)
				.Sample(TimeSpan.FromSeconds(0.2))
				.Select(b => b[0] < b[1] ? GreenBrush : RedBrush)
				.ToProperty(this, t => t.Trend);

			var scheduler = new HistoricalScheduler(DateTimeOffset.UtcNow.AddMinutes(-61));
			source.Subscribe(t => scheduler.AdvanceTo(t.Timestamp));

			this.Candles = prices
				.Window(TimeSpan.FromMinutes(5), scheduler)
				.Select(w => new Candle(
					lo: w.Scan(double.MaxValue, (min, current) => min > current ? current : min),
					hi: w.Scan(double.MinValue, (max, current) => max < current ? current : max),
					open: w.Take(1),
					close: w,
					time: new DateTime(scheduler.Clock.Ticks)))
				.CreateCollection();

			this.Chart = new PlotModel { PlotAreaBackground = OxyColors.Black, PlotAreaBorderThickness = 0 };
			this.Chart.Axes.Add(new DateTimeAxis { StringFormat = "hh:mm", AxislineStyle = LineStyle.Solid, AxislineColor = OxyColors.White, AxislineThickness = 1, TextColor = OxyColors.White, TicklineColor = OxyColors.White });
			this.Chart.Axes.Add(new LinearAxis { AxislineStyle = LineStyle.Solid, AxislineColor = OxyColors.White, AxislineThickness = 1, TextColor = OxyColors.White, TicklineColor = OxyColors.White, MajorGridlineColor = OxyColors.Gray, MajorGridlineStyle = LineStyle.Dash });
			this.Chart.Series.Add(new CandleStickSeries
			{
				ItemsSource = this.Candles,
				DataFieldX = "Time",
				DataFieldHigh = "Hi",
				DataFieldLow = "Lo",
				DataFieldOpen = "Open",
				DataFieldClose = "Close",

				CandleWidth = 12, StrokeThickness = 2, IncreasingFill = OxyColors.DarkGreen, DecreasingFill = OxyColors.Red, Color = OxyColors.White
			});

			this.Candles.ChangeTrackingEnabled = true;
			this.Candles.ItemChanged
				.Sample(TimeSpan.FromSeconds(0.2))
				.Subscribe(p => this.Chart.InvalidatePlot(updateData: true));

			source.Connect();
		}
	}
}