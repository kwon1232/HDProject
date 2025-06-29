using System.Collections.Generic;
using UnityEngine;

public class Dialogue
{
    public int IDX;
    public int NPCInfoIDX;
    public string MapID;
}

public static class DialogueData
{
    public static readonly Dialogue[] Items = new Dialogue[]
    {
        new Dialogue
        {
            IDX = 1,
            NPCInfoIDX = 1,
            MapID = "ABMissingMine"
        },
    };
}
