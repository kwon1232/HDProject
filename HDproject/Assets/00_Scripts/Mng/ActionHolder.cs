using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public enum Action_State
{
    None = 0,
    InviteParty,
    Trade,
    InviteGuild
}

public class ActionHolder : MonoBehaviourPunCallbacks
{
    public static SpriteAtlas Atlas;
    public static Dictionary<Action_State, Action> Actions = new Dictionary<Action_State, Action>();
    public static PhotonView photonView;
    public static int TargetPlayerIndex;

    public static Sprite GetAtlas(string temp)
    {
        return Atlas.GetSprite(temp);
    }

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        Atlas = Resources.Load<SpriteAtlas>("Atlas");

        Actions[Action_State.InviteParty] = InviteParty;
        Actions[Action_State.Trade] = Trade;
        Actions[Action_State.InviteGuild] = InviteGuild;
    }

    // RPC 기능 사용할 것
    public static void InviteParty()
    {
        photonView.RPC("ReceivePartyInvite", RpcTarget.Others, 
            PhotonNetwork.LocalPlayer.ActorNumber, TargetPlayerIndex);
    }

    [PunRPC]
    public void ReceivePartyInvite(int inviterID, int targetPlayerID)
    {
        string temp = string.Format("<color=#FFF000>{0}</color>님이 " +
            "파티를 초대하셨습니다.\n수락하시겠습니까?", 
            PhotonHelper.GetPlayerNickName(inviterID));

        PopUPManager.instance.Initialize(temp, null, null);
    }

    public static void Trade()
    {

    }

    public static void InviteGuild()
    {

    }

}
