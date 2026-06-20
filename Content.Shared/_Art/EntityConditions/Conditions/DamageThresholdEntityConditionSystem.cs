using Content.Shared.EntityConditions;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Damage.Systems;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._Art.EntityConditions.Conditions;

public sealed partial class TotalDamageConditionSystem : EntityConditionSystem<DamageableComponent, TotalDamageCondition>
{
    protected override void Condition(Entity<DamageableComponent> entity, ref EntityConditionEvent<TotalDamageCondition> args)
    {
        var damage = entity.Comp.TotalDamage;
        args.Result = damage >= args.Condition.Min && damage <= args.Condition.Max;
    }
}

public sealed partial class DamageTypeConditionSystem : EntityConditionSystem<DamageableComponent, DamageTypeCondition>
{
    protected override void Condition(Entity<DamageableComponent> entity, ref EntityConditionEvent<DamageTypeCondition> args)
    {
        var damage = entity.Comp.Damage.DamageDict.GetValueOrDefault(args.Condition.Type, FixedPoint2.Zero);
        args.Result = damage >= args.Condition.Min && damage <= args.Condition.Max;
    }
}

public sealed partial class DamageGroupConditionSystem : EntityConditionSystem<DamageableComponent, DamageGroupCondition>
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    protected override void Condition(Entity<DamageableComponent> entity, ref EntityConditionEvent<DamageGroupCondition> args)
    {
        if (!_prototype.TryIndex(args.Condition.Group, out var groupProto))
        {
            args.Result = false;
            return;
        }

        FixedPoint2 totalGroupDamage = FixedPoint2.Zero;
        foreach (var damageType in groupProto.DamageTypes)
        {
            if (entity.Comp.Damage.DamageDict.TryGetValue(damageType, out var value))
                totalGroupDamage += value;
        }

        args.Result = totalGroupDamage >= args.Condition.Min && totalGroupDamage <= args.Condition.Max;
    }
}

public sealed partial class TotalDamageCondition : EntityConditionBase<TotalDamageCondition>
{
    [DataField]
    public FixedPoint2 Min = FixedPoint2.Zero;

    [DataField]
    public FixedPoint2 Max = FixedPoint2.MaxValue;

    public override string EntityConditionGuidebookText(IPrototypeManager prototype) =>
        Loc.GetString("entity-condition-guidebook-total-damage",
            ("min", Min.Float()),
            ("max", Max == FixedPoint2.MaxValue ? int.MaxValue : Max.Float()));
}

public sealed partial class DamageTypeCondition : EntityConditionBase<DamageTypeCondition>
{
    [DataField]
    public FixedPoint2 Min = FixedPoint2.Zero;

    [DataField]
    public FixedPoint2 Max = FixedPoint2.MaxValue;

    [DataField(required: true)]
    public ProtoId<DamageTypePrototype> Type;

    public override string EntityConditionGuidebookText(IPrototypeManager prototype)
    {
        if (!prototype.TryIndex(Type, out var typeProto))
            return string.Empty;

        return Loc.GetString("entity-condition-guidebook-type-damage",
            ("type", typeProto.LocalizedName),
            ("min", Min.Float()),
            ("max", Max == FixedPoint2.MaxValue ? int.MaxValue : Max.Float()));
    }
}

public sealed partial class DamageGroupCondition : EntityConditionBase<DamageGroupCondition>
{
    [DataField]
    public FixedPoint2 Min = FixedPoint2.Zero;

    [DataField]
    public FixedPoint2 Max = FixedPoint2.MaxValue;

    [DataField(required: true)]
    public ProtoId<DamageGroupPrototype> Group;

    public override string EntityConditionGuidebookText(IPrototypeManager prototype)
    {
        if (!prototype.TryIndex(Group, out var groupProto))
            return string.Empty;

        return Loc.GetString("entity-condition-guidebook-group-damage",
            ("type", groupProto.LocalizedName),
            ("min", Min.Float()),
            ("max", Max == FixedPoint2.MaxValue ? int.MaxValue : Max.Float()));
    }
}