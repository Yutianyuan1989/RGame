/*****************************************************************************/
/****************** Auto Generate Script, Do Not Modify! *********************/
/*****************************************************************************/
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TableTaskTaskDialogContentScriptable))]
public class TableTaskTaskDialogContentScriptableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TableTaskTaskDialogContentScriptable script = (TableTaskTaskDialogContentScriptable)target;

        if (GUILayout.Button("Update"))
			script.LoadGameTable(true);

        GUILayout.Space(20);

        DrawDefaultInspector();
    }
}