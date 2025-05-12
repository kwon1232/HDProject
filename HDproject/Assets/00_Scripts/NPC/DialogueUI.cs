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

    private DialogueUI dialogueUI;


    private string prefabPath = "UI/UIDialogue";

    private DialogueUI CreateUIDialogue()
    {
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            return null;
        }
        GameObject instance = Instantiate(prefab);
        dialogueUI = instance.GetComponent<DialogueUI>();
        return dialogueUI;

    }

    public void Initialize()
    {
        if (dialogueUI == null) this.CreateUIDialogue();

        // 각종 초기화 세팅 여기서 추가
    }

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