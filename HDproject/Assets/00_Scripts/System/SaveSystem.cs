using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[Serializable]
public class QuestSaveData
{
    public List<int> completedQuestIDs = new List<int>();
    public List<ActiveQuestSaveInfo> activeQuests = new List<ActiveQuestSaveInfo>();
}

[Serializable]
public class ActiveQuestSaveInfo
{
    public int questID;
    public List<RequirementProgress> requirements = new List<RequirementProgress>();
}

[Serializable]
public class RequirementProgress
{
    public int requirementID;
    public int progress;
}


public static class SaveSystem
{
    private static readonly string saveDir = Path.Combine(Application.persistentDataPath, "save");
    private static readonly string saveFile = Path.Combine(saveDir, "quest.json");

    public static void SaveQuests(List<int> completedQuestIDs, List<ActiveQuestSaveInfo> activeQuests)
    {
        var data = new QuestSaveData
        {
            completedQuestIDs = completedQuestIDs,
            activeQuests = activeQuests
        };
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

    public static QuestSaveData LoadQuestData()
    {
        if (!File.Exists(saveFile))
            return new QuestSaveData();

        try
        {
            string json = File.ReadAllText(saveFile);
            var data = JsonUtility.FromJson<QuestSaveData>(json);
            if (data != null)
                return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load quests: {e.Message}");
        }
        return new QuestSaveData();
    }
}