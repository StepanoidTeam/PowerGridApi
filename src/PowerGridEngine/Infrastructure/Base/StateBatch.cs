using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class StateBatch
    {
        public GameContext GameContext { get; private set; }

        private List<State> _doneList { get; set; }

        private Func<GameContext, bool> _restartBatchCondition { get; set; }


        public int RoundNumber { get; private set; }

        public Queue<State> List { get; private set; }

        public State Current { get; private set; }

        public bool IsFinished { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameContext"></param>
        /// <param name="restartBatchCondition">if null - will not be a cycle (start first state after last finished), 
        /// otherwise will check conditions if need to run round again</param>
        public StateBatch(GameContext gameContext, Func<GameContext, bool> restartBatchCondition = null)
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

        //todo do we need to clear last state when Next with it?
        public void Next()
        {
            if (Current == null || Current.IsFinished)
            {
                _doneList.Add(Current);
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
                        IsFinished = true;
                }
            }
        }

        public StateBatch StartRound()
        {
            Next();
            return this;
        }
    }

}
