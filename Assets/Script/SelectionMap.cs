using UnityEngine;
using UnityEngine.UIElements;

public class SelectionMap : MonoBehaviour
{
    private VisualElement scroller;

    private void AddButton(string mapName)
    {
        Button button = new Button(() =>
        {
            SelectMap(mapName);
        })
        {
            text = mapName,
        };
        scroller.Add(button);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        UIDocument uiDocument = this.GetComponent<UIDocument>();
        scroller = uiDocument.rootVisualElement.Q("Map");

        for (int i = 0; i < 5; i++)
        {
            AddButton("bonjour");
        }
        AddButton("Salut");
    }

    private void SelectMap(string mapName)
    {
        Debug.Log(mapName);
    }
}
