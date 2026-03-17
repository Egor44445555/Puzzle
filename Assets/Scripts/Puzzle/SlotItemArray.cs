using System;
using UnityEngine;

[Serializable]
public class SlotItemArray
{
    public string name;
    public int amount;
    public Sprite icon;

    public SlotItemArray(string _name, int _amount, Sprite _icon)
    {
        name = _name;
        amount = _amount;
        icon = _icon;
    }
}
