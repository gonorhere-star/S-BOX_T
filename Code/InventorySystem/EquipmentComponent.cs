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
        { "Helmet", new EquipmentSlotDef { SlotName = "Helmet", AcceptedTag = "шлем" } },
        { "Armor",  new EquipmentSlotDef { SlotName = "Armor",  AcceptedTag = "броня" } },
        { "Weapon", new EquipmentSlotDef { SlotName = "Weapon", AcceptedTag = "оружие" } }
    };

    public Dictionary<string, InventorySlot> Slots =>
        slots.ToDictionary( kv => kv.Key, kv => kv.Value.Item );

    public delegate void EquipmentChangedHandler();
    public event EquipmentChangedHandler OnEquipmentChanged;

    private InventoryComponent Inventory => GameObject.Components.Get<InventoryComponent>();

    public bool Equip( InventorySlot slot, string slotName )
    {
        if ( !slots.TryGetValue( slotName, out var def ) || slot == null )
            return false;

        // Проверяем тег предмета
        if ( !slot.Definition.Tags.Contains( def.AcceptedTag ) )
            return false;

        // Если слот занят, сначала возвращаем старый предмет
        if ( def.Item != null )
        {
            if ( !Inventory.AddItem( def.Item.Definition, def.Item.Quantity ) )
                return false;
        }

        def.Item = slot;
        Inventory.RemoveSlot( slot );
        OnEquipmentChanged?.Invoke();
        return true;
    }

    public bool Unequip( string slotName )
    {
        if ( !slots.TryGetValue( slotName, out var def ) || def.Item == null )
            return false;

        var slot = def.Item;
        if ( Inventory.AddItem( slot.Definition, slot.Quantity ) )
        {
            def.Item = null;
            OnEquipmentChanged?.Invoke();
            return true;
        }
        return false;
    }
}