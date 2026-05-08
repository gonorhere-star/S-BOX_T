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

    // Гарантируем, что EquipmentComponent существует
    var equip = GameObject.Components.GetOrCreate<EquipmentComponent>();

    var panelObject = Scene.Directory.FindByName("InventoryPanelObject")?.FirstOrDefault();
    if (panelObject != null)
    {
        inventoryPanel = panelObject.Components.GetOrCreate<InventoryPanel>();
        // Передаём панели нашего игрока (этот GameObject)
        inventoryPanel.Initialize(GameObject);
        Log.Info("[InteractionController] Панель инвентаря инициализирована");
    }
    else
    {
        Log.Error("[InteractionController] Объект 'InventoryPanelObject' не найден");
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
            // ---- Действия с инвентарём (только когда он открыт) ----
    if (inventoryPanel != null && inventoryPanel.IsVisible)
    {
        // Выбор слота цифрами 1-0 (т.е. 1..10)
        for (int i = 0; i < 10; i++)
        {
            var key = $"Slot{i+1}"; // например "Slot1"
            if (Input.Released(key))
            {
                inventoryPanel.SelectSlotByIndex(i);
                break;
            }
        }

        // Основное действие (надеть / использовать)
        if (Input.Released("Use")) // клавиша E
        {
            inventoryPanel.DoPrimaryAction();
        }

        // Выбросить предмет
        if (Input.Released("Drop")) // клавиша G – нужно назначить в Input Settings
        {
            inventoryPanel.DoDropAction();
        }

   // Снятие экипировки Alt+1/2/3

{
// Снятие экипировки (Z - шлем, X - броня, C - оружие)
if (Input.Released("Z")) inventoryPanel.UnequipSlot("Helmet");
if (Input.Released("X")) inventoryPanel.UnequipSlot("Armor");
if (Input.Released("C"))
{
    Log.Info("[InteractionController] Клавиша C нажата, вызываю UnequipSlot(Weapon)");
    inventoryPanel.UnequipSlot("Weapon");
}
}
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