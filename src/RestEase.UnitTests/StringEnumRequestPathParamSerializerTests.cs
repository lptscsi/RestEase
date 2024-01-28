using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Xunit;

namespace RestEase.UnitTests
{
    public class StringEnumRequestPathParamSerializerTests
    {
        private static readonly RequestPathParamSerializerInfo info = new(null, null, null);

        private static readonly RequestPathParamSerializer serializer = new StringEnumRequestPathParamSerializer();

        private enum Foo
        {
            Bar,

            [EnumMember(Value = "enum_member")]
            Baz,

            [Display(Name = "display")]
            Fizz,
            Buzz,

            [EnumMember(Value = "all_enum_member")]
            [Display(Name = "all_display")]
            All,

            [Display(Name = "display+name_display")]
            DisplayAndName
        }

        [Fact]
        public void UndecoratedEnumUsesToString()
        {
            string serialized = serializer.SerializePathParam(Foo.Bar, info);
            Assert.Equal("Bar", serialized);
        }

        [Fact]
        public void EnumMemberDecoratedUsesValue()
        {
            string serialized = serializer.SerializePathParam(Foo.Baz, info);

#if NETCOREAPP1_0
            const string expected = "Baz";
#else
            const string expected = "enum_member";
#endif

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void DisplayDecoratedUsesName()
        {
            string serialized = serializer.SerializePathParam(Foo.Fizz, info);

            const string expected = "display";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void DisplayNameDecoratedUsesName()
        {
            string serialized = serializer.SerializePathParam(Foo.Buzz, info);

#if NETCOREAPP2_0
            const string expected = "display_name";
#else
            const string expected = "Buzz";
#endif

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void PrioritisesEnumMemberAboveAll()
        {
            string serialized = serializer.SerializePathParam(Foo.All, info);

#if NETCOREAPP1_0
            const string expected = "all_display";
#else
            const string expected = "all_enum_member";
#endif

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void PrioritisesDisplayNameOverDisplay()
        {
            string serialized = serializer.SerializePathParam(Foo.DisplayAndName, info);

#if NETCOREAPP2_0
            const string expected = "display+name_display_name";
#else
            const string expected = "display+name_display";
#endif

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void NonEnumValueUsesToString()
        {
            string serialized = serializer.SerializePathParam(new NotAnEnum(), info);
            Assert.Equal("NotAnEnum", serialized);
        }

        private class NotAnEnum
        {
            public override string ToString()
            {
                return "NotAnEnum";
            }
        }
    }
}
