using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [System.Serializable]
    public class HealthSprite
    {
        public Sprite fullHealth;
        public Sprite emptyHealth;
    }

    [SerializeField] private HealthManager healthManager;
    [SerializeField] private GameObject healthIconPrefab;
    [SerializeField] private Transform iconsParent;
    [SerializeField] private float iconSpacing = 10f;
    [SerializeField] private HealthSprite healthSprite;

    private Image[] healthIcons;

    private void Start()
    {
        if (healthManager == null)
        {
            Debug.LogError("HealthManager not assigned to HealthDisplay!");
            return;
        }

        healthManager.OnHealthChanged.AddListener(UpdateHealthDisplay);
        InitializeHealthIcons(healthManager.GetMaxHealth());
    }

    private void InitializeHealthIcons(int maxHealth)
    {
        // 清除现有的图标
        foreach (Transform child in iconsParent)
        {
            Destroy(child.gameObject);
        }

        // 创建新的图标
        healthIcons = new Image[maxHealth];
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject icon = Instantiate(healthIconPrefab, iconsParent);
            RectTransform rectTransform = icon.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(i * iconSpacing, 0);
            healthIcons[i] = icon.GetComponent<Image>();
            healthIcons[i].sprite = healthSprite.fullHealth;
        }

        UpdateHealthDisplay(healthManager.GetCurrentHealth(), maxHealth);
    }

    private void UpdateHealthDisplay(int currentHealth, int maxHealth)
    {
        if (healthIcons.Length != maxHealth)
        {
            InitializeHealthIcons(maxHealth);
        }

        for (int i = 0; i < healthIcons.Length; i++)
        {
            healthIcons[i].sprite = (i < currentHealth) ? healthSprite.fullHealth : healthSprite.emptyHealth;
        }
    }

    private void OnDrawGizmos()
    {
        if (iconsParent != null && healthIconPrefab != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 iconSize = healthIconPrefab.GetComponent<RectTransform>().sizeDelta;
            for (int i = 0; i < 10; i++) // 假设最多显示10个图标
            {
                Vector3 position = iconsParent.position + new Vector3(i * iconSpacing, 0, 0);
                Gizmos.DrawWireCube(position, iconSize);
            }
        }
    }
}