using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrefabInstantiator))]
public class PrefabInstantiatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PrefabInstantiator instantiator = target as PrefabInstantiator;
        if (GUILayout.Button("Create"))
        {
            instantiator.CreatePrefab();
        }
    }
}
