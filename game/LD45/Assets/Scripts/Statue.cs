using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour
{
    [SerializeField]
    int debugPlayer = -1;

    [SerializeField]
    float interval = 1.0f;

    [SerializeField]
    float healthBoost = 1.0f;

    [SerializeField]
    float damageBoost = 1.0f;

    [SerializeField]
    float range = 2.0f;

    [SerializeField]
    ParticleSystem particles;
    
    [SerializeField]
    AudioClip[] rumbles;

    AudioSource audioSource;

    public Player player;

    float timer;
    LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");
        audioSource = GetComponent<AudioSource>();
        timer = Time.time + interval;
        if (debugPlayer >= 0)
        {
            SetPlayer(GameManager.getManager().players[debugPlayer]);
        }
        else
        {
            audioSource.clip = Util.getRandom(rumbles);
            audioSource.Play();
        }
        checkGround();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < Time.time)
        {
            Creature nearest = findNearestUpgradeable();
            if (nearest != null)
            {
                nearest.UpgradeHealth(healthBoost);
                nearest.UpgradeDamage(damageBoost);
                particles.Play();
            }
            timer = Time.time + interval;
        }
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    Creature findNearestUpgradeable()
    {
        Creature nearest = null;
        float nearestDist = 100000;
        foreach (Creature c in player.creatures)
        {
            if (!c.CanUpgrade())
            {
                continue;
            }
            float dist = Vector3.Distance(transform.position, c.transform.position);
            if (nearest == null || dist < nearestDist)
            {
                nearest = c;
                nearestDist = dist;
            }
        }
        if (nearest != null && nearestDist < range)
        {
            return nearest;
        }
        return null;
    }

    private void checkGround()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up * 10f, Vector3.down, Mathf.Infinity, groundLayer);

        Ground nearest = null;
        float nearestDist = 10000000f;
        foreach (RaycastHit hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (nearest == null || dist < nearestDist)
            {
                nearest = hit.collider.GetComponent<Ground>();
                nearestDist = dist;
            }
        }
        nearest.AddStatue(this);
    }
}
