using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;

namespace CruxIT.Library.API.Tests.Fixtures;

internal static class TestMapperFactory
{
    public static IMapper Create() => new MapperConfiguration(_ => { }, NullLoggerFactory.Instance).CreateMapper();
}
