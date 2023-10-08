using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;
using static Terraria.Player;

namespace SlugNPCsEX
{
	public class SlugNPCsEX : Mod
	{
	}

	public class SlugPetting : ModPlayer
	{
        public bool isPettingSlug = false;
        public override void PostUpdate()
        {
            if (Player.talkNPC == -1)
            {
                isPettingSlug = false;
            } else { 
            int num = Math.Sign(Main.npc[Player.talkNPC].Center.X - Player.Center.X);
            if (Player.controlLeft || Player.controlRight || Player.controlUp || Player.controlDown || Player.controlJump || Player.pulley || Player.mount.Active || num != Player.direction)
            {
                isPettingSlug = false;
            }
            
            }
            if (isPettingSlug)
            {
                int timer = Player.miscCounter % 14 / 7;
                CompositeArmStretchAmount stretch = CompositeArmStretchAmount.ThreeQuarters;
                if (timer == 1)
                {
                    stretch = CompositeArmStretchAmount.Full;
                }
                Player.SetCompositeArmBack(enabled: true, stretch, (float)Math.PI*-0.2f * (float)Player.direction);
            }
        }
        public void PetAnimal(int animalNpcIndex)
        {
            var npc = Main.npc[animalNpcIndex];

            var targetDirection = ((npc.Center.X > Player.Center.X) ? 1 : (-1));
            var playerPositionWhenPetting = npc.Bottom + new Vector2(-targetDirection * 25, 0);
            playerPositionWhenPetting = playerPositionWhenPetting.Floor();
            Vector2 offset = playerPositionWhenPetting - Player.Bottom;

            bool flag = Player.CanSnapToPosition(offset);
            if (flag && !WorldGen.SolidTileAllowBottomSlope((int)playerPositionWhenPetting.X / 16, (int)playerPositionWhenPetting.Y / 16))
            {
                flag = false;
            }
            if (!flag)
            {
                return;
            }
            if (isPettingSlug && Player.Bottom == playerPositionWhenPetting)
            {
                isPettingSlug = false;
                return;
            }
            Player.StopVanityActions();
            Player.RemoveAllGrapplingHooks();
            if (Player.mount.Active)
            {
                Player.mount.Dismount(Player);
            }
            Player.Bottom = playerPositionWhenPetting;
            Player.ChangeDir(targetDirection);
            isPettingSlug = true;
            Player.isTheAnimalBeingPetSmall = true;
            Player.velocity = Vector2.Zero;
            Player.gravDir = 1f;
            npc.direction = targetDirection;
            npc.spriteDirection = targetDirection;
            if (Player.whoAmI == Main.myPlayer)
            {
                AchievementsHelper.HandleSpecialEvent(Player, 21);
            }
        }
    }
}