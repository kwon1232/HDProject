using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class AHUD : MonoBehaviour
{
    public int ZOrderHUD;
    public Dictionary<string, AHUD> hudRegister { get; private set; }

    private void Awake()
    {
        ZOrderHUD = 0;
        if(!hudRegister.ContainsKey(this.gameObject.name))
            hudRegister[this.gameObject.name] = this;
    }

    virtual public void Initialization(int ZOrderHUD = 0)
    {
        if (!hudRegister.ContainsKey(this.gameObject.name))
            hudRegister[this.gameObject.name] = this;
    }

    virtual public Button GetButton (string btnName)
    {
        if (btnName.IsNullOrEmpty()) { return null; }

        Button[] allButton = Object.FindObjectsOfType<Button>();
        foreach (var btn in allButton) { 
            if(btn.name == btnName) return btn;
        } 
        return null;
    }

    virtual public Image GetImage(string imName)
    {
        if (imName.IsNullOrEmpty()) { return null; }

        Image[] allImage = Object.FindObjectsOfType<Image>();
        foreach (var im in allImage)
        {
            if (im.name == imName) return im;
        }
        return null;
    }
}
