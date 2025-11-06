using UnityEngine;

public class RotateAndOscillate : MonoBehaviour
{
    // параметры дл€ вращени€
    public float rotationSpeed = 50f; // скорость вращени€ по оси Z

    // параметры дл€ качани€
    public float floatAmplitude = 0.5f; // амплитуда колебаний
    public float floatFrequency = 1f;   // частота колебаний

    private Vector3 initialPosition;

    void Start()
    {
        // запомним начальную позицию дл€ осцилл€ции
        initialPosition = transform.localPosition;
    }

    void FixedUpdate()
    {
        // ¬ращение по оси Z
        transform.Rotate(Vector3.forward, rotationSpeed * Time.fixedDeltaTime);

        // ѕлавное качание вверх/вниз
        float newY = initialPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.localPosition = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}