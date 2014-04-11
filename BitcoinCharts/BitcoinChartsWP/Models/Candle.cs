using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinChartsWP.Models
{
	public class Candle
	{
		public decimal Hi { get; set; }
		public decimal Lo { get; set; }
		public decimal Open { get; set; }
		public decimal Close { get; set; }
	}
}
