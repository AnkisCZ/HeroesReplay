﻿using HeroesReplay.Analyzer;
using HeroesReplay.Shared;

namespace HeroesReplay.Spectator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Heroes.ReplayParser;

    public class StormReplayHeroSelector
    {
        public List<StormPlayer> Select(AnalyzerResult result, GameCriteria gameCriteria)
        {
            List<StormPlayer> players = new List<StormPlayer>();

            players.AddRange(gameCriteria switch
            {
                GameCriteria.Alive => HandleAlive(result),
                GameCriteria.Ping => HandlePings(result),
                GameCriteria.Structure => HandleStructures(result),
                GameCriteria.MapObjective => HandleMapObjectives(result),
                GameCriteria.TeamObjective => HandleTeamObjectives(result),
                GameCriteria.CampObjective => HandleCampObjectives(result),
                GameCriteria.Death => HandleDeaths(result),
                GameCriteria.Kill => HandleKills(result, 1),
                GameCriteria.MultiKill => HandleKills(result, 2),
                GameCriteria.TripleKill => HandleKills(result, 3),
                GameCriteria.QuadKill => HandleKills(result, 4),
                GameCriteria.PentaKill => HandleKills(result, 5),
                GameCriteria.Any => HandleAny(result),
            });

            players.Sort((playerA, playerB) => playerA.When.CompareTo(playerB.When));

            return players;
        }

        private IEnumerable<StormPlayer> HandleKills(AnalyzerResult result, int count)
        {
            IEnumerable<IGrouping<Player, Unit>> playerKills = result.PlayerDeaths.GroupBy(unit => unit.PlayerKilledBy).Where(kills => kills.Count() == count);

            foreach (IGrouping<Player, Unit> players in playerKills)
            {
                Player player = players.Key;
                int kills = players.Count();
                TimeSpan maxTime = players.Max(unit => unit.TimeSpanDied.Value);

                yield return new StormPlayer(player, maxTime, count switch { 1 => GameCriteria.Kill, 2 => GameCriteria.MultiKill, 3 => GameCriteria.TripleKill, 4 => GameCriteria.QuadKill, 5 => GameCriteria.PentaKill });

                //Hero? hero = player.TryGetHero();
                //if (hero != null)
                //{
                // 
                //}
                //else
                //{
                //}
            }
        }

        private IEnumerable<StormPlayer> HandleAny(AnalyzerResult result)
        {
            return result.PlayerDeaths.Any() ? HandleDeaths(result) :
                result.MapObjectives.Any() ? HandleMapObjectives(result) :
                result.TeamObjectives.Any() ? HandleTeamObjectives(result) :
                result.Structures.Any() ? HandleStructures(result) :
                result.PingSources.Any() ? HandlePings(result) :
                result.PlayersAlive.Any() ? HandleAlive(result) : Enumerable.Empty<StormPlayer>();
        }

        // Ping events are only from the team which the replay file originates from
        private IEnumerable<StormPlayer> HandlePings(AnalyzerResult result)
        {
            foreach (GameEvent ping in result.PingSources)
            {
                yield return new StormPlayer(ping.player, ping.TimeSpan, GameCriteria.Ping);
            }
        }

        private IEnumerable<StormPlayer> HandleMapObjectives(AnalyzerResult result)
        {
            foreach (Unit unit in result.MapObjectives)
            {
                yield return new StormPlayer(unit.PlayerKilledBy ?? unit.PlayerControlledBy, unit.TimeSpanDied.Value, GameCriteria.MapObjective);
            }
        }

        private IEnumerable<StormPlayer> HandleDeaths(AnalyzerResult result)
        {
            foreach (Unit unit in result.PlayerDeaths.OrderByDeath())
            {
                Hero hero = unit.PlayerKilledBy.TryGetHero();

                if (hero != null)
                {
                    Player player = hero.IsMelee ? unit.PlayerKilledBy : unit.PlayerControlledBy;

                    yield return new StormPlayer(player, unit.TimeSpanDied.Value, GameCriteria.Death);
                }
                else
                {
                    yield return new StormPlayer(unit.PlayerControlledBy, unit.TimeSpanDied.Value, GameCriteria.Death);
                }
            }
        }

        private IEnumerable<StormPlayer> HandleStructures(AnalyzerResult result)
        {
            foreach (Unit unit in result.Structures)
            {
                yield return new StormPlayer(unit.PlayerKilledBy, unit.TimeSpanDied.Value, GameCriteria.Structure);
            }
        }

        private IEnumerable<StormPlayer> HandleAlive(AnalyzerResult result)
        {
            //IEnumerable<IGrouping<int, Player>> teams = result.PlayersAlive.Shuffle().GroupBy(p => p.Team).ToList();
            //IEnumerable<Player> equalDistribution = teams.First().Interleave(teams.Last()).ToList();

            // Interleave between team 1 and team 2 players 
            foreach (Player player in result.PlayersAlive)
            {
                yield return new StormPlayer(player, result.Duration, GameCriteria.Alive);
            }
        }

        private IEnumerable<StormPlayer> HandleTeamObjectives(AnalyzerResult result)
        {
            foreach (TeamObjective objective in result.TeamObjectives)
            {
                yield return new StormPlayer(objective.Player, objective.TimeSpan, GameCriteria.TeamObjective);
            }
        }

        /// <summary>
        /// Standard camps are not captured in TeamObjectives
        /// </summary>
        /// <remarks>
        /// https://github.com/barrett777/Heroes.ReplayParser/blob/2d29bf2f66bfd44c471a4214698e6b517d38ecd3/Heroes.ReplayParser/Statistics.cs#L343
        /// </remarks>
        private IEnumerable<StormPlayer> HandleCampObjectives(AnalyzerResult result)
        {
            foreach ((Player player, TimeSpan capture) in result.Camps)
            {
                yield return new StormPlayer(player, capture, GameCriteria.CampObjective);
            }
        }
    }
}