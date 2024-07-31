using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using Discord.Interactions;
using DiscordBot.Services;
using DiscordBot.Util;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;


namespace DiscordBot.Commands
{
    public class TestCommands : BaseCommandModule
    {
        private readonly GuessingGame _guessingGame;

        public TestCommands(GuessingGame guessingGame)
        {
            _guessingGame = guessingGame;
        }
        DiscordEmbedHelper newEmbed = new DiscordEmbedHelper();

        [Command("test")]
        public async Task MyFirstCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"hello");
        }

        [Command("ping")]
        public async Task PingCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"Pong! :ping_pong:");
        }

        [Command("userinfo")]
        [Description("Exibe informações sobre o usuário que pediu o comando.")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task UserInfoCommand(CommandContext ctx)
        {
            var user = ctx.User;

            var embed = newEmbed.CreateEmbedMoreOptions(user.Username, user.AvatarUrl);

            embed.AddField("ID", user.Id.ToString(), inline: true)
                 .AddField("Name", user.Username.ToString(), inline: true)
                 .AddField("Data de Criação", user.CreationTimestamp.DateTime.ToString("dd-MM-yyyy HH:mm:ss"), inline: true);

            await ctx.RespondAsync(embed: embed);
        }

        [Command("comandos")]
        [Description("Lista todos os comandos disponíveis.")]
        public async Task ListCommands(CommandContext ctx)
        {
            var commands = ctx.CommandsNext.RegisteredCommands.Values
                .Where(cmd => !cmd.CustomAttributes.Any(attr => attr is HiddenAttribute)) // Filtra comandos ocultos
                .Select(cmd => $"`{ctx.Prefix}{cmd.Name}` - {cmd.Description ?? "Sem descrição"}");

            var embed = new DiscordEmbedBuilder
            {
                Title = "Lista de Comandos",
                Description = string.Join("\n", commands),
                Color = new DiscordColor(0x00ff00) // Cor verde
            };

            await ctx.RespondAsync(embed: embed);
        }

        [Command("dado")]
        [Description("Rola um dado de 6 lados.")]
        public async Task RollDiceCommand(CommandContext ctx)
        {
            Random random = new Random();
            int result = random.Next(1, 7);

            var embed = new DiscordEmbedBuilder
            {
                Title = "Resultado do Dado",
                Description = $"🎲 Você rolou um dado e obteve: **{result}**!",
                Color = new DiscordColor(0x3498db) // Cor azul
            };

            await ctx.RespondAsync(embed: embed);
        }

        [Command("limpar")]
        [Description("Limpa mensagens no canal.")]
        [RequirePermissions(DSharpPlus.Permissions.Administrator)]
        public async Task ClearMessagesCommand(CommandContext ctx, int quantidade)
        {
            var messages = await ctx.Channel.GetMessagesAsync(quantidade);
            await ctx.Channel.DeleteMessagesAsync(messages);

            // Aguarda um curto período antes de limpar a mensagem do comando
            await Task.Delay(2000);

            await ctx.Message.DeleteAsync(); // Limpa a mensagem do comando
        }

        [Command("limpartudo")]
        [Description("Limpa todas as mensagens no canal.")]
        [RequirePermissions(DSharpPlus.Permissions.Administrator)]
        public async Task ClearAllMessagesCommand(CommandContext ctx)
        {
            var messages = await ctx.Channel.GetMessagesAsync();
            await ctx.Channel.DeleteMessagesAsync(messages);

            // Aguarda um curto período antes de limpar a mensagem do comando
            await Task.Delay(2000);

            await ctx.Message.DeleteAsync(); // Limpa a mensagem do comando
        }

        [Command("boasvindas")]
        [Description("Dá boas-vindas a um novo membro marcado.")]
        public async Task WelcomeCommand(CommandContext ctx, DiscordMember member)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Boas-Vindas",
                Description = $"👋 Bem-vindo(a) ao servidor, {member.Mention}!",
                Color = new DiscordColor(0x00ff00) // Cor verde
            };

            await ctx.RespondAsync(embed: embed);
        }

        [Command("contagem")]
        [Description("Exibe a contagem de usuários no servidor.")]
        public async Task CountCommand(CommandContext ctx)
        {
            var membersCount = ctx.Guild.MemberCount;
            await ctx.RespondAsync($"O servidor tem atualmente {membersCount} membros.");
        }


        [Command("infoserver")]
        [Description("Exibe informações sobre o servidor.")]
        public async Task ServerInfoCommand(CommandContext ctx)
        {
            var guild = ctx.Guild;

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Informações sobre o Servidor - {guild.Name}",
                Color = DiscordColor.Blurple,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = guild.IconUrl }
            };

            embed.AddField("ID do Servidor", guild.Id.ToString(), inline: true);
            embed.AddField("Dono do Servidor", $"{guild.Owner.Username}#{guild.Owner.Discriminator}", inline: true);
            embed.AddField("Membros", guild.MemberCount.ToString(), inline: true);

            // Itera sobre os canais do servidor
            var textoCount = 0;
            var vozCount = 0;
            foreach (var channel in guild.Channels.Values)
            {
                if (channel.Type == ChannelType.Text)
                {
                    textoCount++;
                }
                else if (channel.Type == ChannelType.Voice)
                {
                    vozCount++;
                }
            }

            embed.AddField("Canais de Texto", textoCount.ToString(), inline: true);
            embed.AddField("Canais de Voz", vozCount.ToString(), inline: true);

            await ctx.RespondAsync(embed: embed);
        }

        [Command("iniciarsorteio")]
        [Description("Envia a mensagem para o inicio do sorteio, os usuários que reagiram a essa mensagem vão participar.")]
        [RequirePermissions(DSharpPlus.Permissions.Administrator)]
        public async Task IniciarSorteio(CommandContext ctx, string premio)
        {
            var embed = newEmbed.CreateEmbedSuccessMessage($"Sorteando {premio}", "Reaga para participar");

            var targetMessage = await ctx.Channel.SendMessageAsync(embed: embed);

            // Use um emoji de confirmação (✅) como exemplo
            var emoji = DiscordEmoji.FromUnicode("✅");

            // Adicione a reação à mensagem
            await targetMessage.CreateReactionAsync(emoji);
        }



        [Command("sorteio")]
        [Description("Realiza um sorteio entre os usuários que reagiram a uma mensagem específica.")]
        [RequirePermissions(DSharpPlus.Permissions.Administrator)]
        public async Task SorteioCommand(CommandContext ctx, ulong messageId)
        {
            // Obtenha a mensagem alvo usando o ID fornecido
            var targetMessage = await ctx.Channel.GetMessageAsync(messageId);

            if (targetMessage is null)
            {
                var embed = newEmbed.CreateEmbedWarningMessage("Não foi possível encontrar a mensagem alvo.", "");
                await ctx.RespondAsync(embed: embed);
            }

            // Obtenha as reações da mensagem
            var reactions = targetMessage.Reactions;

            // Se houver pelo menos uma reação
            if (reactions.Count > 0)
            {
                // Obtenha uma lista de usuários que reagiram à mensagem
                var reactionUsers = await targetMessage.GetReactionsAsync(reactions.First().Emoji);

                // pega a lista de DiscordUsers que reagiram a msg e que não sao bots e ordena de forma pseudoaleatoria
                var random = new Random();
                var shuffledUsers = reactionUsers.Where(u => !u.IsBot).OrderBy(u => random.Next()).ToList();

                // Se houver pelo menos dois usuários, selecione o vencedor
                if (shuffledUsers.Count >= 2)
                {
                    var winner = shuffledUsers.First();
                    var embed = newEmbed.CreateEmbedSuccessMessage("Sorteio", $"O vencedor é: {winner.Mention}!");
                    await ctx.RespondAsync(embed: embed);
                }
                else
                {
                    var embed = newEmbed.CreateEmbedWarningMessage("Não há usuários suficientes para realizar o sorteio.", $"");
                    await ctx.RespondAsync(embed: embed);
                }
            }
            else
            {
                var embed = newEmbed.CreateEmbedWarningMessage("Nenhum usuario reagiu ao esta mensagem. Por favor verique o id da mensagem.", $"");
                await ctx.RespondAsync(embed: embed);
            }
        }

        [Command("startgame")]
        public async Task StartGame(CommandContext ctx)
        {
            _guessingGame.StartNewGame();
            var embed = newEmbed.CreateEmbedWarningMessage("Novo jogo iniciado! Tente adivinhar o número.", $"");
            await ctx.RespondAsync(embed: embed);
        }

        [Command("guess")]
        public async Task GuessNumber(CommandContext ctx, int number)
        {
            var result = _guessingGame.Guess(number);

            if (_guessingGame._isGameOver)
            {
                var embedSucess = newEmbed.CreateEmbedSuccessMessage(result, "");
                await ctx.RespondAsync(embed: embedSucess);
            }
            else
            {
                var embed = newEmbed.CreateEmbedWarningMessage(result, "");
                await ctx.RespondAsync(embed: embed);
            }
        }

        [Command("calculate")]
        [Description("!calculate {operação - add, divide, subtract, multiply} {num1} {num2}")]
        public async Task Calculate(CommandContext ctx, string operation, double num1, double num2)
        {
            Calculator calculator = new Calculator();
            double result = 0;

            try
            {
                switch (operation.ToLower())
                {
                    case "add":
                        result = calculator.Add(num1, num2);
                        break;
                    case "subtract":
                        result = calculator.Subtract(num1, num2);
                        break;
                    case "multiply":
                        result = calculator.Multiply(num1, num2);
                        break;
                    case "divide":
                        result = calculator.Divide(num1, num2);
                        break;
                    default:
                        var embedInvalid = newEmbed.CreateEmbedErrorMessage("Invalid operation. Available operations: add, subtract, multiply, divide.", "");
                        await ctx.Channel.SendMessageAsync(embed: embedInvalid);
                        return;
                }

                var embed = newEmbed.CreateEmbedSuccessMessage($"Result of {operation} {num1} and {num2} is: {result}", "");
                await ctx.Channel.SendMessageAsync(embed: embed);
            }
            catch (InvalidOperationException ex)
            {
                var embed = newEmbed.CreateEmbedErrorMessage($"Error: {ex.Message}", "");
                await ctx.Channel.SendMessageAsync(embed: embed);
            }
        }
        [Command("createinvite")]
        public async Task CreateInvite(CommandContext ctx)
        {
            // Check if the bot has permission to create instant invites in the current channel
            var botPermissions = ctx.Channel.PermissionsFor(ctx.Guild.CurrentMember);

            if (!botPermissions.HasPermission(Permissions.CreateInstantInvite))
            {
                await ctx.Channel.SendMessageAsync("O bot não tem permissão para criar convites neste servidor ou canal.");
                return;
            }

            // Create an invite for the current channel
            var invite = await ctx.Channel.CreateInviteAsync();

            // Send the invite URL back to the channel
            await ctx.Channel.SendMessageAsync($"Convite criado: {invite}");
        }

        [Command("channel-list")]
        public async Task ChannelList(CommandContext ctx)
        {
            var channelComponent = new DiscordChannelSelectComponent("channelDropDownList", "Select...");

            var dropDownMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Gold)
                    .WithTitle("This embed has a channel drop-down list on it"))
                .AddComponents(channelComponent);

            await ctx.Channel.SendMessageAsync(dropDownMessage);
        }

        [Command("mention-list")]
        public async Task MentionList(CommandContext ctx)
        {
            var mentionComponent = new DiscordMentionableSelectComponent("mentionDropDownList", "Select...");

            var dropDownMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Gold)
                    .WithTitle("This embed has a mention drop-down list on it"))
                .AddComponents(mentionComponent);

            await ctx.Channel.SendMessageAsync(dropDownMessage);
        }

        [Command("poll50")]
        [Description("Cria uma votaçao que se encerra apos 50 segundos !poll {opçao1} {opçao2} {opçao3} {opçao4} {tituloDaPoll}")]
        public async Task Poll(CommandContext ctx, string option1, string option2, string option3, string option4, [RemainingText] string pollTitle)
        {
            var interactivity = Program.Client.GetInteractivity();
            var pollTime = TimeSpan.FromSeconds(50);

            DiscordEmoji[] emojiOptions = { DiscordEmoji.FromName(Program.Client, ":one:"),
                                            DiscordEmoji.FromName(Program.Client, ":two:"),
                                            DiscordEmoji.FromName(Program.Client, ":three:"),
                                            DiscordEmoji.FromName(Program.Client, ":four:") };

            string optionsDescription = $"{emojiOptions[0]} | {option1} \n" +
                                        $"{emojiOptions[1]} | {option2} \n" +
                                        $"{emojiOptions[2]} | {option3} \n" +
                                        $"{emojiOptions[3]} | {option4}";

            var pollMessage = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Red,
                Title = pollTitle,
                Description = optionsDescription
            };

            var sentPoll = await ctx.Channel.SendMessageAsync(embed: pollMessage);
            foreach (var emoji in emojiOptions)
            {
                await sentPoll.CreateReactionAsync(emoji);
            }

            var totalReactions = await interactivity.CollectReactionsAsync(sentPoll, pollTime);

            int count1 = 0;
            int count2 = 0;
            int count3 = 0;
            int count4 = 0;

            foreach (var emoji in totalReactions)
            {
                if (emoji.Emoji == emojiOptions[0])
                {
                    count1++;
                }
                if (emoji.Emoji == emojiOptions[1])
                {
                    count2++;
                }
                if (emoji.Emoji == emojiOptions[2])
                {
                    count3++;
                }
                if (emoji.Emoji == emojiOptions[3])
                {
                    count4++;
                }
            }

            int totalVotes = count1 + count2 + count3 + count4;
            string resultsDescription = $"{emojiOptions[0]}: {count1} Votes \n" +
                                        $"{emojiOptions[1]}: {count2} Votes \n" +
                                        $"{emojiOptions[2]}: {count3} Votes \n" +
                                        $"{emojiOptions[3]}: {count4} Votes \n\n" +
                                        $"Total Votes: {totalVotes}";

            var resultEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Green,
                Title = "Results of the Poll",
                Description = resultsDescription
            };

            await ctx.Channel.SendMessageAsync(embed: resultEmbed);
        }

        [Command("1v1")]
        public async Task ChallengeUser(CommandContext ctx, DiscordMember opponent)
        {
            var challengeEmbed = new DiscordEmbedBuilder
            {
                Title = "Desafio 1v1",
                Description = $"{opponent.Mention}, você foi desafiado para um 1v1 por {ctx.User.Mention}! Reaja com ✅ para aceitar ou ❌ para recusar.",
                Color = DiscordColor.Blurple // Cor personalizada do embed
            };

            var challengeMessage = await ctx.Channel.SendMessageAsync(embed: challengeEmbed);

            var acceptEmoji = DiscordEmoji.FromUnicode("✅");
            var rejectEmoji = DiscordEmoji.FromUnicode("❌");

            await challengeMessage.CreateReactionAsync(acceptEmoji);
            await challengeMessage.CreateReactionAsync(rejectEmoji);

            var interactivity = ctx.Client.GetInteractivity();
            var reactionResult = await interactivity.WaitForReactionAsync(
                x => x.Message.Id == challengeMessage.Id && (x.Emoji == acceptEmoji || x.Emoji == rejectEmoji),
                TimeSpan.FromMinutes(1)
            );

            if (reactionResult.Result == null)
            {
                await SendTimeoutMessage(ctx.Channel);
                return;
            }

            var emoji = reactionResult.Result.Emoji;
            if (emoji == rejectEmoji)
            {
                await SendRejectionMessage(ctx.Channel, opponent);
                return;
            }

            // Se chegou aqui, a reação foi ✅
            await ProcessAcceptance(ctx, opponent, challengeMessage);
        }

        private async Task ProcessAcceptance(CommandContext ctx, DiscordMember opponent, DiscordMessage challengeMessage)
        {
            var trophyEmoji = DiscordEmoji.FromUnicode("🏆");
            await challengeMessage.CreateReactionAsync(trophyEmoji);

            var interactivity = ctx.Client.GetInteractivity();
            var trophyResult = await interactivity.WaitForReactionAsync(
                x => x.Message.Id == challengeMessage.Id && x.Emoji == trophyEmoji && (x.User.Id == ctx.User.Id || x.User.Id == opponent.Id),
                TimeSpan.FromMinutes(10)
            );

            var resultEmbed = trophyResult.Result != null
                ? GenerateResultEmbed(ctx.User, opponent, trophyResult.Result.User)
                : GenerateTimeoutEmbed("O tempo para reagir com o troféu expirou.");

            await ctx.Channel.SendMessageAsync(embed: resultEmbed);
        }

        private async Task SendRejectionMessage(DiscordChannel channel, DiscordMember opponent)
        {
            var resultEmbed = new DiscordEmbedBuilder
            {
                Title = "Desafio Recusado",
                Description = $"{opponent.Mention} recusou o desafio.",
                Color = DiscordColor.Red // Cor personalizada do embed para erro
            };

            await channel.SendMessageAsync(embed: resultEmbed);
        }

        private async Task SendTimeoutMessage(DiscordChannel channel)
        {
            var resultEmbed = new DiscordEmbedBuilder
            {
                Title = "Tempo Esgotado",
                Description = "O tempo para aceitar o desafio expirou.",
                Color = DiscordColor.Orange // Cor personalizada do embed para aviso
            };

            await channel.SendMessageAsync(embed: resultEmbed);
        }

        private DiscordEmbedBuilder GenerateResultEmbed(DiscordUser challenger, DiscordUser opponent, DiscordUser winner)
        {
            var loser = winner.Id == challenger.Id ? opponent : challenger;

            UserStatsService.UpdateUserStats(winner.Id, true);
            UserStatsService.UpdateUserStats(loser.Id, false);

            return new DiscordEmbedBuilder
            {
                Title = "Resultado do Desafio",
                Description = $"O desafio 1v1 foi aceito! O vencedor é {winner.Mention}. {loser.Mention} perdeu.",
                Color = DiscordColor.Green // Cor personalizada do embed para sucesso
            };
        }

        private DiscordEmbedBuilder GenerateTimeoutEmbed(string message)
        {
            return new DiscordEmbedBuilder
            {
                Title = "Tempo Esgotado",
                Description = message,
                Color = DiscordColor.Orange // Cor personalizada do embed para aviso
            };
        }


        [Command("stats")]
        public async Task ShowStats(CommandContext ctx)
        {
            var stats = UserStatsService.GetUserStats(ctx.User.Id);
            var embed = new DiscordEmbedBuilder
            {
                Title = $"{ctx.User.Username}'s Stats",
                Color = DiscordColor.Blurple
            };

            embed.AddField("Wins", stats.Wins.ToString(), inline: true);
            embed.AddField("Losses", stats.Losses.ToString(), inline: true);

            await ctx.RespondAsync(embed: embed);
        }

        [Command("ping2")]
        public async Task Ping(CommandContext ctx)
        {
            var latency = ctx.Client.Ping;
            await ctx.RespondAsync($"Pong! Latência: {latency}ms");
        }




    }

}

