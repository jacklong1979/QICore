using System;
using System.Collections.Generic;
using System.Text;

namespace QICore.ElasticSearchCore
{
   public class Test
    {
        private static IFunction1 s = AutofacIoc.Resolve<IFunction1>();

        public static void Write()
        {
            Console.WriteLine(s.GetName());
        }
    }
}
