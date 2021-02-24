using Discord.Commands;
using GameBang_Bot.Properties;
using GameBang_Bot.Properties.Configs;
using System;
using System.Threading.Tasks;

namespace GameBang_Bot.CommandBases {
	public class ChannelAttribute : PreconditionAttribute {
		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services) {
			var type = PropertiesContext.Channel.GetType();
			var property = type.GetProperty(command.Module.Name + "Id");
			ulong channelId = (UInt64)property.GetValue(PropertiesContext.Channel);

			if (context.Channel.Id == channelId)
				return Task.FromResult(PreconditionResult.FromSuccess());
			else
				return Task.FromResult(PreconditionResult.FromError("The command is not for this channel"));
		}
	}
}