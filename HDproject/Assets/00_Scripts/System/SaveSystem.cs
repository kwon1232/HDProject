using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[Serializable]
public class QuestSaveData
{
    public List<int> completedQuestIDs = new List<int>();
}

public static class SaveSystem
{
    private static readonly string saveDir = Path.Combine(Application.persistentDataPath, "save");
    private static readonly string saveFile = Path.Combine(saveDir, "quest.json");

    public static void SaveQuests(List<int> completedQuestIDs)
    {
        QuestSaveData data = new QuestSaveData { completedQuestIDs = completedQuestIDs };

        string json = JsonUtility.ToJson(data);
        try
        {
            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);
            File.WriteAllText(saveFile, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save quests: {e.Message}");
        }
    }

    public static List<int> LoadCompletedQuestIDs()
    {
        if(!File.Exists(saveFile))
        {
            return new List<int>();
        }

        try
        {
            string json = File.ReadAllText(saveFile);
            var data = JsonUtility.FromJson<QuestSaveData>(json);
            if (data != null && data.completedQuestIDs != null)
                return data.completedQuestIDs;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load quests: {e.Message}");
        }
        return new List<int>();
    }
}