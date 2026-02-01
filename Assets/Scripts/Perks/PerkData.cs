using UnityEngine;

[CreateAssetMenu(fileName = "PerkData", menuName = "Scriptable Objects/PerkData")]
public class PerkData : ScriptableObject
{
    public NetworkPlayerPerksManager.TypeOfPerks perkType;
    public Material perkMaterial;
    public float duration;
}
