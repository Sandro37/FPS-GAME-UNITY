using Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    private enum ShootMode
    {
        AUTO,
        SEMI
    }

    [Header("CONTROLE DO TIRO")]
    [Range(0f,100f)]
    [SerializeField] private float range;
    [SerializeField] private int totalBullet;
    [Range(0f, 100f)]
    [SerializeField] private int bulletsLeft;
    [SerializeField] private int currentBullets;
    [SerializeField] private Transform shooterPoint;

    [Header("Tempo do Tiro")]
    [Range(0.1f, 1f)]
    [SerializeField] private float fireRate;

    [Header("Sistema de particulas")]
    [SerializeField] private ParticleSystem fireEffect;

    [Header("Sounds")]
    [SerializeField] private AudioClip shootSound;

    [Header("Tiro efeito/buraco que fica")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject bulletImpact;

    [Header("Controle de espalhar o tiro")]
    [Range(0.01f, 0.1f)]
    [SerializeField] private float spredFactor;

    [SerializeField] private Animator anim;
    private float fireTimer;
    private bool isReloading;
    private AudioSource audioSource;

    [Header("Tipo de tiro")]
    [SerializeField]private ShootMode mode;

    [Header("Dano do tiro")]
    [SerializeField] private int damage;
    private bool shootInput;

    [Header("Mira")]
    [SerializeField] private Vector3 aimPos;
    [SerializeField] private float aimSpeed;
    [SerializeField] private GameObject obj;
    private Vector3 originalPos;


    [Header("UI")]
    [SerializeField] private Text ammoText;

    private PlayerHealth playerHealth;
    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        currentBullets = totalBullet;
        audioSource = GetComponent<AudioSource>();
        originalPos = obj.transform.localPosition;
        UpdateAmmoText();
    }

    // Update is called once per frame  
    void Update()
    {
        if (!playerHealth.IsDead)
        {
            Shoot();
            ToAim();
        }
    }
    private void FixedUpdate()
    {
        if (!playerHealth.IsDead)
            CheckAnimationStatus();
    }

    private void OnEnable()
    {
        UpdateAmmoText();
    }

    public void ToAim()
    {
        if (Input.GetButton("Fire2") && !isReloading)
        {
            obj.transform.localPosition = Vector3.Lerp(obj.transform.localPosition, aimPos, Time.deltaTime * aimSpeed);
        }
        else
        {
            obj.transform.localPosition = Vector3.Lerp(obj.transform.localPosition, originalPos, Time.deltaTime * aimSpeed);
        }
    }
    private void Shoot()
    {
        switch (mode)
        {
            case ShootMode.AUTO:
                shootInput = Input.GetButton("Fire1");
                break;
            case ShootMode.SEMI:
                shootInput = Input.GetButtonDown("Fire1");
                break;
        }

        if (shootInput)
        {
            if (currentBullets > 0)
            {
                Fire();
            }
            else if (bulletsLeft > 0)
            {
                DoReload();
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if ((currentBullets < totalBullet) && (bulletsLeft > 0))
                DoReload();
        }


        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }
    }
    private void CheckAnimationStatus()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        isReloading = info.IsName("Reload");
    }
    private void Fire()
    {
        if ((fireTimer < fireRate) || (isReloading) || (currentBullets <= 0))
        {
            return;
        }

        currentBullets--;
        UpdateAmmoText();
        fireEffect.Play();
        anim.CrossFadeInFixedTime("Fire", 0.01f);
        PlayShootSound();

        RaycastHit raycastHit;

        Vector3 shootDirection = shooterPoint.transform.forward;
        shootDirection += shooterPoint.TransformDirection
            (
                new Vector3
                (
                    Random.Range(-spredFactor, spredFactor),
                    Random.Range(-spredFactor, spredFactor)
                )
            );

        if(Physics.Raycast(shooterPoint.position, shootDirection, out raycastHit, range))
        {
            GameObject hitParticle = Instantiate(hitEffect, raycastHit.point, Quaternion.FromToRotation(Vector3.up, raycastHit.normal));
            GameObject bullet = Instantiate(bulletImpact, raycastHit.point, Quaternion.FromToRotation(Vector3.forward, raycastHit.normal));
            
            bullet.transform.SetParent(raycastHit.transform);

            if (raycastHit.transform.GetComponent<ObjHelth>())
            {
                raycastHit.transform.GetComponent<ObjHelth>().ApplyDamage(damage);
            }else if (raycastHit.transform.GetComponent<Soldier>())
            {
                Destroy(bullet);
                raycastHit.transform.GetComponent<Soldier>().ApplyDamage(damage);
            }

            Destroy(hitParticle, 1f);
            Destroy(bullet, 3f);
        }
        fireTimer = 0f;
    }
    private void DoReload()
    {
        if (isReloading)
            return;

        anim.CrossFadeInFixedTime("Reload", 0.01f);
    }

    public void Reload()
    {
        if (bulletsLeft <= 0)
            return;

        int bulletsToLoad = totalBullet - currentBullets;
        int bulletsToDeduct = (bulletsLeft >= bulletsToLoad) ? bulletsToLoad : bulletsLeft;

        bulletsLeft -= bulletsToDeduct;
        currentBullets += bulletsToDeduct;
        UpdateAmmoText();
    }

    public void PlayShootSound()
    {
        audioSource.PlayOneShot(shootSound);
    }

    public void UpdateAmmoText()
    {
        ammoText.text = $"{currentBullets} / {bulletsLeft}";    
    }
}


