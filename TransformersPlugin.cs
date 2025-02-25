using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using Digit.Client.UI;
using Digit.Networking.Core;
using Digit.Prime.Navigation;
using Digit.Prime.Ships;
using Digit.PrimeServer.Models;
using Digit.PrimeServer.Services;
using HarmonyLib;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Optimus.STFC.Transformers
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class TransformersPlugin : BasePlugin
    {
        #region[Declarations]
        public const string
            PROJECT = "STFC",
            MODNAME = "Transformers",
            AUTHOR = "Optimus",
            GUID = AUTHOR + "." + PROJECT + "." + MODNAME,
            VERSION = MyPluginInfo.PLUGIN_VERSION;
        #endregion

        internal static new ManualLogSource Log;

        private static ConfigEntry<bool> configInventoryPopUp;
        private static ConfigEntry<int> configInventoryPopUpScale;

        private static ConfigEntry<bool> configCombatReport;
        private static ConfigEntry<int> configCombatReportScale;

        private static ConfigEntry<bool> configShipManagement;
        private static ConfigEntry<int> configShipManagementScale;

        private static ConfigEntry<bool> configPopUp;
        private static ConfigEntry<int> configPopUpScale;

        private static ConfigEntry<bool> configAlwaysShowNames;

        private static ConfigEntry<bool> configVerbose;

        public override void Load()
        {
            TransformersPlugin.Log = base.Log;

            configInventoryPopUp = Config.Bind("General",
                "Extended speedups inventory popup menu",  // The key of the configuration option in the configuration file
                true, // The default value
                "Увеличенное окно выбора ускорений для ремонта и строительства");
            configInventoryPopUpScale = Config.Bind("General",
                "Scale for Extended speedups inventory popup menu",  // The key of the configuration option in the configuration file
                250, // The default value
                "Множитель увеличения окна выбора ускорений для ремонта и строительства");

            configCombatReport = Config.Bind("General",
                "Extended combat report window",
                true,
                "Увеличенное окно просмотра боевого отчета");
            configCombatReportScale = Config.Bind("General",
                "Scale for Extended combat report window",  // The key of the configuration option in the configuration file
                1000, // The default value
                "Множитель увеличения окна просмотра боевого отчета");

            configShipManagement = Config.Bind("General",
                "Extended ship management stats window",
                true,
                "Увеличенное окно статистики в разделе управления кораблем");
            configShipManagementScale = Config.Bind("General",
                "Scale for Extended ship management stats window",  // The key of the configuration option in the configuration file
                250, // The default value
                "Множитель увеличения окна статистики в разделе управления кораблем");

            configPopUp = Config.Bind("General",
                "Change pop-up object viewer scale (ships, systems, armadas)",
                true,
                "Изменить размер всплывающего окна при выделении объектов");
            configPopUpScale = Config.Bind("General",
                "Scale for pop-up object viewer (100 is normal value)",  // The key of the configuration option in the configuration file
                100, // The default value
                "Множитель изменения всплывающего окна при выделении объектов (100 - обычное значение)");

            configAlwaysShowNames = Config.Bind("General",
                "Always show names and tags of ships on any zoom",
                true,
                "Всегда показывать имена кораблей на любом оттдалении");

            configVerbose = Config.Bind("General",      // The section under which the option is shown
                "Verbose",  // The key of the configuration option in the configuration file
                false, // The default value
                "write more logs");

            Harmony.CreateAndPatchAll(typeof(TransformersPlugin));
            Log.LogInfo($"Plugin {GUID} is loaded!");
        }


        [HarmonyPatch(typeof(CanvasController), "Start")] //"Show", new Type[] { typeof(int), typeof(bool) })] //"Start")]
        [HarmonyPostfix]
        public static void CanvasController_Start(CanvasController __instance)
        {

            if (__instance != null)
            {
                //Extended speedups inventory popup menu
                if (__instance.name == "InventoryPopUp_Canvas" && configInventoryPopUp.Value)
                {
                    if (configVerbose.Value) Log.LogInfo($"CanvasController.Start({__instance.name})");

                    Transform popUpContainer = __instance.transform.Find("PopUpContainer");

                    if (configVerbose.Value) Log.LogInfo($"Change  PopUpContainer.RectTransform.anchorMax.Y  to  {popUpContainer.GetComponent<RectTransform>().anchorMax.y + ((float)configInventoryPopUpScale.Value / 1000)}");
                    popUpContainer.GetComponent<RectTransform>().anchorMax = new Vector2 { x = popUpContainer.GetComponent<RectTransform>().anchorMax.x, y = popUpContainer.GetComponent<RectTransform>().anchorMax.y + ((float)configInventoryPopUpScale.Value / 1000) }; // 0,5 0,5

                    if (configVerbose.Value) Log.LogInfo($"Change  PopUpContainer.RectTransform.anchorMin.Y  to  {popUpContainer.GetComponent<RectTransform>().anchorMin.y - ((float)configInventoryPopUpScale.Value / 1000)}");
                    popUpContainer.GetComponent<RectTransform>().anchorMin = new Vector2 { x = popUpContainer.GetComponent<RectTransform>().anchorMin.x, y = popUpContainer.GetComponent<RectTransform>().anchorMin.y - ((float)configInventoryPopUpScale.Value / 1000) }; // 0,5 0,5
                }
                //Extended combat report window
                if ((__instance.name == "CombatBattleReport_Canvas" && configCombatReport.Value)
                    || (__instance.name == "CombatScanScreen_Canvas" && configCombatReport.Value)
                    )
                {
                    try
                    {
                        Transform internalLeft = __instance.transform.Find("LeftSectionContainer/LeftInfo_Combat/Internal");
                        if (internalLeft == null)
                        {
                            if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t internalLeft is null");
                        }
                        if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t change FameSolid");
                        internalLeft.Find("FameSolid").GetComponent<RectTransform>().anchorMax = new Vector2 { x = 0, y = 0.5F + ((float)configCombatReportScale.Value / 1000) };        //new Vector2 { x = 0, y = 1.5F };   //  0 0,5 ---> 0 1,5
                        internalLeft.Find("FameSolid").GetComponent<RectTransform>().anchorMin = new Vector2 { x = 0, y = 0.5F - ((float)configCombatReportScale.Value / 1000) };        //  0 0,5 ---> 0 -0,5

                        if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t change FameGradient");
                        internalLeft.Find("FameGradient").GetComponent<RectTransform>().anchorMax = new Vector2 { x = 0, y = 0.5F + ((float)configCombatReportScale.Value / 1000) };     //  0 0,5 ---> 0 1,5
                        internalLeft.Find("FameGradient").GetComponent<RectTransform>().anchorMin = new Vector2 { x = 0, y = 0.5F - ((float)configCombatReportScale.Value / 1000) };     //  0 0,5 ---> 0 -0,5

                        if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t change ShipName");
                        internalLeft.Find("ShipName").GetComponent<RectTransform>().pivot = new Vector2 { x = 0, y = 0.5F + ((float)configCombatReportScale.Value / 100) };              // 0 0,5 ---> 0 10

                        if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t change HealthBar_Ships");
                        internalLeft.Find("HealthBar_Ships").GetComponent<RectTransform>().pivot = new Vector2 { x = 0, y = 0 + ((float)configCombatReportScale.Value / 100) };        // 0 0 ---> 0 9

                        if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t change OfficersObject");
                        internalLeft.Find("OfficersObject").GetComponent<RectTransform>().pivot = new Vector2 { x = 0, y = 1 - ((float)configCombatReportScale.Value / 100 / 2.222222F) };     // 0 1 ---> 0 -3,5

                        if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t change StatListContainer/ThDarkness");
                        internalLeft.Find("StatListContainer/ThDarkness").GetComponent<RectTransform>().anchorMax = new Vector2 { x = 1F, y = 0.5F + ((float)configCombatReportScale.Value / 1000) };   // 1 0,5 ---> 1 1,5
                        internalLeft.Find("StatListContainer/ThDarkness").GetComponent<RectTransform>().anchorMin = new Vector2 { x = 0, y = 0.5F - ((float)configCombatReportScale.Value / 1000) };   // 0 0,5 ---> 0 -0,5

                        if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t change StatListContainer/ScrollView");
                        internalLeft.Find("StatListContainer/ScrollView").GetComponent<RectTransform>().anchorMax = new Vector2 { x = 0, y = 0.5F + ((float)configCombatReportScale.Value / 1000) };    // 0 0,5 ---> 0 1,5
                        internalLeft.Find("StatListContainer/ScrollView").GetComponent<RectTransform>().anchorMin = new Vector2 { x = 0, y = 0.5F - ((float)configCombatReportScale.Value / 1000) };   // 0 0,5 ---> 0 -0,5

                        Transform internalRight = __instance.transform.Find("RightSectionContainer/RightSingleShipInfo_Combat/Internal");
                        if (internalRight == null)
                        {
                            if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t internalRight is null");

                        }
                        if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t change FrameSolid");
                        internalRight.Find("FrameSolid").GetComponent<RectTransform>().anchorMax = new Vector2 { x = 1, y = 0.5F + ((float)configCombatReportScale.Value / 1000) };
                        internalRight.Find("FrameSolid").GetComponent<RectTransform>().anchorMin = new Vector2 { x = 1, y = 0.5F - ((float)configCombatReportScale.Value / 1000) };
                        if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t change FrameGradient");
                        internalRight.Find("FrameGradient").GetComponent<RectTransform>().anchorMax = new Vector2 { x = 1, y = 0.5F + ((float)configCombatReportScale.Value / 1000) };
                        internalRight.Find("FrameGradient").GetComponent<RectTransform>().anchorMin = new Vector2 { x = 1, y = 0.5F - ((float)configCombatReportScale.Value / 1000) };
                        if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t change ShipName");
                        internalRight.Find("ShipName").GetComponent<RectTransform>().pivot = new Vector2 { x = 1, y = 0.5F + ((float)configCombatReportScale.Value / 100) };
                        if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t change HealthBar_Ships");
                        internalRight.Find("HealthBar_Ships").GetComponent<RectTransform>().pivot = new Vector2 { x = 1, y = 0 + ((float)configCombatReportScale.Value / 100) };
                        if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t change OfficerContainer");
                        internalRight.Find("OfficersObject").GetComponent<RectTransform>().pivot = new Vector2 { x = 1, y = 1 - ((float)configCombatReportScale.Value / 100 / 2.222222F) };
                        if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t change StatListContainer/ThDarkness");
                        internalRight.Find("StatListContainer/TheDarkness").GetComponent<RectTransform>().anchorMax = new Vector2 { x = 1, y = 0.5F + ((float)configCombatReportScale.Value / 1000) };
                        internalRight.Find("StatListContainer/TheDarkness").GetComponent<RectTransform>().anchorMin = new Vector2 { x = 0F, y = 0.5F - ((float)configCombatReportScale.Value / 1000) };
                        if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t change StatListContainer/ScrollView");
                        internalRight.Find("StatListContainer/ScrollView").GetComponent<RectTransform>().anchorMax = new Vector2 { x = 1, y = 0.5F + ((float)configCombatReportScale.Value / 1000) };
                        internalRight.Find("StatListContainer/ScrollView").GetComponent<RectTransform>().anchorMin = new Vector2 { x = 1, y = 0.5F - ((float)configCombatReportScale.Value / 1000) };
                        if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t All changes are done");
                    }
                    catch (Exception e)
                    {
                        if (configVerbose.Value) Log.LogInfo($"\t\t\t\t\t ERROR in transforming CombatBattleReport_Canvas: {e.Message}");
                    }


                }
                //Extended ship management stats window
                if (__instance.name == "ShipManagement_Canvas" && configShipManagement.Value)
                {
                    Transform detailsPanel = __instance.transform.Find("Content/DetailsPanel");
                    detailsPanel.GetComponent<RectTransform>().anchorMax = new Vector2 { x = 1, y = 0.5F + ((float)configShipManagementScale.Value / 1000) }; // 1 0,5 ---> 1 0,75
                    detailsPanel.GetComponent<RectTransform>().anchorMin = new Vector2 { x = 1, y = 0.5F - ((float)configShipManagementScale.Value / 1000) }; // 1 0,5 ---> 1 0,3
                }
                //Scale for pop-up object viewer on Start()
                if (__instance.name == "ObjectViewerTemplate_Canvas" && configPopUp.Value)
                {
                    if (configPopUpScale.Value < 1) configPopUpScale.Value = 1;
                    __instance.transform.localScale = new Vector3 { x = (float)configPopUpScale.Value / 100,
                        y = (float)configPopUpScale.Value / 100,
                        z = (float)configPopUpScale.Value / 100 };
                }
            }
        }

        [HarmonyPatch(typeof(CanvasController), "LateUpdate")]//"OnEnable")] //"Show", new Type[] { typeof(int), typeof(bool) })]
        [HarmonyPostfix]
        public static void CanvasController_LateUpdate(CanvasController __instance)
        {
            if (__instance != null)
            {
                //Scale for pop-up object viewer on LateUpdate()
                if (__instance.name == "ObjectViewerTemplate_Canvas" && configPopUp.Value)
                {
                    if (configVerbose.Value) Log.LogInfo($"localScale = [x:{__instance.transform.localScale.x},y:{__instance.transform.localScale.y},z:{__instance.transform.localScale.z}]");

                    if (configPopUpScale.Value < 1) configPopUpScale.Value = 1;
                    __instance.transform.localScale = new Vector3
                    {
                        x = (float)configPopUpScale.Value / 100,
                        y = (float)configPopUpScale.Value / 100,
                        z = (float)configPopUpScale.Value / 100
                    };
                    if (configVerbose.Value) Log.LogInfo($"localScale = [x:{__instance.transform.localScale.x},y:{__instance.transform.localScale.y},z:{__instance.transform.localScale.z}]");

                }
            }
        }

        [HarmonyPatch(typeof(NavigationFleetWidget), "OnZoomChanged", new Type[] { typeof(ZoomLevels) })]
        [HarmonyPrefix]
        public static void NavigationFleetWidget_OnZoomChanged(NavigationFleetWidget __instance)
        {
            // Show names and tags of ships on any zoom
            if (configAlwaysShowNames.Value)
            {
                if (__instance != null)
                {   
                    if (__instance.name == "FleetWidget(Clone)")
                    {
                        Transform TagContainer = __instance.gameObject.transform.Find("InnerParent/NearLODGroup/BG/TagContainer");
                        if (configVerbose.Value) Log.LogInfo($"TagContainer.gameObject.activeSelf from {TagContainer.gameObject.activeSelf}");
                        TagContainer.gameObject.SetActive(true);
                        if (configVerbose.Value) Log.LogInfo($"TagContainer.gameObject.activeSelf to   {TagContainer.gameObject.activeSelf}");


                        Transform PlayerName = __instance.gameObject.transform.Find("InnerParent/NearLODGroup/BG/PlayerName");
                        if (configVerbose.Value) Log.LogInfo($"PlayerName.gameObject.activeSelf from   {PlayerName.gameObject.activeSelf}");
                        PlayerName.gameObject.SetActive(true);
                        if (configVerbose.Value) Log.LogInfo($"PlayerName.gameObject.activeSelf to     {PlayerName.gameObject.activeSelf}");

                        var fleetWidgets = __instance.transform.parent.GetComponentsInChildren<NavigationZoomEventHandler>(true);
                        if (configVerbose.Value) Log.LogInfo($"fleetWidgets.Count = {fleetWidgets.Count}");
                    }
                }
            }
        }
    }
}
