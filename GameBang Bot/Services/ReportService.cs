using Discord;
using Discord.WebSocket;
using GameBang_Bot.Properties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameBang_Bot.Services {
	class ReportService {
        private DiscordSocketClient discord;
        private ISocketMessageChannel adminChannel { get => discord.GetGuild(PropertiesContext.Discord.ServerId).GetTextChannel(PropertiesContext.Channel.ReportAdminChannelId); }
        private const string MESSAGE = ":shushing_face: 아무도 못보게 몰래 접수해드렸어요!\r\n해당 건은 관리자가 확인하고 바로 처리할테니 조금만 기다려주세요!";

        public ReportService(DiscordSocketClient discord) {
            this.discord = discord;
            this.discord.MessageReceived += Discord_MessageReceived;
        }

        private async Task Discord_MessageReceived(SocketMessage arg) {
            if (arg.Channel.Id == PropertiesContext.Channel.ReportChannelId) {
                var embed = new EmbedBuilder() {
                    Author = new EmbedAuthorBuilder() {
                        Name = discord.GetGuild(PropertiesContext.Discord.ServerId).GetUser(arg.Author.Id).Username,
                        IconUrl = arg.Author.GetAvatarUrl()
                    },
                    Timestamp = arg.Timestamp
                };
                embed.AddField("신고자", arg.Author.Username + "#" + arg.Author.Discriminator);

                if (arg.Attachments.Count > 0) {
                    var attachIter = arg.Attachments;
                    foreach (var a in attachIter) {
                        using (var client = new System.Net.WebClient()) {
                            byte[] imageData = client.DownloadData(a.Url);
                            await adminChannel.SendFileAsync(new System.IO.MemoryStream(imageData), a.Filename, embed: embed.Build());
                            await arg.Author.SendMessageAsync(MESSAGE);
                        }
                    }
                } else {
                    embed.Description = arg.Content;
                    await adminChannel.SendMessageAsync(embed: embed.Build());
                    await arg.Author.SendMessageAsync(MESSAGE + "\r\n 접수 내용:\r\n" + arg.Content);
                }

                await arg.DeleteAsync();
            }
        }
    }
}
