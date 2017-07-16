using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class Scene
{
    public int sceneIndex;
    public string name;
    public Sprite sprite; // thumbnail
}

public class MainMenuManager : MonoBehaviour {

    public List<Scene> scenes;
    public GameObject canvasObject;
    public GameObject buttonParent;
    public GameObject sceneButtonObject;
    
    private Canvas canvas;
    private int width = 4;
    
    private void Start()
    {
        canvas = canvasObject.GetComponent<Canvas>();
        CreateSceneButtons();
    }

    public void TriggerAction(Ray ray)
    {
        RaycastHit hit = new RaycastHit();
        if (ExtraUtils.RaycastCanvas(canvas, ray, out hit))
        {
            Button button = ExtraUtils.GetButtonAtPosition(canvas, hit.point);
            if (button != null)
            {
                button.onClick.Invoke();
            }
        }
    }

    public bool Raycast(Ray ray, out RaycastHit hit)
    {
        return ExtraUtils.RaycastCanvas(canvas, ray, out hit);
    }

    private void CreateSceneButtons()
    {
        for (int i = 0; i < Mathf.Min(scenes.Count, 8); i++)
        {
            GameObject buttonObj = Instantiate(sceneButtonObject);
            SceneButtonAction action = buttonObj.GetComponent<SceneButtonAction>();
            action.SetScene(scenes[i]);
            RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
            rectTransform.parent = buttonParent.GetComponent<RectTransform>();
            ExtraUtils.ResetRectTransform(rectTransform);
            SetButtonPosition(i, rectTransform);
        }
    }

    private void SetButtonPosition(int index, RectTransform rectTransform)
    {
        int i = index % width;
        int j = index / width;

        rectTransform.anchoredPosition = new Vector2(i * 110 - 165, 40 - j * 110);
    }
}
