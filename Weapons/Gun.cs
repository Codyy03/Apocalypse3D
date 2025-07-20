using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    //Animator component attached to weapon
    protected  Animator anim;

    // weapon damage when hit enemy
    public float damage;

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

    public virtual void Shoot(Camera mainCamera)
    {
        RaycastHit hit;

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
        {
            float currentDamage = damage;
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Transform boneHit = hit.collider.transform;
                if (boneHit.name == "Head")
                    currentDamage *= 2f;

                hit.transform.GetComponent<EnemyHealth>()?.ChangeHealth(-currentDamage);
                Instantiate(bloodPrefab, hit.point, Quaternion.identity);
            }
        }
    }
    public void UpdateHosler()
    {
        //Toggle weapon holster when pressing tab key
        if (Input.GetKeyDown(KeyCode.Tab) && !playerController.noWeapons)
        {
            holstered = !holstered; // prze³¹czanie stanu

            mainAudioSource.clip = holstered ? SoundClips.holsterSound : SoundClips.takeOutSound;
            mainAudioSource.Play();

            hasBeenHolstered = holstered;
        }
    }
    public void HideWeapon()
    {
        holstered = true;
        mainAudioSource.clip = SoundClips.holsterSound;
        mainAudioSource.Play();

        anim.SetBool("Holster", true);
        hasBeenHolstered = true;
    }
    public void ShowWeapon()
    {
        holstered = false;
        mainAudioSource.clip = SoundClips.takeOutSound;
        mainAudioSource.Play();

        anim.SetBool("Holster", false);
        hasBeenHolstered = false;
    }
}
