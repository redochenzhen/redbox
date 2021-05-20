namespace Keep.Redbox.Redis
{
    public class RedisOptions
    {
        public const string CONFIG_PREFIX = "redbox:redis";

        public const string CONNECTION_NAME = "redisconnection";

        public string ConnectionString { get; set; }

        public bool DisableSubscribe { get; set; }
    }
}
