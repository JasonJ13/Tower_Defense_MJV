using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;
using System.Collections.Generic;



public class SelectionMap : MonoBehaviour
{

    private VisualElement scroller;

    private void AddButton(string mapName)
    {
        string lastWord = mapName.Split('/').Last(); 
        Button button = new Button(() =>
        {
            SelectMap(mapName);
        })
        {
            text = lastWord,
        };
        scroller.Add(button);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        UIDocument uiDocument = this.GetComponent<UIDocument>();
        scroller = uiDocument.rootVisualElement.Q("Map");

        List<string> filesPNG = new();
        filesPNG = Directory.GetFiles(Application.dataPath + "/Maps/", "*png", SearchOption.AllDirectories).ToList();
        Debug.Log("Tes");
        for (int i = 0; i < filesPNG.Count; i++)
        {
            AddButton(filesPNG[i]);
        }
    }

    private void SelectMap(string mapName)
    {
        Map.mapDiskPath = mapName;
        Debug.Log(Map.mapDiskPath);
        App.Instance.MapSelected();
    }

    private void Quit()
    {
        App.Instance.QuitMapSelection();
    }
}
