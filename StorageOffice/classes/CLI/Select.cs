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
    public void DisplayOptions(string specialSign = "")
    {
        foreach (Option option in Options)
        {
            option.Display(specialSign);
            Console.WriteLine();
        }
    }
    public virtual void SelectOption()
    {
        Options[CurrentIndex].Toggle();
    }
}

public class RadioSelect : Select
{
    private readonly string specialSign = "\u2022";

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
}

public class CheckBoxSelect : Select
{
    private readonly string specialSign = "\u2713";
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
}

public class Option
{
    public string Text { get; set; }
    public bool IsSelected { get; set; } = false;

    public Option(string text)
    {
        Text = text;
    }
    public void Toggle()
    {
        IsSelected = !IsSelected;
    }

    public void Display(string specialSign)
    {
        if(IsSelected)
        {
            Console.WriteLine($"[{specialSign}] {Text}");
        }
        else
        {
            Console.WriteLine($"[ ] {Text}");
        }
    }
}