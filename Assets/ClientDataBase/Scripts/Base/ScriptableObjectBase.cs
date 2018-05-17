/**********************************************************
// Author   : K.(k79k06k02k)
// FileName : ScriptableObjectBase.cs
**********************************************************/
namespace ClientDataBase
{
    public abstract class ScriptableObjectBase : UnityEngine.ScriptableObject
    {
        public abstract bool LoadGameTable(bool bSaveToAsset);

        public abstract string GetTableName();

        public abstract bool LoadGameTable(string data);

        public abstract int Count { get; }

        /// <summary>
        /// 进行表格验证
        /// </summary>
        public abstract void Validate();

        /// <summary>
        /// 该字段是否包含该值
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="checkVal"></param>
        /// <returns></returns>
        public abstract bool CheckField(string fieldName, string checkVal);
    }
}

