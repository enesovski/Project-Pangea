using UnityEditor;
using Archura.TerrainEngine.Preview;
using UnityEngine;


namespace Archura.TerrainEngine.EditorTools
{
    [CustomEditor(typeof(MapPreview))]
    public class MapPreviewEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            MapPreview mapPreview = (MapPreview)target;

            if (DrawDefaultInspector()) 
            {
                if (mapPreview.autoUpdate)
                {
                    mapPreview.DrawMapInEditor();
                }
            }
            
            GUILayout.Space(10); 

            //Button
            if (GUILayout.Button("Generate Map"))
            {
                mapPreview.DrawMapInEditor();
            }            
        }
    }
}