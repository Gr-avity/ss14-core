game-ticker-restart-round = Перезапуск раунда...
game-ticker-start-round = Раунд начинается...
game-ticker-start-round-cannot-start-game-mode-fallback = Не удалось запустить режим «{ $failedGameMode }»! Запускаем «{ $fallbackMode }»...
game-ticker-start-round-cannot-start-game-mode-restart = Не удалось запустить режим «{ $failedGameMode }»! Перезапуск раунда...
game-ticker-start-round-invalid-map = Выбранная карта «{$map}» недоступна для игрового режима «{$mode}». Игровой режим может работать неправильно...
game-ticker-unknown-role = Неизвестный
game-ticker-delay-start = Начало раунда было отложено на {$seconds} {RU-PLURAL($seconds, "секунду", "секунды", "секунд")}.
game-ticker-pause-start = Начало раунда было приостановлено.
game-ticker-pause-start-resumed = Отсчет начала раунда возобновлен.
game-ticker-player-join-game-message = Добро пожаловать на Космическую Станцию 14! Если вы играете впервые, обязательно нажмите ESC на клавиатуре и прочитайте правила игры, а также не бойтесь просить помощи в «АХелп» (Админ Помощь).
game-ticker-get-info-text = Hi and welcome to [color=white]Space Station 14![/color]
                            The current round is: [color=white]#{$roundId}[/color]
                            The current player count is: [color=white]{$playerCount}[/color]
                            The current map is: [color=white]{$mapName}[/color]
                            The current game mode is: [color=white]{$gmTitle}[/color]
                            >[color=yellow]{$desc}[/color]
game-ticker-get-info-preround-text = Hi and welcome to [color=white]Space Station 14![/color]
                            The current round is: [color=white]#{$roundId}[/color]
                            The current player count is: [color=white]{$playerCount}[/color] ([color=white]{$readyCount}[/color] {$readyCount ->
                                [one] is
                                *[other] are
                            } ready)
                            The current map is: [color=white]{$mapName}[/color]
                            The current game mode is: [color=white]{$gmTitle}[/color]
                            >[color=yellow]{$desc}[/color]
game-ticker-no-map-selected = [color=red]Карта ещё не выбрана![/color]
game-ticker-player-no-jobs-available-when-joining = При попытке присоединиться к игре ни одной роли не было доступно.
game-ticker-player-no-character-for-job-available-when-joining = При попытке присоединиться к игре для выбранной должности { $job } не оказалось доступных персонажей.

# Displayed in chat to admins when a player joins
player-join-message = Игрок «{ $name }» зашёл.
player-first-join-message = Игрок «{ $name }» зашёл на сервер впервые!

# Displayed in chat to admins when a player leaves
player-leave-message = Игрок «{ $name }» вышел.

latejoin-arrival-announcement = {$character} ({$job}) has arrived at the station!
latejoin-arrival-announcement-special = {$job} {$character} на станции!
latejoin-arrival-sender = Станция
latejoin-arrivals-direction = Вскоре прибудет шаттл, который доставит вас на вашу станцию.
latejoin-arrivals-direction-time = Шаттл, который доставит вас на станцию, прибудет через { $time }.
latejoin-arrivals-dumped-from-shuttle = Таинственная сила не позволяет вам улететь на шаттле прибытия.
latejoin-arrivals-teleport-to-spawn = Таинственная сила телепортирует вас с шаттла прибытия. Удачной смены!

preset-not-enough-ready-players = Не удалось запустить пресет «{ $presetName }». Требуется { $minimumPlayers } {RU-PLURAL($minimumPlayers, "игрок", "игрока", "игроков")}, но {RU-PLURAL($readyPlayersCount, "готов", "готовы", "готовы")} только { $readyPlayersCount }.
preset-no-one-ready = Не удалось запустить режим «{ $presetName }». Нет готовых игроков.

game-run-level-PreRoundLobby = Лобби
game-run-level-InRound = Раунд
game-run-level-PostRound = Конец раунда
