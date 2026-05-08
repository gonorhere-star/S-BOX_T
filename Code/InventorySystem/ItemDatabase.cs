using System;
using System.Collections.Generic;
using System.Linq;

public enum ItemType
{
    Sword,
    HealthPotion,
    Shield,
    ManaPotion,
    Bomb,
    Key,
    Scroll,
    Helmet
}

public static class ItemDatabase
{
    public static List<ItemDefinition> AllDefinitions { get; } = new()
    {
        new ItemDefinition( "sword",     "Меч",           "🗡️",  maxStack: 1,   category: "Оружие",     tags: new List<string>{ "оружие", "ближний бой" } ),
        new ItemDefinition( "healthpotion", "Зелье здоровья", "🧪", maxStack: 10,  category: "Зелья",      tags: new List<string>{ "зелье", "восстановление" } ),
        new ItemDefinition( "shield",    "Щит",           "🛡️",  maxStack: 1,   category: "Броня",       tags: new List<string>{ "броня" } ),
        new ItemDefinition( "mana_potion","Зелье маны",    "💧",  maxStack: 10,  category: "Зелья",      tags: new List<string>{ "зелье", "мана" } ),
        new ItemDefinition( "bomb",      "Бомба",         "💣",  maxStack: 5,   category: "Снаряжение", tags: new List<string>{ "взрывчатка" } ),
        new ItemDefinition( "key",       "Ключ",          "🔑",  maxStack: 1,   category: "Квест",      tags: new List<string>{ "ключ" } ),
        new ItemDefinition( "scroll",    "Свиток",        "📜",  maxStack: 3,   category: "Магия",      tags: new List<string>{ "свиток" } ),
        new ItemDefinition( "helmet",    "Шлем",          "⛑️",  maxStack: 1,   category: "Броня",       tags: new List<string>{ "шлем", "броня" } ),
    };

    public static ItemDefinition GetById( string id ) => AllDefinitions.FirstOrDefault( d => d.Id == id );

    public static ItemDefinition GetByType( ItemType type )
    {
        string id = type switch
        {
            ItemType.ManaPotion => "mana_potion",
            _ => type.ToString().ToLowerInvariant()
        };
        return GetById( id );
    }

    public static ItemType GetItemType( string id )
    {
        foreach ( ItemType type in Enum.GetValues( typeof(ItemType) ) )
        {
            if ( GetByType( type )?.Id == id )
                return type;
        }
        return ItemType.Sword;
    }
}