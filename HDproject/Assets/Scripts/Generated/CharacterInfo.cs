using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo
{
    public int IDX;
    public string CharacterID;
    public string CharacterType;
    public string CharacterTypeParam;
    public string PrefabPath;
    public string IconPath;
}

public static class CharacterInfoData
{
    public static readonly CharacterInfo[] Items = new CharacterInfo[]
    {
        new CharacterInfo
        {
            IDX = 1,
            CharacterID = "CH0001",
            CharacterType = "NPC",
            CharacterTypeParam = "NPC0001",
            PrefabPath = "",
            IconPath = "",
        },
        new CharacterInfo
        {
            IDX = 2,
            CharacterID = "CH0002",
            CharacterType = "Monster",
            CharacterTypeParam = "MON0001",
            PrefabPath = "",
            IconPath = "",
        },
        new CharacterInfo
        {
            IDX = 3,
            CharacterID = "CH0003",
            CharacterType = "Monster",
            CharacterTypeParam = "MON0002",
            PrefabPath = "",
            IconPath = "",
        },
        new CharacterInfo
        {
            IDX = 4,
            CharacterID = "CH0004",
            CharacterType = "Monster",
            CharacterTypeParam = "MON0003",
            PrefabPath = "",
            IconPath = "",
        },
        new CharacterInfo
        {
            IDX = 5,
            CharacterID = "CH0005",
            CharacterType = "Monster",
            CharacterTypeParam = "MON0004",
            PrefabPath = "",
            IconPath = "",
        },
        new CharacterInfo
        {
            IDX = 6,
            CharacterID = "CH0006",
            CharacterType = "NPC",
            CharacterTypeParam = "NPC0002",
            PrefabPath = "",
            IconPath = "",
        },
        new CharacterInfo
        {
            IDX = 7,
            CharacterID = "CH0007",
            CharacterType = "NPC",
            CharacterTypeParam = "NPC0003",
            PrefabPath = "",
            IconPath = "",
        },
    };
}
