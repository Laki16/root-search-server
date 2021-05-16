# RootSearch Server
RootSearch server is a server for RootSearch search engine.


# Requirements

## .NET 5.0 / .NET Runtime 5.0

RootSearch server is in development on [.NET 5.0][dotnetcoresdk]


## Redis 6.0

RootSearch depends on [Redis][redis].

[dotnetcoresdk]:https://dotnet.microsoft.com/download
[redis]:https://redis.io/download


# Configuration

`appsettings.json` example.

## Environment configuration

```json
{
  "RedisSettings": {
    "Host": "Host for redis server",
    "Port": "Port number for redis server"
  },
  "SearchEngineApiSettings": {
    "GoogleCustomSearchApiSettings": {
      "ApiKey": "Google custom search api key",
      "Cx": "Google Custom search engine ID"
    }
  }
}
```


# Run

```bash
# You can skip this step if you already migrated to .NET Core 3.1 to .NET 5.0
dotnet nuget locals --clear all

dotnet restore

dotnet run apiserver
```
