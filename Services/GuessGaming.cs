using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class GuessingGame
    {
        private int _answer;
        public bool _isGameOver;

        public GuessingGame()
        {
            var random = new Random();
            _answer = random.Next(1, 101); // Número aleatório entre 1 e 100
            _isGameOver = false;
        }

        public string Guess(int number)
        {
            if (_isGameOver)
            {
                return "O jogo já terminou. Inicie um novo jogo com o comando !startgame.";
            }

            if (number < 1 || number > 100)
            {
                return "Por favor, escolha um número entre 1 e 100.";
            }

            if (number == _answer)
            {
                _isGameOver = true;
                return "Parabéns! Você acertou o número. O jogo foi encerrado.";
            }

            return number < _answer ? "Tente um número maior." : "Tente um número menor.";
        }

        public void StartNewGame()
        {
            _answer = new Random().Next(1, 101);
            _isGameOver = false;
        }
    }
}