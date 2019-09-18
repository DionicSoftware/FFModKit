using UnityEngine;
using System.Linq;


// It's good practice to keep all of your source code in your own namespace
// Founders' Fortune doesn't use an own namespace though.
namespace SourceMod1 {

    // System.Serializable is obligatory.
    [System.Serializable]
    public class MyMod : Mod {

        // With System.Serializable in front of the class, every field is automatically saved and loaded with the game
        private int numberOfClicks = 0;

        // It's important to add System.NonSerialized to stuff you don't want to save automatically with the game
        // WE MUST NOT SERIALIZE MONOBEHAVIOURS. That's why we we add System.NonSerialized here
        [System.NonSerialized]
        private HumanAI someReferenceIDontWantToSave = null;

        // Since we can't save Monobehaviours as they are, we instead pack them into a reference object, which can be saved
        // When the game is loaded, after Load() and before StartFirstTime(), all references are automatically hooked up
        private Reference<HumanAI> someReferenceIWantToSave = null;

        // The Load() function is called when the mods are loaded into memory.
        // This happens either before entering the colonist selection screen when you click on "New Game"
        // or during the loading process when you load an old one
        public override void Load() {
            MonoBehaviour.print("loaded mod");
        }

        // Start() is called after the game world has been loaded and initialized
        // This function is called every time a world has been loaded, doesn't matter if it's a new game or loaded from a save game
        public override void Start() {
            MonoBehaviour.print("started mod");
        }

        // Update() is called every single frame.
        // Be very careful that you don't put anything too performance-heavy directly in here or you will easily slow down the player's game.
        public override void Update() {

            // Check for left mouse click
            if (Input.GetButtonDown("Fire1")) {

                Interactable clickTarget = null;

                // Do a raycast to find all colliders that have been clicked on
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray).OrderBy(h => h.distance).ToArray();

                // Find out if any of the colliders have an Interactable attached somewhere
                foreach (RaycastHit hit in hits) {
                    GameObject go = hit.collider.gameObject;
                    InteractableHolder ih = Helper.GetComponentInHierarchy<InteractableHolder>(ref go);
                    if (ih != null) {
                        clickTarget = ih.GetInteractable();
                        break;
                    }
                }

                // Compose a message depending on what was clicked on
                string hitMessage = "You clicked on nothing.";
                if (clickTarget is HumanInteractable) {
                    // GetHuman() returns a HumanAI object, which has all kinds of uses
                    hitMessage = "You clicked on " + (clickTarget as HumanInteractable).GetHuman().GetFullName() + ".";
                } else if (clickTarget is FurnitureInteractable) {
                    // GetFurniture() returns a Furniture object, which has all kinds of uses
                    hitMessage = "You clicked on " + (clickTarget as FurnitureInteractable).GetFurniture().GetDescription().GetDisplayName() + ".";
                } else if (clickTarget is WallInteractable) {
                    hitMessage = "You clicked on a wall.";
                } else if (clickTarget is FloorInteractable) {
                    hitMessage = "You clicked on a floor tile.";
                } else {
                    hitMessage = "I don't know what you clicked on.";
                }

                // WorldScripts.Instance also contains a lot of useful things
                string playtime = WorldScripts.Instance.timeManager.GetTotalPlayTimeFormatted();

                // CanvasHandler.Instance gives you access to some UI stuff
                numberOfClicks++;
                CanvasHandler.Instance.smallMessagePanel.ShowMessage(hitMessage + "\nClick number " + numberOfClicks + ". Playtime: " + playtime, 2);
            }
        }
    }
}
