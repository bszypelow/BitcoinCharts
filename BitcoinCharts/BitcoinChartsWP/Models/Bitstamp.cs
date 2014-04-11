using Newtonsoft.Json;
using PusherClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
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
		private const string TradingHistory = "https://www.bitstamp.net/api/transactions/";

		private Subject<Trade> allTrades = new Subject<Trade>();
		private Subject<Trade> newTrades = new Subject<Trade>();
		private Pusher pusher;
		private HttpClient httpClient;

		public Bitstamp()
		{
			this.httpClient = new HttpClient();
			this.LoadHistory();

			this.pusher = new Pusher(Key);
			this.ObserveRealtimeTrades();
		}

		private async void ObserveRealtimeTrades()
		{
			await this.pusher.ConnectAsync();
			var channel = await this.pusher.Subscribe(TradesChannel);
			channel.Bind(TradeEvent, data =>
			{
				this.newTrades.OnNext(new Trade
				{
					Amount = (decimal)data.amount,
					Price = (decimal)data.price
				});
			});

		}

		private async void LoadHistory()
		{
			string json = await this.httpClient.GetStringAsync(TradingHistory);
			var template = new[]{ new { tid = 0, date = 0L, price = 0m, amount = 0m}};
			var previousTrades = JsonConvert.DeserializeAnonymousType(json, template);
			previousTrades.Reverse();

			previousTrades
				.ToObservable()
				.Select(t => new Trade { Id = t.tid, Amount = t.amount, Price = t.price})
				.Concat(newTrades)
				.Subscribe(this.allTrades);
		}

		public IDisposable Subscribe(IObserver<Trade> observer)
		{
			return this.allTrades.Subscribe(observer);
		}
	}
}
