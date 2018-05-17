using System.Collections.Generic;

namespace ClientDataBase
{
#if UNITY_EDITOR
    using System.Xml.Serialization;

    [XmlRoot("Config")]
    public class CheckConfigs
    {
        [XmlElement("Table")]
        [XmlArrayItem("Table")]
        public List<TableCheckConfig> TableCheckConfigs { get; set; }
    }

    /// <summary>
    /// 表格验证配置
    /// </summary>
    [XmlRoot("Table")]
    public class TableCheckConfig
    {
        /// <summary>
        /// 表名
        /// </summary>
        [XmlAttribute("name")]
        public string TableName { get; set; }

        /// <summary>
        /// 各字段的验证配置
        /// </summary>
        [XmlElement("Field")]
        [XmlArrayItem("Field")]
        public List<FieldCheckConfig> FieldCheckConfigs { get; set; }
    }

    /// <summary>
    /// 各字段的验证
    /// </summary>
    public class FieldCheckConfig
    {
        /// <summary>
        /// 字段名
        /// </summary>
        [XmlAttribute("name")]
        public string FieldName { get; set; }

        /// <summary>
        /// 验证结点
        /// </summary>
        [XmlElement("ValidateNode")]
        [XmlArrayItem("ValidateNode")]
        public List<ValidateConfig> ValidateConfigs { get; set; }
    }

    public class ValidateConfig
    {
        /// <summary>
        /// 验证名(ValidateNode + "Validate" 就是类名)
        /// </summary>
        [XmlAttribute("name")]
        public string ValidateNode { get; set; }

        /// <summary>
        /// 验证需要的参数
        /// </summary>
        [XmlAttribute]
        public string Parameter { get; set; }

        /// <summary>
        /// 子验证结点
        /// </summary>
        [XmlElement("ValidateNode")]
        [XmlArrayItem("ValidateNode")]
        public List<ValidateConfig> ChildConfigs { get; set; }

        /// <summary>
        /// 对应的验证器
        /// </summary>
        private ITableCheckValidateNodeBase validateNode;

        /// <summary>
        /// 是否初始化过
        /// </summary>
        private bool init;

        public ITableCheckValidateNodeBase ToValidateNode()
        {
            string err = string.Empty;
            return ToValidateNode(ref err);
        }

        public ITableCheckValidateNodeBase ToValidateNode(ref string err)
        {
            if (init)
            {
                return validateNode;
            }

            init = true;

            validateNode = this.MakeValidateNode(ref err);
            return validateNode;
        }
    }
#endif
}
