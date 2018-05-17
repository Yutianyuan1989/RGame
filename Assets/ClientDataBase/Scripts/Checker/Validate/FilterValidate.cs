namespace ClientDataBase
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 过滤结点: 只有满足这个条件的才进行验证，不然都是都算验证通过
    /// </summary>
    public class FilterValidate : ITableCheckValidateNodeBase
    {
        private readonly ITableCheckValidateNodeBase mFilter;

        private readonly List<ITableCheckValidateNodeBase> mChildCheckValidateNodes;

        public FilterValidate(ITableCheckValidateNodeBase filter, List<ITableCheckValidateNodeBase> childCheckValidateNodes)
        {
            mFilter = filter;
            mChildCheckValidateNodes = childCheckValidateNodes;
        }

        public bool Constraint(Type type)
        {
            return true;
        }

        public bool Validate(Type fieldType, string value)
        {
            if (mFilter == null || mChildCheckValidateNodes == null || mChildCheckValidateNodes.Count == 0)
            {
                return false;
            }

            // 不满足条件就是过滤的，所以直接返回true
            if (!mFilter.Validate(fieldType, value))
            {
                return true;
            }

            return mChildCheckValidateNodes[0].Validate(fieldType, value);
        }

        public string GetDesc()
        {
            if (mChildCheckValidateNodes != null && mChildCheckValidateNodes.Count != 0)
            {
                return string.Format("过滤的子结点:[{0}]({1})", mChildCheckValidateNodes[0].ToString(), mChildCheckValidateNodes[0].GetDesc());
            }
            return "过滤结点";
        }
    }
}
