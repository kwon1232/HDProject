#if UNITY_EDITOR 
using UnityEngine;
using TMPro;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;
using static UnityEngine.Rendering.DebugUI.MessageBox;
using System.Collections.Generic;


[System.Serializable]
public class TMPStyleOutline
{
    public bool useOutline = true;
    [Range(0, 1)] public float width = 0.1f;
    [Range(0, 1)] public float dilate = 0.1f;
    public Color color = Color.black;
}

[System.Serializable]
public class TMPStyleMainColor
{
    public bool overrideColor = false;
    public Color color = Color.white;
}

[System.Serializable]
public class TMPStyleShadow
{
    public bool useShadow = false;
    public Color shadowColor = Color.black;
    public Vector2 shadowDistance = new Vector2(1f, -1f);
}

[System.Serializable]
public class TMPStyleGlow
{
    public bool useGlow = false;
    [Range(0, 1)] public float glowOffset = 0.1f;
    public Color glowColor = Color.white;
}

[System.Serializable]
public class TMPStyleGradient
{
    public bool useGradient = false;
    public Color topColor = Color.white;
    public Color bottomColor = Color.gray;
}

[System.Serializable]
[CreateAssetMenu(fileName = "TMPTextStyle", menuName = "TMP Style/New Text Style")]
public class TMPTextStyle : ScriptableObject
{
    public TMPStyleOutline outline = new TMPStyleOutline();
    public TMPStyleMainColor mainColor = new TMPStyleMainColor();
    public TMPStyleShadow shadow = new TMPStyleShadow();
    public TMPStyleGlow glow = new TMPStyleGlow();
    public TMPStyleGradient gradient = new TMPStyleGradient();

    public void ApplyTo(TextMeshProUGUI targetText)
    {
        if (targetText == null) return;

        Material mat = new Material(targetText.fontMaterial);

        // Outline
        if (outline.useOutline)
        {
            mat.SetFloat(ShaderUtilities.ID_FaceDilate, outline.dilate);
            mat.SetFloat(ShaderUtilities.ID_OutlineWidth, outline.width);
            mat.SetColor(ShaderUtilities.ID_OutlineColor, outline.color);
        }
        else
        {
            mat.SetFloat(ShaderUtilities.ID_OutlineWidth, 0f);
        }

        // Main Color
        if (mainColor.overrideColor)
        {
            targetText.color = mainColor.color;
            mat.SetColor(ShaderUtilities.ID_MainTex, mainColor.color);
        }

        // Shadow
        var shadowComp = targetText.GetComponent<UnityEngine.UI.Shadow>();
        if (shadow.useShadow)
        {
            if (shadowComp == null)
                shadowComp = targetText.gameObject.AddComponent<UnityEngine.UI.Shadow>();

            shadowComp.effectColor = shadow.shadowColor;
            shadowComp.effectDistance = shadow.shadowDistance;
        }
        else
        {
            if (shadowComp != null)
                GameObject.DestroyImmediate(shadowComp);
        }

        // Glow (Underlay)
        if (glow.useGlow)
        {
            mat.EnableKeyword("UNDERLAY_ON");
            mat.SetColor(ShaderUtilities.ID_UnderlayColor, glow.glowColor);
            mat.SetFloat(ShaderUtilities.ID_UnderlaySoftness, glow.glowOffset);
        }
        else
        {
            mat.DisableKeyword("UNDERLAY_ON");
        }

        // Gradient
        if (gradient.useGradient)
        {
            targetText.enableVertexGradient = true;
            VertexGradient grad = new VertexGradient(
                gradient.topColor, gradient.topColor,
                gradient.bottomColor, gradient.bottomColor
            );
            targetText.colorGradient = grad;
        }
        else
        {
            targetText.enableVertexGradient = false;
        }



        targetText.fontMaterial = mat;
        targetText.UpdateMeshPadding();
        targetText.SetMaterialDirty();
    }
}

[CanEditMultipleObjects]
[CustomEditor(typeof(TMPStyleApplier))]
public class TMPStyleApplier : MonoBehaviour
{
    [Header("Target")]
    [SerializeField]
    public TextMeshProUGUI targetText;

    [Header("Style Settings")]
    [SerializeField]
    public TMPTextStyle style;

    [Header("Apply Settings")]
    [SerializeField]
    public bool applyLocked = false; // Apply 한번 하고 잠글지 여부

    private Material sharedMaterialInstance; // 복사본 머티리얼

    private Material originalMaterial; // 리셋할 때 쓸 원본 저장

    [Header("Preset Lock Settings")]
    [SerializeField]
    public bool isPresetLocked = false;    // Preset Lock
    public TMPTextStyle backupStyle;       // Backup before applying preset
    private TMPTextStyle runtimePreviewStyle;


    private void Awake()
    {
        if (targetText == null)
        {
            targetText = GetComponent<TextMeshProUGUI>();
        }
        if (style == null)
        {
            style = ScriptableObject.CreateInstance<TMPTextStyle>();
        }

        if (originalMaterial == null)
            originalMaterial = targetText.fontMaterial; 

        if (originalMaterial == null && sharedMaterialInstance == null)
        {
            sharedMaterialInstance = new Material(targetText.fontMaterial);
            targetText.fontMaterial = sharedMaterialInstance;
        }
    }

    private void Start()
    {
        if (Application.isPlaying)
        {
            Apply();
        }
    }

    public void Apply()
    {
        if (targetText == null || style == null) return;

        Material mat = targetText.fontMaterial;

        // Outline
        if (style.outline.useOutline)
        {
            mat.SetFloat(ShaderUtilities.ID_OutlineWidth, style.outline.width);
            mat.SetFloat(ShaderUtilities.ID_FaceDilate, style.outline.dilate);
            mat.SetColor(ShaderUtilities.ID_OutlineColor, style.outline.color);
        }
        else
        {
            mat.SetFloat(ShaderUtilities.ID_OutlineWidth, 0f);
        }

        // Main Color
        if (style.mainColor.overrideColor)
        {
            targetText.color = style.mainColor.color;
            mat.SetColor(ShaderUtilities.ID_FaceColor, style.mainColor.color);
        }

        // Underlay (Shadow or Glow)
        bool enableUnderlay = false;

        if (style.shadow.useShadow)
        {
            mat.SetColor(ShaderUtilities.ID_UnderlayColor, style.shadow.shadowColor);
            mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, style.shadow.shadowDistance.x);
            mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, style.shadow.shadowDistance.y);
            enableUnderlay = true;
        }
        else if (style.glow.useGlow)
        {
            mat.SetColor(ShaderUtilities.ID_UnderlayColor, style.glow.glowColor);
            mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0f);
            mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, 0f);
            mat.SetFloat(ShaderUtilities.ID_UnderlaySoftness, style.glow.glowOffset);
            enableUnderlay = true;
        }

        if (enableUnderlay)
            mat.EnableKeyword("UNDERLAY_ON");
        else
            mat.DisableKeyword("UNDERLAY_ON");

        // Gradient
        if (style.gradient.useGradient)
        {
            targetText.enableVertexGradient = true;
            targetText.colorGradient = new VertexGradient(
                style.gradient.topColor, style.gradient.topColor,
                style.gradient.bottomColor, style.gradient.bottomColor
            );
        }
        else
        {
            targetText.enableVertexGradient = false;
        }

        targetText.fontMaterial = mat;



        targetText.UpdateMeshPadding();
        targetText.SetMaterialDirty();
        targetText.ForceMeshUpdate();
    }

    public void ApplyStyleToOrignalMaterial()
    {
        if (originalMaterial == null || style == null)
        {
            Debug.LogWarning("Original Mererial 또는 Style이 존재하지 않습니다.");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(originalMaterial);
        if (string.IsNullOrEmpty(assetPath))
        {
            Debug.LogWarning("Original Material이 에셋 파일이 아닙니다. 저장할 수 없습니다.");
            return;
        }

        // BackUp
        string backupPath = assetPath.Replace(".mat", "_Backup.mat");
    }

    public void ResetToDefault()
    {
        if (originalMaterial == null || targetText == null) return;

        targetText.font.material = originalMaterial;
        targetText.UpdateMeshPadding();
        targetText.SetMaterialDirty();
    }

    public void SaveMaterialAsAsset(string path)
    {
        sharedMaterialInstance = targetText.font.material;
        if (sharedMaterialInstance == null)
        {
            //Debug.LogWarning("저장할 머티리얼이 없습니다. 먼저 Apply를 해주세요.");
            return;
        }

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(new Material(sharedMaterialInstance), path);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log($"머티리얼이 저장되었습니다: {path}");
#else
        Debug.LogWarning("에셋 저장은 에디터에서만 가능합니다.");
#endif
    }

    private bool validateScheduled = false;

    private void OnValidate()
    {
        if (validateScheduled) return;
        validateScheduled = true;

        EditorApplication.delayCall += () =>
        {
            validateScheduled = false;

            if (this == null) return;
            InitializeMaterial();
            Apply();
        };
    }

    private void Reset()
    {
        if (this == null) return;
        InitializeMaterial();
        Apply();
    }

    private void InitializeMaterial()
    {
        if (targetText == null)
            targetText = GetComponent<TextMeshProUGUI>();

        if (originalMaterial == null && targetText.font.material != null)
        {
            originalMaterial = targetText.font.material;
            sharedMaterialInstance = originalMaterial;
        }
        if (targetText != null && sharedMaterialInstance == null && targetText.font.material != null && originalMaterial == null)
        {
            sharedMaterialInstance = new Material(targetText.font.material);
            targetText.font.material = sharedMaterialInstance;
        }
    }


    public TMPTextStyle GetCurrentStyle()
    {
        if (style != null)
            return style;


        if (!Application.isPlaying)
        {
            Undo.RecordObject(this, "Create TMP Preview Style"); // Undo 지원
            style = ScriptableObject.CreateInstance<TMPTextStyle>();
            style.name = "(Runtime Preview Style)";
            return style;
        }

        return null;
    }
}
#endif