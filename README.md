<div align="center">

<h1 align="center"> <img alt="Space Station 14" width="600" src="Resources/Textures/Logo/logo.png" /> </h1>

**Форк STARLIGHT с уникальным контентом и геймплеем**

[![Discord](https://img.shields.io/discord/1039584848689496065?style=for-the-badge&logo=discord&logoColor=white&label=Discord&color=%237289da)](https://discord.gg/PDDpByYXgM)
[![Steam](https://img.shields.io/badge/Steam-SS14%20-003459?style=for-the-badge)](https://store.steampowered.com/app/1255460/Space_Station_14/)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge)](https://dotnet.microsoft.com/)

</div>

---

## 📋 О проекте

**SS14-ART-CORE** — это форк [Starlight](https://github.com/ss14Starlight/space-station-14), который в свою очередь основан на [Space Station 14](https://github.com/space-wizards/space-station-14). Проект сочетает классический хаос SS13 с экспериментальными механиками, возможными только на новом движке.

Space Station 14 — это remake SS13, работающий на [Robust Toolbox](https://github.com/space-wizards/RobustToolbox) — собственном движке, написанном на C#.

## 📚 Документация

- **[Официальная документация SS14](https://docs.spacestation14.io/)** — движок, контент, геймдизайн
- **[Robust Generic Attribution](https://docs.spacestation14.com/en/specifications/robust-generic-attribution.html)** — информация об атрибуции
- **[Robust Station Image](https://docs.spacestation14.com/en/specifications/robust-station-image.html)** — правила использования изображений

## 🤝 Контрибуция

Мы всегда рады помощи от всех желающих! Присоединяйтесь к [Discord](https://discord.gg/qcK4ZKFNUb), если хотите внести свой вклад.

У нас есть [список задач](https://github.com/ss14-art/shine/issues), которые нужно решить. Не стесняйтесь спрашивать помощь!

## 🚀 Сборка проекта

### Требования

- **Git** — [скачать](https://git-scm.com/downloads)
- **.NET SDK 10.0 или выше** — [скачать](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Python 3.11+** — [скачать](https://www.python.org/downloads/)

### Инструкция

> [!IMPORTANT]
> Убедитесь, что путь к папке проекта не содержит кириллицу или пробелы!

#### Windows

```bash
# 1. Клонируйте репозиторий
git clone https://github.com/ss14-art/ss14-core.git
cd ss14-core

# 2. Загрузите движок
git submodule update --init --recursive

# 3. Соберите проект
Scripts\bat\buildAllRelease.bat

# 4. Запустите клиент и сервер
Scripts\bat\runQuickAll.bat
```

#### Linux / macOS

```bash
# 1. Клонируйте репозиторий
git clone https://github.com/ss14-art/ss14-core.git
cd ss14-core

# 2. Загрузите движок
git submodule update --init --recursive

# 3. Соберите проект
chmod +x Scripts/sh/buildAllRelease.sh
Scripts/sh/buildAllRelease.sh

# 4. Запустите клиент и сервер
chmod +x Scripts/sh/runQuickAll.sh
Scripts/sh/runQuickAll.sh
```

После запуска подключитесь к **localhost** в окне клиента.

> **Более подробная инструкция**: [официальное руководство по сборке SS14](https://docs.spacestation14.com/en/general-development/setup.html)

## 📜 Лицензия

Подробную информацию о лицензиях смотрите в файле **[LEGAL.md](./LEGAL.md)**.

### Код

Код проекта распространяется под лицензией **GNU AGPLv3** (если не указано иное). Информация о лицензии каждого файла представлена в формате REUSE (в заголовке файла) или в отдельном файле `.license`.

### Ассеты (графика, звуки, спрайты)

Большинство ассетов лицензированы под **[CC-BY-SA 3.0](https://creativecommons.org/licenses/by-sa/3.0/)**, если не указано иное. Лицензия и информация об авторских правах содержатся в metadata файлах. [Пример](https://github.com/ss14-art/ss14-core/blob/master/Resources/Textures/Objects/Tools/crowbar.rsi/meta.json).

> [!NOTE]
> Некоторые ассеты имеют некоммерческую лицензию **[CC-BY-NC-SA 3.0](https://creativecommons.org/licenses/by-nc-sa/3.0/)**. Они будут удалены, если вы планируете использовать проект в коммерческих целях.
