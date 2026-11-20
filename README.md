# CruxIT

[![CI](https://github.com/SlimAissaoui1989/CruxIT/actions/workflows/ci.yml/badge.svg)](https://github.com/SlimAissaoui1989/CruxIT/actions/workflows/ci.yml)
[![Publish NuGet packages](https://github.com/SlimAissaoui1989/CruxIT/actions/workflows/publish-packages.yml/badge.svg)](https://github.com/SlimAissaoui1989/CruxIT/actions/workflows/publish-packages.yml)
[![Latest release](https://img.shields.io/github/v/release/SlimAissaoui1989/CruxIT?display_name=tag)](https://github.com/SlimAissaoui1989/CruxIT/releases)

CruxIT is a set of reusable .NET libraries for common application helpers, Entity Framework Core data access, and ASP.NET Core API setup.

## Requirements

- .NET 10 SDK to build from source.
- .NET 10 runtime for applications using these packages.

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

### ASP.NET Core API

The API package adds these builder extensions:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddCorpsDefaultPolicyAllowAny();
builder.IgnoreCyclesJsonAndSavePropertyNamingPolicy();
builder.AddJWTAuthentication();
builder.AddSwaggerGenWithBearer();
```

`AddJWTAuthentication()` reads `Jwt:Issuer` and `Jwt:Key` from application configuration. Configure a strong key through a secret store or environment-specific configuration; never commit production credentials.

### EF Core data toolkit

Create a context derived from `CxDbContext` and configure it with the EF Core provider used by your application:

```csharp
using CruxIT.Library.DataToolkit.EntityFramework;
using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : CxDbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDatabase")));
```

The data toolkit also includes entity base types, soft-delete support, repository abstractions, and date/time converters. Choose the EF Core provider package that matches your database.

### Core helpers

The base package contains independent helpers for strings, objects, MIME types, phone numbers, logging, JSON conversion, and asynchronous events. For example:

```csharp
using CruxIT.Library;

bool looksLikeEmail = CxStringHelper.IsEmail("person@example.com");
string title = "hello world".ToTitleCase();
```

Install only the package(s) your application needs.

## Build from source

Install the .NET 10 SDK, then run:

```sh
dotnet restore CruxIT.sln
dotnet build CruxIT.sln --configuration Release --no-restore
dotnet test CruxIT.sln --configuration Release --no-build --no-restore
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

This project is licensed under the [MIT License](LICENSE).
