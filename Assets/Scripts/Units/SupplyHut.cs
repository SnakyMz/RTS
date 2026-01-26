using Assets.Scripts.EventBus;
using Assets.Scripts.Events;
using Assets.Scripts.Units;
using UnityEngine;

public class SupplyHut : MonoBehaviour, ISelectable
{
    [SerializeField] GameObject decal;
    [SerializeField] int health;

    public void Select()
    {
        if (decal) decal.SetActive(true);

        Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this));
    }

    public void Deselect()
    {
        if (decal) decal.SetActive(false);

        Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this));
    }
}
