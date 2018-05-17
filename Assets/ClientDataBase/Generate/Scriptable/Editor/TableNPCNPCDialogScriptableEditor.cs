/*****************************************************************************/
/****************** Auto Generate Script, Do Not Modify! *********************/
/*****************************************************************************/
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TableNPCNPCDialogScriptable))]
public class TableNPCNPCDialogScriptableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TableNPCNPCDialogScriptable script = (TableNPCNPCDialogScriptable)target;

        if (GUILayout.Button("Update"))
			script.LoadGameTable(true);

        GUILayout.Space(20);

        DrawDefaultInspector();
    }
}