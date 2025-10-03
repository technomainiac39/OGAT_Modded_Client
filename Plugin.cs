using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BepInEx;
using System.Reflection;
using System.IO.Pipes;
using System.Diagnostics;
using UnityEngine;
using System.Collections;
using HarmonyLib;
using SG.OGAT;
using SG.OGAT.State;
using SG.Util;
using SG.UI;
using UnityEngine.Events;
using UnityEngine.UI;
using BepInEx.Logging;
using System.Collections.Generic;
using SG.Transport;
using Mono.Cecil.Cil;
using BepInEx.Configuration;
using SG;
using System.Net.NetworkInformation;
using OGAT_modding_API;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using UnityEngine.SocialPlatforms;

//The required Zt libs and so on for the Zt manager are installed automatically with a small pop up from windows its actually fucking great
//using unity 5.6.6f2
namespace OGAT_Modded_Client
{
    public static class Globals
    {
        public static string ZtIp;
        public static string MasterServerIP;
        public static int MasterServerPort = 23466;
        public static bool hostingMasterServer;
        public static bool ConnectedToMasterServer = false;
        public static bool HasIpAssigned = false;
        public static bool IsConnectedZT = false;
        public static bool test = false;
        public static string GameType = "OGAT_92.1";
        public static NamedPipeClientStream pipeClient;
        public static bool isHost=false;

        public static Color OGAT_Orange = new Color(0.769f, 0.439f, 0.000f, 1.000f);
        public static Color OGAT_LightGrey = new Color(30f, 32f, 36f, 1.000f);

        public static List<UnityAction<Game>> CustomGameModes = new List<UnityAction<Game>>();
        public static List<GameMode> ModdedGameModes = new List<GameMode>();
        public static Dictionary<string, Func<PlayerClass[]>> GetModdedBlueClassIds = new Dictionary<string, Func<PlayerClass[]>>();
        public static Dictionary<string, Func<PlayerClass[]>> GetModdedRedClassIds = new Dictionary<string, Func<PlayerClass[]>>();
        public static List<string> CustomGameModeNames = new List<string>();
        public static List<string> CustomGameModeIds = new List<string>();
        public static Dictionary<string, bool> CustomGameNameLoaded = new Dictionary<string, bool>();
        
        public static List<GameMode> OriginalGameModes = new List<GameMode>();
        public static List<GameObject> gameModeButtons = new List<GameObject>();
        public static bool IsModdedGameMode = false;
        public static string CurrentModdedGameAbrv = "";
        public static string CurrentModdedOrigAbrv = "";
        public static bool IsOriginalGameModesLoaded = false;
    }

    public static class API_Methods
    {

        public static void AddCustomGameMode(UnityAction<Game> gm, GameMode game, string gm_displayname, string gm_idName, Func<PlayerClass[]> GetBlueClasses, Func<PlayerClass[]> GetRedClasses)
        {
            var myLogSource = new ManualLogSource("OGAT_MODDING_API");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);
            myLogSource.LogInfo($"WWWWWWWWWWWWWWWWWWWWWWWWWWWWW {gm_displayname}");

            Globals.CustomGameModes.Add(gm);
            //Globals.ModdedGameModes.Add(game);
            Globals.CustomGameModeNames.Add(gm_displayname);
            Globals.CustomGameNameLoaded.Add(gm_displayname, false);
            Globals.CustomGameModeIds.Add(gm_idName);
            Globals.GetModdedBlueClassIds.Add(gm_idName, GetBlueClasses);
            Globals.GetModdedRedClassIds.Add(gm_idName, GetRedClasses);
            myLogSource.LogInfo($"WWWWWWWWWWWWWWWWWWWWWWWWWWWWW {Globals.CustomGameModes.Count}");
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);
        }

        public static void LoadOriginalGameModes()
        {
            var myLogSource = new ManualLogSource("OGAT_MODDING_API");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);

            GameObject obj = new GameObject();
            obj.name = "GaT";
            Mode_GaT GAT = obj.AddComponent<Mode_GaT>();
            Globals.OriginalGameModes.Add(GAT);
            GAT.gameObject.SetActive(false);

            GameObject obj2 = new GameObject();
            obj.name = "DE";
            Mode_DE DE = obj.AddComponent<Mode_DE>();
            Globals.OriginalGameModes.Add(DE);
            DE.gameObject.SetActive(false);

            GameObject obj3 = new GameObject();
            obj.name = "VIP";
            Mode_VIP VIP = obj.AddComponent<Mode_VIP>();
            Globals.OriginalGameModes.Add(VIP);
            VIP.gameObject.SetActive(false);

            GameObject obj4 = new GameObject();
            obj.name = "ZR";
            Mode_ZR ZR = obj.AddComponent<Mode_ZR>();
            Globals.OriginalGameModes.Add(ZR);
            ZR.gameObject.SetActive(false);

            GameObject obj5 = new GameObject();
            obj.name = "RES";
            Mode_RES RES = obj.AddComponent<Mode_RES>();
            Globals.OriginalGameModes.Add(RES);
            RES.gameObject.SetActive(false);

            GameObject obj1 = new GameObject();
            obj.name = "TDM";
            Mode_TDM TDM = obj.AddComponent<Mode_TDM>();
            Globals.OriginalGameModes.Add(TDM);
            TDM.gameObject.SetActive(false);

            GameObject obj6 = new GameObject();
            obj.name = "SOC";
            Mode_SOC SOC = obj.AddComponent<Mode_SOC>();
            Globals.OriginalGameModes.Add(SOC);
            SOC.gameObject.SetActive(false);

            GameObject obj7 = new GameObject();
            obj.name = "MC";
            Mode_MC MC = obj.AddComponent<Mode_MC>();
            Globals.OriginalGameModes.Add(MC);
            MC.gameObject.SetActive(false);

            GameObject obj8 = new GameObject();
            obj.name = "MAP";
            Mode_MAP MAP = obj.AddComponent<Mode_MAP>();
            //GameMode MAP = Singleton<Game>.I.gameModes[9];
            //Globals.OriginalGameModes.Add(Singleton<Game>.I.gameModes[8]);
            MAP.gameObject.SetActive(false);


            GameObject obj9 = new GameObject();
            obj.name = "TR";
            Mode_Training TR = obj.AddComponent<Mode_Training>();
            Globals.OriginalGameModes.Add(TR);
            TR.gameObject.SetActive(false);

            GameObject obj10 = new GameObject();
            obj.name = "MB";
            Mode_MB MB = obj.AddComponent<Mode_MB>();
            Globals.OriginalGameModes.Add(MB);
            MB.gameObject.SetActive(false);

            GameObject obj11 = new GameObject();
            obj.name = "IG";
            Mode_IG IG = obj.AddComponent<Mode_IG>();
            Globals.OriginalGameModes.Add(IG);
            IG.gameObject.SetActive(false);

            GameObject obj12 = new GameObject();
            obj.name = "RP";
            Mode_RP RP = obj.AddComponent<Mode_RP>();
            Globals.OriginalGameModes.Add(RP);
            RP.gameObject.SetActive(false);

            GameObject obj13 = new GameObject();
            obj.name = "CTF";
            Mode_CTF CTF = obj.AddComponent<Mode_CTF>();
            Globals.OriginalGameModes.Add(CTF);
            CTF.gameObject.SetActive(false);

            GameObject obj14 = new GameObject();
            obj.name = "igCTF";
            Mode_igCTF igCTF = obj.AddComponent<Mode_igCTF>();
            Globals.OriginalGameModes.Add(igCTF);
            igCTF.gameObject.SetActive(false);

            GameObject obj15 = new GameObject();
            obj.name = "CLNG";
            Mode_CLNG CLNG = obj.AddComponent<Mode_CLNG>();
            Globals.OriginalGameModes.Add(CLNG);
            CLNG.gameObject.SetActive(false);

            GameObject obj16 = new GameObject();
            obj.name = "gTDM";
            Mode_gTDM gTDM = obj.AddComponent<Mode_gTDM>();
            Globals.OriginalGameModes.Add(gTDM);
            gTDM.gameObject.SetActive(false);

            GameObject obj17 = new GameObject();
            obj.name = "gCTF";
            Mode_gCTF gCTF = obj.AddComponent<Mode_gCTF>();
            Globals.OriginalGameModes.Add(gCTF);
            gCTF.gameObject.SetActive(false);

            GameObject obj18 = new GameObject();
            obj.name = "sbTDM";
            Mode_sbTDM sbTDM = obj.AddComponent<Mode_sbTDM>();
            Globals.OriginalGameModes.Add(sbTDM);
            sbTDM.gameObject.SetActive(false);

            GameObject obj19 = new GameObject();
            obj.name = "RAC";
            Mode_RAC RAC = obj.AddComponent<Mode_RAC>();
            Globals.OriginalGameModes.Add(RAC);
            RAC.gameObject.SetActive(false);

            GameObject obj20 = new GameObject();
            obj.name = "SOCk";
            Mode_SOCk SOCk = obj.AddComponent<Mode_SOCk>();
            Globals.OriginalGameModes.Add(SOCk);
            SOCk.gameObject.SetActive(false);

            foreach(GameMode gm in Globals.OriginalGameModes)
            {
                gm.game = Singleton<Game>.I;
                gm.gm_LoadConfig();
                myLogSource.LogInfo($"loaded original gamemode {gm.gameObject.name} - {gm.config.FDLIEBOMBAK}");
            }
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);
        }

        public static void LoadCustomGameModeUI(MessageBoxUI parent, SGButton closeButton)
        {
            var myLogSource = new ManualLogSource("OGAT_MODDING_API");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);
            myLogSource.LogInfo($"gamemmode modded count {Globals.ModdedGameModes.Count}");
            for (int i = 0; i < (Globals.ModdedGameModes.Count); i++)
            {
                int index2 = i;
                myLogSource.LogInfo($"loop {i}, {Globals.ModdedGameModes.Count}");
                int ypos = (50 - (i * (45 + 10)));
                string name = $"{Globals.ModdedGameModes[i].name}_btn";
                myLogSource.LogInfo($"loaded button {name}, {Globals.ModdedGameModes[i].config.KHPDMLLJJFP}");
                GameObject gmButton = API_Methods.CreateSGButton(parent, name, new Vector2(250, 45), new Vector2(-710, ypos), $"{Globals.ModdedGameModes[i].config.FDLIEBOMBAK}", 20, Color.white, Color.white, Globals.OGAT_Orange, FillColour: Globals.OGAT_LightGrey,on_click: (btn) =>
                  {
                      var myLogSource2 = new ManualLogSource("OGAT_MODDING_API");
                      BepInEx.Logging.Logger.Sources.Add(myLogSource2);
                      try
                      {
                          int index = index2;
                          myLogSource2.LogInfo($"{Singleton<Game>.I.gameModes.Count}");
                         
                          myLogSource2.LogInfo($"{Singleton<Game>.I.gameModes[1]}");
                          Globals.IsModdedGameMode = true;
                          myLogSource2.LogInfo($"i is {index}");
                          Globals.CurrentModdedGameAbrv = $"{Globals.ModdedGameModes[index].gameObject.name}";
                          
                          myLogSource2.LogInfo($"{Globals.ModdedGameModes[index].gameObject.name}, {Globals.CustomGameModeNames[index]}");
                      //config.KHPDMLLJJFP is the Game name abrieveation
                      for (int j = 0; j < Singleton<Game>.I.gameModes.Count; j++)
                          {
                              if (Singleton<Game>.I.gameModes[j].config.KHPDMLLJJFP == Globals.ModdedGameModes[index].config.KHPDMLLJJFP)
                              {
                                  Singleton<Game>.I.gameModes[j] = Globals.ModdedGameModes[index];
                                  Singleton<Match>.I.SetGameModeIndex(j);
                                  Globals.CurrentModdedOrigAbrv = $"{Singleton<Game>.I.gameModes[j].config.KHPDMLLJJFP}";
                              }
                          }
 
                          myLogSource2.LogInfo($"{name}, {Singleton<Match>.I.SerializeMatchInfo()}, HFKPNENBLJI.DEDFAPOKEIE is{HFKPNENBLJI.DEDFAPOKEIE}");
                          Singleton<Lobby>.I.SetState(Lobby.JFKEGPLCGDI.SHOW_TEAMS);

                          foreach (GameMode gm in Globals.OriginalGameModes)
                          {
                              myLogSource2.LogInfo($"original gamemode stored - {gm.config.FDLIEBOMBAK}");
                          }
                          BepInEx.Logging.Logger.Sources.Remove(myLogSource2);
                          closeButton.OnClick(closeButton);
                      }
                      catch(Exception e)
                      {
                          
                          myLogSource2.LogInfo($"{e}");
                          BepInEx.Logging.Logger.Sources.Remove(myLogSource2);
                      }

                      
                  });
                gmButton.SetActive(true);
                Globals.gameModeButtons.Add(gmButton);
                
            }
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);
        }

        //these Create Objects are for message box UIs but if you wanted the code inside them could be applied to other Types of Parent you would just need to copy and paste with a different parent type


        public static GameObject CreateInputField(MessageBoxUI parent, string name, Vector2 size, Vector2 pos, Color textColor, Color bgColor,int fontSize, UnityEngine.Events.UnityAction<string> onEndEditAction, Font font = null)
        {
            // Create InputField GameObject
            GameObject inputObject = new GameObject(name);
            inputObject.transform.SetParent(parent.transform, false);

            // Add RectTransform
            RectTransform rectTransform = inputObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = pos;

            // Add Image for InputField Background
            Image backgroundImage = inputObject.AddComponent<Image>();
            backgroundImage.color = bgColor; // You can change the background color

            // Add InputField Component
            InputField inputField = inputObject.AddComponent<InputField>();
            inputField.textComponent = null; // Will be assigned later

            // Create Text for InputField
            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(inputObject.transform, false);

            Text textComponent = textObject.AddComponent<Text>();
            textComponent.text = "";
            textComponent.font = font != null ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.fontSize = fontSize;
            textComponent.color = textColor;
            textComponent.alignment = TextAnchor.MiddleLeft;

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.sizeDelta = size - new Vector2(10, 10); // Add some padding
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5, 5);
            textRect.offsetMax = new Vector2(-5, -5);

            // Set InputField's Text Component
            inputField.textComponent = textComponent;
            inputField.onEndEdit.AddListener(((input) => onEndEditAction(input)));
            // Enable the GameObject
            //inputObject.SetActive(true);

            return inputObject;
        }

        public static GameObject CreateText(MessageBoxUI parent, string name, string text, Vector2 size, Vector2 pos, Color textColor, Int32 fontSize, Font font=null)
        {
            GameObject TextObject = new GameObject(name);
            TextObject.transform.SetParent(parent.transform, false);

            Text Text = TextObject.AddComponent<Text>();
            Text.text = "Hello World";
            if (font == null) { Text.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); }
            else { Text.font = font; }
            Text.fontSize = fontSize;
            Text.color = textColor;
            Text.alignment = TextAnchor.MiddleCenter;

            RectTransform rectTransform = Text.GetComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = pos;

            return TextObject;
        }

        public static GameObject CreateSGButton(MessageBoxUI parent, string buttonName,Vector2 size, Vector2 pos, string text, Int32 fontSize,Color textColor, Color text_highlight, Color button_highlight, Font font = null, Action<SGButton> on_click=null, bool Toggle=false, Color FillColour=new Color())
        {
            // Create a new GameObject for the SGButton
            GameObject buttonObject = new GameObject(buttonName);
            SGButton newButton = buttonObject.AddComponent<SGButton>();
            
            //sets parent transform
            buttonObject.transform.SetParent(parent.transform, false);

            // This adds the highlight image that is used when clicked
            Image buttonHighlight = buttonObject.AddComponent<Image>();
            buttonHighlight.name = "HIGHLIGHT";
            buttonHighlight.color = button_highlight; //the default color of the image NOT the highlight colour unless its Fade only
            buttonHighlight.transform.SetParent(buttonObject.transform, false);
            newButton.imageHighlight = button_highlight;

            // Create a child object for the button text
            GameObject textObject = new GameObject($"{buttonName}Text");
            textObject.transform.SetParent(buttonObject.transform, false);

            // Add Text component to the child object
            Text buttonText = textObject.AddComponent<Text>();
            buttonText.name = "TEXT";
            buttonText.text = text;
            buttonText.fontSize = fontSize;
            buttonText.color = textColor;
            buttonText.alignment = TextAnchor.MiddleCenter;
            if (font == null) { buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); }
            else {  buttonText.font = font; }
            newButton.textHighlight = text_highlight;
            newButton.textNormal = textColor;


            // Configure RectTransform for layout
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.sizeDelta = size;
            textRect.anchoredPosition = Vector2.zero;

            RectTransform highlightRect = buttonHighlight.GetComponent< RectTransform>();
            highlightRect.sizeDelta = new Vector2(size.x, (size.y/10));
            highlightRect.anchoredPosition = new Vector2(0, 0);
            

            RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
            buttonRect.sizeDelta = size;
            buttonRect.anchoredPosition = pos;

            //sets onclick function
            if (on_click == null)
            {
                newButton.OnClick += (btn) =>
                {
                    var logSource = new ManualLogSource("OGAT_MODDING_API");
                    BepInEx.Logging.Logger.Sources.Add(logSource);
                    logSource.LogInfo($"Button clicked! Button name: {btn.name}");

                    // Example action: Display a message
                    parent.transform.FindChildRecursive<Text>("TXT_MESSAGE", true).text = "You clicked the Hello World button!";

                    BepInEx.Logging.Logger.Sources.Remove(logSource);
                };
            }
            else { newButton.OnClick = on_click; }

            // Enable and display
            newButton.isToggle = Toggle; //used for togglable buttons
            if(Toggle) { newButton.toggled = true; }
            //newButton.EOIJIAMCPHM = buttonText;
            //fadeOnly works fine for now
            
            newButton.highlight_anim = SGButton.OCFPFGBMCAO.FadeOnly;//SGButton.OCFPFGBMCAO.HorizontalExpandAndFade; //am not using this for now as it breaks but will eventually do that ogat underline thing
            newButton.enabled = true; ;
            newButton.OnEnable();
            //buttonObject.SetActive(true); Thats for user to do


            //000000352495ad47
            return buttonObject;
        }

        public static GameObject CreateSGButtonGENERIC(Transform parent, string buttonName, Vector2 size, Vector2 pos, string text, Int32 fontSize, Color textColor, Color text_highlight, Color button_highlight, Font font = null, Action<SGButton> on_click = null, bool Toggle = false)
        {
            // Create a new GameObject for the SGButton
            GameObject buttonObject = new GameObject(buttonName);
            SGButton newButton = buttonObject.AddComponent<SGButton>();

            //sets parent transform
            buttonObject.transform.SetParent(parent, false);

            // This adds the highlight image that is used when clicked
            Image buttonHighlight = buttonObject.AddComponent<Image>();
            buttonHighlight.name = "HIGHLIGHT";
            buttonHighlight.color = button_highlight; //the default color of the image NOT the highlight colour unless its Fade only
            buttonHighlight.transform.SetParent(buttonObject.transform, false);
            newButton.imageHighlight = button_highlight;

            // Create a child object for the button text
            GameObject textObject = new GameObject($"{buttonName}Text");
            textObject.transform.SetParent(buttonObject.transform, false);

            // Add Text component to the child object
            Text buttonText = textObject.AddComponent<Text>();
            buttonText.name = "TEXT";
            buttonText.text = text;
            buttonText.fontSize = fontSize;
            buttonText.color = textColor;
            buttonText.alignment = TextAnchor.MiddleCenter;
            if (font == null) { buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); }
            else { buttonText.font = font; }
            newButton.textHighlight = text_highlight;
            newButton.textNormal = textColor;


            // Configure RectTransform for layout
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.sizeDelta = size;
            textRect.anchoredPosition = Vector2.zero;

            RectTransform highlightRect = buttonHighlight.GetComponent<RectTransform>();
            highlightRect.sizeDelta = new Vector2(size.x, (size.y / 10));
            highlightRect.anchoredPosition = new Vector2(0, 0);


            RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
            buttonRect.sizeDelta = size;
            buttonRect.anchoredPosition = pos;

            //sets onclick function
            if (on_click == null)
            {
                newButton.OnClick += (btn) =>
                {
                    var logSource = new ManualLogSource("OGAT_MODDING_API");
                    BepInEx.Logging.Logger.Sources.Add(logSource);
                    logSource.LogInfo($"Button clicked! Button name: {btn.name}");

                    // Example action: Display a message
                    parent.transform.FindChildRecursive<Text>("TXT_MESSAGE", true).text = "You clicked the Hello World button!";

                    BepInEx.Logging.Logger.Sources.Remove(logSource);
                };
            }
            else { newButton.OnClick = on_click; }

            // Enable and display
            newButton.isToggle = Toggle; //used for togglable buttons
            if (Toggle) { newButton.toggled = true; }
            //newButton.EOIJIAMCPHM = buttonText;
            //fadeOnly works fine for now

            newButton.highlight_anim = SGButton.OCFPFGBMCAO.FadeOnly;//SGButton.OCFPFGBMCAO.HorizontalExpandAndFade; //am not using this for now as it breaks but will eventually do that ogat underline thing
            newButton.enabled = true; ;
            newButton.OnEnable();
            //buttonObject.SetActive(true); Thats for user to do


            //000000352495ad47
            return buttonObject;
        }
    }



    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("OGAT_modding_API", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin plugin;
        
        public static string lastCmd = "ip";
        public static Coroutine ConnecterCo; //so i can check wether it has connected to zt manager
        private static int tick = 0; //used to help save pc power
        private static int hosttick = 0;
        public static ConfigEntry<bool> configShowNoZTManager;
        public static ConfigEntry<bool> configShowNoMasterServer;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            configShowNoZTManager = Config.Bind("General",      // The section under which the option is shown
                                         "Show_No_ZeroTier_Manager_window",  // The key of the configuration option in the configuration file
                                         false, // The default value
                                         "ZT window set");
            configShowNoMasterServer = Config.Bind("General",      // The section under which the option is shown
                                         "Show_No_MasterServer_window",  // The key of the configuration option in the configuration file
                                         false, // The default value
                                         "ZT window set");

            var harmony = new Harmony("com.technomainiac.OGAT_Modded_Client");
            harmony.PatchAll();
        }

        public void Start()
        {
            StartZerotier();
            //ConnecterCo = StartCoroutine(ConnectToPipe());
            plugin = this;
        }
        public void Update()
        {
            if (Singleton<Game>.I.EBPLBIMDIJB == Singleton<Lobby>.I)
            {
                tick++;
                if(tick >= 9)
                {
                    //Logger.LogInfo("in Lobby");
                    if(OGAT_modding_API.API_Methods.GetHostName() == NetPlayer.Mine.profile.username)
                    {
                        Globals.isHost = true;
                        //Logger.LogInfo("You are the Host");
                    }
                }
            }

            if(Globals.IsConnectedZT == true)
            {
               
                tick++;
                if (tick >= 9) 
                {
                    if (string.IsNullOrEmpty(Globals.ZtIp))
                    {
                        tick = 0;
                        foreach (NetworkInterface netInt in NetworkInterface.GetAllNetworkInterfaces())
                        {
                            // Often ZeroTier adapters have "z" in their name.
                            if (netInt.Description.Contains("ZeroTier") ||
                                netInt.Description.Contains("zt") ||
                                netInt.Name.Contains("zt"))
                            {
                                foreach (UnicastIPAddressInformation ip in netInt.GetIPProperties().UnicastAddresses)
                                {
                                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                                        !IPAddress.IsLoopback(ip.Address) &&
                                        ip.Address.ToString().StartsWith("10.") ||
                                        ip.Address.ToString().StartsWith("172.") ||
                                        ip.Address.ToString().StartsWith("192."))
                                    {
                                        Globals.HasIpAssigned = true;
                                        Globals.ZtIp = ip.Address.ToString();
                                        Logger.LogInfo($"IP is : {Globals.ZtIp}");
                                    }
                                }
                            }
                        }
                        return;
                    }
                    tick = 0;
                }
                
            }
            
           
        }
        /////////////////////////////////////////////////////////////////////////////// KLEP.. class debug

        public static void LogAllKeys(KLEPJCJNCIJ node, ManualLogSource logger, int indent = 0)
        {
            string pad = new string(' ', indent * 2);

            if (node.type == KLEPJCJNCIJ.MBDEHGCIIJM.DICT && node.data is Dictionary<string, KLEPJCJNCIJ> dict)
            {
                foreach (var kvp in dict)
                {
                    logger.LogInfo($"{pad}{kvp.Key}");
                    LogAllKeys(kvp.Value, logger, indent + 1);
                }
            }
            else if (node.type == KLEPJCJNCIJ.MBDEHGCIIJM.LIST && node.data is List<KLEPJCJNCIJ> list)
            {
                foreach (var item in list)
                {
                    LogAllKeys(item, logger, indent + 1);
                }
            }
        }



        /// ZERO-TIER MANAGER STUFF ###############################################################################

        //starts the Zt manager application ready to connect to using pipes
        private void StartZerotier()
        {
            try
            {
                Logger.LogInfo($"Starting Zerotier Manager...");
                string startupPath = System.IO.Directory.GetCurrentDirectory();
                string pathZtMan = startupPath + "\\BepInEx\\plugins\\OGAT_ModdedClient\\OGAT_ZeroTierManager\\OGAT_ZeroTierManager.exe";
                string cmd = $"/c start \"\" \"{pathZtMan}\"";//$"start \"test\" \"{pathZtMan}\" ";
                Logger.LogInfo(pathZtMan + cmd);

                if (configShowNoZTManager.Value == false)
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = cmd, // 'start' opens a new window
                        UseShellExecute = true,
                        CreateNoWindow = false
                    };
                    //psi.EnvironmentVariables["__COMPAT_LAYER"] = "DetachedProcess";
                    Process.Start(psi);
                }
                else
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = cmd, // 'start' opens a new window
                        UseShellExecute = true,
                        CreateNoWindow = false
                    };
                    //psi.EnvironmentVariables["__COMPAT_LAYER"] = "DetachedProcess";
                    Process.Start(psi);
                }
                
                Logger.LogInfo($"Zerotier Manager started");
                Thread.Sleep(50);
                Globals.IsConnectedZT = true;
                //Globals.pipeClient = new NamedPipeClientStream(".", "ZeroTierPipe", PipeDirection.InOut);
                
            }
            catch (Exception ex)
            {
                Logger.LogInfo("Error: " + ex.Message);
            }        
        }

        /// UNITY MASTER SERVER STUFF ###############################################################################
        private void StartMasterServer() 
        {
            Logger.LogInfo("Starting Unity MasterServer...");
            string startupPath = System.IO.Directory.GetCurrentDirectory();
            string pathZtMan = startupPath + "\\BepInEx\\plugins\\OGAT_ModdedClient\\OGAT_MasterServer\\MasterServer.exe";
            string cmd = $"/c start \"\" \"{pathZtMan}\" -i {Globals.ZtIp}";//$"start \"test\" \"{pathZtMan}\"";
            Logger.LogInfo(pathZtMan + cmd);

            if(configShowNoMasterServer.Value == false) 
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = cmd, // 'start' opens a new window $"start \"\" \'{pathZtMan}\' "
                    UseShellExecute = true,
                    CreateNoWindow = false
                };
                //psi.EnvironmentVariables["__COMPAT_LAYER"] = "DetachedProcess";
                Process.Start(psi);
            }
            else
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = cmd, // 'start' opens a new window
                    UseShellExecute = true,
                    CreateNoWindow = false
                };
                //psi.EnvironmentVariables["__COMPAT_LAYER"] = "DetachedProcess";
                Process.Start(psi);
            }
            

            
        }
        public static void HostMasterServer(SGButton causer)
        {
            plugin.StartMasterServer();
        }

        public static void SetMasterServerIP(string IP, bool InputField)
        {
            Globals.test = true;
            var myLogSource = new ManualLogSource("OGAT_MODDING_API");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);
            myLogSource.LogInfo($"input entered {IP}, {Globals.test}");
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);
            Globals.MasterServerIP = IP;
            
            ConnectToMasterServer(InputField);
        }
        public static void ConnectToMasterServer(bool IpTyped= false)
        {
            try
            {
                Network.Disconnect();
                MasterServer.ipAddress = Globals.MasterServerIP;
                MasterServer.port = Globals.MasterServerPort;
                Network.natFacilitatorIP = Globals.MasterServerIP;
                Network.natFacilitatorPort = 50005;
                Netcode netcode = ObjectCache.Get<Netcode>(true);
                if (netcode != null)
                {
                    var myLogSource = new ManualLogSource("OGAT_MODDING_API");
                    BepInEx.Logging.Logger.Sources.Add(myLogSource);
                    myLogSource.LogInfo($"hmmmm");

                    netcode.MasterServers.Clear();
                    netcode.MasterServers.Add(new MasterServerInfo()
                    {
                        Name = "ModdedMasterServer",
                        //Host = "127.0.0.1",
                        Host = Globals.MasterServerIP,
                        Port = 23466,
                        myPublicEndPoint = ""       //////////////////maybe somethign
                    });
                    Globals.ConnectedToMasterServer = true;
                }
                if (IpTyped != false)
                {
                    //IpTyped.text = "Connected to masterServer";    
                }
            }
            catch(Exception ex)
            {
                var myLogSource = new ManualLogSource("OGAT_MODDING_API");
                BepInEx.Logging.Logger.Sources.Add(myLogSource);
                myLogSource.LogInfo($"{ex}");
                BepInEx.Logging.Logger.Sources.Remove(myLogSource);
            }
            

        }

        public static IEnumerator WaitTillShowButton(Func<bool> condition, SGButton buttonToHold, Action onComplete=null)
        {
            var myLogSource = new ManualLogSource("OGAT_MODDING_API");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);
            myLogSource.LogInfo("hiding button");
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);
            while (condition() == false)
            {
                yield return new WaitForSeconds(0.5f);
            }
            BepInEx.Logging.Logger.Sources.Add(myLogSource);
            myLogSource.LogInfo("showing button");
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);
            buttonToHold.gameObject.SetActive(true);
            if (onComplete != null) { onComplete(); }
            
        }
        public static void RunCoroutine(IEnumerator toRun)
        {
            Coroutine coroutine;
            coroutine = plugin.StartCoroutine(toRun);
        }
        public static IEnumerator ShowMessageDelayed(string title, string msg, Action action)
        {
            yield return new WaitForSeconds(0.5f);
            Singleton<MessageBoxUI>.I.Show(title, msg, action);
        }
       
    }
    [HarmonyPatch]
    public class MasterServerPatches
    {
        [HarmonyPostfix]//force connects to masterserver when server list is clicked
        [HarmonyPatch(typeof(ServerList), "OnEnterState")]
        public static void WaitForZtIP(ServerList __instance)
        {
            var myLogSource = new ManualLogSource("OGAT_MODDING_API");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);
            myLogSource.LogInfo($"About to wait for IP address");
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);

            //allows you to change the masterserver IP and see your own
            GameObject hostButton = API_Methods.CreateSGButtonGENERIC(__instance.AGLMLJACABD, "reEnterMasterServer", new Vector2(410, 50), new Vector2(-710, -395), "Change Master Server", 24, Color.white, Color.white, Globals.OGAT_Orange, on_click: (btn) =>
            {
                Singleton<MessageBoxUI>.I.Show("#HOSTORCLIENT", "does it", null);
            });
            hostButton.SetActive(true);
            GameObject ShowIP = API_Methods.CreateSGButtonGENERIC(__instance.AGLMLJACABD, "FuckedUp", new Vector2(350, 50), new Vector2(710, -395), "Show my host IP", 24, Color.white, Color.white, Globals.OGAT_Orange, on_click: (btn) =>
            {
                Singleton<MessageBoxUI>.I.Show("#DISPLAYIP", "does it", null);
            });
            //starts the host or client message box right after
            if (!Globals.ConnectedToMasterServer)
            {
                Singleton<MessageBoxUI>.I.Show("#WAITFORZTIP", "does it", () => { Plugin.RunCoroutine(Plugin.ShowMessageDelayed("#HOSTORCLIENT", "well well well", null)); });
            }

            
        }
        //next thing to log is how the Game.ServerStart method works as that is responsible for actually hosting the server
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ServerList), "UpdateServerList")]
        public static void LogUpdateServerList(ServerList __instance)
        {
            var myLogSource = new ManualLogSource("OGAT_MODDING_API");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);
            myLogSource.LogInfo($"updating serverlist {__instance}, this is show ip: {__instance.showIP}");
            
            //trying to see if when the dedicated servers are actually removed from the update check
            HostList hostList = ObjectCache.Get<HostList>(true);
            for(int i = 0; i < hostList.HBEMNHBBPIM; i++)
	        {
                ServerInfo server = hostList.GetServer(i);
                myLogSource.LogInfo($"server in server list: {server.RoomName}");
            }
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Lobby), "OnEnterState")]
        public static void AddCustomGameModeButton(Lobby __instance)
        {
            var myLogSource = new ManualLogSource("OGAT_MODDING_API");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);
            myLogSource.LogInfo($"entering lobby {Globals.IsModdedGameMode} {Globals.CurrentModdedGameAbrv}");
            foreach (PlayerClass p in Singleton<Game>.I.prefab_redClasses)
            {
                if (p != null)
                {
                    myLogSource.LogInfo($"loaded redTeam class: {p.className}, {p.GetClassId()}");
                }
            }
            foreach (PlayerClass p in Singleton<Game>.I.prefab_blueClasses)
            {
                if (p != null)
                {
                    myLogSource.LogInfo($"loaded blueTeam class: {p.className}, {p.GetClassId()}");
                }
            }
            //here loads in the modded gameModes
            try
            {
                if(!Globals.IsModdedGameMode)
                {
                    Globals.OriginalGameModes.Clear();
                    API_Methods.LoadOriginalGameModes();
                    Globals.OriginalGameModes = Singleton<Game>.I.gameModes.ToList();
                    //Globals.OriginalGameModes = Singleton<Game>.I.gameModes;
                    Singleton<Game>.I.gameModes = Globals.OriginalGameModes.ToList();
                    foreach (GameMode gm in Globals.OriginalGameModes)
                    {
                        myLogSource.LogInfo($"original gamemode stored looking for map- {gm.config.FDLIEBOMBAK}");
                    }
                }

                foreach (UnityAction<Game> gm in Globals.CustomGameModes)
                {
                    gm(Singleton<Game>.I);
                }
                if (Singleton<Game>.I.gameModes.Count > 21)
                {
                    myLogSource.LogInfo($"Called the right function thingy");


                    for (int i = 0; i < (Globals.CustomGameModes.Count); i++)
                    {
                        int index = 21;

                        myLogSource.LogInfo($"CustomCount {Globals.CustomGameModes.Count}, modded count {Globals.ModdedGameModes.Count}, {index}");

                        bool isInListAlready = false;
                        foreach (GameMode gmm in Globals.ModdedGameModes)
                        {
                            if (gmm.config.FDLIEBOMBAK == Singleton<Game>.I.gameModes[index].config.FDLIEBOMBAK)
                            {
                                isInListAlready = true;
                            }
                        }
                        
                        if (!isInListAlready) { Globals.ModdedGameModes.Add(Singleton<Game>.I.gameModes[index]); }
                        myLogSource.LogInfo($"CustomCount {Globals.CustomGameModes.Count}, modded count {Globals.ModdedGameModes.Count}, {i}");
                        myLogSource.LogInfo($"Called the right {i}");
                        myLogSource.LogInfo($"Called the right {Globals.ModdedGameModes[i].config.FDLIEBOMBAK}, {Singleton<Game>.I.gameModes[index].gameObject.name}");
                        Singleton<Game>.I.gameModes.Remove(Singleton<Game>.I.gameModes[index]);
                    }
                    myLogSource.LogInfo($"finished {Singleton<Game>.I.gameModes.Count} normal gms, {Globals.ModdedGameModes.Count} modded gms");
                }
            }
            catch (Exception e)
            {
                myLogSource.LogInfo(e.ToString());
            }


            myLogSource.LogInfo($"{OGAT_modding_API.API_Methods.GetHostName()}, {NetPlayer.Mine.profile.username}");

            GameObject ShowCustomGameModes = API_Methods.CreateSGButtonGENERIC(__instance.AGLMLJACABD, "CustomGameModes", new Vector2(350, 50), new Vector2(-710, -270), "Custom Game Modes", 24, Color.white, Color.white, Globals.OGAT_Orange, on_click: (btn) =>
            {
                    Singleton<MessageBoxUI>.I.Show("cat", "meow", null);

            });
            ShowCustomGameModes.SetActive(false);
            Plugin.RunCoroutine((Plugin.WaitTillShowButton(() => Globals.isHost, ShowCustomGameModes.GetComponent<SGButton>())));

            GameObject UnLoadGameModdedGameModes = API_Methods.CreateSGButtonGENERIC(__instance.AGLMLJACABD, "UnloadGameModes", new Vector2(375, 50), new Vector2(-710, -330), "Unload Custom Game Modes", 24, Color.white, Color.white, Globals.OGAT_Orange, on_click: (btn) =>
            {
            Singleton<Game>.I.gameModes = Globals.OriginalGameModes.ToList();
            // Singleton<Match>.I.SetGameModeIndex(0);
            myLogSource.LogInfo($"unloaded GameModes {Singleton<Game>.I.gameModes.Count} normal GameModes");
            Globals.CurrentModdedGameAbrv = "";
            Globals.IsModdedGameMode = false;
                Singleton<Match>.I.SetGameModeIndex(0);
                myLogSource.LogInfo($"{ Singleton<Match>.I.SerializeMatchInfo()}");
            
            });
            UnLoadGameModdedGameModes.SetActive(false);
            Plugin.RunCoroutine((Plugin.WaitTillShowButton(() => Globals.isHost, UnLoadGameModdedGameModes.GetComponent<SGButton>())));
            myLogSource.LogInfo($"enter lobby: game modes {Singleton<Game>.I.gameModes.Count}");
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MessageBoxUI), "Show")]
        public static void ManageCustomMessageBoxes(MessageBoxUI __instance, string OLEPFGPMEPP, string LKPHAKGDPGL, Action CDHJDMEBOCF)
        {
            Text msg = __instance.transform.FindChildRecursive<Text>("TXT_MESSAGE", true);
            Text title = __instance.transform.FindChildRecursive<Text>("TXT_TITLE", true);
            RectTransform hline = __instance.transform.FindChildRecursive<RectTransform>("HLINE", true); //i think its the rectangle boarder of the message
            SGButton close = __instance.transform.FindChildRecursive<SGButton>("BTN_OK", true);
           

            var myLogSource = new ManualLogSource("OGAT_MODDING_API");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);
            myLogSource.LogInfo($"OGAT ORANGE {close.imageHighlight},      {OLEPFGPMEPP}");
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);

            if (OLEPFGPMEPP == "#GETZTIPADDRESS")
            {
                Plugin.RunCoroutine((Plugin.WaitTillShowButton(() => Globals.HasIpAssigned, close)));
                close.gameObject.SetActive(false);

                GameObject testText = API_Methods.CreateText(__instance, "testText", "testing testing 123", new Vector2(350, 50), new Vector2(0, 20), Color.white, 24);
                GameObject testButton = API_Methods.CreateSGButton(__instance, "testButton", new Vector2(200, 50), new Vector2(0, 100), "Cancel", 24, Color.white, Color.white, Color.white);
                //just to clean up my stuff
                close.OnClick += (btn) =>
                {
                    // Perform cleanup when the button is clicked
                    if (testText != null) GameObject.Destroy(testText);
                    if (testButton != null) GameObject.Destroy(testButton);
                };

                testText.SetActive(true);
                testButton.SetActive(true);

            }

           

            else if (OLEPFGPMEPP == "#WAITFORZTIP")
            {
                
                if (Globals.HasIpAssigned)
                {
                    msg.text = "IP Address has already been assigned... your welcome";
                    title.text = "I was one step ahead";
                }
                else
                {
                    msg.text = "Waiting for IP Address to be assigned...";
                    title.text = "Waiting For IP";
                    close.gameObject.SetActive(false);
                    //close.EOIJIAMCPHM.text = "close";
                    Plugin.RunCoroutine((Plugin.WaitTillShowButton(() => Globals.HasIpAssigned, close, () =>
                    {
                        msg.text = "IP has been assigned";
                        title.text = "IP Assigned";      
                    })));
                } 
 
            }
            else if (OLEPFGPMEPP == "#HOSTORCLIENT")
            {
                title.text =  "Host or Join a Master Server";
                GameObject clientButton = new GameObject("placeholder2");
                GameObject hostButton = new GameObject("placeholder");
                bool oneChoiceClicked = false;
                msg.text = "Do you want to Host or Join a MasterServer";
                close.gameObject.SetActive(false);
                close.OnClick += (btn) =>
                {
                    // Perform cleanup when the button is clicked
                    if (hostButton != null) GameObject.Destroy(hostButton);
                    if (clientButton != null) GameObject.Destroy(clientButton);
                    //if (HostIpInput != null) GameObject.Destroy(HostIpInput);
                };

                hostButton = API_Methods.CreateSGButton(__instance, "hostButton", new Vector2(200, 50), new Vector2(125, -50), "Host", 24, Color.white, Color.white, Globals.OGAT_Orange, on_click: (btn) =>
                {
                    //logic for trying to see if zerotier ip address is working
                    string ipFromUnity = Network.player.ipAddress;
                    string exIPFromUnity = Network.player.externalIP;
                    

                    var myLogSource2 = new ManualLogSource("OGAT_MODDING_API");
                    BepInEx.Logging.Logger.Sources.Add(myLogSource2);
                    myLogSource2.LogInfo($"IP from Unity is: internal {ipFromUnity}, external {exIPFromUnity} on port {Network.player.externalPort}");
                    BepInEx.Logging.Logger.Sources.Remove(myLogSource2);
                    ////
                    oneChoiceClicked = true;
                    Globals.MasterServerPort = 23466;
                    Plugin.HostMasterServer(close);
                    msg.text = "Hosting MasterServer";
                    Globals.MasterServerIP = "127.0.0.1";
                    //Globals.MasterServerIP = Globals.ZtIp;
                    Globals.hostingMasterServer = true;
                    new WaitForSeconds(1f);
                    Plugin.ConnectToMasterServer();
                    clientButton.gameObject.SetActive(false);
                    close.gameObject.SetActive(true);
                    close.OnClick(close);

                    //ok so it appears that a server isnt added to the server list but I probably need to debug more with another device to be sure


                    //trying to make a dummy server in order to see if update server list is working on my own
                    Singleton<Game>.I.Server_start("test", "", false);

                    //I think instead of calling Singleton<ServerList>.I.UpdateServerList(); as an Action here I should actually call the StartCoroutine method (if it isnt just a generic method)
                    //as I think this will constistanbtly update the server list which fixes the issue of servers not showing up
                    //nevermind StartCoroutine is a monobehaviour function, will need to run with the new logger to check if it is still called frequently after new masterserver is set
                    Plugin.RunCoroutine(Plugin.ShowMessageDelayed("#DISPLAYIP", "mwa ha ha", () => { Singleton<ServerList>.I.UpdateServerList(); }));

                });
                clientButton = API_Methods.CreateSGButton(__instance, "clientButton", new Vector2(200, 50), new Vector2(-125, -50), "Join", 24, Color.white, Color.white, Globals.OGAT_Orange, on_click: (btn) =>
                {
                    oneChoiceClicked = true;
                    Globals.hostingMasterServer = false;
                    Globals.ConnectedToMasterServer = false;
                    hostButton.SetActive(false);
                    //HostIpInput.SetActive(true);

                    //I think instead of calling Singleton<ServerList>.I.UpdateServerList(); as an Action here I should actually call the StartCoroutine method (if it isnt just a generic method)
                    //as I think this will constistanbtly update the server list which fixes the issue of servers not showing up
                    Plugin.RunCoroutine(Plugin.ShowMessageDelayed("#ENTERMASTERSERVERIP", "well well well", ()=> { Singleton<ServerList>.I.UpdateServerList(); }));
                    close.gameObject.SetActive(true);
                    close.OnClick(close);
                });
                
                hostButton.SetActive(true);
                clientButton.SetActive(true);

            }
            else if(OLEPFGPMEPP == "#ENTERMASTERSERVERIP")
            {
                msg.text = "Enter the master server IP in the text box";
                title.text = "Enter Master Server IP";

                GameObject HostIpInput = API_Methods.CreateInputField(__instance, "HostIpInput", new Vector2(800, 50), new Vector2(0, 0), Color.white, Color.black, 24, onEndEditAction: (input)=>
                {
                    var myLogSource = new ManualLogSource("OGAT_MODDING_API");
                    BepInEx.Logging.Logger.Sources.Add(myLogSource);
                    myLogSource.LogInfo($"INPUT BOX INPUT {input}");
                    BepInEx.Logging.Logger.Sources.Remove(myLogSource);
                    Plugin.SetMasterServerIP(input, true);
                });
                HostIpInput.GetComponent<InputField>().text = "Enter Host Ip Address here...";
                HostIpInput.SetActive(true);

                close.OnClick += (btn) =>
                {
                    // Perform cleanup when the button is clicked
                    if (HostIpInput != null) GameObject.Destroy(HostIpInput);
                };

                close.gameObject.SetActive(false);
                Plugin.RunCoroutine(Plugin.WaitTillShowButton(() => Globals.ConnectedToMasterServer, close, () =>
                {
                    msg.text = "Connected to MasterServer";
                    title.text = "Connected To MasterServer";
                }));
            }
            else if(OLEPFGPMEPP == "#DISPLAYIP")
            {
                msg.text = "Here is your host IP Address copy it and share with friends, if you fuck that up press the i fucked up button and copy it again";
                title.text = "Host Ip Address";
                GameObject HostIP = API_Methods.CreateInputField(__instance, "HostIp", new Vector2(500, 50), new Vector2(-250, -50), Color.white, Color.black, 24, onEndEditAction: (input) =>{});
                HostIP.GetComponent<InputField>().text = $"{Globals.ZtIp}";
                HostIP.SetActive(true);

                GameObject FuckedUpButton = API_Methods.CreateSGButton(__instance, "FuckedUp", new Vector2(300, 50), new Vector2(200, -50), "I fucked up", 24, Color.white, Color.white, Globals.OGAT_Orange, on_click: (btn) =>
                {
                   HostIP.GetComponent<InputField>().text = $"{Globals.ZtIp}";
                    msg.text = "wow we have a dumbass in our midst be gratefull I'm gracious";
                });
                FuckedUpButton.SetActive(true);
                close.OnClick += (btn) =>
                {
                    // Perform cleanup when the button is clicked
                    if (HostIP != null) GameObject.Destroy(HostIP);
                    if (FuckedUpButton != null) GameObject.Destroy(FuckedUpButton);
                };

            }
            else if (OLEPFGPMEPP == "cat")
            {

                myLogSource = new ManualLogSource("OGAT_MODDING_API");
                BepInEx.Logging.Logger.Sources.Add(myLogSource);
                myLogSource.LogInfo($"Called the right function thingy");


               // close.EOIJIAMCPHM.text = "cancel";
                 title.text = "Select Custom Gamemode";
                 msg.text = "";

                try
                {
                    API_Methods.LoadCustomGameModeUI(__instance, close);
                }
                catch (Exception e)
                {
                    myLogSource.LogInfo(e.ToString());
                } 
                close.OnClick += (btn) =>
                {
                    // Perform cleanup when the button is clicked
                    if (Globals.gameModeButtons != null)
                    {
                        foreach (GameObject game in Globals.gameModeButtons)
                        {
                            GameObject.Destroy(game);
                        }
                    }
                };
                myLogSource.LogInfo($"Called the right function");
                BepInEx.Logging.Logger.Sources.Remove(myLogSource);
            }

        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Game), "Server_start")]
        public static bool StartServerPatch(Game __instance, string DMMPKHIECJK, string EDCCGAHKIJB, bool AAMKAMCPAFO)      //important sets nat to false so the local servers work
        {
            var myLogSource = new ManualLogSource("OGAT_MODDED_CLIENT");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);

            JIFNDMNAFHL.HJHKJJMPHBB(" *** Starting server ***", new object[0]);
            SG.Analytics.NewEvent("StartHosting", 0f, DMMPKHIECJK);
            __instance.serverNameTxt.CLPBEAGICAJ = DMMPKHIECJK;
            __instance.connectedServerName = DMMPKHIECJK;
            Network.incomingPassword = EDCCGAHKIJB;
            __instance.isSinglePlayer = AAMKAMCPAFO;
            if (AAMKAMCPAFO)
            {
                Network.InitializeServer(0, OCAPFBIIEAI.DJELCJAIFAC, false);
                myLogSource.LogInfo("Starting server is single player is true");
            }
            else
            {
                myLogSource.LogInfo("Starting server is single player is false");
                bool useNat = false;
                Network.InitializeServer(Singleton<Match>.I.maxNumConnections.val, OCAPFBIIEAI.DJELCJAIFAC, useNat);
                __instance.needMasterServerRegistration = true;
                __instance.autoQuitCountdown.val = OCAPFBIIEAI.EACADINHECO.val;
            }
            Singleton<Match>.I.Reset();
            __instance.UseState("lobby");
            if (OCAPFBIIEAI.DEINGFBMAFB)
            {
                SGNet.I.ctok = NetPlayer.generateCtok();
            }
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch (typeof(Game), "OnFailedToConnectToMasterServer")]
        public static void FailedTOCOnnectToCustomMasterServer()
        {
            Globals.ConnectedToMasterServer = false;
            Plugin.RunCoroutine(Plugin.ShowMessageDelayed("#HOSTORCLIENT", "well well well", null));
        }

        ///////Manage custom game Modes/////////////////////////////////////////////////////////////////////////////////////////////
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Game), "LoadGameModes")]
        public static bool LoadCustomGameModes(Game __instance)
        {

            var myLogSource = new ManualLogSource("OGAT_GuardsInTheDark_Mod");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);

            //////////////////need to set original gamemodes by hand bc c# uses references for lists which is normally great but here it really fucks me up
            if (!Globals.IsOriginalGameModesLoaded)
            {
                GameObject obj = new GameObject();
                obj.name = "GaT";
                Mode_GaT GAT = obj.AddComponent<Mode_GaT>();
                Globals.OriginalGameModes.Add(GAT);
                GAT.gameObject.SetActive(false);

                GameObject obj2 = new GameObject();
                obj.name = "DE";
                Mode_DE DE = obj.AddComponent<Mode_DE>();
                Globals.OriginalGameModes.Add(DE);
                DE.gameObject.SetActive(false);

                GameObject obj3 = new GameObject();
                obj.name = "VIP";
                Mode_VIP VIP = obj.AddComponent<Mode_VIP>();
                Globals.OriginalGameModes.Add(VIP);
                VIP.gameObject.SetActive(false);

                GameObject obj4 = new GameObject();
                obj.name = "ZR";
                Mode_ZR ZR = obj.AddComponent<Mode_ZR>();
                Globals.OriginalGameModes.Add(ZR);
                ZR.gameObject.SetActive(false);

                GameObject obj5 = new GameObject();
                obj.name = "RES";
                Mode_RES RES = obj.AddComponent<Mode_RES>();
                Globals.OriginalGameModes.Add(RES);
                RES.gameObject.SetActive(false);

                GameObject obj1 = new GameObject();
                obj.name = "TDM";
                Mode_TDM TDM = obj.AddComponent<Mode_TDM>();
                Globals.OriginalGameModes.Add(TDM);
                TDM.gameObject.SetActive(false);

                GameObject obj6 = new GameObject();
                obj.name = "SOC";
                Mode_SOC SOC = obj.AddComponent<Mode_SOC>();
                Globals.OriginalGameModes.Add(SOC);
                SOC.gameObject.SetActive(false);

                GameObject obj7 = new GameObject();
                obj.name = "MC";
                Mode_MC MC = obj.AddComponent<Mode_MC>();
                Globals.OriginalGameModes.Add(MC);
                MC.gameObject.SetActive(false);

                GameObject obj8 = new GameObject();
                obj.name = "MAP";
                Mode_MAP MAP = obj.AddComponent<Mode_MAP>();
                //GameMode MAP = Singleton<Game>.I.gameModes[9];
                Globals.OriginalGameModes.Add(MAP);
                MAP.gameObject.SetActive(false);

                GameObject obj9 = new GameObject();
                obj.name = "TR";
                Mode_Training TR = obj.AddComponent<Mode_Training>();
                Globals.OriginalGameModes.Add(TR);
                TR.gameObject.SetActive(false);

                GameObject obj10 = new GameObject();
                obj.name = "MB";
                Mode_MB MB = obj.AddComponent<Mode_MB>();
                Globals.OriginalGameModes.Add(MB);
                MB.gameObject.SetActive(false);

                GameObject obj11 = new GameObject();
                obj.name = "IG";
                Mode_IG IG = obj.AddComponent<Mode_IG>();
                Globals.OriginalGameModes.Add(IG);
                IG.gameObject.SetActive(false);

                GameObject obj12 = new GameObject();
                obj.name = "RP";
                Mode_RP RP = obj.AddComponent<Mode_RP>();
                Globals.OriginalGameModes.Add(RP);
                RP.gameObject.SetActive(false);

                GameObject obj13 = new GameObject();
                obj.name = "CTF";
                Mode_CTF CTF = obj.AddComponent<Mode_CTF>();
                Globals.OriginalGameModes.Add(CTF);
                CTF.gameObject.SetActive(false);

                GameObject obj14 = new GameObject();
                obj.name = "igCTF";
                Mode_igCTF igCTF = obj.AddComponent<Mode_igCTF>();
                Globals.OriginalGameModes.Add(igCTF);
                igCTF.gameObject.SetActive(false);

                GameObject obj15 = new GameObject();
                obj.name = "CLNG";
                Mode_CLNG CLNG = obj.AddComponent<Mode_CLNG>();
                Globals.OriginalGameModes.Add(CLNG);
                CLNG.gameObject.SetActive(false);

                GameObject obj16 = new GameObject();
                obj.name = "gTDM";
                Mode_gTDM gTDM = obj.AddComponent<Mode_gTDM>();
                Globals.OriginalGameModes.Add(gTDM);
                gTDM.gameObject.SetActive(false);

                GameObject obj17 = new GameObject();
                obj.name = "gCTF";
                Mode_gCTF gCTF = obj.AddComponent<Mode_gCTF>();
                Globals.OriginalGameModes.Add(gCTF);
                gCTF.gameObject.SetActive(false);

                GameObject obj18 = new GameObject();
                obj.name = "sbTDM";
                Mode_sbTDM sbTDM = obj.AddComponent<Mode_sbTDM>();
                Globals.OriginalGameModes.Add(sbTDM);
                sbTDM.gameObject.SetActive(false);

                GameObject obj19 = new GameObject();
                obj.name = "RAC";
                Mode_RAC RAC = obj.AddComponent<Mode_RAC>();
                Globals.OriginalGameModes.Add(RAC);
                RAC.gameObject.SetActive(false);

                GameObject obj20 = new GameObject();
                obj.name = "SOCk";
                Mode_SOCk SOCk = obj.AddComponent<Mode_SOCk>();
                Globals.OriginalGameModes.Add(SOCk);
                SOCk.gameObject.SetActive(false);
            }
            //////////////////////

            for (int i=0; i<__instance.gameModes.Count;i++)
            {
                __instance.gameModes[i].game = __instance;
                __instance.gameModes[i].gm_LoadConfig();
                myLogSource.LogInfo($"loaded gamemode {__instance.gameModes[i]} - {__instance.gameModes[i].config.FDLIEBOMBAK}");

                
                if (!Globals.IsOriginalGameModesLoaded)
                {
                    Globals.OriginalGameModes[i].game = __instance;
                    Globals.OriginalGameModes[i].gm_LoadConfig();
                    myLogSource.LogInfo($"loaded original gamemode {Globals.OriginalGameModes[i]} - {Globals.OriginalGameModes[i].config.FDLIEBOMBAK}");
                }
                
                for (int j = 0; j < (Globals.CustomGameModes.Count); j++)
                {
                    if (__instance.gameModes[i].config.FDLIEBOMBAK == Globals.CustomGameModeNames[j])
                    {
                        Globals.CustomGameNameLoaded[Globals.CustomGameModeNames[j]] = true;
                        Globals.ModdedGameModes.Add(__instance.gameModes[i]);
                    }
                }
                
            }

            Globals.OriginalGameModes = __instance.gameModes.ToList();

            foreach(GameMode gm in Globals.OriginalGameModes)
            {
                myLogSource.LogInfo($"original gamemode stored - {gm.config.FDLIEBOMBAK}");
            }

            Globals.IsOriginalGameModesLoaded = true;
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);
            return false;
        }
        /*
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Lobby), "OnEnterState")]   //#############################################################################################
        public static void AddGameModesToVoting(Lobby __instance)
        {
            for(int i = 0;i < Globals.CustomGameModes.Count; i++)
          {
               __instance.voting_valid_modes.Add(Globals.CustomGameModeIds[i]);
           }
        }*/
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Netcode), "RegisterHost")]
        public static bool CheckingRegisterHost(Netcode __instance, uint KNKHIGBJOAE, uint IBCJPIPFMFM, string DDGMNHCHIDI, string DMMPKHIECJK, string DJNBJCKFHID, int MCGGLEJMBMM, int IPDBOPGAFKN, string OANEPPJGNMN)
        {
            var myLogSource = new ManualLogSource("OGAT_MODDING_API");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);
            myLogSource.LogInfo($"Registering Host");
            myLogSource.LogInfo($" {KNKHIGBJOAE}");
            myLogSource.LogInfo($" {IBCJPIPFMFM}");
            myLogSource.LogInfo($" {DDGMNHCHIDI}");
            myLogSource.LogInfo($" {DMMPKHIECJK}");
            myLogSource.LogInfo($" {DJNBJCKFHID}");
            myLogSource.LogInfo($" {MCGGLEJMBMM}");
            myLogSource.LogInfo($" {IPDBOPGAFKN}");
            myLogSource.LogInfo($" {OANEPPJGNMN}");
            myLogSource.LogInfo($"my id {UserProfile.mine.ID}");
            

            foreach(MasterServerInfo ms in __instance.MasterServers)
            {
                myLogSource.LogInfo($" {ms.endpoint}, {ms.Host}, {ms.myPublicEndPoint}");
            }
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UserProfile), "DeserializeLoginResponse")]
        public static bool GetKlepClassFromLogin(UserProfile __instance, string ELNPDAFLKGC)
        {
            var myLogSource = new ManualLogSource("OGAT_MODDING_API");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);
            myLogSource.LogInfo($"string passed to get login klep is {ELNPDAFLKGC}");
            KLEPJCJNCIJ klepjcjncij = KLEPJCJNCIJ.FEMOFKDKPHN(ELNPDAFLKGC);
            Plugin.LogAllKeys(klepjcjncij, myLogSource);
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Game), "KJIJEELKAHL")]
        public static bool AlterMakeGameOrSomething(Game __instance)
        {
            if (!SGNet.DHIJOAJNGNH || __instance.isSinglePlayer)
            {
                return true;
            }
            Match i = Singleton<Match>.I;
            string text = string.Empty;
            try
            {
                __instance.needMasterServerRegistration = false;
                text = ((!OCAPFBIIEAI.DEINGFBMAFB) ? "H" : "D");
                text = text + "|" + __instance.myCountry;
                text = text + "|" + i.maxNumConnections.val;
                text = text + "|" + -1;
                text = text + "|" + __instance.PKELIPPMFJE.config.KHPDMLLJJFP;
                text = text + "|" + i.custom_map_info.Title;
                text += ((!(__instance.EBPLBIMDIJB == Singleton<InGame>.I)) ? "|L" : "|G");
                if (OCAPFBIIEAI.DEINGFBMAFB)
                {
                    text += "|Dedicated";
                }
                else
                {
                    text = text + "|" + UserProfile.mine.username;
                }
                if (!OCAPFBIIEAI.DEINGFBMAFB && UserProfile.mine != null)
                {
                    if (UserProfile.mine.inventory != null)
                    {
                        if (UserProfile.mine.inventory.DGAPKGEBECA("DEV"))
                        {
                            text += "|DEV";
                        }
                        else if (UserProfile.mine.inventory.DGAPKGEBECA("DLC_PGA"))
                        {
                            text += "|PGA";
                        }
                        else
                        {
                            text += "|";
                        }
                    }
                }
                else
                {
                    text += "|";
                }
               // text += "|";
               // text = text + "|" + UserProfile.mine.ID;
                text = text + UserProfile.mine.ID;

                
                
                // Add ZeroTier IP to the comment
               // string ztIp = Globals.ZtIp;  // Assume this is a method that gets the ZeroTier IP
               // if (!string.IsNullOrEmpty(ztIp))
              //  {
              //      text += "|ZT:" + ztIp;  // Add the ZT IP to the comment
              //  }

                var myLogSource = new ManualLogSource("OGAT_MODDING_API");
                BepInEx.Logging.Logger.Sources.Add(myLogSource);
                myLogSource.LogInfo($"TEXT FOR REGISTERHOST {text}");
                myLogSource.LogInfo($"STUFF FOR REGISTERHOST {text}");

                Netcode netcode = ObjectCache.Get<Netcode>(true);
                foreach(MasterServerInfo servers in netcode.MasterServers)
                {
                    myLogSource.LogInfo($"Mastersever is: {servers.endpoint}");
                }

                BepInEx.Logging.Logger.Sources.Remove(myLogSource);
                
                MasterServer.RegisterHost("OGAT_92.1", __instance.serverNameTxt.CLPBEAGICAJ, text);
                ObjectCache.Get<Netcode>(true).RegisterHost(UserProfile.mine.ID, UserProfile.mine.sessionToken, "OGAT_92.1", __instance.serverNameTxt.CLPBEAGICAJ, text, NetPlayer.All.Count, i.maxNumConnections.val, Network.incomingPassword);

                return false;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Match), "SerializeMatchInfo")]
        public static void SerializeModdedMatchInfo(Match __instance, ref KLEPJCJNCIJ __result)
        {
            var myLogSource = new ManualLogSource("OGAT_MODDING_API");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);
            myLogSource.LogInfo($"Serializing match info");


            (Singleton<Game>.I != null).Assert("game is null");
            KLEPJCJNCIJ klepjcjncij = KLEPJCJNCIJ.GOLPPFKNCEH;
            if (NetPlayer.Mine != null)
            {
                klepjcjncij = klepjcjncij.IAHHEEMDJBA(new KLEPJCJNCIJ[]
                {
                    KLEPJCJNCIJ.BPIFHLOBCLE("host"),
                    KLEPJCJNCIJ.BPIFHLOBCLE(UserProfile.mine.userID)
                });
            }
            klepjcjncij = klepjcjncij.IAHHEEMDJBA(new KLEPJCJNCIJ[]
            {
                KLEPJCJNCIJ.BPIFHLOBCLE("map"),
                KLEPJCJNCIJ.NMGCPIEEKHN.LIAOFNJDAID(new KLEPJCJNCIJ[]
                {
                    KLEPJCJNCIJ.BPIFHLOBCLE("map:{0}".fmt(new object[]
                    {
                        __instance.custom_map_info.MapId
                    })),
                    KLEPJCJNCIJ.BPIFHLOBCLE(__instance.custom_map_info.Title)
                })
            });
            klepjcjncij = klepjcjncij.IAHHEEMDJBA(new KLEPJCJNCIJ[]
            {
                KLEPJCJNCIJ.BPIFHLOBCLE("gm"),
                KLEPJCJNCIJ.BPIFHLOBCLE((double)__instance.GetGameModeIndex())
            });
            klepjcjncij = klepjcjncij.IAHHEEMDJBA(new KLEPJCJNCIJ[]
            {
                KLEPJCJNCIJ.BPIFHLOBCLE("nwb"),
                KLEPJCJNCIJ.BPIFHLOBCLE((double)__instance.nWin_blue.val)
            });
            klepjcjncij = klepjcjncij.IAHHEEMDJBA(new KLEPJCJNCIJ[]
            {
                KLEPJCJNCIJ.BPIFHLOBCLE("nwr"),
                KLEPJCJNCIJ.BPIFHLOBCLE((double)__instance.nWin_red.val)
            });
            klepjcjncij = klepjcjncij.IAHHEEMDJBA(new KLEPJCJNCIJ[]
            {
                KLEPJCJNCIJ.BPIFHLOBCLE("ff"),
                KLEPJCJNCIJ.BPIFHLOBCLE(__instance.opt_friendly_fire)
            });
            klepjcjncij = klepjcjncij.IAHHEEMDJBA(new KLEPJCJNCIJ[]
            {
                KLEPJCJNCIJ.BPIFHLOBCLE("mc"),
                KLEPJCJNCIJ.BPIFHLOBCLE((double)__instance.maxNumConnections.val)
            });
            klepjcjncij = klepjcjncij.IAHHEEMDJBA(new KLEPJCJNCIJ[]
            {
                KLEPJCJNCIJ.BPIFHLOBCLE("ats"),
                KLEPJCJNCIJ.BPIFHLOBCLE(__instance.opt_auto_team_swap)
            });

            if (Globals.IsModdedGameMode)
            {
                klepjcjncij = klepjcjncij.IAHHEEMDJBA(new KLEPJCJNCIJ[]
                {
                KLEPJCJNCIJ.BPIFHLOBCLE("saj"),
                KLEPJCJNCIJ.BPIFHLOBCLE(__instance.opt_spectator_auto_join)
                });

                __result = klepjcjncij.IAHHEEMDJBA(new KLEPJCJNCIJ[]
                {
                KLEPJCJNCIJ.BPIFHLOBCLE("mgm"),
                KLEPJCJNCIJ.BPIFHLOBCLE(Globals.CurrentModdedGameAbrv)
                });
            }
            else
            {
                klepjcjncij = klepjcjncij.IAHHEEMDJBA(new KLEPJCJNCIJ[]
                {
                KLEPJCJNCIJ.BPIFHLOBCLE("saj"),
                KLEPJCJNCIJ.BPIFHLOBCLE(__instance.opt_spectator_auto_join)
                });

                __result = klepjcjncij.IAHHEEMDJBA(new KLEPJCJNCIJ[]
                {
                KLEPJCJNCIJ.BPIFHLOBCLE("mgm"),
                KLEPJCJNCIJ.BPIFHLOBCLE("null")
                });

            }
            myLogSource.LogInfo($"match info {__result}");
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Match), "DeserializeMatchInfo")]
        public static void DeserializeModdedMatchInfo(Match __instance, KLEPJCJNCIJ CPLCOJJDBND)
        {
            var myLogSource = new ManualLogSource("OGAT_MODDING_API");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);
            myLogSource.LogInfo($"DeSerializing match info");
            try
            {
                
                string msg = (CPLCOJJDBND.HBNBAIPOKIO(new string[]
                    {
                        "mgm"
                    }).ToString());
                myLogSource.LogInfo($"DeSerializing mgm: {msg}");

                if((msg == "null" && !Globals.IsModdedGameMode))
                {
                    myLogSource.LogInfo($"No modded GameMode loading required");
                }
                else if((msg == "null") && (Globals.IsModdedGameMode))
                {
                    Singleton<Game>.I.gameModes = Globals.OriginalGameModes.ToList(); ;
                    Globals.CurrentModdedGameAbrv = "";
                    Globals.IsModdedGameMode = false;
                }
                else if (msg != "null" && Globals.IsModdedGameMode)
                {
                    //here check if the mod is the current mod or not if not then change it to current one
                    if(Globals.CurrentModdedGameAbrv == msg)
                    {
                        myLogSource.LogInfo($"Already Loaded modded GameMode ");
                    }
                    else
                    {
                        myLogSource.LogInfo($"Need to mod GameModes");
                        //add logic to set gamemode equal to the Object name of msg also add error checking incase a gamemode doesnt exist for the client
                        try         //first load in the CustomGameMode jst in case they havent already
                        {
                           //no setting original gamemodes here bc there is already a modded gamemode loaded
                            int IndexOfMGm = -1;
                            foreach (UnityAction<Game> gm in Globals.CustomGameModes)
                            {
                                gm(Singleton<Game>.I);
                            }
                            if (Singleton<Game>.I.gameModes.Count > 21)
                            {
                                myLogSource.LogInfo($"Called the right function thingy");


                                for (int i = 21; i <= (Singleton<Game>.I.gameModes.Count - 1); i++)
                                {
                                    bool isInListAlready = false;
                                    foreach (GameMode gmm in Globals.ModdedGameModes)
                                    {
                                        if (gmm.config.FDLIEBOMBAK == Singleton<Game>.I.gameModes[i].config.FDLIEBOMBAK)
                                        {
                                            isInListAlready = true;
                                        }
                                    }
                                    if (!isInListAlready) { Globals.ModdedGameModes.Add(Singleton<Game>.I.gameModes[i]); }

                                    myLogSource.LogInfo($"Called the right {i}");
                                    myLogSource.LogInfo($"Called the right {Globals.ModdedGameModes[i - 21].config.FDLIEBOMBAK}, {Singleton<Game>.I.gameModes[i].gameObject.name}");
                                    Singleton<Game>.I.gameModes.Remove(Singleton<Game>.I.gameModes[i]);
                                }
                                myLogSource.LogInfo($"finished {Singleton<Game>.I.gameModes.Count} normal gms, {Globals.ModdedGameModes.Count} modded gms");
                            }

                            //now replace non Modded Gm
                            for (int i = 0; i < Globals.ModdedGameModes.Count; i++)
                            {
                                myLogSource.LogInfo($"name of gm checked for name in order to replace {Globals.ModdedGameModes[i].gameObject.name}");
                                if (Globals.ModdedGameModes[i].gameObject.name == msg)
                                {
                                    int IndexOfMgm = i;
                                }
                            }

                            if (IndexOfMGm != -1)
                            {
                                if (Globals.ModdedGameModes[IndexOfMGm].config.KHPDMLLJJFP == Singleton<Game>.I.gameModes[__instance.GetGameModeIndex()].config.KHPDMLLJJFP)
                                {
                                    Singleton<Game>.I.gameModes[__instance.GetGameModeIndex()] = Globals.ModdedGameModes[IndexOfMGm];
                                }
                            }
                            else
                            {
                                myLogSource.LogInfo($"ERROR CUSTOM GAME MODE (WITH GAME OBJECT NAME) {msg} NOT FOUND");
                            }


                        }
                        catch (Exception e)
                        {
                            myLogSource.LogInfo($"{e}");
                        }
                    }
                }
                else if (msg == "null" && Globals.IsModdedGameMode)
                {
                    myLogSource.LogInfo($"No modded GameMode loading required but some dummy forgot to update the client");
                    // here i need to revert to original game mode list and set globals to false
                    Globals.OriginalGameModes.Clear();
                    //API_Methods.LoadOriginalGameModes();
                    Singleton<Game>.I.gameModes = Globals.OriginalGameModes.ToList();
                }
                else if (msg != "null" && !Globals.IsModdedGameMode)
                {
                    myLogSource.LogInfo($"Need to mod GameModes");
                    //add logic to set gamemode equal to the Object name of msg also add error checking incase a gamemode doesnt exist for the client
                    try         //first load in the CustomGameMode jst in case they havent already
                    {
                        

                        int IndexOfMGm = -1;
                        foreach (UnityAction<Game> gm in Globals.CustomGameModes)
                        {
                            gm(Singleton<Game>.I);
                        }
                        if (Singleton<Game>.I.gameModes.Count > 21)
                        {
                            myLogSource.LogInfo($"Called the right function thingy");


                            for (int i = 21; i <= (Singleton<Game>.I.gameModes.Count - 1); i++)
                            {
                                bool isInListAlready = false;
                                foreach (GameMode gmm in Globals.ModdedGameModes)
                                {
                                    if(gmm.config.FDLIEBOMBAK == Singleton<Game>.I.gameModes[i].config.FDLIEBOMBAK)
                                    {
                                        isInListAlready = true;
                                    }
                                }
                                if (!isInListAlready) { Globals.ModdedGameModes.Add(Singleton<Game>.I.gameModes[i]); }
                               
                                myLogSource.LogInfo($"Called the right {i}");
                                myLogSource.LogInfo($"Called the right {Globals.ModdedGameModes[i - 21].config.FDLIEBOMBAK}, {Singleton<Game>.I.gameModes[i].gameObject.name}");
                                Singleton<Game>.I.gameModes.Remove(Singleton<Game>.I.gameModes[i]);
                            }
                            myLogSource.LogInfo($"finished {Singleton<Game>.I.gameModes.Count} normal gms, {Globals.ModdedGameModes.Count} modded gms");
                        }

                        //now replace non Modded Gm
                        for(int i=0; i< Globals.ModdedGameModes.Count; i++)
                        {
                            myLogSource.LogInfo($"name of gm checked for name in order to replace {Globals.ModdedGameModes[i].gameObject.name}");
                            if (Globals.ModdedGameModes[i].gameObject.name == msg)
                            {
                                int IndexOfMgm = i;
                            }
                        }

                        if(IndexOfMGm != -1)
                        {
                            if (Globals.ModdedGameModes[IndexOfMGm].config.KHPDMLLJJFP == Singleton<Game>.I.gameModes[__instance.GetGameModeIndex()].config.KHPDMLLJJFP)
                            {
                                Singleton<Game>.I.gameModes[__instance.GetGameModeIndex()] = Globals.ModdedGameModes[IndexOfMGm];
                            }
                        }
                        else
                        {
                            myLogSource.LogInfo($"ERROR CUSTOM GAME MODE (WITH GAME OBJECT NAME) {msg} NOT FOUND");
                        }
                        
                       
                    }
                    catch (Exception e)
                    {
                        myLogSource.LogInfo($"{e}");
                    }
                }


            }
            catch (Exception e)
            {
                myLogSource.LogInfo($"DeSerializing error {e}");
            }
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SGNet), "S_assignStartingClass")]

        public static bool AssignModdedStartingClasses(SGNet __instance, NetPlayer KINJJOICFNG)
        {
            JIFNDMNAFHL.OCJPEJANCHI(" ****** Assegno la classe iniziale a {0}", new object[]
            {
                KINJJOICFNG.profile.username
            });
            string khpdmlljjfp = Singleton<Game>.I.PKELIPPMFJE.config.KHPDMLLJJFP;
            if (Globals.IsModdedGameMode && (khpdmlljjfp == Globals.CurrentModdedOrigAbrv))
            {
                if(KINJJOICFNG.GGOBEEOMBGG == IDJNNEJNMMO.Blue)
                {
                    PlayerClass[] classes = Globals.GetModdedBlueClassIds[Globals.CurrentModdedGameAbrv]();

                    for (int i = 0; i < classes.Length; i++)
                    {

                        KINJJOICFNG.SetClass(classes[i].GetClassId(), 0);
                        SGNet.BroadcastClassAndSkin(KINJJOICFNG);
                        break;
                    }
                }
                else
                {
                    PlayerClass[] classes2 = Globals.GetModdedRedClassIds[Globals.CurrentModdedGameAbrv]();

                    for (int j = 0; j < classes2.Length; j++)
                    {

                        KINJJOICFNG.SetClass(classes2[j].GetClassId(), 0);
                        SGNet.BroadcastClassAndSkin(KINJJOICFNG);
                        break;
                    }
                }
            }
            else
            {
                if (KINJJOICFNG.GGOBEEOMBGG == IDJNNEJNMMO.Blue)
                {
                    for (int i = 0; i < Singleton<Game>.I.prefab_blueClasses.Length; i++)
                    {
                        PlayerClass playerClass = Singleton<Game>.I.prefab_blueClasses[i];
                        if (!(playerClass == null))
                        {
                            if (playerClass.isValidForMode(khpdmlljjfp) && playerClass.isValidForAccount(KINJJOICFNG))
                            {
                                KINJJOICFNG.SetClass(playerClass.GetClassId(), 0);
                                SGNet.BroadcastClassAndSkin(KINJJOICFNG);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < Singleton<Game>.I.prefab_redClasses.Length; j++)
                    {
                        PlayerClass playerClass2 = Singleton<Game>.I.prefab_redClasses[j];
                        if (!(playerClass2 == null))
                        {
                            if (playerClass2.isValidForMode(khpdmlljjfp) && playerClass2.isValidForAccount(KINJJOICFNG))
                            {
                                KINJJOICFNG.SetClass(playerClass2.GetClassId(), 0);
                                SGNet.BroadcastClassAndSkin(KINJJOICFNG);
                                break;
                            }
                        }
                    }
                }
            }

            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ClassSelection), "GOMCLIKMEED")]
         public static void ModdedClassModesReturner(ref bool __result, PlayerClass OKABONPHCJI, GameMode DLAGIKNOLJC)
         {
            var myLogSource = new ManualLogSource("OGAT_MODDING_API");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);
            myLogSource.LogInfo($"running class mode checker");
            if (Globals.IsModdedGameMode && (DLAGIKNOLJC.config.KHPDMLLJJFP == Globals.CurrentModdedOrigAbrv)) 
            {
                bool foundit = false;
                myLogSource.LogInfo($"running modded class mode checker");
                foreach (string b in OKABONPHCJI.modes)
                {
                    myLogSource.LogInfo($"checking {b}, to currentModAbrv {Globals.CurrentModdedGameAbrv}");
                    if (Globals.CurrentModdedGameAbrv == b)
                    {
                        __result = true;
                        foundit = true;
                    }
                }
                if (!foundit) { __result = false; }
            }
            /*      since only a post fix no point changing the originalgms outputs
            else
            {
                bool foundit = false;
                foreach (string b in OKABONPHCJI.modes)
                {
                    if (DLAGIKNOLJC.config.KHPDMLLJJFP == b)
                    {
                        __result = true;
                        foundit = true;
                    }
                }
                if (!foundit) { __result = false; }
            }*/
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ClassSelection), "NEAFMMOJLAH")]

        public static bool ModdedClassSelection(ClassSelection __instance)
        {
            var myLogSource = new ManualLogSource("OGAT_MODDING_API");
            BepInEx.Logging.Logger.Sources.Add(myLogSource);
            myLogSource.LogInfo($"running class selection");
            try
            {
               
                if (Globals.IsModdedGameMode && (Singleton<Game>.I.gameModes[Singleton<Match>.I.GetGameModeIndex()].config.KHPDMLLJJFP == Globals.CurrentModdedOrigAbrv))
                {
                    myLogSource.LogInfo($"running modded class selection");
                    Game i = Singleton<Game>.I;
                    for (int j = 0; j < 8; j++)
                    {
                        SGButton sgbutton = __instance.ABMKMDCGFLB[j];
                        sgbutton.CENDBKLLBDG = false;
                        sgbutton.gameObject.SetActive(false);
                    }
                    PlayerClass[] array = (NetPlayer.Mine.GGOBEEOMBGG != IDJNNEJNMMO.Blue) ? Globals.GetModdedRedClassIds[Globals.CurrentModdedGameAbrv]() : Globals.GetModdedBlueClassIds[Globals.CurrentModdedGameAbrv]();
                    int num = Mathf.Max(8, array.Length);
                    int num2 = 0;
                    for (int k = 0; k < num; k++)
                    {
                        myLogSource.LogInfo($"running modded class loop");
                        if (k >= array.Length)
                        {
                            break;
                        }
                        PlayerClass playerClass = array[k];
                        if (!(playerClass == null))
                        {
                            if (!playerClass.required_inventory_items.Contains("DEV") || UserProfile.mine.inventory.DGAPKGEBECA("DEV") || 1 == 1)
                            {
                                bool flag = ClassSelection.GOMCLIKMEED(playerClass, i.PKELIPPMFJE);
                                myLogSource.LogInfo($"class flag is {flag}");
                                if (!flag) { flag=true; } //since im already using a manually made class list it doesnt need to check anyways
                                if (flag)
                                {
                                    if (__instance.restrictToClass != null && __instance.restrictToClass != playerClass)
                                    {
                                        flag = false;
                                    }
                                    if (__instance.hiddenClass != null && __instance.hiddenClass == playerClass)
                                    {
                                        flag = false;
                                    }
                                    if (flag)
                                    {
                                        SGButton sgbutton2 = __instance.ABMKMDCGFLB[num2];
                                        sgbutton2.gameObject.SetActive(true);
                                        sgbutton2.UserData = KLEPJCJNCIJ.BPIFHLOBCLE((double)playerClass.GetClassId());
                                        sgbutton2.transform.FindChildRecursive<Text>("_CLS_NAME_", true).text = playerClass.className;
                                        sgbutton2.transform.FindChildRecursive<Image>("CLS_ICON", true).sprite = playerClass.Icon;
                                        if (!playerClass.isValidForAccount(NetPlayer.Mine) && 1==2) //dont really need to give class requirements for modded games
                                        {
                                            sgbutton2.GetComponent<TooltipInfo>().tooltip = I18n.GetString("This class will unlock when you reach <color={0}>level {1}</color> or with a <color={0}>Permanent Gold Account</color>.").fmt(new object[]
                                            {
                                        "#d57801",
                                        playerClass.requiredMinLevel
                                            });
                                            sgbutton2.CENDBKLLBDG = false;
                                        }
                                        else
                                        {
                                            (playerClass.Icon != null).Assert("Missing Icon for class " + playerClass.className);
                                            sgbutton2.CENDBKLLBDG = true;
                                            sgbutton2.GetComponent<TooltipInfo>().tooltip = string.Empty;
                                            if (playerClass.GetClassId() == __instance.selectedClassId)
                                            {
                                                sgbutton2.Toggle();
                                            }
                                        }
                                        myLogSource.LogInfo($"class button loaded {sgbutton2.name}, ");
                                        num2++;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Game i = Singleton<Game>.I;
                    for (int j = 0; j < 8; j++)
                    {
                        SGButton sgbutton = __instance.ABMKMDCGFLB[j];
                        sgbutton.CENDBKLLBDG = false;
                        sgbutton.gameObject.SetActive(false);
                    }
                    PlayerClass[] array = (NetPlayer.Mine.GGOBEEOMBGG != IDJNNEJNMMO.Blue) ? i.prefab_redClasses : i.prefab_blueClasses;
                    int num = Mathf.Max(8, array.Length);
                    int num2 = 0;
                    for (int k = 0; k < num; k++)
                    {
                        if (k >= array.Length)
                        {
                            break;
                        }
                        PlayerClass playerClass = array[k];
                        if (!(playerClass == null))
                        {
                            if (!playerClass.required_inventory_items.Contains("DEV") || UserProfile.mine.inventory.DGAPKGEBECA("DEV"))
                            {
                                bool flag = ClassSelection.GOMCLIKMEED(playerClass, i.PKELIPPMFJE);
                                if (flag)
                                {
                                    if (__instance.restrictToClass != null && __instance.restrictToClass != playerClass)
                                    {
                                        flag = false;
                                    }
                                    if (__instance.hiddenClass != null && __instance.hiddenClass == playerClass)
                                    {
                                        flag = false;
                                    }
                                    if (flag)
                                    {
                                        SGButton sgbutton2 = __instance.ABMKMDCGFLB[num2];
                                        sgbutton2.gameObject.SetActive(true);
                                        sgbutton2.UserData = KLEPJCJNCIJ.BPIFHLOBCLE((double)playerClass.GetClassId());
                                        sgbutton2.transform.FindChildRecursive<Text>("_CLS_NAME_", true).text = playerClass.className;
                                        sgbutton2.transform.FindChildRecursive<Image>("CLS_ICON", true).sprite = playerClass.Icon;
                                        if (!playerClass.isValidForAccount(NetPlayer.Mine))
                                        {
                                            sgbutton2.GetComponent<TooltipInfo>().tooltip = I18n.GetString("This class will unlock when you reach <color={0}>level {1}</color> or with a <color={0}>Permanent Gold Account</color>.").fmt(new object[]
                                            {
                                        "#d57801",
                                        playerClass.requiredMinLevel
                                            });
                                            sgbutton2.CENDBKLLBDG = false;
                                        }
                                        else
                                        {
                                            (playerClass.Icon != null).Assert("Missing Icon for class " + playerClass.className);
                                            sgbutton2.CENDBKLLBDG = true;
                                            sgbutton2.GetComponent<TooltipInfo>().tooltip = string.Empty;
                                            if (playerClass.GetClassId() == __instance.selectedClassId)
                                            {
                                                sgbutton2.Toggle();
                                            }
                                        }
                                        num2++;
                                    }
                                }
                            }
                        }
                    }
                }
                BepInEx.Logging.Logger.Sources.Remove(myLogSource);
                return false;
            }
            catch (Exception e)
            {
                myLogSource.LogInfo(e);
                BepInEx.Logging.Logger.Sources.Remove(myLogSource);
                return false;
            }
            
        }
    }

}