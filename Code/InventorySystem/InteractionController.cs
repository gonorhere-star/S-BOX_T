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
        // Убедимся, что EquipmentComponent присутствует
        GameObject.Components.GetOrCreate<EquipmentComponent>();

        var panelObject = Scene.Directory.FindByName("InventoryPanelObject")?.FirstOrDefault();
        if (panelObject != null)
        {
            inventoryPanel = panelObject.Components.GetOrCreate<InventoryPanel>();
            inventoryPanel.Initialize(GameObject);
        }
    }

    protected override void OnUpdate()
    {
        if (inventory == null) return;

        // Открытие / закрытие инвентаря
        if (Input.Released("Inventory"))
        {
            inventoryPanel?.ToggleVisibility();

            // Показать/скрыть курсор мыши в зависимости от видимости инвентаря
            if (inventoryPanel != null)
            {
                bool isOpen = inventoryPanel.IsVisible;
                Mouse.Visible = isOpen;          // современный способ (s&box)

            }
        }

        // Действия с инвентарём (только когда он открыт)
        if (inventoryPanel != null && inventoryPanel.IsVisible)
        {
            if (Input.Released("Use"))
                inventoryPanel.DoPrimaryAction();

            if (Input.Released("Drop"))
                inventoryPanel.DoDropAction();
        }

        // Все внешние взаимодействия (поиск предметов, подбор) блокируем при открытом инвентаре
        bool inventoryOpen = inventoryPanel != null && inventoryPanel.IsVisible;
        if (!inventoryOpen)
        {
            if (lastPickupSearch >= SearchInterval)
            {
                lastPickupSearch = 0f;
                UpdateHighlightedPickup();
            }

            if (highlightedPickup != null && Input.Pressed("Use"))
            {
                TryPickUp();
            }
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