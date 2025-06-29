using UnityEngine;

public class BaseManager : MonoBehaviour
{
    //public static ActionHolder actionHolder;
    public static PartyManager party;
    public static BaseManager instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        party = GetComponentInChildren<PartyManager>();
    }
}
