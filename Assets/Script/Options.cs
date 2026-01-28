using UnityEngine;
using UnityEngine.UIElements;

public class Options : MonoBehaviour
{
    private Slider mainAudio;
    private Slider music;
    private Slider sfx;
    private Button backButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        var uiDocument = this.GetComponent<UIDocument>();

        mainAudio = uiDocument.rootVisualElement.Q("MainAudio") as Slider;
        music = uiDocument.rootVisualElement.Q("Music") as Slider;
        sfx = uiDocument.rootVisualElement.Q("SFX") as Slider;
        backButton = uiDocument.rootVisualElement.Q("Quit") as Button;

        backButton.RegisterCallback<ClickEvent>(Quit);


        mainAudio.RegisterValueChangedCallback(evt => {mainAudio.value = evt.newValue;});
        music.RegisterValueChangedCallback(evt => {music.value = evt.newValue;});
        sfx.RegisterValueChangedCallback(evt => {sfx.value = evt.newValue;});

    }

    private void Quit(ClickEvent evt)
    {
        App.Instance.QuitOption();
    }
}
