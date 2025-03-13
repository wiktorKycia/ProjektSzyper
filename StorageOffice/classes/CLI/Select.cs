using System;

namespace StorageOffice.classes.CLI;

public class Select
{
    public List<Option> Options { get; set; }
    public int CurrentIndex { get; set; } = 0;
    public Select()
    {
        Options = new List<Option>();
    }
    public Select(List<Option> options)
    {
        Options = options;
    }
    public Select(IEnumerable<string> options)
    {
        Options = new List<Option>();
        foreach (string option in options)
        {
            Options.Add(new Option(option));
        }
    }
    public Select(params string[] options)
    {
        Options = new List<Option>();
        foreach (string option in options)
        {
            Options.Add(new Option(option));
        }
    }

    public void AddOption(string option)
    {
        Options.Add(new Option(option));
    }
    public void AddOption(Option option)
    {
        Options.Add(option);
    }
    public void DisplayOptions(string specialSign)
    {
        foreach (Option option in Options)
        {
            option.Display(specialSign);
        }
    }
    public virtual void SelectOption()
    {
        Options[CurrentIndex].Toggle();
    }
    public virtual void MoveUp()
    {
        if (CurrentIndex > 0)
        {
            CurrentIndex--;
        }
    }
    public virtual void MoveDown()
    {
        if (CurrentIndex < Options.Count - 1)
        {
            CurrentIndex++;
        }
    }
}

public class RadioSelect : Select
{
    private readonly string specialSign = "\u2022";

    public RadioSelect() : base()
    {
        SelectOption();
    }
    public RadioSelect(List<Option> options) : base(options)
    {
        SelectOption();
    }
    public RadioSelect(IEnumerable<string> options) : base(options)
    {
        SelectOption();
    }
    public RadioSelect(params string[] options) : base(options)
    {
        SelectOption();
    }

    public override void SelectOption()
    {
        Options[CurrentIndex].Toggle();
        Options.Where(o => o != Options[CurrentIndex]).ToList().ForEach(o => o.IsSelected = false);
    }
    public void SelectOption(int index)
    {
        if (index < 0 || index >= Options.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }
        Options[index].Toggle();
        Options.Where(o => o != Options[index]).ToList().ForEach(o => o.IsSelected = false);
    }
    public void SelectOption(Option option)
    {
        if (!Options.Contains(option))
        {
            throw new ArgumentException("Option not found in the list.", nameof(option));
        }
        option.Toggle();
        Options.Where(o => o != option).ToList().ForEach(o => o.IsSelected = false);
    }
    public void DisplayOptions()
    {
        base.DisplayOptions(specialSign);
        Console.WriteLine();
    }
    public override void MoveUp()
    {
        base.MoveUp();
        SelectOption();
    }
    public override void MoveDown()
    {
        base.MoveDown();
        SelectOption();
    }
}

public class CheckBoxSelect : Select
{
    private readonly string specialSign = "\u2713";
    public CheckBoxSelect() : base()
    {
        HighlightOption();
    }
    public CheckBoxSelect(List<Option> options) : base(options)
    {
        HighlightOption();
    }
    public CheckBoxSelect(IEnumerable<string> options) : base(options)
    {
        HighlightOption();
    }
    public CheckBoxSelect(params string[] options) : base(options)
    {
        HighlightOption();
    }
    public void HighlightOption()
    {
        Options[CurrentIndex].ToggleHighlight();
    }
    public override void SelectOption()
    {
        Options[CurrentIndex].Toggle();
    }
    public void SelectOption(int index)
    {
        if (index < 0 || index >= Options.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }
        Options[index].Toggle();
    }
    public void SelectOption(Option option)
    {
        if (!Options.Contains(option))
        {
            throw new ArgumentException("Option not found in the list.", nameof(option));
        }
        option.Toggle();
    }
    public void DisplayOptions()
    {
        base.DisplayOptions(specialSign);
        Console.WriteLine();
    }
    public override void MoveUp()
    {
        HighlightOption();
        base.MoveUp();
        HighlightOption();
    }
    public override void MoveDown()
    {
        HighlightOption();
        base.MoveDown();
        HighlightOption();
    }
}

public class Option
{
    public string Text { get; set; }
    public bool IsSelected { get; set; } = false;
    public bool IsHighlighted { get; set; } = false;

    public Option(string text)
    {
        Text = text;
    }
    public void Toggle()
    {
        IsSelected = !IsSelected;
    }
    public void ToggleHighlight()
    {
        IsHighlighted = !IsHighlighted;
    }

    public void Display(string specialSign)
    {
        if(IsHighlighted)
        {
            ConsoleOutput.PrintColorMessage("[", ConsoleColor.Yellow);
        }
        else
        {
            Console.Write("[");
        }

        if(IsSelected)
        {
            Console.Write($"{specialSign}");
        }
        else
        {
            Console.Write(" ");
        }
        
        if(IsHighlighted)
        {
            ConsoleOutput.PrintColorMessage("]", ConsoleColor.Yellow);
        }
        else
        {
            Console.Write("]");
        }
        
        Console.WriteLine($" {Text}");
        
    }
}