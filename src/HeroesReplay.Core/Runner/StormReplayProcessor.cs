﻿using System;
using System.Threading.Tasks;
using HeroesReplay.Core.Picker;
using HeroesReplay.Core.Replays;
using HeroesReplay.Core.Shared;
using Microsoft.Extensions.Logging;

namespace HeroesReplay.Core.Runner
{
    public sealed class StormReplayProcessor
    {
        private readonly CancellationTokenProvider tokenProvider;
        private readonly ILogger<StormReplayConsumer> logger;
        private readonly IStormReplayProvider provider;
        private readonly IStormReplaySaver saver;
        private readonly StormReplayPicker picker;

        public StormReplayProcessor(ILogger<StormReplayConsumer> logger, IStormReplayProvider provider, IStormReplaySaver saver, StormReplayPicker picker, CancellationTokenProvider tokenProvider)
        {
            this.logger = logger;
            this.provider = provider;
            this.saver = saver;
            this.picker = picker;
            this.tokenProvider = tokenProvider;
        }

        public async Task RunAsync()
        {
            try
            {
                while (!tokenProvider.Token.IsCancellationRequested)
                {
                    StormReplay? stormReplay = await provider.TryLoadReplayAsync();

                    if (stormReplay != null)
                    {
                        logger.LogInformation("Loaded: " + stormReplay.Path);

                        if (picker.IsInteresting(stormReplay))
                        {
                            await saver.SaveReplayAsync(stormReplay);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "There was an error in the replay consumer.");
            }
            finally
            {

            }
        }
    }
}