using System;

namespace StorageOffice.classes.CLI;

public abstract class Select
{
    protected abstract int CurrentIndex { get; set; }
    public IEnumerable<ISelectable> Options => GetOptions();

    protected abstract IEnumerable<ISelectable> GetOptions();
    public abstract void SelectOption();
    public abstract void MoveUp();
    public abstract void MoveDown();
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
        SelectOption();
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
