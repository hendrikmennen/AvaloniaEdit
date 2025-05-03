using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Document;

public class DataObjectCopyingEventArgs :
    RoutedEventArgs
{
    public DataObjectCopyingEventArgs(IDataObject dataObject, bool isDragDrop) : base(DataObjectEx.DataObjectCopyingEvent)
    {
        DataObject = dataObject;
        IsDragDrop = isDragDrop;
    }

    public bool CommandCancelled { get; private set; }
    public IDataObject DataObject { get; }
    public bool IsDragDrop { get; }
    public void CancelCommand() => CommandCancelled = true;
}
