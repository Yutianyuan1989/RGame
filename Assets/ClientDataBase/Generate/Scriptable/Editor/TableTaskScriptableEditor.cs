/*****************************************************************************/
/****************** Auto Generate Script, Do Not Modify! *********************/
/*****************************************************************************/
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TableTaskScriptable))]
public class TableTaskScriptableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TableTaskScriptable script = (TableTaskScriptable)target;

        if (GUILayout.Button("Update"))
			script.LoadGameTable(true);

        GUILayout.Space(20);

        DrawDefaultInspector();
    }
}