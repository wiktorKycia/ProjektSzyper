using System;

namespace StorageOffice.classes.CLI;

public class Select
{

}

public class Option
{
    public string Text { get; set; }
    public bool IsSelected { get; set; } = false;

    public Option(string text)
    {
        Text = text;
    }
}