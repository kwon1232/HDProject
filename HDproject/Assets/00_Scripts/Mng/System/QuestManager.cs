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

    public Quest(QuestInfo info)
    {
        this.info = info;
        state = QuestState.NotStarted;
    }

    public void Start()
    {
        state = QuestState.InProgress;
    }

    public void Complete()
    {
        state = QuestState.Completed;
    }
}

public class QuestManager : MonoBehaviour
{
    private readonly List<Quest> activeQuests = new List<Quest>();
    private readonly List<Quest> completedQuests = new List<Quest>();

    private void Awake()
    {
        LoadCompletedQuests();
    }

    public IReadOnlyList<Quest> ActiveQuests => activeQuests;
    public IReadOnlyList<Quest> CompletedQuests => completedQuests;

    private void LoadCompletedQuests()
    {
        List<int> ids = SaveSystem.LoadCompletedQuestIDs();
        foreach (int id in ids)
        {
            QuestInfo info = System.Array.Find(DataDirectory.QuestInfoItems, q => q.IDX == id);
            if (info != null)
            {
                var quest = new Quest(info);
                quest.Complete();
                completedQuests.Add(quest);
            }
        }
    }

    private void SaveCompletedQuests()
    {
        var ids = new List<int>();
        foreach (var quest in completedQuests)
        {
            ids.Add(quest.info.IDX);
        }
        SaveSystem.SaveQuests(ids);
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
                foreach (var questInfo in completedQuests)
                {
                    if (questInfo.info.IsLinkedToStory && questInfo.info.StoryID == info.StoryID)
                    {
                        int idx;
                        if (int.TryParse(questInfo.info.StoryIndex, out idx) && idx == currentIndex - 1)
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

        Quest quest = new Quest(info);
        quest.Start();
        activeQuests.Add(quest);
    }

    public void CompleteQuest(int questID)
    {
        Quest quest = activeQuests.Find(q => q.info.IDX == questID);
        if (quest == null)
            return;

        quest.Complete();
        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        SaveCompletedQuests();

        // TODO: reward logic (exp, items, etc.)
        // 플레이어 클래스 가져와서 경험치, 아이템 인벤토리에 추가, 등등 해주기
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