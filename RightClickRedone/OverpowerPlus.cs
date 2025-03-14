﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;


namespace RightClickRedone.Cards {
    class OverpowerPlus : CustomCard {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            //UnityEngine.Debug.Log($"[{RCRedone.ModInitials}][Card] {GetTitle()} has been setup.");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats) {
            //Edits values on player when card is selected
            //UnityEngine.Debug.Log($"[{RCRedone.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats) {
            //Run when the card is removed from the player
            //UnityEngine.Debug.Log($"[{RCRedone.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");
        }
        protected override string GetTitle() {
            return "Overpower+";
        }
        protected override string GetDescription() {
            return "Deal damage around you that scales with the max HP of affected targets.";
        }
        protected override GameObject GetCardArt() {
            return null;
        }
        protected override CardInfo.Rarity GetRarity() {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats() {
            return new CardInfoStat[] {
                new CardInfoStat() {
                    positive = true,
                    stat = "Area Damage",
                    amount = "+50",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat() {
                    positive = true,
                    stat = "Scaled Area Damage",
                    amount = "+10%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat() {
                    positive = false,
                    stat = "Block Cooldown",
                    amount = "+0.25s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme() {
            return CardThemeColor.CardThemeColorType.ColdBlue;
        }
        public override string GetModName() {
            return RCRedone.ModInitials;
        }
    }
}