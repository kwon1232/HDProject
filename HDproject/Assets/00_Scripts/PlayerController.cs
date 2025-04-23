using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public int OwnerActorNumber { get; private set; }
    public float speed;
    
    private CharacterController characterController;
    private Animator animator;

    private float gravity = -9.81f;
    private Vector3 velocity;

    PhotonView view;

    public void Initialize(int actorNumber)
    {
        if(IsMinePhoton())
        {
            OwnerActorNumber = actorNumber;
            //AllBuffered 참여하지 않은 플레이어도 같이 RPC 호출 실행하도록 보관
            //All 참여한 플레이어만 RPC 호출 실행하도록 한다.
            view.RPC("SetActorNumber", RpcTarget.AllBuffered, actorNumber);
        }
    }

    public bool IsMinePhoton()
    {
        return view.IsMine;
    }

    // RPC : Remote Procedure Call - 네트워크 상의 다른 플레이어가 실행 중인 특정 메서드를 호출한다.
    // 포톤에서 사용되는 네트워크 통신 방법 중 하나이다.

    [PunRPC]
    public void SetActorNumber(int actorNumber)
    {
        OwnerActorNumber = actorNumber;
    }


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        view = GetComponent<PhotonView>();
    }

    public bool isMinePhoton()
    {
        return view.IsMine;
    }

    private void Update()
    {
        if (!view.IsMine) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(h, 0, v);

        if (movement.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);

            characterController.Move(movement * speed * Time.deltaTime);

            animator.SetFloat("Movement", movement.magnitude);
        }
        else
        {
            animator.SetFloat("Movement", 0);
        }

    }
}
