parcel-wrap-verb-wrap = Завернуть
parcel-wrap-verb-unwrap = Развернуть

parcel-wrap-popup-parcel-destroyed = Упаковка, содержащая { THE($contents) }, уничтожена!
parcel-wrap-popup-being-wrapped = { CAPITALIZE(THE($user)) } пытается завернуть вас в обёртку!
parcel-wrap-popup-being-wrapped-self = Вы начинаете завёртывать себя в обёртку.

# Shown when parcel wrap is examined in details range
parcel-wrap-examine-detail-uses = { $uses ->
    [one] There is [color={$markupUsesColor}]{$uses}[/color] use left
    *[other] There are [color={$markupUsesColor}]{$uses}[/color] uses left
}.
