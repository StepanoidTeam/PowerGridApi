using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{    
    /// <summary>
    /// Round contais several phases and runs and runs again: after last phase done - start from first one
    /// </summary>
    public class Round
    {
        private GameRoom _gameRoomRef { get; set; }

        private List<Phase> Phases { get; set; }

        public Phase CurrentPhase
        {
            get
            {
                if (Phases.All(m => m.IsFinished))
                {
                    //todo this somewhere else, need to check here also if Game Finished completely - no sense in this case to begin round again
                    NextRound();
                }
                return Phases.FirstOrDefault(m => !m.IsFinished);
            }
        }

        public Round(GameRoom gameRoomRef)
        {
            _gameRoomRef = gameRoomRef;
        }

        private void NextRound()
        {
            foreach (var phase in Phases)
                phase.StartNewRound();
        }

        public void Add(Phase phase)
        {
            phase.Init(_gameRoomRef.Players.Values.Select(m => m.Player));
            Phases.Add(phase);

            NextRound();
        }

    }
}
