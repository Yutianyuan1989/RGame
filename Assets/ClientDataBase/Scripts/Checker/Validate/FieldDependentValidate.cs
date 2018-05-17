namespace ClientDataBase
{
    using System;

    /// <summary>
    /// 验证文件是否存在
    /// </summary>
    public class FieldDependentValidate : ITableCheckValidateNodeBase
    {
        private readonly string tableName;
        private readonly string fieldName;

        public FieldDependentValidate(string table, string field)
        {
            tableName = table;
            fieldName = field;
        }

        public bool Constraint(Type type)
        {
            return true;
        }

        public bool Validate(Type fieldType, string value)
        {
            ScriptableObjectBase t = ClientDataBaseManager.Get().LoadTable(tableName);

            if (t == null)
            {
                return false;
            }

            return t.CheckField(fieldName, value);
        }

        public string GetDesc()
        {
            return string.Format("验证值是否存在于[{0}:{1}]", tableName, fieldName);
        }
    }
}
