using UnityEngine;
using TMPro;
using UnityEditor;

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
    public TextMeshProUGUI targetText;

    [Header("Style Settings")]
    public TMPTextStyle style;

    [Header("Apply Settings")]
    public bool applyLocked = false; // Apply 한번 하고 잠글지 여부

    private Material sharedMaterialInstance; // 복사본 머티리얼

    private Material originalMaterial; // 리셋할 때 쓸 원본 저장

    [Header("Preset Lock Settings")]
    public bool isPresetLocked = false;    // Preset Lock
    public TMPTextStyle backupStyle;       // Backup before applying preset

    public void Apply()
    {
        if (applyLocked)
        {
            Debug.LogWarning("스타일 적용이 잠겨있습니다. Reset을 해야 다시 Apply할 수 있습니다.");
            return;
        }

        if (targetText == null || style == null)
            return;

        if (originalMaterial == null)
            originalMaterial = targetText.fontMaterial; // 원본 백업 (처음 Apply할 때만)

        if (sharedMaterialInstance == null)
        {
            sharedMaterialInstance = new Material(targetText.fontMaterial);
            targetText.fontMaterial = sharedMaterialInstance;
        }

        ApplyStyleToMaterial(sharedMaterialInstance, targetText);

        targetText.UpdateMeshPadding();
        targetText.SetMaterialDirty();
    }

    private void ApplyStyleToMaterial(Material mat, TextMeshProUGUI text)
    {
        if (mat == null || text == null) return;

        if (style.outline.useOutline)
        {
            mat.SetFloat(ShaderUtilities.ID_OutlineWidth, style.outline.width);
            mat.SetColor(ShaderUtilities.ID_OutlineColor, style.outline.color);
        }
        else
        {
            mat.SetFloat(ShaderUtilities.ID_OutlineWidth, 0f);
        }

        if (style.mainColor.overrideColor)
        {
            text.color = style.mainColor.color;
        }
    }

    public void ResetToDefault()
    {
        if (targetText == null || originalMaterial == null)
        {
            Debug.LogWarning("Reset할 원본 머티리얼이 없습니다.");
            return;
        }

        targetText.fontMaterial = originalMaterial;
        targetText.UpdateMeshPadding();
        targetText.SetMaterialDirty();
        sharedMaterialInstance = null;
        applyLocked = false;

        Debug.Log("원본 머티리얼로 복원 완료!");
    }

    public void SaveMaterialAsAsset(string path)
    {
        if (sharedMaterialInstance == null)
        {
            Debug.LogWarning("저장할 머티리얼이 없습니다. 먼저 Apply를 해주세요.");
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
    void OnValidate()
    {
        if (!Application.isPlaying && !applyLocked)
        {
            Apply();
        }
    }
#endif
}