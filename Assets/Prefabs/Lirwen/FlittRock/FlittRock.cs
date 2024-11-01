using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Autumn_Mehs1.wav by Keskaowl -- https://freesound.org/s/646955/ -- License: Creative Commons 0
*/
public class FlittRock : MonoBehaviour, IDamageable
{
    [SerializeField] private float radius;
    private AudioSource audioSource;
    private Animator animator;
    float hitTime = 0f;
    float nextChirp = 0f;
    float hitPenalty = 20.0f;
    float chirpDelay = 20.0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (Time.time > hitTime + hitPenalty+1) 
        {
            CheckForPlayers();
        }

        if(Time.time > nextChirp && audioSource != null && animator.GetBool("isPlayerNearby")==false)
        {
            nextChirp = Time.time + (chirpDelay * Random.Range(0.75f,1.5f));
            audioSource.Play();
        }
    }

    private void CheckForPlayers()
    {
        if (animator == null) return;
        if (Time.time <= hitTime + hitPenalty) return;
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, transform.up);

        bool treat = false;
        bool shouldHide = false;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.name.Equals
                ("crystal"))
            {
                treat = true;
            }

            if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("NetworkPlayer"))
            {
                shouldHide = true;
            }
        }

        animator.SetBool("isPlayerNearby", shouldHide && !treat);
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckForPlayers();
    }
    private void OnTriggerExit(Collider other)
    {
        CheckForPlayers();
    }

    public void Damage(int dmg)
    {
        hitTime = Time.time;
        if (animator == null) return;
        animator.SetBool("isPlayerNearby", true);
    }
}