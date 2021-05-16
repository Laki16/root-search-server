# RootSearch Server
RootSearch server is a server for RootSearch search engine.


# Requirements

## .NET 5.0 / .NET Runtime 5.0

RootSearch server is in development on [.NET 5.0][dotnetcoresdk]

## Redis 6.0

RootSearch depends on [Redis][redis].

[dotnetcoresdk]:https://dotnet.microsoft.com/download
[redis]:https://redis.io/download

## Docker (optional)

RootSearch can also running on [Docker][docker].

[docker]:https://docs.docker.com/get-docker/

# Configuration

`appsettings.json` example.

## Environment configuration

```json
{
  "Kestrel": {
    "Endpoints": {
      "HttpsDefaultCert": {
        "Url": "Url for api server (e.g. https://0.0.0.0:5001)"
      }
    },
    "Certificates": {
      "Default": {
        "Path": "SSL key path (e.g. my-key.pfx)",
        "Password": "SSL key password"
      }
    }
  },
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

#### Docker
```bash
docker build -t {image name} .

docker run --name {container name} -d -p {hostPort:innerPort} {image name}

TODO: docker compose
```

#### .NET standalone (without docker)
```bash
# You can skip this step if you already migrated to .NET Core 3.1 to .NET 5.0
dotnet nuget locals --clear all

dotnet restore

dotnet run apiserver
```
