﻿using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HeroesReplay.Analyzer;
using HeroesReplay.CLI.Options;
using HeroesReplay.Processes;
using HeroesReplay.Replays;
using HeroesReplay.Runner;
using HeroesReplay.Shared;
using HeroesReplay.Spectator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HeroesReplay.CLI.Commands
{
    public class HotsApiCommand : Command
    {
        public HotsApiCommand() : base("hotsapi", "Access the HotsApi database to download uploaded replays and spectate them.")
        {
            AddOption(new LaunchOption());
            AddOption(new MinimumReplayIdOption());
            AddOption(new BattlenetOption());
            AddOption(new AwsAccessKeyOption());
            AddOption(new AwsSecretKeyOption());
            AddOption(new CaptureMethodOption());

            Handler = CommandHandler.Create<int, string, string, bool, DirectoryInfo, CaptureMethod, CancellationToken>(CommandAsync);
        }

        private async Task CommandAsync(int minReplayId, string awsAccessKey, string awsSecretKey, bool launch, DirectoryInfo bnet, CaptureMethod captureMethod, CancellationToken cancellationToken)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>(Constants.ConfigKeys.BattleNetPath, bnet.FullName),
                    new KeyValuePair<string, string>(Constants.ConfigKeys.MinReplayId, minReplayId.ToString()),
                    new KeyValuePair<string, string>(Constants.ConfigKeys.Launch, launch.ToString()),
                    new KeyValuePair<string, string>(Constants.ConfigKeys.AwsAccessKey, awsAccessKey),
                    new KeyValuePair<string, string>(Constants.ConfigKeys.AwsSecretKey, awsSecretKey),
                })
                .Build();

            ServiceProvider serviceProvider = new ServiceCollection()
                .AddLogging(loggingBuilder => loggingBuilder.AddConsole())
                .AddSingleton<IConfiguration>(provider => configuration)
                .AddSingleton((provider) => new CancellationTokenProvider(cancellationToken))
                .AddSingleton<HeroesOfTheStorm>()
                .AddSingleton<BattleNet>()
                .AddSingleton<ScreenCapture>((provider => new ScreenCapture(captureMethod)))
                .AddSingleton<StormReplayAnalyzer>()
                .AddSingleton<StormReplayHeroSelector>()
                .AddSingleton<StormReplaySpectator>()
                .AddSingleton<StormReplayDetailsWriter>()
                .AddSingleton<PlayerBlackListChecker>()
                .AddSingleton<IStormReplayProvider, StormReplayHotsApiProvider>()
                .AddSingleton<StormReplayConsumer>()
                .AddSingleton<StormReplayRunner>()
                .AddSingleton<AdminChecker>()
                .BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                StormReplayConsumer stormReplayConsumer = scope.ServiceProvider.GetRequiredService<StormReplayConsumer>();

                await stormReplayConsumer.ReplayAsync(launch);
            }
        }
    }
}