/**********************************************************
// Author   : K.(k79k06k02k)
// FileName : ClientDataBaseEditorWindow.cs
**********************************************************/
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ClientDataBase
{
	using System;

	using Assets.Scripts.Tool;

	using Object = UnityEngine.Object;

	public class ClientDataBaseEditorWindow : EditorWindow
    {
        //按下執行按鈕
        bool _boolExecuteButtonClick = false;
        float _floatWaitForExecute = 0.1f;

        //一次執行的開始
        bool _boolStartCreate = false;

        //是否是更新全部
        bool _boolUpdateAll = false;

        //等待 Application Compiling Script
        bool _boolNeedToAttach = false;
        float _floatWaitForCompile = 1;

        string[] _types = new string[] { "Create", "Update" };
        int _intTabIndex = 0;

        List<Object> _objList;
        int _intNowCount = 0;
        int _inttotalCount = 0;

        Vector2 _scrollPos;
        GUIStyle _btnStyle;

        private bool createScriptableAssets = false;
        private bool exportToCSharp = true;
        private bool exportToErlang = false;
        private bool exportToLua = false;

        const string kSimulationMode = "CSVClient DataBase/Simulation Mode";

        [MenuItem(kSimulationMode)]
        public static void ToggleSimulationMode()
        {
            ClientDataBaseManager.SimulateTableAssetInEditor = !ClientDataBaseManager.SimulateTableAssetInEditor;
        }

        [MenuItem(kSimulationMode, true)]
        public static bool ToggleSimulationModeValidate()
        {
            Menu.SetChecked(kSimulationMode, ClientDataBaseManager.SimulateTableAssetInEditor);
            return true;
        }

        [MenuItem("CSVClient DataBase/Window %#&c", false, 110)]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = EditorWindow.GetWindow(typeof(ClientDataBaseEditorWindow));
            editorWindow.position = new Rect(editorWindow.position.xMin + 100f, editorWindow.position.yMin + 100f, 600f, 400f);
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.Show();
            editorWindow.titleContent = new GUIContent("Client DataBase Tool");
        }

        [MenuItem("CSVClient DataBase/Update All", false, 110)]
        public static void UpdateAll()
        {
            ClientDataBaseEditorWindow window = EditorWindow.GetWindow<ClientDataBaseEditorWindow>();
            window._objList = UtilityEditor.LoadAllAssetsAtPath(ClientDataBaseManager.Instance.m_config.GetGameTablePath()).ToList();

            if (window._objList.Count == 0)
            {
                Debug.Log("No GameTable file (.csv)");
                window.Close();
                return;
            }

            window._boolUpdateAll = true;
            window._boolStartCreate = true;
            window._boolExecuteButtonClick = true;
        }


        void Update()
        {
            //等待 Application Compiling Script，建立 ScriptableObject Asset
            if (_boolNeedToAttach)
            {
                _floatWaitForCompile -= 0.01f;

                if (_floatWaitForCompile <= 0)
                {
                    if (!EditorApplication.isCompiling)
                    {
                        foreach (Object go in _objList)
                        {
                            string path = AssetDatabase.GetAssetPath(go);
                            string fileName = Path.GetFileNameWithoutExtension(path);
                            string scriptableScriptName = ClientDataBaseManager.Instance.m_config.GetScriptableScriptName(fileName, true);
                            string scriptableAssetName = ClientDataBaseManager.Instance.m_config.GetScriptableAssetName(fileName, true);

                            _intNowCount++;
                            UpdateProgressBar("Generate Scriptable Assets", string.Format("[File Name:{0}]", scriptableAssetName));
                            
                            if (createScriptableAssets)
                            {
                                if (ClientDataBaseParse.Instance.CreateScriptableAssets(scriptableScriptName, scriptableAssetName) == false)
                                    continue;
                            }
                            
                        }

                        _boolStartCreate = false;
                        _boolNeedToAttach = false;
                        _floatWaitForCompile = 1;
                        EditorUtility.ClearProgressBar();

                        if (_boolUpdateAll)
                            this.Close();
                    }
                }
            }

            //點下按鈕後，延遲執行，先讓 Loading 畫面出來
            if (_boolExecuteButtonClick)
            {
                _floatWaitForExecute -= 0.01f;
                if (_floatWaitForExecute <= 0)
                {
                    Execute();

                    _boolExecuteButtonClick = false;
                    _floatWaitForExecute = 0.1f;
                }
            }
        }

        void OnSelectionChange()
        {
            Repaint();
        }

        void OnGUI()
        {
            _btnStyle = new GUIStyle(GUI.skin.button);
            _btnStyle.fontSize = 20;
            _btnStyle.alignment = TextAnchor.MiddleLeft;
            GUIStyle myStyle = GUI.skin.GetStyle("HelpBox");
            myStyle.fontSize = 16;

            //遮罩
            if (_boolStartCreate)
                GUI.enabled = false;


            _intTabIndex = UtilityEditor.Tabs(_types, _intTabIndex);
            GUILayout.Space(10);

            //還沒開始時才需要抓物件
            if (_boolStartCreate == false && _boolUpdateAll == false)
                _objList = Selection.objects.ToList();


            //排除
            for (int i = _objList.Count - 1; i >= 0; i--)
            {
                if (GetFiltered(_objList[i]))
                    _objList.Remove(_objList[i]);
            }

            if (GUILayout.Button("清除生成目录的只读属性"))
            {
                FileUtils.ClearReadOnly(ClientDataBaseManager.Instance.m_config.m_root + "/Generate");
                FileUtils.ClearReadOnly(ClientDataBaseManager.Instance.m_config.GetScriptableAssetPath());
                FileUtils.ClearReadOnly(Application.dataPath + "/../../Server/apps/config");
                FileUtils.ClearReadOnly(Application.dataPath + "/LuaFramework/Lua/Table");
            }

            createScriptableAssets = EditorGUILayout.ToggleLeft("是否生成Script Assets文件(发布版本时,可以不用生成，版本机会出)", createScriptableAssets);
            exportToCSharp = EditorGUILayout.ToggleLeft("是否生成C#读取程序", exportToCSharp);
            exportToErlang = EditorGUILayout.ToggleLeft("是否导出服务器表格", exportToErlang);
            exportToLua = EditorGUILayout.ToggleLeft("是否导出lua表格", exportToLua);

        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Choose GameTable Asset", EditorStyles.boldLabel, GUILayout.Width(200));
            EditorGUILayout.LabelField("Count : " + _objList.Count, EditorStyles.boldLabel, GUILayout.Width(100));
            string btnTip = _intTabIndex == 0 ? "创建所有": "更新所有表格到ScriptableObject asset";
            if (GUILayout.Button(btnTip))
            {
                UpdateAll();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (_objList.Count == 0)
            {
                EditorGUILayout.HelpBox(GetHelpString(), MessageType.Warning);
                return;
            }

            //字母排序
            _objList.Sort(delegate (Object a, Object b)
            {
                return a.name.CompareTo(b.name);
            });

            EditorGUILayout.BeginVertical();
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);
            EditorGUILayout.Space();

            foreach (Object go in _objList)
            {
                if (GUILayout.Button(go.name, _btnStyle))
                    EditorGUIUtility.PingObject(go);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            if (UtilityEditor.GetCommonButton(_types[_intTabIndex]))
            {
                if (EditorApplication.isCompiling)
                {
                    Debug.LogError("After wait application compiling then try again.");
                    return;
                }

                _boolStartCreate = true;
                _boolExecuteButtonClick = true;
            }

            if (_boolStartCreate)
            {
                GUI.enabled = true;
                UtilityEditor.ShowLoading();
            }

        }

        void OnDestroy()
        {
            if (_boolNeedToAttach)
                Debug.LogError("Please wait complete, Or may cause a crash...");

            EditorUtility.ClearProgressBar();
        }

        void Execute()
        {
            switch (_intTabIndex)
            {
                case 0:
                    CreateAsset();
                    break;

                case 1:
                    UpdateAsset();
                    break;
            }
        }

        void CreateAsset()
        {
            //乘2是因為把 Script 與 Scriptable Asset 分開處理，必須先等 Application Compiling 完，才找的到 Scriptable Class，最後才能透過 Class 建立 Scriptable Asset 
            //加1是等 Application Compiling 的區間
            _inttotalCount = _objList.Count * 2 + 1;

            _intNowCount = 0;

            foreach (Object go in _objList)
            {
                string path = AssetDatabase.GetAssetPath(go);

                _intNowCount++;

                string fileName = Path.GetFileName(path);

                UpdateProgressBar("Generate GameTable Script", string.Format("[File Name:{0}]", fileName));

                try
                {
                    if (ClientDataBaseParse.Instance.LoadGameTable(go, exportToCSharp, exportToErlang, exportToLua) == false)
                        continue;
                }
                catch (Exception e)
                {
                    Debug.LogError("table:" + fileName +" Exception:"+e);
                }

            }

            _intNowCount++;
            UpdateProgressBar("Please Wait", "Wait Application Compiling....");
            _boolNeedToAttach = true;
        }

        void UpdateAsset()
        {
            foreach (Object go in _objList)
            {
                ScriptableObjectBase script = (ScriptableObjectBase)go;

                if (script.LoadGameTable(true) == false)
                    continue;
            }
            _boolStartCreate = false;
        }


        bool GetFiltered(Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            string extension = Path.GetExtension(path);

            switch (_intTabIndex)
            {
                case 0:
		            {
			            string checkStr = ClientDataBaseManager.Instance.m_config.m_gameTableCheck;

						bool needCheckTable = !string.IsNullOrEmpty(checkStr);

						return string.Compare(extension, ClientDataBaseManager.Instance.m_config.m_extensionCsv, StringComparison.OrdinalIgnoreCase) != 0
						  || (needCheckTable && ((TextAsset)obj).ToString().StartsWith(checkStr) == false);
					}
                   

                case 1:
                    return extension != ClientDataBaseManager.Instance.m_config.m_extensionAsset || obj.name == ClientDataBaseManager.Instance.m_config.name;

                default:
                    return true;
            }
        }


        string GetHelpString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("没有选择csv文件.." + "\n\n");
            sb.Append("请选择csv文件：" + "\n");

            switch (_intTabIndex)
            {
                case 0:
                    sb.Append("1.资源必须在工程里" + "\n");
                    sb.Append("2.资源的扩展名必须为: [" + ClientDataBaseManager.Instance.m_config.m_extensionCsv + "]" + "\n");
                    sb.Append("3.资源内容必须为表格");
                    break;

                case 1:
                    sb.Append("1.资源必须在工程里" + "\n");
                    sb.Append("2.资源内容必须为 ScriptableObject Asset");
                    break;

                default:
                    break;
            }

            return sb.ToString();
        }


        void UpdateProgressBar(string title, string info)
        {
            float process = _intNowCount / (float)_inttotalCount;
            EditorUtility.DisplayProgressBar(title, string.Format("[{0}%] {1}", (int)(process * 100), info), process);
        }
    }
}