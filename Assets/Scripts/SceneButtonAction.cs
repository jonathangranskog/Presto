using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneButtonAction : MonoBehaviour {

    public GameObject thumbnailObject;
    public GameObject textObject;
    
    private Scene scene;
    private Image thumbnail;
    private Text text;

    private void Awake()
    {
        text = textObject.GetComponent<Text>();
        thumbnail = thumbnailObject.GetComponent<Image>();
    }

    public void LoadScene()
    {
        SceneManager.LoadSceneAsync(scene.sceneIndex);
    }

    public void SetScene(Scene s)
    {
        scene = s;
        if (s.sprite != null) thumbnail.sprite = s.sprite;
        text.text = s.name;
    }
}
