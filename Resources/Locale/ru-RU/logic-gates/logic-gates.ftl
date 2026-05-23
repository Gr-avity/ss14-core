logic-gate-examine = В данным момент принадлежит шлюзу { INDEFINITE($gate) } { $gate }.

logic-gate-cycle = Переключено на шлюз { INDEFINITE($gate) } { $gate }

power-sensor-examine = В данный момент проверяет { $output ->
    [true] output
    *[false] input
} battery.
power-sensor-voltage-examine = Проверяет { $voltage } электросеть.

power-sensor-switch = Переключено на проверку { $output ->
    [true] output
    *[false] input
} battery.
power-sensor-voltage-switch = Сеть переключена на { $voltage ->
