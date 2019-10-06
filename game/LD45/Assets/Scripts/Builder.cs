using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [SerializeField]
    int player;

    Player _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameManager.getManager().players[player];
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Build(Buildable buildable, Vector3 position)
    {
        var inst = Instantiate(buildable);
        inst.transform.position = new Vector3(position.x, 0.0f, position.z);
        inst.player = _player;
        inst.Build();
        var hut = inst.GetComponent<Hut>();
        if (hut != null)
        {
            hut.SetPlayer(_player);
        }
        var ground = inst.GetComponent<Ground>();
        if (ground != null)
        {
            ground.SetPlayer(_player);
        }
        var statue = inst.GetComponent<Statue>();
        if (statue != null)
        {
            statue.SetPlayer(_player);
        }
    }
}
