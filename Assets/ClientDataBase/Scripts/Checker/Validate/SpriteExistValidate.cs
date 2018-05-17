namespace ClientDataBase
{
    using System;

    /// <summary>
    /// 验证图标是否存在
    /// </summary>
    public class SpriteExistValidate : ITableCheckValidateNodeBase
    {
        public SpriteExistValidate(string root)
        {
        }

        bool ITableCheckValidateNodeBase.Constraint(Type type)
        {
            return type == typeof(string);
        }

        bool ITableCheckValidateNodeBase.Validate(Type fieldType, string value)
        {
            return string.IsNullOrEmpty(value);
        }

        string ITableCheckValidateNodeBase.GetDesc()
        {
            return "UI贴图位置检查";
        }
    }
}
