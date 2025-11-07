using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ManaBar : MonoBehaviour
{
    [Header("Настройки маны")]
    public float maxMana = 100f;
    public float currentMana;

    [Header("UI элемент маны")]
    public Image manaBar;

    [Header("Плавность обновления")]
    [Range(0f, 20f)]
    public float smoothSpeed = 10f;

    [Header("Параметры исчезновения/появления")]
    public float fadeOutDuration = 1.5f;
    public float fadeInDuration = 0.8f;

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;
    private bool isAtMax = false; // Чтобы не запускать корутину по сто раз

    private void Start()
    {
        currentMana = maxMana;
        UpdateManaUI(true);

        // Добавляем или находим CanvasGroup
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 1f;
    }

    private void Update()
    {
        // Тестовые клавиши
        if (Input.GetKeyDown(KeyCode.T))
            SpendMana(15f);
        if (Input.GetKeyDown(KeyCode.Y))
            RestoreMana(15f);

        UpdateManaUI();

        // Проверяем состояние и запускаем нужный fade
        if (currentMana >= maxMana && !isAtMax)
        {
            isAtMax = true;
            StartFade(0f, fadeOutDuration);
        }
        else if (currentMana < maxMana && isAtMax)
        {
            isAtMax = false;
            StartFade(1f, fadeInDuration);
        }
    }

    public void SpendMana(float amount)
    {
        currentMana = Mathf.Max(0, currentMana - amount);
    }

    public void RestoreMana(float amount)
    {
        currentMana = Mathf.Min(maxMana, currentMana + amount);
    }

    private void UpdateManaUI(bool instant = false)
    {
        if (manaBar == null) return;

        float targetFill = currentMana / maxMana;

        if (instant)
            manaBar.fillAmount = targetFill;
        else
            manaBar.fillAmount = Mathf.Lerp(manaBar.fillAmount, targetFill, Time.deltaTime * smoothSpeed);
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
