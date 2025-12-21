using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BingoUI.Data;

public class SaveData
{
    private static SaveData? _instance;

    [AllowNull]
    public static SaveData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new();
            }
            return _instance;
        }
        internal set
        {
            _instance = value;
        }
    }

    public Dictionary<CurrencyType, int> SpentCurrency { get; set; } = new()
    {
        [CurrencyType.Money] = 0,
        [CurrencyType.Shard] = 0,
    };

    public Dictionary<string, int> PickedShinies { get; set; } = [];

    public HashSet<string> ShakraScenes { get; set; } = [];

    public int TotalDeliveryQuestsCompleted { get; set; } = 0;
}
