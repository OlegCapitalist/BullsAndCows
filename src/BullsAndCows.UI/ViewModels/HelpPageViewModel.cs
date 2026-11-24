using BullsAndCows.UI.Interfaces;
using BullsAndCows.UI.Models;
using PropertyChanged;

namespace BullsAndCows.UI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class HelpPageViewModel
{
    private readonly ILocalizationService _localizationService = null!;

    public string HelpText { get; private set; } = string.Empty;
    public bool IsHelpTextVisible { get; private set; } = false;
    public bool IsLoadingErrorVisible { get; private set; } = true;


    public HelpPageViewModel(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task LoadRulesAsync()
    {
        var fileName = _localizationService.CurrentLanguage switch
        {
            Languages.Russian => "Help/rules.ru.txt",
            Languages.English => "Help/rules.en.txt",
            Languages.QazaqCyrillic => "Help/rules.kk.txt",
            Languages.QazaqLatin => "Help/rules.kk-latin.txt",
            _ => "Help/rules.ru.txt"
        };

        try
        {
            await using var stream =
            await FileSystem.OpenAppPackageFileAsync(fileName);

            using var reader = new StreamReader(stream);
            HelpText = await reader.ReadToEndAsync();

            IsHelpTextVisible = true;
            IsLoadingErrorVisible = false;
        }
        catch 
        {
            IsHelpTextVisible = false;
            IsLoadingErrorVisible = true;
        }
    }
}
