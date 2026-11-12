# CruxIT

[![Publish NuGet packages](https://github.com/SlimAissaoui1989/CruxIT/actions/workflows/publish-packages.yml/badge.svg)](https://github.com/SlimAissaoui1989/CruxIT/actions/workflows/publish-packages.yml)
[![Latest release](https://img.shields.io/github/v/release/SlimAissaoui1989/CruxIT?display_name=tag)](https://github.com/SlimAissaoui1989/CruxIT/releases)

CruxIT is a set of reusable .NET libraries for common application helpers, Entity Framework Core data access, and ASP.NET Core API setup.

## Packages

| Package | What it provides |
| --- | --- |
| `CruxIT.Library` | String and object helpers, exception types, logging utilities, JSON converters, phone helpers, and async events. |
| `CruxIT.Library.DataToolkit` | EF Core base entities, soft-delete contracts, `CxDbContext`, repository abstractions, unit of work, and date/time converters. |
| `CruxIT.Library.API` | ASP.NET Core helpers for JWT authentication, CORS, JSON options, Swagger bearer configuration, request and exception logging, and API utilities. |

All packages target **.NET 10**. The data toolkit depends on `CruxIT.Library`; the API package depends on the data toolkit.

## Install

CruxIT packages are published to GitHub Packages. Add the package feed to your NuGet configuration:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="github" value="https://nuget.pkg.github.com/SlimAissaoui1989/index.json" />
  </packageSources>
</configuration>
```

Authenticate to GitHub Packages with a token that has `read:packages` access, then install the package(s) you need. Replace `<VERSION>` with a published release version.

```sh
dotnet add package CruxIT.Library --version <VERSION> --source github
dotnet add package CruxIT.Library.DataToolkit --version <VERSION> --source github
dotnet add package CruxIT.Library.API --version <VERSION> --source github
```

For automated builds, store the token as a secret and configure NuGet credentials in the consuming workflow. Do not commit tokens or credentials to the repository.

## Quick start

Use helpers from the packages you installed. For example, the API package adds these builder extensions:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddCorpsDefaultPolicyAllowAny();
builder.IgnoreCyclesJsonAndSavePropertyNamingPolicy();
builder.AddJWTAuthentication();
builder.AddSwaggerGenWithBearer();
```

`AddJWTAuthentication()` reads `Jwt:Issuer` and `Jwt:Key` from application configuration. Review the project's source and configure these values securely before enabling authentication.

## Build from source

Install the .NET 10 SDK, then run:

```sh
dotnet restore CruxIT.sln
dotnet build CruxIT.sln --configuration Release --no-restore
```

## Releases

Pushing a tag beginning with `v` starts the package publishing workflow. The tag without the leading `v` is used as the NuGet package version; for example, `v1.2.3` publishes version `1.2.3`.

```sh
git tag v1.2.3
git push origin v1.2.3
```

## Contributing

Bug reports, suggestions, and pull requests are welcome. Please read [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request, and review [SECURITY.md](SECURITY.md) for reporting vulnerabilities.

## License

No license has been specified in this repository yet. Until one is added, do not assume the source is available for reuse under an open-source license.
