namespace ClientDataBase
{
    using System.Collections.Generic;

    public class TableCheckCommonStrategy : ITableCheckStrategyBase
    {
        public bool Check<T>(ScriptableObjectBase table, ICollection<T> values, string keyFieldName)
            where T : TableClassBase
        {
            return false;
        }

        public bool Contains(string tableName)
        {
            return false;
        }
    }
}
