namespace ClientDataBase
{
#if UNITY_EDITOR
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using UnityEditor;
    using UnityEngine;

    public class TableCheckConfigStrategy : ITableCheckStrategyBase
    {
        private const string CheckConfigFileName = "CheckConfig.xml";

        /// <summary>
        /// 各自表格验证配置
        /// </summary>
        private readonly CheckConfigs tableCheckConfigs;

        public TableCheckConfigStrategy()
        {
            string configPath = ClientDataBaseManager.Get().m_config.m_root + CheckConfigFileName;
            if (File.Exists(configPath))
            {
                File.SetAttributes(configPath, FileAttributes.Normal);

                tableCheckConfigs = (CheckConfigs)XmlUtil.DeserializeFromXml(typeof(CheckConfigs), configPath);
                if (tableCheckConfigs == null)
                {
                    Debug.LogError(string.Format("#TableCheck# 验证表格的配置读取错误，请确认({0})", configPath));
                }
                else
                {
                    ValidateXmlConfig();
                }
            }
        }

        public bool Check<T>(ScriptableObjectBase table, ICollection<T> values, string keyFieldName)
            where T : TableClassBase
        {
            // 是否这个table的配置检测
            string tableName = table.GetTableName();
            var tableConfig = GetTableConfig(tableName);
            if (tableConfig == null)
            {
                return true;
            }

            bool success = true;

            // ValidateXmlConfig已经做过提示了, 所以这里只做数据验证提示
            foreach (var fieldConfig in tableConfig.FieldCheckConfigs)
            {
                string fieldName = Utility.TypeRelate.CorrectFieldName(fieldConfig.FieldName);

                // 是否有该字段的属性
                var propertyInfo = typeof(T).GetProperty(fieldName);
                if (propertyInfo == null)
                {
                    continue;
                }

                foreach (var fieldValidater in fieldConfig.ValidateConfigs)
                {
                    // 是否存在验证node
                    var validateNode = fieldValidater.ToValidateNode();
                    if (validateNode == null)
                    {
                        continue;
                    }

                    // 验证约束
                    if (!validateNode.Constraint(propertyInfo.PropertyType))
                    {
                        continue;
                    }

                    bool keyExist = !string.IsNullOrEmpty(keyFieldName);

                    int row = 1;
                    foreach (var oneValue in values)
                    {
                        string val = propertyInfo.GetValue(oneValue, null).ToString();
                        if (!validateNode.Validate(propertyInfo.PropertyType, val))
                        {
                            if (keyExist)
                            {
                                string keyValue = oneValue.GetPropertyValue(keyFieldName).ToString();

                                Debug.LogError(
                                    string.Format(
                                        "#TableCheck# 表格[{0}]主键为[{1}({2})]行[{3}]列的值[{4}]不能通过[{5}]验证 ",
                                        tableName,
                                        keyFieldName,
                                        keyValue,
                                        fieldName,
                                        val,
                                        validateNode.GetDesc()));
                            }
                            else
                            {
                                Debug.LogError(
                                    string.Format(
                                        "#TableCheck# 表格[{0}]第[{1}]行[{2}]列的值[{3}]不能通过[{4}]验证 ",
                                        tableName,
                                        row.ToString(),
                                        fieldName,
                                        val,
                                        validateNode.GetDesc()));
                            }
                        }

                        row++;
                    }

                    success = false;
                }
            }

            return success;
        }

        public bool Contains(string tableName)
        {
            return GetTableConfig(tableName) != null;
        }

        /// <summary>
        /// 获得表格的配置
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public TableCheckConfig GetTableConfig(string tableName)
        {
            if (tableCheckConfigs == null)
            {
                return null;
            }

            foreach (var tableConfig in tableCheckConfigs.TableCheckConfigs)
            {
                if (string.Compare(tableConfig.TableName, tableName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return tableConfig;
                }
            }

            return null;
        }

        /// <summary>
        /// 验证xml是否配置
        /// </summary>
        private void ValidateXmlConfig()
        {
            if (tableCheckConfigs == null)
            {
                return;
            }

            foreach (var tableConfig in tableCheckConfigs.TableCheckConfigs)
            {
                DoTableValidate(tableConfig);
            }
        }

        /// <summary>
        /// 表格验证
        /// </summary>
        /// <param name="tableConfig"></param>
        private void DoTableValidate(TableCheckConfig tableConfig)
        {
            string tableName = tableConfig.TableName;
#if UNITY_EDITOR
            string tablePath = ClientDataBaseManager.Get().m_config.GetTableClassPath()
                               + ClientDataBaseManager.Get().m_config.GetTableClassScriptName(tableName, true);
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(tablePath);
            if (script == null || script.GetClass() == null)
            {
                Debug.LogError(
                    string.Format("#TableCheck# 配置的表名[{0}] 没有对应的csv或者没有生成对应的脚本代码", tableName));
                return;
            }

            foreach (var fieldConfig in tableConfig.FieldCheckConfigs)
            {
                DoFieldValidate(fieldConfig, script.GetClass(), tableName);
            }
#endif
        }

        /// <summary>
        /// 字段验证
        /// </summary>
        /// <param name="fieldConfig"></param>
        /// <param name="tableClass"></param>
        /// <param name="tableName"></param>
        private void DoFieldValidate(FieldCheckConfig fieldConfig, Type tableClass, string tableName)
        {
            string fieldName = fieldConfig.FieldName;

            var realFieldName = Utility.TypeRelate.CorrectFieldName(fieldName);
            var filedObj = tableClass.GetProperty(realFieldName);
            if (filedObj == null)
            {
                Debug.LogError(
                    string.Format(
                        "#TableCheck# 表[{0}]里的字段名[{1}]不存在",
                        tableName,
                        realFieldName));
                return;
            }

            foreach (var fieldValidater in fieldConfig.ValidateConfigs)
            {
                DoNodeValidate(tableName, fieldValidater, fieldName, filedObj);
            }
        }

        private void DoNodeValidate(
            string tableName,
            ValidateConfig fieldValidater,
            string fieldName,
            PropertyInfo filedObj)
        {
            string err = string.Empty;
            var validateNode = fieldValidater.ToValidateNode(ref err);
            if (validateNode == null)
            {
                Debug.LogError(
                    string.Format(
                        "#TableCheck# 表里[{0} : {1}]的验证结点[{2}]配置不正确. 原因:[{3}]",
                        tableName,
                        fieldName,
                        fieldValidater.ValidateNode,
                        err));
            }
            else if (!validateNode.Constraint(filedObj.PropertyType))
            {
                Debug.LogError(
                    string.Format(
                        "#TableCheck# 表里[{0} : {1}]无法满足验证结点[{2}]的类型条件, 结点描述:[{3}]",
                        tableName,
                        fieldName,
                        fieldValidater.ValidateNode,
                        validateNode.GetDesc()));
            }
        }
    }
#endif
}