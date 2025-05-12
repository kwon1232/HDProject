using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text dialogueText;
    [SerializeField] private Image fallbackUITextPanel;
    [SerializeField] private Image portraitImage;
    [SerializeField] private Button closeButton;

    [Header("Localization (Optional)")]
    public LocalizedString localizedText;

    
    private static readonly string prefabPath = "UI/UIDialogue";

    public static DialogueUI instance { get; private set; }

    /// <summary>
    /// Resources에서 UIDialogue 프리팹을 불러와 생성
    /// </summary>
    public static DialogueUI CreateUIDialogue()
    {
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogWarning($"DialogueUI 프리팹을 찾을 수 없습니다: Resources/{prefabPath}");
            return null;
        }

        GameObject uiInstance = Instantiate(prefab);
        DialogueUI createdUI = uiInstance.GetComponent<DialogueUI>();
        if (createdUI != null)
        {
            instance = createdUI;
            createdUI.Initialize();
        }
        return createdUI;
    }

    public void Initialize()
    {
        // 초기화 로직
        if (localizedText != null && dialogueText != null)
        {
            localizedText.StringChanged += UpdateLocalizedText;
            localizedText.RefreshString();
        }

        if (closeButton != null && dialogueText != null)
        {
            localizedText.StringChanged += UpdateLocalizedText;
            localizedText.RefreshString();
        }

        if(closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
        }
        Close(); // 시작 시 비활성화
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

    public void ShowText(string text, string portrait = null)
    {
        if (dialogueText != null)
            dialogueText.text = text;

        if (fallbackUITextPanel != null)
            fallbackUITextPanel.gameObject.SetActive(true);

        if (portraitImage != null)
        {
            Sprite loadedSprite = Resources.Load<Sprite>(portrait);
            portraitImage.gameObject.SetActive(portrait != null);
        }

        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
