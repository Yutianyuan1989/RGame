/*****************************************************************************/
/****************** Auto Generate Script, Do Not Modify! *********************/
/*****************************************************************************/
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TableNPCScriptable))]
public class TableNPCScriptableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TableNPCScriptable script = (TableNPCScriptable)target;

        if (GUILayout.Button("Update"))
			script.LoadGameTable(true);

        GUILayout.Space(20);

        DrawDefaultInspector();
    }
}