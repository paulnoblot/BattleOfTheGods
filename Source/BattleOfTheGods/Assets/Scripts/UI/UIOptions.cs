using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOptions : MonoBehaviour
{
    [SerializeField] Slider sliderMasterVolume = null;
    [SerializeField] Text textMasterVolume = null;
    [SerializeField] Slider sliderMusicVolume = null;
    [SerializeField] Text textMusicVolume = null;
    [SerializeField] Slider sliderSFXVolume = null;
    [SerializeField] Text textSFXVolume = null;

    void Start()
    {
        sliderMasterVolume.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sliderMusicVolume.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sliderSFXVolume.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        textMasterVolume.text = ((int)(PlayerPrefs.GetFloat("MasterVolume", 1f) * 100)).ToString();
        textMusicVolume.text = ((int)(PlayerPrefs.GetFloat("MusicVolume", 1f) * 100)).ToString();
        textSFXVolume.text = ((int)(PlayerPrefs.GetFloat("SFXVolume", 1f) * 100)).ToString();

    }

    public void OnSliderMasterVolumeValueChange(float _value)
    {
        textMasterVolume.text = ((int)(_value * 100)).ToString();
        AudioManager.UpdateMasterVolume(_value);
    }

    public void OnSliderMusicVolumeValueChange(float _value)
    {
        textMusicVolume.text = ((int)(_value * 100)).ToString();
        AudioManager.UpdateVolume(AudioType.Music, _value);
    }

    public void OnSliderSFXVolumeValueChange(float _value)
    {
        textSFXVolume.text = ((int)(_value * 100)).ToString();
        AudioManager.UpdateVolume(AudioType.SFX, _value);
    }
}
