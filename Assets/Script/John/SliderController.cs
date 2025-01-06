using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class SliderController : MonoBehaviour
{
    public static SliderController instance;

    public Slider[] sliders;
    public TMP_Text[] texts;

    public GameObject panel;

    private void Awake()
    {
        // Ensure only one instance of SliderController exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep the SliderController between scene changes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void Start()
    {
        for (int i = 0; i < sliders.Length; i++) {
            SliderChange(i);
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            // Allow the user to press the Escape key to quit the application
            if (Input.GetKeyDown(KeyCode.Space ))
            {
                if (!panel.activeInHierarchy)
                {
                    if (EcosystemManager.instance != null)
                    {
                        EcosystemManager.instance.showGUI = false;
                        PauseScene();
                    }
                }
                else {
                    EcosystemManager.instance.showGUI = true;
                    ResumeScene();
                }
                
               
            }
        }
    }

    public void SliderChange(int sliderIndex)
    {
        texts[sliderIndex].text = sliders[sliderIndex].value.ToString("0.00");
    }

    public void LoadScene(int sceneIndex)
    {
        panel.SetActive(false);
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else {
            Resources.UnloadUnusedAssets();
            SceneManager.LoadScene(1);
        }
        
    }

    public void PauseScene() {
        panel.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void ResumeScene()
    {
        panel.SetActive(false);
        Time.timeScale = EcosystemManager.instance.currentTimeScale;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
