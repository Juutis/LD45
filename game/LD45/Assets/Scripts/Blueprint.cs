using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blueprint : MonoBehaviour
{
    Renderer[] rend;

    [SerializeField]
    Color legalColor;

    [SerializeField]
    Color illegalColor;

    [SerializeField]
    LayerMask layerToBuildOn;

    [SerializeField]
    LayerMask blockingLayers;

    [SerializeField]
    bool needToOwn;

    [SerializeField]
    bool needContinuous;

    [SerializeField]
    float contCheckRadius;

    [SerializeField]
    float radius;

    [SerializeField]
    public Buildable buildable;

    bool wasValid = false;

    LayerMask groundLayer;

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponentsInChildren<Renderer>();
        wasValid = false;
        groundLayer = LayerMask.GetMask("Ground");
        player = GameManager.getManager().players[0];
    }

    // Update is called once per frame
    void Update()
    {
        var valid = checkValid();

        if (valid && !wasValid)
        {
            foreach (var r in rend)
            {
                r.material.color = legalColor;
            }
        }

        if (!valid && wasValid)
        {
            foreach (var r in rend)
            {
                r.material.color = illegalColor;
            }
        }

        wasValid = valid;
    }

    public bool isValid()
    {
        return wasValid;
    }

    private bool checkValid()
    {
        RaycastHit hit;

        if (buildable.Cost > player.Mana)
        {
            return false;
        }

        if (needContinuous && player.CurrentGround > 0)
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position + new Vector3(0, 10, 0), contCheckRadius, Vector3.down, Mathf.Infinity, blockingLayers);
            var ok = false;
            foreach (var h in hits)
            {
                var ground = h.collider.GetComponent<Ground>();
                if (ground != null && ground.player == player)
                {
                    ok = true;
                    break;
                }
            }
            if (!ok)
            {
                return false;
            }
        }

        if (needToOwn)
        {
            Ground ground = getGround();
            if (ground == null || ground.player != player)
            {
                return false;
            }
        }

        return Physics.Raycast(transform.position + new Vector3(0, 10, 0), Vector3.down, out hit, Mathf.Infinity, layerToBuildOn)
            && Physics.Raycast(transform.position + new Vector3(-radius, 10, 0), Vector3.down, out hit, Mathf.Infinity, layerToBuildOn)
            && Physics.Raycast(transform.position + new Vector3(-radius, 10, 0), Vector3.down, out hit, Mathf.Infinity, layerToBuildOn)
            && Physics.Raycast(transform.position + new Vector3(0, 10, radius), Vector3.down, out hit, Mathf.Infinity, layerToBuildOn)
            && Physics.Raycast(transform.position + new Vector3(0, 10, -radius), Vector3.down, out hit, Mathf.Infinity, layerToBuildOn)
            && Physics.Raycast(transform.position + new Vector3(radius * 0.7f, 10, radius * 0.7f), Vector3.down, out hit, Mathf.Infinity, layerToBuildOn)
            && Physics.Raycast(transform.position + new Vector3(-radius * 0.7f, 10, radius * 0.7f), Vector3.down, out hit, Mathf.Infinity, layerToBuildOn)
            && Physics.Raycast(transform.position + new Vector3(radius * 0.7f, 10, -radius * 0.7f), Vector3.down, out hit, Mathf.Infinity, layerToBuildOn)
            && Physics.Raycast(transform.position + new Vector3(-radius * 0.7f, 10, -radius * 0.7f), Vector3.down, out hit, Mathf.Infinity, layerToBuildOn)

            && !Physics.SphereCast(transform.position + new Vector3(0, 10, 0), radius, Vector3.down, out hit, Mathf.Infinity, blockingLayers);
    }

    private Ground getGround()
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
        return nearest;
    }
}
