using QICore.ElasticSearchCore.WebApi.DbProvider;
using QICore.ElasticSearchCore.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QICore.ElasticSearchCore.WebApi.Dao
{
    /// <summary>
    /// 位号类
    /// </summary>
    public class TagService : SqlSugarBase, ITag
    {
        public List<ElasticModel> GetTagList(int pageIndex=1, int pageSize=10)
        {
            List<ElasticModel> list = new List<ElasticModel>();
            //using (var db = DbConnection)
            //{
            //    var sql = $"select * from tag ";
            //    var dataList = db.QueryList<object>(sql,pageIndex,pageSize);
            //    var jarryRows=dataList.ToJArray();

            //    foreach (var row in jarryRows)
            //    {
            //        var model = new ElasticModel();
            //        //var value = row.Find("Name");//查询某列的值 
            //        model.Name= row.Find("Name")?.ToString();
            //        list.Add(model);
            //    }              
            //}

            using (var db = DbContext)
            {

                var tagList = db.Queryable<Tag>().ToPageList(pageIndex, pageSize);
                if (tagList != null && tagList.Count > 0)
                {
                    foreach (var ent in tagList)
                    {
                        var model = new ElasticModel();
                        model.Id = ent.Id;
                        model.SiteId = ent.SiteId;
                        // model.ClassID = ent.;
                        model.ParentId = ent.ParentId;
                        model.NameSpace = ent.NameSpace;
                        model.Code = ent.Code;
                        model.Name = ent.Name;
                        //model.ShortDescription = ent.ShortDescription;
                        //model.LongDescription = ent.LongDescription;
                        model.Flag = ent.Flag;
                        model.Version = ent.Version;
                        model.Status = ent.Status;
                        model.CreatedTime = ent.CreatedTime;
                        model.CreatedUser = ent.CreatedUser;
                        model.LastModifiedTime = ent.LastModifiedTime;
                        model.LastModifiedUser = ent.LastModifiedUser;
                        list.Add(model);

                    }
                }
            }
            return list;
        }
    }

}
