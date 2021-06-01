﻿using com.mirle.ibg3k0.sc.App;
using NLog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Common
{
    public class RedisCacheManager
    {
        public bool IsConnection { get { return GetConnection().IsConnected; } }
        SCApplication scApp = null;
        static string productID = null;
        static string REDIS_KEY_WORK_REDIS_USING_COUNT = "REDIS_USING_COUNT";
        static readonly string KEY_WORD_REDIS_CONTEXT = "redis_context";


        private static Lazy<ConfigurationOptions> configOptions
                    = new Lazy<ConfigurationOptions>(() =>
                    {
                        var configOptions = new ConfigurationOptions();
                        configOptions.EndPoints.Add(SCAppConstants.ConnectionSetting.REDIS_SERVER_CONFIGURATION);
                        configOptions.ClientName = "OHxCRedisConnection";
                        configOptions.ConnectTimeout = 100000;
                        configOptions.SyncTimeout = 100000;
                        configOptions.AbortOnConnectFail = false;
                        return configOptions;
                    });

        private static Lazy<ConnectionMultiplexer> conn
                    = new Lazy<ConnectionMultiplexer>(
                        () => ConnectionMultiplexer.Connect(configOptions.Value));

        Logger logger = LogManager.GetCurrentClassLogger();
        int DBConnectionNum = -1;
        public RedisCacheManager(SCApplication _app, string product_id, int db_connection_num = -1)
        {
            DBConnectionNum = db_connection_num;
            scApp = _app;
            productID = product_id;
            REDIS_KEY_WORK_REDIS_USING_COUNT = $"{productID}_{REDIS_KEY_WORK_REDIS_USING_COUNT}";
            GetConnection().ConnectionFailed += RedisCacheManager_ConnectionFailed;
            GetConnection().ConnectionRestored += RedisCacheManager_ConnectionRestored;
        }

        private void RedisCacheManager_ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            logger.Info("ConnectionRestored");
            ALINE line = scApp.getEQObjCacheManager().getLine();
            if (line != null)
            {
                line.Redis_Link_Stat = SCAppConstants.LinkStatus.LinkOK;
            }
        }

        private void RedisCacheManager_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            logger.Info(e.Exception,
                        "Redis ConnectionFailed.ConnectionType[{0}],FailureType[{1}]",
                        e.ConnectionType,
                        e.FailureType);
            ALINE line = scApp.getEQObjCacheManager().getLine();
            if (line != null)
            {
                line.Redis_Link_Stat = SCAppConstants.LinkStatus.LinkFail;
            }
        }

        object _lock = new object();
        bool redisConnectionValid = true;

        private ConnectionMultiplexer GetConnection()
        {
            return conn.Value;
            //if (RedisConnection != null && RedisConnection.IsConnected) return RedisConnection;

            //lock (_lock)
            //{
            //    if (RedisConnection != null && RedisConnection.IsConnected) return RedisConnection;

            //    if (RedisConnection != null)
            //    {
            //        logger.Warn("Redis connection disconnected. Disposing connection...");
            //        RedisConnection.Dispose();
            //    }

            //    logger.Warn("Creating new instance of Redis Connection");
            //    var options = ConfigurationOptions.Parse(SCAppConstants.ConnectionSetting.REDIS_SERVER_CONFIGURATION);
            //    RedisConnection = ConnectionMultiplexer.Connect(options);
            //}
            //return RedisConnection;
        }

        private IServer GetServer()
        {
            var conn = GetConnection();
            var end_point = conn.GetEndPoints()[0];
            return conn.GetServer(end_point);
        }

        public IDatabase Database()
        {
            try
            {
                int db_index = -1;
                //#if DEBUG
                //                db_index = 1;
                //#endif

                return !redisConnectionValid ? null : GetConnection().GetDatabase(DBConnectionNum);
            }
            catch (Exception ex)
            {
                redisConnectionValid = false;
                logger.Error(String.Format("Unable to create Redis connection: {0}", ex.Message));
                return null;
            }
        }

        public ISubscriber Subscriber()
        {
            try
            {
                return !redisConnectionValid ? null : GetConnection().GetSubscriber();
            }
            catch (Exception ex)
            {
                redisConnectionValid = false;
                logger.Error(String.Format("Unable to create Redis connection: {0}", ex.Message));
                return null;
            }
        }


        public void CloseRedisConnection()
        {
            GetConnection().Close();
        }

        public bool KeyExists(string key)
        {
            IDatabase db = Database();
            if (db == null) return false;
            UsingCount();
            key = $"{productID}_{key}";
            return db.KeyExists(key);
        }
        public bool KeyDelete(string key, bool isNeedAddProduct = true)
        {
            IDatabase db = Database();
            if (db == null) return false;
            UsingCount();
            if (isNeedAddProduct)
                key = $"{productID}_{key}";
            return db.KeyDelete(key);
        }
        public long KeyDelete(RedisKey[] keys)
        {
            IDatabase db = Database();
            if (db == null) return -1;
            UsingCount();
            return db.KeyDelete(keys);
        }


        public bool stringSetAsync(string key, RedisValue set_object, TimeSpan? timeOut = null, When when = When.Always)
        {
            IDatabase db = Database();
            if (db == null) return false;
            key = $"{productID}_{key}";
            db.StringSetAsync(key, set_object, timeOut, when);
            UsingCount();
            return true;
        }
        public bool stringSet(string key, RedisValue set_object, TimeSpan? timeOut = null, When when = When.Always)
        {
            bool is_success = false;
            IDatabase db = Database();
            if (db == null) return false;
            key = $"{productID}_{key}";
            is_success = db.StringSet(key, set_object, timeOut, when);
            UsingCount();
            return is_success;
        }

        public bool StringIncrementAsync(string key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            IDatabase db = Database();
            if (db == null) return false;
            key = $"{productID}_{key}";
            db.StringIncrementAsync(key, value, flags);
            return true;
        }

        public bool Obj2ByteArraySetAsync<T>(string key, T set_object, TimeSpan? timeOut = null)
        {
            IDatabase db = Database();
            if (db == null) return false;
            var serialize_Vo = SCUtility.ToByteArray(set_object);
            key = $"{productID}_{key}";
            db.StringSetAsync(key, serialize_Vo, timeOut);
            UsingCount();
            return true;
        }
        public bool Obj2ByteArraySetAsync(string key, byte[] set_byteArray, TimeSpan? timeOut = null)
        {
            IDatabase db = Database();
            if (db == null) return false;
            key = $"{productID}_{key}";
            db.StringSetAsync(key, set_byteArray, timeOut);
            UsingCount();
            return true;
        }

        public IEnumerable<RedisKey> SearchKey(string pattern)
        {
            IServer server = GetServer();
            if (server == null) return null;
            pattern = $"{productID}_{pattern}";
            var keylist = server.Keys(database: Database().Database, pattern: pattern);
            return keylist;
        }

        public RedisValue StringGet(string key, bool isNeedAddProduct = true)
        {
            IDatabase db = Database();
            if (db == null) return string.Empty;
            UsingCount();
            if (isNeedAddProduct)
                key = $"{productID}_{key}";
            var value = db.StringGet(key);
            if (!value.HasValue)
            {
                // logger.Warn($"redis key not exist:{key}");
                return string.Empty;
            }
            return value;
        }

        public RedisValue[] StringGet(RedisKey[] keys)
        {
            IDatabase db = Database();
            if (db == null) return null;
            UsingCount();
            var value = db.StringGet(keys);
            return value;
        }

        public T StringGet<T>(string key)
        {
            IDatabase db = Database();
            if (db == null) return default(T);
            key = $"{productID}_{key}";
            var value = db.StringGet(key);
            UsingCount();
            if (!value.HasValue)
            {
                // logger.Warn($"redis key not exist:{key}");
                return default(T);
            }
            var deserialization_obj = (T)SCUtility.ToObject(value);
            return deserialization_obj;
        }

        public long ListLength(string list_key)
        {
            IDatabase db = Database();
            if (db == null) return 0;
            UsingCount();
            list_key = $"{productID}_{list_key}";
            return db.ListLength(list_key);
        }

        public void ListRightPush(string list_key, List<AVEHICLE> values)
        {
            IDatabase db = Database();
            if (db == null) return;
            StackExchange.Redis.RedisValue[] redisArray = new StackExchange.Redis.RedisValue[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                redisArray[i] = values[i].ToString();
            }
            list_key = $"{productID}_{list_key}";
            Task taskSand = db.ListRightPushAsync(list_key, redisArray);
            UsingCount();
            //taskSand.Wait();
        }
        public void ListRightPush(string list_key, string value, TimeSpan? timeSpan = null)
        {
            IDatabase db = Database();
            if (db == null) return;
            list_key = $"{productID}_{list_key}";
            Task taskSand = db.ListRightPushAsync(list_key, value);
            if (timeSpan != null)
            {
                db.KeyExpireAsync(list_key, timeSpan);
            }
            UsingCount();
        }
        public void ListRightPush(string list_key, IEnumerable<string> values, TimeSpan? timeSpan = null)
        {
            IDatabase db = Database();
            if (db == null) return;
            StackExchange.Redis.RedisValue[] redisArray = values.Select(v => (RedisValue)v).ToArray();
            list_key = $"{productID}_{list_key}";
            Task taskSand = db.ListRightPushAsync(list_key, redisArray);
            if (timeSpan != null)
            {
                db.KeyExpireAsync(list_key, timeSpan);
            }
            UsingCount();
        }
        public List<string> ListRange(string list_key)
        {
            IDatabase db = Database();
            if (db == null) new List<string>();
            list_key = $"{productID}_{list_key}";
            RedisValue[] redisValues = db.ListRangeAsync(list_key).Result;
            List<string> values = redisValues.Select(vs => (string)vs).ToList();
            UsingCount();
            return values;
        }

        public RedisValue ListLeftPopAsync(string list_key)
        {
            IDatabase db = Database();
            if (db == null) return default(RedisValue);
            list_key = $"{productID}_{list_key}";
            Task<RedisValue> taskSand = db.ListLeftPopAsync(list_key);
            UsingCount();
            return taskSand.Result;
            //taskSand.Wait();
        }

        public void ListSetByIndexAsync(string list_key, string vh_id, string value)
        {
            IDatabase db = Database();
            if (db == null) return;
            int vh_num = 0;
            if (int.TryParse(vh_id.Substring(vh_id.Length - 2), out vh_num))
            {
                list_key = $"{productID}_{list_key}";
                Task taskSand = db.ListSetByIndexAsync(list_key, vh_num - 1, value);
                UsingCount();
                //taskSand.Wait();
            }
        }

        public void UsingCount()
        {
            IDatabase db = Database();
            if (db == null) return;

            db.StringIncrementAsync(REDIS_KEY_WORK_REDIS_USING_COUNT);
        }

        public void StringIncrementAsync(string key)
        {
            IDatabase db = Database();
            if (db == null) return;
            key = $"{productID}_{key}";
            db.StringIncrementAsync(key);
        }
        public void StringDecrementAsync(string key)
        {
            IDatabase db = Database();
            if (db == null) return;
            key = $"{productID}_{key}";
            db.StringDecrementAsync(key);
        }

        public void PublishEvent(string key, byte[] set_byteArray)
        {
            ISubscriber sub = Subscriber();
            if (sub != null)
            {
                key = $"{productID}_{key}";
                sub.Publish(key, set_byteArray);
            }
        }

        public void SubscriptionEvent(string subscription_key, Action<RedisChannel, RedisValue> action)
        {
            ISubscriber sub = Subscriber();
            if (sub != null)
            {
                subscription_key = $"{productID}_{subscription_key}";
                sub.Subscribe(subscription_key, action);
            }
        }
        public void UnsubscribeEvent(string subscription_key, Action<RedisChannel, RedisValue> action)
        {
            ISubscriber sub = Subscriber();
            if (sub != null)
            {
                subscription_key = $"{productID}_{subscription_key}";
                sub.Unsubscribe(subscription_key, action);
            }
        }


        public Task<bool> HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always)
        {
            key = $"{productID}_{key}";
            IDatabaseAsync db = null;
            ITransaction context = CallContext.GetData(KEY_WORD_REDIS_CONTEXT) as ITransaction;
            if (context == null)
            {
                db = Database();
                if (db == null) return Task.FromResult(false);
            }
            else
            {
                db = context;
            }

            UsingCount();
            return db.HashSetAsync(key, hashField, value, when); ;
        }
        
        public Task<bool> HashDeleteAsync(RedisKey key, RedisValue hashField, When when = When.Always)
        {
            key = $"{productID}_{key}";
            IDatabaseAsync db = null;
            ITransaction context = CallContext.GetData(KEY_WORD_REDIS_CONTEXT) as ITransaction;
            if (context == null)
            {
                db = Database();
                if (db == null) return Task.FromResult(false);
            }
            else
            {
                db = context;
            }
            UsingCount();
            return db.HashDeleteAsync(key, hashField);
        }

        public Task<long> HashDeleteAsync(RedisKey key, RedisValue[] hashFields, When when = When.Always)
        {
            key = $"{productID}_{key}";
            IDatabaseAsync db = null;
            ITransaction context = CallContext.GetData(KEY_WORD_REDIS_CONTEXT) as ITransaction;
            if (context == null)
            {
                db = Database();
                if (db == null) return Task.FromResult(long.Parse("0"));
            }
            else
            {
                db = context;
            }
            UsingCount();
            return db.HashDeleteAsync(key, hashFields);
        }

        public bool HashExists(RedisKey key, RedisValue hashField)
        {
            IDatabase db = Database();
            if (db == null) return false;
            key = $"{productID}_{key}";
            db.HashExists(key, hashField);
            UsingCount();
            return true;
        }

        public Task<RedisValue[]> HashValuesAsync(string key)
        {
            IDatabase db = Database();
            if (db == null) return null;
            var value = db.HashValuesAsync(key);
            UsingCount();
            return value;
        }
        public Task<RedisValue[]> HashValuesProductOnlyAsync(string key)
        {
            IDatabase db = Database();
            if (db == null) return null;
            key = $"{productID}_{key}";
            var value = db.HashValuesAsync(key);
            UsingCount();
            return value;
        }
        public RedisValue[] HashKeys(string key)
        {
            IDatabase db = Database();
            if (db == null) return null;
            key = $"{productID}_{key}";
            var value = db.HashKeys(key);
            UsingCount();
            return value;
        }


        public void BeginTransaction()
        {
            IDatabase db = Database();
            if (db == null) return;
            var tran = db.CreateTransaction();
            CallContext.SetData(KEY_WORD_REDIS_CONTEXT, tran);
        }

        public void AddTransactionCondition(Condition condition)
        {
            ITransaction context = CallContext.GetData(KEY_WORD_REDIS_CONTEXT) as ITransaction;
            if (context == null) throw new InvalidOperationException("Redis of transaction not find!");
            context.AddCondition(condition);
        }
        public bool ExecuteTransaction()
        {
            ITransaction context = CallContext.GetData(KEY_WORD_REDIS_CONTEXT) as ITransaction;
            if (context == null) return false;
            return context.Execute();
        }

    }
}
