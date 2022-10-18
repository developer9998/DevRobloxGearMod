using BepInEx;
using System;
using System.Net;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using UnityEngine;
using Utilla;
using GorillaNetworking;

namespace DevRobloxGearMod
{
    [Description("HauntedModMenu")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [ModdedGamemode("devmonkblox", "MONKBLOX", Utilla.Models.BaseGamemode.Casual)]
    public class Plugin : BaseUnityPlugin
    {

        /*Mod under the MIT license, if you reproduce please credit*/

        /*Assetloading*/
        public static readonly string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static GameObject Burger; // the burger asset
        static GameObject BloxyCola; // the cola asset
        static GameObject Pizza; // the pizza asset
        static GameObject SpeedCoil; // the speedcoil asset
        static GameObject GarbageBin; // the bin asset
        static GameObject Giver1; // a giver asset
        static GameObject Giver2; // a giver asset
        static GameObject Giver3; // a giver asset
        static GameObject Giver4; // a giver asset
        static GameObject GiverFolder; // the giver folder
        static GameObject Head; // the player's head

        /*Mod Logic*/
        static bool inAllowedRoom = false; // in a modded lobby
        static float distance; // the distance from the burger to the player's head
        static bool canEat = false; // can the player eat the burger
        static int mode = 0; // 0 for no item 1 for burger 2 for bloxy cola
        //static bool isOutdated = false; // is the mod outdated
        static WebClient currentVersionWebClient; // the thing that finds the recent version
        static string recentVersion; // the recent version of the mod
        public static bool?[] machinesActive = new bool?[] { true, true, false, false };

        /*SpeedCoil Logic*/
        static float? maxJumpSpeed;
        static float JumpMultiplier;

        /*Materials*/
        public static Material activedButton = Resources.Load<Material>("objects/treeroom/materials/pressed");
        public static Material inactiveButton = Resources.Load<Material>("objects/treeroom/materials/plastic");

        void Awake()
        {
            Utilla.Events.RoomJoined += RoomJoined;
            Utilla.Events.RoomLeft += RoomLeft;
        }

        void OnEnable() // when the mod is enabled, it'll run OnGameInitialized
        {
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnGameInitialized(object sender, EventArgs e) // when the game is initialized
        {
            if (maxJumpSpeed == null)
            {
                maxJumpSpeed = GorillaLocomotion.Player.Instance.maxJumpSpeed;
                JumpMultiplier = GorillaLocomotion.Player.Instance.jumpMultiplier;
            }

            GiverFolder = new GameObject();
            GiverFolder.transform.SetParent(null, false);
            GiverFolder.name = "DevRobloxGearModGiverFolder"; // long name lol
            GiverFolder.gameObject.SetActive(false);

            Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevRobloxGearMod.Resources.Items.burger");
            AssetBundle bundle = AssetBundle.LoadFromStream(str);
            GameObject cheezeburgerGameObject = bundle.LoadAsset<GameObject>("Cheezburger");
            Burger = Instantiate(cheezeburgerGameObject);

            GameObject Hand = GorillaTagger.Instance.offlineVRRig.rightHandTransform.parent.Find("palm.01.R").gameObject;
            Head = GameObject.Find("OfflineVRRig/Actual Gorilla/rig/body/head");
            Burger.transform.SetParent(Hand.transform, false);
            Burger.transform.localPosition = new Vector3(-0.078f, 0.01130111f, 0.004799806f);
            Burger.transform.localRotation = Quaternion.Euler(5.756f, 177.007f, 347.015f);
            Burger.transform.localScale = new Vector3(0.1066138f, 0.1066138f, 0.1066138f);
            Burger.SetActive(false);

            str = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevRobloxGearMod.Resources.Items.bloxy cola");
            bundle = AssetBundle.LoadFromStream(str);
            GameObject bcolaGameObject = bundle.LoadAsset<GameObject>("BloxyCola");
            BloxyCola = Instantiate(bcolaGameObject);

            BloxyCola.transform.SetParent(Hand.transform, false);
            BloxyCola.transform.localPosition = new Vector3(-0.078f, 0.01130111f, 0.004799806f);
            BloxyCola.transform.localRotation = Quaternion.Euler(5.756f, 177.007f, 347.015f);
            BloxyCola.transform.localScale = new Vector3(0.1066138f, 0.1066138f, 0.1066138f);
            BloxyCola.SetActive(false);

            str = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevRobloxGearMod.Resources.Items.pizza");
            bundle = AssetBundle.LoadFromStream(str);
            GameObject pizzaGameObject = bundle.LoadAsset<GameObject>("Pizza");
            Pizza = Instantiate(pizzaGameObject);

            Pizza.transform.SetParent(Hand.transform, false);
            Pizza.transform.localPosition = new Vector3(-0.078f, 0.01130111f, 0.004799806f);
            Pizza.transform.localRotation = Quaternion.Euler(5.756f, 177.007f, 347.015f);
            Pizza.transform.localScale = new Vector3(0.1066138f, 0.1066138f, 0.1066138f);
            Pizza.SetActive(false);

            str = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevRobloxGearMod.Resources.Items.speed coil");
            bundle = AssetBundle.LoadFromStream(str);
            GameObject speedCoilGameObject = bundle.LoadAsset<GameObject>("SpeedCoil");
            SpeedCoil = Instantiate(speedCoilGameObject);

            SpeedCoil.transform.SetParent(Hand.transform, false);
            SpeedCoil.transform.localPosition = new Vector3(-0.078f, 0.01130111f, 0.004799806f);
            SpeedCoil.transform.localRotation = Quaternion.Euler(5.756f, 177.007f, 347.015f);
            SpeedCoil.transform.localScale = new Vector3(0.1066138f, 0.1066138f, 0.1066138f);
            SpeedCoil.SetActive(false);

            str = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevRobloxGearMod.Resources.giver");
            bundle = AssetBundle.LoadFromStream(str);
            GameObject giverGameObject = bundle.LoadAsset<GameObject>("GiverModel");
            Giver1 = Instantiate(giverGameObject);
            Giver2 = Instantiate(giverGameObject);
            Giver3 = Instantiate(giverGameObject);
            Giver4 = Instantiate(giverGameObject);

            GiverFolder.transform.position = new Vector3(0f, 0f, 0f);
            GiverFolder.transform.localScale = new Vector3(1f, 1f, 1f);
            GiverFolder.transform.eulerAngles = new Vector3(0f, 0f, 0f); // fuck quaternions

            Giver1.name = "BurgerGiver";
            Giver1.transform.localScale = new Vector3(0.0521f, 0.0521f, 0.0521f);
            Giver1.transform.localPosition = new Vector3(-67.796f, 11.664f, -80.48f);
            Giver1.transform.localRotation = Quaternion.Euler(0f, 61.875f, 0f);
            Giver1.transform.SetParent(GiverFolder.transform, true);
            Giver1.SetActive(true);

            Giver2.name = "ColaGiver";
            Giver2.transform.localScale = new Vector3(0.0521f, 0.0521f, 0.0521f);
            Giver2.transform.localPosition = new Vector3(-67.509f, 11.664f, -80.326f);
            Giver2.transform.localRotation = Quaternion.Euler(0f, 61.875f, 0f);
            Giver2.transform.SetParent(GiverFolder.transform, true);
            Giver2.SetActive(true);

            Giver3.name = "PizzaGiver";
            Giver3.transform.localScale = new Vector3(0.0521f, 0.0521f, 0.0521f);
            Giver3.transform.localPosition = new Vector3(-67.796f, 11.664f, -80.48f);
            Giver3.transform.localRotation = Quaternion.Euler(0f, 61.875f, 0f);
            Giver3.transform.SetParent(GiverFolder.transform, true);
            Giver3.SetActive(true);

            Giver4.name = "SpeedcoilGiver";
            Giver4.transform.localScale = new Vector3(0.0521f, 0.0521f, 0.0521f);
            Giver4.transform.localPosition = new Vector3(-67.509f, 11.664f, -80.326f);
            Giver4.transform.localRotation = Quaternion.Euler(0f, 61.875f, 0f);
            Giver4.transform.SetParent(GiverFolder.transform, true);
            Giver4.SetActive(true);

            Giver1.transform.GetChild(0).gameObject.AddComponent<Give>();
            Giver2.transform.GetChild(0).gameObject.AddComponent<Give>();
            Giver3.transform.GetChild(0).gameObject.AddComponent<Give>();
            Giver4.transform.GetChild(0).gameObject.AddComponent<Give>();

            Giver1.transform.GetChild(1).GetChild(0).gameObject.AddComponent<Spin>();
            Giver1.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            Giver1.transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
            Giver1.transform.GetChild(1).GetChild(3).gameObject.SetActive(false);

            Giver2.transform.GetChild(1).GetChild(1).gameObject.AddComponent<Spin>();
            Giver2.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            Giver2.transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
            Giver2.transform.GetChild(1).GetChild(3).gameObject.SetActive(false);

            Giver3.transform.GetChild(1).GetChild(2).gameObject.AddComponent<Spin>();
            Giver3.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            Giver3.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            Giver3.transform.GetChild(1).GetChild(3).gameObject.SetActive(false);

            Giver4.transform.GetChild(1).GetChild(3).gameObject.AddComponent<Spin>();
            Giver4.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            Giver4.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            Giver4.transform.GetChild(1).GetChild(2).gameObject.SetActive(false);

            ToggleGivers();
            MakeButton("Page1Button", new Vector3(-67.259f, 11.632f, -80.242f), Quaternion.Euler(0f, 61.875f, 0f));
            MakeButton("Page2Button", new Vector3(-67.259f, 11.512f, -80.242f), Quaternion.Euler(0f, 61.875f, 0f));

            str = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevRobloxGearMod.Resources.garbage");
            bundle = AssetBundle.LoadFromStream(str);
            GameObject garbageBinGameObject = bundle.LoadAsset<GameObject>("GarbageBin");
            GarbageBin = Instantiate(garbageBinGameObject);

            GarbageBin.name = "ItemRemover";
            GarbageBin.transform.localScale = new Vector3(0.667f, 0.667f, 0.667f);
            GarbageBin.transform.localPosition = new Vector3(-66.082f, 11.48f, -80.402f);
            GarbageBin.transform.localRotation = Quaternion.Euler(0f, 141.502f, 0f);
            GarbageBin.transform.SetParent(GiverFolder.transform, true);
            GarbageBin.SetActive(true);

            //garbage tree reference :))

            str = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevRobloxGearMod.Resources.outdated");
            bundle = AssetBundle.LoadFromStream(str);
            GameObject outdatedGameObject = bundle.LoadAsset<GameObject>("TextExample");
            GameObject OutdatedText = Instantiate(outdatedGameObject);
            OutdatedText.transform.position = new Vector3(-67.634f, 11.805f, -80.42f);
            OutdatedText.transform.rotation = Quaternion.Euler(0f, -28.65f, 0f);
            OutdatedText.transform.localScale = new Vector3(0.46627f, 0.46627f, 0.46627f);
            OutdatedText.transform.SetParent(null, true);
            OutdatedText.SetActive(false);//

            GarbageBin.transform.GetChild(0).gameObject.AddComponent<Give>();

            currentVersionWebClient = new WebClient();
            recentVersion = currentVersionWebClient.DownloadString("https://raw.githubusercontent.com/developer9998/DevRobloxFoodMod/main/recentversion.txt");
            if (recentVersion == "v1.0.0") { }
            else
            {
                OutdatedText.SetActive(true);
            }

        }

        private void RoomJoined(object sender, Events.RoomJoinedArgs e)
        {
            string queue = GorillaComputer.instance.currentGameMode;
            if (queue == "MODDED_devmonkbloxCASUAL" && !e.isPrivate)
            {
                GiverFolder.gameObject.SetActive(true);
                inAllowedRoom = true;
                Burger.transform.GetChild(0).gameObject.SetActive(true);
                BloxyCola.transform.GetChild(0).gameObject.SetActive(true);
                SpeedCoil.transform.GetChild(0).gameObject.SetActive(true);
                Pizza.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            if (queue == "MODDED_devmonkbloxCASUAL" && e.isPrivate)
            {
                GiverFolder.gameObject.SetActive(false);
                inAllowedRoom = false;
                Burger.transform.GetChild(0).gameObject.SetActive(false);
                BloxyCola.transform.GetChild(0).gameObject.SetActive(false);
                SpeedCoil.transform.GetChild(0).gameObject.SetActive(false);
                Pizza.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                GiverFolder.gameObject.SetActive(false);
                inAllowedRoom = false;
                Burger.transform.GetChild(0).gameObject.SetActive(false);
                BloxyCola.transform.GetChild(0).gameObject.SetActive(false);
                SpeedCoil.transform.GetChild(0).gameObject.SetActive(false);
                Pizza.transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        private void RoomLeft(object sender, Events.RoomJoinedArgs e)
        {
            GiverFolder.gameObject.SetActive(false);
            inAllowedRoom = false;
            Burger.transform.GetChild(0).gameObject.SetActive(false);
            BloxyCola.transform.GetChild(0).gameObject.SetActive(false);
            SpeedCoil.transform.GetChild(0).gameObject.SetActive(false);
            Pizza.transform.GetChild(0).gameObject.SetActive(false);
        }

        void Update() // every frame after OnGameInitialized
        {
            if (mode == 1)
            {
                distance = Vector3.Distance(Burger.transform.position, Head.transform.position);
                if (distance < 0.195f)
                {
                    if (!canEat)
                    {
                        canEat = true;
                        Burger.transform.GetChild(2).gameObject.SetActive(false);
                        Burger.transform.GetChild(2).gameObject.SetActive(true);
                    }
                }
                if (distance > 0.225f)
                {
                    if (canEat)
                    {
                        canEat = false;
                    }
                }
            }
            if (mode == 2)
            {
                distance = Vector3.Distance(BloxyCola.transform.position, Head.transform.position);
                if (distance < 0.275f)
                {
                    if (!canEat)
                    {
                        canEat = true;
                        BloxyCola.transform.GetChild(2).gameObject.SetActive(false);
                        BloxyCola.transform.GetChild(2).gameObject.SetActive(true);
                    }
                }
                if (distance > 0.315f)
                {
                    if (canEat)
                    {
                        canEat = false;
                    }
                }
            }
            if (mode == 3)
            {
                distance = Vector3.Distance(Pizza.transform.position, Head.transform.position);
                if (distance < 0.245f)
                {
                    if (!canEat)
                    {
                        canEat = true;
                        Pizza.transform.GetChild(2).gameObject.SetActive(false);
                        Pizza.transform.GetChild(2).gameObject.SetActive(true);
                    }
                }
                if (distance > 0.275f)
                {
                    if (canEat)
                    {
                        canEat = false;
                    }
                }
            }
            if (mode == 4 && inAllowedRoom)
            {
                GorillaLocomotion.Player.Instance.maxJumpSpeed = 12f;
                GorillaLocomotion.Player.Instance.jumpMultiplier = 1.5f;
            }
            else
            if (mode == 4 && !inAllowedRoom)
            {

                GorillaLocomotion.Player.Instance.maxJumpSpeed = maxJumpSpeed.Value;
                GorillaLocomotion.Player.Instance.jumpMultiplier = JumpMultiplier;
                maxJumpSpeed = GorillaLocomotion.Player.Instance.maxJumpSpeed;
                JumpMultiplier = GorillaLocomotion.Player.Instance.jumpMultiplier;
            }
            else
            {
                GorillaLocomotion.Player.Instance.maxJumpSpeed = maxJumpSpeed.Value;
                GorillaLocomotion.Player.Instance.jumpMultiplier = JumpMultiplier;
                maxJumpSpeed = GorillaLocomotion.Player.Instance.maxJumpSpeed;
                JumpMultiplier = GorillaLocomotion.Player.Instance.jumpMultiplier;
            }

        }

        [ModdedGamemodeJoin]
        private void RoomJoined(string gamemode) // joined a modded lobby
        {
            // The room is modded. Enable mod stuff.
            //inAllowedRoom = true;
            maxJumpSpeed = GorillaLocomotion.Player.Instance.maxJumpSpeed;
            JumpMultiplier = GorillaLocomotion.Player.Instance.jumpMultiplier;
        }

        [ModdedGamemodeLeave]
        private void RoomLeft(string gamemode) // left a modded lobby
        {
            // The room was left. Disable mod stuff.
           // inAllowedRoom = false;
            maxJumpSpeed = GorillaLocomotion.Player.Instance.maxJumpSpeed;
            JumpMultiplier = GorillaLocomotion.Player.Instance.jumpMultiplier;
        }

        static void MakeButton(string buttonName, Vector3 buttonPosition, Quaternion buttonRotation)
        {
            GameObject buttonObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            buttonObject.transform.SetParent(GiverFolder.transform, false);
            buttonObject.name = buttonName;
            buttonObject.transform.localPosition = buttonPosition;
            buttonObject.transform.localRotation = buttonRotation;
            buttonObject.transform.localScale = new Vector3(0.08734304f, 0.08734304f, 0.08734304f);
            buttonObject.layer = 18;
            buttonObject.GetComponent<BoxCollider>().isTrigger = true;
            buttonObject.AddComponent<PageButton>();
        }

        public static void ToggleGivers() // toggles certain givers
        {
           // Debug.Log(machinesActive[0].Value.ToString() + " " + machinesActive[1].Value.ToString() + " " + machinesActive[2].Value.ToString() + " " + machinesActive[3].Value.ToString());
            Giver1.SetActive(machinesActive[0].Value);
            Giver2.SetActive(machinesActive[1].Value);
            Giver3.SetActive(machinesActive[2].Value);
            Giver4.SetActive(machinesActive[3].Value);
        }
        public static void GiveItem(string theName) // toggles a certain item
        {
            if (theName == "BurgerGiver")
            {
                if (mode == 1) { }
                else
                {
                    Burger.SetActive(true);
                    BloxyCola.SetActive(false);
                    Pizza.SetActive(false);
                    SpeedCoil.SetActive(false);
                    mode = 1;
                    Burger.transform.GetChild(1).gameObject.SetActive(false);
                    Burger.transform.GetChild(1).gameObject.SetActive(true);
                    Burger.transform.GetChild(2).gameObject.SetActive(false);
                }
            }
            else
            if (theName == "ColaGiver")
            {
                if (mode == 2) { }
                else
                {
                    Burger.SetActive(false);
                    BloxyCola.SetActive(true);
                    Pizza.SetActive(false);
                    SpeedCoil.SetActive(false);
                    mode = 2;
                    BloxyCola.transform.GetChild(1).gameObject.SetActive(false);
                    BloxyCola.transform.GetChild(1).gameObject.SetActive(true);
                    BloxyCola.transform.GetChild(2).gameObject.SetActive(false);
                }
            }
            else
            if (theName == "ItemRemover")
            {
                Burger.SetActive(false);
                BloxyCola.SetActive(false);
                Pizza.SetActive(false);
                SpeedCoil.SetActive(false);
                mode = 0;
                Burger.transform.GetChild(1).gameObject.SetActive(false);
                Burger.transform.GetChild(2).gameObject.SetActive(false);
                BloxyCola.transform.GetChild(1).gameObject.SetActive(false);
                BloxyCola.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            if (theName == "PizzaGiver")
            {
                if (mode == 3) { }
                else
                {
                    Burger.SetActive(false);
                    BloxyCola.SetActive(false);
                    Pizza.SetActive(true);
                    SpeedCoil.SetActive(false);
                    mode = 3;
                    Pizza.transform.GetChild(1).gameObject.SetActive(false);
                    Pizza.transform.GetChild(1).gameObject.SetActive(true);
                    Pizza.transform.GetChild(2).gameObject.SetActive(false);
                }
            }
            else
            if (theName == "SpeedcoilGiver")
            {
                if (mode == 4) { }
                else
                {
                    Burger.SetActive(false);
                    BloxyCola.SetActive(false);
                    Pizza.SetActive(false);
                    SpeedCoil.SetActive(true);
                    mode = 4;
                    SpeedCoil.transform.GetChild(1).gameObject.SetActive(false);
                    SpeedCoil.transform.GetChild(1).gameObject.SetActive(true);
                    SpeedCoil.transform.GetChild(2).gameObject.SetActive(false);
                }
            }
        }
    }
}
