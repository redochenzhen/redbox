using System;
using System.Collections.Generic;
using System.Text;

namespace Keep.Redbox.Cache
{
    public interface ICacheClientFactory
    {
        ICacheClient CreateClient();
    }
}
