using UnityEngine;

public class PlayerSystem
{
    public string _IDX_;
    public string _UIAssetName_;
    public string _Attribute_;
    public string _FolderPath_;
    public string __;
}

public static class PlayerSystemData
{
    public static readonly PlayerSystem[] Items = new PlayerSystem[]
    {
        new PlayerSystem { _IDX_ = "1", _UIAssetName_ = "UIQuest", _Attribute_ = "Quest", _FolderPath_ = "DataStorage/System/Quest", __ = "" },
    };
}
