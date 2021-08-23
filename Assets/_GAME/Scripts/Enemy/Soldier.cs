using Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]

public class Soldier : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent navMeshAgent;
    private PlayerHealth playerHealth;

    [Header("Particula do tiro")]
    [SerializeField] private ParticleSystem fireEffect;
    [Header("Ponto do tiro")]
    [SerializeField] private Transform shootPoint;
    [Range(1, 100)]
    [SerializeField] private float range = 1f;

    [Header("Referência do player/jogador")]
    [SerializeField] private GameObject player;

    [Header("Distancia para poder atacar")]
    [SerializeField] private float atkDistance;
    [SerializeField] private float followDistance;

    [Header("Probabilidade")]
    [SerializeField] private float atkProbabily; 

    [Header("DANO E VIDA")]
    [SerializeField] private int damage = 10;
    [SerializeField] private int health = 100;
    private bool isDead = false;

    [Header("Tempo entre um tiro e outro")]
    [Range(0.1f, 1f)]
    [SerializeField] private float fireRate = 0.1f;
    private float fireTimer;


    [Header("SOM")]
    [SerializeField] private AudioClip shootAudio;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerHealth.IsDead)
            if (navMeshAgent.enabled)
            { 
                float dist = Vector3.Distance(player.transform.position, transform.position);
                bool shoot = false;
                bool follow = (dist < followDistance);
                if (follow)
                {
                    if (dist < atkDistance)
                    {
                        shoot = true;
                        Fire();
                    }

                    navMeshAgent.SetDestination(player.transform.position);
                    transform.LookAt(player.transform.position);
                }

                if (!follow || shoot)
                {
                    navMeshAgent.SetDestination(transform.position);
                }

                anim.SetBool("shoot", shoot);
                anim.SetBool("run", follow);
            }

        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }
    }

    public void Fire()
    {
        if (fireTimer < fireRate)
        {
            return;
        }

        RaycastHit hit;
        PlayShootAudio();
        if (Physics.Raycast(shootPoint.position,shootPoint.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            if (hit.transform.GetComponent<PlayerHealth>())
            {
                hit.transform.GetComponent<PlayerHealth>().ApplyDamage(damage);
            }
        }

        fireEffect.Play();
        fireTimer = 0f;
    }

    public void ApplyDamage(int damage)
    {
        health -= damage;

        if (health <= 0  && !isDead)
        {
            navMeshAgent.enabled = false;
            anim.SetBool("shoot", false);
            anim.SetBool("run", false);
            anim.SetTrigger("die");
            isDead = true;
        }
    }

    public void PlayShootAudio()
    {
        audioSource.PlayOneShot(shootAudio);
    }
}
