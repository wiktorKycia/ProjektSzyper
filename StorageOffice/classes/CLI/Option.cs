using System;
namespace StorageOffice.classes.CLI;
/// <summary>
/// Interface representing a selectable menu item or option that can trigger operations.
/// </summary>
public interface ISelectable
{
    /// <summary>
    /// Event that gets triggered when the option's operation is invoked.
    /// </summary>
    public event Action Operation;

    /// <summary>
    /// Invokes the operation associated with this option.
    /// </summary>
    public void InvokeOperation();

    /// <summary>
    /// Adds an operation to the option's event handler.
    /// </summary>
    /// <param name="operation">The action to add to the operation event.</param>
    public void AddOperation(Action operation);

    /// <summary>
    /// Removes an operation from the option's event handler.
    /// </summary>
    /// <param name="operation">The action to remove from the operation event.</param>
    public void RemoveOperation(Action operation);

    /// <summary>
    /// Toggles the selection state of the option.
    /// </summary>
    public void Toggle();

    /// <summary>
    /// Returns a string representation of the option.
    /// </summary>
    /// <returns>A string representation of the option.</returns>
    public string ToString();
}

/// <summary>
/// Interface for elements that can be highlighted in a UI context.
/// </summary>
internal interface IHighlightable
{
    /// <summary>
    /// Toggles the highlight state of the element.
    /// </summary>
    public void ToggleHighlight();

    /// <summary>
    /// Returns a string representation of the highlightable element.
    /// </summary>
    /// <returns>A string representation of the highlightable element.</returns>
    public string ToString();
}

/// <summary>
/// Represents a radio-button style option that can be selected exclusively.
/// </summary>
public class RadioOption: ISelectable
{
    public string Text { get; set; }
    public bool IsSelected { get; set; } = false;
    public char SpecialSign { get; set; }
    public event Action Operation;

    /// <param name="text">The display text of the option.</param>
    /// <param name="operation">The action to execute when the option is selected.</param>
    /// <param name="specialSign">The character used to indicate selection (defaults to a bullet point).</param>
    public RadioOption(string text, Action operation, char specialSign = '\u2022')
    {
        Text = text;
        SpecialSign = specialSign;
        Operation = operation;
    }

    public void InvokeOperation() => Operation?.Invoke();
    public void AddOperation(Action operation) => Operation += operation;
    public void RemoveOperation(Action operation) => Operation -= operation;

    public void Toggle() => IsSelected = !IsSelected;
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

/// <summary>
/// Represents a checkbox style option that can be selected or deselected.
/// </summary>
public class CheckBoxOption: ISelectable, IHighlightable
{
    public string Text { get; set; }
    public bool IsSelected { get; set; } = false;
    public char SpecialSign { get; set; }
    public bool IsHighlighted { get; set; } = false;
    public event Action Operation;

    /// <param name="text">The display text of the option.</param>
    /// <param name="operation">The action to execute when the option is selected.</param>
    /// <param name="specialSign">The character used to indicate selection (defaults to a check mark).</param>
    public CheckBoxOption(string text, Action operation, char specialSign = '\u2713')
    {
        Text = text;
        SpecialSign = specialSign;
        Operation = operation;
    }

    public void InvokeOperation() => Operation?.Invoke();
    public void AddOperation(Action operation) => Operation += operation;
    public void RemoveOperation(Action operation) => Operation -= operation;

    public void Toggle() => IsSelected = !IsSelected;
    public void ToggleHighlight() => IsHighlighted = !IsHighlighted;
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