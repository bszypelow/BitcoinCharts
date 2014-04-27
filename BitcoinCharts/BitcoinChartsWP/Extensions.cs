using System;

namespace BitcoinChartsWP
{
	public static class Extensions
	{
		private static readonly DateTimeOffset UnixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

		public static DateTimeOffset AsDateTimeOffset(this long timestamp)
		{
			return UnixEpoch.AddSeconds(timestamp);
		}
	}
}
