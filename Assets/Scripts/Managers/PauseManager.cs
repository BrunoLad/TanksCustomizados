using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseManager : MonoBehaviour
{

    public AudioMixerSnapshot paused;               // snapshot para áudio enquanto o jogo estiver pausado
    public AudioMixerSnapshot unpaused;             // snapshot para áudio enquanto o jogo não estiver pausado
    public GameManager gm;                          // referência ao script para controlar os tanks

    Canvas canvas;                                  // Referência ao menu de pausa do jogo
    private bool isActive =  true;                  // Flag para o estado dos controles do tank

    void Start()
    {
        canvas = GetComponent<Canvas>();  
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canvas.enabled = !canvas.enabled;
            Pause();

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

            isActive = !isActive;
        }
    }

    public void Pause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
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

    public void Quit()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
		Application.Quit();
        #endif
    }
}