using System.ComponentModel.DataAnnotations;

namespace Common.Enums;

public enum SettingType
{
    [Display(Name = "Файл robots.txt")]
    RobotsFile = 0,

    [Display(Name = "Файл sitemap.xml")]
    SitemapFile = 1,

    [Display(Name = "Хедер")]
    Header = 2,

    [Display(Name = "Футер")]
    Footer = 3
}