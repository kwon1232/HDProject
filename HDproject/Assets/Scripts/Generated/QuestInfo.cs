using System.Collections.Generic;
using UnityEngine;

public class QuestInfo
{
    public int IDX;
    public string NameID;
    public string Name_KO;
    public string InfoNotesID;
    public string InfoNotes_KO;
    public string Type;
    public string Difficulty;
    public bool IsLinkedToStory;
    public string StoryID;
    public string StoryIndex;
    public string Region;
    public string MapName;
    public int RewardExp;
    public string RewardItem1;
    public string RewardItem2;
    public string RewardItem3;
    public int RewardGold;
}

public static class QuestInfoData
{
    public static readonly QuestInfo[] Items = new QuestInfo[]
    {
        new QuestInfo
        {
            IDX = 1,
            NameID = "QND0001",
            Name_KO = "광산 안의 해독제",
            InfoNotesID = "QIN0001",
            InfoNotes_KO = "광산에서 몬스터 잡고 해독제 3개 얻기",
            Type = "Main",
            Difficulty = "Normal",
            IsLinkedToStory = true,
            StoryID = "Story001",
            StoryIndex = "1",
            Region = "",
            MapName = "",
            RewardExp = 50,
            RewardItem1 = "Item_Haedok",
            RewardItem2 = "",
            RewardItem3 = "",
            RewardGold = 50,
        },
        new QuestInfo
        {
            IDX = 2,
            NameID = "QND0002",
            Name_KO = "초원의 수호자",
            InfoNotesID = "QIN0002",
            InfoNotes_KO = "초원에서 늑대 10마리 퇴치",
            Type = "Sub",
            Difficulty = "Easy",
            IsLinkedToStory = false,
            StoryID = "",
            StoryIndex = "",
            Region = "",
            MapName = "ContinentA",
            RewardExp = 60,
            RewardItem1 = "",
            RewardItem2 = "Item_WolfFur",
            RewardItem3 = "Item_HealingHerb",
            RewardGold = 60,
        },
        new QuestInfo
        {
            IDX = 3,
            NameID = "QND0003",
            Name_KO = "마을의 데일리 심부름",
            InfoNotesID = "QIN0003",
            InfoNotes_KO = "마을 사람의 부탁 1회 수행",
            Type = "Daily",
            Difficulty = "Easy",
            IsLinkedToStory = false,
            StoryID = "",
            StoryIndex = "",
            Region = "",
            MapName = "ContinentB",
            RewardExp = 120,
            RewardItem1 = "",
            RewardItem2 = "Item_Bread",
            RewardItem3 = "",
            RewardGold = 150,
        },
        new QuestInfo
        {
            IDX = 4,
            NameID = "QND0004",
            Name_KO = "잃어버린 가디언",
            InfoNotesID = "QIN0004",
            InfoNotes_KO = "광산에서 해독제를 구한 후 던전 입구로 이동하세요",
            Type = "Main",
            Difficulty = "Normal",
            IsLinkedToStory = true,
            StoryID = "Story001",
            StoryIndex = "2",
            Region = "",
            MapName = "",
            RewardExp = 1000,
            RewardItem1 = "Item_DungeonKey",
            RewardItem2 = "Item_Torch",
            RewardItem3 = "",
            RewardGold = 1000,
        },
    };
}
