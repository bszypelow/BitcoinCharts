using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinChartsWP.ViewModels
{
	public class Candle : ReactiveObject
	{
		private ObservableAsPropertyHelper<double> hi;
		private ObservableAsPropertyHelper<double> lo;
		private ObservableAsPropertyHelper<double> open;
		private ObservableAsPropertyHelper<double> close;

		public DateTime Time { get; set; }
		public double Hi { get { return hi.Value; } set { } } // Empty setter is a workaround for bug in SparrowChart.
		public double Lo { get { return lo.Value; } set { } }
		public double Open { get { return open.Value; } set { } }
		public double Close { get { return close.Value; } set { } }
		

		public Candle(DateTime time, IObservable<double> hi, IObservable<double> lo, IObservable<double> open, IObservable<double> close)
		{
			this.Time = time;
			this.hi = hi.ToProperty(this, c => c.Hi);
			this.lo = lo.ToProperty(this, c => c.Lo);
			this.open = open.ToProperty(this, c => c.Open);
			this.close = close.ToProperty(this, c => c.Close);
		}
	}
}
