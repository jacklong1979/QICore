using System;
using System.Collections.Generic;

namespace QICore.Modes
{
    public class InterfaceEntity
    {
        /// <summary>
        /// 输出路径
        /// </summary>
        public string OutPath { get; set; }
        /// <summary>
        ///命名空间
        /// </summary>
        public string NameSpace { get; set; }
        /// <summary>
        /// 接口列表
        /// </summary>
        public List<InterfaceMode> InterfaceList { get; set; }
    }
    public class InterfaceMode
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string InterfaceName { get; set; }
        /// <summary>
        /// 功能列表
        /// </summary>
        public List<Feature> Features { get; set; }
    }
    public class Feature
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 返回的结构果
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public object Parameters { get; set; }       
        /// <summary>
        /// 中文说明|前端控件名称|逻辑说明
        /// </summary>
        public string Description { get; set; }
    }
}
