namespace Common.Helpers;

public static class PhoneHelper
{
    public static string ToServerRuPhone(string? phone)
    {
        var digits = ExtractDigits(phone);

        if (digits.Length == 0)
        {
            return string.Empty;
        }
        if (digits.Length == 10)
        {
            return string.Concat("+7", digits);
        }
        if (digits.Length == 11 && (digits[0] == '7' || digits[0] == '8'))
        {
            return string.Concat("+7", digits.AsSpan(1));
        }
        if (digits.Length > 10)
        {
            return string.Concat("+7", digits.AsSpan(digits.Length - 10));
        }

        return string.Empty;
    }

    public static string ToUiRuPhone(string? phone)
    {
        var digits = ExtractDigits(phone);

        if (digits.Length == 0)
        {
            return string.Empty;
        }
        if (digits.Length == 11 && (digits[0] == '7' || digits[0] == '8'))
        {
            return digits[1..];
        }
        if (digits.Length == 10)
        {
            return digits;
        }
        if (digits.Length > 10)
        {
            return digits[^10..];
        }

        return digits;
    }

    public static string ToDisplayRuPhone(string? phone)
    {
        var digits = ExtractDigits(phone);

        if (digits.Length == 0)
        {
            return string.Empty;
        }

        if (digits.Length == 11 && (digits[0] == '7' || digits[0] == '8'))
        {
            digits = digits[1..];
        }
        else if (digits.Length > 10)
        {
            digits = digits[^10..];
        }

        if (digits.Length != 10)
        {
            return digits;
        }

        var area = digits[..3];
        var firstPart = digits.Substring(3, 3);
        var secondPart = digits.Substring(6, 2);
        var thirdPart = digits.Substring(8, 2);

        return $"+7 ({area}) {firstPart}-{secondPart}-{thirdPart}";
    }

    private static string ExtractDigits(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var digits = input.Where(char.IsDigit).ToArray();

        return new string(digits);
    }
}
