/**********************************************************
// Author   : K.(k79k06k02k)
// FileName : Utility.cs
**********************************************************/
using UnityEngine;

namespace ClientDataBase
{
	using System;
	using System.Linq;

    public class Utility
    {
        public struct TypeRelate
        {
            public static bool StringToBool(string value)
            {
                if (string.CompareOrdinal(value.ToUpper(), "T") == 0
                    || string.CompareOrdinal(value.ToUpper(), "TRUE") == 0
                    || string.CompareOrdinal(value.ToUpper(), "YES") == 0)
                {
                    return true;
                }
                else if (string.CompareOrdinal(value.ToUpper(), "F") == 0
                         || string.CompareOrdinal(value.ToUpper(), "FALSE") == 0
                         || string.CompareOrdinal(value.ToUpper(), "NO") == 0)
                {
                    return false;
                }
                else
                {
                    Debug.LogError(string.Format("Unable to convert value:[{0}]", value));
                    return false;
                }
            }

            public static T StringToCommonType<T>(string value)
            {
                return string.IsNullOrEmpty(value) ? default(T) : (T) Convert.ChangeType(value, typeof(T));
            }

            public static Vector2 StringToVector2(string value)
            {
                string[] str = value.Split(',');
                return new Vector2(float.Parse(str[0]), float.Parse(str[1]));
            }
            public static Vector3 StringToVector3(string value)
            {
                string[] str = value.Split(',');
                return new Vector3(float.Parse(str[0]), float.Parse(str[1]), float.Parse(str[2]));
            }

            public static string CorrectFieldName(string fieldNameInTable, string separate="")
            {
                string[] trimedStr = fieldNameInTable.Split(' ').Select(s => s.Trim()).ToArray();
                return string.Join(separate, trimedStr);
            }
        }


        public struct AssetRelate
        {
            /// <summary>
            /// Resources.Load
            /// </summary>
            public static T ResourcesLoadCheckNull<T>(string name) where T : UnityEngine.Object
            {
                T loadGo = Resources.Load<T>(name);

                if (loadGo == null)
                {
                    Debug.LogError("ResourcesLoadCheckNull [ " + name + " ] is Null !!");
                    return default(T);
                }

                return loadGo;
            }
        }
    }
}