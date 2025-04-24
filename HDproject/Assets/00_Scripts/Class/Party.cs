using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Unity.VisualScripting;

public class Party
{
    public int partyID { get; private set; }
    public Player leader { get; private set; }
    public List<Player> members { get; private set; }

    public Party(int partyID, Player leader)
    {
        partyID = this.partyID;
        leader = this.leader;
        members = new List<Player>() { leader };
    }

    public bool AddMember(Player player)
    {
        if(!members.Contains(player))
        {
            members.Add(player);
            return true;
        }
        return false;
    }

    public bool RemoveMember(Player player)
    {
        if(members.Contains(player))
        {
            members.Remove(player);
            if (player == leader && members.Count > 0)
            {
                leader = members[0];
            }
            return true;
        }
        return false;
    }

    public bool IsMember(Player player)
    {
        return members.Contains(player);
    }

    public void DisbandParty()
    {
        members.Clear();
    }
}
