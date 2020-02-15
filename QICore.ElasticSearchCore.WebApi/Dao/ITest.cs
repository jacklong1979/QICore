﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QICore.ElasticSearchCore.WebApi.Dao
{
    /// <summary>
    /// 接口基类（不实现任务功能）
    /// </summary>
    public interface ITest : IBase
    {
        string GetTest(string name);
      
    }
    public class Test : ITest
    {
        public string GetTest(string name)
        {
            return "您好：" + name;
        }
    }
}
