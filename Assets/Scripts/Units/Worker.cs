using Assets.Scripts.EventBus;
using Assets.Scripts.Events;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Worker : MonoBehaviour, ISelectable, IMoveable
    {
        [SerializeField] GameObject decal;

        NavMeshAgent agent;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        public void Select()
        {
            if (decal) decal.SetActive(true);

            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this));
        }

        public void Deselect()
        {
            if (decal) decal.SetActive(false);
        }

        public void MoveTo(Vector3 position)
        {
            agent.SetDestination(position);
        }
    }
}
