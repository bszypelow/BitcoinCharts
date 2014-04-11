using PusherClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinChartsWP.Models
{
	public class Bitstamp : IObservable<Trade>
	{
		private const string Key = "de504dc5763aeef9ff52";
		private const string TradesChannel = "live_trades";
		private const string TradeEvent = "trade";

		private Subject<Trade> subject = new Subject<Trade>();
		private Pusher pusher;

		public Bitstamp()
		{
			this.pusher = new Pusher(Key);
			this.pusher.Connected += pusher_Connected;
			this.pusher.Connect();
		}

		private async void pusher_Connected(object sender)
		{
			var channel = await this.pusher.Subscribe(TradesChannel);
			channel.Bind(TradeEvent, data =>
			{
				this.subject.OnNext(new Trade
				{
					Amount = (decimal)data.amount,
					Price = (decimal)data.price
				});
			});
		}

		public IDisposable Subscribe(IObserver<Trade> observer)
		{
			return this.subject.Subscribe(observer);
		}
	}
}
