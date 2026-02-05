using System;
using System.Collections.Generic;
using SI;


[Serializable]
public class ItemListWrapper
{
    public List<Potion> potions;
    public List<Weapon> weapons;
    public List<Armor> armors;
    public List<Item> items;

    public ItemListWrapper(List<Potion> potions, List<Weapon> weapons, List<Armor> armors, List<Item> items)
    {
        this.potions = potions;
        this.weapons = weapons;
        this.armors = armors;
        this.items = items;
    }
}