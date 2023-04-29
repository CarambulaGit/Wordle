using System;
using System.Collections.Generic;
using System.Linq;
using Codebase.Infrastructure;
using HtmlAgilityPack;
using MyBox;
using UnityEngine;

namespace Codebase.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public const int WordsLength = 6;
        [SerializeField] private GameField gameField;
        [SerializeField, ReadOnly] private string currentWord;
        public Dictionary<char, string[]> Words { get; private set; }

        public string CurrentWord => currentWord;

        private void Awake()
        {
            Words = GetWords();
            gameField.Initialize(this);
            StartGame();
        }

        [ButtonMethod]
        public void StartGame()
        {
            currentWord = SelectRandomWord();
            gameField.SpawnWordsInputText();
        }

        private Dictionary<char, string[]> GetWords()
        {
            var url = "https://eslforums.com/6-letter-words/#6_Letter_Words_Infographic";
            var web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            var node = doc.DocumentNode.SelectSingleNode(
                "//article[@class='article-post article post-4667 post type-post status-publish format-standard has-post" +
                "-thumbnail hentry category-vocabulary tag-6-letter-words tag-6-letter-words-starting-with-a tag-6-letter" +
                "-words-starting-with-b tag-6-letter-words-starting-with-c tag-6-letter-words-starting-with-d tag-6-letter" +
                "-words-starting-with-e tag-6-letter-words-starting-with-f tag-6-letter-words-starting-with-g tag-6-letter" +
                "-words-starting-with-h tag-6-letter-words-starting-with-i tag-6-letter-words-starting-with-k tag-6-letter" +
                "-words-starting-with-l tag-6-letter-words-starting-with-m tag-6-letter-words-starting-with-n tag-6-letter" +
                "-words-starting-with-o tag-6-letter-words-starting-with-p tag-6-letter-words-starting-with-q tag-6-letter" +
                "-words-starting-with-r tag-6-letter-words-starting-with-s tag-6-letter-words-starting-with-t tag-6-letter" +
                "-words-starting-with-u tag-6-letter-words-starting-with-v tag-6-letter-words-starting-with-y tag-find-6-letter" +
                "-words-with-these-letters grow-content-body']");
            return node.SelectNodes(".//ul").ToArray()[3..]
                .Select(node => node.InnerText.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(words => words[0][0]);
        }

        private string SelectRandomWord() => Words.GetRandomValue();
    }
}