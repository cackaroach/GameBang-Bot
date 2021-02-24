using Discord.Commands;
using GameBang_Bot.Database;

namespace GameBang_Bot.CommandBases {
	public class CommandBase : ModuleBase<SocketCommandContext> {
		private static PostgresContext dbContext = null;
		protected static PostgresContext DbContext {
			get => (dbContext ??= new PostgresContext());
		}
	}
}
