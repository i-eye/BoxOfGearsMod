using BepInEx;
using R2API;
using RoR2;
using RoR2.Items;
using UnityEngine;
using UnityEngine.AddressableAssets;


[assembly: HG.Reflection.SearchableAttribute.OptIn]
namespace IEye.BoxOfGears
{
    [BepInDependency(ItemAPI.PluginGUID)]

    [BepInDependency(LanguageAPI.PluginGUID)] // NO LANGUAGE API IT'S FOR FILTHY HOPOO ENJOYERS

    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    public class BoxOfGearsPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "IEye.BoxOfGears";
        public const string PluginName = "Box_Of_Gears";
        public const string PluginVersion = "1.0.0";

        public void Awake()
        {
            Log.Init(Logger);
            BoxOfGearsItem.Init();
        }
        /*
        private void Update()
        {
            // This if statement checks if the player has currently pressed F2.
            if (Input.GetKeyDown(KeyCode.F2))
            {
                // Get the player body to use a position:
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                // And then drop our defined item in front of the player.

                Log.Info($"Player pressed F2. Spawning our custom item at coordinates {transform.position}");
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(BoxOfGearsItem.boxItemDef.itemIndex), transform.position, transform.forward * 20f);
            }
        }
        */
    }


    public class BoxOfGearsItem()
    {

        public static ItemDef boxItemDef;
        public static void Init()
        {
            boxItemDef = ScriptableObject.CreateInstance<ItemDef>();

            boxItemDef.name = "IEYE_GEARBOX_NAME";
            boxItemDef.nameToken = "IEYE_GEARBOX_NAME";
            boxItemDef.pickupToken = "IEYE_GEARBOX_PICKUP";
            boxItemDef.descriptionToken = "IEYE_GEARBOX_DESC";
            boxItemDef.loreToken = "IEYE_GEARBOX_LORE";

            boxItemDef._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/Tier1Def.asset").WaitForCompletion();

            boxItemDef.pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            boxItemDef.pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();

            //heeeeheeeee
            boxItemDef.canRemove = false;

            boxItemDef.hidden = false;

            //aint no way I do this
            var displayRules = new ItemDisplayRuleDict(null);

            ItemAPI.Add(new CustomItem(boxItemDef, displayRules));
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }


        private static void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig.Invoke(self);
            int count = self.inventory.GetItemCount(boxItemDef);
            double fps = 1.0 / Time.deltaTime;
            if (count > 0)
            {
                self.moveSpeed *= (float)(fps / 30.0) * count;
                self.attackSpeed *= (float)(fps / 30.0) * count;
                self.damage *= (float)(fps / 30.0) * count;
                self.maxJumpCount *= (int)(fps / 30.0) * count;
                //self.maxHealth *= (float)(fps / 30.0) * count;
                self.regen *= (float)(fps / 30.0) * count;
                self.armor *= (float)(fps / 30.0) * count;
                self.acceleration = ((self.baseMoveSpeed == 0f) ? 0f : (self.moveSpeed / self.baseMoveSpeed * self.baseAcceleration));
            }

        }


        class BoxItemBehavior: BaseItemBodyBehavior 
        {
            //this code is the worst shit I have ever written like wtf
            [ItemDefAssociation]
            private static ItemDef GetItemDef() => boxItemDef;

            private void Update()
            {
                body.RecalculateStats();
            }
        }
    }
}