using UnityEngine;
using UnityEngine.UI;

public class DualHealthBar : MonoBehaviour
{
    [Header("Настройки здоровья")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("UI элементы")]
    public Image leftBar;
    public Image rightBar;

    [Header("Плавность движения")]
    [Range(0f, 20f)]
    public float smoothSpeed = 10f;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI(true);
    }

    private void Update()
    {
        // Тестовые клавиши (можно убрать)
        if (Input.GetKeyDown(KeyCode.H))
            TakeDamage(10f);
        if (Input.GetKeyDown(KeyCode.J))
            Heal(10f);

        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
            currentHealth = 0;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    private void UpdateHealthUI(bool instant = false)
    {
        if (leftBar == null || rightBar == null)
            return;

        float targetFill = currentHealth / maxHealth;

        if (instant)
        {
            leftBar.fillAmount = targetFill;
            rightBar.fillAmount = targetFill;
        }
        else
        {
            leftBar.fillAmount = Mathf.Lerp(leftBar.fillAmount, targetFill, Time.deltaTime * smoothSpeed);
            rightBar.fillAmount = Mathf.Lerp(rightBar.fillAmount, targetFill, Time.deltaTime * smoothSpeed);
        }
    }
}
