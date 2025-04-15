using System;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public enum Action_State
{
    None = 0,
    InviteParty,
    Trade,
    InviteGuild
}

public class ActionHolder : MonoBehaviour
{
    public static SpriteAtlas Atlas;
    public static Dictionary<Action_State, Action> Actions = new Dictionary<Action_State, Action>();

    public static Sprite GetAtlas(string temp)
    {
        return Atlas.GetSprite(temp);
    }

    private void Start()
    {
        Atlas = Resources.Load<SpriteAtlas>("Atlas");
        
        Actions[Action_State.InviteParty] = InviteParty;
        Actions[Action_State.Trade] = Trade;
        Actions[Action_State.InviteParty] = InviteGuild;
    }

    public static void InviteParty()
    {

    }

    public static void Trade()
    {

    }

    public static void InviteGuild()
    {

    }
}
