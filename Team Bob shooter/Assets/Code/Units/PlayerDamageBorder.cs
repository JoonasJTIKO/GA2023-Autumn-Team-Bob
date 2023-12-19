using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TeamBobFPS
{
    public class PlayerDamageBorder : BaseFixedUpdateListener
    {
        [SerializeField] UnitHealth unitHealth = null;
        [SerializeField] private Transform player;
        [SerializeField] private ScreenShake screenShake;

        private Volume volume = null;
        private Vignette vignette = null;
        private ColorAdjustments ca = null;

        public float intensity = 0;

        private Coroutine damageRoutine;
        private Coroutine healRoutine;

        protected override void Awake()
        {
            base.Awake();
            player = this.GetComponent<Transform>().parent;
        }

        private void Start()
        {
            unitHealth = player.GetComponent<UnitHealth>();
            if (unitHealth == null) return;

            volume = GetComponent<Volume>();
            volume.profile.TryGet(out vignette);
            volume.profile.TryGet(out ca);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            unitHealth = player.GetComponent<UnitHealth>();
            if (unitHealth == null) return;
            unitHealth.OnTakeDamage += RemoveHealth;
            unitHealth.OnDied += OnDie;
            unitHealth.OnHeal += GiveHealth;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (unitHealth == null) return;
            unitHealth.OnTakeDamage -= RemoveHealth;
            unitHealth.OnDied -= OnDie;
            unitHealth.OnHeal -= GiveHealth;
        }

        protected override void OnDestroy()
        {
            enabled = false;
        }

        private void RemoveHealth(float amount)
        {
            screenShake.Shake(1);
            GameInstance.Instance.GetInGameHudCanvas().ReduceHealth(amount);
            if (damageRoutine != null) StopCoroutine(damageRoutine);
            damageRoutine = StartCoroutine(TakeDamageEffect());
        }

        private void GiveHealth(float amount)
        {
            GameInstance.Instance.GetInGameHudCanvas().AddHealth(amount);
            if (healRoutine != null) StopCoroutine(healRoutine);
            healRoutine = StartCoroutine(HealEffect());
        }

        private void OnDie(float ExplosionStrength, Vector3 explosionPoint, EnemyGibbing.DeathType deathType = EnemyGibbing.DeathType.Normal)
        {
            ca.colorFilter.Override(Color.red);
            ca.contrast.Override(-100f);
        }

        private IEnumerator TakeDamageEffect()
        {
            if (UnityEngine.Random.Range(0, 2)  == 0)
            {
                GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_PLAYER_TAKE_DAMAGE, transform.position, 0.5f, make2D: true);
            }
            else
            {
                GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_PLAYER_TAKE_DAMAGE2, transform.position, 0.5f, make2D: true);
            }



            vignette.color.Override(Color.red);

            //if(isGreen)
            //{
            //   yield return new WaitForSeconds(0.5f);
            //}
            if (unitHealth.Health > 75)
            {
                yield return null;
            }
            if (unitHealth.Health <= 75)
            {
                vignette.intensity.Override(0.2f);
            }
            if (unitHealth.Health <= 50)
            {
                vignette.intensity.Override(0.4f);
            }
            if (unitHealth.Health <= 25)
            {
                vignette.intensity.Override(0.6f);
            }
            yield break;
        }

        private IEnumerator HealEffect()
        {
            vignette.color.Override(Color.green);
            float timer = 0f;
            while (timer < 0.2f)
            {
                vignette.intensity.Override(Mathf.Lerp(0, 0.4f, timer / 0.2f));
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            timer = 0f;
            while (timer < 0.4f)
            {
                vignette.intensity.Override(Mathf.Lerp(0.4f, 0, timer / 0.4f));
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            vignette.color.Override(Color.white);
        }
    }
}
