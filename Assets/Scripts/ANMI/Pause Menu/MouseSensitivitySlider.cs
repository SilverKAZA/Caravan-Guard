using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[HelpURL("https://docs.unity3d.com/Manual/index.html")]
public class MouseSensitivitySlider : MonoBehaviour
{
    [Header("UI")]
    public Slider sensitivitySlider;                 // UI Slider (min..max)
    public Text valueText;                           // optional: показывать значение в UI

    [Header("Range")]
    public float minSensitivity = 0.1f;
    public float maxSensitivity = 5f;
    public float defaultSensitivity = 1f;

    [Header("PlayerPrefs")]
    public string prefsKey = "mouseSensitivity";

    [Header("Events")]
    // ѕозвол€ет другим скриптам подписатьс€ на изменение чувствительности
    // и получать новое значение как float
    public UnityEventFloat onSensitivityChanged;

    // “екущее значение (public дл€ удобства, но изменени€ должны идти через SetSensitivity)
    [HideInInspector]
    public float CurrentSensitivity;

    void Reset()
    {
        // попытка автоподв€зки
        sensitivitySlider = GetComponent<Slider>();
    }

    void Awake()
    {
        if (sensitivitySlider == null)
            Debug.LogError("MouseSensitivitySlider: sensitivitySlider не задан в инспекторе.");

        // »нициализируем слайдер диапазоном
        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = minSensitivity;
            sensitivitySlider.maxValue = maxSensitivity;
        }

        // ѕодпишем слушатель
        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.AddListener(OnSliderChanged);
    }

    void Start()
    {
        // загружаем из PlayerPrefs (если нет Ч используем default)
        CurrentSensitivity = PlayerPrefs.GetFloat(prefsKey, defaultSensitivity);

        // защита от выходов за границы
        CurrentSensitivity = Mathf.Clamp(CurrentSensitivity, minSensitivity, maxSensitivity);

        // установим слайдер и UI
        if (sensitivitySlider != null)
        {
            // временно отключаем слушатель, чтобы не вызвать рекурсию
            sensitivitySlider.onValueChanged.RemoveListener(OnSliderChanged);
            sensitivitySlider.value = CurrentSensitivity;
            sensitivitySlider.onValueChanged.AddListener(OnSliderChanged);
        }


        UpdateValueText(CurrentSensitivity);

        // оповещаем подписчиков о начальном значении
        onSensitivityChanged?.Invoke(CurrentSensitivity);
    }

    // ¬ызываетс€ при движении слайдера
    void OnSliderChanged(float newValue)
    {
        SetSensitivity(newValue);
    }

    // ”нифицированный метод дл€ изменени€ чувствительности программно
    void SetSensitivity(float val)
    {
        float clamped = Mathf.Clamp(val, minSensitivity, maxSensitivity);

        if (Mathf.Approximately(clamped, CurrentSensitivity))
            return; // не обновл€ем, если значение то же самое

        CurrentSensitivity = clamped;
        PlayerPrefs.SetFloat(prefsKey, CurrentSensitivity);
        PlayerPrefs.Save();

        UpdateValueText(CurrentSensitivity);

        onSensitivityChanged?.Invoke(CurrentSensitivity);
    }


    void UpdateValueText(float v)
    {
        if (valueText != null)
            valueText.text = v.ToString("F2");
    }
}

// ѕроста€ обЄртка, т.к. UnityEvent<float> не отображаетс€ в инспекторе как собственный тип
[System.Serializable]
public class UnityEventFloat : UnityEvent<float> { }
