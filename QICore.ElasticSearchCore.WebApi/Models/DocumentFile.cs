using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QICore.ElasticSearchCore.WebApi.Models
{
    public class DocumentFile
    {
        public DateTime CreatedTime { get; set; }
        public string CreatedUser { get; set; }
        public string DocumentId { get; set; }
        public string FileName { get; set; }
        public string FileNameOrigin { get; set; }
        public int Flag { get; set; }
        public string FullText { get; set; }
        public string Id { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string LastModifiedUser { get; set; }
        public string Md5 { get; set; }
        public string Md5Origin { get; set; }
        public int Revision { get; set; }
        public string SiteId { get; set; }
        public int Size { get; set; }
        public int Status { get; set; }
        public int Version { get; set; }
        public string TagId { get; set; }
        public string FileId { get; set; }
    }
}
