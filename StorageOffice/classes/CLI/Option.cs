using System;
namespace StorageOffice.classes.CLI;
public interface ISelectable
{
    public event Action Operation;
    public void InvokeOperation();
    public void AddOperation(Action operation);
    public void RemoveOperation(Action operation);

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
    public event Action Operation;

    public RadioOption(string text, Action operation, char specialSign = '\u2022')
    {
        Text = text;
        SpecialSign = specialSign;
        Operation = operation;
    }

    public void InvokeOperation()
    {
        Operation?.Invoke();
    }
    public void AddOperation(Action operation)
    {
        Operation += operation;
    }
    public void RemoveOperation(Action operation)
    {
        Operation -= operation;
    }

    public void Toggle()
    {
        IsSelected = !IsSelected;
    }
    public override string ToString()
    {
        if (IsSelected)
        {
            return $"[{SpecialSign}] {Text}";
        }
        else
        {
            return $"[ ] {Text}";
        }
    }
}

public class CheckBoxOption: ISelectable, IHighlightable
{
    public string Text { get; set; }
    public bool IsSelected { get; set; } = false;
    public char SpecialSign { get; set; }
    public bool IsHighlighted { get; set; } = false;
    public event Action Operation;

    public CheckBoxOption(string text, Action operation, char specialSign = '\u2713')
    {
        Text = text;
        SpecialSign = specialSign;
        Operation = operation;
    }

    public void InvokeOperation()
    {
        Operation?.Invoke();
    }
    public void AddOperation(Action operation)
    {
        Operation += operation;
    }
    public void RemoveOperation(Action operation)
    {
        Operation -= operation;
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