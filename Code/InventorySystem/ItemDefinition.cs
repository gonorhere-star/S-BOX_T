public class ItemDefinition
{
    public string Id { get; }
    public string Name { get; }
    public string Icon { get; }
    public int MaxStack { get; }
    public string Category { get; }
    public List<string> Tags { get; }
    public string WorldModel { get; }
    public Color HighlightTint { get; }

    public ItemDefinition(string id, string name, string icon, int maxStack = 32, string category = "Разное", List<string> tags = null, string worldModel = null, Color? highlightTint = null)
    {
        Id = id;
        Name = name;
        Icon = icon;
        MaxStack = maxStack;
        Category = category;
        Tags = tags ?? new List<string>();
        WorldModel = worldModel;
        HighlightTint = highlightTint ?? new Color(1f, 1f, 0.5f); // жёлтый по умолчанию
    }
}