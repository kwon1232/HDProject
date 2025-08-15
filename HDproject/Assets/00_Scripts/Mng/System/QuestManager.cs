using System;
using System.Collections.Generic;
using UnityEngine;

public enum QuestState
{
    NotStarted,
    InProgress,
    Completed
}

public class Quest
{
    public QuestInfo info { get; private set; }
    public QuestState state { get; private set; }
    public Dictionary<int, int> Progress { get; private set; }

    public Quest(QuestInfo info, List<QuestRequireInfo> requirements)
    {
        this.info = info;
        Progress = new Dictionary<int, int>();
        foreach (var req in requirements)
        {
            if (!Progress.ContainsKey(req.IDX))
                Progress.Add(req.IDX, 0);
        }
        state = QuestState.NotStarted;
    }

    public void Start()
    {
        state = QuestState.InProgress;
    }

    public void SetRequirementProgress(int requirementID, int value)
    {
        if (Progress.ContainsKey(requirementID))
            Progress[requirementID] = value;
    }

    public void Complete()
    {
        state = QuestState.Completed;
    }
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance { get; private set; }
    private readonly List<Quest> activeQuests = new List<Quest>();
    private readonly List<Quest> completedQuests = new List<Quest>();

    private void Awake()
    {
        // 싱글톤 패턴 인스턴스가 이미 존재하면 중복 생성 방지
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        LoadQuests();
    }

    public IReadOnlyList<Quest> ActiveQuests => activeQuests;
    public IReadOnlyList<Quest> CompletedQuests => completedQuests;

    private static string NormalizeQuestID(string nameId)
    {
        string digits = "";
        for (int i = 0; i < nameId.Length; i++)
        {
            char c = nameId[i];
            if (c >= '0' && c <= '9')
                digits += c;
        }
        int num = 0;
        int.TryParse(digits, out num);
        if (num < 0) num = 0;
        string qid = "QND" + num.ToString("D4");
        return qid;
    }

    private List<QuestRequireInfo> GetRequirementsForQuest(QuestInfo info)
    {
        string qid = NormalizeQuestID(info.NameID);
        var list = new List<QuestRequireInfo>();
        var items = DataDirectory.QuestRequireInfoItems;
        for (int i = 0; i < items.Length; i++)
        {
            var r = items[i];
            if (r.QuestID == qid)
                list.Add(r);
        }
        return list;
    }

    private void LoadQuests()
    {
        QuestSaveData data = SaveSystem.LoadQuestData();
        foreach (int id in data.completedQuestIDs)
        {
            QuestInfo info = System.Array.Find(DataDirectory.QuestInfoItems, q => q.IDX == id);
            if (info != null)
            {
                var reqs = GetRequirementsForQuest(info);
                var quest = new Quest(info, reqs);
                quest.Complete();
                completedQuests.Add(quest);
            }
        }

        foreach (var saved in data.activeQuests)
        {
            QuestInfo info = System.Array.Find(DataDirectory.QuestInfoItems, q => q.IDX == saved.questID);
            if (info != null)
            {
                var reqs = GetRequirementsForQuest(info);
                var quest = new Quest(info, reqs);
                for (int i = 0; i < saved.requirements.Count; i++)
                {
                    var rp = saved.requirements[i];
                    quest.SetRequirementProgress(rp.requirementID, rp.progress);
                }
                quest.Start();
                activeQuests.Add(quest);
            }
        }
    }

    private void SaveQuests()
    {
        var completedIDs = new List<int>();
        foreach (var quest in completedQuests)
        {
            completedIDs.Add(quest.info.IDX);
        }

        var activeInfos = new List<ActiveQuestSaveInfo>();
        foreach (var quest in activeQuests)
        {
            var info = new ActiveQuestSaveInfo();
            info.questID = quest.info.IDX;
            foreach (var kv in quest.Progress)
            {
                var rp = new RequirementProgress();
                rp.requirementID = kv.Key;
                rp.progress = kv.Value;
                info.requirements.Add(rp);
            }
            activeInfos.Add(info);
        }

        SaveSystem.SaveQuests(completedIDs, activeInfos);
    }

    public void AcceptQuest(int questID)
    {
        QuestInfo info = System.Array.Find(DataDirectory.QuestInfoItems, q => q.IDX == questID);
        if (info == null)
        {
            Debug.LogWarning($"Quest {questID} not found");
            return;
        }

        if (activeQuests.Exists(q => q.info.IDX == questID) ||
            completedQuests.Exists(q => q.info.IDX == questID))
            return;

        if (info.IsLinkedToStory)
        {
            int currentIndex = 0;
            int.TryParse(info.StoryIndex, out currentIndex);
            if (currentIndex > 1)
            {
                bool prevCompleted = false;
                foreach (var quest in completedQuests)
                {
                    if (quest.info.IsLinkedToStory && quest.info.StoryID == info.StoryID)
                    {
                        int idx;
                        if (int.TryParse(quest.info.StoryIndex, out idx) && idx == currentIndex - 1)
                        {
                            prevCompleted = true;
                            break;
                        }
                    }
                }

                if (!prevCompleted)
                {
                    Debug.Log($"Previous story quest not completed. Cannot accept {questID}");
                    return;
                }
            }
        }

        var reqsForQuest = GetRequirementsForQuest(info);
        Quest Newquest = new Quest(info, reqsForQuest);
        Newquest.Start();
        activeQuests.Add(Newquest);
        SaveQuests();
    }

    public bool IsQuestCleared(int questID)
    {
        return false;
    }

    public void CompleteQuest(int questID)
    {
        Quest quest = activeQuests.Find(q => q.info.IDX == questID);
        if (quest == null)
            return;

        quest.Complete();
        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        SaveQuests();

        // TODO: reward logic (exp, items, etc.)
    }

    public void UpdateQuestProgress(int questID, int requirementID, int progress)
    {
        Quest quest = activeQuests.Find(q => q.info.IDX == questID);
        if (quest == null)
            return;
        quest.SetRequirementProgress(requirementID, progress);
        SaveQuests();
    }

    public static void PrintAllQuestInfo()
    {
        Debug.Log("IDX\tNameID\tName_KO\tInfoNotesID\tInfoNotes_KO\tType\tDifficulty\tIsLinkedToStory\tStoryID\tStoryIndex\tRegion\tMapName\tRewardExp\tRewardItem1\tRewardItem2\tRewardItem3\tRewardGold");
        foreach (var info in DataDirectory.QuestInfoItems)
        {
            string line = string.Join("\t", new string[]
            {
                info.IDX.ToString(),
                info.NameID,
                info.Name_KO,
                info.InfoNotesID,
                info.InfoNotes_KO,
                info.Type,
                info.Difficulty,
                info.IsLinkedToStory.ToString(),
                info.StoryID,
                info.StoryIndex,
                info.Region,
                info.MapName,
                info.RewardExp.ToString(),
                info.RewardItem1,
                info.RewardItem2,
                info.RewardItem3,
                info.RewardGold.ToString()
            });
            Debug.Log(line);
        }
    }
}