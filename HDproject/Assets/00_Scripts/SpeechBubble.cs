using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{
    [Range(0.0f, 8.0f)]
    public float yPosFloat = 8.0f;

    [HideInInspector] public Transform target;

    public Text SpeechText;

    public void Initialize(int actorNumber)
    {
        Player targetPlayer = PhotonNetwork.PlayerList.FirstOrDefault(p => p.ActorNumber == actorNumber);
        if(targetPlayer != null)
        {
            target = ((GameObject)targetPlayer.TagObject).transform;
        }
    }

    public void SetText(string message)
    {
        SpeechText.text = message;
    }

    private void LateUpdate()
    {
        if(target != null)
        {
            Vector3 targetPosition = target.position + new Vector3(0.0f, yPosFloat, 0.0f);
            transform.position = Camera.main.WorldToScreenPoint(targetPosition);
        }
    }
}
