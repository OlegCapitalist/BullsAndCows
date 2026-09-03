namespace BullsAndCows.UI.Behaviors;

public class NumericEntryBehavior : Behavior<Entry>
{
    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);

        entry.TextChanged += OnTextChanged;
    }

    private void OnTextChanged(
        object? sender,
        TextChangedEventArgs e)
    {
        if (sender is not Entry entry)
            return;

        if (string.IsNullOrEmpty(e.NewTextValue))
            return;

        var filtered = new string(
            e.NewTextValue
                .Where(char.IsDigit)
                .ToArray());

        if (entry.Text != filtered)
            entry.Text = filtered;
    }

}
