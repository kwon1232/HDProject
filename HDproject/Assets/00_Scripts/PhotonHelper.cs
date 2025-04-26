using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonHelper : MonoBehaviour
{
    public static string GetPlayerNickName(int actorNumber)
    {
        if(PhotonNetwork.CurrentRoom != null && 
            PhotonNetwork.CurrentRoom.Players.ContainsKey(actorNumber))
        {
            return PhotonNetwork.CurrentRoom.Players[actorNumber].NickName;
        }

        return "Unknow Player!";
    }

    public static Player GetPlayer(int actionNumber)
    {
        foreach(var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == actionNumber)
                return player;
        }
        return null;
    }
}
