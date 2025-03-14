﻿namespace SkipIntro2.Patch

open System.Reflection
open BaboonAPI.Hooks.Initializer
open HarmonyLib
open UnityEngine.SceneManagement

[<HarmonyPatch(typeof<BrandingController>, "doHolyWowAnim")>]
type BrandingControllerPatch() =
    static member Prefix(__instance: BrandingController) =
        // Only skip if BaboonAPI successfully initialized.
        if BaboonInitializer.IsInitialized() then
            __instance.CancelInvoke()
            __instance.Invoke("killandload", 0f)
            false
        else
            true

[<HarmonyPatch(typeof<SaveSlotController>, "Start")>]
type SaveSlotControllerPatch() =
    static member val internal saveIndex = 0
        with get, set

    static member Postfix(__instance: SaveSlotController) =
        SaverLoader.global_saveindex <- SaveSlotControllerPatch.saveIndex
        SaverLoader.loadSavedGame ()

        let mi =
            __instance
                .GetType()
                .GetMethod("checkScores", BindingFlags.NonPublic ||| BindingFlags.Instance)

        mi.Invoke(__instance, null) |> ignore
        AchievementSetter.checkAllCheevos ()
        SaverLoader.loadAllSaveHighScores ()
        SceneManager.LoadScene("home")
        ()
