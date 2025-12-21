using BepInEx.Configuration;
using BingoUI.Data;
using System;
using TeamCherry.Localization;
using UnityEngine;

namespace BingoUI.Components;

[RequireComponent(typeof(CurrencyCounter))]
internal class CurrencyCounterModifier : MonoBehaviour
{
    private ConfigEntry<bool>? GetConfigEntry()
    {
        if (gameObject.transform.parent.parent.gameObject.name == "Hud Canvas")
        {
            return ConfigSettings.ShowSpentRosariesInHud;
        }
        if (gameObject.transform.parent.parent.parent.gameObject.name == "Inv")
        {
            return ConfigSettings.ShowSpentRosariesInInventory;
        }

        // Counter above the rosary cannon
        return null;
    }

    private bool ShouldDisplay()
    {
        return GetConfigEntry()?.Value ?? false;
    }

    private CurrencyCounter _counter;
    private float origFontSize;
    private float origInitialAddTextX;
    private float origInitialSubTextX;


    void Awake()
    {
        _counter = gameObject.GetComponent<CurrencyCounter>();

        if (_counter.geoTextMesh.tmpText is TMProOld.TMP_Text text)
        {
            origFontSize = text.fontSize;
        }
        origInitialAddTextX = _counter.initialAddTextX;
        origInitialSubTextX = _counter.initialSubTextX;

        SetParams();

        if (GetConfigEntry() is ConfigEntry<bool> entry)
        {
            entry.SettingChanged += SettingChanged;
        }
    }

    private void SettingChanged(object sender, EventArgs e) => SetParams();

    private void SetParams()
    {
        if (ShouldDisplay())
        {
            if (_counter.geoTextMesh.tmpText is TMProOld.TMP_Text text)
            {
                text.fontSize = origFontSize * 0.6f;
            }

            _counter.initialAddTextX = origInitialAddTextX + 0.7f;
            _counter.initialSubTextX = origInitialSubTextX + 0.7f;
        }
        else
        {
            if (_counter.geoTextMesh.tmpText is TMProOld.TMP_Text text)
            {
                text.fontSize = origFontSize;
            }

            _counter.initialAddTextX = origInitialAddTextX;
            _counter.initialSubTextX = origInitialSubTextX;
        }
    }

    void LateUpdate()
    {
        if (!ShouldDisplay()) return;

        string spentGeoString;
        try
        {
            spentGeoString = string.Format(
                Language.Get("SPENT_GEO_FMT", $"Mods.{BingoUIPlugin.Id}"),
                CurrencyTracker.GetCurrencySpent(_counter.CounterType)
            );
        }
        catch (Exception)
        {
            // If a localizer didn't put {0} in the string I don't want to blow up the mod
            spentGeoString = $"{CurrencyTracker.GetCurrencySpent(_counter.CounterType)} spent";
        }

        _counter.geoTextMesh.Text = $"{_counter.counterCurrent}\n({spentGeoString})";
    }

    void OnDestroy()
    {
        if (GetConfigEntry() is ConfigEntry<bool> entry)
        {
            entry.SettingChanged -= SettingChanged;
        }
    }
}
