using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hut : MonoBehaviour
{
    [SerializeField]
    int debugPlayer = -1;

    [SerializeField]
    GameObject spawnedObject;

    [SerializeField]
    float spawnInterval;

    [SerializeField]
    Transform spawnPosition;

    [SerializeField]
    Transform targetPosition;

    [SerializeField]
    int creatures;

    [SerializeField]
    AudioClip[] rumbles;

    AudioSource audioSource;

    int quota;

    float spawnTimer;

    Player player;

    LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        groundLayer = LayerMask.GetMask("Ground");
        spawnTimer = Time.time + spawnInterval;
        quota = creatures;
        
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
        if (spawnTimer < Time.time)
        {
            if (quota > 0)
            {
                quota--;
                SpawnCreature();
                player.IncomingCreatures--;
            }
            else if (player.MaxCreatures > player.CurrentCreatures + player.IncomingCreatures)
            {
                SpawnCreature();
            }
            spawnTimer = Time.time + spawnInterval;
        }
    }

    void SpawnCreature()
    {
        var go = Instantiate(spawnedObject);
        go.transform.position = spawnPosition.position;
        var c = go.GetComponent<Creature>();
        c.TargetPos = targetPosition.position;
        c.SetPlayer(player);
    }

    public void UpdatePlayer(Player player)
    {
        if (this.player != null)
        {
            this.player.MaxCreatures -= creatures;
            this.player.IncomingCreatures -= quota;
        }
        player.MaxCreatures += creatures;
        player.IncomingCreatures += quota;
        this.player = player;
    }

    public void SetPlayer(Player player)
    {
        player.MaxCreatures += creatures;
        player.IncomingCreatures += creatures;
        this.player = player;
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
        nearest.AddHut(this);
    }
}
