using UnityEngine;

public class UIAssetInfo
{
    public string _IDX_;
    public string _SystemType_;
    public string _Attribute_;
    public string _PrefabPath_;
    public string _UIAssetName_;
}

public static class UIAssetInfoData
{
    public static readonly UIAssetInfo[] Items = new UIAssetInfo[]
    {
        new UIAssetInfo { _IDX_ = "1", _SystemType_ = "SystemNPC", _Attribute_ = "Dialogue", _PrefabPath_ = "Prefabs/UI/UIDialogue", _UIAssetName_ = "UIDialogue" },
    };
}
