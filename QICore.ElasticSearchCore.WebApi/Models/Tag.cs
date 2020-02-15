using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QICore.ElasticSearchCore.WebApi.Models
{
    public class Tag
    {
        public string Id { get; set; }
        public string SiteId { get; set; }
        public string ClassID { get; set; }
        public string ClassName { get; set; }
        public string ParentId { get; set; }
        public string NameSpace { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public int Flag { get; set; }
        public int Version { get; set; }
        public int Status { get; set; }
        public string CreatedTime { get; set; }
        public string CreatedUser { get; set; }
        public string LastModifiedTime { get; set; }
        public string LastModifiedUser { get; set; }

    }
}
