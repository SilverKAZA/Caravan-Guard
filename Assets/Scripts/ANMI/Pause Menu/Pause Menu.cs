using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    public AudioClip clickSound;
    public GameObject player;
    public GameObject Settings;
    public GameObject Autors;
    public GameObject PauseGameMenu;
    public bool PauseGame { get; private set; }
    private bool isSettingsVisible = false;
    private bool isAuthorsVisible = false;
    public float volume = 1f;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += OnSceneChanged;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        if (Settings != null)
        {
            Settings.SetActive(false);
            isSettingsVisible = false;
        }
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    public void OpenAutors()
    {
        if (Autors != null)
        {
            PlayClickSound();
            Autors.SetActive(true);
            isAuthorsVisible = true;
        }
    }

    public void CloseAutors()
    {
        if (isAuthorsVisible && Autors != null)
        {
            PlayClickSound();
            Autors.SetActive(false);
            isAuthorsVisible = false;
        }
    }

    private void OnSceneChanged(Scene previousScene, Scene newScene)
    {
        if (newScene.name == "Main Menu")
        {
            if (PauseGame) Resume();
            if (PauseGameMenu != null) PauseGameMenu.SetActive(false);
            enabled = false;
        }
        else
        {
            enabled = true;
        }
    }

    private void OnGUI()
    {
        if (!enabled) return;

        Event e = Event.current;
        if (e.isKey && e.keyCode == KeyCode.Escape && e.type == EventType.KeyUp)
        {
            if (PauseGame) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (player != null)
        {
            var ctrl = player.GetComponent<playerStamina>();
            if (ctrl != null)
                ctrl.allowMouseLook = true; // снова включаем вращение камеры
        }

        if (PauseGameMenu != null)
            PauseGameMenu.SetActive(false);

        Time.timeScale = 1f;
        PauseGame = false;
    }


    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (player != null)
        {
            var ctrl = player.GetComponent<playerStamina>();
            if (ctrl != null)
                ctrl.allowMouseLook = false; // блокируем камеру, но не выключаем компонент
        }

        if (PauseGameMenu != null)
            PauseGameMenu.SetActive(true);

        Time.timeScale = 0f;
        PauseGame = true;
    }

    public void LoadMenu()
    {
        PlayClickSound();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(0);
    }

    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound, volume);
        }
    }

    public void OpenSettings()
    {
        PlayClickSound();
        if (Settings != null)
        {
            Settings.SetActive(true);
            isSettingsVisible = true;

            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void CloseSettings()
    {
        if (isSettingsVisible && Settings != null)
        {
            PlayClickSound();
            Settings.SetActive(false);
            isSettingsVisible = false;
        }
    }
}
