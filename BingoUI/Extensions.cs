using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingoUI;

internal static class Extensions
{
    public static void Increment<TKey>(this Dictionary<TKey, int> data, TKey key, int value = 1)
    {
        int current = data.GetAmount(key);

        data[key] = current + value;
    }

    public static int GetAmount<TKey>(this Dictionary<TKey, int> data, TKey key)
    {
        if (data.TryGetValue(key, out int amount))
        {
            return amount;
        }
        return 0;
    }

    public static int GetTotalAmount<TKey>(this Dictionary<TKey, int> data, IEnumerable<TKey> keys)
    {
        return keys.Select(key => data.GetAmount(key)).Sum();
    }
}
