﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Heroes.ReplayParser;
using static System.Environment;

namespace HeroesReplay.Shared
{
    public static class Constants
    {
        public const string STORM_INTERFACE_NAME = "AhliObs 0.66.StormInterface";
        public const string STORM_REPLAY_EXTENSION = ".StormReplay";

        public const string STORM_REPLAY_WILDCARD = "*.StormReplay";
        public const string VARIABLES_WILDCARD = "*Variables.txt";

        public static readonly string USER_GAME_FOLDER = Path.Combine(GetFolderPath(SpecialFolder.MyDocuments), "Heroes of the Storm");
        public static readonly string USER_REPLAYS_PATH = Path.Combine(USER_GAME_FOLDER, "Accounts");
        public static readonly string USER_VARIABLES_PATH = Path.Combine(USER_GAME_FOLDER, "Variables.txt");
        public static readonly string USER_STORM_INTERFACE_PATH = Path.Combine(USER_GAME_FOLDER, "Interfaces", STORM_INTERFACE_NAME);


        public const int ZEMILL_BASE_LINE_HOTS_API_REPLAY_ID = 23940740;

        public static Dictionary<MatchAwardType, string[]> MatchAwards = new Dictionary<MatchAwardType, string[]>()
        {
            {MatchAwardType.ClutchHealer, new[] {"Clutch Healer"}},
            {MatchAwardType.HatTrick, new[] {"Hat Trick"}},
            {MatchAwardType.HighestKillStreak, new[] {"Dominator"}},
            {MatchAwardType.MostAltarDamage, new[] {"Cannoneer"}},
            {MatchAwardType.MostCoinsPaid, new[] {"Moneybags"}},
            {MatchAwardType.MostCurseDamageDone, new[] {"Master of the Curse"}},
            {MatchAwardType.MostDamageDoneToZerg, new[] {"Zerg Crusher"}},
            {MatchAwardType.MostDamageTaken, new[] {"Bulwark"}},
            {MatchAwardType.MostDamageToMinions, new[] {"Guardian Slayer"}},
            {MatchAwardType.MostDaredevilEscapes, new[] {"Daredevil"}},
            {MatchAwardType.MostDragonShrinesCaptured, new[] {"Shriner"}},
            {MatchAwardType.MostEscapes, new[] {"Escape Artist"}},
            {MatchAwardType.MostGemsTurnedIn, new[] {"Jeweler"}},
            {MatchAwardType.MostHealing, new[] {"Main Healer"}},
            {MatchAwardType.MostHeroDamageDone, new[] {"Painbringer"}},
            {MatchAwardType.MostImmortalDamage, new[] {"Immortal Slayer"}},
            {MatchAwardType.MostInterruptedCageUnlocks, new[] {"Loyal Defender"}},
            {MatchAwardType.MostKills, new[] {"Finisher"}},
            {MatchAwardType.MostMercCampsCaptured, new[] {"Headhunter"}},
            {MatchAwardType.MostNukeDamageDone, new[] {"Da Bomb"}},
            {MatchAwardType.MostProtection, new[] {"Protector"}},
            {MatchAwardType.MostRoots, new[] {"Trapper"}},
            {MatchAwardType.MostSeedsCollected, new[] {"Seed Collector"}},
            {MatchAwardType.MostSiegeDamageDone, new[] {"Siege Master"}},
            {MatchAwardType.MostSilences, new[] {"Silencer"}},
            {MatchAwardType.MostSkullsCollected, new[] {"Skull Collector"}},
            {MatchAwardType.MostStuns, new[] {"Stunner"}},
            {MatchAwardType.MostTeamfightDamageTaken, new[] {"Guardian"}},
            {MatchAwardType.MostTeamfightHealingDone, new[] {"Combat Medic"}},
            {MatchAwardType.MostTeamfightHeroDamageDone, new[] {"Scrapper"}},
            {MatchAwardType.MostTimeInTemple, new[] {"Temple Master"}},
            {MatchAwardType.MostTimeOnPoint, new[] {"Point Guard"}},
            {MatchAwardType.MostTimePushing, new[] {"Pusher"}},
            {MatchAwardType.MostVengeancesPerformed, new[] {"Avenger"}},
            {MatchAwardType.MostXPContribution, new[] {"Experienced"}},
            {MatchAwardType.MVP, new[] {"MVP"}},
            {MatchAwardType.ZeroDeaths, new[] {"Sole Survivor"}},
            {MatchAwardType.ZeroOutnumberedDeaths, new[] {"Team Player"}},
        };

        public static class Heroes
        {
            public static readonly Key[] KEYS_HEROES = { Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0 };
            public static readonly Key[] KEYS_CONSOLE_PANEL = { Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8 };

            public static readonly int[] TALENT_LEVELS = { 1, 4, 7, 10, 13, 16, 20 };

            public static List<Hero> All = typeof(Heroes).GetProperties(BindingFlags.Public).Where(p => p.PropertyType == typeof(Hero)).Select(p => (Hero)p.GetValue(null)).ToList();

            public static Hero Abathur = new Hero("Abathur", HeroType.Melee);
            public static Hero Alarak = new Hero("Alarak", HeroType.Melee);
            public static Hero Alexstrasza = new Hero("Alexstrasza", HeroType.Ranged);
            public static Hero Amazon = new Hero("Amazon", HeroType.Ranged);
            public static Hero Ana = new Hero("Ana", HeroType.Ranged);
            public static Hero Anduin = new Hero("Anduin", HeroType.Ranged);
            public static Hero Anubarak = new Hero("Anubarak", HeroType.Melee);
            public static Hero Artanis = new Hero("Artanis", HeroType.Melee);
            public static Hero Arthas = new Hero("Arthas", HeroType.Melee);
            public static Hero Auriel = new Hero("Auriel", HeroType.Ranged);
            public static Hero Azmodan = new Hero("Azmodan", HeroType.Ranged);
            public static Hero Barbarian = new Hero("Barbarian", HeroType.Melee);
            public static Hero Butcher = new Hero("Butcher", HeroType.Melee);
            public static Hero Chen = new Hero("Chen", HeroType.Melee);
            public static Hero Cho = new Hero("Cho", HeroType.Melee);
            public static Hero Chromie = new Hero("Chromie", HeroType.Ranged);
            public static Hero Crusader = new Hero("Crusader", HeroType.Melee);
            public static Hero Deathwing = new Hero("Deathwing", HeroType.Melee);
            public static Hero Deckard = new Hero("Deckard", HeroType.Melee);
            public static Hero Dehaka = new Hero("Dehaka", HeroType.Melee);
            public static Hero DemonHunter = new Hero("DemonHunter", HeroType.Ranged);
            public static Hero Diablo = new Hero("Diablo", HeroType.Melee);
            public static Hero Dryad = new Hero("Dryad", HeroType.Ranged);
            public static Hero DVa = new Hero("DVa", HeroType.Ranged);
            public static Hero FaerieDragon = new Hero("FaerieDragon", HeroType.Ranged);
            public static Hero Falstad = new Hero("Falstad", HeroType.Ranged);
            public static Hero Fenix = new Hero("Fenix", HeroType.Ranged);
            public static Hero Firebat = new Hero("Firebat", HeroType.Ranged);
            public static Hero Gall = new Hero("Gall", HeroType.Ranged);
            public static Hero Garrosh = new Hero("Garrosh", HeroType.Melee);
            public static Hero Genji = new Hero("Genji", HeroType.Ranged);
            public static Hero Greymane = new Hero("Greymane", HeroType.Ranged);
            public static Hero Guldan = new Hero("Guldan", HeroType.Ranged);
            public static Hero Hanzo = new Hero("Hanzo", HeroType.Ranged);
            public static Hero Illidan = new Hero("Illidan", HeroType.Melee);
            public static Hero Imperius = new Hero("Imperius", HeroType.Melee);
            public static Hero Jaina = new Hero("Jaina", HeroType.Ranged);
            public static Hero Junkrat = new Hero("Junkrat", HeroType.Ranged);
            public static Hero Kaelthas = new Hero("Kaelthas", HeroType.Ranged);
            public static Hero KelThuzad = new Hero("KelThuzad", HeroType.Ranged);
            public static Hero Kerrigan = new Hero("Kerrigan", HeroType.Melee);
            public static Hero L90ETC = new Hero("L90ETC", HeroType.Melee);
            public static Hero Leoric = new Hero("Leoric", HeroType.Melee);
            public static Hero LiLi = new Hero("LiLi", HeroType.Ranged);
            public static Hero LostVikings = new Hero("LostVikings", HeroType.Melee);
            public static Hero Lucio = new Hero("Lucio", HeroType.Ranged);
            public static Hero Maiev = new Hero("Maiev", HeroType.Melee);
            public static Hero Malfurion = new Hero("Malfurion", HeroType.Ranged);
            public static Hero MalGanis = new Hero("MalGanis", HeroType.Melee);
            public static Hero Malthael = new Hero("Malthael", HeroType.Melee);
            public static Hero Medic = new Hero("Medic", HeroType.Ranged);
            public static Hero Medivh = new Hero("Medivh", HeroType.Ranged);
            public static Hero Mephisto = new Hero("Mephisto", HeroType.Ranged);
            public static Hero Monk = new Hero("Monk", HeroType.Melee);
            public static Hero Muradin = new Hero("Muradin", HeroType.Melee);
            public static Hero Murky = new Hero("Murky", HeroType.Melee);
            public static Hero Necromancer = new Hero("Necromancer", HeroType.Melee);
            public static Hero NexusHunter = new Hero("NexusHunter", HeroType.Ranged);
            public static Hero Nova = new Hero("Nova", HeroType.Ranged);
            public static Hero Orphea = new Hero("Orphea", HeroType.Ranged);
            public static Hero Probius = new Hero("Probius", HeroType.Ranged);
            public static Hero Ragnaros = new Hero("Ragnaros", HeroType.Melee);
            public static Hero Raynor = new Hero("Raynor", HeroType.Ranged);
            public static Hero Rehgar = new Hero("Rehgar", HeroType.Melee);
            public static Hero Rexxar = new Hero("Rexxar", HeroType.Ranged);
            public static Hero Samuro = new Hero("Samuro", HeroType.Melee);
            public static Hero SgtHammer = new Hero("SgtHammer", HeroType.Ranged);
            public static Hero Stitches = new Hero("Stitches", HeroType.Melee);
            public static Hero Stukov = new Hero("Stukov", HeroType.Melee);
            public static Hero Sylvanas = new Hero("Sylvanas", HeroType.Ranged);
            public static Hero Tassadar = new Hero("Tassadar", HeroType.Ranged);
            public static Hero Thrall = new Hero("Thrall", HeroType.Melee);
            public static Hero Tinker = new Hero("Tinker", HeroType.Melee);
            public static Hero Tracer = new Hero("Tracer", HeroType.Ranged);
            public static Hero Tychus = new Hero("Tychus", HeroType.Ranged);
            public static Hero Tyrael = new Hero("Tyrael", HeroType.Melee);
            public static Hero Tyrande = new Hero("Tyrande", HeroType.Ranged);
            public static Hero Uther = new Hero("Uther", HeroType.Melee);
            public static Hero Valeera = new Hero("Valeera", HeroType.Melee);
            public static Hero Varian = new Hero("Varian", HeroType.Melee);
            public static Hero Whitemane = new Hero("Whitemane", HeroType.Ranged);
            public static Hero WitchDoctor = new Hero("WitchDoctor", HeroType.Ranged);
            public static Hero Wizard = new Hero("Wizard", HeroType.Ranged);
            public static Hero Yrel = new Hero("Yrel", HeroType.Melee);
            public static Hero Zagara = new Hero("Zagara", HeroType.Ranged);
            public static Hero Zarya = new Hero("Zarya", HeroType.Ranged);
            public static Hero Zeratul = new Hero("Zeratul", HeroType.Melee);
            public static Hero Zuljin = new Hero("Zuljin", HeroType.Ranged);

            public const string HEROES_PROCESS_NAME = "HeroesOfTheStorm_x64";
            public const string HEROES_SWITCHER_PROCESS = "HeroesSwitcher_x64.exe";

            public static TimeSpan KILL_STREAK_TIMER = TimeSpan.FromSeconds(12);

            public static TimeSpan MAX_PENTA_KILL_STREAK_POTENTIAL = KILL_STREAK_TIMER * 4;
            public static TimeSpan MAX_QUAD_KILL_STREAK_POTENTIAL = KILL_STREAK_TIMER * 3;
            public static TimeSpan MAX_TRIPLE_KILL_STREAK_POTENTIAL = KILL_STREAK_TIMER * 2;
            public static TimeSpan MAX_MULTI_KILL_STREAK_POTENTIAL = KILL_STREAK_TIMER * 1;
        }

        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_CHAR = 0x102;
        public const int SRCCOPY = 0x00CC0020;

        public static class Cmd
        {
            public const string ARG_REPLAY = "replay";
            public const string ARG_LAUNCH = "launch";
            public const string ARG_BNET_INSTALL = "bnet";
            public const string ARG_REPLAYS_DIR = "replays";
        }

        public static class Ocr
        {
            public const string LOADING_SCREEN_TEXT = "WELCOME TO";
            public const string GAME_RUNNING_TEXT = "Game is running.";
            public const string PLAY_BUTTON_TEXT = "PLAY";
            public const string SHOP_HEROES_TEXT = "Shop Heroes of the Storm";

            public const string TIMER_COLON = ":";
            public const int TIMER_HOURS = 3;
            public const int TIMER_MINUTES = 2;

            public const string TIMESPAN_FORMAT_HOURS = "hh\\:mm\\:ss";
            public const string TIMESPAN_MATCH_START_FORMAT = "\\-mm\\:ss";
            public const string TIMESPAN_FORMAT_MINUTES = "mm\\:ss";
        }

        public static class Bnet
        {
            public const string BATTLE_NET_LAUNCHER_EXE = "Battle.net Launcher.exe";
            public const string BATTLE_NET_EXE = "Battle.net.exe";
            public const string BATTLE_NET_PROCESS_NAME = "Battle.net";
            public const string BATTLE_NET_SELECT_HEROES_ARG = "--game heroes";
            public const string BATTLE_NET_DEFAULT_INSTALL_PATH = "C:\\Program Files (x86)\\Battle.net";
        }
    }
}
