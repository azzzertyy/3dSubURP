using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class UIManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private StatManager statManager;
    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject UI;
    [SerializeField] private float lerpSpeed;

    public override void OnNetworkSpawn()
    {
        UI.SetActive(IsOwner);
    }

    private void Update()
    {
        ManageHealthBar();
    }

    private Color targetColor; // Color to lerp towards

    private void ManageHealthBar()
    {
        float health = statManager.GetStat("Health");
        float maxHealth = statManager.GetMaxStat("Health");
        float normalizedHealth = Mathf.Clamp01(health / maxHealth);

        // Calculate target color based on health
        float alpha = Mathf.Lerp(1f, 0f, normalizedHealth); // Lerp alpha from 1 to 0
        targetColor = new Color(1f, 1f, 1f, alpha);

        // Apply the color gradually
        healthBar.color = Color.Lerp(healthBar.color, targetColor, Time.deltaTime * lerpSpeed);
    }
}
