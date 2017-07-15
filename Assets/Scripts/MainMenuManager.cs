using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour {

    [System.Serializable]
	public class Scene
    {
        public int sceneIndex;
        public Sprite sprite;
    }

    public List<Scene> scenes;
    public GameObject canvasObject;

    private Canvas canvas;

    private void Start()
    {
        scenes = new List<Scene>();
        canvas = canvasObject.GetComponent<Canvas>();
    }

    public bool Raycast(Ray ray, out RaycastHit hit)
    {
        return ExtraUtils.RaycastCanvas(canvas, ray, out hit);
    }


}
