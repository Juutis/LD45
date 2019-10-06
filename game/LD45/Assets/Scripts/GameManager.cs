using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public Color[] CreatureMinColors;

    [SerializeField]
    public Color[] CreatureColors;

    [SerializeField]
    public Color[] CreatureMaxColors;

    [SerializeField]
    public float[] playerMana;

    [SerializeField]
    public UIHandler uiHandler;

    [SerializeField]
    public GameObject[] targets;

    public Player[] players;

    public bool won = false;

    static GameManager instance;

    float winTimer = -1;

    public static GameManager getManager()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
        players = new Player[CreatureColors.Length];
        for (int i = 0; i < CreatureColors.Length; i++)
        {
            Player player = new Player(i, CreatureMinColors[i], CreatureColors[i], CreatureMaxColors[i], playerMana[i], targets[i]);
            players[i] = player;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        bool enemiesAlive = false;
        foreach (Player p in players)
        {
            if (!p.IsHuman() && (p.CurrentGround > 0 || p.CurrentCreatures > 0))
            {
                enemiesAlive = true;
                break;
            }
        }
        if (!enemiesAlive)
        {
            Win();
        }
        if (winTimer > 0 && winTimer < Time.time)
        {
            uiHandler.Victory();
        }
    }

    public void Win()
    {
        if (!won)
        {
            won = true;
            winTimer = Time.time + 2.0f;
        }
    }

    public void Lose()
    {
        Debug.Log("LOSE!");
    }

}

public class Player
{
    public int MaxCreatures = 0;
    public int CurrentCreatures = 0;
    public int CurrentGround = 0;
    public int IncomingCreatures = 0;
    public float Mana = 40;
    public Color CreatureColor;
    public Color CreatureMinColor;
    public Color CreatureMaxColor;
    public GameObject target;
    public int index;

    public List<Creature> creatures = new List<Creature>();

    public Player(int index, Color creatureMinColor, Color creatureColor, Color creatureMaxColor, float mana, GameObject target)
    {
        this.index = index;
        creatures = new List<Creature>();
        CreatureMinColor = creatureMinColor;
        CreatureColor = creatureColor;
        CreatureMaxColor = creatureMaxColor;
        Mana = mana;
        this.target = target;
    }

    public void AddCreature(Creature creature)
    {
        CurrentCreatures++;
        creatures.Add(creature);
    }

    public void RemoveCreature(Creature creature)
    {
        CurrentCreatures--;
        creatures.Remove(creature);
    }

    public bool IsHuman()
    {
        return index == 0;
    }

}
