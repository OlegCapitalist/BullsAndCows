using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace BullsAndCows.UI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class GamePageViewModel 
{
    public string SecretNumber { get; set; } = "";

    public ICommand AddDigitCommand => new Command<string>(AddDigit);

    public ICommand BackspaceCommand => new Command(Backspace);

    public ICommand ClearCommand => new Command(Clear);

    public ICommand CheckGuessCommand => new Command(CheckGuess);

    public GamePageViewModel()
    {

    }

    private void AddDigit(string num)
    {
        if (string.IsNullOrEmpty(num) || SecretNumber.Length >= 4)
            return;

        SecretNumber += num;
    }

    private void Backspace()
    {
        if (SecretNumber.Length > 0)
        {
            SecretNumber = SecretNumber.Substring(0, SecretNumber.Length - 1);
        }
    }

    private void Clear()
    {
        SecretNumber = "";
    }

    private void CheckGuess()
    {
        // Implementation for checking the guess
    }
}
