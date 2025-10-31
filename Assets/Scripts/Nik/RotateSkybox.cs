using UnityEngine;

public class RotateSkybox : MonoBehaviour
{
    public float rotationSpeed = 10f;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
    }
}