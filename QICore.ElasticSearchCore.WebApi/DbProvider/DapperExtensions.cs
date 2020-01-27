using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QICore.ElasticSearchCore.WebApi.DbProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
namespace QICore.ElasticSearchCore.WebApi.DbProvider
{
    /// <summary>
    /// 对象扩展方法
    /// </summary>
    public static partial class DapperExtensions
    {
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db">IDbConnection</param>
        /// <param name="sql">SQL语句，不带limit</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页显页记录数据</param>
        /// <param name="param">参数</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        public static IEnumerable<T> QueryList<T>(this IDbConnection db, string sql,int pageIndex,int pageSize, object param = null, IDbTransaction transaction = null)
        {
            var start = (pageIndex - 1) * pageSize;            
            var  exeSql = $"{sql} limit {start},{pageSize};";
            var dataList = db.Query<T>(exeSql, param,transaction);
            return dataList;
        }
    }
}
