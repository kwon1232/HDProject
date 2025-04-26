using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TMPStyleApplier))]
public class TMPStyleApplierEditor : Editor
{
    private Texture2D applyIcon;

    private void OnEnable()
    {
        applyIcon = EditorGUIUtility.IconContent("d_UnityEditor.AnimationWindow").image as Texture2D;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TMPStyleApplier applier = (TMPStyleApplier)target;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Style Tools", EditorStyles.boldLabel);

        GUILayout.Space(5);

        GUIContent buttonContent = new GUIContent("Apply Style", applyIcon);

        EditorGUI.BeginDisabledGroup(applier.applyLocked);
        if (GUILayout.Button(buttonContent, GUILayout.Height(40)))
        {
            applier.Apply();
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);

        if (GUILayout.Button("Apply Once and Lock", GUILayout.Height(30)))
        {
            applier.Apply();
            applier.applyLocked = true;
            Debug.Log("Apply Once 완료, 잠금되었습니다.");
        }

        if (GUILayout.Button("Reset to Default", GUILayout.Height(30)))
        {
            applier.ResetToDefault();
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

        GUILayout.Space(20);

        // Batch Apply All 버튼 추가
        EditorGUILayout.LabelField("Batch Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Batch Apply to All Selected", GUILayout.Height(30)))
        {
            BatchApplyToAllSelected();
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

        Debug.Log($"Batch Apply 완료! 선택된 {targets.Length}개 오브젝트 적용됨");
    }
}
