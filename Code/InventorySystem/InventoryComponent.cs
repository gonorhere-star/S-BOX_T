using Sandbox;
using System.Collections.Generic;

public partial class InventoryComponent : Component
{
    [Property] public int MaxSlots { get; set; } = 16;

    public List<InventorySlot> Items { get; private set; } = new();

    public delegate void InventoryChangedHandler();
    public event InventoryChangedHandler OnInventoryChanged;

    public InventoryPanel TheInventoryPanel { get; set; }

    /// <summary> Попытаться добавить предмет по определению и количеству. Возвращает true, если добавлено полностью или частично. </summary>
    public bool AddItem(ItemDefinition def, int quantity = 1)
    {
        if (def == null || quantity <= 0) return false;

        int remaining = quantity;

        // Дополняем существующие неполные стаки
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

        // Создаём новые слоты при необходимости
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

    /// <summary> Выбросить предмет в мир (создать объект перед игроком). </summary>
    public void DropItem(InventorySlot slot)
    {
        if (slot == null || slot.Definition == null) return;

        // Создаём объект подбираемого предмета в мире
        var pickup = new GameObject();
        pickup.Name = slot.Definition.Name;
        pickup.WorldPosition = WorldPosition + WorldRotation.Forward * 50f; // перед игроком
        pickup.WorldRotation = Rotation.Identity;

        var itemPickup = pickup.Components.Create<ItemPickup>();
        itemPickup.ItemType = ItemDatabase.GetItemType(slot.Definition.Id); // нужен метод для обратного преобразования
        itemPickup.Quantity = slot.Quantity;

        // Удаляем из инвентаря
        RemoveSlot(slot);
    }
}

public class InventorySlot
{
    public ItemDefinition Definition { get; set; }
    public int Quantity { get; set; } = 1;
}