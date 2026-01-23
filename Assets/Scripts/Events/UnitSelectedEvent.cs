using Assets.Scripts.EventBus;
using Assets.Scripts.Units;

namespace Assets.Scripts.Events
{
    public struct UnitSelectedEvent : IEvent
    {
        public ISelectable Unit { get; private set; }

        public UnitSelectedEvent(ISelectable unit)
        {
            this.Unit = unit;
        }
    }
}
