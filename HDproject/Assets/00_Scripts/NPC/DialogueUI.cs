using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;



public class DialogueUI : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text dialogueText;
    public GameObject fallbackUITextPanel; 
    public SpeechBubble speechBubblePrefab; 

    [Header("Localization (Optional)")]
    public LocalizedString localizedText;

    private void OnEnable()
    {
        if (localizedText != null && dialogueText != null)
        {
            localizedText.StringChanged += UpdateLocalizedText;
            localizedText.RefreshString();
        }
    }

    private void UpdateLocalizedText(string translated)
    {
        if (dialogueText != null)
            dialogueText.text = translated;
    }

    /// <summary>
    /// AI 응답이나 수동 대사를 출력 (말풍선 아님)
    /// </summary>
    public void ShowText(string text)
    {
        if (dialogueText != null)
        {
            dialogueText.text = text;
        }

        if (fallbackUITextPanel != null)
        {
            fallbackUITextPanel.SetActive(true);
        }
    }

    /// <summary>
    /// NPC 머리 위에 말풍선으로 텍스트 출력
    /// </summary>
    public void ShowSpeechBubble(string text, Transform npcTransform)
    {
        if (speechBubblePrefab == null || npcTransform == null)
        {
            Debug.LogWarning("SpeechBubblePrefab 또는 NPC Transform이 설정되지 않았습니다.");
            return;
        }

        SpeechBubble bubble = Instantiate(speechBubblePrefab, transform); // Canvas 아래 생성
        bubble.target = npcTransform;
        bubble.SetText(text);
    }
}
