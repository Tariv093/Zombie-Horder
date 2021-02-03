using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;

namespace Character
{
    public class WeaponHolder : MonoBehaviour
    {

        [SerializeField] private GameObject Weapon;

        [SerializeField] private Transform WeaponSocket;

        private Transform GripLocation;

        //Components
        private PlayerController PlayerController;
        private Animator PlayerAnimator;
        
        

        //Ref
        private Camera MainCamera;
        private WeaponComponent EquippedWeapon;
    
        //Animator Hashes
        private readonly int AimVerticalHash = Animator.StringToHash("AimVertical");
        private readonly int AimHorizontalHash = Animator.StringToHash("AimHorizontal");
        private readonly int IsFiringHash = Animator.StringToHash("IsFiring");
        private readonly int IsReloadingHash = Animator.StringToHash("IsReloading");

        private void Awake()
        {
            PlayerController = GetComponent<PlayerController>();
            PlayerAnimator = GetComponent<Animator>();
        
            MainCamera = Camera.main;
        }

        // Start is called before the first frame update
        private void Start()
        {
            GameObject spawnedWeapon = Instantiate(Weapon, WeaponSocket.position, WeaponSocket.rotation);

            if (!spawnedWeapon) return;

            spawnedWeapon.transform.parent = WeaponSocket;
            EquippedWeapon = spawnedWeapon.GetComponent<WeaponComponent>();
            GripLocation = EquippedWeapon.HandPosition;
            EquippedWeapon.Initialize(this, PlayerController.CrosshairComponent);
            
            PlayerEvents.Invoke_OnWeaponEquipped(EquippedWeapon);
        }

        public void OnLook(InputValue delta)
        {
            Vector3 independentMousePosition =
                MainCamera.ScreenToViewportPoint(PlayerController.CrosshairComponent.CurrentMousePosition);


            Debug.Log(independentMousePosition);
            PlayerAnimator.SetFloat(AimVerticalHash, independentMousePosition.y);
            PlayerAnimator.SetFloat(AimHorizontalHash, independentMousePosition.x);
        }

        public void OnFire(InputValue button)
        {
            if (button.isPressed)
            {
                PlayerController.IsFiring = true;
                PlayerAnimator.SetBool(IsFiringHash, PlayerController.IsFiring);
                EquippedWeapon.StartFiring();
            }
            else
            {
                PlayerController.IsFiring = false;
                PlayerAnimator.SetBool(IsFiringHash, PlayerController.IsFiring);
                EquippedWeapon.StopFiring();
            }
            
        }
        
        public void OnReload(InputValue button)
        {
            StartReloading();
     
            
        }

        public void StartReloading()
        {
            PlayerController.IsReloading = true;
            PlayerAnimator.SetBool(IsReloadingHash, PlayerController.IsReloading);
            EquippedWeapon.StartReloading();
            
            InvokeRepeating(nameof(StopReloading),0, 0.1f);
        }
        
        public void StopReloading()
        {
            if (PlayerAnimator.GetBool(IsReloadingHash)) return;

            PlayerController.IsReloading = false;
            
            //EquippedWeapon.StopReloading();
            
            CancelInvoke(nameof(StopReloading));
        }

        private void OnAnimatorIK(int layerIndex)
        {
            PlayerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            PlayerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, GripLocation.position);
        }
        
    }
}
