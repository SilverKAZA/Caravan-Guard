using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class playerStamina : MonoBehaviour
{
    [Header("UI Elements")]
    public Image staminaBar;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 16f;
    public float jumpForce = 3f;

    [Header("Energy for Running")]
    public float maxEnergy = 100f;
    public float energyConsumptionRate = 10f;
    public float energyRecoveryRate = 5f;

    [Header("Camera & Mouse")]
    public Transform cameraTransform;
    [HideInInspector] public bool allowMouseLook = true;
    public float mouseSensitivity = 2f;

    [Header("Fade Settings")]
    public float fadeOutDuration = 1.5f;
    public float fadeInDuration = 0.8f;

    private Rigidbody rb;
    private float rotationX = 0f;
    private bool isGrounded = false;
    private float currentEnergy;
    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;
    private bool isAtMax = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        currentEnergy = maxEnergy;
        canvasGroup = staminaBar.GetComponentInParent<CanvasGroup>() ?? staminaBar.transform.parent.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        if (PlayerPrefs.HasKey("mouseSensitivity"))
            mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity");
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleCameraRotation();
        HandleEnergy();
        UpdateStaminaUI();

        if (currentEnergy >= maxEnergy && !isAtMax)
        {
            isAtMax = true;
            StartFade(0f, fadeOutDuration);
        }
        else if (currentEnergy < maxEnergy && isAtMax)
        {
            isAtMax = false;
            StartFade(1f, fadeInDuration);
        }
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal"), v = Input.GetAxis("Vertical");
        Vector3 dir = (transform.forward * v + transform.right * h).normalized;
        bool run = Input.GetKey(KeyCode.LeftShift) && currentEnergy > 0 && (h != 0 || v != 0);
        float speed = run ? runSpeed : walkSpeed;

        Vector3 vel = rb.linearVelocity;
        vel.x = dir.x * speed;
        vel.z = dir.z * speed;
        rb.linearVelocity = vel;

        if (run)
            currentEnergy = Mathf.Max(0, currentEnergy - energyConsumptionRate * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Vector3 vel = rb.linearVelocity; vel.y = 0;
            rb.linearVelocity = vel;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void HandleCameraRotation()
    {
        if (!allowMouseLook) return;
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity;
        transform.Rotate(0, mx, 0);
        rotationX = Mathf.Clamp(rotationX - my, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    private void HandleEnergy()
    {
        if (!Input.GetKey(KeyCode.LeftShift) && currentEnergy < maxEnergy)
            currentEnergy = Mathf.Min(maxEnergy, currentEnergy + energyRecoveryRate * Time.deltaTime);
    }

    private void UpdateStaminaUI()
    {
        if (staminaBar)
            staminaBar.fillAmount = Mathf.Lerp(staminaBar.fillAmount, currentEnergy / maxEnergy, Time.deltaTime * 10f);
    }

    private void OnCollisionStay(Collision c)
    {
        foreach (var p in c.contacts)
            if (Vector3.Dot(p.normal, Vector3.up) > 0.5f) { isGrounded = true; return; }
    }

    private void OnCollisionExit(Collision c) => isGrounded = false;

    private void StartFade(float target, float dur)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeCanvas(target, dur));
    }

    private IEnumerator FadeCanvas(float target, float dur)
    {
        float start = canvasGroup.alpha, t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t / dur);
            yield return null;
        }
        canvasGroup.alpha = target;
    }

    public void SetMouseSensitivity(float newSensitivity) => mouseSensitivity = newSensitivity;
}
