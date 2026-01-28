using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class Options : MonoBehaviour
{
    private Slider mainAudio;
    private Slider music;
    private Slider sfx;
    private Button backButton;
    
    [SerializeField] AudioMixer mixer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        var uiDocument = this.GetComponent<UIDocument>();

        mainAudio = uiDocument.rootVisualElement.Q("MainAudio") as Slider;
        music = uiDocument.rootVisualElement.Q("Music") as Slider;
        sfx = uiDocument.rootVisualElement.Q("SFX") as Slider;
        backButton = uiDocument.rootVisualElement.Q("Back") as Button;

        mixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("mainAudio", 0));
        mixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("music", 0));
        mixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("sfx", 0));
        mainAudio.SetValueWithoutNotify(PlayerPrefs.GetFloat("mainAudio", 0)+80);
        music.SetValueWithoutNotify(PlayerPrefs.GetFloat("music", 0)+80);
        sfx.SetValueWithoutNotify(PlayerPrefs.GetFloat("sfx", 0)+80);

        backButton.RegisterCallback<ClickEvent>(Quit);

        mainAudio.RegisterValueChangedCallback(evt => {mainAudio.value = evt.newValue; mixer.SetFloat("MasterVolume", mainAudio.value-80);PlayerPrefs.SetFloat("mainAudio", mainAudio.value-80);});
        music.RegisterValueChangedCallback(evt => {music.value = evt.newValue; mixer.SetFloat("MusicVolume", music.value-80);PlayerPrefs.SetFloat("music", music.value-80);});
        sfx.RegisterValueChangedCallback(evt => {sfx.value = evt.newValue; mixer.SetFloat("SFXVolume", sfx.value-80); PlayerPrefs.SetFloat("sfx", sfx.value-80);});

    }

    private void Quit(ClickEvent evt)
    {
        App.Instance.QuitOption();
    }
}
