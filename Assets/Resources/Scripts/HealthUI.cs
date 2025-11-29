using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image healthFillImage;
    public TextMeshProUGUI healthText;

    void Update()
    {
        if (playerHealth != null)
        {
            float current = playerHealth.GetCurrentHealth();
            float max = playerHealth.GetMaxHealth();
            healthFillImage.fillAmount = current / max;
            healthText.text = $"{Mathf.Ceil(current)} / {max}";
        }
    }
}


