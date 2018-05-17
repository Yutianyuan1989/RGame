namespace ClientDataBase
{
    using System;

    /// <summary>
    /// 验证字段非空
    /// </summary>
    public class NoEmptyValidate : ITableCheckValidateNodeBase
    {
        public bool Constraint(Type type)
        {
            return true;
        }

        public bool Validate(Type fieldType, string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public string GetDesc()
        {
            return "验证字段非空(non empty requested)";
        }
    }
}
