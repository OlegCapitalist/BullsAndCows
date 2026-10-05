using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Services;
using BullsAndCows.Infrastructure.Repositories;
using BullsAndCows.UI.Models;
using PropertyChanged;

namespace BullsAndCows.UI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class RecordsPageViewModel
{
    private readonly IGameRecordRepository _repository;
    private readonly ISettingsService _settingsService;

    private int _codeLengthFilter;
    private const int _recordsLimit = 10;
    public RecordsPageViewModel(IGameRecordRepository repository, ISettingsService settingsService)
    {
        _repository = repository;
        _settingsService = settingsService;
    }

    public async Task LoadRecordsAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();

        _codeLengthFilter = settings.CodeLength;

        var bestRecords = await _repository.GetBestAsync(_codeLengthFilter, _recordsLimit);
    }
}
