using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource playerSource;
    [SerializeField]
    private GameObject oneShotAudioSource;
    public AudioClip shootingSound, projectileHitSound, enemyEliminatedSound, playerEliminatedSound, shootingCooldownSound;

    //Singleton
    public static AudioManager instance;

    private void Awake()
    {
        if (AudioManager.instance == null)
        {
            AudioManager.instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void PlayOneShot(AudioClip audioClip)
    {
        playerSource.PlayOneShot(audioClip);
    }

    public void PlaySound(AudioClip audioClip)
    {
        playerSource.clip = audioClip;
        playerSource.Play();
    }

    public void StopSound(AudioClip audioClip)
    {
        if (playerSource.clip = audioClip)
        {
            playerSource.Stop();
        }
    }

    public void PlaySoundSpawner(AudioClip audioClip, Vector3 hitPosition)
    {
        GameObject audioSourceSpawn = Instantiate(oneShotAudioSource, hitPosition, Quaternion.identity);
        AudioSource spawnedAudioSource = audioSourceSpawn.GetComponent<AudioSource>();

        spawnedAudioSource.clip = audioClip;
        spawnedAudioSource.PlayOneShot(audioClip);

        Destroy(audioSourceSpawn, audioClip.length);
    }

    private void PlaySoundSpawner2(AudioClip audioClip, Vector3 hitPosition)
    {
        AudioSource oneShotAudioSource = new GameObject("One-shot AudioSource").AddComponent<AudioSource>();

        oneShotAudioSource.transform.position = hitPosition;
        oneShotAudioSource.clip = audioClip;
        oneShotAudioSource.spatialBlend = 1.0f;
        oneShotAudioSource.PlayOneShot(audioClip);

        Destroy(oneShotAudioSource.gameObject, audioClip.length);
    }
}
