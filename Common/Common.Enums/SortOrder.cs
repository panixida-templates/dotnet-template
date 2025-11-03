using System.ComponentModel.DataAnnotations;

namespace Common.Enums;

public enum SortOrder
{
    [Display(Name = "По возрастанию")]
    Ascending = 0,

    [Display(Name = "По убыванию")]
    Descending = 1,
}
