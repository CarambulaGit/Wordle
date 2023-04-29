using System;
using System.Collections.Generic;
using Codebase.Infrastructure;
using MyBox;
using UnityEngine;

namespace Codebase.Scripts
{
    public class GameField : MonoBehaviour
    {
        [SerializeField] private int numOfWords = 7;
        [SerializeField] private Transform wordsParent;
        [SerializeField] private WordInputField wordPrefab;
        private GameManager _gameManager;
        private readonly List<WordInputField> _words = new List<WordInputField>();

        private int CurrentWordIndex => _words.FindIndex(word => word.GetContent().IsNullOrEmpty());

        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void SpawnWordsInputText()
        {
            for (var i = 0; i < numOfWords; i++)
            {
                _words.Add(SpawnWord());
            }

            _words[0].UnblockLetter();
        }

        private WordInputField SpawnWord()
        {
            var word = Instantiate(wordPrefab, wordsParent);
            word.Initialize();
            word.Block();
            word.OnWordEntered += OnWordEntered;
            return word;
        }

        private void OnWordEntered()
        {
            var nextWordIndex = CurrentWordIndex;
            var prevIndex = nextWordIndex == -1 ? _words.Count - 1 : nextWordIndex - 1;
            var wordExists = CheckWord(prevIndex, out var isWin);
            if (!wordExists)
            {
                Debug.Log($"Word {_words[prevIndex]} doesn't exist");
                _words[prevIndex].Clear(false);
                _words[prevIndex].UnblockLetter();
                return;
            }

            if (isWin)
            {
                _words[prevIndex].Block();
                Win();
                return;
            }

            if (nextWordIndex != -1)
            {
                _words[prevIndex].Block();
                _words[nextWordIndex].UnblockLetter();
            }
            else
            {
                _words[prevIndex].Block();
                GameOver();
            }
        }

        private bool CheckWord(int wordIndex, out bool isWin)
        {
            isWin = false;
            var word = _words[wordIndex];
            var wordContent = word.GetContent();
            if (wordContent.Equals(_gameManager.CurrentWord, StringComparison.CurrentCultureIgnoreCase))
            {
                word.SetCorrect();
                isWin = true;
                return true;
            }

            if (!_gameManager.Words.Contains(wordContent))
                return false;

            for (var i = 0; i < wordContent.Length; i++)
            {
                if (wordContent[i].Equals(_gameManager.CurrentWord[i], Extensions.CharComparisonType.IgnoreCase))
                {
                    word.SetCorrect(i);
                }
                else if (_gameManager.CurrentWord.Contains(wordContent[i], StringComparison.CurrentCultureIgnoreCase))
                {
                    word.SetMisplaced(i);
                }
            }

            return true;
        }

        private void Win()
        {
            Debug.Log("Win");
        }

        private void GameOver()
        {
            Debug.Log("Game over");
        }
    }
}