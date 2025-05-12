using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text dialogueText;
    public Image fallbackUITextPanel;   
    public Image portraitImage;       

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

    private void OnDisable()
    {
        if (localizedText != null)
        {
            localizedText.StringChanged -= UpdateLocalizedText;
        }
    }

    private void UpdateLocalizedText(string translated)
    {
        if (dialogueText != null)
            dialogueText.text = translated;
    }

    /// <summary>
    /// AI 응답이나 수동 대사를 출력 (UI 텍스트만)
    /// </summary>
    public void ShowText(string text, Sprite portrait = null)
    {
        if (dialogueText != null)
        {
            dialogueText.text = text;
        }

        if (fallbackUITextPanel != null)
        {
            fallbackUITextPanel.gameObject.SetActive(true);
        }

        if (portraitImage != null)
        {
            if (portrait != null)
            {
                portraitImage.sprite = portrait;
                portraitImage.gameObject.SetActive(true);
            }
            else
            {
                portraitImage.gameObject.SetActive(false); 
            }
        }
    }
}