using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviourPunCallbacks
{
    private Dictionary<int, Party> activeParties = new Dictionary<int, Party>();

    // if user has part, do not enter other party
    // so,  this function tells the other user that party invitations are not possible.
    public bool HasParty(Player player)
    {
        foreach(var party in activeParties.Values)
        {
            if (party.IsMember(player))
                return true;   
        }
        return false;
    }

    // Gets the party to which the player belongs.
    public Party GetParty(Player player)
    {
        foreach(var party in activeParties.Values)
        {
            if (party.IsMember(player))
                return party;
        }
        return null;
    }

    public Party CreateParty(Player leader)
    {
        if(HasParty(leader))
            return null;

        int partyID = activeParties.Count + 1;
        Party newParty = new Party(partyID, leader);
        activeParties.Add(partyID, newParty);

        return newParty;
    }

    public bool JoinParty(Player player, int PartyID)
    {
        // if the process of inviting a certain user to a party
        // if target player has party or the party ID in the party manager dose not exist,
        // target player cannot enter the party.
        if(HasParty(player) || !activeParties.ContainsKey(PartyID))
        {
            return false;
        }

        // if target can invite in party,
        // target player add member in this party
        return activeParties[PartyID].AddMember(player);    
    }

    public void LeaveParty(Player player)
    {
        Party party = GetParty(player);
        if(party == null)
            return;

        party.RemoveMember(player);
        if (party.members.Count == 0)
            activeParties.Remove(party.partyID);
        
    }
}
