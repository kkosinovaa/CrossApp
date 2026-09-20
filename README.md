# CrossApp
Наскрізний проєкт з крос-платформного програмування.

Предметна область: Замовлення. 
Сутності: Customer, Product, Order, OrderLine.
Призначення: оформлення замовлень і підрахунок сум.

## Структура рішення (Solution)
- **src/Core** — бібліотека класів (classlib). Містить логіку збору інформації про середовище (`EnvironmentInfo.cs`). Налаштовано multi-targeting.
- **src/Cli** — консольний застосунок (точка входу). Містить `Program.cs`, який посилається на `Core` (через ProjectReference) і відповідає виключно за вивід даних.

## Запуск
```bash
dotnet build
dotnet run --project src/Cli
```
## Команди для публікації
Self-contained (автономний):
```bash
dotnet publish src/Cli -c Release -r win-x64 --self-contained true
```

Framework-dependent (залежний від фреймворку):
```bash
dotnet publish src/Cli -c Release -r win-x64 --self-contained false
```
## Порівняння режимів публікації
| RID | Режим | Розмір каталогу publish | Потрібен встановлений runtime |
| :--- | :--- | :--- | :--- |
| **win-x64** | self-contained | ~78 МБ | ні |
| **win-x64** | framework-dependent | ~229 КБ | так (.NET 10) |
| **linux-x64**| self-contained | ~75 МБ | ні |

## Середовище
.NET SDK 10.0, Windows 11 x64

## Додаткове завдання (Розміри publish)
- win-x64: ~75 МБ
- linux-x64: ~76 МБ