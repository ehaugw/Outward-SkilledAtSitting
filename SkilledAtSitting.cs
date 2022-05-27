namespace SkilledAtSitting
{
    using System.Collections.Generic;
    using UnityEngine;
    using SideLoader;
    using HarmonyLib;
    using BepInEx;
    using InstanceIDs;
    using TinyHelper;

    [BepInPlugin(GUID, NAME, VERSION)]
    public class SkilledAtSitting : BaseUnityPlugin
    {
        public const string GUID = "com.ehaugw.skilledatsitting";
        public const string VERSION = "4.2.0";
        public const string NAME = "Skilled at Sitting";

        public Skill sitInstance;

        internal void Awake()
        {
            SL.OnPacksLoaded += OnPackLoaded;
            SL.OnSceneLoaded += OnSceneLoaded;

            //var harmony = new Harmony(GUID);
            //harmony.PatchAll();
        }

        private void OnSceneLoaded()
        {
            foreach (Character chr in  CharacterManager.Instance.Characters.Values)
            {
                if (!chr.IsAI && chr.Inventory != null && !chr.Inventory.LearnedSkill(sitInstance))
                {
                    chr.Inventory.ReceiveSkillReward(sitInstance.ItemID);
                } 
            }
        }
        private void OnPackLoaded()
        {
            MakeSittingPrefab();
            sitInstance = Sit();
        }

        private Skill Sit()
        {
            var myitem = new SL_Skill()
            {
                Name = "Sit",
                EffectBehaviour = EditBehaviours.Destroy,
                Target_ItemID = IDs.pushKickID,
                New_ItemID = IDs.sitID,
                SLPackName = "SkilledAtSitting",
                SubfolderName = "Sit",
                Description = "Sit down to regain health, burnt health and burnt stamina.",
                
                CastType = Character.SpellCastType.Sit,
                CastModifier = Character.SpellCastModifier.Immobilized,
                CastLocomotionEnabled = true,
                MobileCastMovementMult = -1f,

                EffectTransforms = new SL_EffectTransform[] {
                    new SL_EffectTransform() {
                        TransformName = "Effects",
                        Effects = new SL_Effect[] {
                            new SL_AddStatusEffect() {StatusEffect = SITTING_EFFECT_NAME, ChanceToContract=100, Delay = 2},
                        }
                    }
                },
                Cooldown = 1,
                StaminaCost = 0,
                HealthCost = 0,
                ManaCost = 0,
            };
            myitem.ApplyTemplate();
            Skill skill = ResourcesPrefabManager.Instance.GetItemPrefab(myitem.New_ItemID) as Skill;
            skill.IgnoreLearnNotification = true;
            return skill;
        }

        public static string SITTING_EFFECT_NAME = "Sitting";

        private void MakeSittingPrefab()
        {
            var statusEffect = TinyEffectManager.MakeStatusEffectPrefab(SITTING_EFFECT_NAME, SITTING_EFFECT_NAME, "Regain health, burnt health and stamina while sitting.", -1, Sitting.TICK_RATE, StatusEffectFamily.StackBehaviors.Override, "Bandage", false, modGUID: GUID);

            var effectSignature = statusEffect.StatusEffectSignature;
            var effectComponent = TinyGameObjectManager.MakeFreshObject("Effects", true, true, effectSignature.transform).AddComponent<Sitting>();
            effectComponent.UseOnce = false;
            effectSignature.Effects = new List<Effect>() { effectComponent };

            statusEffect.IsHidden = false;
            statusEffect.DisplayInHud = true;
        }
    }
}