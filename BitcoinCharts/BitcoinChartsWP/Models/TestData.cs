using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinChartsWP.Models
{
	public class TestData
	{
		public static IConnectableObservable<Trade> Create()
		{
			return Observable.Generate(
					new Random(),
					r => true,
					r => r,
					r => r.NextDouble() - 0.49,
					r => TimeSpan.FromMilliseconds(20))
				.Scan((acc,curr) => acc+curr)
				.Select(v => new Trade
				{
					Amount = 1,
					Price = (double)v,
					Timestamp = DateTime.Now
				})
				.Publish();
		}
	}
}
