﻿using QICore.ElasticSearchCore.WebApi.DbProvider;
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
            using (var db = DbConnection)
            {
                var sql = $@"select b.name ClassName, a.* from tag a
                                    left join tagclass b on b.id = a.ClassID and b.`Status`= 0
                                    where a.`Status`= 0  ";
                 list = db.QueryList<ElasticModel>(sql, pageIndex, pageSize).ToList();
                if (list != null && list.Count> 0)
                {
                    var tagids = list.Select(c => c.Id).ToList();
                    #region 位号关联的文件
                     sql = $@"select a.fileid,a.tagid, c.* from  tagfileassociation a
                            inner join tag b on b.id = a.tagid
                            inner join documentfile c on c.id = a.fileid
                            where a.`Status`= 0 and b.`Status`= 0 and c.`Status`= 0
                            and a.tagid in @tagids";
                    var fileList = db.Query<DocumentFile>(sql, new { tagids = tagids });
                    if (fileList != null && fileList.Count() > 0)
                    {
                        foreach (var ent in list)
                        {
                            var files = fileList.Where(c => c.TagId == ent.Id).ToList();
                            if (files != null && files.Count > 0)
                            {
                                ent.Files = files;
                            }
                        }
                    }
                    #endregion
                }
                //var jarryRows = dataList.ToJArray();

                //foreach (var row in jarryRows)
                //{
                //    var model = new ElasticModel();
                //    var value = row.Find("Name");//查询某列的值 
                //    model.Name = row.Find("Name")?.ToString();
                //    list.Add(model);
                //}

            }

            //using (var db = DbContext)
            //{
            //    var tagList = db.Queryable<Tag>().ToPageList(pageIndex, pageSize);
            //    if (tagList != null && tagList.Count > 0)
            //    {
            //        var tagids = tagList.Select(c => c.Id).ToList();
            //        #region 位号关联的文件
            //        var sql = $@"select  c.* from  tagfileassociation a
            //                inner join tag b on b.id = a.tagid
            //                inner join documentfile c on c.id = a.fileid
            //                where a.`Status`= 0 and b.`Status`= 0 and c.`Status`= 0
            //                and a.tagid in @tagids";
            //        var fileList = db.Ado.SqlQuery<object>(sql, new { tagids = tagids });
            //        #endregion
            //        foreach (var ent in tagList)
            //        {
            //            var model = new ElasticModel();
            //            model.Id = ent.Id;
            //            model.SiteId = ent.SiteId;
            //            // model.ClassID = ent.;
            //            model.ParentId = ent.ParentId;
            //            model.NameSpace = ent.NameSpace;
            //            model.Code = ent.Code;
            //            model.Name = ent.Name;
            //            //model.ShortDescription = ent.ShortDescription;
            //            //model.LongDescription = ent.LongDescription;
            //            model.Flag = ent.Flag;
            //            model.Version = ent.Version;
            //            model.Status = ent.Status;
            //            model.CreatedTime = ent.CreatedTime;
            //            model.CreatedUser = ent.CreatedUser;
            //            model.LastModifiedTime = ent.LastModifiedTime;
            //            model.LastModifiedUser = ent.LastModifiedUser;
            //            list.Add(model);

            //        }
            //    }               
            //}
            return list;
        }
    }

}
