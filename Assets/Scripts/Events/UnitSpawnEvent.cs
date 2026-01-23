using Assets.Scripts.EventBus;
using Assets.Scripts.Units;

namespace Assets.Scripts.Events
{
    public struct UnitSpawnEvent : IEvent
    {
        public AbstractUnit Unit { get; private set; }

        public UnitSpawnEvent(AbstractUnit unit)
        {
            this.Unit = unit;
        }
    }
}
