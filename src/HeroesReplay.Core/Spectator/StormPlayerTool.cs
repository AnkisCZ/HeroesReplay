﻿using System;
using System.Collections.Generic;
using System.Linq;
using Heroes.ReplayParser;
using Heroes.ReplayParser.MPQFiles;
using HeroesReplay.Core.Analyzer;
using HeroesReplay.Core.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HeroesReplay.Core.Spectator
{
    public class StormPlayerTool
    {
        private readonly ILogger<StormPlayerTool> logger;
        private readonly StormReplayAnalyzer analyzer;
        private readonly ReplayHelper replayHelper;
        private readonly Settings settings;

        public StormPlayerTool(ILogger<StormPlayerTool> logger, StormReplayAnalyzer analyzer, IOptions<Settings> settings, ReplayHelper replayHelper)
        {
            this.logger = logger;
            this.analyzer = analyzer;
            this.replayHelper = replayHelper;
            this.settings = settings.Value;
        }

        public IEnumerable<StormPlayer> GetPlayers(Replay replay, TimeSpan timer)
        {
            DateTime start = DateTime.Now;

            List<IEnumerable<StormPlayer>> priority = new List<IEnumerable<StormPlayer>>
            {
                Select(analyzer.Analyze(replay, timer, timer.Add(settings.MaxQuintupleTime)), SpectateEvent.QuintupleKill),
                Select(analyzer.Analyze(replay, timer, timer.Add(settings.MaxQuadTime)), SpectateEvent.QuadKill),
                Select(analyzer.Analyze(replay, timer, timer.Add(settings.MaxTripleTime)), SpectateEvent.TripleKill),
                Select(analyzer.Analyze(replay, timer, timer.Add(settings.MaxMultiTime)), SpectateEvent.MultiKill),
                Select(analyzer.Analyze(replay, timer, timer.Add(settings.KillTime)), SpectateEvent.Kill),
                Select(analyzer.Analyze(replay, timer, timer.Add(TimeSpan.FromSeconds(10))), SpectateEvent.Death),
                Select(analyzer.Analyze(replay, timer, timer.Add(TimeSpan.FromSeconds(10))), SpectateEvent.Boss),
                Select(analyzer.Analyze(replay, timer, timer.Add(TimeSpan.FromSeconds(10))), SpectateEvent.Camp),
                Select(analyzer.Analyze(replay, timer, timer.Add(TimeSpan.FromSeconds(10))), SpectateEvent.MapObjective),
                Select(analyzer.Analyze(replay, timer, timer.Add(TimeSpan.FromSeconds(10))), SpectateEvent.TeamObjective),
                Select(analyzer.Analyze(replay, timer, timer.Add(TimeSpan.FromSeconds(5))), SpectateEvent.Unit),
                Select(analyzer.Analyze(replay, timer, timer.Add(TimeSpan.FromSeconds(5))), SpectateEvent.Taunt),
                Select(analyzer.Analyze(replay, timer, timer.Add(TimeSpan.FromSeconds(5))), SpectateEvent.Structure),
                Select(analyzer.Analyze(replay, timer, timer.Add(TimeSpan.FromSeconds(5))), SpectateEvent.Proximity),
                Select(analyzer.Analyze(replay, timer.Subtract(TimeSpan.FromSeconds(5)), timer.Add(TimeSpan.FromSeconds(5))), SpectateEvent.Killer),
                Select(analyzer.Analyze(replay, timer, timer.Add(TimeSpan.FromSeconds(5))), SpectateEvent.Alive)
            };

            IEnumerable<StormPlayer> result = priority.FirstOrDefault(collection => collection.Any()) ?? Enumerable.Empty<StormPlayer>();

            logger.LogDebug("analyze time: " + (DateTime.Now - start));

            return result;
        }

        private IEnumerable<StormPlayer> Select(AnalyzerResult result, SpectateEvent spectateEvent)
        {
            IEnumerable<StormPlayer> players = GetPlayers(result, spectateEvent);

            // Ordered by which one happens first
            return players.OrderBy(stormPlayer => stormPlayer.Duration);
        }

        private IEnumerable<StormPlayer> GetPlayers(AnalyzerResult result, SpectateEvent spectateEvent)
        {
            return spectateEvent switch
            {
                SpectateEvent.QuintupleKill => HandleKills(result, SpectateEvent.QuintupleKill),
                SpectateEvent.QuadKill => HandleKills(result, SpectateEvent.QuadKill),
                SpectateEvent.TripleKill => HandleKills(result, SpectateEvent.TripleKill),
                SpectateEvent.MultiKill => HandleKills(result, SpectateEvent.MultiKill),
                SpectateEvent.Kill => HandleKills(result, SpectateEvent.Kill),
                SpectateEvent.Death => HandleDeaths(result),
                SpectateEvent.MapObjective => HandleMapObjectives(result),
                SpectateEvent.TeamObjective => HandleTeamObjectives(result),
                SpectateEvent.Boss => HandleBosses(result),
                SpectateEvent.Camp => HandleCamps(result),
                SpectateEvent.Unit => HandleUnits(result),
                SpectateEvent.Structure => HandleStructures(result),
                SpectateEvent.Killer => HandleKillers(result),
                SpectateEvent.Proximity => HandleProximity(result),
                SpectateEvent.Taunt => HandleTaunts(result),
                SpectateEvent.Alive => HandleAlive(result),
                SpectateEvent.Ping => HandlePings(result),
                _ => throw new ArgumentOutOfRangeException(nameof(spectateEvent), spectateEvent, null)
            };
        }

        private IEnumerable<StormPlayer> HandleTaunts(AnalyzerResult result)
        {
            foreach ((Player player, TimeSpan time) in result.Taunts)
            {
                yield return new StormPlayer(player, result.Start, time, SpectateEvent.Taunt);
            }
        }

        private IEnumerable<StormPlayer> HandleBosses(AnalyzerResult result)
        {
            foreach ((Player player, TimeSpan time) in result.BossCaptures)
            {
                yield return new StormPlayer(player, result.Start, time, SpectateEvent.Boss);
            }
        }

        private IEnumerable<StormPlayer> HandleProximity(AnalyzerResult result)
        {
            foreach (Player player in result.Proximity)
            {
                yield return new StormPlayer(player, result.Start, result.Duration, SpectateEvent.Proximity);
            }
        }

        private IEnumerable<StormPlayer> HandleUnits(AnalyzerResult result)
        {
            foreach ((Player player, TimeSpan time) in result.EnemyUnits)
            {
                yield return new StormPlayer(player, result.Start, time, SpectateEvent.Unit);
            }
        }

        private IEnumerable<StormPlayer> HandleKillers(AnalyzerResult result)
        {
            foreach ((Player player, TimeSpan time) in result.Killers)
            {
                yield return new StormPlayer(player, result.Start, time, SpectateEvent.Killer);
            }
        }

        private IEnumerable<StormPlayer> HandleDeaths(AnalyzerResult result)
        {
            foreach (Unit death in result.Deaths)
            {
                yield return new StormPlayer(death.PlayerControlledBy, result.Start, death.TimeSpanDied.Value.Add(TimeSpan.FromSeconds(1)), SpectateEvent.Death);
            }
        }

        private IEnumerable<StormPlayer> HandlePings(AnalyzerResult result)
        {
            foreach (GameEvent ping in result.Pings)
            {
                yield return new StormPlayer(ping.player, result.Start, ping.TimeSpan, SpectateEvent.Ping);
            }
        }

        private IEnumerable<StormPlayer> HandleMapObjectives(AnalyzerResult result)
        {
            foreach ((Player player, TimeSpan time) in result.MapObjectives)
            {
                yield return new StormPlayer(player, result.Start, time, SpectateEvent.MapObjective);
            }
        }

        private IEnumerable<StormPlayer> HandleStructures(AnalyzerResult result)
        {
            foreach (Unit unit in result.Structures)
            {
                yield return new StormPlayer(unit.PlayerKilledBy, result.Start, unit.TimeSpanDied.Value, SpectateEvent.Structure);
            }
        }

        private IEnumerable<StormPlayer> HandleAlive(AnalyzerResult result)
        {
            if (result.Alive.Except(result.AllyCore).Any() || result.EnemyCore.Any())
            {
                if (result.EnemyCore.Any())
                {
                    foreach (Player player in result.EnemyCore)
                    {
                        yield return new StormPlayer(player, result.Start, result.Duration, SpectateEvent.Alive);
                    }
                }
                else
                {
                    foreach (Player player in result.Alive.Except(result.AllyCore))
                    {
                        yield return new StormPlayer(player, result.Start, result.Duration, SpectateEvent.Alive);
                    }
                }
            }
            else
            {
                foreach (Player player in result.Alive)
                {
                    yield return new StormPlayer(player, result.Start, result.Duration, SpectateEvent.Alive);
                }
            }
        }

        private IEnumerable<StormPlayer> HandleTeamObjectives(AnalyzerResult result)
        {
            foreach (TeamObjective objective in result.TeamObjectives)
            {
                yield return new StormPlayer(objective.Player, result.Start, objective.TimeSpan, SpectateEvent.TeamObjective);
            }
        }

        private IEnumerable<StormPlayer> HandleCamps(AnalyzerResult result)
        {
            foreach ((Player player, TimeSpan time) in result.CampCaptures)
            {
                yield return new StormPlayer(player, result.Start, time, SpectateEvent.Camp);
            }
        }

        private IEnumerable<StormPlayer> HandleKills(AnalyzerResult result, SpectateEvent spectateEvent)
        {
            IEnumerable<IGrouping<Player, Unit>> playerKills = result.Deaths.Where(u => u.PlayerKilledBy != null).GroupBy(unit => unit.PlayerKilledBy).Where(kills => kills.Count() == spectateEvent.ToKills());

            foreach (IGrouping<Player, Unit> players in playerKills)
            {
                Player player = players.Key;
                TimeSpan maxTime = players.Max(unit => unit.TimeSpanDied.Value);
                Hero? hero = replayHelper.TryGetHero(player);

                if (hero != null)
                {
                    if (hero.Name.Equals("abathur", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (Unit death in players)
                        {
                            yield return new StormPlayer(death.PlayerControlledBy, result.Start, death.TimeSpanDied.Value.Add(TimeSpan.FromSeconds(1)), SpectateEvent.Death);
                        }
                    }
                    else if (hero.IsMelee)
                    {
                        yield return new StormPlayer(player, result.Start, maxTime.Add(TimeSpan.FromSeconds(1)), spectateEvent);
                    }
                    else if (hero.IsRanged)
                    {
                        // TODO: calculate how FAR from the death you are. If you are too far, focus the enemy who died, otherwise focus yourself?

                        foreach (Unit death in players)
                        {
                            yield return new StormPlayer(death.PlayerControlledBy, result.Start, death.TimeSpanDied.Value.Add(TimeSpan.FromSeconds(1)), SpectateEvent.Death);
                        }
                    }
                }
                else
                {
                    yield return new StormPlayer(player, result.Start, maxTime.Add(TimeSpan.FromSeconds(2)), spectateEvent);
                }
            }
        }
    }
}
