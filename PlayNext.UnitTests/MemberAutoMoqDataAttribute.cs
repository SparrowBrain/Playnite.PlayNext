using AutoFixture.Xunit2;

namespace PlayNext.UnitTests
{
    public class MemberAutoMoqDataAttribute : MemberAutoDataAttribute
    {
        public MemberAutoMoqDataAttribute(string memberName, params object[] parameters)
            : base(new AutoMoqDataAttribute(), memberName, parameters)
        {
        }
    }
}