using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Volume : MonoBehaviour
{
    public AudioMixerGroup _mixer;
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;    
    [SerializeField] private Slider _effectsSlider;
    public AudioMixerGroup effectsGroup;
    public AudioSource effectsSource;
    public AudioSource musicSource;

    private void Start()
    {
        _musicSlider.onValueChanged.AddListener(call => { ChangeMusicVolume(); });
        _effectsSlider.onValueChanged.AddListener(call => { ChangeEffectsVolume(); });
        _masterSlider.onValueChanged.AddListener(call => { ChangeMasterVolume(); });
    }
    public void ChangeMusicVolume()
    {
        _mixer.audioMixer.SetFloat(name: "Music", _musicSlider.value <= 0.0001f ? -80f : Mathf.Log10(_musicSlider.value) * 20);
    }

    public void ChangeEffectsVolume()
    {
        _mixer.audioMixer.SetFloat(name: "Sound", _effectsSlider.value <= 0.0001f ? -80f : Mathf.Log10(_effectsSlider.value) * 20);
    }

    public void ChangeMasterVolume()
    {
        _mixer.audioMixer.SetFloat(name: "Master", _masterSlider.value <= 0.0001f ? -80f : Mathf.Log10(_masterSlider.value) * 20);
    }
}