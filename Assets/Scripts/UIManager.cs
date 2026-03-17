using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager main;
        
    [Header("Menu")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject successMenu;

    [Header("Audio")]
    [SerializeField] GameObject sounIconDisabled;
    [SerializeField] AudioSource music;
    [SerializeField] AudioSource grabEffect;
    [SerializeField] AudioSource throwEffect;
    [SerializeField] AudioSource collectedEffect;
    
    Camera cam;
    Vector2 startPoint = new Vector2();
    int currentLevel = 0;
    bool gamePause = false;
    bool endLevel = false;
    float timer = 0f;
    PlayerData playerData;
    string[] availableScenes;

    void Awake()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        availableScenes = new string[SceneManager.sceneCountInBuildSettings];
        
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            availableScenes[i] = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        }

        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (JsonSave.main != null)
        {
            playerData = JsonSave.LoadData<PlayerData>("playerData");
            currentLevel = playerData.currentLevel;
        }

        Time.timeScale = 1f;
        cam = Camera.main;

        if (music != null)
        {
            if (!IsSoundsActive())
            {
                music.Pause();
            }
            else
            {
                music.Play();
            }
        }
    }

    void Update()
    {
        if (endLevel)
        {
            timer += Time.deltaTime;
        }

        if (timer > 1f && endLevel)
        {
            StartCoroutine(SuccessLevel());                       
            endLevel = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePause();
        }

        if (Input.GetMouseButtonDown(0))
        {
            startPoint = Input.mousePosition;
        }
    }

    void TogglePause()
    {
        if (gamePause)
        {
            UnpauseGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (gamePause) return;

        gamePause = true;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
            CheckSoundIcon();
        }

        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        if (!gamePause) return;
        
        gamePause = false;

        if (GameObject.FindGameObjectWithTag("Popup"))
        {
            GameObject.FindGameObjectWithTag("Popup").SetActive(false);
        }

        Time.timeScale = 1f;
    }
    
    public bool IsGamePause()
    {
        return gamePause;
    }

    public void SwitchSound()
    {
        if (!IsSoundsActive())
        {
            sounIconDisabled.SetActive(false);
            PlayerPrefs.SetString("SoundEnable", "1");

            if (music != null)
            {
                music.Play();
            }
        }
        else
        {
            sounIconDisabled.SetActive(true);
            PlayerPrefs.SetString("SoundEnable", "0");

            if (music != null)
            {
                music.Pause();
            }
        }

        CheckSoundIcon();
    }

    public void PlayThrowEffect()
    {
        if (IsSoundsActive() && throwEffect != null)
        {
            throwEffect.Play();
        }  
    }

    public void PlayGrabEffect()
    {
        if (IsSoundsActive() && grabEffect != null)
        {
            grabEffect.Play();
        }        
    }

    public void PlayCollectedEffectEffect()
    {
        if (IsSoundsActive() && collectedEffect != null)
        {
            collectedEffect.Play();
        }        
    }    

    public void CheckSoundIcon()
    {
        if (!IsSoundsActive())
        {
            sounIconDisabled.SetActive(true);
        }
    }

    public bool IsSoundsActive()
    {
        return PlayerPrefs.GetString("SoundEnable") == "1";
    }

    public void EndLevel()
    {
        if (successMenu != null)
        {
            endLevel = true;
        }
    }
    
    public void StartLevel()
    {
        if (System.Array.Exists(availableScenes, scene => scene == "Level" + currentLevel))
        {
            SceneManager.LoadSceneAsync("Level" + currentLevel);
        }
        else
        {
            SceneManager.LoadSceneAsync("Level0");
        }
    }

    public void StartNextLevel()
    {
        string sceneName = "Level" + (currentLevel + 1);
        bool existNewLevel = System.Array.Exists(availableScenes, scene => scene == sceneName);

        if (JsonSave.main != null)
        {
            playerData = JsonSave.LoadData<PlayerData>("playerData");
            playerData.currentLevel = existNewLevel ? currentLevel + 1 : 0;
            JsonSave.SaveData(playerData, "playerData");
        }

        if (existNewLevel)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadSceneAsync("MainMenu");
        }
    }

    IEnumerator SuccessLevel()
    {
        yield return new WaitForSeconds(0.3f);
        gamePause = true;

        if (successMenu != null)
        {
            successMenu.SetActive(true);
        }
    }
   
    public void ToMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
