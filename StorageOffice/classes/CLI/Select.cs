using System;

namespace StorageOffice.classes.CLI;

public class Select
{
    public List<Option> Options { get; set; }
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
            Console.Write($"[{specialSign}] {Text}");
        }
        else
        {
            Console.Write($"[ ] {Text}");
        }
    }
}