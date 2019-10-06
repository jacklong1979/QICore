using System;
using System.Collections.Generic;
using System.Text;

namespace QICore.ElasticSearchCore
{
   public  class test
    {
        static IFunction1 s = AutofacIoc.Resolve<IFunction1>();
        public static void write()
        {
            Console.WriteLine(s.GetName());
        }
    }
}
