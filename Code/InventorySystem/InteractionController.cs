using Sandbox;
using System.Linq;

public class InteractionController : Component
{
    [Property] public float PickupRadius { get; set; } = 80f;
    [Property] public float HighlightRadius { get; set; } = 120f;

    private InventoryComponent inventory;
    private ItemPickup highlightedPickup;
    private InventoryPanel inventoryPanel;

    private TimeSince lastPickupSearch = 0f;
    private const float SearchInterval = 0.1f;

    protected override void OnStart()
    {
        inventory = GameObject.Components.Get<InventoryComponent>();

        var panelObject = Scene.Directory.FindByName("InventoryPanelObject")?.FirstOrDefault();
        if (panelObject != null)
        {
            inventoryPanel = panelObject.Components.GetOrCreate<InventoryPanel>();
        }
    }

    protected override void OnUpdate()
    {
        if (inventory == null) return;

        // Открытие / закрытие инвентаря
        if (Input.Released("Inventory"))
        {
            inventoryPanel?.ToggleVisibility();
        }

        // Поиск ближайшего предмета с интервалом
        if (lastPickupSearch >= SearchInterval)
        {
            lastPickupSearch = 0f;
            UpdateHighlightedPickup();
        }

        // Подбор предмета
        if (highlightedPickup != null && Input.Pressed("Use"))
        {
            TryPickUp();
        }
    }

    private void UpdateHighlightedPickup()
    {
        var nearest = FindNearestPickup(HighlightRadius);
        if (nearest == highlightedPickup)
            return;

        highlightedPickup?.SetHighlight(false);
        highlightedPickup = nearest;
        highlightedPickup?.SetHighlight(true);
    }

    private void TryPickUp()
    {
        float dist = Vector3.DistanceBetween(WorldPosition, highlightedPickup.GameObject.WorldPosition);
        if (dist > PickupRadius)
            return;

        if (highlightedPickup.AddToInventory(inventory))
        {
            highlightedPickup.SetHighlight(false);
            highlightedPickup = null;
        }
    }

    private ItemPickup FindNearestPickup(float maxRadius)
    {
        ItemPickup nearest = null;
        float nearestDist = maxRadius;

        foreach (var pickup in Scene.GetAllComponents<ItemPickup>())
        {
            if (pickup.GameObject == null)
                continue;

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