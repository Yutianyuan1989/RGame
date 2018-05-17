/*****************************************************************************/
/****************** Auto Generate Script, Do Not Modify! *********************/
/*****************************************************************************/
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TablePanelScriptable))]
public class TablePanelScriptableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TablePanelScriptable script = (TablePanelScriptable)target;

        if (GUILayout.Button("Update"))
			script.LoadGameTable(true);

        GUILayout.Space(20);

        DrawDefaultInspector();
    }
}