// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileUtils.cs" company="Lilith">
//   Jacky
// </copyright>
// <summary>
//   Defines the FileUtils type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Assets.Scripts.Tool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;

    public class FileUtils
    {
        /// <summary>
        /// 检索一个目录下所有指定后缀名的文件，输出文件和路径
        /// </summary>
        /// <param name="_ext">指定后缀</param>
        /// <param name="_path">指定目录</param>
        /// <param name="_outList">指定文件列表</param>
        /// <param name="_paths">？</param>
        public static void FilesWithExt(
            string _ext,
            string _path, List<string> _outList
            , List<string> _paths)
        {
            string[] names = Directory.GetFiles(_path);
            string[] dirs = Directory.GetDirectories(_path);
            foreach (string filename in names)
            {
                string ext = Path.GetExtension(filename);
                //策划写了一个大写的csv
                if (string.Compare(ext, _ext, StringComparison.OrdinalIgnoreCase) != 0
                    && !string.IsNullOrEmpty(_ext))
                {
                    continue;
                }
                _outList.Add(filename.Replace('\\', '/'));
            }

            foreach (string dir in dirs)
            {
                _paths.Add(dir.Replace('\\', '/'));
                FilesWithExt(_ext, dir, _outList, _paths);
            }
        }

        public static void FilesWithoutExt(string _ext, string _path, List<string> _outList, List<string> _paths)
        {
            string[] names = Directory.GetFiles(_path);
            string[] dirs = Directory.GetDirectories(_path);
            foreach (string filename in names)
            {
                string ext = Path.GetExtension(filename);
                if (ext.Equals(_ext)
                    && !string.IsNullOrEmpty(_ext))
                {
                    continue;
                }
                _outList.Add(filename.Replace('\\', '/'));
            }

            foreach (string dir in dirs)
            {
                _paths.Add(dir.Replace('\\', '/'));
                FilesWithoutExt(_ext, dir, _outList, _paths);
            }
        }

        public static void FilesWithoutExt(List<string> extList, string path, List<string> outList, List<string> paths)
        {
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string filename in names)
            {
                string ext = Path.GetExtension(filename);

                bool isExt = false;

                List<string>.Enumerator v = extList.GetEnumerator();
                while (v.MoveNext())
                {
                    if (ext.Equals(v.Current))
                    {
                        isExt = true;
                        continue;
                    }
                }

                if (isExt)
                    continue;

                outList.Add(filename.Replace('\\', '/'));
            }

            foreach (string dir in dirs)
            {
                paths.Add(dir.Replace('\\', '/'));
                FilesWithoutExt(extList, dir, outList, paths);
            }
        }

        /// <summary>
        /// 在指定路径，下创建一个指定文件名的文件，把内容写进去
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public static void SaveFile(string path, string name, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            string localFile = path + "/" + name;
            FileInfo info = new FileInfo(localFile);
            if (info.Exists)
            {
                File.Delete(localFile);
            }
            StreamWriter sw = info.CreateText();
            sw.WriteLine(content);
            sw.Close();
            sw.Dispose();
        }

        public static void DelFile(string _path)
        {
            if (File.Exists(_path))
            {
                SetAttrNormal(_path);

                File.Delete(_path);
            }
        }

        public static void DelU3dFile(string _path)
        {
            SetAttrNormal(_path);

            File.Delete(_path);

            SetAttrNormal(_path + ".meta");
            File.Delete(_path + ".meta");
        }

        public static void SetAttrNormal(string _path)
        {
            if (File.GetAttributes(_path) != FileAttributes.Normal)
            {
                File.SetAttributes(_path, FileAttributes.Normal);
            }
        }

        /// <summary>
        /// 删除一个目录
        /// </summary>
        /// <param name="_path"></param>
        public static void DelDir(string _path)
        {
            DelDir(new DirectoryInfo(_path));
        }

        /// <summary>
        /// 删除一个目录
        /// </summary>
        /// <param name="_dirInfo"></param>
        public static void DelDir(DirectoryInfo _dirInfo)
        {
	        try
	        {
		        foreach (DirectoryInfo kNewInfo in _dirInfo.GetDirectories())
		        {
			        DelDir(kNewInfo);
		        }
		        foreach (FileInfo kNewInfo in _dirInfo.GetFiles())
		        {
			        kNewInfo.Attributes = kNewInfo.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
			        kNewInfo.Delete();
		        }
		        _dirInfo.Attributes = _dirInfo.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
		        _dirInfo.Delete();
			}
	        catch (Exception e)
	        {
		        Debug.LogError("DelDir is error: " + _dirInfo.Name + "" + e.Message);
	        }
        }

        /// <summary>
        /// 删除一个目录
        /// </summary>
        /// <param name="_name"></param>
        public static void RemoveDir(string _name)
        {
            if (Directory.Exists(_name))
            {
                DelDir(_name);
            }

            //AssetDatabase.Refresh();
        }

        /// <summary>
        /// 删除一个目录并创建这个目录
        /// </summary>
        /// <param name="_name"></param>
        public static void ClearDir(string _name)
        {
            Debug.Log("ClearDir:" + _name);

            if (Directory.Exists(_name))
            {
                DelDir(_name);
            }

            Directory.CreateDirectory(_name);

            //AssetDatabase.Refresh();
        }

        public static string RemoveFileExt(string _ab)
        {
            int index = _ab.IndexOf(".");

            if (index > -1)
            {
                return _ab.Substring(0, index);
            }
            else
            {
                return _ab;
            }
        }

        public static void CopyFilesTo(string _ext, string _inputPath, string _outputPath)
        {
            List<string> paths = new List<string>();
            List<string> files = new List<string>();

            FilesWithExt(_ext, _inputPath, files, paths);

            //int n = 0;
            foreach (string f in files)
            {
                if (f.EndsWith(".meta")) continue;
                string newFile = f.Replace(_inputPath, "");
                string newPath = _outputPath + newFile;

                string path = Path.GetDirectoryName(newPath);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (File.Exists(newPath))
                {
                    File.Delete(newPath);
                }

                File.Copy(f, newPath, true);
            }

            //AssetDatabase.Refresh();
        }

        public static void CopyFileTo(string _inputFile, string _outputFile)
        {
            File.Copy(_inputFile, _outputFile);
        }

        public static void CutFileTo(string _inputFile, string _outputFile)
        {
            File.Copy(_inputFile, _outputFile);

            DelFile(_inputFile);
        }

        public static void CutAssetBundleTo(string _inputFile, string _outputFile)
        {
            File.Copy(_inputFile, _outputFile);

            DelAssetBundle(_inputFile);
        }

        public static string ReadFile(string _path)
        {
            var utf8 = new UTF8Encoding(true);

            StreamReader sr = new StreamReader(_path, utf8);
            string text = sr.ReadToEnd();
            sr.Close();

            return text;
        }

        /// <summary>
        /// 把路径过滤只保留文件名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileName(string path)
        {
            int index = path.LastIndexOf("/", StringComparison.Ordinal);

            if (index > -1)
            {
                return path.Substring(index + 1);
            }
            else
            {
                return path;
            }
        }

        public static string GetFileExt(string name)
        {
            var index = name.LastIndexOf(".", StringComparison.Ordinal);

            return index > -1 ? name.Substring(index + 1) : name;
        }

        public static void DelAssetBundle(string name)
        {
            DelFile(name + ".meta");
            DelFile(name + ".manifest.meta");
            DelFile(name);
            DelFile(name + ".manifest");
        }

        /// <summary>
        /// 清理目录的只读属性
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void ClearReadOnly(string directoryPath)
        {
            DirectoryInfo parentDirectory = new DirectoryInfo(directoryPath);
            ClearReadOnly(parentDirectory);
        }

        public static void ClearReadOnly(DirectoryInfo parentDirectory)
        {
            if (parentDirectory != null && parentDirectory.Exists)
            {
                parentDirectory.Attributes = FileAttributes.Normal;
                foreach (FileInfo fi in parentDirectory.GetFiles())
                {
                    fi.Attributes = FileAttributes.Normal;
                }
                foreach (DirectoryInfo di in parentDirectory.GetDirectories())
                {
                    ClearReadOnly(di);
                }
            }
            else if(parentDirectory != null)
            {
                Debug.LogError("could not found dir path:" + parentDirectory.Name);
            }
        }
    }
}
