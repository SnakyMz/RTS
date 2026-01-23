using Assets.Scripts.EventBus;
using Assets.Scripts.Units;

namespace Assets.Scripts.Events
{
    public struct UnitDeselectedEvent : IEvent
    {
        public ISelectable Unit { get; private set; }

        public UnitDeselectedEvent(ISelectable unit)
        {
            this.Unit = unit;
        }
    }
}
