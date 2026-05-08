using Sandbox;

public class ItemPickup : Component
{
    [Property] public ItemType ItemType { get; set; } = ItemType.Sword;
    [Property] public int Quantity { get; set; } = 1;

    [Property] public Material DefaultMaterial { get; set; }
    [Property] public Material HighlightMaterial { get; set; }

    public ItemDefinition Definition => ItemDatabase.GetByType(ItemType);

    public void SetHighlight(bool on)
    {
        var renderer = GameObject.GetComponent<ModelRenderer>();
        if (renderer == null) return;

        if (DefaultMaterial != null && HighlightMaterial != null)
        {
            renderer.MaterialOverride = on ? HighlightMaterial : DefaultMaterial;
        }
        else
        {
            renderer.Tint = on ? new Color(1f, 1f, 0.5f) : Color.White;
        }
    }

    public bool AddToInventory(InventoryComponent inventory)
    {
        if (Definition == null) return false;

        // Попытаться добавить; если получилось, удалить объект
        if (inventory.AddItem(Definition, Quantity))
        {
            GameObject.Destroy();
            return true;
        }
        return false;
    }
}