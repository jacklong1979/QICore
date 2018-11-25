using System;
using Xunit;

namespace QICore.NxUnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Assert.True(true);
            //断言参数
            /*
             忽略大小写 ignoreCase：
             包含子字符串 Contains
             正则表达式，Matches
             范围， InRange:
             如果想对集合的每个元素进行Assert, 当然可以通过循环来Assert了, 但是更好的写法是调用Assert.All()方法:
             Assert.All(plumber.Tools, t => Assert.False(string.IsNullOrEmpty(t)));
            */
        }
    }
}
