using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Player{
    public class PlayerHealth : MonoBehaviour
    {
        [Range(0, 100)]
        [SerializeField] private int health = 100;
        [SerializeField] private int recoveryFactor;
        private float totalHealth;

        [Header("UI")]
        [SerializeField] private Image BloodImage;
        [SerializeField] private Image redImage;
        private Color alphaAmount;
        private Color alphaBackground;


        [SerializeField] private float recoveryRate;
        private float recoveryTimer;


        public bool IsDead { get; set; }
        private void Awake()
        {
            totalHealth = health;
            IsDead = false;
        }

        private void Update()
        {
            recoveryTimer += Time.deltaTime;
            if (recoveryTimer > recoveryRate)
            {
                StartCoroutine(RecoveryHealth());
            }
        }
        // Start is called before the first frame update
        public void ApplyDamage(int damage)
        {
            health -= damage;

            alphaBackground = redImage.color;
            alphaAmount = BloodImage.color;


            alphaBackground.a += ((float)damage / totalHealth);
            alphaAmount.a += ((float)damage / totalHealth);

            BloodImage.color = alphaAmount;
            if (redImage.color.a < 0.325f)
                redImage.color = alphaBackground;

            if (health <= 0)
            {
                IsDead = true;
                GameController.gameController.ShowGameOver();
            }

            recoveryTimer = 0f;
        }


        public IEnumerator RecoveryHealth()
        {

            while (health < totalHealth)
            {
                if (health < totalHealth)
                    health += recoveryFactor;

                alphaAmount.a -= (float)recoveryFactor / totalHealth;
                alphaBackground.a -= (float)recoveryFactor / totalHealth;

                BloodImage.color = alphaAmount;
                redImage.color = alphaBackground;
                yield return new WaitForSeconds(2f);
            }
        }
    }

}
