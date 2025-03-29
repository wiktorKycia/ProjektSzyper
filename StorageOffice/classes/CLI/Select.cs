using System;

namespace StorageOffice.classes.CLI;

public abstract class Select
{
    protected abstract int CurrentIndex { get; set; } 

    public abstract void SelectOption();
    public abstract void MoveUp();
    public abstract void MoveDown();
}

public class RadioSelect(List<RadioOption> options) : Select
// jeśli mamy konstruktor, który nie robi nic poza przypisywaniem wartości do właściwości, to możemy go zapisać tak jak powyżej
{
    public List<RadioOption> Options { get; set; } = options;

    protected override int CurrentIndex { get; set; } = 0;

    public override void SelectOption()
    {
        Options[CurrentIndex].Toggle();
    }
    public override void MoveUp()
    {
        if (CurrentIndex > 0)
        {
            CurrentIndex--;
        }
    }
    public override void MoveDown()
    {
        if (CurrentIndex < Options.Count - 1)
        {
            CurrentIndex++;
        }
    }
}

public class CheckBoxSelect(List<CheckBoxOption> options) : Select
{
    public List<CheckBoxOption> Options { get; set; } = options;

    protected override int CurrentIndex { get; set; } = 0;

    public override void SelectOption()
    {
        Options[CurrentIndex].Toggle();
    }
    private void HighlightOption()
    {
        Options[CurrentIndex].ToggleHighlight();
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
        if (CurrentIndex < Options.Count - 1)
        {
            HighlightOption();
            CurrentIndex++;
            HighlightOption();
        }
    }
}
