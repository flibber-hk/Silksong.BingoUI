using MonoDetour.HookGen;

[assembly: MonoDetourTargets(typeof(UIManager))]
[assembly: MonoDetourTargets(typeof(HeroController))]
[assembly: MonoDetourTargets(typeof(FullQuestBase))]
[assembly: MonoDetourTargets(typeof(CollectableRelicManager))]
[assembly: MonoDetourTargets(typeof(CurrencyCounterBase))]
[assembly: MonoDetourTargets(typeof(CurrencyCounter))]
[assembly: MonoDetourTargets(typeof(CollectableItemManager))]
[assembly: MonoDetourTargets(typeof(ToolItem))]
[assembly: MonoDetourTargets(typeof(CollectableItemPickup))]
[assembly: MonoDetourTargets(typeof(PlayMakerNPC))]
