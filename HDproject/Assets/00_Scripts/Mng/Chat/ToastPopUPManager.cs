using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ToastPopUPManager : MonoBehaviour
{
    public static ToastPopUPManager instance;
    public TextMeshProUGUI popUpText;
    Animator animator;

    private void Awake()
    {
        if (instance == null) instance = this;
        this.transform.localScale = Vector3.one;
        this.gameObject.SetActive(false);
        animator = GetComponent<Animator>();
    }
    public void Initialize(string temp)
    {
        this.gameObject.SetActive(true);
        popUpText.text = temp;
        animator.Play("Toast_Open");
    }
    public void Deactive() => gameObject.SetActive(false);
}
