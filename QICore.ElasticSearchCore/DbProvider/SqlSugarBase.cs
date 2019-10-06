using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace QICore.ElasticSearchCore.DbProvider
{
    public class SqlSugarBase
    {
        public static string ConnectionString { get; set; }
        public static SqlSugarClient DbContext
        {
            get => new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnectionString,
                DbType = DbType.MySql,
                IsAutoCloseConnection = false,  //默认 false, 时候知道关闭数据库连接, 设置为true无需使用using或者Close操作
                InitKeyType = InitKeyType.SystemTable,
                IsShardSameThread = true////默认SystemTable, 字段信息读取, 如：该属性是不是主键，是不是标识列等等信息
            }
            );
        }
    }
}
