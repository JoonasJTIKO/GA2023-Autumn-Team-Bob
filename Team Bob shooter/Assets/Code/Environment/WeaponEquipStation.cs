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

        /// <summary>
        /// Deactivates this station if the weapon is equipped by the player or not unlocked
        /// </summary>
        private void StartCheck()
        {
            if (GameInstance.Instance == null) return;

            if (GameInstance.Instance.GetWeaponLoadout().CheckEquipStatus(weapon) == true
                || !GameInstance.Instance.GetGameProgressionManager().CheckWeaponUnlockState(weapon))
            {
                collider.enabled = false;
                model.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Equips this stations weapon to the players active slot
        /// </summary>
        /// <param name="currentWeapon"></param>
        /// <returns></returns>
        public bool OnInteract(int currentWeapon)
        {
            GameInstance.Instance.GetWeaponLoadout().EquipWeapon(weapon, currentWeapon);
            collider.enabled = false;
            model.gameObject.SetActive(false);

            return true;
        }

        /// <summary>
        /// Called when the player equips another weapon unequipping their current weapon. Enables this station if this weapon was unequipped
        /// </summary>
        /// <param name="weapon"></param>
        private void CheckOnUnequip(EquippableWeapon weapon)
        {
            if (weapon.WeaponType == this.weapon.WeaponType)
            {
                collider.enabled = true;
                model.gameObject.SetActive(true);
            }
        }
    }
}
