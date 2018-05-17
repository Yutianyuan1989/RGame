namespace ClientDataBase
{
    using System;
    using System.IO;


    using UnityEngine;

    /// <summary>
    /// 验证文件是否存在
    /// </summary>
    public class FileExistValidate : ITableCheckValidateNodeBase
    {
        private readonly string rootDir;

        public FileExistValidate(string root)
        {
            rootDir = root;

            if (!string.IsNullOrEmpty(rootDir) && rootDir[rootDir.Length - 1] == '/')
            {
                rootDir += '/';
            }
        }

        public bool Constraint(Type type)
        {
            return type == typeof(string);
        }

        public bool Validate(Type fieldType, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            Debug.Assert(Constraint(fieldType), "Type Failed:" + GetDesc());

            // 有可能会没有配置后缀,这里就暂时先假定为prefab
            return File.Exists(rootDir + value) || File.Exists(rootDir + value + ".prefab");
        }

        public string GetDesc()
        {
            return "验证文件是否存在(string requested)";
        }
    }
}
