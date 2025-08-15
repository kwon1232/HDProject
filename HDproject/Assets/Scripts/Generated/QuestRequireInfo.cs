using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;


public enum CleareType
{
    None,
    Item,
    Monster,
    TalkToNPC,
    END
}

public class QuestRequireInfo
{
    public int IDX;
    public string Q_RequireIDX;
    public string QuestID;
    public string Condition_KO;
    public int Amount;
    public CleareType cleareType;
    public string TargetID;
    public string InfoNotes_KO;
}


public static class QuestRequireInfoData
{
    public static readonly QuestRequireInfo[] Items = new QuestRequireInfo[]
    {
        new QuestRequireInfo
        {
            IDX = 1,
            Q_RequireIDX = "Q_RE0001",
            QuestID = "QDN0001",
            Condition_KO = "해독제 얻기",
            Amount = 3,
            cleareType = CleareType.Item,
            TargetID = "ITEM_A001",
            InfoNotes_KO = "마을 약사에게 전달 필요",
        },
        new QuestRequireInfo
        {
            IDX = 2,
            Q_RequireIDX = "Q_RE0002",
            QuestID = "QDN0002",
            Condition_KO = "늑대 퇴치",
            Amount = 10,
            cleareType = CleareType.Monster,
            TargetID = "MON0004",
            InfoNotes_KO = "던전 1층 한정",
        },
        new QuestRequireInfo
        {
            IDX = 3,
            Q_RequireIDX = "Q_RE0003",
            QuestID = "QDN0003",
            Condition_KO = "채집(붉은꽃)",
            Amount = 5,
            cleareType = CleareType.Item,
            TargetID = "Item_RedFlower",
            InfoNotes_KO = "강가에서만 가능",
        },
        new QuestRequireInfo
        {
            IDX = 4,
            Q_RequireIDX = "Q_RE0004",
            QuestID = "QDN0004",
            Condition_KO = "늙은 던전지기 NPC와 대화",
            Amount = 1,
            cleareType = CleareType.TalkToNPC,
            TargetID = "NPC0002",
            InfoNotes_KO = "던전 입구 위치",
        },
        new QuestRequireInfo
        {
            IDX = 5,
            Q_RequireIDX = "Q_RE0005",
            QuestID = "QDN0005",
            Condition_KO = "가디언 NPC를 찾은 뒤 대화",
            Amount = 1,
            cleareType = CleareType.TalkToNPC,
            TargetID = "NPC0003",
            InfoNotes_KO = "던전 내부 위치",
        },
    };
}
