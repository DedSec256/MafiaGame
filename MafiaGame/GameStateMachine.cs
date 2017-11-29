using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaGame.DataLayer.Models;
using MafiaGame.Structures;

namespace MafiaGame.Process
{

    public enum GameState
    {
        HelloGame,
        DayDiscussion,
        EveningVoting,
        MafiaStep,
        ManiacStep,
        DoctorStep,
        ComissarStep
    }
    class GameStateMachine
    {

        private CyclicQueue<GameState> stateQueue;
        public event EventHandler<GameState> StateChanged; 

        public GameStateMachine(IEnumerable<Role> roles)
        {
            LinkedList<GameState> existStates = new LinkedList<GameState>();
            existStates.AddLast(GameState.HelloGame);
            existStates.AddLast(GameState.DayDiscussion);
            existStates.AddLast(GameState.EveningVoting);
            existStates.AddLast(GameState.MafiaStep);

            if (roles.Select(t => t is ManiacRole).Any())
            {
                existStates.AddLast(GameState.ManiacStep);
            }
            if (roles.Select(t => t is DoctorRole).Any())
            {
                existStates.AddLast(GameState.DoctorStep);
            }
            if (roles.Select(t => t is ComissarRole).Any())
            {
                existStates.AddLast(GameState.ComissarStep);
            }

            stateQueue = new CyclicQueue<GameState>(existStates);
        }

        public void NextState()
        {
            StateChanged.Invoke(this, stateQueue.GetNext());
        }

        public void Start()
        {
            StateChanged.Invoke(this, stateQueue.Current);
        }

    }
}
