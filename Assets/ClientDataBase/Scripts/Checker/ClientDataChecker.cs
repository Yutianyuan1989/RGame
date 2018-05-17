namespace ClientDataBase
{
    using System;
    using System.Collections.Generic;

    public class ClientDataChecker : Singleton<ClientDataChecker>
    {
        private readonly List<ITableCheckStrategyBase> strategies = new List<ITableCheckStrategyBase>();


        public ClientDataChecker()
        {
            // 在前面的优先处理
            strategies.Add(new TableCheckCustomStrategy());
#if UNITY_EDITOR
            strategies.Add(new TableCheckConfigStrategy());
#endif
            strategies.Add(new TableCheckCommonStrategy());
        }

        public void Check<T>(ScriptableObjectBase table, ICollection<T> values, string keyFieldName = null)
            where T : TableClassBase
        {
            if (values.Count == 0)
            {
                return;
            }

            string tableName = table.GetTableName();
            foreach (var oneStrategy in strategies)
            {
                if (oneStrategy.Contains(tableName))
                {
                    oneStrategy.Check(table, values, keyFieldName);
                    break;
                }
            }
        }

        /// <summary>
        /// 生成最终的lua调用代码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string ConcatLuaCode(string code)
        {
            string script = @"  local function luaFunc(fieldVal)     
";
            string script2 = @"
            end

            tableCheckLua = {}
            tableCheckLua.luaFunc = luaFunc
        ";

            return script + code + script2;
        }

    }
}
