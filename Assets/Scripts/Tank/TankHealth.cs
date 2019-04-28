using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;               // The amount of health each tank starts with.
    public Slider m_Slider;                             // The slider to represent how much health the tank currently has.
    public Image m_FillImage;                           // The image component of the slider.
    public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
    public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
    public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.
    public int lives = 3;                               // A quantidade de vidas que cada tank possui
    [HideInInspector] public Vector3 posInicial;        // A posição de renascimento do tank depois que esgota uma vida
    [HideInInspector] public Quaternion rotInitical;    // Rotação inicial do tank
    public GameObject livesUI;                          // Referência para as sprites de vidas


    private AudioSource m_ExplosionAudio;               // The audio source to play when the tank explodes.
    private ParticleSystem m_ExplosionParticles;        // The particle system the will play when the tank is destroyed.
    private float m_CurrentHealth;                      // How much health the tank currently has.
    private bool m_Dead;                                // Has the tank been reduced beyond zero health yet?


    private void Awake()
    {
        // Instantiate the explosion prefab and get a reference to the particle system on it.
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();

        // Get a reference to the audio source on the instantiated prefab.
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        // Disable the prefab so it can be activated when it's required.
        m_ExplosionParticles.gameObject.SetActive(false);

        // Pega o spawn point inicial do objeto
        posInicial = transform.position;
        rotInitical = transform.rotation;
    }


    private void OnEnable()
    {
        // When the tank is enabled, reset the tank's health and whether or not it's dead.
        // e também o número de vidas
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;
        lives = 3;

        // Update the health slider's value and color.
        // Atualiza também a quantidade corações a exibir
        SetHealthUI();
        SetLivesUI();
    }

    private void Update()
    {
        Image[] imgs = livesUI.GetComponentsInChildren<Image>();

        foreach (Image img in imgs)
        {
            img.transform.Rotate(0, 5, 0);
        }
    }


    public void TakeDamage(float amount)
    {
        // Reduce current health by the amount of damage done.
        m_CurrentHealth -= amount;

        // Change the UI elements appropriately.
        SetHealthUI();

        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
    }


    private void SetHealthUI()
    {
        // Set the slider's value appropriately.
        m_Slider.value = m_CurrentHealth;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }

    private void SetLivesUI()
    {
        // Pega as referências das imagens dos corações
        Image[] hearts = livesUI.GetComponentsInChildren<Image>();

        if (lives == 3)
        {
            foreach (Image heart in hearts)
            {
                // exibe todos caso a quantidade de vidas seja a total
                // Chamado quando a rodada é iniciada
                heart.enabled = true;
            }
        }
        else
        {
            for (int i = 2; i > (lives - 1); i--)
            {
                // Vai atualizando a UI conforme a vida vai diminuindo
                hearts[i].enabled = false;
            }
        }
    }


    private void OnDeath()
    {
        // Move the instantiated explosion prefab to the tank's position and turn it on.
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        // Play the particle system of the tank exploding.
        m_ExplosionParticles.Play();

        // Play the tank explosion sound effect.
        m_ExplosionAudio.Play();

        // atualiza o contador total de vidas
        lives--;

        // verifica se já esgotou o total de vida e termina o round
        // senão faz respawn do tank e reseta a posição/rotação
        if (lives <= 0) {
            // Turn the tank off.
            gameObject.SetActive(false);
            
            // Set the flag so that this function is only called once.
            m_Dead = true;
        }
        else {
            // Depois que uma das vidas se esgota, reseta a posição e rotação do tank que morreu
            transform.position = posInicial;
            transform.rotation = rotInitical;

            // reseta a vida do tank
            m_CurrentHealth = m_StartingHealth;

            // Reseta a barra de vida
            SetHealthUI();

            SetLivesUI();

            // Chama as funções que deixa o tank invulnerável e a 'animação'
            StartCoroutine(Flasher());
            StartCoroutine(Invincible());
        }
    }

    // Faz o objeto 'piscar' para mostrar que acabou de ser renascido
    IEnumerator Flasher()
    { 
        // cria uma referência para todos os Renderers do prefab
        Renderer[] rends = GetComponentsInChildren<Renderer>();

        // Itera por um valor arbitrário e alterna o mesh a cada .1s
        // Dando a ilusão de que o objeto está 'piscando'
        for (int i = 0; i < 15; i++)
        {

            foreach (Renderer r in rends)
            {
                // inativa o mesh para deixar a instância 'invisível'
                r.enabled = false;
            }

            yield return new WaitForSeconds(.1f);

            foreach (Renderer r in rends)
            {
                // reativa o mesh para torná-lo visível
                r.enabled = true;
            }

            yield return new WaitForSeconds(.1f);
        }
    }

    // Deixa o tank 'invencível' por 3 segundos logo após o respawn
    IEnumerator Invincible()
    {
        // Desativa o collider para não tomar dano
        // Transforma em kinematic para não se movimentar
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

        yield return new WaitForSeconds(3f);

        // Restaura as condições iniciais do tank
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }
}