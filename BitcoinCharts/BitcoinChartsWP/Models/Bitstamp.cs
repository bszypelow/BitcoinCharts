using Newtonsoft.Json;
using PusherClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Disposables;
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

		private Pusher pusher;
		private HttpClient httpClient;
		private Subject<Trade> allTrades = new Subject<Trade>();

		public Bitstamp()
		{
			this.httpClient = new HttpClient();
			this.pusher = new Pusher(Key);
			Init();
		}

		private async void Init()
		{
			var newTrades = await GetRealtimeStream();
			var historicalTrades = await GetHistoricalStream();

			historicalTrades.Concat(newTrades).Subscribe(this.allTrades);
		}

		private async Task<IObservable<Trade>> GetRealtimeStream()
		{
			await this.pusher.ConnectAsync();

			return Observable.Create<Trade>(async observer =>
			{
				var channel = await this.pusher.Subscribe(TradesChannel);
				channel.Bind(TradeEvent, data =>
					{
						observer.OnNext(new Trade
							{
								Amount = (decimal)data.amount,
								Price = (decimal)data.price,
								Timestamp = DateTimeOffset.UtcNow
							});
					});

				return Disposable.Create(() => channel.Unsubscribe());
			});
		}

		private async Task<IObservable<Trade>> GetHistoricalStream()
		{
			string json = await this.httpClient.GetStringAsync(TradingHistory);
			var template = new[] { new { tid = 0, date = 0L, price = 0m, amount = 0m } };
			var previousTrades = JsonConvert.DeserializeAnonymousType(json, template);

			return previousTrades
				.Reverse()
				.ToObservable()
				.Select(t => new Trade
				{
					Id = t.tid,
					Amount = t.amount,
					Price = t.price,
					Timestamp = t.date.AsDateTimeOffset()
				});
		}

		public IDisposable Subscribe(IObserver<Trade> observer)
		{
			return this.allTrades.Subscribe(observer);
		}
	}
}
