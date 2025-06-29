using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public int IDX;
    public string NameID;
    public string Name_KO;
    public string InfoNotesID;
    public string InfoNotes_KO;
}

public static class QuestData
{
    public static readonly Quest[] Items = new Quest[]
    {
        new Quest
        {
            IDX = 1,
            NameID = "QND0001",
            Name_KO = "광산 안의 해독제",
            InfoNotesID = "QIN0001",
            InfoNotes_KO = "광산에서 몬스터 잡고 해독제 3개 얻기",
        },
    };
}
