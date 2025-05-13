using UnityEngine;
using UnityEngine.EventSystems;

public class NPCInteraction : MonoBehaviour
{
    public static NPCInteraction instance { get; private set; }

    [TextArea(6, 12)]
    public string concept = @"
Name: Bertolt
Status: A former miner who once toiled deep underground.
Background: Sacrificed in an alchemist’s forbidden experiment and now poisoned by a deadly toxin. Every time he speaks, his whole body trembles and he gasps as if his lungs are aflame. He begs his comrade—the player—for an antidote.
Appearance: His skin is pale and covered in toxic rashes, shoulders slumped as if all strength has drained away.
Voice: Each sentence is cut short by ragged breaths and sighs. He speaks desperately yet resolutely, but never more than three sentences at a time.
Goal: With his last ounce of strength, he pleads with the player to defeat monsters and bring back two antidotes.
".Trim();

    [TextArea(6, 12)]
    public string prompt = @"
Keep the Bertolt character:
1) Speak in two sentences or less.
2) Keep the miner's desperate yet tacit determination.
3) Deliver the quest: 'Defeat the Monster and bring two antidotes.'
4) Always call the player a 'friend'.
5) Tag [start NPC descript] before each NPC line.
6) Please add the [end NPC descript] tag after each NPC line.
7) Don't include any internal thoughts, analyses, or <thinking> blocks, just output the NPC's voice lines between the start/end tags.
8) Just print out the NPC's lines, skip the player lines or narrations.
9) Each NPC dialog line should consist of a maximum of 5 sentences.
10) **Do not print the tag 'Start NPC Description' or 'End NPC Description' only returns the voice lines themselves.**.
".Trim();

    [SerializeField] private LayerMask npcLayerMask;
    [SerializeField] private LocalLLMChatManager chatManager;
    [SerializeField] private EmbeddingManager embeddingManager;

    private DialogueUI dialogueUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        if (dialogueUI == null)
            dialogueUI = DialogueUI.CreateUIDialogue();

        npcLayerMask = LayerMask.GetMask("NPC");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            TryInteract();
    }

    private void TryInteract()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, npcLayerMask))
        {
            if (hit.collider.GetComponent<NPCInteraction>() == this)
                HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        if (embeddingManager != null)
        {
            //StartCoroutine(embeddingManager.RequestEmbeddings(concept, prompt, (float[][] vectors) =>
            //{
            //    if (vectors != null && vectors.Length > 0 && vectors[0] != null)
            //    {
            //        Debug.Log($"[Embedding] concept 벡터 길이: {vectors[0].Length}");
            //        // 원하면 임베딩 결과도 UI로 보여줄 수 있습니다.
            //        // dialogueUI.PlayDialogue(new[] { $"Embedding length: {vectors[0].Length}" }, null);
            //    }
            //    else
            //    {
            //        Debug.LogWarning("임베딩에 실패했습니다.");
            //    }
            //}));
        }

        else if (chatManager)
        {
            // 2) LLM 대화 요청
            StartCoroutine(chatManager.SendPrompt(concept, prompt, reply =>
            {
                Debug.Log("[Chat] NPC replied: " + reply);
            }));
        }
    }

    public DialogueUI GetDialogueUI() => dialogueUI;
}

