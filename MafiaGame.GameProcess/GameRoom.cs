using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaGame.DataLayer.Models;
using MafiaGame.Process;

namespace MafiaGame
{
    class GameRoom
    {
        private GameStateMachine gameStateMachine;
        private IEnumerable<User> _players;
        public GameRoom(IEnumerable<User> players)
        {
            //_players = players;
            //   gameStateMachine = new GameStateMachine(
             //      new GamePreprocessor(players).InitializeRoles());
            gameStateMachine.StateChanged += GameStateMachine_StateChanged;
               
        }

        private void GameStateMachine_StateChanged(object sender, GameState e)
        {
            switch (e)
            {
                case GameState.HelloGame:
                    HelloGame();
                    break;
                case GameState.DayDiscussion:
                    DayDiscussion();
                    break;
                case GameState.EveningVoting:
                    EveningVoting();
                    break;
            }
        }

        private void EveningVoting()
        {
            throw new NotImplementedException();
        }

        private void DayDiscussion()
        {
            
        }

        public void HelloGame()
        {
            //TODO: broadcast to all hello
        }
    }
}
