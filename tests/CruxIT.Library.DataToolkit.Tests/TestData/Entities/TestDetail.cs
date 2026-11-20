using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;

namespace CruxIT.Library.DataToolkit.Tests.TestData.Entities;

internal sealed class TestDetail : EntityIdentityIntKeyBase
{
    public string Description { get; set; } = string.Empty;
    public TestMetadata? Metadata { get; set; }
}
