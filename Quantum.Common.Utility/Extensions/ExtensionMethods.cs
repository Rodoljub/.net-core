using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Quantum.Utility.Extensions
{
    public static class ExtensionMethods
    {
        public static int GetAsInteger(this IConfiguration _config, string key, int defaultValue = 0)
        {
            int i = int.TryParse(_config[key], out i) ? i : defaultValue;

            return i;
        }

        public static double GetAsDouble(this IConfiguration _config, string key, double defaultValue = 0)
        {
            double i = double.TryParse(_config[key], out i) ? i : defaultValue;

            return i;
        }

		public static decimal GetAsDecimal(this IConfiguration _config, string key, decimal defaultValue = 0)
		{
			decimal i = decimal.TryParse(_config[key], out i) ? i : defaultValue;

			return i;
		}

		public static long GetAsLong(this IConfiguration _config, string key, long defaultValue = 0)
		{
			long i = long.TryParse(_config[key], out i) ? i : defaultValue;

			return i;
		}

		//public static Stream ToStream(this Image image)
		//{
		//	var stream = new System.IO.MemoryStream();
		//	image.Save(stream, image.RawFormat);
		//	stream.Position = 0;
		//	return stream;
		//}

		//public static Stream ToStreamFromBitmap(this Image image, ImageFormat imageFormat)
		//{
		//	var stream = new System.IO.MemoryStream();
		//	image.Save(stream, imageFormat);
		//	stream.Position = 0;
		//	return stream;
		//}
	}
}
