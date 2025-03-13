namespace SkipIntro2

open BaboonAPI.Hooks.Initializer
open BepInEx
open BepInEx.Configuration
open HarmonyLib
open SkipIntro2.Patch

[<BepInPlugin("SkipIntro", "SkipIntro", "2.0.1")>]
[<BepInDependency("ch.offbeatwit.baboonapi.plugin", "2.8.0")>]
type SkipIntroPlugin() =
    inherit BaseUnityPlugin()

    let harmony = Harmony("ch.offbeatwit.skipintro.harmony")

    member this.Awake() =
        GameInitializationEvent.EVENT.Register this

        try
            harmony.PatchAll typeof<BrandingControllerPatch>
        with exc ->
            this.Logger.LogError "Could not patch BrandingController, oh no"
            this.Logger.LogError exc

        let saveSlotConfig =
            this.Config.Bind(
                "Default",
                "SaveSlot",
                0,
                ConfigDescription("Save slot to load", AcceptableValueRange(0, 2))
            )

        SaveSlotControllerPatch.saveIndex <- saveSlotConfig.Value

    member this.TryInitialize() =
        harmony.PatchAll typeof<SaveSlotControllerPatch>

    interface GameInitializationEvent.Listener with
        member this.Initialize() =
            GameInitializationEvent.attempt this.Info this.TryInitialize
