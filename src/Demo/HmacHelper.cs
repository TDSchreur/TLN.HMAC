using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Serilog;

namespace Demo;

public class HmacHelper
{
    private const int Noncesize = 16;
    private const string Secret = "$f*Q#8aEWAa%WmnSBh%r#6Dfv6GJteQn";
    private const string Textencodig = "UTF-8";

    public static string GenerateQueryString(Guid orderId)
    {
        string nonce = NewNonce();
        string timestamp = CurrentDateStamp();
        string data = $"{orderId}{nonce}{timestamp}";

        Log.Logger.Information("Generate URL");
        Log.Logger.Information("Nonce     : {Nonce}", nonce);
        Log.Logger.Information("Timestamp : {Timestamp}", timestamp);

        string hmac = GenerateHash(data);
        Log.Logger.Information("HMAC      : {Hmac}", hmac);

        return $"nonce={HttpUtility.UrlEncode(nonce)}&timestamp={timestamp}&hmac={HttpUtility.UrlEncode(hmac)}";
    }

    public bool IsValidHash(string url)
    {
        Uri uri = new(url, UriKind.Absolute);

        NameValueCollection param = HttpUtility.ParseQueryString(uri.Query);
        string nonce = param.Get("nonce") ?? string.Empty;
        string timestamp = param.Get("timestamp") ?? string.Empty;
        string hmac = param.Get("hmac") ?? string.Empty;
        string data = $"{uri.Segments.Last()}{nonce}{timestamp}";
        string hash = GenerateHash(data);

        Log.Logger.Information("Validate URL");
        Log.Logger.Information("Nonce     : {Nonce}", nonce);
        Log.Logger.Information("Timestamp : {Timestamp}", timestamp);
        Log.Logger.Information("HMAC      : {Hmac}", hmac);
        Log.Logger.Information("New HASH  : {Hash}", hash);

        return hmac.Equals(hash);
    }

    private static byte[] ComputeHash(byte[] data, byte[] key)
    {
        HMAC mac = new HMACSHA256 { Key = key };
        mac.Initialize();

        return mac.ComputeHash(data);
    }

    private static string CurrentDateStamp()
    {
        return ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
    }

    private static string GenerateHash(string rawData)
    {
        byte[] key = Encoding.GetEncoding(Textencodig).GetBytes(Secret);
        byte[] data = Encoding.GetEncoding(Textencodig).GetBytes(rawData);
        byte[] hash = ComputeHash(data, key);

        return Convert.ToBase64String(hash);
    }

    private static string NewNonce()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(Noncesize));
    }
}
