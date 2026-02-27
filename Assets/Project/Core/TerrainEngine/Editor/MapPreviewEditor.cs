#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Archura.TerrainEngine.Preview.EditorTools
{
    [CustomEditor(typeof(MapPreview))]
    public class MapPreviewEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            MapPreview preview = (MapPreview)target;

            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            bool changed = EditorGUI.EndChangeCheck();

            EditorGUILayout.Space(6);

            GUI.backgroundColor = new Color(0.3f, 0.8f, 0.4f);
            if (GUILayout.Button("Generate Preview", GUILayout.Height(32)))
            {
                preview.DrawMapInEditor();
            }
            GUI.backgroundColor = Color.white;

            if (changed && preview.autoUpdate)
            {
                preview.DrawMapInEditor();
            }
        }
    }
}
#endif