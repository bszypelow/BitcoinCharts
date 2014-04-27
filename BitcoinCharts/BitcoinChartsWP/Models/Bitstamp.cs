using Newtonsoft.Json;
using PusherClient;
using System;
using System.Linq;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace BitcoinChartsWP.Models
{
	public class Bitstamp : IConnectableObservable<Trade>
	{
		private const string Key = "de504dc5763aeef9ff52";
		private const string TradesChannel = "live_trades";
		private const string TradeEvent = "trade";
		private const string TradingHistory = "https://www.bitstamp.net/api/transactions/";

		private Pusher pusher;
		private HttpClient httpClient;
		private ISubject<Trade> all, published;

		public Bitstamp()
		{
			this.httpClient = new HttpClient();
			this.pusher = new Pusher(Key);
			this.all = new ReplaySubject<Trade>(TimeSpan.FromMinutes(1));
			this.published = new Subject<Trade>();
			Init();
		}

		private async void Init()
		{
			var realtime = await GetRealtimeStream();
			var historical = await GetHistoricalStream();

			historical.Concat(realtime).Subscribe(this.all);
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
								Amount = (double)data.amount,
								Price = (double)data.price,
								Timestamp = DateTimeOffset.UtcNow
							});
					});

				return Disposable.Create(() => channel.Unsubscribe());
			});
		}

		private async Task<IObservable<Trade>> GetHistoricalStream()
		{
			string json = await this.httpClient.GetStringAsync(TradingHistory);
			var template = new[] { new { tid = 0, date = 0L, price = 0.0, amount = 0.0 } };
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
			return this.published.Subscribe(observer);
		}

		public IDisposable Connect()
		{
			return this.all.SubscribeOn(ThreadPoolScheduler.Instance).Subscribe(published);
		}
	}
}
