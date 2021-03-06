﻿using System;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Vintagestory.ModSamples
{
    /// <summary>
    /// Super basic example on how to set blocks in the game
    /// </summary>
    public class VillageGenerator : ModBase
    {
        ICoreServerAPI api;

        public override ModInfo GetModInfo()
        {
            return new ModInfo()
            {
                Author = "Tyron",
                Description = "A collection of sample mods",
                Name = "VS Sample Mods",
                Version = "1.0",
                Type = EnumModType.Content
            };
        }

        private void OnPlayerJoin(IServerPlayer byPlayer)
        {
            BlockPos plrpos = byPlayer.Entity.Pos.AsBlockPos;

            Block firebrickblock = api.World.GetBlock(new AssetLocation("claybricks-fire"));
            ushort blockId = firebrickblock.BlockId;
            api.World.BlockAccessor.SetBlock(blockId, plrpos.DownCopy());

            // Check a 3x3x3 area for logs
            int quantityLogs = 0;
            api.World.BlockAccessor.WalkBlocks(
                plrpos.AddCopy(-3, -3, -3),
                plrpos.AddCopy(3, 3, 3),
                (block, pos) => quantityLogs += block.Code.Path.Contains("log") ? 1 : 0
            );

            byPlayer.SendMessage(GlobalConstants.GeneralChatGroup, "You have " + quantityLogs + " logs nearby you", EnumChatType.Notification);
        }



        public override void StartServerSide(ICoreServerAPI api)
        {
            this.api = api;
            
            this.api.RegisterCommand("house", "Places a house (sample mod)", "", CmdGenHouse, Privilege.controlserver);
            this.api.RegisterCommand("block", "", "Places a block 2m in front of you (sample mod)", CmdBlock, Privilege.controlserver);
        }

        private void CmdBlock(IServerPlayer player, int groupId, CmdArgs args)
        {
            ushort blockID = api.WorldManager.GetBlockId(new AssetLocation("log-birch-ud"));
            BlockPos pos = player.Entity.Pos.HorizontalAheadCopy(2).AsBlockPos;
            api.World.BlockAccessor.SetBlock(blockID, pos);
        }

        private void CmdGenHouse(IServerPlayer player, int groupId, CmdArgs args)
        {
            IBlockAccessor blockAccessor = api.WorldManager.GetBlockAccessorBulkUpdate(true, true);
            ushort blockID = api.WorldManager.GetBlockId(new AssetLocation("log-birch-ud"));
            
            BlockPos pos = player.Entity.Pos.AsBlockPos;

            for (int dx = -3; dx <= 3; dx++)
            {
                for (int dz = -3; dz <= 3; dz++)
                {
                    for (int dy = 0; dy <= 3; dy++)
                    {
                        if (Math.Abs(dx) != 3 && Math.Abs(dz) != 3 && dy < 3) continue; // Hollow
                        if (dx == -3 && dz == 0 && dy < 2) continue; // Door

                        blockAccessor.SetBlock(blockID, pos.AddCopy(dx, dy, dz));
                    }
                }
            }

            blockAccessor.Commit();
        }
    }
}