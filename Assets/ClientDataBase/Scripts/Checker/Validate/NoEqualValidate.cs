namespace ClientDataBase
{
    using System;
    using System.IO;

    using UnityEngine;

    /// <summary>
    /// 不等性验证
    /// </summary>
    public class NoEqualValidate : ITableCheckValidateNodeBase
    {
        private readonly string mValidateVal;

        public NoEqualValidate(string val)
        {
            mValidateVal = val;
        }

        public bool Constraint(Type type)
        {
            return true;
        }

        public bool Validate(Type fieldType, string value)
        {
            return mValidateVal != value;
        }

        public string GetDesc()
        {
            return string.Format("不等性验证!=({0})", mValidateVal);
        }
    }
}
