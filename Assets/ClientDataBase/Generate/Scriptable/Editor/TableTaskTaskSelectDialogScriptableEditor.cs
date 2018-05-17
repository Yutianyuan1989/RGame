/*****************************************************************************/
/****************** Auto Generate Script, Do Not Modify! *********************/
/*****************************************************************************/
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TableTaskTaskSelectDialogScriptable))]
public class TableTaskTaskSelectDialogScriptableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TableTaskTaskSelectDialogScriptable script = (TableTaskTaskSelectDialogScriptable)target;

        if (GUILayout.Button("Update"))
			script.LoadGameTable(true);

        GUILayout.Space(20);

        DrawDefaultInspector();
    }
}