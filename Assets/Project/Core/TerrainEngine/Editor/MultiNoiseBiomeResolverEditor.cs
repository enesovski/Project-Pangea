#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Archura.TerrainEngine.Climate;

namespace Archura.TerrainEngine.BiomeModule.EditorTools
{

    [CustomEditor(typeof(MultiNoiseBiomeResolver))]
    public class MultiNoiseBiomeResolverEditor : Editor
    {
        private enum PreviewAxis { Temperature, Moisture, Continentalness, Erosion, Weirdness }

        private PreviewAxis _xAxis = PreviewAxis.Temperature;
        private PreviewAxis _yAxis = PreviewAxis.Moisture;

        private float _fixedTemp  = 0.5f;
        private float _fixedMoist = 0.5f;
        private float _fixedCont  = 0.5f;
        private float _fixedEros  = 0.5f;
        private float _fixedWeird = 0f;

        private Texture2D _previewTex;
        private bool _previewDirty = true;
        private bool _showPreview  = true;
        private int  _previewRes   = 96;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MultiNoiseBiomeResolver resolver = (MultiNoiseBiomeResolver)target;

            // Validation
            if (resolver.Validate(out string error) == false)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.HelpBox(error, MessageType.Error);
            }

            EditorGUILayout.Space(8);
            _showPreview = EditorGUILayout.Foldout(_showPreview, "Biome Distribution Preview", true, EditorStyles.foldoutHeader);
            if (_showPreview && resolver.biomes != null && resolver.biomes.Length > 0)
            {
                DrawPreviewControls(resolver);
                DrawPreviewTexture(resolver);
                DrawLegend(resolver);
            }
        }

        private void DrawPreviewControls(MultiNoiseBiomeResolver resolver)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Axes to display", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            _xAxis = (PreviewAxis)EditorGUILayout.EnumPopup("X Axis", _xAxis);
            _yAxis = (PreviewAxis)EditorGUILayout.EnumPopup("Y Axis", _yAxis);
            if (EditorGUI.EndChangeCheck()) _previewDirty = true;
            EditorGUILayout.EndHorizontal();

            if (_xAxis == _yAxis)
            {
                EditorGUILayout.HelpBox("X and Y axes should be different for a useful preview.", MessageType.Warning);
            }

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Fixed axis values (for axes not displayed)", EditorStyles.miniLabel);

            EditorGUI.BeginChangeCheck();
            if (_xAxis != PreviewAxis.Temperature     && _yAxis != PreviewAxis.Temperature)     _fixedTemp  = EditorGUILayout.Slider("Temperature",     _fixedTemp,  0f, 1f);
            if (_xAxis != PreviewAxis.Moisture         && _yAxis != PreviewAxis.Moisture)         _fixedMoist = EditorGUILayout.Slider("Moisture",         _fixedMoist, 0f, 1f);
            if (_xAxis != PreviewAxis.Continentalness  && _yAxis != PreviewAxis.Continentalness)  _fixedCont  = EditorGUILayout.Slider("Continentalness",  _fixedCont,  0f, 1f);
            if (_xAxis != PreviewAxis.Erosion          && _yAxis != PreviewAxis.Erosion)          _fixedEros  = EditorGUILayout.Slider("Erosion",          _fixedEros,  0f, 1f);
            if (_xAxis != PreviewAxis.Weirdness        && _yAxis != PreviewAxis.Weirdness)        _fixedWeird = EditorGUILayout.Slider("Weirdness",        _fixedWeird, 0f, 1f);
            if (EditorGUI.EndChangeCheck()) _previewDirty = true;

            EditorGUILayout.BeginHorizontal();
            _previewRes = EditorGUILayout.IntSlider("Resolution", _previewRes, 32, 256);
            if (GUILayout.Button("Refresh", GUILayout.Width(60))) _previewDirty = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawPreviewTexture(MultiNoiseBiomeResolver resolver)
        {
            if (_previewDirty || _previewTex == null || _previewTex.width != _previewRes)
            {
                RebuildPreview(resolver);
                _previewDirty = false;
            }

            EditorGUILayout.Space(4);

            float displaySize = Mathf.Min(EditorGUIUtility.currentViewWidth - 60f, 300f);
            Rect previewRect = GUILayoutUtility.GetRect(displaySize, displaySize);

            // Axis labels
            EditorGUI.LabelField(
                new Rect(previewRect.x + previewRect.width * 0.5f - 40, previewRect.yMax + 2, 80, 16),
                $"← {_xAxis} →", EditorStyles.centeredGreyMiniLabel);

            GUI.DrawTexture(previewRect, _previewTex, ScaleMode.StretchToFill);

            Event evt = Event.current;
            if (previewRect.Contains(evt.mousePosition))
            {
                float mx = (evt.mousePosition.x - previewRect.x) / previewRect.width;
                float my = 1f - (evt.mousePosition.y - previewRect.y) / previewRect.height;

                ClimateData sample = BuildClimate(mx, my);
                BiomeBlendResult result = resolver.Resolve(sample);
                string biomeName = result.DominantBiome != null ? result.DominantBiome.biomeName : "null";

                Rect tooltipRect = new Rect(evt.mousePosition.x + 16, evt.mousePosition.y - 20, 200, 36);
                EditorGUI.DrawRect(tooltipRect, new Color(0, 0, 0, 0.8f));
                GUI.Label(tooltipRect, $"  {biomeName}\n  {_xAxis}:{mx:F2}  {_yAxis}:{my:F2}",
                    new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = Color.white } });
                Repaint();
            }

            EditorGUILayout.Space(20);
        }

        private void RebuildPreview(MultiNoiseBiomeResolver resolver)
        {
            if (_previewTex == null || _previewTex.width != _previewRes)
            {
                if (_previewTex != null) DestroyImmediate(_previewTex);
                _previewTex = new Texture2D(_previewRes, _previewRes, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Point,
                    wrapMode   = TextureWrapMode.Clamp
                };
            }

            Color[] pixels = new Color[_previewRes * _previewRes];

            for (int py = 0; py < _previewRes; py++)
            {
                float vy = (float)py / (_previewRes - 1);
                for (int px = 0; px < _previewRes; px++)
                {
                    float vx = (float)px / (_previewRes - 1);
                    ClimateData sample = BuildClimate(vx, vy);
                    BiomeBlendResult result = resolver.Resolve(sample);

                    Color color = result.DominantBiome != null
                        ? result.DominantBiome.debugColor
                        : Color.black;

                    pixels[py * _previewRes + px] = color;
                }
            }

            _previewTex.SetPixels(pixels);
            _previewTex.Apply();
        }

        private ClimateData BuildClimate(float xVal, float yVal)
        {
            float temp  = _xAxis == PreviewAxis.Temperature     ? xVal : (_yAxis == PreviewAxis.Temperature     ? yVal : _fixedTemp);
            float moist = _xAxis == PreviewAxis.Moisture         ? xVal : (_yAxis == PreviewAxis.Moisture         ? yVal : _fixedMoist);
            float cont  = _xAxis == PreviewAxis.Continentalness  ? xVal : (_yAxis == PreviewAxis.Continentalness  ? yVal : _fixedCont);
            float eros  = _xAxis == PreviewAxis.Erosion          ? xVal : (_yAxis == PreviewAxis.Erosion          ? yVal : _fixedEros);
            float weird = _xAxis == PreviewAxis.Weirdness        ? xVal : (_yAxis == PreviewAxis.Weirdness        ? yVal : _fixedWeird);
            return new ClimateData(temp, moist, cont, eros, weird);
        }

        private void DrawLegend(MultiNoiseBiomeResolver resolver)
        {
            EditorGUILayout.LabelField("Legend", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            foreach (var biome in resolver.biomes)
            {
                if (biome == null) continue;
                EditorGUILayout.BeginHorizontal();

                Rect swatchRect = GUILayoutUtility.GetRect(16, 16, GUILayout.Width(16));
                EditorGUI.DrawRect(swatchRect, biome.debugColor);

                EditorGUILayout.LabelField(biome.biomeName, GUILayout.Width(120));
                EditorGUILayout.LabelField(
                    $"T{biome.temperatureRange} M{biome.moistureRange} C{biome.continentalnessRange} E{biome.erosionRange}",
                    EditorStyles.miniLabel);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private void OnDisable()
        {
            if (_previewTex != null)
                DestroyImmediate(_previewTex);
        }
    }
}
#endif