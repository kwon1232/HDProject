using System.Collections.Generic;
using UnityEngine;

public class Map_MissingMineInAbyssinian
{
    public int IDX;
    public string MapID;
    public string MapName;
    public string NPCIDList;
    public string MonsterList;
}

public static class Map_MissingMineInAbyssinianData
{
    public static readonly Map_MissingMineInAbyssinian[] Items = new Map_MissingMineInAbyssinian[]
    {
        new Map_MissingMineInAbyssinian
        {
            IDX = 1,
            MapID = "RM0001",
            MapName = "DustMine",
            NPCIDList = "NPC0001",
            MonsterList = "MON0001"
        },
        new Map_MissingMineInAbyssinian
        {
            IDX = 2,
            MapID = "RM0001",
            MapName = "DustMine",
            NPCIDList = "NPC0001",
            MonsterList = "MON0002"
        },
        new Map_MissingMineInAbyssinian
        {
            IDX = 3,
            MapID = "RM0001",
            MapName = "DustMine",
            NPCIDList = "NPC0001",
            MonsterList = "MON0003"
        },
    };
}
