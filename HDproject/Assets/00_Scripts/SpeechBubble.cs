using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{
    [Range(0.0f, 8.0f)]
    public float yPosFloat = 8.0f;

    [HideInInspector] public Transform target;

    public Text SpeechText;
    public RectTransform[] contents;
    private Animator animator;
    private Coroutine coroutine;

    public void Initialize(int actorNumber)
    {
        animator = GetComponent<Animator>();
        target = FindPlayerTransformByActorNumber(actorNumber);
    }


    private Transform FindPlayerTransformByActorNumber(int targetActorNumber)
    {
        PlayerController[] allPlayers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        foreach(PlayerController player in allPlayers)
        {
            if(player.OwnerActorNumber == targetActorNumber)
            {
                return player.transform;
            }
        }

        return null;
    }

    public void SetText(string message)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = StartCoroutine(HideCoroutine(0f, () =>
            {
                SpeechText.text = message;
                animator.Play("SpeechBubble_Open");
                coroutine = StartCoroutine(HideCoroutine(3.0f, null));
            }));
            return;
        } 
        SpeechText.text = message;
        for (int i = 0; i < contents.Length; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contents[i]);
        }
        animator.Play("SpeechBubble_Open");
        coroutine = StartCoroutine(HideCoroutine(3f, null));
    }

    IEnumerator HideCoroutine(float timer, Action action)
    {
        yield return new WaitForSeconds(timer);
        animator.Play("SpeechBubble_Hide");
        yield return new WaitForSeconds(0.3f);
        if (action != null)
        {
            action?.Invoke();
            yield break;
        }
        else gameObject.SetActive(false);

        coroutine = null;
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
