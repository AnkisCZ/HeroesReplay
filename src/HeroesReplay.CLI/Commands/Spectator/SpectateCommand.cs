﻿using HeroesReplay.Core.Shared;
using System.CommandLine;

namespace HeroesReplay.CLI.Commands
{
    public class SpectateCommand : Command
    {
        public SpectateCommand() : base("spectate", $"Spectate {Constants.STORM_REPLAY_EXTENSION} files using OCR detection, real-time replay analysis and game automation.")
        {
            AddCommand(new FileCommand());
            AddCommand(new DirectoryCommand());
            AddCommand(new HotsApiCommand());
        }
    }
}