using System;


namespace Edu.CSV
{
    public class CsvConfigField
    {
        public int Sequence { get; private set; }

        /// <summary>
        /// 是否是关键字
        /// </summary>
        public bool IsKey { get; private set; }

        /// <summary>
        /// 是否是索引
        /// </summary>
        public bool IsIndex { get; private set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name {get; private set; }
        
        public CsvValue Value { get; set; }

        public CsvConfigField(int sequence, bool isKey, bool isIndex, string name, CsvValue value)
        {
            Sequence = sequence;
            IsKey = isKey;
            Name = name;
            IsIndex = isIndex;
            Value = value;
        }

        public static implicit operator int(CsvConfigField field)
        {
            return field.Value;
        }
        public static implicit operator long(CsvConfigField field)
        {
            return field.Value;
        }
        public static implicit operator float(CsvConfigField field)
        {
            return field.Value;
        }
        public static implicit operator double(CsvConfigField field)
        {
            return field.Value;
        }
        public static implicit operator bool(CsvConfigField field)
        {
            return field.Value;
        }
        public static implicit operator string(CsvConfigField field)
        {
            return field.Value;
        }

        public static implicit operator int[](CsvConfigField field)
        {
            return (field == null? new int[0] : field.Value);
        }

        public static implicit operator float[] (CsvConfigField field)
        {
            return (field == null ? new float[0] : field.Value);
        }

        public CsvConfigField Clone()
        {
            CsvConfigField temp = this.MemberwiseClone() as CsvConfigField;
            temp.Value = temp.Value.Clone();
            return temp;
        }

    }
}
