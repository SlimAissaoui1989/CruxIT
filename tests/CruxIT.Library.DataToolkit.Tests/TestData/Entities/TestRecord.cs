using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;

namespace CruxIT.Library.DataToolkit.Tests.TestData.Entities;

internal sealed class TestRecord : SoftDeletedEntityIdentityIntKey
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int IntegerAmount { get; set; }
    public int? NullableIntegerAmount { get; set; }
    public long LongAmount { get; set; }
    public long? NullableLongAmount { get; set; }
    public float FloatAmount { get; set; }
    public float? NullableFloatAmount { get; set; }
    public double DoubleAmount { get; set; }
    public double? NullableDoubleAmount { get; set; }
    public decimal? NullableAmount { get; set; }
    public DateOnly LocalDate { get; set; }
    public TimeOnly LocalTime { get; set; }
    public ICollection<TestChild> Children { get; set; } = new List<TestChild>();
}
