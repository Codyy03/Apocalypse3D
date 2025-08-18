using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    //Animator component attached to weapon
    protected Animator anim;

    // weapon damage when hit enemy
    public float damage;

    public float weaponNoiseRange;

    public GameObject bloodPrefab;

    //Holstering weapon
    protected bool hasBeenHolstered = false;
    //If weapon is holstered
    protected bool holstered;

    [System.Serializable]
    public class soundClips
    {
        public AudioClip shootSound;
        public AudioClip silencerShootSound;
        public AudioClip takeOutSound;
        public AudioClip holsterSound;
        public AudioClip reloadSoundOutOfAmmo;
        public AudioClip reloadSoundAmmoLeft;
        public AudioClip aimSound;
    }
    public soundClips SoundClips;

    [Header("Audio Source")]
    //Main audio source
    public AudioSource mainAudioSource;
    //Audio source used for shoot sound
    public AudioSource shootAudioSource;

    protected PlayerController playerController;

    [Header("Atak no¿em")]
    [SerializeField] protected GameObject attackPoint;
    protected PlayerAttackPoint playerAttackPoint;

    public virtual void Shoot(Camera mainCamera)
    {
        RaycastHit hit;

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
        {
            float currentDamage = damage;
            NoiseSystem.MakeNoise(transform.position, weaponNoiseRange);
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Transform boneHit = hit.collider.transform;
                if (boneHit.name == "Head")
                    currentDamage *= 2f;

                hit.transform.GetComponent<EnemyHealth>()?.ChangeHealth(-currentDamage);
                GameObject efect = Instantiate(bloodPrefab, hit.point, Quaternion.identity);
                efect.GetComponent<ParticleSystem>().Play();
            }
        }
    }
    public void UpdateHosler()
    {
        //Toggle weapon holster when pressing tab key
        if (Input.GetKeyDown(KeyCode.Tab) && !playerController.noWeapons)
        {
            CrosshairController.ControllCroshair(holstered);

            holstered = !holstered; // prze³¹czanie stanu

            mainAudioSource.clip = holstered ? SoundClips.holsterSound : SoundClips.takeOutSound;
            mainAudioSource.Play();

            hasBeenHolstered = holstered;
        }
    }
    /// <summary>
    /// schowaj obecnie wybran¹ broñ
    /// </summary>
    public void HideWeapon()
    {
        holstered = true;
        mainAudioSource.clip = SoundClips.holsterSound;
        mainAudioSource.Play();

        CrosshairController.ControllCroshair(false);

        anim.SetBool("Holster", true);
        hasBeenHolstered = true;
    }
    /// <summary>
    /// poka¿ obecnie wybran¹ broñ
    /// </summary>
    public void ShowWeapon()
    {
        holstered = false;
        mainAudioSource.clip = SoundClips.takeOutSound;
        mainAudioSource.Play();

        CrosshairController.ControllCroshair(true);

        anim.SetBool("Holster", false);
        hasBeenHolstered = false;


    }

    public void EnbleAttackPoint() => attackPoint.SetActive(true);
    public void DisableAttackPoint() => attackPoint.SetActive(false);

}
