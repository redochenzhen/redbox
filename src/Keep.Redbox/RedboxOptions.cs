using Keep.Framework;

namespace Keep.Redbox
{
    public class RedboxOptions : OptionsBase
    {
        public int RetryInterval { get; set; } = 30;

        public int Retries { get; set; } = 30;

        public string Version { get; set; } = "v1.0";

        public int BatchCount = 50;

        public int MaxFailedRetries = 30;
    }
}

