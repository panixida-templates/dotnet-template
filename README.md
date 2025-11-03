# Конфигурация Code Generator

Ниже приведён пример содержания файла `appsettings.json`, в котором задаются «джобы» для генерации кода, а также описание ключевых параметров.

---

## Пример `appsettings.json`

```json
{
  "jobs": [
    {
      "pathToModels": "../../../../../DAL/DAL.DbModels",
      "pathToMigrator": "../../../../../DAL/DAL.Migrator/DAL.Migrator.csproj",
      "models": ["User"],
      "generatedFiles": [
        "Entity",
        "SearchParams",
        "ConvertParams",
        "DALFilter",
        "DALConvert",
        "IDAL",
        "DAL",
        "DALDependencyInjection",
        "DefaultDbContext",
        "IBL",
        "BL",
        "BLDependencyInjection",
        "APIModel",
        "APIController"
      ],

      "templates": {
        "Entity": "Templates/Domain/Entity.sbn",
        "SearchParams": "Templates/Common/SearchParams.sbn",
        "ConvertParams": "Templates/Common/ConvertParams.sbn",
        "DALFilter": "Templates/DAL/DAL.Filter.sbn",
        "DALConvert": "Templates/DAL/DAL.Convert.sbn",
        "IDAL": "Templates/DAL/DAL.Interface.sbn",
        "DAL": "Templates/DAL/DAL.Implementation.sbn",
        "DALDependencyInjection": "Templates/DAL/DAL.DependencyInjection.sbn",
        "DefaultDbContext": "Templates/DAL/DefaultDbContext.sbn",
        "IBL": "Templates/BL/BL.Interface.sbn",
        "BL": "Templates/BL/BL.Implementation.sbn",
        "BLDependencyInjection": "Templates/BL/BL.DependencyInjection.sbn",
        "APIModel": "Templates/API/API.Model.sbn",
        "APIController": "Templates/API/API.Controller.sbn"
      },

      "outputDirectory": {
        "Entity": "../../../../../Domain/Entities",
        "SearchParams": "../../../../../Common/Common.SearchParams",
        "ConvertParams": "../../../../../Common/Common.ConvertParams",
        "DALFilter": "../../../../../DAL/DAL.Implementations/Filters",
        "DALConvert": "../../../../../DAL/DAL.Implementations/Converts",
        "IDAL": "../../../../../DAL/DAL.Interfaces",
        "DAL": "../../../../../DAL/DAL.Implementations",
        "DALDependencyInjection": "../../../../../DAL/DAL.DependencyInjection",
        "DefaultDbContext": "../../../../../DAL/DAL.EF",
        "IBL": "../../../../../BL/BL.Interfaces",
        "BL": "../../../../../BL/BL.Implementations",
        "BLDependencyInjection": "../../../../../BL/BL.DependencyInjection",
        "APIModel": "../../../../../PL/Dev.Template.AspNetCore.API/Infrastructure/Models",
        "APIController": "../../../../../PL/Dev.Template.AspNetCore.API/Controllers"
      },

      "overwriteExisting": false,
      "deleteGenerated": false,
      "deleteSourceModel": false,
      "needMigrate": true
    }
  ]
}
````

---

## Описание параметров

| Параметр               | Тип    | Описание                                                                                 |
| ---------------------- | ------ | ---------------------------------------------------------------------------------------- |
| **jobs**               | массив | Список заданий (джоб), каждое из которых генерирует код по указанной модели.             |
| ├─ `pathToModels`      | строка | Путь к папке или проекту, где лежит модель (клаccы `.cs`), по которой будет генерация.   |
| ├─ `pathToMigrator`    | строка | Путь к `.csproj` мигратора, с помощью которого будут накатываться миграции.			     |
| ├─ `models`            | массив | Имя классов-моделей (без расширения), из которых строятся файлы.                          |
| ├─ `generatedFiles`    | массив | Список типов файлов, которые нужно сгенерировать (ключи для шаблонов и выходных папок).  |
| ├─ `templates`         | объект | Словарь: ключ — тип файла, значение — путь к шаблону (`.sbn` или иной шаблонный файл).   |
| ├─ `outputDirectory`   | объект | Словарь: ключ — тип файла, значение — путь к папке, куда сохранять сгенерированный файл. |
| ├─ `overwriteExisting` | булево | При `true` — существующие файлы перезаписываются; при `false` — пропускаются.            |
| ├─ `deleteGenerated`   | булево | При `true` — перед запуском удаляются все ранее сгенерированные файлы (по этой джобе).   |
| └─ `deleteSourceModel` | булево | При `true` — исходная модель удаляется после успешной генерации.                         |
| └─ `needMigrate` 		 | булево | При `true` — происходит создание миграций и обновление базы данных.                      |

---

## Пример структуры проекта

```
/ProjectRoot
│
├─ /DAL
│   └─ /DAL.DbModels            ← здесь лежат модели (в config: pathToModels)
│
├─ /Templates
│   ├─ /Domain
│   │   └─ Entity.sbn
│   ├─ /Common
│   │   ├─ SearchParams.sbn
│   │   └─ ConvertParams.sbn
│   ├─ /DAL
│   │   ├─ DAL.Filter.sbn
│   │   ├─ DAL.Convert.sbn
│   │   └─ … 
│   └─ /API
│       ├─ API.Model.sbn
│       └─ API.Controller.sbn
│
├─ /Domain
│   └─ /Entities              ← выход для «Entity»
│
├─ /Common
│   ├─ /Common.SearchParams   ← выход для «SearchParams»
│   └─ /Common.ConvertParams  ← выход для «ConvertParams»
│
├─ /DAL
│   ├─ /DAL.Interfaces        ← выход для «IDAL»
│   ├─ /DAL.Implementations   ← выход для «DAL»
│   ├─ /DAL.Implementations/Filters    ← выход для «DALFilter»
│   └─ /DAL.EF                ← выход для «DefaultDbContext»
│
├─ /BL
│   ├─ /BL.Interfaces         ← выход для «IBL»
│   └─ /BL.Implementations    ← выход для «BL»
│
└─ /PL/Dev.Template.AspNetCore.API
    ├─ /Infrastructure/Models ← выход для «APIModel»
    └─ /Controllers           ← выход для «APIController»
```

---

## Инструкция по использованию

1. **Добавьте нужные «джобы»** в секцию `"jobs"` вашего `appsettings.json`.
2. **Укажите путь** к проекту или папке с моделями через `pathToModels` и имена моделей `models`.
3. **Перечислите** в `generatedFiles` все типы файлов, которые хотите сгенерировать.
4. **Настройте шаблоны**: в `templates` опишите, какой файл-шаблон использовать для каждого типа.
5. **Задайте выходные папки** в `outputDirectory` для каждого типа файлов.
6. **При необходимости** включите или отключите перезапись (`overwriteExisting`), удаление старых файлов (`deleteGenerated`) или удаление модели (`deleteSourceModel`).
7. **Укажите путь** к мигратору через `pathToMigrator` и для активации автоматических миграций включите флаг `needMigrate`.

Теперь при запуске генератора каждая «джоба» сгенерирует код по вашим настройкам и разместит его в нужных папках. Удачи в разработке!

# Атрибуты для генерации фильтров и SearchParams

Ниже приведён пример использования атрибутов в модели `Test`, а также описание того, как они влияют на генерацию кода.

---

## 1. Пример модели с атрибутами

```csharp
using Common.Attributes;
using Common.Enums;
using DAL.DbModels.Core;

namespace DAL.DbModels;

public class Test : BaseDbModel<int>
{
    [Filter]
    [SearchParam]
    public string DefaultFilter { get; set; } = string.Empty;

    [Filter(FilterType.AccurateComparison)]
    [SearchParam]
    public int AccurateComparisonFilter { get; set; }

    [Filter(FilterType.InaccurateComparison)]
    [SearchParam]
    public int InaccurateComparisonFilter { get; set; }

    [Filter(FilterType.InRange)]
    [SearchParam(SearchParamType.Range)]
    public DateTime InRangeFilter { get; set; }

    [Filter(FilterType.OutRange)]
    [SearchParam(SearchParamType.Range)]
    public DateTime OutRangeFilter { get; set; }

    [Filter(FilterType.GreaterThan)]
    [SearchParam]
    public decimal GreaterThanFilter { get; set; }

    [Filter(FilterType.GreaterThanOrEqual)]
    [SearchParam]
    public decimal GreaterThanOrEqualFilter { get; set; }

    [Filter(FilterType.LessThan)]
    [SearchParam]
    public decimal LessThanFilter { get; set; }

    [Filter(FilterType.LessThanOrEqual)]
    [SearchParam]
    public decimal LessThanOrEqualFilter { get; set; }

    [SearchParam("CustomName")]
    [Filter("CustomName", FilterType.LessThan)]
    public int NamedFilter { get; set; }

    [SearchParam("StartTo", "EndTo")]
    [Filter("StartTo", "EndTo", FilterType.InRange)]
    public int RangeFilter { get; set; }

    [SearchParam(SearchParamType.Range)]
    public int RangeSearch { get; set; }

    public List<string> Tags { get; set; } = [];

    [Navigation]
    public User? User { get; set; }

    [Navigation]
    public List<User> Users { get; set; } = [];
}
````

---

## 2. Атрибут `[Navigation]`

* Помечает свойства навигации (связанные сущности).
* Обеспечивает корректную генерацию DTO и `ConvertParams` для связанных сущностей.

---

## 3. Атрибут `[Filter]`

* По умолчанию создаёт фильтр с точным сравнением (`AccurateComparison`).
* Позволяет указать собственное имя фильтра и тип сравнения.

```csharp
[Filter]                            // Точное сравнение
[Filter(FilterType.LessThan)]      // Меньше (<)
[Filter("CustomName", FilterType.InRange)]  // В диапазоне, имя — CustomName
```

### Типы фильтров (`Common.Enums.FilterType`)

| Константа              | Описание                |
| ---------------------- | ----------------------- |
| `AccurateComparison`   | Точное сравнение        |
| `InaccurateComparison` | Неточное сравнение      |
| `InRange`              | В диапазоне             |
| `OutRange`             | Вне диапазона           |
| `GreaterThan`          | Больше (`>`)            |
| `GreaterThanOrEqual`   | Больше или равно (`>=`) |
| `LessThan`             | Меньше (`<`)            |
| `LessThanOrEqual`      | Меньше или равно (`<=`) |

---

## 4. Пример сгенерированного фильтра в DAL

```csharp
using Common.SearchParams;
namespace DAL.Implementations.Filters;

internal static class TestsFilter
{
    internal static IQueryable<DbModels.Test> Filter(
        this IQueryable<DbModels.Test> dbObjects,
        TestsSearchParams searchParams)
    {
        if (!string.IsNullOrEmpty(searchParams.DefaultFilter))
            dbObjects = dbObjects.Where(item =>
                item.DefaultFilter.ToLower() == searchParams.DefaultFilter
                                                   .ToLower().Trim());

        if (searchParams.AccurateComparisonFilter.HasValue)
            dbObjects = dbObjects.Where(item =>
                item.AccurateComparisonFilter == searchParams.AccurateComparisonFilter.Value);

        // … другие проверки …

        if (searchParams.InRangeFilterFrom.HasValue)
            dbObjects = dbObjects.Where(item =>
                item.InRangeFilter >= searchParams.InRangeFilterFrom.Value);

        if (searchParams.InRangeFilterTo.HasValue)
            dbObjects = dbObjects.Where(item =>
                item.InRangeFilter <= searchParams.InRangeFilterTo.Value);

        // Пример фильтра «вне диапазона»
        if (searchParams.OutRangeFilterFrom.HasValue &&
            searchParams.OutRangeFilterTo.HasValue)
        {
            dbObjects = dbObjects.Where(item =>
                item.OutRangeFilter < searchParams.OutRangeFilterFrom.Value ||
                item.OutRangeFilter > searchParams.OutRangeFilterTo.Value);
        }

        // … и так далее …

        return dbObjects;
    }
}
```

---

## 5. Атрибут `[SearchParam]`

* Создаёт свойства в классе параметров поиска.
* По умолчанию — одиночное значение (`Value`), можно задать диапазон (`Range`).

```csharp
[SearchParam]                                  // Value
[SearchParam(SearchParamType.Range)]           // Range (добавит свойства From/To)
[SearchParam("StartTo", "EndTo")]              // Пользовательские имена
```

### Пример класса SearchParams

```csharp
using Common.SearchParams.Core;
using Common.Enums;

namespace Common.SearchParams;

public sealed class TestsSearchParams : BaseSearchParams
{
    public string? DefaultFilter { get; set; }
    public int? AccurateComparisonFilter { get; set; }
    public int? InaccurateComparisonFilter { get; set; }

    public DateTime? InRangeFilterFrom { get; set; }
    public DateTime? InRangeFilterTo   { get; set; }

    public DateTime? OutRangeFilterFrom { get; set; }
    public DateTime? OutRangeFilterTo   { get; set; }

    public decimal? GreaterThanFilter { get; set; }
    public decimal? GreaterThanOrEqualFilter { get; set; }
    public decimal? LessThanFilter   { get; set; }
    public decimal? LessThanOrEqualFilter { get; set; }

    public int? CustomName { get; set; }
    public int? StartTo   { get; set; }
    public int? EndTo     { get; set; }

    public int? RangeSearchFrom { get; set; }
    public int? RangeSearchTo   { get; set; }

    public TestsSearchParams(string? searchQuery = null,
                             int startIndex = 0,
                             int? objectsCount = null)
        : base(startIndex, objectsCount)
    {
        SearchQuery = searchQuery;
    }
}
```

---


## Конфигурация мигратора

`appsettings.json`:

```json
{
  "EF": {
    "ProjectPath": "../../../../DAL.EF"
  },
  "ApplyEntities": true,
  "ApplyHistory": true,
  "ConnectionStrings": {
    "DefaultConnectionString": "Host=10.10.0.7;Port=5432;Database=devtemplate;Username=devtemplate;Password=U@a#wRPcY4uMTP"
  }
}
```

* `"EF:ProjectPath"` — относительный путь (от папки, где запускается приложение-мигратор) до проекта с `DbContext` (например, `DAL.EF`).
* `"ApplyEntities": true` — флаг, отвечающий за применение изменений в структуре сущностей.
* `"ApplyHistory": true` — флаг, отвечающий за запись истории миграций (таблица `__EFMigrationsHistory`).
* `"ConnectionStrings:DefaultConnectionString"` — строка подключения к вашей БД PostgreSQL.

---

## Как работает мигратор (общая схема)

1. **Загрузка конфигурации и создание сервисов.**
   При запуске приложения-мигратор читает `appsettings.json`, получает строку подключения и путь к проекту с контекстом (`EF:ProjectPath`). Далее регистрирует `DbContext` (например, `DefaultDbContext`) с указанной строкой подключения и указывает сборку миграций (`MigrationsAssembly`).

2. **Сравнение текущей модели и снапшота.**
   В рантайме выполняется сравнение «живой» модели (`DefaultDbContext`) с тем снимком (Snapshot), который хранился в папке миграций проекта. Если в снимке (Snapshot) того же контекста нет сущностей (новых миграций) или модель совпадает со снапшотом, считается, что изменений нет.

3. **Если изменений нет, просто обновляем базу.**
   Когда разницы не обнаружено, вызывается стандартный `db.Database.MigrateAsync()`, который откатывает все недоиспользованные миграции (если они не применены) и примени­яет их к базе. То есть, в этом случае база просто «догоняет» уже сгенерированные миграции без создания новых.

4. **Если есть отличие — создаём миграцию и применяем её.**
   Если при сравнении модели и снапшота находим «diff» (дополнительные операции: добавление/удаление колонок, таблиц и т. п.), происходит следующее:

   * Генерируется уникальное имя миграции на основе набора операций (например, `Add_Column_Name_To_User_Table` и т.д.).
   * С помощью службы `IMigrationsScaffolder` создаётся новый скон­фигурированный класс миграции и подконтрольные файлы (класс миграции + её «snapshot»).
   * Эти файлы сохраняются в папке `Migrations` указанного проекта (`EF:ProjectPath`).

5. **Применение SQL-команд к БД.**
   После того как миграция сгенерирована и сохранена, из полученного списка операций формируются чистые SQL-скрипты (через `IMigrationsSqlGenerator`). Затем эти скрипты по очереди выполняются на самой базе (через `db.Database.ExecuteSqlRawAsync()`), в рамках одной транзакции.

6. **Запись истории миграций.**
   Если в конфигурации `"ApplyHistory": true`, то после применения основных изменений выполняется ещё один SQL-скрипт, который вставляет запись о только что выполненной миграции в системную таблицу `__EFMigrationsHistory`. Таким образом сохраняется, какой именно набор изменений (MigrationId + версия EF) уже применён.

7. **Коммит транзакции и завершение.**
   Если все команды успешно отработали, транзакция фиксируется, и мигратор завершает свою работу, возвращая `IHost`. Если же произошла ошибка на каком-то этапе, транзакция откатывается, а в логах можно увидеть подробную информацию об исключении.

---

Таким образом, при каждом запуске мигратора:

* Мы «понимаем», какие изменения появились в модели (`DefaultDbContext`) по сравнению с тем, что уже зафиксировано в папке миграций (Snapshot).
* Если изменений нет, просто обновляем базу по существующим миграциям.
* Если есть, автоматически создаём новый класс миграции, сохраняем его, а затем выполняем SQL-скрипты для добавления/удаления/изменения объектов БД и фиксируем запись в таблице истории.

В результате у вас всегда актуальный набор миграций в проекте и база данных, приведённая к нужному состоянию, без ручного вмешательства.
