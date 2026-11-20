using AutoMapper;
using CruxIT.Library.API.Controller;
using CruxIT.Library.DataToolkit.RepositoryPattern;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CruxIT.Library.API.Tests.Fixtures;

internal sealed class TestController : CxControllerBase<TestDbContext, UnitOfWork<TestDbContext>>
{
    public TestController(
        IConfiguration configuration,
        ILogger<CxControllerBase<TestDbContext, UnitOfWork<TestDbContext>>> logger,
        UnitOfWork<TestDbContext> unitOfWork,
        IMapper mapper)
        : base(configuration, logger, unitOfWork, mapper) { }
}
