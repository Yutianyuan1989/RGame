#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

using ClientDataBase;
 using UnityEditor;


public static class CheckExtensions
{
    /// <summary>
    /// 表格配置转成对应的类
    /// </summary>
    /// <param name="validateConfig"> </param>
    /// <param name="err"> The err. </param>
    /// <returns> </returns>
    public static ITableCheckValidateNodeBase MakeValidateNode(this ValidateConfig validateConfig, ref string err)
    {
        var validateNode = DoMakeValidateNode(validateConfig.ValidateNode, validateConfig.Parameter, validateConfig.ChildConfigs, ref err);

        return validateNode;
    }

    private static ITableCheckValidateNodeBase DoMakeValidateNode(string validateNodeName, string parameter, List<ValidateConfig> childeConfigs, ref string err)
    {
        ITableCheckValidateNodeBase validateNode = null;
        if (string.Compare(validateNodeName, "DirectoryExist", StringComparison.OrdinalIgnoreCase) == 0)
        {
            validateNode = new DirectoryExistValidate(parameter);
        }
        else if (string.Compare(validateNodeName, "FileExist", StringComparison.OrdinalIgnoreCase) == 0)
        {
            validateNode = new FileExistValidate(parameter);
        }
        else if (string.Compare(validateNodeName, "Equal", StringComparison.OrdinalIgnoreCase) == 0)
        {
            validateNode = new EqualValidate(parameter);
        }
        else if (string.Compare(validateNodeName, "NoEqual", StringComparison.OrdinalIgnoreCase) == 0)
        {
            validateNode = new NoEqualValidate(parameter);
        }
        else if (string.Compare(validateNodeName, "Range", StringComparison.OrdinalIgnoreCase) == 0)
        {
            string[] rangeValues = parameter.Split(',');
            double min, max;
            if (rangeValues.Length == 2 && double.TryParse(rangeValues[0], out min)
                && double.TryParse(rangeValues[1], out max))
            {
                validateNode = new RangeValidate(min, max);
            }
            else
            {
                err = "期望的参数为2个，以,进行分格，并可以转化为浮点数";
            }
        }
        else if (string.Compare(validateNodeName, "NoEmpty", StringComparison.OrdinalIgnoreCase) == 0)
        {
            validateNode = new NoEmptyValidate();
        }
        else if (string.Compare(validateNodeName, "Lua", StringComparison.OrdinalIgnoreCase) == 0)
        {
            //validateNode = new LuaValidate(parameter);
        }
        else if (string.Compare(validateNodeName, "SpriteExist", StringComparison.OrdinalIgnoreCase) == 0)
        {
            validateNode = new SpriteExistValidate(parameter);
        }
        else if (string.Compare(validateNodeName, "ClassExist", StringComparison.OrdinalIgnoreCase) == 0)
        {
            validateNode = new ClassExist(parameter);
        }
        else if (string.Compare(validateNodeName, "FieldDependent", StringComparison.OrdinalIgnoreCase) == 0)
        {
            string[] rangeValues = parameter.Split(':');

            bool bOk = true;
            do
            {
                if (rangeValues.Length == 2)
                {
                    string tableName = rangeValues[0];
                    string fieldName = rangeValues[1];

#if UNITY_EDITOR
                    string tablePath = ClientDataBaseManager.Get().m_config.GetTableClassPath() + ClientDataBaseManager
                                           .Get().m_config.GetTableClassScriptName(tableName, true);
                    MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(tablePath);
                    if (script == null || script.GetClass() == null)
                    {
                        // 表不存在
                        bOk = false;
                        break;
                    }

                    var realFieldName = Utility.TypeRelate.CorrectFieldName(fieldName);
                    var filedObj = script.GetType().GetProperty(realFieldName);
                    if (filedObj != null)
                    {
                        // 字段不存在
                        bOk = false;
                        break;
                    }
#endif
                    validateNode = new FieldDependentValidate(tableName, fieldName);
                }
            }
            while (false);

            if (!bOk)
            {
                err = "期望的参数为1个(表名:字段名), 同时要求表名跟字段名存在";
            }
        }
        else if (string.Compare(validateNodeName, "Filter", StringComparison.OrdinalIgnoreCase) == 0)
        {
            string[] filterValues = parameter.Split('|');
            if (filterValues.Length == 2)
            {
                string filterErr = string.Empty;
                ITableCheckValidateNodeBase filterNode = DoMakeValidateNode(filterValues[0], filterValues[1], childeConfigs, ref filterErr);

                if (filterNode == null)
                {
                    err = string.Format("过滤参数配置出错:({0})", filterErr);
                    return null;
                }

                List<ITableCheckValidateNodeBase> childValidateNodes = new List<ITableCheckValidateNodeBase>();
                if (childeConfigs != null)
                {
                    foreach (var childConfig in childeConfigs)
                    {
                        filterErr = string.Empty;
                        ITableCheckValidateNodeBase childeValide = DoMakeValidateNode(childConfig.ValidateNode, childConfig.Parameter, childConfig.ChildConfigs, ref filterErr);
                        if (childeValide == null)
                        {
                            err = string.Format("过滤结点的子验证结点配置出错:([{0}]{1})", childConfig.ValidateNode, filterErr);
                            return null;
                        }
                        childValidateNodes.Add(childeValide);
                    }
                }

                if (childValidateNodes.Count != 1)
                {
                    err = "过滤结点的子验证没有配置或者配置过多，期望配置一个";
                    return null;
                }

                validateNode = new FilterValidate(filterNode, childValidateNodes);

            }
            else
            {
                err = "期望的参数以'|'分割，前面的为真正的验证结点,后面的为验证结点的参数";
            }
        }
        else
        {
            err = "无法找到对应的验证器:" + validateNodeName;
        }

        return validateNode;
    }
}
#endif