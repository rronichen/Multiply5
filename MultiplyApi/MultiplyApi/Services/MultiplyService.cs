namespace MultiplyApi.Services;

public class MultiplyService : IMultiplyService
{
    public bool TryParseValue(string value, out int parsed)
    {
        parsed = 0;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (!int.TryParse(value, out parsed))
            return false;

        return parsed is >= 0 and <= 10000;
    }

    public bool IsMultiplyOfFive(int value) => value % 5 == 0;
}