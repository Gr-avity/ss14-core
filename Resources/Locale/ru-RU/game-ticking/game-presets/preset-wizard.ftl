## Survivor

roles-antag-survivor-name = Выживший
# It's a Halo reference
roles-antag-survivor-objective = Текущая задача: Выжить

survivor-role-greeting =
    You are a Survivor. Above all you need to make it back to Central Command alive.
    Collect as much firepower as needed to guarantee your survival.
    Trust no one.

survivor-round-end-dead-count =
{
    $deadCount ->
        [one] [color=red]{$deadCount}[/color] survivor died.
        *[other] [color=red]{$deadCount}[/color] survivors died.
}

survivor-round-end-alive-count =
{
    $aliveCount ->
        [one] [color=yellow]{$aliveCount}[/color] survivor was marooned on the station.
        *[other] [color=yellow]{$aliveCount}[/color] survivors were marooned on the station.
}

survivor-round-end-alive-on-shuttle-count =
{
    $aliveCount ->
        [one] [color=green]{$aliveCount}[/color] survivor made it out alive.
        *[other] [color=green]{$aliveCount}[/color] survivors made it out alive.
}

## Wizard

objective-issuer-swf = [color=turquoise]Федерация космических волшебников[/color]

wizard-title = Волшебник
wizard-description = Космический Волшебник посещает станцию, дабы продемонстрировать чудеса своей магии.

roles-antag-wizard-name = Космический волшебник
roles-antag-wizard-objective = Устройте хаос на станции

wizard-role-greeting =
    It's wizard time, fireball!
    There's been tensions between the Space Wizards Federation and NanoTrasen. You've been selected by the Space Wizards Federation to pay a visit to the station and "remind them" why spellcasters are not to be trifled with.
    Cause mayhem and destruction! What you do is up to you, but remember that the Space Wizards want you to make it out alive.

wizard-round-end-name = волшебник

## TODO: Wizard Apprentice (Coming sometime post-wizard release)
