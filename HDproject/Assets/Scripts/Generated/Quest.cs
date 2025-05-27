using UnityEngine;

public class Quest
{
    public string _IDX_;
    public string _NameID_;
    public string _Name_KO_;
    public string _InfoNotesID_;
    public string _InfoNotes_KO_;
}

public static class QuestData
{
    public static readonly Quest[] Items = new Quest[]
    {
        new Quest { _IDX_ = "1", _NameID_ = "QND0001", _Name_KO_ = "광산 안의 해독제", _InfoNotesID_ = "QIN0001", _InfoNotes_KO_ = "광산에서 몬스터 잡고 해독제 3개 얻기" },
    };
}
