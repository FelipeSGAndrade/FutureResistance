using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AutoTile))]
public class AutoTileEditor : Editor
{
    public override void OnInspectorGUI() {
        AutoTile autoTile = (AutoTile)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Update")) {
            autoTile.UpdateState();
        }
    }
}
