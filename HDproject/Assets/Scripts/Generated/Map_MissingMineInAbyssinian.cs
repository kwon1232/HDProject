using UnityEngine;

public class Map_MissingMineInAbyssinian
{
    public string _IDX_;
    public string _MapID_;
    public string _MapName_;
    public string _NPCIDList_;
    public string _MonsterList_;
}

public static class Map_MissingMineInAbyssinianData
{
    public static readonly Map_MissingMineInAbyssinian[] Items = new Map_MissingMineInAbyssinian[]
    {
        new Map_MissingMineInAbyssinian { _IDX_ = "1", _MapID_ = "RM0001", _MapName_ = "DustMine", _NPCIDList_ = "NPC0001", _MonsterList_ = "MON0001" },
        new Map_MissingMineInAbyssinian { _IDX_ = "2", _MapID_ = "RM0001", _MapName_ = "DustMine", _NPCIDList_ = "NPC0001", _MonsterList_ = "MON0002" },
        new Map_MissingMineInAbyssinian { _IDX_ = "3", _MapID_ = "RM0001", _MapName_ = "DustMine", _NPCIDList_ = "NPC0001", _MonsterList_ = "MON0003" },
    };
}
