using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Blueprint groundBp;

    [SerializeField]
    Blueprint hutBp;

    [SerializeField]
    Blueprint barracksBp;

    [SerializeField]
    GameObject targetMarker;

    [SerializeField]
    Text currentCreatures;

    [SerializeField]
    Text maxCreatures;

    [SerializeField]
    Text mana;

    GameObject target;

    Builder builder;
    Camera mainCamera;

    LayerMask waterMask;
    LayerMask groundMask;

    Blueprint selectedBlueprint;

    Player player;
    AudioSource audioSource;

    int prevCreatures = 0, prevMaxCreatures = 0;
    float prevMana = 0;

    [SerializeField]
    AudioClip[] plops;

    [SerializeField]
    AudioClip[] clicks;

    [SerializeField]
    AudioClip[] merps;


    // Start is called before the first frame update
    void Start()
    {
        builder = GetComponent<Builder>();
        mainCamera = Camera.main;
        waterMask = LayerMask.GetMask("Water");
        groundMask = LayerMask.GetMask("Ground");
        selectedBlueprint = null;
        player = GameManager.getManager().players[0];
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = getMousePos();
        
        if (Input.GetButtonDown("Click") && Input.mousePosition.x < Screen.width * 0.76)
        {
            if (selectedBlueprint != null)
            {
                if (selectedBlueprint.isValid())
                {
                    builder.Build(selectedBlueprint.buildable, mousePos);
                    audioSource.clip = Util.getRandom(plops);
                    audioSource.Play();
                }
                else
                {
                    audioSource.clip = Util.getRandom(merps);
                    audioSource.Play();
                }
            }
        }

        if (Input.GetButtonDown("ClickRight"))
        {
            if (selectedBlueprint != null)
            {
                SetBluePrint(null);
            }
            else
            {
                RaycastHit[] hits = Physics.RaycastAll(mousePos + Vector3.up * 10, Vector3.down, Mathf.Infinity, groundMask);
                GameObject ground = null;
                foreach (RaycastHit hit in hits)
                {
                    if (ground == null || Vector3.Distance(mousePos, hit.transform.position) < Vector3.Distance(mousePos, ground.transform.position))
                    {
                        ground = hit.collider.gameObject;
                    }
                }

                if (ground != null)
                {
                    target = targetMarker;
                    targetMarker.transform.position = mousePos;
                    targetMarker.SetActive(true);
                    player.target = target;
                    audioSource.clip = Util.getRandom(plops);
                    audioSource.Play();
                }
                else
                {
                    target = null;
                    targetMarker.SetActive(false);
                    player.target = null;
                    audioSource.clip = Util.getRandom(clicks);
                    audioSource.Play();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetBluePrint(groundBp);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetBluePrint(hutBp);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetBluePrint(barracksBp);
        }

        if (selectedBlueprint != null)
        {
            selectedBlueprint.transform.position = mousePos + Vector3.up * 0.1f;
        }

        if (prevCreatures != player.CurrentCreatures)
        {
            currentCreatures.text = player.CurrentCreatures.ToString();
        }
        if (prevMaxCreatures != player.MaxCreatures)
        {
            maxCreatures.text = player.MaxCreatures.ToString();
        }
        if (prevMana != player.Mana)
        {
            mana.text = ((int)(player.Mana)).ToString();
        }

        prevCreatures = player.CurrentCreatures;
        prevMaxCreatures = player.MaxCreatures;
        prevMana = player.Mana;
    }

    public void SetBluePrint(Blueprint blueprint)
    {
        if (selectedBlueprint != null)
        {
            selectedBlueprint.gameObject.SetActive(false);
        }

        selectedBlueprint = blueprint;

        if (selectedBlueprint != null)
        {
            selectedBlueprint.gameObject.SetActive(true);
            audioSource.clip = Util.getRandom(plops);
            audioSource.Play();
        } else
        {
            audioSource.clip = Util.getRandom(clicks);
            audioSource.Play();
        }
    }

    Vector3 getMousePos()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, waterMask))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    public void SelectGround()
    {
        SetBluePrint(groundBp);
    }

    public void SelectHole()
    {
        SetBluePrint(hutBp);
    }

    public void SelectStatue()
    {
        SetBluePrint(barracksBp);
    }
}
