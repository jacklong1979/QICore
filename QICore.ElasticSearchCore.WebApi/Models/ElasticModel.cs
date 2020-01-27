using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QICore.ElasticSearchCore.WebApi.Models
{
    public class ElasticModel
    {
        /// <summary>
        /// 类型【0：位号】【1：文档】【2：文件】
        /// </summary>
        public int Type { get; set; }
        public string SiteId { get; set; }
        public string Id{ get; set; }
        public string ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameSpace { get; set; }
        public string ClassId{ get; set; }
        public string ClassName { get; set; }
        public int Version { get; set; }
        public int Status { get; set; }
        public int Flag { get; set; }
        public string CreatedTime { get; set; }
        public string LastModifiedTime { get; set; }
        public string CreatedUser { get; set; }
        public string CreatedUserName { get; set; }
        public string LastModifiedUser { get; set; }
        public string LastModifiedUserName { get; set; }
        public List<object> Attributes { get; set; }
        public List<object> Files { get; set; }
        public List<object> Tags { get; set; }
        public List<object> Docs { get; set; }
    }
}
