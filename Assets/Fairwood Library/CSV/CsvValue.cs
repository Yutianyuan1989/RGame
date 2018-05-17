using System;
//using NUnit.Framework;


namespace Edu.CSV
{
    public sealed class CsvValue
    {
        /// <summary>
        ///a tiny floating point value
        /// <value>0.0001</value>
        /// </summary>
        public const float Epsilon = 0.0001f;

        /// <summary>
        /// <para>value type</para>
        /// <list type="bullet">
        /// <item><see cref="Int"/></item>
        /// <item><see cref="Long"/></item>
        /// <item><see cref="Bool"/></item>
        /// <item><see cref="Float"/></item>
        /// <item><see cref="Double"/></item>
        /// <item><see cref="String"/></item>
        /// <item><see cref="VFixedPoint"/></item>
        /// </list>
        /// </summary>
        public enum CsvValueType
        {
            /// <summary>
            /// none
            /// </summary>
            None,
            /// <summary>
            /// type of <see cref="bool"/>
            /// </summary>
            Bool,
            /// <summary>
            /// type of <see cref="int"/>
            /// </summary>
            Int,
            /// <summary>
            /// type of <see cref="long"/>
            /// </summary>
            Long,
            /// <summary>
            /// type of <see cref="float"/>
            /// </summary>
            Float,
            /// <summary>
            /// type of <see cref="double"/>
            /// </summary>
            Double,
            /// <summary>
            /// type of <see cref="string"/>
            /// </summary>
            String,
            /// <summary>
            /// type of <see cref="VFixedPoint"/>
            /// </summary>
            VFixedPoint,
            /// <summary>
            /// type of <see cref="int[]"/>
            /// </summary> - add by lxh
            ArrayInt,
            /// <summary>
            /// type of <see cref="long[]"/>
            /// </summary> - add by lxh
            ArrayLong,
            /// <summary>
            /// type of <see cref="float[]"/>
            /// </summary> - add by lxh
            ArrayFloat,
            /// <summary>
            /// type of <see cref="string[]"/>
            /// </summary> - add by lxh
            ArrayString,
            /// <summary>
            /// type of <see cref="VFixedPoint[]"/>
            /// </summary> - add by lxh
            ArrayVFixedPoint,
            /// <summary>
            /// type of <see cref="Tuple"/>
            /// </summary>
            Tuple,
        }

        /// <summary>
        /// type of this value
        /// </summary>
        public CsvValueType Type { get; private set; }

        /// <summary>
        /// it can be :
        /// <item><see cref="int"/></item>
        /// <item><see cref="long"/></item>
        /// <item><see cref="bool"/></item>
        /// <item><see cref="float"/></item>
        /// <item><see cref="double"/></item>
        /// <item><see cref="string"/></item>
        /// <item><see cref="VFixedPoint"/></item>
        /// /// <item><see cref="Tuple"/></item>
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// it can be :(by lxh)
        /// <item><see cref="int[]"/></item>
        /// <item><see cref="long[]"/></item>
        /// <item><see cref="float[]"/></item>
        /// <item><see cref="string[]"/></item>
        /// <item><see cref="VFixedPoint[]"/></item>
        /// </summary>
        public object[] ArrayValue { get; set; }

        private CsvValue()
        {
            Value = null;
        }

        public CsvValue(CsvValueType type, string strValue)
        {
            Init(type, strValue);
        }

        public CsvValue(CsvValueType type, object objValue)
        {
            string strValue = objValue.ToString();
            Init(type, strValue);
        }

        private void Init(CsvValueType type, string strValue)
        {
            Type = type;
            switch (type)
            {
                case CsvValueType.Int:
                    int intValue = default(int);
                    if (string.IsNullOrEmpty(strValue) || int.TryParse(strValue, out intValue))
                    {
                        Value = intValue;
                    }
                    else
                    {
                        throw new CsvException(string.Format("parse int error,string is {0}", strValue));
                    }
                    break;
                case CsvValueType.Long:
                    long longValue = default(long);
                    if (string.IsNullOrEmpty(strValue) || long.TryParse(strValue, out longValue))
                    {
                        Value = longValue;
                    }
                    else
                    {
                        throw new CsvException(string.Format("parse long error,string is {0}", strValue));
                    }
                    break;
                case CsvValueType.Bool:
                    string low = null == strValue ? null : strValue.ToLower();
                    if (low == "true" || low == "yes")
                    {
                        Value = true;
                    }
                    else if (string.IsNullOrEmpty(low) || low == "false" || low == "no")
                    {
                        Value = false;
                    }
                    else
                    {
                        throw new CsvException(string.Format("parse bool error,string is {0}", strValue));
                    }
                    break;
                case CsvValueType.Float:
                    float fValue = default(float);
                    if (string.IsNullOrEmpty(strValue) || float.TryParse(strValue, out fValue))
                    {
                        Value = fValue;
                    }
                    else
                    {
                        throw new CsvException(string.Format("parse float error,string is {0}", strValue));
                    }
                    break;
                case CsvValueType.Double:
                    double dValue = default(double);
                    if (string.IsNullOrEmpty(strValue) || double.TryParse(strValue, out dValue))
                    {
                        Value = dValue;
                    }
                    else
                    {
                        throw new CsvException(string.Format("parse double error,string is {0}", strValue));
                    }
                    break;
                case CsvValueType.String:
                    Value = strValue ?? string.Empty;
                    break;

                case CsvValueType.ArrayString: // add by lxh
                case CsvValueType.ArrayInt:
                case CsvValueType.ArrayLong:
                case CsvValueType.ArrayFloat:
                case CsvValueType.ArrayVFixedPoint:
                    string[] arrString = strValue.Split(';');
                    InitForArrayValue(type, arrString);
                    break;

                case CsvValueType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        private void InitForArrayValue(CsvValueType type, string[] values)
        {
            if (values.Length > 0)
            {
                ArrayValue = new object[values.Length];
            }
            else
            {
                throw new CsvException("empty string[] value for ArrayValue");
            }

            switch (type)
            {
                case CsvValueType.ArrayString:
                    ArrayValue = values;
                    break;
                case CsvValueType.ArrayInt:
                    for (int i = 0; i < values.Length; i++)
                    {
                        int tempIntValue = default(int);
                        if (string.IsNullOrEmpty(values[i]) || int.TryParse(values[i], out tempIntValue))
                        {
                            ArrayValue[i] = tempIntValue;
                        }
                        else
                        {
                            throw new CsvException(string.Format("parse int error,string is {0}", values[i]));
                        }
                    }
                    break;
                case CsvValueType.ArrayLong:
                    for (int i = 0; i < values.Length; i++)
                    {
                        long tempLongValue = default(long);
                        if (string.IsNullOrEmpty(values[i]) || long.TryParse(values[i], out tempLongValue))
                        {
                            ArrayValue[i] = tempLongValue;
                        }
                        else
                        {
                            throw new CsvException(string.Format("parse long error,string is {0}", values[i]));
                        }
                    }
                    break;
                case CsvValueType.ArrayFloat:
                    for (int i = 0; i < values.Length; i++)
                    {
                        float tempFloatValue = default(float);
                        if (string.IsNullOrEmpty(values[i]) || float.TryParse(values[i], out tempFloatValue))
                        {
                            ArrayValue[i] = tempFloatValue;
                        }
                        else
                        {
                            throw new CsvException(string.Format("parse float error,string is {0}", values[i]));
                        }
                    }
                    break;
               
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        #region implicit bool
        public static implicit operator bool(CsvValue value)
        {
            if (value.Type == CsvValueType.Bool)
            {
                return (bool) value.Value;
            }
            throw new CsvException(string.Format("this is not a bool,but is {0}",value.Value));
        }

        public static implicit operator CsvValue(bool b)
        {
            return new CsvValue{Type = CsvValueType.Bool,Value = b};
        }
        #endregion implicit bool

        #region implicit int
        public static implicit operator int(CsvValue value)
        {
            if (value.Type == CsvValueType.Int)
            {
                return (int)value.Value;
            }
            if (value.Type == CsvValueType.Long)
            {
                long longValue = (long) value.Value;
                if (longValue > int.MaxValue || longValue < int.MinValue)
                {
                    throw new CsvException(string.Format("can not change to int, but is {0}",value.Value));
                }
                return (int) longValue;
            }
            if (value.Type == CsvValueType.Float)
            {
                float f = (float) value.Value;
                if (f > int.MaxValue || f < int.MinValue)
                {
                    throw new CsvException(string.Format("can not change to int, but is {0}", value.Value));
                }
                return (int) f;
            }
            if (value.Type == CsvValueType.Double)
            {
                double f = (double)value.Value;
                if (f > int.MaxValue || f < int.MinValue)
                {
                    throw new CsvException(string.Format("can not change to int, but is {0}", value.Value));
                }
                return (int)f;
            }

            throw new CsvException(string.Format("this is not a int, but is {0}", value.Value));
        }



        public static implicit operator CsvValue(int value)
        {
            return new CsvValue{Type = CsvValueType.Int,Value = value};
        }

        #endregion implicit int

        #region implicit long
        public static implicit operator long(CsvValue value)
        {
            if (value.Type == CsvValueType.Int)
            {
                return (int) value.Value;
            }
            if (value.Type == CsvValueType.Long)
            {
                return (long)value.Value;
            }
            if (value.Type == CsvValueType.Float)
            {
                float v = (float) value.Value;
                return (long) v;
            }
            if (value.Type == CsvValueType.Double)
            {
                double v = (double) value.Value;
                return (long) v;

            }

            throw new CsvException(string.Format("this is not a long,but is {0}", value.Value));
        }

        public static implicit operator CsvValue(long value)
        {
            return new CsvValue{Type = CsvValueType.Long,Value = value};
        }
        #endregion

        #region implicit float
        public static implicit operator float(CsvValue value)
        {

            if (value.Type == CsvValueType.Int)
            {
                return (int)value.Value;
            }
            if (value.Type == CsvValueType.Long)
            {
                return (long)value.Value;
            }
            if (value.Type == CsvValueType.Float)
            {
                float v = (float)value.Value;
                return v;
            }
            if (value.Type == CsvValueType.Double)
            {
                double v = (double)value.Value;
                return (float)v;

            }

            throw new CsvException(string.Format("this is not a float,but is {0}", value.Value));
        }

        public static implicit operator CsvValue(float value)
        {
            return new CsvValue{Type = CsvValueType.Float,Value = value};
        }
        #endregion

        #region implicit double
        public static implicit operator double(CsvValue value)
        {
            if (value.Type == CsvValueType.Int)
            {
                return (int)value.Value;
            }
            if (value.Type == CsvValueType.Long)
            {
                return (long)value.Value;
            }
            if (value.Type == CsvValueType.Float)
            {
                float v = (float)value.Value;
                return v;
            }
            if (value.Type == CsvValueType.Double)
            {
                double v = (double)value.Value;
                return v;

            }

            throw new CsvException(string.Format("this is not a double,but is {0}", value.Value));
        }

        public static implicit operator CsvValue(double value)
        {
            return new CsvValue{Type = CsvValueType.Double,Value = value};
        }
        #endregion

        #region implicit string
        public static implicit operator string(CsvValue value)
        {
            if (value.Type == CsvValueType.String)
            {
                return (string)value.Value;
            }
            throw new CsvException(string.Format("this is not a string,but is {0}", value.Value));
        }

        public static implicit operator CsvValue(string value)
        {
            return new CsvValue{Type = CsvValueType.String,Value = value};
        }
        #endregion

        #region implicit VFixedPoint

        #endregion

        #region implicit ArrayInt
        public static implicit operator int[](CsvValue value)
        {
            if (value.Type == CsvValueType.ArrayInt)
            {
                int[] resValue = new int[value.ArrayValue.Length];

                for(int i = 0; i < value.ArrayValue.Length; i++)
                {
                    resValue[i] = (int)value.ArrayValue[i];
                }

                return resValue;
            }

            throw new CsvException(string.Format("this is not a int[], but is {0}", value.ArrayValue));
        }
        #endregion

        #region implicit ArrayLong
        public static implicit operator long[] (CsvValue value)
        {
            if (value.Type == CsvValueType.ArrayInt)
            {
                long[] resValue = new long[value.ArrayValue.Length];

                for (int i = 0; i < value.ArrayValue.Length; i++)
                {
                    resValue[i] = (long)value.ArrayValue[i];
                }

                return resValue;
            }

            throw new CsvException(string.Format("this is not a long[], but is {0}", value.ArrayValue));
        }
        #endregion

        #region implicit ArrayFloat
        public static implicit operator float[] (CsvValue value)
        {
            if (value.Type == CsvValueType.ArrayInt)
            {
                float[] resValue = new float[value.ArrayValue.Length];

                for (int i = 0; i < value.ArrayValue.Length; i++)
                {
                    resValue[i] = (float)value.ArrayValue[i];
                }

                return resValue;
            }

            throw new CsvException(string.Format("this is not a float[], but is {0}", value.ArrayValue));
        }
        #endregion

        #region implicit ArrayString
        public static implicit operator string[](CsvValue value)
        {
            if(value.Type == CsvValueType.ArrayString)
            {
                return (string[])value.ArrayValue;
            }

            throw new CsvException(string.Format("this is not a string[], but is {0}", value.ArrayValue));
        }
        #endregion


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }
            CsvValue otherValue = other as CsvValue;
            if (otherValue == null || otherValue.Type == CsvValueType.None)
            {
                return false;
            }

            switch (Type)
            {
                case CsvValueType.Bool:
                    if (otherValue.Type != CsvValueType.Bool)
                    {
                        return false;
                    }
                    return (bool) Value == (bool) otherValue.Value;
                case CsvValueType.Int:
                    if (otherValue.Type == CsvValueType.Bool || otherValue.Type == CsvValueType.String)
                    {
                        return false;
                    }
                    int intValue = (int) Value;
                    switch (otherValue.Type)
                    {
                        case CsvValueType.Int:
                            return intValue == (int) otherValue.Value;
                        case CsvValueType.Long:
                            return intValue == (long)otherValue.Value;
                        case CsvValueType.Float:
                            return Approximately(intValue, (float) otherValue.Value);
                        case CsvValueType.Double:
                            return Approximately(intValue, (double) otherValue.Value);
                    }
                    return false;
                case CsvValueType.Long:
                    if (otherValue.Type == CsvValueType.Bool || otherValue.Type == CsvValueType.String)
                    {
                        return false;
                    }
                    long longValue = (long) Value;
                     
                     switch (otherValue.Type)
                     {
                        case CsvValueType.Int:
                             return longValue == (int) (otherValue.Value);
                        case CsvValueType.Long:
                             return longValue == (long) (otherValue.Value);
                        case CsvValueType.Float:
                             return Approximately(longValue, (float)otherValue.Value);
                        case CsvValueType.Double:
                             return Approximately(longValue, (double) otherValue.Value);
                     }
                    return false;
                case CsvValueType.Float:
                    if (otherValue.Type == CsvValueType.Bool || otherValue.Type == CsvValueType.String)
                    {
                        return false;
                    }
                    float fValue = (float) Value;
                     switch (otherValue.Type)
                     {
                        case CsvValueType.Int:
                             return Approximately((int) otherValue.Value, fValue);
                        case CsvValueType.Long:
                             return Approximately((long)otherValue.Value,fValue);
                        case CsvValueType.Float:
                             return Approximately((float)otherValue.Value,fValue);
                        case CsvValueType.Double:
                             return Approximately((double) otherValue.Value,fValue);
                     }
                    return false;

                case CsvValueType.Double:
                    if (otherValue.Type == CsvValueType.Bool || otherValue.Type == CsvValueType.String)
                    {
                        return false;
                    }
                    double dValue = (double) Value;
                     switch (otherValue.Type)
                     {
                        case CsvValueType.Int:
                             return Approximately((int) otherValue.Value, dValue);
                        case CsvValueType.Long:
                             return Approximately((long)otherValue.Value,dValue);
                        case CsvValueType.Float:
                             return Approximately((float)otherValue.Value,dValue);
                        case CsvValueType.Double:
                             return Approximately((double) otherValue.Value,dValue);
                     }
                    return false;
                case CsvValueType.VFixedPoint:
                    if (otherValue.Type == CsvValueType.VFixedPoint)
                    {
                        return false;
                    }
                    return Value == otherValue.Value;
                case CsvValueType.String:
                    if (otherValue.Type != CsvValueType.String)
                    {
                        return false;
                    }
                    return Value.Equals(otherValue.Value);

                case CsvValueType.ArrayFloat: // add by lxh
                case CsvValueType.ArrayInt:
                case CsvValueType.ArrayLong:
                case CsvValueType.ArrayString:
                case CsvValueType.ArrayVFixedPoint:
                    if(Type != otherValue.Type ||
                        ArrayValue.Length != otherValue.ArrayValue.Length)
                    {
                        return false;
                    }

                    for(int i = 0; i < ArrayValue.Length; i++)
                    {
                        if(ArrayValue[i] != otherValue.ArrayValue[i])
                        {
                            return false;
                        }
                    }

                    break;

                case CsvValueType.None:
                default:
                    throw new ArgumentOutOfRangeException("Error in CsvValue.Equals");
            }

            return false;
        }

        public override string ToString()
        {
            return string.Format("(type:{0},value:{1})",Type,Value);
        }


        private bool Approximately(int v1, float v2)
        {
            double c = Math.Abs(v1 - v2);
            if (c < Epsilon)
            {
                return true;
            }
            return false;
        }
        private bool Approximately(int v1, double v2)
        {
            double c = Math.Abs(v1 - v2);
            if (c < Epsilon)
            {
                return true;
            }
            return false;
        }
        private bool Approximately(long v1, float v2)
        {
            double c = Math.Abs(v1 - v2);
            if (c < Epsilon)
            {
                return true;
            }
            return false;
        }
        private bool Approximately(long v1,double v2)
        {
            double c = Math.Abs(v1 - v2);
            if (c < Epsilon)
            {
                return true;
            }
            return false;
        }
        private bool Approximately(float v1, float v2)
        {
            double c = Math.Abs(v1 - v2);
            if (c < Epsilon)
            {
                return true;
            }
            return false;
        }
        private bool Approximately(float v1, double v2)
        {
            double c = Math.Abs(v1 - v2);
            if (c < Epsilon)
            {
                return true;
            }
            return false;
        }
        private bool Approximately(double v1, double v2)
        {
            double c = Math.Abs(v1 - v2);
            if (c < Epsilon)
            {
                return true;
            }
            return false;
        }

        public CsvValue Clone()
        {
            return this.MemberwiseClone() as CsvValue;
        }

    }
}
