namespace ClientDataBase
{
    using System;

    /// <summary>
    /// 验证图标是否存在
    /// </summary>
    public class ClassExist : ITableCheckValidateNodeBase
    {
        readonly string @namespace;

        public ClassExist(string root)
        {
            @namespace = root;
        }

        bool ITableCheckValidateNodeBase.Constraint(Type type)
        {
            return type == typeof(string);
        }

        bool ITableCheckValidateNodeBase.Validate(Type fieldType, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            string fullName = @namespace + "." + value;
            Type type = null;
            try
            {
                type = Type.GetType(fullName);
            }
            catch
            {

            }

            return (type != null);
        }

        string ITableCheckValidateNodeBase.GetDesc()
        {
            return "脚本文件是否存在检查";
        }
    }
}
