using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class OptionsMenuController : MonoBehaviour
{

    public AudioMixer mixer;
    public Slider slider;

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("GameVolume", 0.75f);
    }
    public void SetLevel(float sliderValue)
    {
        mixer.SetFloat("GameVolume", Mathf.Log10(sliderValue) * 20);
    }
}