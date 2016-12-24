using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class StateBatch: State
    {
        public GameContext GameContext { get; private set; }

        private List<State> _doneList { get; set; }

        private Func<GameContext, bool> _restartBatchCondition { get; set; }


        public int RoundNumber { get; private set; }

        public Queue<State> List { get; private set; }

        public State Current { get; private set; }

        public StateBatch CurrentBatch
        {
            get
            {
                if (Current is StateBatch)
                    return Current as StateBatch;
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameContext"></param>
        /// <param name="restartBatchCondition">if null - will not be a cycle (start first state after last finished), 
        /// otherwise will check conditions if need to run round again</param>
        public StateBatch(GameContext gameContext, Func<GameContext, bool> restartBatchCondition = null)
            : base(null)
        {
            GameContext = gameContext;
            _restartBatchCondition = restartBatchCondition;
            List = new Queue<State>();
            _doneList = new List<State>();
        }

        public StateBatch Add<T>() where T : State
        {
            List.PushItem((State)Activator.CreateInstance(typeof(T), this));
            return this;
        }

        /// <summary>
        /// Add another batch to current batch. It means current batch is container for inner batch   
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        public StateBatch Add(StateBatch batch)
        {
            batch._container = this;
            List.PushItem(batch);
            return this;
        }

        //todo do we need to clear last state when Next with it?
        public void Next()
        {
            if (Current == null || Current.IsFinished)
            {
                if (Current != null)
                {
                    _doneList.Add(Current);
                    Current = null;
                }
                if (List.Any())
                {
                    Current = List.Dequeue();
                    Current.Begin();
                }
                else
                {
                    //check if we need to restart batch or just finish it
                    if (_restartBatchCondition != null && _restartBatchCondition(GameContext))
                    {
                        foreach (var state in _doneList)
                            List.PushItem(state);
                        _doneList.Clear();
                        RoundNumber++;

                        Current = List.Dequeue();
                        Current.Begin();
                    }
                    else
                    {
                        IsFinished = true;
                        if (_container != null)
                            _container.Next();
                    }
                }
            }
        }

        /// <summary>
        /// It contains logic for run State contains State Batch
        /// </summary>
        public override void Begin()
        {
            base.Begin();
            StartRound();
        }

        public StateBatch StartRound()
        {
            Next();
            return this;
        }

        public override T RouteAction<T>(UserAction<T> action)
        {
            return Current.RouteAction(action);
        }
    }

}
