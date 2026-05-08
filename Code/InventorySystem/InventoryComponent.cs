using Sandbox;
using System.Collections.Generic;

public partial class InventoryComponent : Component
{
    [Property] public int MaxSlots { get; set; } = 16;

    public List<InventorySlot> Items { get; private set; } = new();

    public delegate void InventoryChangedHandler();
    public event InventoryChangedHandler OnInventoryChanged;

    public InventoryPanel TheInventoryPanel { get; set; }

    public bool AddItem(ItemDefinition def, int quantity = 1)
    {
        if (def == null || quantity <= 0) return false;

        int remaining = quantity;

        foreach (var slot in Items)
        {
            if (slot.Definition == def && slot.Quantity < def.MaxStack)
            {
                int canAdd = def.MaxStack - slot.Quantity;
                int add = canAdd < remaining ? canAdd : remaining;
                slot.Quantity += add;
                remaining -= add;
                if (remaining <= 0) break;
            }
        }

        while (remaining > 0 && Items.Count < MaxSlots)
        {
            int add = def.MaxStack < remaining ? def.MaxStack : remaining;
            Items.Add(new InventorySlot { Definition = def, Quantity = add });
            remaining -= add;
        }

        if (remaining < quantity)
        {
            OnInventoryChanged?.Invoke();
            return true;
        }

        return false;
    }

    public bool RemoveSlot(InventorySlot slot)
    {
        if (Items.Remove(slot))
        {
            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void RemoveItem(ItemDefinition def, int quantity = 1)
    {
        int remaining = quantity;
        for (int i = Items.Count - 1; i >= 0; i--)
        {
            if (Items[i].Definition == def)
            {
                if (Items[i].Quantity <= remaining)
                {
                    remaining -= Items[i].Quantity;
                    Items.RemoveAt(i);
                }
                else
                {
                    Items[i].Quantity -= remaining;
                    remaining = 0;
                    break;
                }
                if (remaining <= 0) break;
            }
        }
        if (remaining < quantity)
            OnInventoryChanged?.Invoke();
    }

    public void DropItem(InventorySlot slot)
    {
        if (slot == null || slot.Definition == null) return;

        var pickupGo = new GameObject();
        pickupGo.Name = slot.Definition.Name;
        pickupGo.WorldPosition = GameObject.WorldPosition + GameObject.WorldRotation.Forward * 50f;
        pickupGo.WorldRotation = Rotation.Identity;

        var itemPickup = pickupGo.Components.Create<ItemPickup>();
        itemPickup.ItemId = slot.Definition.Id;   // теперь строка
        itemPickup.Quantity = slot.Quantity;

        RemoveSlot(slot);
    }
}

public class InventorySlot
{
    public ItemDefinition Definition { get; set; }
    public int Quantity { get; set; } = 1;
}