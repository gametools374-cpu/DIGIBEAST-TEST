// TradeManager.cs
// Steam P2P escrow.
using UnityEngine;
using System.Collections.Generic;

public class TradeManager : ManagerBase<TradeManager>
{
    public List<DigibeastInstance> playerOffer = new List<DigibeastInstance>();
    public List<DigibeastInstance> partnerOffer = new List<DigibeastInstance>();
    public bool locked;

    public void AddToPlayerOffer(DigibeastInstance d) => playerOffer.Add(d);
    public void AddToPartnerOffer(DigibeastInstance d) => partnerOffer.Add(d);

    public void LockTrade()
    {
        locked = true;
        // TODO: Steam API lock handshake
    }

    public void ExecuteTrade()
    {
        if (!locked) return;
        // swap lists
        var temp = new List<DigibeastInstance>(playerOffer);
        Inventory.Instance.RemoveDigibeasts(playerOffer);
        Inventory.Instance.AddDigibeasts(partnerOffer);
        // partner logic via Steam callback
        playerOffer.Clear();
        partnerOffer.Clear();
        locked = false;
        AudioManager.Instance.PlayUI("TradeComplete");
    }
}