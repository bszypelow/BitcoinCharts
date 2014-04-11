using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinChartsWP.Models
{
	public class Bitstamp : IObservable<decimal>
	{
		private Subject<decimal> subject = new Subject<decimal>();

		public IDisposable Subscribe(IObserver<decimal> observer)
		{
			return this.subject.Subscribe(observer);
		}
	}
}
