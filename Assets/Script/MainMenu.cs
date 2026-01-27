using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private Button startButton;
    private Button optionButton;
    private Button quitButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        var uiDocument = this.GetComponent<UIDocument>();

        startButton = uiDocument.rootVisualElement.Q("Start") as Button;
        optionButton = uiDocument.rootVisualElement.Q("Option") as Button;
        quitButton = uiDocument.rootVisualElement.Q("Quit") as Button;

        startButton.RegisterCallback<ClickEvent>(start);
        optionButton.RegisterCallback<ClickEvent>(option);
        quitButton.RegisterCallback<ClickEvent>(quit);
    }

    private void start(ClickEvent evt) { }

    private void option(ClickEvent evt) { }

    private void quit(ClickEvent evt) { }
}
