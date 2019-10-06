using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField]
    int debugPlayer = -1;

    [SerializeField]
    ParticleSystem[] particles;

    [SerializeField]
    Renderer rend;

    [SerializeField]
    float Bounty = 5.0f;

    [SerializeField]
    ParticleSystem manaEffect;

    public Player player;

    [SerializeField]
    AudioClip[] rumbles;

    [SerializeField]
    AudioClip[] captures;

    AudioSource audioSource;

    public List<Hut> huts = new List<Hut>();
    public List<Statue> statues = new List<Statue>();
    public List<Creature> creatures = new List<Creature>();

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        transform.Rotate(Vector3.up, Random.Range(0, 360));
        if (debugPlayer >= 0)
        {
            SetPlayer(GameManager.getManager().players[debugPlayer]);
        }
        else
        {
            CameraHandler.INSTANCE.TriggerShake(transform.position);
            audioSource.clip = Util.getRandom(rumbles);
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayer(Player player)
    {
        if (this.player != null)
        {
            this.player.CurrentGround--;
        }
        this.player = player;
        this.player.CurrentGround++;
        UpdateColor();
    }

    public void UpdateColor()
    {
        rend.material.color = player.CreatureColor;
        foreach (var ps in particles)
        {
            ps.startColor = player.CreatureColor;
        }
    }

    public void AddCreature(Creature creature)
    {
        creatures.Add(creature);
        checkOwnerShip();
    }

    public void RemoveCreature(Creature creature)
    {
        creatures.Remove(creature);
        checkOwnerShip();
    }

    public void AddHut(Hut hut)
    {
        huts.Add(hut);
    }

    public void AddStatue(Statue statue)
    {
        statues.Add(statue);
    }

    void checkOwnerShip()
    {
        Player p = null;
        foreach (Creature c in creatures)
        {
            if (p != null && c.player != p)
            {
                p = null;
                break;
            }
            p = c.player;
        }

        if (p != null && p != player)
        {
            SetPlayer(p);
            p.Mana += Bounty;
            if (p.IsHuman())
            {
                manaEffect.Play();
            }
            foreach (Hut h in huts) {
                h.UpdatePlayer(player);
            }
            foreach (Statue s in statues)
            {
                s.SetPlayer(player);
            }
            foreach (var ps in particles)
            {
                ps.Play();
            }
            CameraHandler.INSTANCE.TriggerMinorShake(transform.position);
            audioSource.clip = Util.getRandom(captures);
            audioSource.Play();
        }
    }
}
