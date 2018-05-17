namespace ClientDataBase
{
    using System;
    using System.IO;

    using UnityEngine;

    /// <summary>
    /// 数值范围验证
    /// </summary>
    public class RangeValidate : ITableCheckValidateNodeBase
    {
        private readonly double minValue;

        private readonly double maxValue;

        public RangeValidate(double min, double max)
        {
            minValue = min;
            maxValue = max;
        }

        public bool Constraint(Type type)
        {
            return type == typeof(int) || type == typeof(long) 
                || type == typeof(float) || type == typeof(double) ;
        }

        public bool Validate(Type fieldType, string value)
        {

            Debug.Assert(Constraint(fieldType), "Type Failed:" + GetDesc());

            if (fieldType == typeof(int) || fieldType == typeof(long))
            {
                long min = (long)minValue;
                long max = (long)maxValue;

               long val = Utility.TypeRelate.StringToCommonType<long>(value);

                return val >= min && val <= max;

            }
            else if (fieldType == typeof(float) || fieldType == typeof(double))
            {
                double val = Utility.TypeRelate.StringToCommonType<double>(value);
                return val >= minValue && val <= maxValue;
            }

            return false;
        }

        public string GetDesc()
        {
            return string.Format("数值范围验证({0}, {1})", minValue, maxValue);
        }
    }
}
