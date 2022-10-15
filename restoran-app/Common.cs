using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace restoran_app
{
    public static class Common
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static DateTime DateTimeFromUnix(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        public static DateTime DateTimeFromUnixMillis(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        public static BigInteger StringToBigInteger(string v)
        {
            var bytes = Encoding.UTF8.GetBytes(v);
            var hex = Convert.ToHexString(bytes);
            return BigInteger.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }
        public static string BigIntegerToString(BigInteger v)
        {
            var bytes = v.ToByteArray();
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
