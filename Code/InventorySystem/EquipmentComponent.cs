using Sandbox;
using System.Collections.Generic;
using System.Linq;

public class EquipmentSlotDef
{
    public string SlotName { get; init; }
    public string AcceptedTag { get; init; }
    public InventorySlot Item { get; set; }
}

public class EquipmentComponent : Component
{
    private Dictionary<string, EquipmentSlotDef> slots = new()
    {
        { "Helmet", new EquipmentSlotDef { SlotName = "Helmet", AcceptedTag = "helmet" } },
        { "Armor",  new EquipmentSlotDef { SlotName = "Armor",  AcceptedTag = "armor" } },
        { "Weapon", new EquipmentSlotDef { SlotName = "Weapon", AcceptedTag = "weapon" } }
    };

    public Dictionary<string, InventorySlot> Slots =>
        slots.ToDictionary( kv => kv.Key, kv => kv.Value.Item );

    public delegate void EquipmentChangedHandler();
    public event EquipmentChangedHandler OnEquipmentChanged;

    private InventoryComponent Inventory => GameObject.Components.Get<InventoryComponent>();
public bool Equip( InventorySlot slot, string slotName )
{
    if ( !slots.TryGetValue( slotName, out var def ) || slot == null )
    {
        Log.Info($"[Equip] Неверный слот или предмет: slotName={slotName}, slot is null={slot == null}");
        return false;
    }

    if ( !slot.Definition.Tags.Contains( def.AcceptedTag ) )
    {
        Log.Info($"[Equip] Тег не подходит: нужен {def.AcceptedTag}, есть {string.Join(", ", slot.Definition.Tags)}");
        return false;
    }

    if ( def.Item != null )
    {
        if ( !Inventory.AddItem( def.Item.Definition, def.Item.Quantity ) )
        {
            Log.Info("[Equip] Не смог вернуть старый предмет в инвентарь");
            return false;
        }
    }

    def.Item = slot;
    Inventory.RemoveSlot( slot );
    OnEquipmentChanged?.Invoke();
    Log.Info($"[Equip] Успешно экипировано в {slotName}");
    return true;
}
public bool Unequip( string slotName )
{
    if ( !slots.TryGetValue( slotName, out var def ) || def.Item == null )
    {
        Log.Info($"[EquipmentComponent] Unequip: слот {slotName} пуст или не существует");
        return false;
    }

    var addResult = Inventory.AddItem( def.Item.Definition, def.Item.Quantity );
    Log.Info($"[EquipmentComponent] Попытка вернуть {def.Item.Definition.Name} в инвентарь: {addResult}, " +
             $"сейчас в инвентаре {Inventory.Items.Count}/{Inventory.MaxSlots} слотов");

    if ( addResult )
    {
        def.Item = null;
        OnEquipmentChanged?.Invoke();
        Log.Info($"[EquipmentComponent] Предмет успешно снят и возвращён в инвентарь");
        return true;
    }
    else
    {
        Log.Warning($"[EquipmentComponent] Не удалось вернуть предмет — нет места в инвентаре");
        return false;
    }
}
}