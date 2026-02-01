using UnityEngine;
using XRMultiplayer;
using Unity.Netcode;

public class PlayerPerkSocket : NetworkBehaviour
{
    [SerializeField] private Transform snapPoint;

    private PerkBottle equippedBottle;

    public Transform SnapPoint => snapPoint;
    public bool IsOccupied => equippedBottle != null;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer || IsOccupied)
            return;

        PerkBottle bottle = other.GetComponent<PerkBottle>();
        if (bottle == null || bottle.IsEquipped)
            return;

        NetworkPlayerPerksManager perks =
            GetComponentInParent<NetworkPlayerPerksManager>();

        if (perks == null)
            return;

        if (!perks.TryAddPerk(bottle.perkData.perkType))
            return;

        equippedBottle = bottle;
        bottle.Equip(this);
    }

    public void RemoveCanister()
    {
        if (!IsServer || equippedBottle == null)
            return;

        NetworkPlayerPerksManager perks =
            GetComponentInParent<NetworkPlayerPerksManager>();

        perks.RemovePerk(equippedBottle.perkData.perkType);

        equippedBottle.Unequip();
        equippedBottle = null;
    }
}
