namespace MultiplyApi.Services;

public interface IMultiplyService
{
    bool TryParseValue(string value, out int parsed);
    bool IsMultiplyOfFive(int value);
}