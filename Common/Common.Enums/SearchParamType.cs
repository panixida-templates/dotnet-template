using System.ComponentModel.DataAnnotations;

namespace Common.Enums;

public enum SearchParamType
{
    [Display(Name = "По значению")]
    Value = 0,

    [Display(Name = "По диапозону")]
    Range = 1
}
