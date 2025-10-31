using UnityEngine;

public class DynamicSky : MonoBehaviour
{
    public Color dayColor = new Color(0.5f, 0.7f, 1f);
    public Color nightColor = new Color(0f, 0f, 0.1f);

    public Color fogDayColor = new Color(0.8f, 0.8f, 0.8f); // цвет тумана днем
    public Color fogNightColor = new Color(0.02f, 0.02f, 0.05f); // цвет тумана ночью

    public float cycleDuration = 60f; // цикл дня и ночи в секундах

    public Light directionalLight; // ссылка на ваш Directional Light

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        float t = (Mathf.Sin((timer / cycleDuration) * Mathf.PI * 2) + 1) / 2; // 0-1, синусоидально

        // Меняем цвет Skybox
        RenderSettings.skybox.SetColor("_Tint", Color.Lerp(nightColor, dayColor, t));
        // Меняем окружающее освещение
        RenderSettings.ambientLight = Color.Lerp(nightColor, dayColor, t);
        // Меняем цвет Directional Light
        if (directionalLight != null)
        {
            directionalLight.color = Color.Lerp(nightColor, dayColor, t);
            // Можно также менять интенсивность или угол, если нужно
        }

        // Меняем цвет тумана
        RenderSettings.fogColor = Color.Lerp(fogNightColor, fogDayColor, t);
    }
}