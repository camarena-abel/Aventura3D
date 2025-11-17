using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    bool loading = true;

    [SerializeField]
    AudioClip sliderSound;

    [SerializeField]
    AudioSource sound;

    [SerializeField]
    AudioMixer mixer;

    [SerializeField]
    Slider masterVolume;

    [SerializeField]
    Slider musicVolume;

    [SerializeField]
    Slider soundsVolume;

    private void Start()
    {
        if (masterVolume)
            masterVolume.value = PlayerPrefs.GetFloat("VolMaster");
        if (musicVolume)
            musicVolume.value = PlayerPrefs.GetFloat("VolMusic");
        if (soundsVolume)
            soundsVolume.value = PlayerPrefs.GetFloat("VolSounds");
        UpdateMixer();
        loading = false;
    }

    private void UpdateMixer()
    {
        if (masterVolume)
            mixer.SetFloat("VolMaster", masterVolume.value);
        if (musicVolume)
            mixer.SetFloat("VolMusic", musicVolume.value);
        if (soundsVolume)
            mixer.SetFloat("VolSounds", soundsVolume.value);
    }

    public void OnChangeVolume()
    {
        if (loading)
            return;

        UpdateMixer();
        PlayerPrefs.SetFloat("VolMaster", masterVolume.value);
        PlayerPrefs.SetFloat("VolMusic", musicVolume.value);
        PlayerPrefs.SetFloat("VolSounds", soundsVolume.value);
        PlayerPrefs.Save();
        sound.PlayOneShot(sliderSound);
    }

}
