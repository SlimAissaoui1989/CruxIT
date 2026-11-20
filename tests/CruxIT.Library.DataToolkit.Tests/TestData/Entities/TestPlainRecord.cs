using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;

namespace CruxIT.Library.DataToolkit.Tests.TestData.Entities;

internal sealed class TestPlainRecord : EntityIdentityIntKeyBase
{
    public string Name { get; set; } = string.Empty;
}
