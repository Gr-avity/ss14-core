-entity-heater-setting-name =
    { $setting ->
        [off] off
        [low] low
        [medium] medium
        [high] high
       *[other] unknown
    }

entity-heater-examined = Настроен на [color=gray]{$setting}[/color]
    [off] [color=gray]{ -entity-heater-setting-name(setting: "off") }[/color]
    [low] [color=yellow]{ -entity-heater-setting-name(setting: "low") }[/color]
    [medium] [color=orange]{ -entity-heater-setting-name(setting: "medium") }[/color]
    [high] [color=red]{ -entity-heater-setting-name(setting: "high") }[/color]
   *[other] [color=purple]{ -entity-heater-setting-name(setting: "other") }[/color]
}.
entity-heater-switch-setting = Переключить на {$setting}
entity-heater-switched-setting = Был переключен на {$setting}
