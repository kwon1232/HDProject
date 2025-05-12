using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RayManager : MonoBehaviour
{
    [SerializeField] private InteractionUI interactionUI;
    [SerializeField] private LayerMask playerLayer;
    private void Awake()
    {
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject(0))
        {
            // 0 = 마우스 왼쪽, 1 = 마우스 오른쪽
            // Down 은 1번 Up 도 1번 그게 아니면 누르는 동안
            FindPlayerClick();
        }
    }

    private void FindPlayerClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, playerLayer))
        {
            PlayerController player = hit.collider.GetComponent<PlayerController>();
            // If the object hit by the ray is the player, don't trace with Ray to prevent displaying the UI.
            if (player.IsMinePhoton()) return;
            if (player != null)
            {
                ActionHolder.TargetPlayerIndex = player.OwnerActorNumber;
                interactionUI.gameObject.SetActive(true);
                interactionUI.Initialize(player, Interaction_State.Player);
            }
            else
            {
                interactionUI.DeactiveObject();
            }
        }
        else
        {
            interactionUI.DeactiveObject();
        }
    }
}
