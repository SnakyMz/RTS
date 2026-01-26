using Assets.Scripts.Units;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveAction", menuName = "Scriptable Objects/Actions/Move")]
public class MoveAction : ActionBase
{
    [SerializeField] float radiusMultiplier = 3f;
    int unitsOnLayer = 0;
    int maxUnitsPerLayer = 1;
    float circleRadius = 0f;
    float radialoffset = 0f;

    public override bool CanHandle(CommandContext context)
    {
        return context.Commandable is AbstractUnit;
    }

    public override void Handle(CommandContext context)
    {
        AbstractUnit unit = (AbstractUnit)context.Commandable;

        if (context.UnitIndex == 0)
        {
            // Reset formation parameters for the first unit
            unitsOnLayer = 0;
            maxUnitsPerLayer = 1;
            circleRadius = 0f;
            radialoffset = 0f;
        }

        Vector3 targetPosition = new(
                    context.Hit.point.x + circleRadius * Mathf.Cos(radialoffset * unitsOnLayer),
                    context.Hit.point.y,
                    context.Hit.point.z + circleRadius * Mathf.Sin(radialoffset * unitsOnLayer)
                );

        unit.MoveTo(targetPosition);
        unitsOnLayer++;

        if (unitsOnLayer >= maxUnitsPerLayer)
        {
            unitsOnLayer = 0;
            circleRadius += unit.AgentRadius * radiusMultiplier;
            maxUnitsPerLayer = Mathf.FloorToInt(2 * Mathf.PI * circleRadius / (unit.AgentRadius * 2f));
            radialoffset = 2 * Mathf.PI / maxUnitsPerLayer;
        }
    }
}
