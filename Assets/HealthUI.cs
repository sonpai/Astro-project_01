// HealthUI.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject heartPrefab; // Prefab of a UI Image representing one heart (with full/empty sprites)
    [SerializeField] private Transform heartsContainer; // Parent GameObject for heart prefabs (e.g., a HorizontalLayoutGroup)

    [Header("Heart Sprites (Optional, if heartPrefab doesn't handle states internally)")]
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite halfHeartSprite; // If damage can be 1 HP
    [SerializeField] private Sprite emptyHeartSprite;

    private List<Image> heartImages = new List<Image>();
    private const int HP_PER_HEART = 2;


    private void OnEnable()
    {
        PlayerHealth.Instance.OnHealthChanged += UpdateHealthDisplay;
        PlayerHealth.Instance.OnMaxHeartsChanged += RebuildHeartsUI;
    }

    private void OnDisable()
    {
        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.OnHealthChanged -= UpdateHealthDisplay;
            PlayerHealth.Instance.OnMaxHeartsChanged -= RebuildHeartsUI;
        }
    }

    private void Start()
    {
        // Initial setup based on PlayerHealth instance, if available
        if (PlayerHealth.Instance != null)
        {
            RebuildHeartsUI(PlayerHealth.Instance.CurrentMaxHearts);
            UpdateHealthDisplay(PlayerHealth.Instance.CurrentHealth, PlayerHealth.Instance.MaxHealth);
        }
        else
        {
            Debug.LogWarning("HealthUI: PlayerHealth.Instance not found on Start.");
        }
    }

    void RebuildHeartsUI(int maxHearts)
    {
        // Clear existing hearts
        foreach (Transform child in heartsContainer)
        {
            Destroy(child.gameObject);
        }
        heartImages.Clear();

        // Create new heart UI elements
        for (int i = 0; i < maxHearts; i++)
        {
            GameObject heartGO = Instantiate(heartPrefab, heartsContainer);
            Image heartImage = heartGO.GetComponent<Image>(); // Assuming heartPrefab has an Image component
            if (heartImage != null)
            {
                heartImages.Add(heartImage);
            }
            else
            {
                Debug.LogError("Heart prefab is missing an Image component!");
            }
        }
        UpdateHealthDisplay(PlayerHealth.Instance.CurrentHealth, PlayerHealth.Instance.MaxHealth); // Refresh display
    }

    void UpdateHealthDisplay(int currentHP, int maxTotalHP) // maxTotalHP is based on current max hearts
    {
        int currentFullHearts = currentHP / HP_PER_HEART;
        bool hasHalfHeart = (currentHP % HP_PER_HEART) == 1;

        for (int i = 0; i < heartImages.Count; i++)
        {
            if (i < currentFullHearts)
            {
                heartImages[i].sprite = fullHeartSprite; // Or heartImages[i].SetState(HeartState.Full);
            }
            else if (i == currentFullHearts && hasHalfHeart)
            {
                heartImages[i].sprite = halfHeartSprite; // Or heartImages[i].SetState(HeartState.Half);
            }
            else
            {
                heartImages[i].sprite = emptyHeartSprite; // Or heartImages[i].SetState(HeartState.Empty);
            }
            heartImages[i].enabled = true; // Ensure visible
        }

        // If max hearts are less than displayed hearts (e.g. after RebuildHeartsUI for fewer hearts)
        // This loop should align with heartImages.Count which is PlayerHealth.Instance.CurrentMaxHearts
    }
}