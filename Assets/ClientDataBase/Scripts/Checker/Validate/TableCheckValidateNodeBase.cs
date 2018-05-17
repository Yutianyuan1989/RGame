using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientDataBase
{
    using System;

    public interface ITableCheckValidateNodeBase
    {
        /// <summary>
        /// 该字段的类型约束
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool Constraint(Type type);


        /// <summary>
        /// 该字段值的验证
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="fieldVaule"></param>
        /// <returns></returns>
        bool Validate(Type fieldType, string fieldVaule);

        /// <summary>
        /// 该验证相关的描述
        /// </summary>
        /// <returns></returns>
        string GetDesc();
    }
}

