using System.Linq; // idk
using UnityEngine; // Unity stuff
using MelonLoader; // Modloader
using Il2Cpp; // To modify general stuff.
using Il2CppMonomiPark.SlimeRancher;
using Il2CppTMPro; // To set text.
using Il2CppMonomiPark.SlimeRancher.Script.UI.Pause; // For F10 Debug key.
using UnityEngine.InputSystem; // For debug input.
using Il2CppMonomiPark.SlimeRancher.UI;
using HarmonyLib; // Harmony Patches stuff.
using Il2CppKinematicCharacterController; // Save bricking

namespace HardMode
{
    [HarmonyPatch(typeof(AttackPlayer), nameof(AttackPlayer.Awake))] // Harmony patch for AttackPlayer.
    public class LargoAttackPatch
    {
        public static void Postfix(AttackPlayer __instance) // Runs after the Awake
        {
            __instance.shouldAttackPlayer = true; // Enables attacking
        }
    }

    public class HardModeEntry : MelonMod
    {
        public override void OnUpdate()
        {
            if (Keyboard.current.f10Key.wasPressedThisFrame) // On F10 key pressed.
            {
                if (Get<GameObject>("SceneContext") != null) // Check for scene context object.
                {
                    Get<PauseMenuDirector>("SceneContext").Quit(); // Quits to MainMenu scene.

                    var deathUI = Get<GameObject>("DeathScreenUI"); // Gets death ui.
                    UnityEngine.Object.Destroy(deathUI, 0); // Deletes death ui.

                }

            }
            if (Keyboard.current.f11Key.wasPressedThisFrame) // On F11 key pressed.
            {
                if (Get<GameObject>("SceneContext") != null) // Check for scene context object.
                {
                    Get<TimeDirector>("SceneContext").FastForwardTo(23403.1062); // Fast Forward time to 6:30 (Day 1)
                }

            }
        }
        public static T Get<T>(string name) where T : UnityEngine.Object // Code to grab any object or component (By KomiksPL)
        {
            return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((T x) => x.name == name);
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "PlayerCore") // On player load
            {
                Get<EconomyDirector>("SceneContext").dailyShutdownMins = 420; // Sets Market Times.
                var pdh = Get<PlayerDeathHandler>("PlayerControllerKCC"); // Gets the death stuff.
                pdh.delayTime = float.MaxValue; // Makes death screen forever.
                pdh.fadeTime = (float)2.1; // Slows fade time on death.
                pdh.ranchHouseUIPrefab = null; // Disables death from ending properly.
                Get<AutoSaveDirector>("GameContext").nextSaveTime = float.MaxValue; ;// Disables normal autosaving.
                Get<GameSettingsDirector>("GameContext").damageMultiplier.Value = (float)2.5; // 2.5x Damage
            }
            else if (sceneName == "DeathLoadScene") // On death.
            {
                Get<GameObject>("DeathScreenUI").active = false; // Removes death ui.

                // Save bricker
                var player = Get<GameObject>("PlayerControllerKCC"); // Gets the player.
                player.GetComponent<KinematicCharacterMotor>().enabled = false; // Disables player motor.
                player.transform.position = new Vector3(99999, 1, 99999); // Resets player position.
                Application.Quit(); // Closes the game.
            }
            else if (sceneName == "zoneFields")
            {
                // Sets the "Market Offline" text.
                Get<GameObject>("ShutdownPanel").transform.GetChild(0).GetComponent<TextMeshProUGUI>().m_text = "Market Offline.";
                Get<GameObject>("ShutdownPanel").active = false;

                Get<GameObject>("ranchHouse").transform.GetChild(2).gameObject.active = false; // Disables ranch house.
            }
        }

    }
}
