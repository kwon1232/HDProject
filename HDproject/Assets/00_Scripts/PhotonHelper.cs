using Photon.Pun;
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
}
