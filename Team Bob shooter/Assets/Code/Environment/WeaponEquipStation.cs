using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class WeaponEquipStation : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private EquippableWeapon weapon;

        [SerializeField]
        private GameObject model;

        [SerializeField]
        private string promptText;

        private new Collider collider;

        public string PromptText
        {
            get { return promptText; }
        }

        private void Awake()
        {
            collider = GetComponent<Collider>();
        }

        private void Start()
        {
            StartCheck();
        }

        private void OnEnable()
        {
            WeaponLoadout.WeaponUnequipped += CheckOnUnequip;
        }

        private void OnDisable()
        {
            WeaponLoadout.WeaponUnequipped -= CheckOnUnequip;
        }

        private void StartCheck()
        {
            if (GameInstance.Instance == null) return;

            if (GameInstance.Instance.GetWeaponLoadout().CheckEquipStatus(weapon) == true)
            {
                collider.enabled = false;
            }
        }

        public bool OnInteract(int currentWeapon)
        {
            GameInstance.Instance.GetWeaponLoadout().EquipWeapon(weapon, currentWeapon);
            collider.enabled = false;

            return true;
        }

        private void CheckOnUnequip(EquippableWeapon weapon)
        {
            if (weapon.WeaponType == this.weapon.WeaponType)
            {
                collider.enabled = true;
            }
        }
    }
}
