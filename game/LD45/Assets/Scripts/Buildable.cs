using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildable : MonoBehaviour
{
    [SerializeField]
    float riseSpeed = 1.0f;

    [SerializeField]
    float riserY = 0.0f;

    [SerializeField]
    ParticleSystem BuildEffect;

    [SerializeField]
    public int Cost = 0;

    [SerializeField]
    public GameObject riser;

    bool spawning = true;
    Collider coll;

    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        if (player != null)
        {
            player.Mana -= Cost;
        }
        coll = GetComponent<Collider>();
        coll.enabled = true;
    }

    public void Build()
    {
        riser.transform.position = new Vector3(riser.transform.position.x, riserY, riser.transform.position.z);
        BuildEffect.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawning)
        {
            if (riser.transform.position.y < 0)
            {
                riser.transform.position = new Vector3(riser.transform.position.x, riser.transform.position.y + riseSpeed * Time.deltaTime, riser.transform.position.z);
            }
            if (riser.transform.position.y > 0)
            {
                riser.transform.position = new Vector3(riser.transform.position.x, 0.0f, riser.transform.position.z);
                spawning = false;
            }
        }
    }
}
