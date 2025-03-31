using System;
namespace StorageOffice.classes.CLI;
internal interface ISelectable
{
    public void Toggle();
    public string ToString();
}

internal interface IHighlightable
{
    public void ToggleHighlight();
    public string ToString();
}

public class RadioOption: ISelectable
{
    public string Text { get; set; }
    public bool IsSelected { get; set; } = false;
    public char SpecialSign { get; set; }
    

    public RadioOption(string text, char specialSign = '\u2022')
    {
        Text = text;
        SpecialSign = specialSign;
    }
    public void Toggle()
    {
        IsSelected = !IsSelected;
    }
    public override string ToString()
    {
        return $"[{SpecialSign}] {Text}";
    }
}

public class CheckBoxOption: ISelectable, IHighlightable
{
    public string Text { get; set; }
    public bool IsSelected { get; set; } = false;
    public char SpecialSign { get; set; }
    public bool IsHighlighted { get; set; } = false;

    public CheckBoxOption(string text, char specialSign = '\u2713')
    {
        Text = text;
        SpecialSign = specialSign;
    }
    public void Toggle()
    {
        IsSelected = !IsSelected;
    }
    public void ToggleHighlight()
    {
        IsHighlighted = !IsHighlighted;
    }
    public override string ToString()
    {
        return $"[{SpecialSign}] {Text}";
    }
}