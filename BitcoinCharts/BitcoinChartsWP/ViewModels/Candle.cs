using ReactiveUI;
using System;

namespace BitcoinChartsWP.ViewModels
{
	public class Candle : ReactiveObject
	{
		private ObservableAsPropertyHelper<double> hi;
		private ObservableAsPropertyHelper<double> lo;
		private ObservableAsPropertyHelper<double> open;
		private ObservableAsPropertyHelper<double> close;

		public DateTime Time { get; set; }
		public double Hi { get { return hi.Value; } }
		public double Lo { get { return lo.Value; } }
		public double Open { get { return open.Value; } }
		public double Close { get { return close.Value; } }
		

		public Candle(DateTime time, IObservable<double> hi, IObservable<double> lo, IObservable<double> open, IObservable<double> close)
		{
			this.Time = time;
			this.hi = hi.ToProperty(this, c => c.Hi, double.NaN);
			this.lo = lo.ToProperty(this, c => c.Lo, double.NaN);
			this.open = open.ToProperty(this, c => c.Open, double.NaN);
			this.close = close.ToProperty(this, c => c.Close, double.NaN);
		}
	}
}
