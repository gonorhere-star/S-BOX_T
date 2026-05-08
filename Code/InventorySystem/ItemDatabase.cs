using System;
using System.Collections.Generic;
using System.Linq;


public static class ItemDatabase
{
    public static List<ItemDefinition> AllDefinitions { get; } = new()
    {
new ItemDefinition("sword", "Меч", "🗡️", maxStack: 1, category: "Weapon", tags: new List<string>{"weapon", "mele"}, worldModel: "models/sword.vmdl_c", highlightTint: new Color(1f, 1f, 0.5f), description: "Острый меч для ближнего боя."),
new ItemDefinition("healthpotion", "Зелье здоровья", "🧪", maxStack: 10, category: "Зелья", tags: new List<string>{"зелье"}, worldModel: "models/bottle_1a.vmdl_c", highlightTint: new Color(0.5f, 1f, 0.5f), description: "Восстанавливает 50 здоровья."),
new ItemDefinition("helmet", "Шлем", "⛑️", maxStack: 1, category: "Armor", tags: new List<string>{"helmet"}, worldModel: "models/bottle_1a.vmdl_c", highlightTint: new Color(0.7f, 0.7f, 1f), description: "Защищает голову."),
new ItemDefinition("armor", "Броня", "🛡️", maxStack: 1, category: "Armor", tags: new List<string>{"armor"}, worldModel: "models/bottle_1a.vmdl_c", highlightTint: new Color(0.7f, 0.7f, 1f), description: "Надёжная броня."),
//new ItemDefinition("ID", "Name", "Icon", maxStack: 1, category: "Category", tags: new List<string>{"tag"}, worldModel: "models/bottle_1a.vmdl_c", highlightTint: new Color(0.5f, 1f, 0.5f)),

// для остальных тоже добавь highlightTint, иначе будет жёлтый
    };

    public static ItemDefinition GetById( string id ) => AllDefinitions.FirstOrDefault( d => d.Id == id );


    
}