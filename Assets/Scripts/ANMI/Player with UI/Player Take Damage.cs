using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DualHealthBar : MonoBehaviour
{
    [Header("Настройки здоровья")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("UI элементы")]
    public Image leftBar;
    public Image rightBar;

    [Header("Плавность движения полосок")]
    [Range(0f, 20f)]
    public float smoothSpeed = 10f;

    [Header("Плавность появления/исчезновения")]
    public float fadeOutDuration = 1.5f;
    public float fadeInDuration = 0.8f;

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;
    private bool isAtMax = false; // чтобы не запускать корутину повторно

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI(true);

        // Добавляем или ищем CanvasGroup на объекте
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 1f;
    }

    private void Update()
    {
        // Тестовые клавиши
        if (Input.GetKeyDown(KeyCode.H))
            TakeDamage(10f);
        if (Input.GetKeyDown(KeyCode.J))
            Heal(10f);

        UpdateHealthUI();

        // Проверяем состояние здоровья
        if (currentHealth >= maxHealth && !isAtMax)
        {
            isAtMax = true;
            StartFade(0f, fadeOutDuration); // исчезает при полном здоровье
        }
        else if (currentHealth < maxHealth && isAtMax)
        {
            isAtMax = false;
            StartFade(1f, fadeInDuration); // появляется при потере здоровья
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
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

    private void StartFade(float targetAlpha, float duration)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeCanvasGroup(targetAlpha, duration));
    }

    private IEnumerator FadeCanvasGroup(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
