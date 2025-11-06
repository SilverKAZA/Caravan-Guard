using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [Header("Настройки маны")]
    public float maxMana = 100f;      // Максимальный запас маны
    public float currentMana;         // Текущее количество маны

    [Header("UI элемент маны")]
    public Image manaBar;             // Ссылка на Image с Fill Amount

    [Header("Плавность обновления (опционально)")]
    [Range(0f, 20f)]
    public float smoothSpeed = 10f;   // Скорость плавного изменения полоски

    private void Start()
    {
        // Инициализация
        currentMana = maxMana;

        // Обновляем UI сразу при старте
        UpdateManaUI(true);
    }

    private void Update()
    {
        // Для теста (можно удалить позже):
        if (Input.GetKeyDown(KeyCode.T))
            SpendMana(15f); // нажми T — потратить ману
        if (Input.GetKeyDown(KeyCode.Y))
            RestoreMana(15f); // нажми Y — восстановить ману

        // Обновляем визуальное отображение
        UpdateManaUI();
    }

    public void SpendMana(float amount)
    {
        currentMana -= amount;
        if (currentMana < 0)
            currentMana = 0;
    }

    public void RestoreMana(float amount)
    {
        currentMana += amount;
        if (currentMana > maxMana)
            currentMana = maxMana;
    }

    private void UpdateManaUI(bool instant = false)
    {
        if (manaBar == null)
            return;

        float targetFill = currentMana / maxMana;

        if (instant)
            manaBar.fillAmount = targetFill;
        else
            manaBar.fillAmount = Mathf.Lerp(manaBar.fillAmount, targetFill, Time.deltaTime * smoothSpeed);
    }
}
