using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class GameStages
    {
        public GameContext GameContext { get; private set; }

        public Queue<Stage> List { get; private set; }

        public Stage CurrentStage { get; private set; }

        public bool IsFinished { get; private set; }

        public GameStages(GameContext gameContext)
        {
            GameContext = gameContext;
            List = new Queue<Stage>();
        }

        public GameStages AddStage<T>() where T : Stage
        {
            List.PushItem((Stage)Activator.CreateInstance(typeof(T), this));
            return this;
        }

        //todo do we need to clear last stage when Next with it?
        public void Next()
        {
            if (CurrentStage == null || CurrentStage.IsFinished)
            {
                if (List.Any())
                    CurrentStage = List.Dequeue();
                else
                    IsFinished = true;
            }
        }

        public GameStages Start()
        {
            Next();
            return this;
        }
    }

}
