﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;

namespace Eco.Plugins.DiscordLink
{
    public static class DSharpExtensions
    {
        public static ulong? TryParseSnowflakeId(string nameOrId)
        {
            ulong id;
            return ulong.TryParse(nameOrId, out id) && id > 0xFFFFFFFFFFFFFUL ? new ulong?(id) : null;
        }

        #region DiscordGuild

        public static string[] TextChannelNames(this DiscordGuild guild)
        {
            return guild != null ? guild.TextChannels().Select(channel => channel.Name).ToArray() : new string[0];
        }
        
        public static IReadOnlyList<DiscordChannel> TextChannels(this DiscordGuild guild)
        {
            return guild != null
                ? guild.Channels.Where(channel => channel.Type == ChannelType.Text).ToList()
                : new List<DiscordChannel>();
        }
        
        public static DiscordChannel ChannelByName(this DiscordGuild guild, string channelName)
        {
            return guild != null
                ? guild.TextChannels().FirstOrDefault(channel => channel.Name == channelName)
                : null;
        }

        public static DiscordChannel ChannelByNameOrId(this DiscordGuild guild, string channelNameOrId)
        {
            if (guild == null) { return null; }

            var maybeChannelId = TryParseSnowflakeId(channelNameOrId);
            return maybeChannelId != null ? guild.GetChannel(maybeChannelId.Value) : guild.ChannelByName(channelNameOrId);
        }

        public async static Task<DiscordMember> MaybeGetMemberAsync(this DiscordGuild guild, ulong userId)
        {
            try { return await guild.GetMemberAsync(userId); }
            catch (NotFoundException ex) { return null; }
        }
        
        #endregion

        #region DiscordClient

        public static string[] GuildNames(this DiscordClient client)
        {
            return client?.Guilds.Values.Select((guild => guild.Name)).ToArray();
        }

        public static DiscordGuild DefaultGuild(this DiscordClient client)
        {
            return client?.Guilds.FirstOrDefault().Value;
        }
     
        public static DiscordGuild GuildByName(this DiscordClient client, string name)
        {
            return client?.Guilds.Values.FirstOrDefault(guild => guild.Name == name);
        }
        
        #endregion

        #region DiscordUser
 
        public static string UniqueUsername(this DiscordUser user)
        {
            return user.Username + user.Discriminator;
        }

        #endregion
    }
}