using System;
using Xunit;

namespace QICore.NxUnitTest
{
    public class UnitTest1
    {
     
        [Fact(DisplayName = "��������")]
        public void Test1()
        {
            Assert.True(true);
            //���Բ���
            /*
             ���Դ�Сд ignoreCase��
             �������ַ��� Contains
             ������ʽ��Matches
             ��Χ�� InRange:
             �����Լ��ϵ�ÿ��Ԫ�ؽ���Assert, ��Ȼ����ͨ��ѭ����Assert��, ���Ǹ��õ�д���ǵ���Assert.All()����:
             Assert.All(plumber.Tools, t => Assert.False(string.IsNullOrEmpty(t)));
            */
        }
    }
}
