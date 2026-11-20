using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;

namespace CruxIT.Library.DataToolkit.Tests.TestData.Entities;

internal sealed class TestChild : EntityIdentityIntKeyBase
{
    public int TestRecordId { get; set; }
    public TestRecord? TestRecord { get; set; }
    public int? TestDetailId { get; set; }
    public TestDetail? Detail { get; set; }
}
