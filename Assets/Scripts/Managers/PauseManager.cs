using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseManager : MonoBehaviour
{

    public AudioMixerSnapshot paused;               // snapshot para áudio enquanto o jogo estiver pausado
    public AudioMixerSnapshot unpaused;             // snapshot para áudio enquanto o jogo não estiver pausado
    public GameManager gm;                          // referência ao script para controlar os tanks
    public ColorPicker picker;

    GameObject[] pauseObjects;                      // Referência ao menu de pausa do jogo
    public bool isActive =  true;                   // Flag para o estado dos controles do tank
    public static AsyncOperation async;             // Evento para controlar o carregamento dos modos de jogo
    public static bool reloaded = false;            // Flag que indica se o TankPicker sofreu reload

    void Start()
    {
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        HidePaused();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Função handler para pausar o jogo
            Pause();
        }
    }

    // Controla a pausa da cena e cria a referência para controlar via botão
    public void Pause()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            ShowPaused();
        }
        else
        {
            Time.timeScale = 1;
            HidePaused();
        }


        ChangeTankControl();
        isActive = !isActive;

        Lowpass();
    }

    void Lowpass()
    {
        if (Time.timeScale == 0)
        {
            paused.TransitionTo(.01f);
        }

        else

        {
            unpaused.TransitionTo(.01f);
        }
    }

    //Esconde os objetos com a tag ShowOnPause
    private void HidePaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
    }

    // Mostra os objetos com a tag ShowOnPause
    private void ShowPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
        }
    }

    //Reloads the Level
    public void Reload()
    {
        Pause();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    //loads inputted level
    public void LoadLevel(string level)
    {
        SceneManager.LoadSceneAsync("TankSelector");

        StartCoroutine(LoadYourAsyncScene(level));
    }

    IEnumerator LoadYourAsyncScene(string level)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        async = SceneManager.LoadSceneAsync(level);
        async.allowSceneActivation = false;

        //// Wait until the asynchronous scene fully loads
        //while (!asyncLoad.isDone)
        //{
        //    yield return null;
        //}

        yield return async;
    }

    public void ActivateScene()
    {
        // Carrega a cor escolhida pelo usuário
        PlayerPrefs.DeleteKey("Player1");
        PlayerPrefs.SetString("Player1", ColorUtility.ToHtmlStringRGBA(picker.CurrentColor));

        async.allowSceneActivation = true;
    }

    public void LoadMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    private void ChangeTankControl() {
        // Desativa cada um dos tanks na Cena, parando os tanks
            foreach (TankManager tm in gm.m_Tanks)
            {
                if (isActive)
                {
                    tm.DisableControl();
                } else
                {
                    tm.EnableControl();
                }
            }
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
		Application.Quit();
        #endif
    }
}