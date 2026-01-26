using Assets.Scripts.EventBus;
using Assets.Scripts.Events;
using Assets.Scripts.Units;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class AbstractUnit : AbstractCommandable, IMoveable
{
    public float AgentRadius => agent.radius;

    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected override void Start()
    {
        base.Start();
        Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
    }

    public void MoveTo(Vector3 position)
    {
        agent.SetDestination(position);
    }
}