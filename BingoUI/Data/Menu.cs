using BepInEx.Configuration;
using Silksong.ModMenu.Elements;
using Silksong.ModMenu.Plugin;
using Silksong.ModMenu.Screens;
using System.Collections.Generic;

namespace BingoUI.Data;

internal static class Menu
{
    public static AbstractMenuScreen GenerateMenu()
    {
        ConfigEntryFactory factory = new();

        List<MenuElement> mainPageElements = new();
        List<MenuElement> subPageElements = new();

        foreach ((ConfigDefinition def, ConfigEntryBase entry) in BingoUIPlugin.Instance.Config)
        {
            if (!factory.GenerateMenuElement(entry, out MenuElement? element) || element == null) continue;

            if (def.Section == "Counters.Individual")
            {
                subPageElements.Add(element);
            }
            else
            {
                mainPageElements.Add(element);
            }
        }

        PaginatedMenuScreenBuilder builder = new("Counter Toggles");
        builder.AddRange(subPageElements);
        AbstractMenuScreen subPage = builder.Build();

        MenuElement subPageButton = new TextButton("Counter Toggles")
        {
            OnSubmit = () => MenuScreenNavigation.Show(subPage)
        };
        mainPageElements.Add(subPageButton);

        SimpleMenuScreen mainPage = new("BingoUI");
        mainPage.AddRange(mainPageElements);

        return mainPage;
    }
}
