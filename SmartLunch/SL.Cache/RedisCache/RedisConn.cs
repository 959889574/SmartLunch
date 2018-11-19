using System.Collections.Concurrent;
using System.Configuration;
using NLog;
using StackExchange.Redis;

namespace EP.APP.Cache.RedisCache
{
    /// <summary>The redis connection.</summary>
    /// <remarks>ygz, 2017/03/16.</remarks>
    public class RedisConn
    {
        //"127.0.0.1:6379,allowadmin=true
        private static ConnectionMultiplexer _redis;

        public static readonly string SysCustomKey = ConfigurationManager.AppSettings["RedisKey"] ?? "";
        private static readonly string RedisConnectionString = ConfigurationManager.AppSettings["RedisExchangeHosts"];
        private static readonly object _lock = new object();

        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> ConnectionCache =
            new ConcurrentDictionary<string, ConnectionMultiplexer>();

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 获取实例
        /// </summary>
        public static ConnectionMultiplexer Redis
        {
            get
            {
                if (_redis == null)
                {
                    lock (_lock)
                    {
                        if (_redis == null || !_redis.IsConnected)
                        {
                            _redis = GetConn();
                        }
                    }
                }
                return _redis;
            }
        }

        /// <summary>
        /// 缓存获取
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static ConnectionMultiplexer GetConnectionMultiplexer(string connStr)
        {
            if (!ConnectionCache.ContainsKey(connStr))
            {
                ConnectionCache[connStr] = GetConn(connStr);
            }
            return ConnectionCache[connStr];
        }

        private static ConnectionMultiplexer GetConn(string connStr = null)
        {
            connStr = connStr ?? RedisConnectionString;
//           如果有密码则：
//           connStr = "Redis服务器IP地址:端口号,password=Redis密码";
            ConnectionMultiplexer connect = ConnectionMultiplexer.Connect(connStr);

            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnRestored;
            connect.ErrorMessage += MuxerErrorMsg;
            connect.ConfigurationChanged += MuxerConfigChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            connect.InternalError += MuxerInternalError;

            return connect;
        }

        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigChanged(object sender, EndPointEventArgs e)
        {
            Log.Info("Configuration changed: " + e.EndPoint);
        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMsg(object sender, RedisErrorEventArgs e)
        {
            Log.Error("ErrorMessage: " + e.Message);
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnRestored(object sender, ConnectionFailedEventArgs e)
        {
            Log.Error("ConnectionRestored: " + e.EndPoint);
        }

        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Log.Warn("重新连接：Endpoint failed: " + e.EndPoint + ", " + e.FailureType +
                     (e.Exception == null ? "" : (", " + e.Exception.Message)));
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Log.Info("HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            Log.Error("InternalError:Message" + e.Exception.Message);
        }
    }
}