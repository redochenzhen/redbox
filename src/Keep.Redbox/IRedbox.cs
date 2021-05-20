using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Redbox
{
    public interface IRedbox
    {
        IServiceProvider ServiceProvider { get; }

        AsyncLocal<IRedboxTransaction> Transaction { get; }

        void Remove(params string[] keys);

        T Get<T>(string key, Func<T> valueFunc);

        Task<T> GetAsync<T>(string key, Func<Task<T>> valueFuncAsync);
    }
}

