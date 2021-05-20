<p align="center">
  <img height="128" src="https://github.com/redochenzhen/redbox/blob/master/images/icon.png">
</p>

# **Keep.Redbox**

## 简介
An eventually-consistancy distributed cache solution (mailbox pattern) for asp.net core.

## 解决什么问题？
当我们使用分布式缓存时，一般会遵循所谓Cache-Aside Pattern:
* 更新：应用程序先从cache取数据，没有得到，则从数据库中取数据，成功后，放到缓存中
* 命中：应用程序从cache中取数据，取到后返回
* 失效：先把数据存到数据库中，成功后，再让缓存失效

遵循这个Pattern，就已经能避免大部分的“脏数据”发生了。

那么“小部分”的不一致问题又是怎么产生的呢？
1. 发生了如下所述场景： <br/>
    1） 缓存刚好失效 <br/>
    2） 请求A查询数据库，得一个旧值 <br/>
    3） 请求B将新值写入数据库 <br/>
    4） 请求B删除缓存 <br/>
    5） 请求A将查到的旧值写入缓存
2. 当数据库写入成功之后，缓存失效之前，应用程序crash了，或者机器重启了

Redbox针对上述两种场景给出了解决方案，可以“大概率”缓解情况1，保证情况2最终一致。

## 快速开始
### NuGet
```
dotnet add package Keep.Redbox
dotnet add package Keep.Redbox.Redis
dotnet add package Keep.Redbox.SqlServer
```

### 配置
在Startup.cs中配置Discovery：
```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddDiscovery(options =>
    {   
        x.UseRedis();
        x.UseSqlServer();
    });
    
    ... ...
}
```
并提供正确的配置：
```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Data Source=localhost;User Id=sa;Password=****;Initial Catalog=redbox;",
        //"RedisConnection": "192.168.117.43:6379"
    },

    ... ...

    "Redbox": {
        "Version": "v1.0",
        "RetryInterval": 30,
        "Retries": 30,
        "Redis": {
            "ConnectionString": "127.0.0.1:6379"
        },
        "SqlServer": {
            //"ConnectionString": "",  //默认使用ConnectionStrings:DefaultConnection
            "Schema": "redbox"
        }
    }
}
```

### 针对场景1
```cs
var result = await _redbox.GetAsync(key,
    async () =>
    {
        using (var conn = new SqlConnection(_connString))
        {
            await Task.Delay(5000);
            var result = await conn.ExecuteScalarAsync<double?>(
                "select avg(Temperature) from Record where City = @City",
                new { City = city });
            return result;
        }
    });
```

### 针对场景2
```cs
using (var conn = new SqlConnection(_connString))
{
    using (var tx = conn.BeginTransaction(_redbox, true))
    {
        await conn.ExecuteAsync(
            "insert into Record values(@Id, @City, @Temperature)",
            record,
            tx);
        _redbox.Remove(key1, key2);
    }
    return Created($"records/{record.Id}", record);
}
```