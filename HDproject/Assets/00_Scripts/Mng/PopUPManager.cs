using UnityEngine;
using UnityEngine.UI;
using System;

public class PopUPManager : MonoBehaviour
{
    public static PopUPManager instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this; 
        }
        transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }

    public Text description;
    public Button yesBnt;
    public Button noBnt;

    public void Initialize(string temp, Action yes, Action no)
    {
        gameObject.SetActive(true);
        description.text = temp;

        RemoveAllButtons();

        yesBnt.onClick.AddListener(() => yes());
        noBnt.onClick.AddListener(() => no());

        yesBnt.onClick.AddListener(() => this.gameObject.SetActive(false));
        noBnt.onClick.AddListener(() => this.gameObject.SetActive(false));
    }

    private void RemoveAllButtons()
    {
        yesBnt.onClick.RemoveAllListeners();
        noBnt.onClick.RemoveAllListeners();
    }
}
