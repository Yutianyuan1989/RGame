/*****************************************************************************/
/****************** Auto Generate Script, Do Not Modify! *********************/
/*****************************************************************************/
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TableTaskTaskDialogScriptable))]
public class TableTaskTaskDialogScriptableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TableTaskTaskDialogScriptable script = (TableTaskTaskDialogScriptable)target;

        if (GUILayout.Button("Update"))
			script.LoadGameTable(true);

        GUILayout.Space(20);

        DrawDefaultInspector();
    }
}