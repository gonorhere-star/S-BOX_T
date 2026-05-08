using Sandbox;
using System.Linq;

public class InteractionController : Component
{
    [Property] public float PickupRadius { get; set; } = 80f;
    [Property] public float HighlightRadius { get; set; } = 120f;

    private InventoryComponent inventory;
    private ItemPickup highlightedPickup;

    protected override void OnStart()
    {
        inventory = GameObject.Components.Get<InventoryComponent>();
    }

    protected override void OnUpdate()
    {
        if (inventory == null) return;

        // ========== Подбор предметов ==========
        var nearest = FindNearestPickup(HighlightRadius);

        if (nearest != highlightedPickup)
        {
            if (highlightedPickup != null)
                highlightedPickup.SetHighlight(false);

            highlightedPickup = nearest;

            if (highlightedPickup != null)
                highlightedPickup.SetHighlight(true);
        }

        if (highlightedPickup != null && Input.Pressed("Use"))
        {
            float dist = Vector3.DistanceBetween(WorldPosition, highlightedPickup.GameObject.WorldPosition);
            if (dist <= PickupRadius)
            {
                if (highlightedPickup.AddToInventory(inventory))
                {
                    highlightedPickup.SetHighlight(false);
                    highlightedPickup = null;
                }
            }
        }

        // ========== КОНСОЛЬНЫЙ ИНВЕНТАРЬ (нажми I) ==========
        if (Input.Pressed("Inventory")) // или "Tab", или "I" – как удобнее
        {
            PrintInventory();
        }
    }

    private void PrintInventory()
    {
        Log.Info("=== ИНВЕНТАРЬ ===");
        int i = 1;
        foreach (var slot in inventory.Items)
        {
            Log.Info($"{i}. {slot.Definition.Icon} {slot.Definition.Name} x{slot.Quantity}");
            i++;
        }
        if (inventory.Items.Count == 0)
            Log.Info("Пусто");

        // Вывод экипировки (если есть EquipmentComponent)
        var equip = GameObject.Components.Get<EquipmentComponent>();
        if (equip != null)
        {
            Log.Info("--- Экипировка ---");
            foreach (var kv in equip.Slots)
            {
                string item = kv.Value != null ? $"{kv.Value.Definition.Icon} {kv.Value.Definition.Name}" : "пусто";
                Log.Info($"{kv.Key}: {item}");
            }
        }
    }

    private ItemPickup FindNearestPickup(float maxRadius)
    {
        ItemPickup nearest = null;
        float nearestDist = maxRadius;
        var pickups = Scene.GetAllComponents<ItemPickup>();
        foreach (var pickup in pickups)
        {
            if (pickup.GameObject == null) continue;
            float dist = Vector3.DistanceBetween(WorldPosition, pickup.GameObject.WorldPosition);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = pickup;
            }
        }
        return nearest;
    }
}