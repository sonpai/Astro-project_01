// Mission.cs
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MissionObjective
{
    public enum ObjectiveType
    {
        Kill,
        Collect,
        ReachLocation // Example, implementation might vary
    }

    public ObjectiveType type;
    public string description;        // e.g., "Defeat 5 Goblins"
    public string targetID;           // e.g., "GoblinFighter", "HealthPotion", "NorthPass"
    public int requiredAmount;
    [HideInInspector] public int currentAmount; // Runtime progress

    public bool IsCompleted()
    {
        return currentAmount >= requiredAmount;
    }

    public void ResetProgress()
    {
        currentAmount = 0;
    }
}

[System.Serializable]
public class MissionReward
{
    public int coins;
    public List<string> itemIDs; // IDs of items to be given
    // Could also include XP, reputation changes, etc.
}

[CreateAssetMenu(fileName = "New Mission", menuName = "Quest System/Mission")]
public class Mission : ScriptableObject
{
    [Header("Mission Info")]
    public string missionID; // Unique ID for this mission (e.g., "MQ01_GOBLIN_SLAYER")
    public string missionTitle;
    [TextArea(3, 5)]
    public string missionDescription;
    public string questGiverNPCID; // ID of the NPC who gives this mission

    [Header("Objectives")]
    public List<MissionObjective> objectives;

    [Header("Rewards")]
    public MissionReward rewards;

    // Call this when a mission is first accepted to ensure progress is zeroed out
    public void InitializeMission()
    {
        foreach (var objective in objectives)
        {
            objective.ResetProgress();
        }
    }
}