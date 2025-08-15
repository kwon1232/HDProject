using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.Localization;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("UI 구성 요소")]    
    public TMP_Text dialogueText;
    [SerializeField] private TextMeshProUGUI npcName;
    [SerializeField] private Image fallbackUITextPanel;    
    [SerializeField] private Image portraitImage;    
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button yesBtn, noBtn;

    [SerializeField] public int questID;

    private const int answerMaxCount = 3;

    [Header("Localization (선택)")]
    public LocalizedString localizedText;

    private static readonly string prefabPath = "Prefabs/UI/UIDialogue";
    public static DialogueUI instance { get; private set; }    


    private string[] lines;       
    private int currentIndex;     
    private string currentPortraitPath;    // 현재 사용할 초상화 리소스 경로를 저장

    void Start()
    {
        // 버튼 클릭 시 AcceptQuest 호출
        yesBtn.onClick.AddListener(OnYesBtnClicked);
    }


    /// <summary>
    /// Resources에서 대화 UI 프리팹을 로드하여 생성하고 캔버스 하위에 배치
    /// </summary>
    public static DialogueUI CreateUIDialogue()
    {
        GameObject UIprefab = Resources.Load<GameObject>(prefabPath);
        if (UIprefab == null)
        {
            Debug.LogWarning($"Resources/{prefabPath} 경로에서 DialogueUI 프리팹을 찾을 수 없음");
            return null;
        }

        GameObject uiInstance = Instantiate(UIprefab);
        DialogueUI createdUI = uiInstance.GetComponent<DialogueUI>();

        float offsetY = 150;

        Canvas canvas = UIManager.GetCurrentSceneCanvas();
        UIManager.SetUIParent(
            parent: canvas.gameObject,
            child: uiInstance,
            childLocalPos: null,
            anchorMin: new Vector2(0.5f, 0.5f),
            anchorMax: new Vector2(0.5f, 0.5f),
            anchoredPos: new Vector2(0, offsetY),
            localScale: Vector3.one,
            localRotation: Quaternion.identity
        );    // 캔버스 중앙 + offsetY로 배치

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

        if (closeBtn != null)
        {
            closeBtn.onClick.RemoveAllListeners();    // 기존 리스너를 제거
            closeBtn.onClick.AddListener(OnNextLine);    // 다음 버튼 클릭 시 OnNextLine을 호출
        }

        if (yesBtn != null && noBtn != null)
        {
            // TODO : DT 로 바꾸기
            yesBtn.image.sprite = Resources.Load<Sprite>("Images/UI/btn/Dialogue/YesBtn");
            yesBtn.image.enabled = false;
            yesBtn.gameObject.SetActive(false);

            noBtn.image.sprite = Resources.Load<Sprite>("Images/UI/btn/Dialogue/NoBtn");
            noBtn.image.enabled = false;
            noBtn.gameObject.SetActive(false);
        }

        Close();
    }

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

    public void OnNextLine()
    {
        if (currentIndex < lines.Length-1)
        {
            currentIndex++;    // 인덱스를 증가
            if(lines.Length-1 == currentIndex)
            {
                yesBtn.gameObject.SetActive(true);
                noBtn.gameObject.SetActive(true);
                nextBtn.gameObject.SetActive(false);
            }
            ShowLine();    // 다음 대사를 표시
        }
    }

    /// <summary>
    /// 현재 인덱스에 해당하는 대사를 화면에 출력
    /// </summary>
    private void ShowLine()
    {
        if (dialogueText != null)
            dialogueText.text = lines[currentIndex];    

        if (fallbackUITextPanel != null)
            fallbackUITextPanel.gameObject.SetActive(true);

        if (!string.IsNullOrEmpty(currentPortraitPath) && portraitImage != null)
        {
            Sprite portraitSprite = Resources.Load<Sprite>(currentPortraitPath);
            if (portraitSprite != null)
            {
                // 초상화 스프라이트가 경로에 있다면 사진을 넣고 보여줌.
                portraitImage.sprite = portraitSprite;
                portraitImage.gameObject.SetActive(true); 
            }
            else
            {
                // 초상화 스프라이트가 경로에 없다면 보여주지 않고 공란으로 남김
                portraitImage.gameObject.SetActive(false);
            }
        }
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

    private void OnYesBtnClicked()
    {
        QuestManager.instance.AcceptQuest(questID);
        // 버튼 비활성화 등 추가 동작
        yesBtn.interactable = false;
    }

    public void Close()
    {
        gameObject.SetActive(false);    
    }
}
