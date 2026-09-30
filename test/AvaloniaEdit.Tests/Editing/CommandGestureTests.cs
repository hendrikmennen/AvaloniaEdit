using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using AvaloniaEdit.AvaloniaMocks;
using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace AvaloniaEdit.Editing;

[TestFixture]
public class CommandGestureTests
{
    [AvaloniaTest]
    public void Default_Gesture_Executes_Command()
    {
        var (window, textEditor) = CreateEditor("first\nsecond");

        window.KeyPress(Key.D, RawInputModifiers.Control, PhysicalKey.D, "d");

        Assert.AreEqual("second", textEditor.Text);
    }

    [AvaloniaTest]
    public void Cleared_Gesture_Does_Not_Execute_Command()
    {
        var original = AvaloniaEditCommands.DeleteLine.Gesture;
        try
        {
            AvaloniaEditCommands.DeleteLine.Gesture = null;
            var (window, textEditor) = CreateEditor("first\nsecond");

            window.KeyPress(Key.D, RawInputModifiers.Control, PhysicalKey.D, "d");

            Assert.AreEqual("first\nsecond", textEditor.Text);
        }
        finally
        {
            AvaloniaEditCommands.DeleteLine.Gesture = original;
        }
    }

    [AvaloniaTest]
    public void Changed_Gesture_Executes_Command()
    {
        var original = AvaloniaEditCommands.DeleteLine.Gesture;
        try
        {
            AvaloniaEditCommands.DeleteLine.Gesture = new KeyGesture(Key.E, KeyModifiers.Control);
            var (window, textEditor) = CreateEditor("first\nsecond");

            window.KeyPress(Key.D, RawInputModifiers.Control, PhysicalKey.D, "d");
            Assert.AreEqual("first\nsecond", textEditor.Text);

            window.KeyPress(Key.E, RawInputModifiers.Control, PhysicalKey.E, "e");
            Assert.AreEqual("second", textEditor.Text);
        }
        finally
        {
            AvaloniaEditCommands.DeleteLine.Gesture = original;
        }
    }

    private static (Window, TextEditor) CreateEditor(string text)
    {
        UnitTestApplication.InitializeStyles();

        var textEditor = new TextEditor { Text = text };
        var window = new Window { Content = textEditor };

        window.Show();
        textEditor.ApplyTemplate();
        textEditor.TextArea.Focus();
        textEditor.CaretOffset = 0;

        return (window, textEditor);
    }
}
