
using UnityEngine;
using TMPro;
using UnityEditor;
using System;

[System.Serializable]
public class TMPStyleOutline
{
    public bool useOutline = true;
    [Range(0, 1)] public float width = 0.1f;
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

        if (sharedMaterialInstance == null)
        {
            sharedMaterialInstance = new Material(targetText.fontMaterial);
            targetText.fontMaterial = sharedMaterialInstance;
        }
    }

    public void Apply()
    {
        // 1) 타겟 텍스트가 없으면 아무 것도 안 함
        if (targetText == null)
            return;

        // 2) fontMaterial이 없으면 sharedMaterial → fontAsset.material 순으로 폴백
        Material baseMat = targetText.fontMaterial;
        if (baseMat == null)
            baseMat = targetText.fontSharedMaterial != null
                      ? targetText.fontSharedMaterial
                      : (targetText.font != null
                         ? targetText.font.material
                         : null);
        if (baseMat == null)
            return;

        // 3) 스타일 ScriptableObject가 없으면 생성
        if (style == null)
            style = ScriptableObject.CreateInstance<TMPTextStyle>();

        // 4) sharedMaterialInstance가 없으면 baseMat 복사
        if (sharedMaterialInstance == null)
        {
            sharedMaterialInstance = new Material(baseMat);
            targetText.fontMaterial = sharedMaterialInstance;
        }

        // 5) 이제 안전하게 스타일 적용
        ApplyStyleToMaterial(sharedMaterialInstance, targetText);
        targetText.UpdateMeshPadding();
        targetText.SetMaterialDirty();
    }


    private void ApplyStyleToMaterial(Material mat, TextMeshProUGUI text)
    {
        if (mat == null || text == null) return;

        // 1. Outline
        if (style.outline.useOutline)
        {
            mat.SetFloat(ShaderUtilities.ID_OutlineWidth, style.outline.width);
            mat.SetColor(ShaderUtilities.ID_OutlineColor, style.outline.color);
            mat.EnableKeyword("OUTLINE_ON");
        }
        else
        {
            mat.SetFloat(ShaderUtilities.ID_OutlineWidth, 0f);
            mat.DisableKeyword("OUTLINE_ON");
        }

        // 2. Main Color
        if (style.mainColor.overrideColor)
        {
            text.color = style.mainColor.color;
        }

        // 3. Shadow or Glow (Underlay 통합 처리)
        bool enableUnderlay = false;

        if (style.shadow.useShadow)
        {
            mat.SetColor(ShaderUtilities.ID_UnderlayColor, style.shadow.shadowColor);
            mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, style.shadow.shadowDistance.x);
            mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, style.shadow.shadowDistance.y);
            mat.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0.2f); // 쉐도우는 약간 소프트
            enableUnderlay = true;
        }
        else if (style.glow.useGlow)
        {
            mat.SetColor(ShaderUtilities.ID_UnderlayColor, style.glow.glowColor);
            mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0f);
            mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, 0f);
            mat.SetFloat(ShaderUtilities.ID_UnderlaySoftness, style.glow.glowOffset); // 글로우는 부드럽게
            enableUnderlay = true;
        }

        if (enableUnderlay)
            mat.EnableKeyword("UNDERLAY_ON");
        else
            mat.DisableKeyword("UNDERLAY_ON");

        // 4. Gradient
        if (style.gradient.useGradient)
        {
            text.enableVertexGradient = true;
            text.colorGradient = new VertexGradient(
                style.gradient.topColor,
                style.gradient.topColor,
                style.gradient.bottomColor,
                style.gradient.bottomColor
            );
        }
        else
        {
            text.enableVertexGradient = false;
        }

        // 최종 적용
        text.fontMaterial = mat;
        text.UpdateMeshPadding();
        text.SetMaterialDirty();
    }


    public void ResetToDefault()
    {
        if (targetText == null || originalMaterial == null)
        {
            //Debug.LogWarning("Reset할 원본 머티리얼이 없습니다.");
            return;
        }

        targetText.fontMaterial = originalMaterial;
        targetText.UpdateMeshPadding();
        targetText.SetMaterialDirty();
        sharedMaterialInstance = null;
        applyLocked = false;

        //Debug.Log("원본 머티리얼로 복원 완료!");
    }

    public void SaveMaterialAsAsset(string path)
    {
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying || applyLocked) return;

        // targetText가 없으면 찾아보고
        if (targetText == null)
            targetText = GetComponent<TextMeshProUGUI>();
        if (targetText == null)
            return;

        // fontSharedMaterial이 비어 있으면, fontAsset에서 머티리얼 가져오기
        if (targetText.fontSharedMaterial == null && targetText.font != null)
        {
            // font.material 은 TMP_FontAsset에 연결된 기본 머티리얼
            targetText.fontSharedMaterial = targetText.font.material;
        }

        // fontMaterial getter 쪽이 SharedMaterial을 기반으로 새 인스턴스를 만들기 때문에
        //    폴백이 완료된 후에 Apply() 호출
        Apply();
    }
#endif


    public TMPTextStyle GetCurrentStyle()
    {
        if (style != null)
            return style;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Undo.RecordObject(this, "Create TMP Preview Style"); // Undo 지원
            style = ScriptableObject.CreateInstance<TMPTextStyle>();
            style.name = "(Runtime Preview Style)";
            return style;
        }
#endif

        return null;
    }
}
