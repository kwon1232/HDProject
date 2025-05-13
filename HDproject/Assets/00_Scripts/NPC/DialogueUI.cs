using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("UI 구성 요소")]    
    public TMP_Text dialogueText;    
    [SerializeField] private Image fallbackUITextPanel;    
    [SerializeField] private Image portraitImage;    
    [SerializeField] private Button closeButton;
    [SerializeField] private Button NextButton;

    [Header("Localization (선택)")]
    public LocalizedString localizedText;

    private static readonly string prefabPath = "Prefabs/UI/UIDialogue";
    public static DialogueUI instance { get; private set; }    


    private string[] lines;       
    private int currentIndex;     
    private string currentPortraitPath;    // 현재 사용할 초상화 리소스 경로를 저장

    /// <summary>
    /// 외부에서 대사 배열과 초상화 경로를 전달받아 대화를 시작
    /// </summary>
    public void PlayDialogue(string[] texts, string portraitPath = null)
    {
        lines = texts;    
        currentPortraitPath = portraitPath;    
        currentIndex = 0;    
        ShowLine();    // 첫 번째 대사
        gameObject.SetActive(true);    
    }

    private void OnNextLine()
    {
        currentIndex++;    // 인덱스를 증가
        if (currentIndex < lines.Length)
        {
            ShowLine();    // 다음 대사를 표시
        }
    }

    /// <summary>
    /// 현재 인덱스에 해당하는 대사를 화면에 출력
    /// </summary>
    public void ShowLine()
    {
        if (dialogueText != null)
            dialogueText.text = lines[currentIndex];    

        if (fallbackUITextPanel != null)
            fallbackUITextPanel.gameObject.SetActive(true);    

        if (!string.IsNullOrEmpty(currentPortraitPath) && portraitImage != null)
        {
            Sprite sprite = Resources.Load<Sprite>(currentPortraitPath);    
            if (sprite != null)
            {
                portraitImage.sprite = sprite;    
  
            }
            portraitImage.gameObject.SetActive(sprite != null); 
        }
    }

    /// <summary>
    /// Resources에서 대화 UI 프리팹을 로드하여 생성하고 캔버스 하위에 배치
    /// </summary>
    public static DialogueUI CreateUIDialogue()
    {
        GameObject prefab = Resources.Load<GameObject>(prefabPath);   
        if (prefab == null)
        {
            Debug.LogWarning($"Resources/{prefabPath} 경로에서 DialogueUI 프리팹을 찾을 수 없음");
            return null;   
        }

        GameObject uiInstance = Instantiate(prefab);    
        DialogueUI createdUI = uiInstance.GetComponent<DialogueUI>();   

        Canvas canvas = UIManager.GetCurrentSceneCanvas();   
        UIManager.SetUIParent(
            parent: canvas.gameObject,
            child: uiInstance,
            childLocalPos: Vector2.zero,
            anchorMin: new Vector2(0.5f, 0.5f),
            anchorMax: new Vector2(0.5f, 0.5f),
            anchoredPos: Vector2.zero,
            localScale: Vector3.one,
            localRotation: Quaternion.identity
        );    // 캔버스 하위로 배치

        if (createdUI != null)
        {
            instance = createdUI;    
            createdUI.Initialize();  
        }
        return createdUI;   
    }

    /// <summary>
    /// 버튼 클릭 이벤트 연결, 로컬라이즈 이벤트 구독 등을 초기화
    /// </summary>
    public void Initialize()
    {
        if (localizedText != null && dialogueText != null)
        {
            localizedText.StringChanged += UpdateLocalizedText;    // 로컬라이즈된 텍스트 변경 이벤트를 구독
            localizedText.RefreshString();    // 초기 문자열을 갱신
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();    // 기존 리스너를 제거
            closeButton.onClick.AddListener(OnNextLine);    // 다음 버튼 클릭 시 OnNextLine을 호출
        }

        Close();    
    }

    private void OnDisable()
    {
        // 이벤트 구독을 해제함
        if (localizedText != null)
            localizedText.StringChanged -= UpdateLocalizedText;    
    }

    private void UpdateLocalizedText(string translated)
    {
        if (dialogueText != null)
            dialogueText.text = translated;    
    }

    public void Close()
    {
        gameObject.SetActive(false);    
    }
}
