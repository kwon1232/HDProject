// GoogleDTTemplate.cs

using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ScriptableObject용 클래스 자동 생성기
/// </summary>
public static class ScriptableObjectTemplateGenerator
{
    // 유니티에서 ScriptableObject용 C# 클래스 소스코드를 자동으로 만들어주는 유틸리티 함수
    /*
     * className : 생성할 클래스 이름 (예: ItemData)
     * fields : 필드(멤버 변수) 이름 목록 (예: ["id", "name", "value"])
     * types : 각 필드의 데이터 타입 목록 (예: ["int", "string", "float"])
     * folder : 생성할 .cs 파일이 저장될 폴더 경로 (예: "Assets/Scripts/Generated")
     */
    public static void GenerateTemplateClass(string className, List<string> fields, List<string> types, string folder)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine($"\n[CreateAssetMenu(fileName = \"{className}\", menuName = \"Data/{className}\")]\npublic class {className} : ScriptableObject\n{{");

        for (int i = 0; i < fields.Count; i++)
        {
            sb.AppendLine($"    public {types[i]} {fields[i]};");
        }
        sb.AppendLine("}");

        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, className + ".cs"), sb.ToString());
        AssetDatabase.Refresh();
    }
}

/// <summary>
/// 시트 데이터에서 타입 자동 추론 (int, float, bool, string)
/// </summary>
public static class TypeInference
{
    /*
     * 목적: 구글 시트나 기타 데이터 테이블을 기반으로,
     * ScriptableObject 클래스를 자동으로 만들어주기 위한 코드 자동 생성기
     * 장점:
     * 사람이 일일이 클래스를 만들지 않아도 됨
     * 데이터 구조가 바뀌어도 자동화 가능
     * 에디터/자동화 파이프라인에서 활용 용이
     */
    public static List<string> InferFieldTypes(List<List<string>> rows)
    {
        var types = new List<string>();
        if (rows.Count == 0) return types;
        int colCount = rows[0].Count;
        for (int i = 0; i < colCount; i++)
        {
            bool isInt = true, isFloat = true, isBool = true;

            for (int j = 0; j < rows.Count; j++)
            {
                if (i >= rows[j].Count) continue;
                string val = rows[j][i].Trim();
                if (!int.TryParse(val, out _)) isInt = false;
                if (!float.TryParse(val, out _)) isFloat = false;
                // 문자열을 소문자로 변경하여 true false 판별 안정화
                string v = val.ToLowerInvariant();
                if (v != "true" && v != "false" && v != "1" && v != "0") isBool = false;
            }

            if (isInt) types.Add("int");
            else if (isFloat) types.Add("float");
            else if (isBool) types.Add("bool");
            else types.Add("string");
        }
        return types;
    }
}


/// <summary>
/// Google 스프레드시트 → ScriptableObject 자동 생성기
/// </summary>
[ExecuteInEditMode]
public class GoogleDTTemplate : MonoBehaviour
{
    [Header("Google 스프레드시트 ID / 시트명")]
    public string spreadsheetId;
    public string sheetName;

    [ContextMenu("구글 시트 → ScriptableObject 자동 변환")]
    public void ConvertSheetToScriptableObjects()
    {
        // 구글 시트 CSV URL 조합
        string csvUrl = $"https://docs.google.com/spreadsheets/d/{spreadsheetId}/gviz/tq?tqx=out:csv&sheet={sheetName}";
        List<List<string>> data = GoogleSheetLoader.LoadCSVFromUrl(csvUrl);

        if (data == null || data.Count < 2)
        {
            Debug.LogError("데이터가 없거나 잘못된 시트입니다.");
            return;
        }

        List<string> headers = data[0];
        List<List<string>> rows = data.GetRange(1, data.Count - 1);
        List<string> types = TypeInference.InferFieldTypes(rows);

        // SO 클래스 자동 생성 (없으면)
        string classFilePath = Path.Combine("Assets/Scripts/Generated", sheetName + ".cs");
        string assetFolderPath = "Assets/Scripts/Generated";
        if (!File.Exists(classFilePath))
        {
            // 추가 내용을  구글 시트나 기타 데이터 테이블을 기반으로,
            // ScriptableObject 클래스를 자동으로 만들어주기 위한 ""코드 자동 생성기""
            // ""클래스가 없다면 클래스 스크립트를 만들어준다"". 그뒤 코드를 자동으로 생성해주는 기능이다.
            ScriptableObjectTemplateGenerator.GenerateTemplateClass(sheetName, headers, types, assetFolderPath); ScriptableObjectTemplateGenerator.GenerateTemplateClass(sheetName, headers, types, assetFolderPath);
            Debug.Log($"Template for {sheetName} generated. (스크립트 컴파일 후 다시 실행하세요)");
            return;
        }

        // 컴파일 후 SO 타입 찾기
        string assemblyName = typeof(GoogleDTTemplate).Assembly.GetName().Name;
        Type soType = Type.GetType($"{sheetName}, {assemblyName}");
        if (soType == null)
        {
            Debug.LogError($"타입 {sheetName} 을 찾을 수 없습니다. (스크립트 컴파일 후 다시 실행)");
            return;
        }

        // 기존 에셋 중복 검사 (ID 기준, ID는 첫 Colum)
        string saveFolder = $"Assets/Resources/Generated/{sheetName}";
        if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

        string[] existingAssets = AssetDatabase.FindAssets("t:" + sheetName, new[] { saveFolder });
        HashSet<string> existingIds = new HashSet<string>();
        for (int i = 0; i < existingAssets.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(existingAssets[i]);
            ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (so == null) continue;
            var field = soType.GetField(headers[0]);
            if (field != null)
            {
                object val = field.GetValue(so);
                if (val != null)
                    existingIds.Add(val.ToString());
            }
        }

        // 신규 ScriptableObject 에셋 생성
        int addCount = 0;
        for (int i = 0; i < rows.Count; i++)
        {
            string id = rows[i][0];
            if (existingIds.Contains(id)) continue;

            ScriptableObject asset = ScriptableObject.CreateInstance(soType);
            for (int j = 0; j < headers.Count && j < rows.Count; j++)
            {
                var field = soType.GetField(headers[j]);
                if (field == null) continue;
                string val = rows[i][j];
                object parsed;
                if (types[j] == "int")
                {
                    parsed = int.TryParse(val, out int iv) ? iv : 0;
                }
                else if (types[j] == "float")
                {
                    parsed = float.TryParse(val, out float fv) ? fv : 0f;
                }
                else if (types[j] == "bool")
                {
                    string v = val.ToLowerInvariant();
                    parsed = (v == "true" || v == "1");
                }
                else
                {
                    parsed = val;
                }
                field.SetValue(asset, parsed);
            }
            string assetPath = Path.Combine(saveFolder, id + ".asset");
            AssetDatabase.CreateAsset(asset, assetPath);
            addCount++;
        }
        // 프로젝트에 저장되어 언제든 에디터/런타임에서 불러올 수 있는 데이터의 단위로 변환한다.
        /* 변환 이유 :
         * ScriptableObject도 이런 에셋 형태로 저장하면
         * 1. 프로젝트 안에서 자산처럼 관리(버전관리, 리팩토링, 검색) 가능
         * 2. 인스펙터에서 쉽게 확인/수정 가능
         * 3. 런타임에서 쉽게 로드 및 활용 
         * ( Resources.Load, Addressables, AssetBundle 등으로
             빌드 후 게임 실행 중에도 바로 불러와 사용할 수 있음 )
         * 직접 메모리에 생성하는 것보다
           이미 저장된 자산을 읽어오는 게 훨씬 쉽고 관리가 쉬움
         */
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"{sheetName} - 신규 ScriptableObject {addCount}개 생성, 이미 있는 데이터는 건너뜀");
    }
}