using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Worker : MonoBehaviour, ISelectable
    {
        [SerializeField] Transform target;
        [SerializeField] GameObject decal;

        NavMeshAgent agent;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            if (target)
            {
                agent.SetDestination(target.position);
            }
        }

        public void Select()
        {
            decal.SetActive(true);
        }

        public void Deselect()
        {
            decal.SetActive(false);
        }
    }
}
