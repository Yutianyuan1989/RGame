/**********************************************************
// Author   : K.(k79k06k02k)
// FileName : UtilityEditor.cs
**********************************************************/
using UnityEngine;
using UnityEditor;

namespace ClientDataBase
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Assets.Scripts.Tool;

    using Object = UnityEngine.Object;

    public class UtilityEditor
    {
        /// <summary>
        /// 建立資料夾，自動建子資料夾 
        /// EX: GameResources/Prefabs/Sprites/Enemy
        /// </summary>
        /// <param name="name">路徑層級，從 Assets 下一層資料夾開始</param>
        /// <returns>資料夾是否存在</returns>
        public static bool CreateFolder(string name)
        {
            //路徑字串開頭如果是 Assets，進行剃除
            if (name.StartsWith("Assets/")) name = name.Replace("Assets/", "");

            //路徑字串最後如果有 / ，進行剃除
            if (name[name.Length - 1] == '/') name = name.Substring(0, name.Length - 1);


            if (System.IO.Directory.Exists(Application.dataPath + "/" + name))
            {
                //Debug.Log("Folder [ Assets/" + name + " ] is Exist!!");
                return false;
            }
            else
            {
                System.IO.Directory.CreateDirectory(Application.dataPath + "/" + name);
                AssetDatabase.Refresh();

                //Debug.Log("Folder [ Assets/" + name + " ] is Create!!");
                return true;
            }
        }

        /// <summary>
        /// 通用按鈕
        /// </summary>
        /// <param name="btnName">按鈕名稱</param>
        /// <returns>是否按下</returns>
        public static bool GetCommonButton(string btnName)
        {
            GUIStyle BtnStyle = new GUIStyle(GUI.skin.button);
            BtnStyle.fontSize = 25;
            BtnStyle.fixedHeight = 50;

            return GUILayout.Button(btnName, BtnStyle);
        }


        /// <summary>
        /// 橫向頁籤
        /// </summary>
        /// <param name="options">名稱</param>
        /// <param name="selected">選擇的index</param>
        /// <returns>選擇的index</returns>
        public static int Tabs(string[] options, int selected)
        {
            const float DarkGray = 0.6f;
            const float LightGray = 0.9f;
            const float StartSpace = 10;

            GUILayout.Space(StartSpace);
            Color storeColor = GUI.backgroundColor;
            Color highlightCol = new Color(LightGray, LightGray, LightGray);
            Color bgCol = new Color(DarkGray, DarkGray, DarkGray);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.padding.bottom = 8;

            GUILayout.BeginHorizontal();
            {
                for (int i = 0; i < options.Length; ++i)
                {
                    GUI.backgroundColor = i == selected ? highlightCol : bgCol;
                    if (GUILayout.Button(options[i], buttonStyle))
                    {
                        selected = i;
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUI.backgroundColor = storeColor;

            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, highlightCol);
            texture.Apply();
            GUI.DrawTexture(
                new Rect(
                    0,
                    buttonStyle.lineHeight + buttonStyle.border.top + buttonStyle.margin.top + StartSpace,
                    Screen.width,
                    4),
                texture);

            return selected;
        }

        /// <summary>
        /// 顯示Loading畫面
        /// </summary>
        public static void ShowLoading()
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, new Color(0, 0, 0, 0.6f));
            texture.Apply();
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);

            GUIStyle LableStyle = new GUIStyle(GUI.skin.label);
            LableStyle.fontSize = 40;
            LableStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Loading...", LableStyle);
        }

        /// <summary>
        /// 讀取路徑下所有資料
        /// </summary>
        /// <param name="path">路徑</param>
        public static Object[] LoadAllAssetsAtPath(string path)
        {
            if (path.EndsWith("/"))
            {
                path = path.TrimEnd('/');
            }
            string[] GUIDs = AssetDatabase.FindAssets("", new string[] { path });
            Object[] objectList = new Object[GUIDs.Length];
            for (int index = 0; index < GUIDs.Length; index++)
            {
                string guid = GUIDs[index];
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)) as Object;
                objectList[index] = asset;
            }

            return objectList;
        }

        /// <summary>
        /// 把csv的内容保存为asset格式的
        /// </summary>
        /// <param name="csvFileName"></param>
        /// <param name="csvContent"></param>
        /// <returns></returns>
        public static ScriptableObjectBase SaveScriptableAsset(string csvFileName, string csvContent)
        {
            ScriptableObjectBase obj = CreateScriptableAsset(csvFileName);
            if (obj == null)
            {
                return null;
            }
            if (!obj.LoadGameTable(csvContent))
            {
                return null;
            }

            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();

            return obj;
        }

        /// <summary>
        /// 删除目录下的所有资源
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        private static bool DeleteAssetDirectory(string assetPath)
        {
            // 自分をDeleteAssetで消すのを試してみる
            // これに失敗したら、個別に消す
            if (AssetDatabase.DeleteAsset(assetPath))
            {
                Debug.Log("Delete Asset: " + assetPath);
                return true;
            }

            string[] dirpathlist = Directory.GetDirectories(assetPath);

            // ディレクトリをひとつづつ消す
            foreach (string path in dirpathlist)
            {
                if (false == DeleteAssetDirectory(path))
                {
                    Debug.LogError("Delete Asset Directory Error: " + path);
                    return false;
                }
            }

            // ファイルをひとつづつ消す
            string[] filepathlist = Directory.GetFiles(assetPath);
            foreach (string path in filepathlist)
            {
                // metaファイルは避ける
                if (path.EndsWith(".meta"))
                {
                    continue;
                }

                if (false == AssetDatabase.DeleteAsset(path))
                {
                    Debug.LogError("Delete Asset Files Error: " + path);
                    return false;
                }
            }

            Debug.Log("Delete Asset: " + assetPath);
            return true;
        }

        public static int UpdateAllScriptableAsset()
        {
            Debug.LogFormat("start UpdateAllScriptableAsset-------------");

            string assetPath = ClientDataBaseManager.Instance.m_config.GetScriptableAssetPath();
            FileUtils.ClearReadOnly(assetPath);
            //DeleteAssetDirectory(assetPath);
            // 这里就不删除了,不然会把assetList的也删除了
            //FileUtils.DelDir(assetPath);

            var objList = LoadAllAssetsAtPath(ClientDataBaseManager.Instance.m_config.GetGameTablePath()).ToList();

            if (objList.Count == 0)
            {
                Debug.Log("No GameTable file (.csv)");
                return 0;
            }
            int count = 0;
            foreach (Object go in objList)
            {
                string path = AssetDatabase.GetAssetPath(go);
                if (string.Compare(
                        Path.GetExtension(path),
                        ClientDataBaseManager.Instance.m_config.m_extensionCsv,
                        StringComparison.OrdinalIgnoreCase) != 0)
                {
                    continue;
                }
                string fileName = Path.GetFileNameWithoutExtension(path);
                string scriptableScriptName =
                    ClientDataBaseManager.Instance.m_config.GetScriptableScriptName(fileName, true);
                string scriptableAssetName =
                    ClientDataBaseManager.Instance.m_config.GetScriptableAssetName(fileName, true);

                count++;
                UpdateProgressBar(
                    "Generate Scriptable Assets",
                    string.Format("[File Name:{0}]", scriptableAssetName),
                    count / (float)objList.Count);

                if (ClientDataBaseParse.Instance.CreateScriptableAssets(scriptableScriptName, scriptableAssetName))
                {
                    Debug.LogFormat("Update Asset table (.csv) name :{0} index:{1}", path, count + 1);

                }
            }
            EditorUtility.ClearProgressBar();

            return count;
        }

        public static void UpdateProgressBar(string title, string info, float process)
        {
            EditorUtility.DisplayProgressBar(title, string.Format("[{0}%] {1}", (int)(process * 100), info), process);
        }

        /// <summary>
        /// 创建对应目录的asset
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static ScriptableObjectBase CreateScriptableAsset(string fileName)
        {
            string scriptableScriptName =
                ClientDataBaseManager.Instance.m_config.GetScriptableScriptName(fileName, true);
            string scriptableAssetName = ClientDataBaseManager.Instance.m_config.GetScriptableAssetName(fileName, true);

            return CreateScriptableAsset(scriptableScriptName, scriptableAssetName);
        }

        public static ScriptableObjectBase CreateScriptableAsset(
            string scriptableScriptName,
            string scriptableAssetName)
        {
            var config = ClientDataBaseManager.Instance.m_config;

            MonoScript script =
                AssetDatabase.LoadAssetAtPath<MonoScript>(config.GetScriptableScriptsPath() + scriptableScriptName);

            if (script == null || script.GetClass() == null)
            {
                Debug.LogError(
                    string.Format(
                        "Scriptable Script is Null. [Path:{0}]",
                        config.GetScriptableScriptsPath() + scriptableScriptName));
                return null;
            }

            string path = config.GetScriptableAssetPath() + scriptableAssetName;
            CreateFolder(config.GetScriptableAssetPath());

            Object obj = ScriptableObject.CreateInstance(script.GetClass());

            AssetDatabase.CreateAsset(obj, path);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            Debug.Log(
                string.Format(
                    "[Scriptable Asset] is Create.\nFile:[{0}] Path:[{1}]",
                    scriptableAssetName,
                    config.GetScriptableAssetPath()));

            //資料讀取
            ScriptableObjectBase scriptableObjectBase = AssetDatabase.LoadAssetAtPath<ScriptableObjectBase>(path);

            return scriptableObjectBase;
        }

        /// <summary>
        /// 目录merge
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        static void DirectoryMerge(string sourceDirName, string destDirName)
        {
            if (!Directory.Exists(sourceDirName)) return;

            const string title = "Hold On";

            string info = "Delete files from dest not in source";
            float progress = 0;

            if (Directory.Exists(destDirName))
            {
                var destFiles = Directory.GetFiles(destDirName, "*.*", SearchOption.AllDirectories);
                foreach (var file in destFiles)
                {
                    if (!File.Exists(sourceDirName + file.Remove(0, destDirName.Length))) File.Delete(file);

                    progress += 1.0f / destFiles.Length;
                    EditorUtility.DisplayProgressBar(title, info, progress);
                }
            }

            info = "Copy changed and new files from source to dest";
            progress = 0;

            // Then copy changed and new files from source to dest
            var sourceFiles = Directory.GetFiles(sourceDirName, "*.*", SearchOption.AllDirectories);
            foreach (var file in sourceFiles)
            {
                var destFile = destDirName + file.Remove(0, sourceDirName.Length);

                progress += 1.0f / sourceFiles.Length;
                EditorUtility.DisplayProgressBar(title, info, progress);

                if (File.Exists(destFile) && File.GetLastWriteTimeUtc(file) == File.GetLastWriteTimeUtc(destFile)
                    && new FileInfo(file).Length == new FileInfo(destFile).Length) continue;

                Directory.CreateDirectory(Path.GetDirectoryName(destFile));

                File.Copy(file, destFile, true);
                File.SetCreationTime(destFile, File.GetCreationTime(file));
                File.SetLastAccessTime(destFile, File.GetLastAccessTime(file));
                File.SetLastWriteTime(destFile, File.GetLastWriteTime(file));
            }

            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 构建assetbundle
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <param name="assetbundles"></param>
        /// <param name="outputPath"></param>
        public static void BuildAssetBundle(BuildTarget buildTarget, AssetBundleBuild[] assetbundles, string outputPath)
        {
            if (assetbundles.Length > 0)
            {
                BuildPipeline.BuildAssetBundles(
                    outputPath,
                    assetbundles,
                    GetBuildAssetBundleOptions(),
                    buildTarget);
            }
            else
            {
                UnityEngine.Debug.LogError("There is no Assets use assetBundleName");
            }
        }

        public static void BuildAssetBundle(
            BuildTarget buildTarget,
            string assetBundleName,
            string assetPath,
            string outputPath)
        {
            List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
            AssetBundleBuild build = new AssetBundleBuild();

            build.assetBundleName = assetBundleName;
            build.assetBundleVariant = string.Empty;
            build.assetNames = new[] { assetPath };

            assetBundleBuilds.Add(build);

            BuildAssetBundle(buildTarget, assetBundleBuilds.ToArray(), outputPath);
        }

        /// <summary>
        /// DeterministicAssetBundle 将保证AssetBundle使用唯一Hash进行标识，若不加这个参数AssetBundle每次构建时都生成不同ID， 这样lua每次打出来的脚本md5总是变
        /// </summary>
        /// <returns></returns>
        public static BuildAssetBundleOptions GetBuildAssetBundleOptions()
        {
            BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle |
                                              //BuildAssetBundleOptions.AppendHashToAssetBundleName |
                                              BuildAssetBundleOptions.ChunkBasedCompression;

            return options;
        }
    }
}

