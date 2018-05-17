/*****************************************************************************/
/****************** Auto Generate Script, Do Not Modify! *********************/
/*****************************************************************************/
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TableTaskTaskCollectScriptable))]
public class TableTaskTaskCollectScriptableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TableTaskTaskCollectScriptable script = (TableTaskTaskCollectScriptable)target;

        if (GUILayout.Button("Update"))
			script.LoadGameTable(true);

        GUILayout.Space(20);

        DrawDefaultInspector();
    }
}