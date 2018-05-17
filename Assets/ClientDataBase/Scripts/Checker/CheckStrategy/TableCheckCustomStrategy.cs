namespace ClientDataBase
{
    using System.Collections.Generic;

    public class TableCheckCustomStrategy : ITableCheckStrategyBase
    {
        /// <summary>
        /// 自定义策略
        /// </summary>
        private readonly Dictionary<string, ITableCheckStrategyBase> customCheckers = new Dictionary<string, ITableCheckStrategyBase>();

        public TableCheckCustomStrategy()
        {
            // 这里可以Register自定义的表格验证
        }

        public bool Check<T>(ScriptableObjectBase table, ICollection<T> values, string keyFieldName)
            where T : TableClassBase
        {
            string tableName = table.GetTableName();

            // 是否有自定义策略
            return customCheckers[tableName].Check(table, values, keyFieldName);
        }

        /// <summary>
        /// 是否包含自定义策略
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool Contains(string tableName)
        {
            return customCheckers.ContainsKey(tableName);
        }

        /// <summary>
        /// 注册自定义策略
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="checkStrategy"></param>
        public void Register(string tableName, ITableCheckStrategyBase checkStrategy)
        {
            if (customCheckers.ContainsKey(tableName))
            {
                UnityEngine.Debug.LogError(string.Format("#TableCheck# table[{0}] custom check strategy already exist!!", tableName));
                return;
            }

            customCheckers.Add(tableName, checkStrategy);
        }
    }
}
