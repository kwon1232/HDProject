using System.Collections.Generic;
using UnityEngine;

public class NPCInfo
{
    public int IDX;
    public string CharacterID;
    public string NPCID;
    public string CharacterName;
    public string JobType;
    public string RaceType;
    public string HometownType;
    public string AttributeType;
    public string OccupationType_;
    public string PrefabPath;
    public string IconPath;
    public string[] QuestList;
}

public static class NPCInfoData
{
    public static readonly NPCInfo[] Items = new NPCInfo[]
    {
        new NPCInfo
        {
            IDX = 1,
            CharacterID = "CH0001",
            NPCID = "NPC0001",
            CharacterName = "Bertolt",
            JobType = "NPC",
            RaceType = "Human",
            HometownType = "Abyssinian",
            AttributeType = "None",
            OccupationType_ = "Miner",
            PrefabPath = "Character/NPC/NPC",
            IconPath = "Images/NPC/PoisonQuestMinerImage",
            QuestList = new string[] { "QND0001" , "QND0004", "QND0004"},
        },
        new NPCInfo
        {
            IDX = 2,
            CharacterID = "CH0002",
            NPCID = "NPC0002",
            CharacterName = "OldDungeonKeeper",
            JobType = "NPC",
            RaceType = "Human",
            HometownType = "Abyssinian",
            AttributeType = "None",
            OccupationType_ = "DungeonKeeper",
            PrefabPath = "",
            IconPath = "",
            QuestList = new string[] { "QND0002" },
        },
        new NPCInfo
        {
            IDX = 3,
            CharacterID = "CH0003",
            NPCID = "NPC0003",
            CharacterName = "Guardian",
            JobType = "NPC",
            RaceType = "Human",
            HometownType = "Abyssinian",
            AttributeType = "None",
            OccupationType_ = "Kid",
            PrefabPath = "",
            IconPath = "",
            QuestList = new string[] { "QIN0005" }
        },
    };
}
