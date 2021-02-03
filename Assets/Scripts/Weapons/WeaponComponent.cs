using System;
using Character;
using UnityEngine;

namespace Weapons
{
    [Serializable]
    public struct WeaponStats
    {
        public string Name;
        public float Damage;
        public int BulletsInClip;
        public int ClipSize;
        public int TotalBulletsAvailable;

        public float FireStartDelay;
        public float FireRate;
        public float FireDistance;
        public bool Repeating;

        public LayerMask WeaponHitLayer;
    }
    
    public class WeaponComponent : MonoBehaviour
    {
        public Transform HandPosition => GripIKLocation;
        [SerializeField] private Transform GripIKLocation;

        public bool Firing { get; private set; }
        public bool Reloading { get; private set; }

        public WeaponStats WeaponStats;

        protected WeaponHolder WeaponHolder;
        protected CrosshairScript Crosshair;

        public void Initialize(WeaponHolder weaponHolder, CrosshairScript crosshair)
        {
            WeaponHolder = weaponHolder;
            Crosshair = crosshair;
        }


        public virtual void StartFiring()
        {
            Firing = true;
            if (WeaponStats.Repeating)
            {
                InvokeRepeating(nameof(FireWeapon),WeaponStats.FireStartDelay, WeaponStats.FireRate);
            }
            else
            {
                FireWeapon();
            }
        }
        
        public virtual void StopFiring()
        {
            Firing = false;
            CancelInvoke(nameof(FireWeapon));
        }

        protected virtual void FireWeapon()
        {
            
        }

        public void StartReloading()
        {
            Reloading = true;
            ReloadWeapon();
        }
        

        private void ReloadWeapon()
        {
            int bulletToReload = WeaponStats.TotalBulletsAvailable - WeaponStats.ClipSize;
            if (bulletToReload < 0)
            {
                Debug.Log("Reload - Out of Ammo");
                WeaponStats.BulletsInClip += WeaponStats.TotalBulletsAvailable;
                WeaponStats.TotalBulletsAvailable = 0;
            }
            else
            {
                Debug.Log("Reload");
                WeaponStats.BulletsInClip = WeaponStats.ClipSize;
                WeaponStats.TotalBulletsAvailable -= WeaponStats.ClipSize;
            }
        }

     
    }
}
