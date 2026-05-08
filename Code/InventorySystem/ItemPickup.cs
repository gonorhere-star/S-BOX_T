using Sandbox;

public class ItemPickup : Component
{
    [Property] public string ItemId { get; set; } = "sword";
    [Property] public int Quantity { get; set; } = 1;

    public ItemDefinition Definition => ItemDatabase.GetById(ItemId);
    private ModelRenderer renderer;

    protected override void OnStart()
{
    if (Definition == null)
    {
        Log.Error($"[ItemPickup] Предмет с Id '{ItemId}' не найден!");
        return;
    }

    Log.Info($"[ItemPickup] Загружаю модель: {Definition.WorldModel}");

    renderer = GameObject.Components.GetOrCreate<ModelRenderer>();
    if (!string.IsNullOrEmpty(Definition.WorldModel))
    {
        var model = Model.Load(Definition.WorldModel);
        if (model == null || model.IsError)
        {
            Log.Error($"[ItemPickup] Не удалось загрузить модель '{Definition.WorldModel}'");
        }
        else
        {
            renderer.Model = model;
            Log.Info($"[ItemPickup] Модель загружена успешно");
        }
    }

        // Добавляем коллайдер
        if (GameObject.Components.Get<Collider>() == null)
        {
            var box = GameObject.Components.Create<BoxCollider>();
            box.Scale = new Vector3(10, 10, 10);
            box.IsTrigger = false;
        }
    }

    public void SetHighlight(bool on)
    {
        if (renderer == null) return;
        renderer.Tint = on ? Definition.HighlightTint : Color.White;
    }

    public bool AddToInventory(InventoryComponent inventory)
    {
        if (Definition == null) return false;
        if (inventory.AddItem(Definition, Quantity))
        {
            GameObject.Destroy();
            return true;
        }
        return false;
    }
}