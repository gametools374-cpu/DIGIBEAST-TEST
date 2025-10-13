// TradeManager.cs
// Steam P2P escrow.
using UnityEngine;
using System.Collections.Generic;

public class TradeManager : ManagerBase<TradeManager>
{
    public List<DigibeeInstance> playerOffer = new List<DigibeeInstance>();
    public List<DigibeeInstance> partnerOffer = new List<DigibeeInstance>();
    public bool locked;

    public void AddToPlayerOffer(DigibeeInstance d) => playerOffer.Add(d);
    public void AddToPartnerOffer(DigibeeInstance d) => partnerOffer.Add(d);

    public void LockTrade()
    {
        locked = true;
        // TODO: Steam API lock handshake
    }

    public void ExecuteTrade()
    {
        if (!locked) return;
        // swap lists
        var temp = new List<DigibeeInstance>(playerOffer);
        Inventory.Instance.RemoveDigibees(playerOffer);
        Inventory.Instance.AddDigibees(partnerOffer);
        // partner logic via Steam callback
        playerOffer.Clear();
        partnerOffer.Clear();
        locked = false;
        AudioManager.Instance.PlayUI("TradeComplete");
    }
}