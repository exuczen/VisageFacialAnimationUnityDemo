using System;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace UnityEditor
{
    // ReSharper disable once InconsistentNaming
    internal class PT_MobileSpecular_v07GUI : ShaderGUI
    {
        private enum WorkflowMode
        {
            Specular,
            Metallic,
            Dielectric
        }

        public enum BlendMode
        {
            Opaque,
            Cutout,
            Fade,
            Transparent
        }

        public enum SmoothnessMapChannel
        {
            SpecularMetallicAlpha,
            AlbedoAlpha
        }

        private static class Styles
        {
            public static readonly GUIContent AlbedoText = new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)");
            public static readonly GUIContent AlphaCutoffText = new GUIContent("Alpha Cutoff", "Threshold for alpha cutoff");
            public static readonly GUIContent SpecularMapText = new GUIContent("Specular", "Specular (RGB) and Smoothness (A)");
            public static readonly GUIContent MetallicMapText = new GUIContent("Metallic", "Metallic (R) and Smoothness (A)");
            public static readonly GUIContent SmoothnessText = new GUIContent("Smoothness", "Smoothness value");
            public static readonly GUIContent SmoothnessScaleText = new GUIContent("Smoothness", "Smoothness scale factor");
            public static readonly GUIContent SmoothnessMapChannelText = new GUIContent("Source", "Smoothness texture and channel");
            public static readonly GUIContent HighlightsText = new GUIContent("Specular Highlights", "Specular Highlights");
            public static readonly GUIContent ReflectionsText = new GUIContent("Reflections", "Glossy Reflections");
            public static readonly GUIContent EmissionText = new GUIContent("Emission Map", "Emission (RGB)");
            public static readonly GUIContent DetailNormalMapText = new GUIContent("Normal", "Normal Map");
            public static readonly GUIContent NormalMapText = new GUIContent("Normal", "Normal Map");

            public const string PrimaryMapsText = "Main Maps";
            public static string SecondaryMapsText = "Secondary Maps";
            public const string ForwardText = "Forward Rendering Options";
            public const string RenderingMode = "Rendering Mode";
            public const string AdvancedText = "Advanced Options";

            public static readonly string[] BlendNames = Enum.GetNames(typeof(BlendMode));
        }

        private MaterialProperty _blendMode;
        private MaterialProperty _albedoMap;
        private MaterialProperty _albedoColor;
        private MaterialProperty _alphaCutoff;
        private MaterialProperty _specularMap;
        private MaterialProperty _specularColor;
        private MaterialProperty _metallicMap;
        private MaterialProperty _metallic;
        private MaterialProperty _smoothness;
        private MaterialProperty _smoothnessScale;
        private MaterialProperty _smoothnessMapChannel;
        private MaterialProperty _highlights;
        private MaterialProperty _reflections;
        private MaterialProperty _bumpScale = null;
        private MaterialProperty _bumpMap = null;
        private MaterialProperty _occlusionStrength = null;
        private MaterialProperty _occlusionMap = null;
        private MaterialProperty _heigtMapScale = null;
        private MaterialProperty _heightMap = null;
        private MaterialProperty _emissionColorForRendering;
        private MaterialProperty _emissionMap;
        private MaterialProperty _detailAlbedoMap;
        private MaterialProperty _detailNormalMapScale;
        private MaterialProperty _detailNormalMap;


        private MaterialEditor _mMaterialEditor;
        private WorkflowMode _mWorkflowMode = WorkflowMode.Specular;
        private readonly ColorPickerHDRConfig _mColorPickerHdrConfig = new ColorPickerHDRConfig(0f, 99f, 1 / 99f, 3f);

        private bool _mFirstTimeApply = true;

        public void FindProperties(MaterialProperty[] props)
        {
            _blendMode = FindProperty("_Mode", props);
            _albedoMap = FindProperty("_MainTex", props);
            _albedoColor = FindProperty("_Color", props);
            _alphaCutoff = FindProperty("_Cutoff", props);
            _specularMap = FindProperty("_SpecGlossMap", props, false);
            _specularColor = FindProperty("_SpecColor", props, false);
            _metallicMap = FindProperty("_MetallicGlossMap", props, false);
            _metallic = FindProperty("_Metallic", props, false);
            
            _bumpScale = FindProperty("_BumpScale", props);
            _bumpMap = FindProperty("_BumpMap", props);
            
            if (_specularMap != null && _specularColor != null)    _mWorkflowMode = WorkflowMode.Specular;
            else if (_metallicMap != null && _metallic != null)    _mWorkflowMode = WorkflowMode.Metallic;
            else                                                   _mWorkflowMode = WorkflowMode.Dielectric;
            
            _smoothness = FindProperty("_Glossiness", props);
            _smoothnessScale = FindProperty("_GlossMapScale", props, false);
            _smoothnessMapChannel = FindProperty("_SmoothnessTextureChannel", props, false);
            _highlights = FindProperty("_SpecularHighlights", props, false);
            _reflections = FindProperty("_GlossyReflections", props, false);
            _emissionColorForRendering = FindProperty("_EmissionColor", props);
            _emissionMap = FindProperty("_EmissionMap", props);
            _detailAlbedoMap = FindProperty("_DetailAlbedoMap", props);
            _detailNormalMapScale = FindProperty("_DetailNormalMapScale", props);
            _detailNormalMap = FindProperty("_DetailNormalMap", props);
      
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            FindProperties(props);
            _mMaterialEditor = materialEditor;
            var material = materialEditor.target as Material;

            if (_mFirstTimeApply)
            {
                MaterialChanged(material, _mWorkflowMode);
                _mFirstTimeApply = false;
            }

            ShaderPropertiesGUI(material);
        }

        public void ShaderPropertiesGUI(Material material)
        {
            EditorGUIUtility.labelWidth = 0f;

            EditorGUI.BeginChangeCheck();
            {
                BlendModePopup();

                GUILayout.Label(Styles.PrimaryMapsText, EditorStyles.boldLabel);
                DoAlbedoArea(material);
                DoSpecularMetallicArea();

                EditorGUI.BeginChangeCheck();
                _mMaterialEditor.TextureScaleOffsetProperty(_albedoMap);
                if (EditorGUI.EndChangeCheck()) _emissionMap.textureScaleAndOffset = _albedoMap.textureScaleAndOffset;

                EditorGUILayout.Space();

                _mMaterialEditor.TexturePropertySingleLine(Styles.DetailNormalMapText, _detailNormalMap, _detailNormalMapScale);
                _mMaterialEditor.TextureScaleOffsetProperty(_detailAlbedoMap);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                DoEmissionArea(material);
                EditorGUILayout.Space();

                GUILayout.Label(Styles.ForwardText, EditorStyles.boldLabel);
                if (_highlights != null)
                    _mMaterialEditor.ShaderProperty(_highlights, Styles.HighlightsText);
                if (_reflections != null)
                    _mMaterialEditor.ShaderProperty(_reflections, Styles.ReflectionsText);
            }
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in _blendMode.targets)
                    MaterialChanged((Material) obj, _mWorkflowMode);
            }

            EditorGUILayout.Space();

            GUILayout.Label(Styles.AdvancedText, EditorStyles.boldLabel);
            _mMaterialEditor.EnableInstancingField();
            //_mMaterialEditor.DoubleSidedGIField();
        }

        internal void DetermineWorkflow(MaterialProperty[] props)
        {
            _mWorkflowMode = WorkflowMode.Specular;
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material.HasProperty("_Emission")) material.SetColor("_EmissionColor", material.GetColor("_Emission"));

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialWithBlendMode(material, (BlendMode) material.GetFloat("_Mode"));
                return;
            }

            var blendMode = BlendMode.Opaque;
            if (oldShader.name.Contains("/Transparent/Cutout/")) blendMode = BlendMode.Cutout;
            else if (oldShader.name.Contains("/Transparent/")) blendMode = BlendMode.Fade;

            material.SetFloat("_Mode", (float) blendMode);

            DetermineWorkflow(MaterialEditor.GetMaterialProperties(new Object[] {material}));
            MaterialChanged(material, _mWorkflowMode);
        }

        private void BlendModePopup()
        {
            EditorGUI.showMixedValue = _blendMode.hasMixedValue;
            var mode = (BlendMode) _blendMode.floatValue;

            EditorGUI.BeginChangeCheck();
            mode = (BlendMode) EditorGUILayout.Popup(Styles.RenderingMode, (int) mode, Styles.BlendNames);
            if (EditorGUI.EndChangeCheck())
            {
                _mMaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
                _blendMode.floatValue = (float) mode;
            }

            EditorGUI.showMixedValue = false;
        }

        private void DoAlbedoArea(Material material)
        {
            _mMaterialEditor.TexturePropertySingleLine(Styles.AlbedoText, _albedoMap, _albedoColor);
            if (((BlendMode) material.GetFloat("_Mode") == BlendMode.Cutout))
            {
                _mMaterialEditor.ShaderProperty(_alphaCutoff, Styles.AlphaCutoffText.text, MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
            }
        }

        private void DoEmissionArea(Material material)
        {
            if (_mMaterialEditor.EmissionEnabledProperty())
            {
                var hadEmissionTexture = _emissionMap.textureValue != null;

                _mMaterialEditor.TexturePropertyWithHDRColor(Styles.EmissionText, _emissionMap, _emissionColorForRendering, _mColorPickerHdrConfig, false);

                var brightness = _emissionColorForRendering.colorValue.maxColorComponent;
                if (_emissionMap.textureValue != null && !hadEmissionTexture && brightness <= 0f)
                    _emissionColorForRendering.colorValue = Color.white;

                _mMaterialEditor.LightmapEmissionFlagsProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel, true);
            }
        }

        private void DoSpecularMetallicArea()
        {
            var hasGlossMap = false;
            if (_mWorkflowMode == WorkflowMode.Specular)
            {
                hasGlossMap = _specularMap.textureValue != null;
                _mMaterialEditor.TexturePropertySingleLine(Styles.SpecularMapText, _specularMap, hasGlossMap ? null : _specularColor);
            }
            else if (_mWorkflowMode == WorkflowMode.Metallic)
            {
                hasGlossMap = _metallicMap.textureValue != null;
                _mMaterialEditor.TexturePropertySingleLine(Styles.MetallicMapText, _metallicMap, hasGlossMap ? null : _metallic);
            }

            var showSmoothnessScale = hasGlossMap;
            if (_smoothnessMapChannel != null)
            {
                var smoothnessChannel = (int) _smoothnessMapChannel.floatValue;
                if (smoothnessChannel == (int) SmoothnessMapChannel.AlbedoAlpha)
                    showSmoothnessScale = true;
            }

            var indentation = 2;
            _mMaterialEditor.ShaderProperty(showSmoothnessScale ? _smoothnessScale : _smoothness,
                showSmoothnessScale ? Styles.SmoothnessScaleText : Styles.SmoothnessText, indentation);

            ++indentation;
            if (_smoothnessMapChannel != null)
                _mMaterialEditor.ShaderProperty(_smoothnessMapChannel, Styles.SmoothnessMapChannelText, indentation);
        }

        internal static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    material.SetOverrideTag("RenderType", "");
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case BlendMode.Cutout:
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int) RenderQueue.AlphaTest;
                    break;
                case BlendMode.Fade:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int) RenderQueue.Transparent;
                    break;
                case BlendMode.Transparent:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int) RenderQueue.Transparent;
                    break;
            }
        }

        private static SmoothnessMapChannel GetSmoothnessMapChannel(Material material)
        {
            var ch = (int) material.GetFloat("_SmoothnessTextureChannel");
            return ch == (int) SmoothnessMapChannel.AlbedoAlpha ? SmoothnessMapChannel.AlbedoAlpha : SmoothnessMapChannel.SpecularMetallicAlpha;
        }

        private static void SetMaterialKeywords(Material material, WorkflowMode workflowMode)
        {
            SetKeyword(material, "_NORMALMAP", material.GetTexture("_BumpMap") || material.GetTexture("_DetailNormalMap"));
            if (workflowMode == WorkflowMode.Specular) SetKeyword(material, "_SPECGLOSSMAP", material.GetTexture("_SpecGlossMap"));
            else if (workflowMode == WorkflowMode.Metallic) SetKeyword(material, "_METALLICGLOSSMAP", material.GetTexture("_MetallicGlossMap"));
            SetKeyword(material, "_PARALLAXMAP", material.GetTexture("_ParallaxMap"));
            SetKeyword(material, "_DETAIL_MULX2", material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap"));

            MaterialEditor.FixupEmissiveFlag(material);
            var shouldEmissionBeEnabled = (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
            SetKeyword(material, "_EMISSION", shouldEmissionBeEnabled);

            if (material.HasProperty("_SmoothnessTextureChannel"))
            {
                SetKeyword(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", GetSmoothnessMapChannel(material) == SmoothnessMapChannel.AlbedoAlpha);
            }
        }

        private static void MaterialChanged(Material material, WorkflowMode workflowMode)
        {
            SetupMaterialWithBlendMode(material, (BlendMode) material.GetFloat("_Mode"));

            SetMaterialKeywords(material, workflowMode);
        }

        private static void SetKeyword(Material m, string keyword, bool state)
        {
            if (state)
                m.EnableKeyword(keyword);
            else
                m.DisableKeyword(keyword);
        }
    }
}