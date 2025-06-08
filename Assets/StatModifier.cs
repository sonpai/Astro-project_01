// StatModifier.cs
using System;

public enum StatType
{
    Damage,
    Armor,
    Strength,
    Health,
    CritChance
    // Add any other stats you can think of!
}

[Serializable]
public class StatModifier
{
    public StatType Stat;
    public int Value;
}