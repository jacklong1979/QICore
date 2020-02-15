using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using SqlSugar;

namespace QICore.ElasticSearchCore.WebApi.DbProvider
{
    /// <summary>
    /// 其类
    /// </summary>
    public class SqlSugarBase
    {
        public static string ConnectionString { get; set; }
        /// <summary>
        /// SqlSugarClient.
        /// </summary>
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

        /// <summary>
        /// MySqlConnection.
        /// </summary>
        public static MySql.Data.MySqlClient.MySqlConnection DbConnection
        {
            get
            {
                MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }
                return conn;
            }
        }
    }
}
