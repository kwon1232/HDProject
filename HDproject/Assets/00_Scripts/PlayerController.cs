using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    private float gravity = -9.81f;
    private Vector3 velocity;
    public float speed;
    PhotonView view;


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
