using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;

namespace CruxIT.Library.DataToolkit.Tests.TestData.Entities;

internal sealed class TestMetadata : EntityIdentityIntKeyBase
{
    public string Value { get; set; } = string.Empty;
}
