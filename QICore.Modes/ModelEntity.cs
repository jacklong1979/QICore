using System;
using System.Collections.Generic;

namespace QICore.Modes
{
    public class ModelEntity
    {
        public string OutPath { get; set; }
        public string NameSpace { get; set; }
        public List<Model> Models { get; set; }
    }
    public class Model
    {
        public string TableName { get; set; }
        public List<Column> Columns { get; set; }
    }
    public class Column
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        ///是否是主键
        /// </summary>
        public string IsPrimaryKey { get; set; }
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool ReadOnly { get; set; }
        /// <summary>
        /// 中文说明|前端控件名称|逻辑说明
        /// </summary>
        public string Comment { get; set; }
    }
}
