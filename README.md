# RootSearch Server
RootSearch server is a server for RootSearch search engine.


# Requirements

## .NET Core SDK 3.1 / Runtime 3.1

RootSearch server is in development on [.NET Core 3.1][dotnetcoresdk]

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
dotnet restore

dotnet run apiserver
```
