using UnityEngine;

public class DynamicSky : MonoBehaviour
{
    public Color dayColor = new Color(0.5f, 0.7f, 1f);
    public Color nightColor = new Color(0f, 0f, 0.1f);

    public Color fogDayColor = new Color(0.8f, 0.8f, 0.8f); // ���� ������ ����
    public Color fogNightColor = new Color(0.02f, 0.02f, 0.05f); // ���� ������ �����

    public float cycleDuration = 60f; // ���� ��� � ���� � ��������

    public Light directionalLight; // ������ �� ��� Directional Light

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        float t = (Mathf.Sin((timer / cycleDuration) * Mathf.PI * 2) + 1) / 2; // 0-1, �������������

        // ������ ���� Skybox
        RenderSettings.skybox.SetColor("_Tint", Color.Lerp(nightColor, dayColor, t));
        // ������ ���������� ���������
        RenderSettings.ambientLight = Color.Lerp(nightColor, dayColor, t);
        // ������ ���� Directional Light
        if (directionalLight != null)
        {
            directionalLight.color = Color.Lerp(nightColor, dayColor, t);
            // ����� ����� ������ ������������� ��� ����, ���� �����
        }

        // ������ ���� ������
        RenderSettings.fogColor = Color.Lerp(fogNightColor, fogDayColor, t);
    }
}