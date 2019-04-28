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
    public bool isActive =  true;                  // Flag para o estado dos controles do tank


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
        //Salva a preferência de cor definida pelo usuário
        PlayerPrefs.SetString("Player1", ColorUtility.ToHtmlStringRGBA(picker.CurrentColor));

        SceneManager.LoadScene(level);
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