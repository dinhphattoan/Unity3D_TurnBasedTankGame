using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    public AudioSource pickupClip;
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;
    private TankShooting TankShooting;
    public float m_CurrentHealth;  
    private bool m_Dead;
    [Header("Particles Buff")]
    public GameObject Tank;
    public ParticleSystem ATKBuff;
    public ParticleSystem DefBuff;
    public ParticleSystem ATKGlow;
    public ParticleSystem DefGlow;
    public ParticleSystem HealthBuff;
    public AudioSource shieldBlockClip;
    public bool isATKBuff;
    public bool isDefBuff;
    [Obsolete]
    private void Start()
    {
        ATKBuff.Stop();
        DefBuff.Stop();
        ATKGlow.Stop();
        DefGlow.Stop();
        HealthBuff.Stop();
    }
    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
        TankShooting = Tank.GetComponent<TankShooting>();
        m_ExplosionParticles.gameObject.SetActive(false);
    }
    [Obsolete]
    private void Update()
    {
        if(isATKBuff)
        {
            if (ATKGlow.isStopped)
            {
                ATKGlow.Play();
                ATKBuff.Play();
            }
        }
        else
        {
            if(ATKGlow.isPlaying)
            {
                ATKGlow.Stop();
            }
        }
        if (isDefBuff)
        {
            if (DefGlow.isStopped)
            {
                DefGlow.Play();
                DefBuff.Play();
            }
        }
        else
        {
            if (DefGlow.isPlaying)
            {
                DefGlow.Stop();
            }
        }
    }
    
    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }
    
    public void TakeDamage(float amount)
    {
        if(!isDefBuff)
        {
            // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
            m_CurrentHealth -= amount;
            SetHealthUI();

            if (m_CurrentHealth <= 0f && !m_Dead)
            {
                OnDeath();
            }
        }
        else
        {
            shieldBlockClip.Play();
        }
        
    }
    public void earnHealth()
    {
        HealthBuff.Play();
        m_CurrentHealth += 30;if (m_CurrentHealth > m_StartingHealth) m_CurrentHealth = m_StartingHealth;
        pickupClip.Play();
        HealthBuff.Play();
        SetHealthUI();
        
    }

    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        m_Slider.value = m_CurrentHealth;
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth/m_StartingHealth);
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        m_Dead = true;
        m_ExplosionParticles.transform.position=transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

         gameObject.SetActive(false);
    }
}