using System;
using UnityEngine;
using UnityEditor;

[Serializable]
public class TankManager
{
    // This class is to manage various settings on a tank.
    // It works with the GameManager class to control how the tanks behave
    // and whether or not players have control of their tank in the 
    // different phases of the game.

    public Color m_PlayerColor;                             // This is the color this tank will be tinted.
    public Transform m_SpawnPoint;                          // The position and direction the tank will have when it spawns.
    [HideInInspector] public int m_PlayerNumber;            // This specifies which player this the manager for.
    [HideInInspector] public string m_ColoredPlayerText;    // A string that represents the player with their number colored to match their tank.
    [HideInInspector] public GameObject m_Instance;         // A reference to the instance of the tank when it is created.
    [HideInInspector] public int m_Wins;                    // The number of wins this player has so far.


    private TankMovement m_Movement;                        // Reference to tank's movement script, used to disable and enable control.
    private TankShooting m_Shooting;                        // Reference to tank's shooting script, used to disable and enable control.
    private GameObject m_CanvasGameObject;                  // Used to disable the world space UI during the Starting and Ending phases of each round.


    public void Setup()
    {
        // Get references to the components.
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;


        // Atribuindo a cor escolhida pelo usuário ao tank
        // Verifica se o jogador 1 já teve cor atribuida
        // Se sim, atribui a cor para o 2° player
        if (PlayerPrefs.HasKey("Player1"))
        {
            ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString("Player1"), out m_PlayerColor);
            PlayerPrefs.DeleteKey("Player1");
            Debug.Log(m_PlayerColor);
        } else
        {
            ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString("Player2"), out m_PlayerColor);
            PlayerPrefs.DeleteKey("Player2");
            Debug.Log(m_PlayerColor);
        }


        // Set the player numbers to be consistent across the scripts.
        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        Color botColorref = RandomColorGenerator();

        if (m_Instance.layer == LayerMask.NameToLayer("Players"))
        {
            // Create a string using the correct color that says 'PLAYER 1' etc based on the tank's color and the player's number.
            m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGBA(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";
        }
        else
        {
            // Create a string using the correct color that says 'Bot 1' etc based on the tank's color and the Bot's number.
            m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGBA(botColorref) + ">Bot " + m_PlayerNumber + "</color>";
        }
        
        // Get all of the renderers of the tank.
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        // Go through all the renderers...
        for (int i = 0; i < renderers.Length; i++)
        {
            // ... set their material color to the color specific to this tank.
            if (m_Instance.layer == LayerMask.NameToLayer("Players"))
            {
                renderers[i].material.color = m_PlayerColor;
            } else
            {
                renderers[i].material.color = botColorref;
            }
        }
    }

    // Gera uma textura aleatória para os bots
    private Color RandomColorGenerator()
    {
        float[] refs = new float[4];
        for (int i = 0; i < 4; i++)
        {
            refs[i] = UnityEngine.Random.Range(0.0f, 1.0f);
        }

        Color c = new Color(refs[0], refs[1], refs[2], refs[3]);

        return c;
    }


    // Used during the phases of the game where the player shouldn't be able to control their tank.
    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    // Used during the phases of the game where the player should be able to control their tank.
    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    // Used at the start of each round to put the tank into it's default state.
    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}