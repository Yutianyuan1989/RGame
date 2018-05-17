namespace ClientDataBase
{
    using System;
    using System.IO;

    using UnityEngine;

    /// <summary>
    /// 目录是否存在
    /// </summary>
    public class DirectoryExistValidate : ITableCheckValidateNodeBase
    {
        private readonly string rootDir;

        public DirectoryExistValidate(string root)
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
                return false;
            }

            Debug.Assert(Constraint(fieldType), "Type Failed:" + GetDesc());

            return Directory.Exists(value) || Directory.Exists(rootDir + value);
        }

        public string GetDesc()
        {
            return "验证目录是否存在(string requested)";
        }
    }
}
