using System.ComponentModel.DataAnnotations;

namespace Common.Enums;

public enum FilterType
{
    [Display(Name = "Точное сравнение")]
    AccurateComparison = 0,

    [Display(Name = "Неточное сравнение")]
    InaccurateComparison = 1,

    [Display(Name = "В диапазоне")]
    InRange = 2,

    [Display(Name = "Вне диапазона")]
    OutRange = 3,

    [Display(Name = "Больше")]
    GreaterThan = 4,

    [Display(Name = "Больше или равно")]
    GreaterThanOrEqual = 5,

    [Display(Name = "Меньше")]
    LessThan = 6,

    [Display(Name = "Меньше или равно")]
    LessThanOrEqual = 7,
}
