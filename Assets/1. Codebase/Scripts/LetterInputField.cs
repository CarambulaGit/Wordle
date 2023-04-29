using System;
using MyBox;
using TMPro;
using UnityEngine;

namespace Codebase.Scripts
{
    public class LetterInputField : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        private string _oldContent;
        public event Action OnLetterEntered;
        public event Action OnLetterRemoved;
        public event Action OnEmptyLetterRemoved;

        public char? GetContent() => inputField.text.Length > 0 ? inputField.text[0] : null;

        public void Initialize()
        {
            Clear();
            Subscribe();
            _oldContent = inputField.text;
        }

        public void Block()
        {
            inputField.interactable = false;
        }

        public void Unblock()
        {
            inputField.interactable = true;
            inputField.ActivateInputField();
        }

        public void Clear(bool notify = true)
        {
            if (notify)
            {
                inputField.text = string.Empty;
            }
            else
            {
                inputField.SetTextWithoutNotify(string.Empty);
                _oldContent = string.Empty;
            }
        }

        public void SetCorrect() => inputField.textComponent.color = Color.green;
        public void SetMisplaced() => inputField.textComponent.color = Color.yellow;

        private void Subscribe() => inputField.onValueChanged.AddListener(CheckForLetter);
        private void Unsubscribe() => inputField.onValueChanged.RemoveListener(CheckForLetter);

        private void CheckForLetter(string newContent)
        {
            if (newContent.Length > 1)
            {
                Debug.LogError($"New content of {this} is not letter, new content = {newContent}");
                return;
            }
            
            
            var oldContent = _oldContent;
            _oldContent = newContent;

            if (newContent.IsNullOrEmpty())
            {
                if (oldContent.IsNullOrEmpty())
                {
                    OnEmptyLetterRemoved?.Invoke();
                }
                else
                {
                    OnLetterRemoved?.Invoke();
                }
            }
            else
            {
                OnLetterEntered?.Invoke();
            }
        }

        public override string ToString()
        {
            var content = GetContent();
            return content == null ? string.Empty : content.ToString();
        }
    }
}