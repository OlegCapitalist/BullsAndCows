using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Models;
using BullsAndCows.Application.Services;
using BullsAndCows.Infrastructure.Repositories;
using BullsAndCows.UI.Models;
using PropertyChanged;
using System.Collections.ObjectModel;

namespace BullsAndCows.UI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class RecordsPageViewModel
{
    private readonly IGameRecordRepository _repository;
    private readonly ISettingsService _settingsService;

    private const int _recordsLimit = 10;

    public IReadOnlyList<int> AvailableCodeLengths { get; private set; } = new List<int>();
    public int SelectedCodeLength { get; set; }

    public ObservableCollection<RecordItem> Records { get; private set; } = new();

    public RecordsPageViewModel(IGameRecordRepository repository, ISettingsService settingsService)
    {
        _repository = repository;
        _settingsService = settingsService;
    }

    public async Task InitializeLoadAsync()
    {
        await LoadAvailableCodeLengthsAsync();

        var settings = await _settingsService.GetSettingsAsync();

        AvailableCodeLengths = AvailableCodeLengths
            .Append(settings.CodeLength)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        SelectedCodeLength = settings.CodeLength;

        await LoadRecordsAsync();
    }

    public async Task LoadRecordsAsync()
    {
        var bestRecords = await _repository.GetBestAsync(SelectedCodeLength, _recordsLimit);

        Records.Clear();

        int number = 1;
        foreach (var record in bestRecords)
        {
            Records.Add(new RecordItem(number++, record.AttemptsCount, record.PlayedAt));   
        }

    }

    private async Task LoadAvailableCodeLengthsAsync()
    {
        AvailableCodeLengths = await _repository.GetAvailableCodeLengthsAsync();

    }
}
