using Assets.Scripts.Commands;
using Assets.Scripts.EventBus;
using Assets.Scripts.Events;
using Assets.Scripts.Units;
using UnityEngine;

public abstract class AbstractCommandable : MonoBehaviour, ISelectable
{
    [field: SerializeField] public int CurrentHealth { private set; get; }
    [field: SerializeField] public int MaxHealth { private set; get; }
    [field: SerializeField] public ActionBase[] AvailaibleCommands { private set; get; }

    [SerializeField] UnitSO unitSO;
    [SerializeField] GameObject decal;

    protected virtual void Start()
    {
        CurrentHealth = unitSO.Health;
        MaxHealth = unitSO.Health;
    }

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
