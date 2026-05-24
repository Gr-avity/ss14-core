# Commands
## Delay shuttle round end
cmd-delayroundend-desc = Останавливает таймер окончания раунда, когда эвакуационный шаттл покидает гиперпространство.
cmd-delayroundend-help = Использование: delayroundend
emergency-shuttle-command-round-yes = Раунд продлён.
emergency-shuttle-command-round-no = Невозможно продлить окончание раунда.

## Dock emergency shuttle
cmd-dockemergencyshuttle-desc = Вызывает спасательный шаттл и пристыковывает его к станции... если это возможно.
cmd-dockemergencyshuttle-help = Использование: dockemergencyshuttle

## Launch emergency shuttle
cmd-launchemergencyshuttle-desc = Досрочно запускает эвакуационный шаттл, если это возможно.
cmd-launchemergencyshuttle-help = Использование: launchemergencyshuttle

# Emergency shuttle # Starlight edit: reword due to potential existence of multiple stations/shuttles
emergency-shuttle-left = Эвакуационный шаттл покинул станцию. Расчетное время прибытия шаттла на станцию ЦентКома — { $transitTime } секунд.
emergency-shuttle-launch-time = Эвакуационный шаттл будет запущен через { $consoleAccumulator } секунд.
emergency-shuttle-docked = Эвакуационный шаттл пристыковался к станции { $location }, направление: { $direction }. Он улетит через { $time } секунд.{ $extended }
emergency-shuttle-good-luck = Эвакуационный шаттл не может найти станцию. Удачи.
emergency-shuttle-nearby = Эвакуационный шаттл не может найти подходящий стыковочный шлюз. Он дрейфует около станции, { $location }, направление: { $direction }. Он улетит через { $time } секунд.{ $extended }
emergency-shuttle-extended = {" "}Время до запуска было увеличено из-за неблагоприятных обстоятельств.

# Emergency shuttle console popup / announcement
emergency-shuttle-console-no-early-launches = Досрочный запуск отключён
emergency-shuttle-console-auth-left = {$remaining} authorizations needed until shuttle is launched early.
emergency-shuttle-console-auth-revoked = Early launch authorization revoked, {$remaining} authorizations needed.
emergency-shuttle-console-denied = Доступ запрещён

# UI
emergency-shuttle-console-window-title = Консоль эвакуационного шаттла
emergency-shuttle-ui-engines = ДВИГАТЕЛИ:
emergency-shuttle-ui-idle = Простой
emergency-shuttle-ui-repeal-all = Повторить всё
emergency-shuttle-ui-early-authorize = Разрешение на досрочный запуск
emergency-shuttle-ui-authorize = АВТОРИЗОВАТЬСЯ
emergency-shuttle-ui-repeal = ПОВТОРИТЬ
emergency-shuttle-ui-authorizations = Авторизации
emergency-shuttle-ui-remaining = Осталось: { $remaining }

# Map Misc.
map-name-centcomm = Центральное Командование
map-name-terminal = Терминал Прибытия
