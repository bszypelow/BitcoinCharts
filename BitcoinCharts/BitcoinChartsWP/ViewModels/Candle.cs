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
		private ObservableAsPropertyHelper<decimal> hi;
		private ObservableAsPropertyHelper<decimal> lo;
		private ObservableAsPropertyHelper<decimal> open;
		private ObservableAsPropertyHelper<decimal> close;

		public decimal Hi { get { return hi.Value; } }
		public decimal Lo { get { return lo.Value; } }
		public decimal Open { get { return open.Value; } }
		public decimal Close { get { return close.Value; } }

		public Candle(IObservable<decimal> hi, IObservable<decimal> lo, IObservable<decimal> open, IObservable<decimal> close)
		{
			this.hi = new ObservableAsPropertyHelper<decimal>(hi, v =>
			{
				raisePropertyChanged("Hi");
			});
			this.lo = new ObservableAsPropertyHelper<decimal>(lo, v => raisePropertyChanged("Lo"));
			this.open = new ObservableAsPropertyHelper<decimal>(open, v => raisePropertyChanged("Open"));
			this.close = new ObservableAsPropertyHelper<decimal>(close, v => raisePropertyChanged("Close"));
		}
	}
}
