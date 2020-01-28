using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QICore.ElasticSearchCore.WebApi.DbProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QICore.ElasticSearchCore.WebApi
{
    /// <summary>
    /// 对象扩展方法
    /// </summary>
    public static partial class DapperExtensions
    {
        /// <summary>
        /// 对象列表转成JArray
        /// </summary>
        /// <param name="objList"></param>
        /// <returns></returns>
        public static JArray ToJArray(this List<object> objList)
        {            
            string json = JsonConvert.SerializeObject(objList);
            JArray rows = (JArray)JsonConvert.DeserializeObject(json);
            return rows;
        }
       
        /// <summary>
        /// 对象列表转成JArray
        /// </summary>
        /// <param name="objList"></param>
        /// <returns></returns>
        public static JArray ToJArray(this IEnumerable<object> objList)
        {
            string json = JsonConvert.SerializeObject(objList);
            JArray rows = (JArray)JsonConvert.DeserializeObject(json);
            return rows;
        }

        /// <summary>
        /// 获对object 某列的值
        /// </summary>
        /// <param name="row">JToken</param>
        /// <param name="name">字段名称（区分大小写）</param>
        /// <returns></returns>
        public static object Find(this JToken row, string name)
        {
            object value=null;           
            value = ((Newtonsoft.Json.Linq.JValue)((Newtonsoft.Json.Linq.JObject)row).GetValue(name))?.Value;
            return value;
        }
        /// <summary>
        /// 获对object 某列的值
        /// </summary>
        /// <param name="row">object</param>
        /// <param name="name">字段名称（区分大小写）</param>
        /// <returns></returns>
        public static object Find(this object obj, string name)
        {
            object value = null;
            JToken row= obj as JToken;
             value = ((Newtonsoft.Json.Linq.JValue)((Newtonsoft.Json.Linq.JObject)row).GetValue(name))?.Value;
            return value;
        }
    }
}
