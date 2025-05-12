using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{
    [Range(0.0f, 8.0f)]
    public float yPosFloat = 8.0f;

    [HideInInspector] public Transform target;

    public TMP_Text SpeechText;
    public RectTransform[] contents;

    private Animator animator;
    private Coroutine coroutine;

    private LocalizedString currentLocalizedString;
    private bool localizationBound = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 기존 방식: Photon의 ActorNumber 기반으로 플레이어 찾기
    /// </summary>
    public void Initialize(int actorNumber)
    {
        animator = GetComponent<Animator>();
        target = FindPlayerTransformByActorNumber(actorNumber);
    }

    private Transform FindPlayerTransformByActorNumber(int targetActorNumber)
    {
        PlayerController[] allPlayers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (PlayerController player in allPlayers)
        {
            if (player.OwnerActorNumber == targetActorNumber)
            {
                return player.transform;
            }
        }
        return null;
    }

    /// <summary>
    /// 직접 문자열 출력
    /// </summary>
    public void SetText(string message)
    {
        CleanupLocalization();
        ApplyText(message);
    }

    /// <summary>
    /// LocalizedString을 이용한 다국어 대사 출력
    /// </summary>
    public void SetLocalizedText(LocalizedString localizedString)
    {
        if (localizedString == null)
        {
            Debug.LogWarning("LocalizedString is null.");
            return;
        }

        CleanupLocalization();

        currentLocalizedString = localizedString;
        currentLocalizedString.StringChanged += ApplyText;
        currentLocalizedString.RefreshString();
        localizationBound = true;
    }

    private void ApplyText(string message)
    {
        if (SpeechText != null)
            SpeechText.text = message;

        foreach (var content in contents)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }

        animator.Play("SpeechBubble_Open");

        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(HideCoroutine(3f, null));
    }

    private IEnumerator HideCoroutine(float timer, Action onComplete)
    {
        yield return new WaitForSeconds(timer);
        animator.Play("SpeechBubble_Hide");
        yield return new WaitForSeconds(0.3f);

        if (onComplete != null)
            onComplete.Invoke();
        else
            gameObject.SetActive(false);

        coroutine = null;
    }

    private void CleanupLocalization()
    {
        if (localizationBound && currentLocalizedString != null)
        {
            currentLocalizedString.StringChanged -= ApplyText;
            localizationBound = false;
            currentLocalizedString = null;
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + new Vector3(0.0f, yPosFloat, 0.0f);
            transform.position = Camera.main.WorldToScreenPoint(targetPosition);
        }
    }

    private void OnDisable()
    {
        CleanupLocalization();
    }
}
