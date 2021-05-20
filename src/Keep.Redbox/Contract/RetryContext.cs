using System;
using System.Collections.Generic;
using System.Text;

namespace Keep.Redbox.Contract
{
    public class RetryContext
    {
        public long KeyId { get; set; }

        public string Key { get; set; }

        public int Retries { get; set; }
    }
}
