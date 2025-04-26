using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TMPStyleApplier))]
public class TMPStyleApplierEditor : Editor
{
    private Texture2D applyIcon;

    private SerializedProperty targetTextProp;
    private SerializedProperty styleProp;
    private SerializedProperty applyLockedProp;
    private SerializedProperty isPresetLockedProp;
    private SerializedProperty backupStyleProp;
    TMPStyleApplier applier;
    TMPTextStyle currentStyle;

    // Auto-apply on drop option
    private static bool autoApplyOnDrop = true;

    private void OnEnable()
    {
        applyIcon = EditorGUIUtility.IconContent("d_UnityEditor.AnimationWindow").image as Texture2D;

        targetTextProp = serializedObject.FindProperty("targetText");
        styleProp = serializedObject.FindProperty("style");
        applyLockedProp = serializedObject.FindProperty("applyLocked");
        isPresetLockedProp = serializedObject.FindProperty("isPresetLocked");
        backupStyleProp = serializedObject.FindProperty("backupStyle");

        applier = (TMPStyleApplier)target;
        currentStyle = applier.GetCurrentStyle();  // style 연결되어 있거나 없으면 생성
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw default properties
        EditorGUILayout.PropertyField(targetTextProp);
        EditorGUILayout.PropertyField(styleProp);
        EditorGUILayout.PropertyField(applyLockedProp);
        EditorGUILayout.PropertyField(isPresetLockedProp);

        TMPStyleApplier applier = (TMPStyleApplier)target;
        DrawStyleSettings();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Style Tools", EditorStyles.boldLabel);

        DrawPresetDragAndDropArea((TMPStyleApplier)target);

        EditorGUILayout.Space();

        GUIContent buttonContent = new GUIContent("Apply Style", applyIcon);

        EditorGUI.BeginDisabledGroup(applier.applyLocked);
        if (GUILayout.Button(buttonContent, GUILayout.Height(40)))
        {
            applier.Apply();
        }
        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("Apply Once and Lock", GUILayout.Height(30)))
        {
            applier.Apply();
            applier.applyLocked = true;
        }

        if (GUILayout.Button("Reset to Default", GUILayout.Height(30)))
        {
            applier.ResetToDefault();
        }

        EditorGUILayout.LabelField("Meterial Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Load Material as Asset", GUILayout.Height(30)))
        {
            LoadMaterialAsAsset();
        }

        if (GUILayout.Button("Save Material as Asset", GUILayout.Height(30)))
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save TMP Material",
                "NewTMPMaterial",
                "mat",
                "Save TMP custom material"
            );

            if (!string.IsNullOrEmpty(path))
            {
                applier.SaveMaterialAsAsset(path);
            }
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Batch Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Batch Apply to All Selected", GUILayout.Height(30)))
        {
            BatchApplyToAllSelected();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Style Preset Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Save TMP Style Preset", GUILayout.Height(30)))
        {
            SaveTMPStylePreset(applier.style);
        }

        if (GUILayout.Button("Load TMP Style Preset", GUILayout.Height(30)))
        {
            LoadTMPStylePreset(applier.style);
        }

        EditorGUILayout.Space();
        autoApplyOnDrop = EditorGUILayout.Toggle("Auto-Apply Style On Drop", autoApplyOnDrop);

        serializedObject.ApplyModifiedProperties();

        // 추가: 변경사항 있으면 강제로 Apply()
        if (!Application.isPlaying && applier.targetText != null && applier.style != null)
        {
            applier.Apply();
        }
    }

    private void BatchApplyToAllSelected()
    {
        foreach (var obj in targets)
        {
            TMPStyleApplier applier = obj as TMPStyleApplier;
            if (applier != null && !applier.applyLocked)
            {
                applier.Apply();
            }
        }
        Debug.Log($"Batch Apply completed for {targets.Length} objects.");
    }

    private void SaveTMPStylePreset(TMPTextStyle style)
    {
#if UNITY_EDITOR
        string path = EditorUtility.SaveFilePanelInProject(
            "Save TMP Style Preset",
            "NewTMPStyle",
            "asset",
            "Save TMP style settings as asset"
        );

        if (!string.IsNullOrEmpty(path))
        {
            TMPTextStyle clone = ScriptableObject.CreateInstance<TMPTextStyle>();
            EditorUtility.CopySerialized(style, clone);
            AssetDatabase.CreateAsset(clone, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Style preset saved: {path}");
        }
#endif
    }

    private void LoadTMPStylePreset(TMPTextStyle targetStyle)
    {
#if UNITY_EDITOR
        string path = EditorUtility.OpenFilePanel("Load TMP Style Preset", "Assets", "asset");

        if (!string.IsNullOrEmpty(path))
        {
            path = FileUtil.GetProjectRelativePath(path);
            TMPTextStyle loaded = AssetDatabase.LoadAssetAtPath<TMPTextStyle>(path);
            if (loaded != null)
            {
                EditorUtility.CopySerialized(loaded, targetStyle);
                Debug.Log($"Style preset loaded: {path}");
            }
        }
#endif
    }

    private void LoadMaterialAsAsset()
    {
#if UNITY_EDITOR
        if (applier == null || applier.targetText == null)
        {
            Debug.LogWarning("Target Text가 설정되지 않았습니다.");
            return;
        }

        string path = EditorUtility.OpenFilePanel(
            "Load TMP Material",
            "Assets",
            "mat"
        );

        if (!string.IsNullOrEmpty(path))
        {
            path = FileUtil.GetProjectRelativePath(path); // 프로젝트 상대경로로 변환

            Material loadedMat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (loadedMat != null)
            {
                Undo.RecordObject(applier.targetText, "Load TMP Material");
                applier.targetText.fontMaterial = loadedMat;
                applier.targetText.UpdateMeshPadding();
                applier.targetText.SetMaterialDirty();
                Debug.Log($"Material loaded from {path}");
            }
            else
            {
                Debug.LogWarning($"Material을 불러올 수 없습니다: {path}");
            }
        }
#endif
    }


    // Drag & Drop Area
    private void DrawPresetDragAndDropArea(TMPStyleApplier applier)
    {
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0f, 50f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag & Drop TMP Style Preset Here");

        if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            if (dropArea.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (var draggedObject in DragAndDrop.objectReferences)
                    {
                        TMPTextStyle stylePreset = draggedObject as TMPTextStyle;
                        if (stylePreset != null)
                        {
                            if (applier.isPresetLocked)
                            {
                                Debug.LogWarning("Preset is locked. Cannot overwrite. Unlock first to apply new preset.");
                                continue;
                            }

                            // Auto backup current style before applying
                            applier.backupStyle = ScriptableObject.CreateInstance<TMPTextStyle>();
                            EditorUtility.CopySerialized(applier.style, applier.backupStyle);

                            // Apply new preset
                            Undo.RecordObject(applier, "Apply TMP Style Preset");
                            EditorUtility.CopySerialized(stylePreset, applier.style);

                            if (autoApplyOnDrop)
                            {
                                applier.Apply();
                                Debug.Log("Style preset applied automatically on drop.");
                            }
                            else
                            {
                                Debug.Log("Style preset loaded. Press 'Apply Style' to apply.");
                            }

                            // Lock after applying
                            applier.isPresetLocked = true;
                        }
                    }
                }
                Event.current.Use();
            }
        }
    }

    private void DrawStyleSettings()
    {
        TMPStyleApplier applier = (TMPStyleApplier)target;
        TMPTextStyle currentStyle = applier.GetCurrentStyle();  // 항상 최신 상태 가져오기!

        if (currentStyle == null)
        {
            EditorGUILayout.HelpBox("현재 적용할 Style이 없습니다.", MessageType.Info);
            return;
        }

        SerializedObject styleSO = new SerializedObject(currentStyle);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Style Settings (Local Preview)", EditorStyles.boldLabel);

        styleSO.Update();

        // Outline
        var outlineProp = styleSO.FindProperty("outline");
        if (outlineProp != null)
        {
            EditorGUILayout.PropertyField(outlineProp, true); // 하위 속성까지 표시
        }

        EditorGUILayout.Space();

        // Main Color
        var mainColorProp = styleSO.FindProperty("mainColor");
        if (mainColorProp != null)
        {
            EditorGUILayout.PropertyField(mainColorProp, true);
        }

        // Shadow
        var shadowProp = styleSO.FindProperty("shadow");
        if (shadowProp != null)
        {
            EditorGUILayout.PropertyField(shadowProp, true);
        }

        // Glow
        var glowProp = styleSO.FindProperty("glow");
        if (glowProp != null)
        {
            EditorGUILayout.PropertyField(glowProp, true);
        }

        // Gradient
        var gradientProp = styleSO.FindProperty("gradient");
        if (gradientProp != null)
        {
            EditorGUILayout.PropertyField(gradientProp, true);
        }

        styleSO.ApplyModifiedProperties();

        //  실시간 적용
        if (!Application.isPlaying)
        {
            applier.Apply();
        }
    }


}