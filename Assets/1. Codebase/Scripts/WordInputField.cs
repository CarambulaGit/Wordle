using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Codebase.Scripts
{
    public class WordInputField : MonoBehaviour
    {
        [SerializeField] private LetterInputField letterPrefab;
        private readonly List<LetterInputField> _letters = new List<LetterInputField>();
        public event Action OnWordEntered;

        public string GetContent() => string.Join("", _letters.Select(letter => letter.GetContent()));
        private int CurrentLetterIndex => _letters.FindIndex(letter => letter.GetContent() == null);
        private LetterInputField CurrentLetter => _letters.FirstOrDefault(letter => letter.GetContent() == null);

        public void Initialize()
        {
            for (var i = 0; i < GameManager.WordsLength; i++)
            {
                var letter = SpawnLetter();
                _letters.Add(letter);
                letter.OnLetterEntered += OnLetterEntered;
                letter.OnLetterRemoved += OnLetterRemoved;
                letter.OnEmptyLetterRemoved += OnEmptyLetterRemoved;
            }
        }

        public void Block() => _letters.ForEach(letter => letter.Block());

        public void UnblockLetter()
        {
            var curLetterIndex = CurrentLetterIndex;
            if (curLetterIndex == -1)
            {
                Debug.LogError($"Can't unblock letter in {this}");
                return;
            }

            UnblockLetter(curLetterIndex);
        }

        public void Clear(bool notify = true) => _letters.ForEach(letter => letter.Clear(notify));

        public void SetCorrect() => _letters.ForEach(letter => letter.SetCorrect());
        public void SetCorrect(int letterIndex) => _letters[letterIndex].SetCorrect();
        public void SetMisplaced(int letterIndex) => _letters[letterIndex].SetMisplaced();

        private void OnLetterEntered()
        {
            var nextLetterIndex = CurrentLetterIndex;
            if (nextLetterIndex != -1)
            {
                _letters[nextLetterIndex - 1].Block();
                UnblockLetter(nextLetterIndex);
            }
            else
            {
                _letters[^1].Block();
                OnWordEntered?.Invoke();
            }
        }

        private void OnLetterRemoved()
        {
            var prevLetterIndex = CurrentLetterIndex;
            if (prevLetterIndex != -1)
            {
                _letters[prevLetterIndex + 1].Block();
                UnblockLetter(prevLetterIndex);
            }
        }

        private void OnEmptyLetterRemoved()
        {
            var curLetterIndex = CurrentLetterIndex;
            if (curLetterIndex != 0) _letters[curLetterIndex - 1].Clear();
        }

        private void UnblockLetter(int index) => _letters[index].Unblock();

        private LetterInputField SpawnLetter()
        {
            var letter = Instantiate(letterPrefab, transform);
            letter.Initialize();
            return letter;
        }

        public override string ToString() => GetContent();
    }
}