using QICore.ElasticSearchCore.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QICore.ElasticSearchCore.WebApi.Dao
{
    /// <summary>
    /// 接口基类
    /// </summary>
    public interface ITag : IBase
    {
        /// <summary>
        /// 获取位号列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<ElasticModel> GetTagList(int pageIndex,int pageSize);
      
    }
   
}
