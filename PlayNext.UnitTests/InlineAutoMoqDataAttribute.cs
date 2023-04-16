using AutoFixture.Xunit2;

namespace PlayNext.UnitTests
{
    public class InlineAutoMoqDataAttribute : InlineAutoDataAttribute
    {
        public InlineAutoMoqDataAttribute(params object[] values) : base(new AutoMoqDataAttribute(), values)
        {
        }
    }
}