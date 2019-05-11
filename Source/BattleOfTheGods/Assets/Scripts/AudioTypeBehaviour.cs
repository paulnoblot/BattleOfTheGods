using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioTypeBehaviour : MonoBehaviour
{
    [SerializeField] AudioType type = AudioType.Music;

    private void Start()
    {
        foreach (AudioSource source in GetComponents<AudioSource>())
        {
            AudioManager.AddAudioSource(type, source);
        }
    }

    private void OnDestroy()
    {
        foreach (AudioSource source in GetComponents<AudioSource>())
        {
            AudioManager.AddAudioSource(type, source);
        }
    }
}

public enum AudioType
{
    Music,
    SFX
}

public static class AudioManager
{
    static Dictionary<AudioType, List<AudioSource>> sources = new Dictionary<AudioType, List<AudioSource>>()
    {
        {AudioType.Music, new List<AudioSource>() },
        {AudioType.SFX, new List<AudioSource>() }
    };

    public static void AddAudioSource(AudioType _type, AudioSource _source)
    {
        _source.volume = PlayerPrefs.GetFloat("MasterVolume", 1f) * PlayerPrefs.GetFloat(_type.ToString() + "Volume", 1f);
        sources[_type].Add(_source);
    }

    public static void RemoveAudioSource(AudioType _type, AudioSource _source)
    {
        sources[_type].Remove(_source);
    }

    public static void UpdateMasterVolume(float _value)
    {
        PlayerPrefs.SetFloat("MasterVolume", _value);

        foreach (KeyValuePair<AudioType, List<AudioSource>> item in sources)
        {
            foreach (var source in item.Value)
            {
                if (source)
                {
                    source.volume = PlayerPrefs.GetFloat("MasterVolume", 1f) * PlayerPrefs.GetFloat(item.Key.ToString() + "Volume", 1f);
                }
            }
        }
    }

    public static void UpdateVolume(AudioType _type, float _value)
    {
        PlayerPrefs.SetFloat(_type.ToString() + "Volume", _value);

        foreach (var source in sources[_type])
        {
            if (source)
            {
                source.volume = PlayerPrefs.GetFloat("MasterVolume", 1f) * PlayerPrefs.GetFloat(_type.ToString() + "Volume", 1f);
            }
        }
    }

}
