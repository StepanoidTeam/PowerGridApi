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
        public int RoundNumber { get; private set; }

        public GameContext GameContext { get; private set; }

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

        public Round(GameContext gameContext)
        {
            GameContext = gameContext;
        }

        private void NextRound()
        {
            foreach (var phase in Phases)
                phase.StartNewRound();
            RoundNumber++;
        }

        public Round Add<T>() where T: Phase
        {
            var phase = (T)Activator.CreateInstance(typeof(T), this);
            Phases.Add(phase);
            return this;
        }

        public Round Start()
        {
            NextRound();
            return this;
        }

    }
}
