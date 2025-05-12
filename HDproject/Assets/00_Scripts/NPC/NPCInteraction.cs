using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    // 엑셀 만들고 DB로 따로 빼서 캐릭터 타입에 따라 나누기 현재는 기능 프로토 타입
    public string concept = "너는 중독되어 괴로워하는 NPC야. 독에 중독되어 말할 때마다 고통스러워하고, 숨이 차. 하지만 간신히 플레이어에게 부탁하려 해. 상황은 심각하고 절박해. 말투는 헐떡이며 짧게 끊기는 느낌.";
    public string prompt = "플레이어가 다가왔어. 말을 걸어, 너는 퀘스트안내  NPC야, 퀘스트 안내를 해줘 퀘스트 내용은 몬스터를 처치해서 해독제 2개를 구해오는 거야 이 내용을 말 걸 때마다 컨셉에 맞게 말해줘";
    public LocalLLMChatManager chatManager;
    public DialogueUI dialogueUI;

    private SpeechBubble activeBubble;

    private void OnMouseDown()
    {
        StartCoroutine(chatManager.GetChatResponse(concept, prompt, response =>
        {
            if(activeBubble == null)
            {
                activeBubble = Instantiate(dialogueUI.speechBubblePrefab, dialogueUI.transform);
                activeBubble.target = this.transform;
            }
            activeBubble.gameObject.SetActive(true);
            activeBubble.SetText(response);
        }));
    }
}
