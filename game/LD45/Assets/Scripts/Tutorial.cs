using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField]
    UIHandler ui;

    [SerializeField]
    PlayerController pc;

    bool groundPlaced = false;
    bool holePlaced = false;

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.getManager().players[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (player.CurrentGround > 0 && !groundPlaced)
        {
            pc.SetBluePrint(null);
            ui.ShowInfo("Marvellous! Now you can build structures on top of this block of Land. Note how it is colored by our Divine Color! That means the territory is ours.");
            ui.levelinfos = new string[2];
            ui.infoidx = 1;
            ui.levelinfos[0] = "Marvellous! Now you can build structures on top of this block of Land. Note how it is colored by our Divine Color! That means the territory is ours.";
            ui.levelinfos[1] = "Next, build a Hole-in-Ground on the Land you just raised. It will draw those fascinating creatures from the bottom of the sea to fight for you.";
            groundPlaced = true;
        }
        if (player.CurrentCreatures > 0 && !holePlaced)
        {
            pc.SetBluePrint(null);
            ui.ShowInfo("Well done! The creatures will keep on pouring out of the hole until you have reached the maximum number of creatures you can house.");
            ui.levelinfos = new string[6];
            ui.infoidx = 1;
            ui.levelinfos[0] = "Well done! The creatures will keep on pouring out of the hole until you have reached the maximum number of creatures you can upkeep.";
            ui.levelinfos[1] = "You can build more Holes-in-Ground to increase your creature limit and draw even more creatures!";
            ui.levelinfos[2] = "Move your mouse to the edge of the screen or use W, S, A and D to pan the viewport.";
            ui.levelinfos[3] = "Use the mouse wheel or Q and E to zoom in and out.";
            ui.levelinfos[4] = "Right click on any piece of land to command all your creatures to move there - and attack any enemy they encounter on the way.";
            ui.levelinfos[5] = "Let's see how this works in practice. Order your minions to attack the red creatures!";
            holePlaced = true;
        }

    }
}
