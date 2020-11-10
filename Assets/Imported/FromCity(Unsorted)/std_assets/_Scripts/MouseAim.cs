using Assets.Imported.std_assets._New_anim;
using UnityEngine;

namespace Assets.Imported.std_assets._Scripts
{
    public class MouseAim : MonoBehaviour {
        public AudioClip GunShot;
        public GameObject MuzzleFlash;
        public Transform spine;
        public Transform weapon;
        public GameObject crosshairImage;
        public Vector2 xLimit = new Vector2(-40f, 40f);
        public Vector2 yLimit = new Vector2(-40f, 40f);
        private float xAxis = 0f;
        private float yAxis = 0f;

        public Transform ShootPoint { get; set; }
        private GameObject player;
        private BasicController2 bc2;
        private Vector3 fromPosition, Destination;
    
        void Start() {
            MuzzleFlash.transform.localScale *= 0.1f;
            player = GameObject.FindGameObjectWithTag("Player");
            bc2 = player.GetComponent<BasicController2>();
            crosshairImage.SetActive(false);
        }

        public void LateUpdate() {
            /*if (bc2.HasRifle) {
            RotateSpine();
            ShowCrosshairIfRaycastHit();
        }*/
        }

        private void RotateSpine() {
            yAxis += Input.GetAxis("Mouse X");
            yAxis = Mathf.Clamp(yAxis, yLimit.x, yLimit.y);
            xAxis -= Input.GetAxis("Mouse Y");
            xAxis = Mathf.Clamp(xAxis, xLimit.x, xLimit.y);
            Vector3 newSpineRotation = new Vector3(xAxis, yAxis, spine.localEulerAngles.z);
            spine.localEulerAngles = newSpineRotation;
        }

        private void ShowCrosshairIfRaycastHit() {
            Vector3 weaponForwardDirection = weapon.TransformDirection(Vector3.forward);
            RaycastHit raycastHit;
            fromPosition = weapon.position + Vector3.one;
            if (Physics.Raycast(fromPosition, weaponForwardDirection, out raycastHit)) {
                Vector3 hitLocation = Camera.main.WorldToScreenPoint(raycastHit.point);
                Destination = hitLocation;
                //BulletController.Direction = (raycastHit.point - weapon.position).normalized;
                DisplayPointerImage(hitLocation);
            } else {
                Destination = Vector3.negativeInfinity;
                crosshairImage.SetActive(false);
                //BulletController.Direction = weaponForwardDirection;
            }
            /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray.origin, ray.direction, out raycastHit)) {
            crosshairImage.transform.position = Camera.main.WorldToScreenPoint(raycastHit.point);
            crosshairImage.SetActive(true);
        } else crosshairImage.SetActive(false);*/
        }
    
        private void DisplayPointerImage(Vector3 hitLocation) {
            crosshairImage.transform.position = hitLocation;
            crosshairImage.SetActive(true);
        }

        public void FireBullet(GameObject Bullet) {
            weapon.GetComponent<AudioSource>().PlayOneShot(GunShot);
            //new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + 0.5f)
        
            GameObject bulletInstance = Instantiate(Bullet, ShootPoint.position,
                player.transform.rotation);

            //bulletInstance.GetComponent<BulletController>().Destination = Destination;

            flashMuzzle(ShootPoint.position);
        
            //Vector3 dir = bullet.GetComponent<BulletController>().Direction;
            //bulletInstance.GetComponent<Rigidbody>().AddForce(500f * BulletController.Direction);
            //bulletInstance.GetComponent<BulletController>().DestroySelf(3);
        }

        public void flashMuzzle(Vector3 pos) {
            GameObject muzzleFlash = Instantiate(MuzzleFlash, pos, Quaternion.identity);
            Destroy(muzzleFlash, 0.1f);
        }
    }
}
