using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using TMPro;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] float normalSensitivity;
    [SerializeField] float aimSensitivity;
    [SerializeField] LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] Transform debugTransform;
    [SerializeField] Transform pfBulletProjectile;
    
    [SerializeField] Transform vfxRed;
    [SerializeField] Transform vfxGreen;
    public TextMeshProUGUI ammunitionDisplay;
    public bool isAlivePlayer = true;

    public List<Gun> gun;
    private Gun currentGun;
    private int previousBulletsLeft;
   

    public int playerHealth = 100;
    [SerializeField] int damage;
    public float timeBetweenShooting, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsMax = 90;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    bool shooting, readyToShoot, reloading;
    bool allowInvoke = true;
    public MeshFilter gunMesh;



    ThirdPersonController thirdPersonController;
    StarterAssetsInputs starterAssetsInputs;
    Animator animator;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        bulletsLeft = magazineSize;
        readyToShoot = true;
        currentGun = gun[0];
        UpdateGunProperties();

    }
    private void Update()
    {
        MyInput();

        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsMax  + " / " + bulletsLeft);

    }
    void MyInput()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }

        
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();
            

        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();



        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeGun(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeGun(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeGun(2);
        }



        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 10f);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));

        }

        if(starterAssetsInputs.shoot)
        {

            if (allowButtonHold) shooting = starterAssetsInputs.shoot;
            else
            {
                shooting = starterAssetsInputs.shoot;
                starterAssetsInputs.shoot = false;
            }
            if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
            {
                bulletsShot = 0;
                StartShooting(hitTransform);

            }
        }
        if (allowButtonHold) shooting = Input.GetMouseButton(0);
        else shooting = Input.GetMouseButtonDown(0);

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
          {
            

             bulletsShot = 0;
             StartShooting(hitTransform);
            
        }

       
        starterAssetsInputs.shoot = false;
        

    }
    
    void StartShooting(Transform hitTransform)
    {
        readyToShoot = false;

        
        if (hitTransform != null)
        {
            if (hitTransform.GetComponent<Enemy>() != null)
            {
                Debug.Log(hitTransform.transform.name);
                Enemy enemy = hitTransform.transform.GetComponent<Enemy>();
                enemy.TakeDamage(damage);
                Instantiate(vfxGreen, debugTransform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(vfxRed, debugTransform.position, Quaternion.identity);
            }
        }

        bulletsLeft--;
        bulletsShot++;
 
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        
    }

    void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }
    void ReloadFinished()
    {
        int bulletsToReload = Mathf.Min(currentGun.magazineSize - bulletsLeft, bulletsMax);

        bulletsLeft += bulletsToReload;
        bulletsMax -= bulletsToReload;
        reloading = false;
    }
    void ChangeGun(int selectedGunIndex)
    {
        if (selectedGunIndex < gun.Count)
        {

            currentGun = gun[selectedGunIndex];

            UpdateGunProperties();
        }
    }
    void UpdateGunProperties()
    {
        gunMesh.mesh = currentGun.gunMesh;
        magazineSize = currentGun.magazineSize;
        damage = currentGun.damage;
        reloadTime = currentGun.reloadTime;
        timeBetweenShooting = currentGun.TimeBetweenShooting;
        timeBetweenShots = currentGun.TimeBetweenShots;
        allowButtonHold = currentGun.AllowToHold;
        bulletsLeft = Mathf.Min(bulletsLeft, currentGun.magazineSize);
        magazineSize = currentGun.magazineSize;

        int previousBulletsLeft = bulletsLeft;

        
        magazineSize = currentGun.magazineSize;
        bulletsLeft = Mathf.Max(previousBulletsLeft, magazineSize);
        bulletsLeft = Mathf.Max(previousBulletsLeft, magazineSize);
    }
    public void TakeDamage(int damage)
    {
        playerHealth -= damage;

        if (playerHealth <= 0)
        {
            Invoke(nameof(DestroyPlayer), 0f);
            animator.SetBool("isAlive", false);
            isAlivePlayer = false;
            
        }
    }
    public void DestroyPlayer()
    {
        Destroy(gameObject);
    }
}
