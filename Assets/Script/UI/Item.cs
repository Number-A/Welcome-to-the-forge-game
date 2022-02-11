using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {
    public enum ItemType{
        Sword,
        HealthPotion,
        XPPotion,
        Coin,
        MedKit
    }

    public ItemType itemType;
    public int amount;

    
}
