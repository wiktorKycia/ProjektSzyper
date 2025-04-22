using System;

namespace StorageOffice.classes.CLI;

/// <summary>
/// Abstract base class for selection UI components that manage selectable options.
/// </summary>
public abstract class Select
{
    /// <summary>
    /// Gets or sets the index of the currently selected option.
    /// </summary>
    protected abstract int CurrentIndex { get; set; }

    /// <summary>
    /// Gets the collection of selectable options available in this selection component.
    /// </summary>
    public IEnumerable<ISelectable> Options => GetOptions();

    /// <summary>
    /// Gets the collection of selectable options for this select component.
    /// </summary>
    /// <returns>A collection of ISelectable objects representing the available options.</returns>
    protected abstract IEnumerable<ISelectable> GetOptions();

    /// <summary>
    /// Selects the currently highlighted option.
    /// </summary>    
    public abstract void SelectOption();

    /// <summary>
    /// Moves the selection highlight up one option in the list.
    /// </summary>
    public abstract void MoveUp();

    /// <summary>
    /// Moves the selection highlight down one option in the list.
    /// </summary>
    public abstract void MoveDown();

    /// <summary>
    /// Invokes the operation associated with the currently selected option.
    /// </summary>
    public abstract void InvokeOperation();
}

public class RadioSelect : Select
{
    public List<RadioOption> RadioOptions { get; set; }

    protected override int CurrentIndex { get; set; }

    public RadioSelect(List<RadioOption> options)
    {
        RadioOptions = options;
        CurrentIndex = 0;
        SelectOption();
    }

    protected override IEnumerable<ISelectable> GetOptions()
    {
        return RadioOptions.Cast<ISelectable>().ToList();
    }

    public override void InvokeOperation()
    {
        RadioOptions[CurrentIndex].InvokeOperation();
    }

    public override void SelectOption()
    {
        RadioOptions[CurrentIndex].Toggle();
    }
    public override void MoveUp()
    {
        if (CurrentIndex > 0)
        {
            SelectOption();
            CurrentIndex--;
            SelectOption();
        }
        
    }
    public override void MoveDown()
    {
        if (CurrentIndex < RadioOptions.Count - 1)
        {
            SelectOption();
            CurrentIndex++;
            SelectOption();
        }
        
    }
}

public class CheckBoxSelect : Select
{
    public List<CheckBoxOption> CheckBoxOptions { get; set; }

    protected override int CurrentIndex { get; set; }

    public CheckBoxSelect(List<CheckBoxOption> options)
    {
        CheckBoxOptions = options;
        CurrentIndex = 0;
        HighlightOption();
    }

    protected override IEnumerable<ISelectable> GetOptions()
    {
        return CheckBoxOptions.Cast<ISelectable>().ToList();
    }

    public override void InvokeOperation()
    {
        foreach (var option in CheckBoxOptions.Where(o => o.IsSelected))
        {
            option.InvokeOperation();
        }
    }

    public override void SelectOption()
    {
        CheckBoxOptions[CurrentIndex].Toggle();
    }
    private void HighlightOption()
    {
        CheckBoxOptions[CurrentIndex].ToggleHighlight();
    }
    public override void MoveUp()
    {
        if (CurrentIndex > 0)
        {
            HighlightOption();
            CurrentIndex--;
            HighlightOption();
        }
    }
    public override void MoveDown()
    {
        if (CurrentIndex < CheckBoxOptions.Count - 1)
        {
            HighlightOption();
            CurrentIndex++;
            HighlightOption();
        }
    }
}
