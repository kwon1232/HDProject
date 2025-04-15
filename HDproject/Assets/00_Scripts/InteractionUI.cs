using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum Interaction_State
{
    Player,
}

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private float yPosFloat;
    Animator animator;
    PlayerController playerController;
    Interaction_State m_State; 
    public InteractionButtonUI[] interactionButtons;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(playerController != null)
        {
            Vector3 targetPosition = playerController.transform.position + new Vector3(0.0f, yPosFloat, 0.0f);
            transform.position = Camera.main.WorldToScreenPoint(targetPosition);
        }
    }

    public void Initialize(PlayerController controller, Interaction_State state)
    {
        playerController = controller;
        m_State = state;
        var actions = InteractionActions(state);
        for (int i = 0; i < interactionButtons.Length; i++)
        {
            interactionButtons[i].Initialize(actions[i]);
        }
        animator.Play("Hexagon_Open");
    }

    public void DeactiveObject()
    {
        animator.Play("Hexagon_Hide");
    }

    public void DeActive() => gameObject.SetActive(false);

    private Action_State[] InteractionActions(Interaction_State state)
    {
        Action_State[] actions = new Action_State[6];
        switch (state)
        {
            case Interaction_State.Player:
                actions[0] = Action_State.InviteParty;
                actions[1] = Action_State.Trade;
                actions[2] = Action_State.InviteGuild;
                break;
        }
        return actions;
    }

}
