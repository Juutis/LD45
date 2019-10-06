using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAnimListener : MonoBehaviour
{
    [SerializeField]
    AudioClip[] footsteps;

    [SerializeField]
    ParticleSystem footstep;

    AudioSource audioSource;

    float lastPlayed = 0;

    // Start is called before the first frame update
    void Start()
    {

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BounceFinished()
    {
        if (lastPlayed + 0.1f < Time.time)
        {
            if (Random.Range(0f, 100f) < 10)
            {
                audioSource.clip = Util.getRandom(footsteps);
                audioSource.Play();
            }
            footstep.Play();
            lastPlayed = Time.time;
        }
    }

    public void BounceStart()
    {
        if (lastPlayed + 0.1f < Time.time)
        {
            if (Random.Range(0f, 100f) < 10)
            {
                audioSource.clip = Util.getRandom(footsteps);
                audioSource.Play();
            }
            footstep.Play();
            lastPlayed = Time.time;
        }
    }
}
