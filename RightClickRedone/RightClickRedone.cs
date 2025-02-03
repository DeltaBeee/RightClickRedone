using BepInEx;
using BepInEx.Configuration;
using UnboundLib;
using UnboundLib.Utils.UI;
using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Simple;


namespace RightClickRedone {
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class RCRedone : BaseUnityPlugin {

        private const string ModId = "com.delta.rounds.RCRedone";
        private const string ModName = "Right Click Redone";
        public const string Version = "0.1.0";
        public const string ModInitials = "RCR";
        public static RCRedone instance {get;private set;}

        public static ConfigEntry<bool> SWORD;
        public static ConfigEntry<bool> SHIELD;

        void Awake() {
            // Use this to call any harmony patch files your mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();

            NetworkingManager.RegisterEvent("com.Delta.Rounds.Run_Reworks", delegate (object[] e) {
                SWORD.Value = (bool)e[0];
                SHIELD.Value = (bool)e[1];
            });
        }

        void Start() {

            Unbound.RegisterMenu("Delta's Reworks", () => { }, DrawOtherUI, null);
            Unbound.RegisterHandshake("com.willis.rounds.unbound", OnHandShakeCompleted);

            instance = this;

            SWORD = Config.Bind("Stats", "Gun Cards", true, "Rebalances stats gained from certain gun cards.");
            SHIELD = Config.Bind("Stats", "Block Cards", true, "Rebalances stats gained and reworks certain effects from many block cards.");

            //CustomCard.BuildCard<MyCard>();

            Rebalance();

            static void Rebalance() { 
                
                foreach (var card in CardChoice.instance.cards) {
                    switch (card.cardName.ToUpper()) {
                        case "POISON": {
                                if (SWORD.Value) {

                                    // Nerfs reload speed from 30% to 20%

                                    card.cardStats.ToList()[1].stat = "Reload time";
                                    card.cardStats.ToList()[1].amount = "-20%";

                                    card.GetComponent<Gun>().reloadTime *= 0.8f;

                                } else {

                                    card.cardStats.ToList()[1].stat = "Reload time";
                                    card.cardStats.ToList()[1].amount = "-30%";

                                    card.GetComponent<Gun>().reloadTime *= 0.7f;
                                }
                                break;
                            }

                        case "QUICK RELOAD": {
                                if (SWORD.Value) {

                                    // Nerfs reload speed from 70% to 50%

                                    card.cardStats.ToList()[0].amount = "-50%";
                                    card.GetComponent<Gun>().reloadTime *= 0.5f;

                                } else {

                                    card.cardStats.ToList()[0].amount = "-70%";
                                    card.GetComponent<Gun>().reloadTime *= 0.3f;
                                }
                                break;
                            }

                        case "GROW": {
                                if (SWORD.Value) {

                                    // Reduces attack speed by 20%, increases reload time from 0.25s to 0.5s.

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Attack Speed", amount = "-20%", positive = false, simepleAmount = CardInfoStat.SimpleAmount.slightlyLower}, 
                                        new CardInfoStat { stat = "Reload time", amount = "+0.5s", positive = false }
                                    };

                                    card.GetComponent<Gun>().attackSpeed *= 0.8f;
                                    card.GetComponent<Gun>().reloadTimeAdd = 0.5f;

                                } else {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Reload time", amount = "+0.25s", positive = false }
                                    };

                                    card.GetComponent<Gun>().attackSpeed *= 1.0f;
                                    card.GetComponent<Gun>().reloadTimeAdd = 0.25f;
                                }
                                break;
                            }

                        case "DRILL AMMO": {
                                if (SWORD.Value) {

                                    // Increases reload time from 0.25s to 0.5s, adds 30% block penetration.

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Bullets drill through walls", amount = "+7m", positive = true },
                                        new CardInfoStat { stat = "Block penetration", amount = "+30%", positive = true },
                                        new CardInfoStat { stat = "Reload time", amount = "+0.5s", positive = false }
                                    };

                                    // add block penetration here
                                    card.GetComponent<Gun>().reloadTimeAdd = 0.5f;

                                } else {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Bullets drill through walls", amount = "+7m", positive = true },
                                        new CardInfoStat { stat = "Reload time", amount = "+0.25s", positive = false }
                                    };

                                    // make sure to reset block penetration here
                                    card.GetComponent<Gun>().reloadTimeAdd = 0.25f;
                                }
                                break;
                            }


                        // For the following: removes health gained, increases cooldown from 0.25s to 0.5s.

                        case "EMP":
                        case "HEALING FIELD":
                        case "RADAR SHOT":
                        case "SAW":
                        case "FROST SLAM": {
                                if (SHIELD.Value) {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Block cooldown", amount = "+0.5s", positive = false }
                                    };

                                    card.GetComponent<CharacterStatModifiers>().health *= 1.0f;
                                    card.GetComponent<Block>().cooldown += 0.5f;

                                } else {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Health", amount = "+30%", positive = true, simepleAmount = CardInfoStat.SimpleAmount.Some},
                                        new CardInfoStat { stat = "Block cooldown", amount = "+0.25s", positive = false }
                                    };

                                    card.GetComponent<CharacterStatModifiers>().health *= 1.3f;
                                    card.GetComponent<Block>().cooldown += 0.25f;
                                }
                                break;
                            }

                        // For the following: reduces health gained from 50% to 15%, increases cooldown from 0.25s to 0.5s (supernova defaults to 0.5s).

                        case "IMPLODE":
                        case "SHOCKWAVE": {
                                if (SHIELD.Value) {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Health", amount = "+15%", positive = true, simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf},
                                        new CardInfoStat { stat = "Block cooldown", amount = "+0.5s", positive = false }
                                    };

                                    card.GetComponent<CharacterStatModifiers>().health *= 1.15f;
                                    card.GetComponent<Block>().cooldown += 0.5f;

                                } else {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Health", amount = "+50%", positive = true, simepleAmount = CardInfoStat.SimpleAmount.Some},
                                        new CardInfoStat { stat = "Block cooldown", amount = "+0.25s", positive = false }
                                    };

                                    card.GetComponent<CharacterStatModifiers>().health *= 1.5f;
                                    card.GetComponent<Block>().cooldown += 0.25f;
                                }
                                break;
                            }

                        case "SUPERNOVA": {
                                if (SHIELD.Value) {

                                    card.cardStats.ToList()[0].amount = "+15%";
                                    card.GetComponent<CharacterStatModifiers>().health *= 1.15f;

                                } else {

                                    card.cardStats.ToList()[0].amount = "+50%";
                                    card.GetComponent<CharacterStatModifiers>().health *= 1.5f;
                                }
                                break;
                            }

                        case "HUGE": {
                                if (SHIELD.Value) {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Health", amount = "+50%", positive = true },
                                        new CardInfoStat { stat = "Size modifier", amount = "+15%", positive = false },
                                        new CardInfoStat { stat = "Movement speed", amount = "-5%", positive = false }
                                    };

                                    card.GetComponent<CharacterStatModifiers>().health *= 1.5f;
                                    card.GetComponent<CharacterStatModifiers>().sizeMultiplier += 0.15f;
                                    card.GetComponent<CharacterStatModifiers>().movementSpeed *= 0.95f;

                                } else {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Health", amount = "+80%", positive = true, simepleAmount = CardInfoStat.SimpleAmount.aHugeAmountOf}
                                    };

                                    card.GetComponent<CharacterStatModifiers>().health *= 1.8f;
                                    card.GetComponent<CharacterStatModifiers>().sizeMultiplier += 0.0f;
                                    card.GetComponent<CharacterStatModifiers>().movementSpeed *= 1.0f;
                                }
                                break;
                            }

                        case "TANK": {
                                if (SHIELD.Value) {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Health", amount = "+100%", positive = true },
                                        new CardInfoStat { stat = "Size modifier", amount = "+25%", positive = false },
                                        new CardInfoStat { stat = "Movement speed", amount = "-10%", positive = false }
                                    };

                                    card.GetComponent<CharacterStatModifiers>().health *= 2.0f;
                                    card.GetComponent<CharacterStatModifiers>().sizeMultiplier += 0.25f;
                                    card.GetComponent<CharacterStatModifiers>().movementSpeed *= 0.9f;

                                    // reset attkspd and reload time debuff bc we all know gun builds didnt really use tank as much as block builds, and these dont debuff them

                                    card.GetComponent<Gun>().attackSpeed *= 1.0f;
                                    card.GetComponent<Gun>().reloadTimeAdd = 0.0f;

                                } else {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Health", amount = "+100%", positive = true, simepleAmount = CardInfoStat.SimpleAmount.aHugeAmountOf},
                                        new CardInfoStat { stat = "ATKSPD", amount = "-25%", positive = false },
                                        new CardInfoStat { stat = "Reload time", amount = "+0.5s", positive = false }
                                    };

                                    card.GetComponent<CharacterStatModifiers>().health *= 2.0f;
                                    card.GetComponent<Gun>().attackSpeed *= 0.75f;
                                    card.GetComponent<Gun>().reloadTimeAdd = 0.5f;
                                }
                                break;
                            }

                        case "TELEPORT": {
                                if (SHIELD.Value) {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Block cooldown", amount = "+0.5s", positive = false },
                                        new CardInfoStat { stat = "Block effectiveness", amount = "-10%", positive = false }
                                    };

                                    card.GetComponent<Block>().cooldown += 0.5f;
                                    // insert code for block effectiveness here

                                } else {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Block cooldown", amount = "-30%", positive = true, simepleAmount = CardInfoStat.SimpleAmount.lower}
                                    };

                                    card.GetComponent<Block>().cooldown *= 0.7f;
                                    // make sure to RESET BLOCK EFFECTIVENESS HERE

                                }
                                break;
                            }

                        case "ECHO": {
                                if (SHIELD.Value) {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Health", amount = "+30%", positive = true },
                                        new CardInfoStat { stat = "Block cooldown", amount = "+0.5s", positive = false },
                                        new CardInfoStat { stat = "Block effectiveness", amount = "-5%", positive = false }
                                    };

                                    card.GetComponent<CharacterStatModifiers>().health *= 1.3f;
                                    card.GetComponent<Block>().cooldown += 0.5f;
                                    // insert code for block effectiveness here

                                } else {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Health", amount = "+30%", positive = true },
                                        new CardInfoStat { stat = "Block cooldown", amount = "+0.25s", positive = false }
                                    };


                                    card.GetComponent<Block>().cooldown += 0.25f;
                                    // make sure to RESET BLOCK EFFECTIVENESS HERE

                                }
                                break;
                            }

                        case "SHIELD CHARGE": {
                                if (SHIELD.Value) {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Block cooldown", amount = "+0.5s", positive = false },
                                        new CardInfoStat { stat = "Block effectiveness", amount = "-10%", positive = false }
                                    };

                                    card.GetComponent<Block>().cooldown += 0.5f;
                                    // insert code for block effectiveness here... maybe remove ending block?

                                } else {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Block cooldown", amount = "+0.25s", positive = false }
                                    };

                                    card.GetComponent<Block>().cooldown += 0.25f;
                                    // make sure to RESET BLOCK EFFECTIVENESS HERE

                                }
                                break;
                            }


                        /*
                            TODO: BRAWLER AND PRISTINE REWORK
                        */

                        case "SILENCE": {
                                if (SHIELD.Value) {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Block cooldown", amount = "+0.5s", positive = false },
                                        new CardInfoStat { stat = "Block effectiveness", amount = "-15%", positive = false }
                                    };

                                    card.GetComponent<CharacterStatModifiers>().health *= 1.0f;
                                    card.GetComponent<Block>().cooldown += 0.5f;
                                    // TODO: Block effectivness & Effect halved on already silenced targets

                                } else {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Health", amount = "+25%", positive = true, simepleAmount = CardInfoStat.SimpleAmount.Some},
                                        new CardInfoStat { stat = "Block cooldown", amount = "+0.25s", positive = false }
                                    };

                                    card.GetComponent<CharacterStatModifiers>().health *= 1.25f;
                                    card.GetComponent<Block>().cooldown += 0.25f;
                                    // Make sure to RESET BLOCK EFFECTIVENESS HERE
                                }
                                break;
                            }

                        case "OVERPOWER": {
                                if (SHIELD.Value) {

                                    // Removes health gained, and increased block cooldown from 0.25s to 0.5s, while reworking the effect's damage calculation.
                                    // Scales well with having lower health and damage stats. Chunk those tanks like the gremlin you are.
                                    // Damage scales positively with enemy's CURRENT health, negatively with user's MAX health and bullet damage.

                                    // Having zero, or negative health will be treated as having 1 health.

                                    card.cardDestription = "Deal damage around you that scales positively with target's health, negatively with your own";

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Block cooldown", amount = "+0.5s", positive = false }
                                    };

                                    card.GetComponent<CharacterStatModifiers>().health *= 1.0f;
                                    card.GetComponent<Block>().cooldown += 0.5f;

                                    var explosion = card.gameObject.GetComponent<CharacterStatModifiers>()
                                        .AddObjectToPlayer.GetComponent<SpawnObjects>().objectToSpawn[0].GetComponent<Reworked_Overpower_Explosion>();
                                    

                                } else {

                                    card.cardStats = new CardInfoStat[] {
                                        new CardInfoStat { stat = "Health", amount = "+30%", positive = true, simepleAmount = CardInfoStat.SimpleAmount.Some},
                                        new CardInfoStat { stat = "Block cooldown", amount = "+0.25s", positive = false }
                                    };

                                    card.GetComponent<CharacterStatModifiers>().health *= 1.3f;
                                    card.GetComponent<Block>().cooldown += 0.25f;

                                    //var explosion = card.gameObject.GetComponent<CharacterStatModifiers>().AddObjectToPlayer.GetComponent<SpawnObjects>().objectToSpawn[0].GetComponent<Explosion_Overpower>();
                                    //explosion.GetComponent<Explosion_Overpower>().dmgPer100Hp = 15;
                                }
                                break;
                            }
                    }
                }
            }
            void DrawOtherUI(GameObject obj) {

                void RaiseEvent(bool sword, bool shield) {

                    SWORD.Value = sword;
                    SHIELD.Value = shield;

                    NetworkingManager.RaiseEvent("com.Delta.Rounds.Run_Reworks", new object[] {
                    sword,
                    shield
                    });
                }
                MenuHandler.CreateText("Delta's reworking options", obj, out _, 70);
                MenuHandler.CreateToggle(SWORD.Value, "Sword", obj, flag => { RaiseEvent(flag, SHIELD.Value); });
                MenuHandler.CreateToggle(SHIELD.Value, "Shield", obj, flag => { RaiseEvent(SWORD.Value, flag); });
            }

             static void OnHandShakeCompleted() {
                if (PhotonNetwork.IsMasterClient) {
                    NetworkingManager.RaiseEvent("com.Delta.Rounds.Run_Reworks", new object[] {
                    SWORD.Value,
                    SHIELD.Value
                    });
                }
            }
        }
    }
}
