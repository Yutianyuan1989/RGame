namespace ClientDataBase
{
    using System.Collections.Generic;

    public interface ITableCheckStrategyBase
    {
        bool Check<T>(ScriptableObjectBase table, ICollection<T> values, string keyFieldName)
            where T : TableClassBase;

        bool Contains(string tableName);
    }
}
