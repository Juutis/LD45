using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField]
    int debugPlayer = -1;

    [SerializeField]
    Renderer rend;

    [SerializeField]
    Animator anim;

    [SerializeField]
    GameObject model;

    [SerializeField]
    float riseSpeed = 1.0f;

    [SerializeField]
    float moveSpeed = 0.5f;

    [SerializeField]
    float aggroRange = 2.5f;

    [SerializeField]
    float BaseHealth = 10f;

    [SerializeField]
    float MaxHealth = 30f;

    [SerializeField]
    float BaseDamage = 4f;

    [SerializeField]
    float MaxDamage = 10f;

    [SerializeField]
    float DamageIntervalMin = 1.5f;

    [SerializeField]
    float DamageIntervalMax = 2.0f;

    [SerializeField]
    ParticleSystem fightEffect;

    [SerializeField]
    ParticleSystem dieEffect;

    [SerializeField]
    ParticleSystem upgradeEffect;

    [SerializeField]
    ParticleSystem manaEffect;

    [SerializeField]
    float ManaInterval = 10f;

    [SerializeField]
    float ManaGenerated = 1f;

    [SerializeField]
    float Bounty = 1f;

    [SerializeField]
    AudioClip[] smacks;

    [SerializeField]
    AudioClip[] ows;

    [SerializeField]
    AudioClip[] splats;

    [SerializeField]
    AudioClip[] upgrades;

    AudioSource audioSource;

    bool spawning = true;

    public Vector3 TargetPos;

    public Player player;

    Rigidbody rb;
    Collider coll;

    float idleTimer;
    float spawnTimer;

    LayerMask groundLayer;
    LayerMask buildingLayer;

    Vector3 prevTarget;
    Creature targetCreature;
    Creature prevTargetCreature;
    Ground currentGround;

    float damage, health, maxHealth, damageTimer, manaTimer;

    bool wasInRange;
    public bool dying;

    Vector3 dummyTarget = new Vector3(10000f, 10000f, 10000f);

    public void SetPlayer(Player player)
    {
        this.player = player;
        health = BaseHealth;
        damage = BaseDamage;
        maxHealth = BaseHealth;
        UpdateColor();
        player.AddCreature(this);
    }

    public void UpdateColor()
    {
        var fullHpColor = Color.Lerp(player.CreatureColor, player.CreatureMaxColor, (maxHealth - BaseHealth) / (MaxHealth - BaseHealth));
        rend.material.color = Color.Lerp(player.CreatureMinColor, fullHpColor, health / maxHealth);
    }

    public void Die(Player killer)
    {
        if (!dying) {
            if (currentGround != null)
            {
                currentGround.RemoveCreature(this);
            }
            player.RemoveCreature(this);
            dieEffect.Play();
            dying = true;
            model.SetActive(false);
            Destroy(gameObject, 1.0f);

            if (killer != null)
            {
                killer.Mana += Bounty;
                if (killer.IsHuman())
                {
                    manaEffect.Play();
                }
            }
            audioSource.clip = Util.getRandom(splats);
            audioSource.Play();
            anim.SetBool("Moving", false);
        }
    }

    public void Hurt(float damage, Player fromPlayer)
    {
        if (!dying)
        {
            health -= damage;
            if (health <= 0)
            {
                Die(fromPlayer);
            }
            UpdateColor();
            if (Random.Range(0.0f, 100.0f) < 50)
            {
                audioSource.clip = Util.getRandom(ows);
                audioSource.Play();
            }
        }
    }

    public void Attack()
    {
        fightEffect.Play();
        targetCreature.Hurt(damage, player);
        audioSource.clip = Util.getRandom(smacks);
        audioSource.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        coll.enabled = false;
        idleTimer = Time.time + 1.0f;
        spawnTimer = Time.time + 1.5f;
        groundLayer = LayerMask.GetMask("Ground");
        buildingLayer = LayerMask.GetMask("Building");

        health = BaseHealth;
        damage = BaseDamage;
        maxHealth = BaseHealth;

        if (debugPlayer >= 0)
        {
            SetPlayer(GameManager.getManager().players[debugPlayer]);
        }

        manaTimer = Time.time + ManaInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (falling && transform.position.y < -0.5)
        {
            Die(null);
        }

        if (targetCreature != null && targetCreature.dying)
        {
            targetCreature = null;
        }

        if (!dying)
        {
            searchForTarget();
            checkGround();

            var lookdir = rb.velocity;
            lookdir.y = 0;

            if (spawning)
            {
                if (spawnTimer < Time.time)
                {
                    spawning = false;
                    coll.enabled = true;
                }
            }
            else if (targetCreature != null)
            {
                lookdir = (targetCreature.transform.position - transform.position);
                lookdir.y = 0;

                if (lookdir.magnitude < 0.3f)
                {
                    if (!wasInRange)
                    {
                        damageTimer = Time.time + Random.Range(DamageIntervalMin, DamageIntervalMax) / 2;
                    }
                    if (damageTimer < Time.time)
                    {
                        Attack();
                        damageTimer = Time.time + Random.Range(DamageIntervalMin, DamageIntervalMax);
                    }
                    TargetPos = transform.position;
                    wasInRange = true;
                }
                else
                {
                    TargetPos = targetCreature.transform.position;
                    wasInRange = false;
                }
                lookdir.Normalize();
            }
            else if (player.target == null)
            {
                if (idleTimer < Time.time)
                {
                    TargetPos = transform.position + Random.Range(-2.0f, 2.0f) * Vector3.forward + Random.Range(-2.0f, 2.0f) * Vector3.left;
                    idleTimer = Time.time + 10.0f;
                }
                prevTarget = dummyTarget;
            }
            else
            {
                if (player.target != null && prevTargetCreature != null && targetCreature == null || Vector3.Distance(prevTarget, player.target.transform.position) > 0.1f)
                {
                    TargetPos = player.target.transform.position + Random.Range(-1f, 1f) * Vector3.forward + Random.Range(-1f, 1f) * Vector3.left;
                }
                prevTarget = player.target.transform.position;
            }

            if (transform.position.y < 0)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + riseSpeed * Time.deltaTime, transform.position.z);
            }
            if (transform.position.y > 0)
            {
                transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
            }

            if (lookdir.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(lookdir);
            }

            prevTargetCreature = targetCreature;

            if (manaTimer < Time.time)
            {
                player.Mana += ManaGenerated;
                manaTimer = Time.time + ManaInterval;
            }

            if (rb.velocity.magnitude > 0.1f)
            {
                anim.SetBool("Moving", true);
            }
            else
            {
                anim.SetBool("Moving", false);
            }
        }
    }

    void FixedUpdate()
    {
        if (!falling)
        {
            var targetDiff = TargetPos - transform.position;
            targetDiff.y = 0;
            if (targetDiff.magnitude > 0.01f)
            {
                rb.velocity = targetDiff.normalized * moveSpeed;
            }
            else
            {
                rb.velocity = Vector3.zero;
                spawning = false;
                coll.enabled = true;
            }

            var moveDir = rb.velocity;
            moveDir.y = 0;
            moveDir.Normalize();
            RaycastHit hit;
            if (!Physics.Raycast(transform.position + moveDir * 0.5f + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                rb.velocity = Vector3.zero;
            }
            /*
            if (!spawning && Physics.Raycast(transform.position + moveDir * 0.5f + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity, buildingLayer))
            {
                rb.velocity = Vector3.zero;
            }
            */

        }
        else
        {
            rb.velocity = Vector3.down * 3;
        }
    }

    float aiTimer = 0;
    float groundTimer = 0;

    private void searchForTarget()
    {
        if (aiTimer < Time.time)
        {
            if (targetCreature == null)
            {
                foreach (Player otherPlayer in GameManager.getManager().players)
                {
                    if (otherPlayer == player)
                    {
                        continue;
                    }

                    Creature nearest = null;
                    float nearestDist = 10000f;
                    foreach (Creature creature in otherPlayer.creatures)
                    {
                        if (creature.dying)
                        {
                            continue;
                        }
                        var dist = Vector3.Distance(transform.position, creature.transform.position);
                        if (nearest == null || dist < nearestDist)
                        {
                            nearestDist = dist;
                            nearest = creature;
                        }
                    }
                    if (nearestDist < aggroRange)
                    {
                        targetCreature = nearest;
                    }
                }
            }

            aiTimer = Time.time + 1.0f;
        }
    }

    bool falling = false;

    private void Fall()
    {
        if (!falling)
        {
            falling = true;
            audioSource.clip = Util.getRandom(ows);
            audioSource.Play();
            rb.useGravity = true;
            coll.enabled = false;
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    private void checkGround()
    {
        if (groundTimer < Time.time)
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up * 10f, Vector3.down, Mathf.Infinity, groundLayer);
            if (hits.Length == 0)
            {
                Fall();
            }
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
            if (nearest != currentGround)
            {
                SetGround(nearest);
            }
            groundTimer = Time.time + 0.2f;
        }
    }

    public void SetGround(Ground ground)
    {
        if (currentGround != null)
        {
            currentGround.RemoveCreature(this);
        }
        currentGround = ground;
        if (currentGround != null)
        {
            currentGround.AddCreature(this);
        }
    }

    public void UpgradeHealth(float plusHealth)
    {
        float prevMax = maxHealth;
        maxHealth += plusHealth;
        if (maxHealth > MaxHealth)
        {
            maxHealth = MaxHealth;
        }
        health += maxHealth - prevMax;
        UpdateColor();
        upgradeEffect.Play();
        //audioSource.clip = Util.getRandom(upgrades);
        //audioSource.Play();
    }

    public void UpgradeDamage(float plusDamage)
    {
        damage += plusDamage;
        if (damage > MaxDamage)
        {
            damage = MaxDamage;
        }
        upgradeEffect.Play();
        //audioSource.clip = Util.getRandom(upgrades);
        //audioSource.Play();
    }

    public bool CanUpgrade()
    {
        return damage < MaxDamage || maxHealth < MaxHealth;
    }
}
