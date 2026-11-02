
# CrossApp

Наскрізний проєкт з крос-платформного програмування.

Предметна область: Замовлення.

Сутності: Customer, Product, Order, OrderLine.

Призначення: оформлення замовлень, перевірка правильності даних, керування статусами та підрахунок сум.

## Структура проєкту

- src/Core — бібліотека класів із доменною логікою.
- src/Core/Dto — record-типи для передачі даних.
- src/Core/Domain — доменні сутності та їхні інваріанти.
- src/Core/Import — імпорт даних із CSV та JSON.
- src/Core/Services — перевірка бізнес-правил.
- src/Cli — консольний застосунок.
- data — тестові файли.

## Лабораторна робота 1–2

Створено solution із проєктами Core та Cli.

Налаштовано multi-targeting для .NET 8 і .NET 10.

Реалізовано отримання інформації про середовище та перевірено різні режими публікації.

## Лабораторна робота 3

Тема: Базові типи домену, records, pattern matching, імпорт CSV/JSON.

Створено DTO:
- CustomerDto
- ProductDto
- OrderDto
- OrderLineDto

Реалізовано імпорт даних із CSV та JSON.

Некоректні записи не зупиняють імпорт. Програма повертає коректні дані та список помилок.

## Лабораторна робота 4

Тема: Доменна модель, сутності, інваріанти, інкапсуляція.

Реалізовано класи Order та OrderLine.

Використано приватні конструктори, фабричні методи Create(), інкапсуляцію стану та обробку винятків.

### Інваріанти

1. ID замовлення не може бути порожнім — Order.Create().
2. ID клієнта не може бути порожнім — Order.Create().
3. Дата замовлення повинна бути коректною — Order.Create().
4. ID товару не може бути порожнім — OrderLine.Create().
5. Назва товару не може бути порожньою — OrderLine.Create().
6. Ціна товару не може бути від'ємною — OrderLine.Create().
7. Кількість товару повинна бути більшою за нуль — OrderLine.Create().
8. Не можна додавати товари до підтвердженого або скасованого замовлення — Order.AddLine().
9. Не можна підтвердити порожнє замовлення — Order.ChangeStatus().
10. Не можна повторно підтвердити або повернути скасоване замовлення до попереднього стану — Order.ChangeStatus().
11. Клієнт не може мати більше п'яти активних замовлень — OrderPolicy.CreateForCustomer().

Для неправильних аргументів використовуються ArgumentException та ArgumentOutOfRangeException.

Для операцій, заборонених поточним станом об'єкта, використовується InvalidOperationException.

### Інкапсуляція

Поля сутностей захищені від прямої зміни.

Список товарів зберігається у приватному полі _lines.

Доступ до списку надається через IReadOnlyList та AsReadOnly().

Конструктори приватні, а створення об'єктів відбувається через методи Create().

### Перетворення DTO

Метод ToDto() перетворює доменну сутність на DTO.

Метод FromDto() відновлює сутність із DTO з повторною перевіркою інваріантів.

### Додаткові завдання

1. Реалізовано OrderImportConverter, який перетворює ImportResult на доменні сутності та збирає помилки перевірок.

2. Реалізовано правило між сутностями Customer та Order. Клієнт не може мати більше п'яти активних замовлень. Перевірка виконується в OrderPolicy.

3. Створено enum OrderStatus із трьома станами:
   - Draft — чернетка.
   - Confirmed — підтверджено.
   - Cancelled — скасовано.

Допустимі переходи:
- Draft → Confirmed, якщо є товари.
- Draft → Cancelled.
- Confirmed → Cancelled.

Інші переходи заборонені. Перевірка реалізована через switch expression у методі ChangeStatus().

### Запуск лабораторної роботи 4

```bash
dotnet build
dotnet run --project src/Cli --framework net10.0 -- --lab04
```

Програма демонструє успішні операції, порушення інваріантів та виконання трьох додаткових завдань.

## Запуск імпорту CSV/JSON

```bash
dotnet run --project src/Cli --framework net10.0 -- data/sample.csv
dotnet run --project src/Cli --framework net10.0 -- data/sample.json
```

## Команди для публікації

Self-contained:

```bash
dotnet publish src/Cli -c Release -r win-x64 --self-contained true
```

Framework-dependent:

```bash
dotnet publish src/Cli -c Release -r win-x64 --self-contained false
```

## Порівняння режимів публікації

| RID | Режим | Розмір publish | Потрібен runtime |
|---|---|---|---|
| win-x64 | self-contained | ~78 МБ | Ні |
| win-x64 | framework-dependent | ~229 КБ | Так |
| linux-x64 | self-contained | ~75 МБ | Ні |

## Середовище

- Windows 11 x64
- .NET SDK 10.0
- C#
- Visual Studio
- Git та GitHub

## Репозиторій

https://github.com/kkosinovaa/CrossApp
