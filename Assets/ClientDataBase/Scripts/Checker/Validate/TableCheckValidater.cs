using System.Collections;
using System.Collections.Generic;

namespace ClientDataBase
{
    public class TableCheckValidater : IEnumerable<KeyValuePair<string, List<ITableCheckValidateNodeBase>>>
    {
        /// <summary>
        /// 各字段的验证节点
        /// </summary>
        private readonly Dictionary<string, List<ITableCheckValidateNodeBase>> fieldValidates = new Dictionary<string, List<ITableCheckValidateNodeBase>>();

        /// <summary>
        /// 该字段是否要验证
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public bool Contains(string fieldName)
        {
            return fieldValidates.ContainsKey(fieldName);
        }

        /// <summary>
        /// 增加字段的验证
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="validateAction"></param>
        public void AddValidate(string fieldName, ITableCheckValidateNodeBase validateAction)
        {
            if (!Contains(fieldName))
            {
                fieldValidates.Add(fieldName, new List<ITableCheckValidateNodeBase>());
            }

            fieldValidates[fieldName].Add(validateAction);
        }

        public IEnumerator<KeyValuePair<string, List<ITableCheckValidateNodeBase>>> GetEnumerator()
        {
            return fieldValidates.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

