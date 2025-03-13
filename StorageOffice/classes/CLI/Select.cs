using System;

namespace StorageOffice.classes.CLI;

public class Select
{
    public List<Option> Options { get; set; } = new List<Option>();
}

public class Option
{
    public string Text { get; set; }
    public bool IsSelected { get; set; } = false;

    public Option(string text)
    {
        Text = text;
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