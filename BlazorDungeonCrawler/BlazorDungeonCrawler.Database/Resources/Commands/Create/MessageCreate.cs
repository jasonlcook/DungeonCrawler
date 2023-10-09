﻿using System.Data.Entity;

using BlazorDungeonCrawler.Database.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Database.Resources.Commands.Create {
    public static class MessageCreate {
        public static void Create(Dungeon dungeon, List<Message> messages) {
            try {
                using (var context = new DungeonContext()) {
                    Dungeon? attachedDungeon = context.Dungeons.Where(d => d.Id == dungeon.Id).FirstOrDefault();
                    if (attachedDungeon != null && attachedDungeon.Messages != null) {
                        foreach (Message message in messages) {
                            attachedDungeon.Messages.Add(message);
                        }

                        foreach (Message message in attachedDungeon.Messages) {
                            context.Messages.Add(message);
                        }
                    }

                    context.SaveChanges();
                }
            } catch (Exception ex) {
                throw new Exception("Message create failed.");

            }
        }
    }
}
