using System.Collections.Generic;

namespace BingoUI;

internal static class SaveDataProxy
{
    public static Dictionary<CurrencyType, int> SpentCurrency { get; } = new()
    {
        [CurrencyType.Money] = 0,
        [CurrencyType.Shard] = 0,
    };
}
