using System.Collections.Generic;

/// <summary> Неизменяемое определение предмета (шаблон). </summary>
public class ItemDefinition
{
    public string Id { get; }
    public string Name { get; }
    public string Icon { get; }
    public int MaxStack { get; }       // максимум в одном слоте
    public string Category { get; }    // например "Оружие", "Зелья"
    public List<string> Tags { get; }  // теги для поиска/фильтров

    public ItemDefinition( string id, string name, string icon, int maxStack = 32, string category = "Разное", List<string> tags = null )
    {
        Id = id;
        Name = name;
        Icon = icon;
        MaxStack = maxStack;
        Category = category;
        Tags = tags ?? new List<string>();
    }
}