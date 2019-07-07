using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace QICore.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.WriteLine(GenerateCheckCode(2));
            Console.WriteLine(GenerateCheckCode(5));
            Console.WriteLine(GenerateCheckCode(10));
            Console.WriteLine(GenerateCheckCode(15));
          //  Commany commany = new Commany();
            Deparment deparment = new Deparment();
            Person person = new Person();
            //var c = GetObjValue<Commany>(commany);
            var d = GetObjValue(deparment);
            var p = GetObjValue(person);
            Console.Read();
        }
        /// <summary>
        /// 随机产生常用汉字
        /// </summary>
        /// <param name="count">要产生汉字的个数</param>
        /// <returns>常用汉字</returns>
        public static string GenerateChineseWord(int count)
        {
            string chineseWords = "";
            System.Random rm = new System.Random();
            Encoding gb = Encoding.GetEncoding("gb2312");

            for (int i = 0; i < count; i++)
            {
                // 获取区码(常用汉字的区码范围为16-55)
                int regionCode = rm.Next(16, 56);

                // 获取位码(位码范围为1-94 由于55区的90,91,92,93,94为空,故将其排除)
                int positionCode;
                if (regionCode == 55)
                {
                    // 55区排除90,91,92,93,94
                    positionCode = rm.Next(1, 90);
                }
                else
                {
                    positionCode = rm.Next(1, 95);
                }

                // 转换区位码为机内码
                int regionCode_Machine = regionCode + 160;// 160即为十六进制的20H+80H=A0H
                int positionCode_Machine = positionCode + 160;// 160即为十六进制的20H+80H=A0H

                // 转换为汉字
                byte[] bytes = new byte[] { (byte)regionCode_Machine, (byte)positionCode_Machine };
                chineseWords += gb.GetString(bytes);
            }
            return chineseWords;
        }
        /// <summary>
        /// 生成随机字母字符串(数字字母混和)
        /// </summary>
        /// <param name="codeCount"></param>
        /// <returns></returns>
        public static string GenerateCheckCode(int codeCount)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks;
            var rep = DateTime.Now.Second;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }
        public static T GetObjValue<T>(T t)
        {
            //object o = Activator.CreateInstance(type);
            Type type = t.GetType();//获取类型
            PropertyInfo[] proList = type.GetProperties();//获取属性集合
            foreach (PropertyInfo p in proList)
            {
                var name = p.Name;
                var dataType = p.PropertyType;
                if (p.CanWrite)
                {
                    if (dataType == typeof(string))
                    {
                        p.SetValue(t, GenerateCheckCode(5));
                    }
                    if (dataType == typeof(string[]))
                    {
                        #region 5个数组
                        string[] strs = new string[5] ;
                        for (var i = 0; i < 5; i++)
                        {
                            var str = GenerateCheckCode(5);
                            strs[i] = str;
                        }
                        p.SetValue(t, strs);
                        #endregion
                    }
                    else if (dataType == typeof(DateTime))
                    {
                        p.SetValue(t, DateTime.Now);
                    }
                    else if (dataType == typeof(int))
                    {
                        p.SetValue(t, 1);
                    }
                    else if (dataType == typeof(int[]))
                    {
                        #region 5个数组
                        int[] strs = new int[5];
                        for (var i = 0; i < 5; i++)
                        {                           
                            strs[i] = i;
                        }
                        p.SetValue(t, strs);
                        #endregion
                    }
                    else if (dataType == typeof(bool))
                    {
                        p.SetValue(t, 1);
                    }
                    else if (dataType == typeof(decimal))
                    {
                        p.SetValue(t, 1.00);
                    }                   
                    else if (dataType == typeof(Nullable))
                    {
                        p.SetValue(t, true);
                    }
                    else if (dataType.IsClass&& p.PropertyType.GenericTypeArguments.Length==0&&!p.PropertyType.IsGenericType)
                    {                       
                      //  object o = Activator.CreateInstance(p.PropertyType);
                      //  var obj = GetObjValue(o);
                      //  p.SetValue(t, obj);
                      ////  p.SetValue(t, obj!=null ? null : Convert.ChangeType(obj, p.PropertyType), null);
                    }
                    else if (dataType.IsClass && p.PropertyType.GenericTypeArguments.Length > 0&& p.PropertyType.IsGenericType)
                    {
                        object entityList = Activator.CreateInstance(p.PropertyType);
                        BindingFlags flag = BindingFlags.Instance | BindingFlags.Public;
                        //根据成员的类型创建列表 List<T> list = new List<T>();
                        MethodInfo methodInfo = p.PropertyType.GetMethod("Add", flag);
                        //如果是列表，创建5条记录                      
                        for (var i = 0; i < 10; i++)
                        {
                            object o = Activator.CreateInstance(p.PropertyType.GenericTypeArguments[0]);
                            var obj = GetObjValue(o);
                            methodInfo.Invoke(entityList, new object[] { obj });//相当于List<T>调用Add方法
                        }
                        p.SetValue(t, entityList);
                    }
                    #region 可以为空
                    else if (dataType.ToString() == "System.Nullable`1[System.DateTime]")
                    {
                        p.SetValue(t, DateTime.Now);
                    }
                    else if (dataType.ToString() == "System.Nullable`1[System.Int32]")
                    {
                        p.SetValue(t, 1);
                    }
                    else if (dataType.ToString() == "System.Nullable`1[System.Double]")
                    {
                        p.SetValue(t, 1);
                    }
                    else if (dataType.ToString() == "System.Nullable`1[System.Decimal]")
                    {
                        p.SetValue(t,decimal.Parse("10"));
                    }
                    else if (dataType.ToString() == "System.Nullable`1[System.Byte]")
                    {

                    }
                    else if (dataType.ToString() == "System.Nullable`1[System.Char]")
                    {

                    }
                    else if (dataType.ToString() == "System.Nullable`1[System.Enum]")
                    {

                    }
                    else if (dataType.ToString() == "System.Nullable`1[System.Boolean]")
                    {
                        p.SetValue(t, true);
                    }
                    #endregion
                }
            }
            return t;
        }
    }
    public class Commany
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int  No { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
      
    }
    public class Deparment: Commany
    {
        public string Id { get; set; }
        public string DepartName { get; set; }
        public string Header { get; set; }
        public string AdCode { get; } = "A0014";
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Person Person { get; set; }
        public List<Person> PersonList { get; set; }
        public string[] ABC { get; set; }
        public int[] ABE { get; set; }
        public Dictionary<string,object> sObject { get; set; }
        public Dictionary<string, Person> dPerson { get; set; }
    }
    public class Person 
    {
       // public Deparment Deparment { get; set; }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime? Birthday { get; set; }
        public int Age { get; set; }
        private double CertificateNumber { get; set; }
        public decimal? Tuition { get; set; }
        public int Sex { get; set; }
    }
}
