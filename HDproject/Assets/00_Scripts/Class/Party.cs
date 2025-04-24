using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Party
{
    public int partyID { get; private set; }
    public Photon.Realtime.Player leader { get; private set; }
    public List<Photon.Realtime.Player> members { get; private set; }

    public Party(int partyID, Photon.Realtime.Player leader)
    {
        partyID = this.partyID;
        leader = this.leader;
        members = new List<Photon.Realtime.Player>() { leader };
    }
}
