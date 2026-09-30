using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using AvaloniaEdit.AvaloniaMocks;
using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace AvaloniaEdit.Search;

[TestFixture]
public class SearchPanelTests
{
    [AvaloniaTest]
    public void Find_Next_Should_Find_Next_Occurence()
    {
        UnitTestApplication.InitializeStyles();

        TextEditor textEditor = CreateEditor();
        textEditor.Text = "hello world";

        textEditor.SearchPanel.SearchPattern = "world";
        textEditor.SearchPanel.Open();
        textEditor.SearchPanel.FindNext();

        Assert.AreEqual(6, textEditor.SelectionStart);
        Assert.AreEqual(5, textEditor.SelectionLength);
    }

    [AvaloniaTest]
    public void Find_Next_After_Find_Next_Should_Find_The_Occurence()
    {
        UnitTestApplication.InitializeStyles();

        TextEditor textEditor = CreateEditor();
        textEditor.Text = "hello world world";

        textEditor.SearchPanel.SearchPattern = "world";
        textEditor.SearchPanel.Open();
        textEditor.SearchPanel.FindNext();
        textEditor.SearchPanel.FindNext();

        Assert.AreEqual(12, textEditor.SelectionStart);
        Assert.AreEqual(5, textEditor.SelectionLength);
    }

    [AvaloniaTest]
    public void Find_Previous_Should_Find_Previous_Occurence()
    {
        UnitTestApplication.InitializeStyles();

        TextEditor textEditor = CreateEditor();
        textEditor.Text = "hello world";

        textEditor.CaretOffset = 6;

        textEditor.SearchPanel.SearchPattern = "hello";
        textEditor.SearchPanel.Open();
        textEditor.SearchPanel.FindPrevious();

        Assert.AreEqual(0, textEditor.SelectionStart);
        Assert.AreEqual(5, textEditor.SelectionLength);
    }

    [AvaloniaTest]
    public void Find_Should_Select_The_First_Result_After_Caret()
    {
        UnitTestApplication.InitializeStyles();

        TextEditor textEditor = CreateEditor();
        textEditor.Text = "hello world world";

        textEditor.CaretOffset = 6;

        textEditor.SearchPanel.SearchPattern = "world";
        textEditor.SearchPanel.Open();
        textEditor.SearchPanel.FindNext();

        Assert.AreEqual(6, textEditor.SelectionStart);
        Assert.AreEqual(5, textEditor.SelectionLength);
    }

    [AvaloniaTest]
    public void Find_Should_Select_First_Result_When_There_Are_No_Results_After_Caret()
    {
        UnitTestApplication.InitializeStyles();

        TextEditor textEditor = CreateEditor();
        textEditor.Text = "hello world lovely";

        textEditor.CaretOffset = 12;

        textEditor.SearchPanel.SearchPattern = "world";
        textEditor.SearchPanel.Open();
        textEditor.SearchPanel.FindNext();

        Assert.AreEqual(6, textEditor.SelectionStart);
        Assert.AreEqual(5, textEditor.SelectionLength);

    }

    [AvaloniaTest]
    public void Find_Previous_After_Find_Previous_Should_Find_Occurence()
    {
        UnitTestApplication.InitializeStyles();

        TextEditor textEditor = CreateEditor();
        textEditor.Text = "hello world world";

        textEditor.CaretOffset = 17;

        textEditor.SearchPanel.SearchPattern = "world";
        textEditor.SearchPanel.Open();
        textEditor.SearchPanel.FindPrevious();
        textEditor.SearchPanel.FindPrevious();

        Assert.AreEqual(6, textEditor.SelectionStart);
        Assert.AreEqual(5, textEditor.SelectionLength);
    }

    [AvaloniaTest]
    public void Replace_Next_Should_Replace_Occurence()
    {
        UnitTestApplication.InitializeStyles();

        TextEditor textEditor = CreateEditor();
        textEditor.Text = "hello world";

        textEditor.CaretOffset = 6;

        textEditor.SearchPanel.SearchPattern = "world";
        textEditor.SearchPanel.ReplacePattern = "universe";
        textEditor.SearchPanel.Open();
        textEditor.SearchPanel.IsReplaceMode = true;
        textEditor.SearchPanel.ReplaceNext();

        Assert.AreEqual("hello universe", textEditor.Text);
    }

    [AvaloniaTest]
    public void Replace_Next_After_Replace_Next_Should_Replace_Occurence()
    {
        UnitTestApplication.InitializeStyles();

        TextEditor textEditor = CreateEditor();
        textEditor.Text = "hello world world world";

        textEditor.CaretOffset = 6;

        textEditor.SearchPanel.SearchPattern = "world";
        textEditor.SearchPanel.ReplacePattern = "universe";
        textEditor.SearchPanel.Open();
        textEditor.SearchPanel.IsReplaceMode = true;
        textEditor.SearchPanel.ReplaceNext();
        textEditor.SearchPanel.ReplaceNext();

        Assert.AreEqual("hello universe universe world", textEditor.Text);
    }

    [AvaloniaTest]
    public void Replace_Next_Should_Replace_Occurence_At_Caret_Index()
    {
        UnitTestApplication.InitializeStyles();

        TextEditor textEditor = CreateEditor();
        textEditor.Text = "hello world world";

        textEditor.CaretOffset = 6;

        textEditor.SearchPanel.SearchPattern = "world";
        textEditor.SearchPanel.ReplacePattern = "universe";
        textEditor.SearchPanel.Open();
        textEditor.SearchPanel.IsReplaceMode = true;
        textEditor.SearchPanel.ReplaceNext();

        Assert.AreEqual("hello universe world", textEditor.Text);
    }

    [AvaloniaTest]
    public void Clear_Search_Pattern_Should_Clean_Selection()
    {
        UnitTestApplication.InitializeStyles();

        TextEditor textEditor = CreateEditor();
        textEditor.Text = "hello world";

        textEditor.SearchPanel.SearchPattern = "world";
        textEditor.SearchPanel.Open();

        textEditor.SearchPanel.SearchPattern = "";

        Assert.AreEqual(0, textEditor.SelectionLength);
    }

    [AvaloniaTest]
    public void Replace_All_Should_Replace_All_Occurences()
    {
        UnitTestApplication.InitializeStyles();

        TextEditor textEditor = CreateEditor();
        textEditor.Text = "hello world hello world";

        textEditor.SearchPanel.SearchPattern = "hello";
        textEditor.SearchPanel.ReplacePattern = "bye";
        textEditor.SearchPanel.Open();
        textEditor.SearchPanel.IsReplaceMode = true;
        textEditor.SearchPanel.ReplaceAll();

        Assert.AreEqual("bye world bye world", textEditor.Text);
    }

    [AvaloniaTest]
    public void Replace_All_With_Regex_Should_Expand_Capture_Groups()
    {
        UnitTestApplication.InitializeStyles();

        TextEditor textEditor = CreateEditor();
        textEditor.Text = "Abcde\nAfgge\n";

        textEditor.SearchPanel.UseRegex = true;
        textEditor.SearchPanel.SearchPattern = "A.(..)e";
        textEditor.SearchPanel.ReplacePattern = @"Ad\1e";
        textEditor.SearchPanel.Open();
        textEditor.SearchPanel.IsReplaceMode = true;
        textEditor.SearchPanel.ReplaceAll();

        Assert.AreEqual("Adcde\nAdgge\n", textEditor.Text);
    }

    [AvaloniaTest]
    public void Replace_Next_With_Regex_Should_Expand_Capture_Groups()
    {
        UnitTestApplication.InitializeStyles();

        TextEditor textEditor = CreateEditor();
        textEditor.Text = "to_integer(a_s) + to_integer(b_s)";

        textEditor.SearchPanel.UseRegex = true;
        textEditor.SearchPanel.SearchPattern = @"to_integer\((?<sig>\w+)\)";
        textEditor.SearchPanel.ReplacePattern = "to_int(${sig})";
        textEditor.SearchPanel.Open();
        textEditor.SearchPanel.IsReplaceMode = true;
        textEditor.SearchPanel.ReplaceNext();

        Assert.AreEqual("to_int(a_s) + to_integer(b_s)", textEditor.Text);

        textEditor.SearchPanel.ReplaceNext();

        Assert.AreEqual("to_int(a_s) + to_int(b_s)", textEditor.Text);
    }

    [AvaloniaTest]
    public void Replace_Without_Regex_Should_Insert_Pattern_Literally()
    {
        UnitTestApplication.InitializeStyles();

        TextEditor textEditor = CreateEditor();
        textEditor.Text = "(a) (a)";

        textEditor.SearchPanel.SearchPattern = "(a)";
        textEditor.SearchPanel.ReplacePattern = @"$1\1";
        textEditor.SearchPanel.Open();
        textEditor.SearchPanel.IsReplaceMode = true;
        textEditor.SearchPanel.ReplaceNext();

        Assert.AreEqual(@"$1\1 (a)", textEditor.Text);

        textEditor.SearchPanel.ReplaceAll();

        Assert.AreEqual(@"$1\1 $1\1", textEditor.Text);
    }

    [TestCase(@"Ad\1e", "Ad${1}e")]
    [TestCase("Ad$1e", "Ad$1e")]
    [TestCase(@"\10", "${1}0")]
    [TestCase(@"a\nb\tc", "a\nb\tc")]
    [TestCase(@"C:\\temp", @"C:\temp")]
    [TestCase(@"\\1", @"\1")]
    [TestCase(@"a\x", @"a\x")]
    [TestCase(@"end\", @"end\")]
    [TestCase("", "")]
    public void ToRegexReplacement_Converts_Backslash_Syntax(string input, string expected)
    {
        Assert.AreEqual(expected, SearchPanel.ToRegexReplacement(input));
    }

    static TextEditor CreateEditor()
    {
        TextEditor textEditor = new TextEditor();

        Window window = new Window();
        window.Content = textEditor;

        window.Show();
        textEditor.ApplyTemplate();

        return textEditor;
    }
}