using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class MainMenuManager : MonoBehaviour 
{
    [SerializeField] private GameObject Canvas;
    [SerializeField] private GameObject AutorsPanel;
    [SerializeField] private GameObject SettingsPanel;

    private bool isAuthorsVisible = false;
    private bool isSettingsVisible = false;

    public AudioClip clickSound;
    public float volume = 1f;

    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        if (AutorsPanel != null)
        {
            AutorsPanel.SetActive(false);
        }
        if (SettingsPanel != null)
        {
            SettingsPanel.SetActive(false);
        }
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isAuthorsVisible)
        {
            PlayClickSound();
            AutorsPanel.SetActive(false);
            isAuthorsVisible = false;
        }

        if (isSettingsVisible)
        {
            PlayClickSound();
            SettingsPanel.SetActive(false);
            isSettingsVisible = false;
        }
    }

    public float delayBeforeLoad = 1f; 

    public void StartGame()
    {
        PlayClickSound();
        StartCoroutine(LoadSceneAfterDelay(delayBeforeLoad));
    }

    private IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(1);
    }

    public void OpenAutorsPanel()
    {
        if (AutorsPanel != null)
        {
            PlayClickSound();
            AutorsPanel.SetActive(true);
            isAuthorsVisible = true;
        }
    }

    public void Exit()
    {
        PlayClickSound();
        Application.Quit();
    }

    public void CloseAutorsPanel()
    {
        PlayClickSound();
        AutorsPanel.SetActive(false);
        isAuthorsVisible = false;
    }

    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound, volume);
        }
    }

    public void OpenSettingsPanel()
    {
        PlayClickSound();
        SettingsPanel.SetActive(true);
        isSettingsVisible = true;
    }

    public void CloseSettingsPanel()
    {
        PlayClickSound();
        SettingsPanel.SetActive(false);
        isSettingsVisible = false;
    }
}
