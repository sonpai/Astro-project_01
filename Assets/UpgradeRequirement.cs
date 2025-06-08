// UpgradeRequirement.cs
using System;

[Serializable]
public class UpgradeRequirement
{
    public ItemData requiredItem; // The material needed (e.g., Goblin Ear ItemData)
    public int requiredQuantity;  // How many are needed (e.g., 5)
}