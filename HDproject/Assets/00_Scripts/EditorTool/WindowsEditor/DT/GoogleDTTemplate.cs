using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

public static class ScriptableObjectTemplateGenerator
{
    public static void GenerateTemplateClass(string className, List<string> fields, List<string> types, string folder)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("\n[CreateAssetMenu(fileName = \"" + className + "\", menuName = \"Data/" + className + "\")]");
        sb.AppendLine("public class " + className + " : ScriptableObject");
        sb.AppendLine("{");

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

public static class TypeInference
{
    public static List<string> InferFieldTypes(List<List<string>> rows)
    {
        var types = new List<string>();
        if (rows.Count == 0) return types;

        int colCount = rows[0].Count;
        for (int i = 0; i < colCount; i++)
        {
            bool isInt = true, isFloat = true, isBool = true;

            foreach (var row in rows)
            {
                if (i >= row.Count) continue;
                string val = row[i].Trim();
                if (!int.TryParse(val, out _)) isInt = false;
                if (!float.TryParse(val, out _)) isFloat = false;
                if (val != "true" && val != "false") isBool = false;
            }

            if (isInt) types.Add("int");
            else if (isFloat) types.Add("float");
            else if (isBool) types.Add("bool");
            else types.Add("string");
        }
        return types;
    }
}
