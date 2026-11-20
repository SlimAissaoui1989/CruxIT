using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;

namespace CruxIT.Library.DataToolkit.Tests.TestData.Entities;

internal sealed class TestDateTimeRecord : EntityIdentityIntKeyBase
{
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
}
