using System;

namespace BitcoinChartsWP.Models
{
	public class Trade
	{
		public int Id { get; set; }
		public double Amount { get; set; }
		public double Price { get; set; }
		public DateTimeOffset Timestamp {get;set;}
	}
}
