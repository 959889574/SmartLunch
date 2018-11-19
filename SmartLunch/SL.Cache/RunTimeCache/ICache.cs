using System;

namespace EP.APP.Cache
{
    public interface ICache
    {
        bool Set(string key, object value);
        bool Set(string key, object value, DateTime expiresAt);
        bool Set(string key, object value, TimeSpan validFor);
        bool SetWithCheckVersion(string key, object value, DateTime expiresAt, ulong version);
        bool SetWithCheckVersion(string key, object value, TimeSpan validFor, ulong version);
        bool Clear();

        T Get<T>(string key);

        bool Remove(string key);
    }
}
