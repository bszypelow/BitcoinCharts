using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
