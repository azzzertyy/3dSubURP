using UnityEngine;


//Not networked, if something is weird it may need to be, for now it's probably fine.
public class StatManager : MonoBehaviour
{

    [SerializeField] private float maxHealth;
    [SerializeField] private float maxStamina;

    private float currentHealth;
    private float currentStamina;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    public void ModifyStat(string stat, float amount)
    {
        Debug.Log("Modifying " + stat + " by " + amount);
        switch (stat)
        {
            case "Health":
                currentHealth += amount;
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }
                break;
            case "Stamina":
                currentStamina += amount;
                if (currentStamina > maxStamina)
                {
                    currentStamina = maxStamina;
                }
                break;
            default:
                break;
        }
    }

    public void SetStat(string stat, float amount)
    {

        switch (stat)
        {
            case "Health":
                currentHealth = amount;
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }
                break;
            case "Stamina":
                currentStamina = amount;
                if (currentStamina > maxStamina)
                {
                    currentStamina = maxStamina;
                }
                break;
            default:
                break;
        }
    }
    public float GetStat(string stat)
    {
        switch (stat)
        {
            case "health":
                return currentHealth;
            case "stamina":
                return currentStamina;
            default:
                break;
        }
        return 0;
    }
}
