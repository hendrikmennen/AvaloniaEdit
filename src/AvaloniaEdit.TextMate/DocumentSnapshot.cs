using System;
using System.Linq;

using AvaloniaEdit.Document;

namespace AvaloniaEdit.TextMate
{
    public class DocumentSnapshot
    {
        private LineRange[] _lineRanges;
        private TextDocument _document;
        private ITextSource _textSource;
        private object _lock = new object();
        private int _lineCount;

        public int LineCount
        {
            get { lock (_lock) { return _lineCount; } }
        }

        public DocumentSnapshot(TextDocument document)
        {
            _document = document;
            _lineRanges = new LineRange[document.LineCount];

            Update(null);
        }

        public void RemoveLines(int startLine, int endLine)
        {
            lock (_lock)
            {
                var tmpList = _lineRanges.ToList();
                tmpList.RemoveRange(startLine, endLine - startLine + 1);
                _lineRanges = tmpList.ToArray();
                _lineCount = _lineRanges.Length;
            }
        }

        public string GetLineText(int lineIndex)
        {
            lock (_lock)
            {
                var lineRange = _lineRanges[lineIndex];
                return _textSource.GetText(lineRange.Offset, lineRange.Length);
            }
        }

        public string GetLineTextIncludingTerminator(int lineIndex)
        {
            lock (_lock)
            {
                var lineRange = _lineRanges[lineIndex];
                return _textSource.GetText(lineRange.Offset, lineRange.TotalLength);
            }
        }

        public ReadOnlyMemory<char> GetLineTextIncludingTerminatorAsMemory(int lineIndex)
        {
            lock (_lock)
            {
                var lineRange = _lineRanges[lineIndex];
                return _textSource.GetTextAsMemory(lineRange.Offset, lineRange.TotalLength);
            }
        }

        /// <summary>
        /// Returns the text of a line including its terminator, unless the line is
        /// longer than <paramref name="maxLength"/> characters (excluding the
        /// terminator). In that case only the line terminator is returned so the
        /// tokenizer effectively skips the line.
        /// Some grammars exhibit catastrophic regex backtracking on very long lines
        /// which can freeze the tokenization thread; the cost of backtracking is
        /// superlinear, so truncating is not enough - the line must be skipped
        /// entirely. The line terminator is preserved so the tokenizer still
        /// recognizes the end of the line and keeps its state consistent.
        /// </summary>
        public ReadOnlyMemory<char> GetLineTextIncludingTerminatorAsMemory(int lineIndex, int maxLength)
        {
            lock (_lock)
            {
                var lineRange = _lineRanges[lineIndex];

                if (maxLength < 0 || lineRange.Length <= maxLength)
                    return _textSource.GetTextAsMemory(lineRange.Offset, lineRange.TotalLength);

                int terminatorLength = lineRange.TotalLength - lineRange.Length;

                if (terminatorLength <= 0)
                    return ReadOnlyMemory<char>.Empty;

                // Return only the line terminator so the tokenizer produces a single
                // (default) token for this line without running the grammar over its
                // potentially pathological content.
                return _textSource.GetTextAsMemory(
                    lineRange.Offset + lineRange.Length, terminatorLength);
            }
        }

        public string GetLineTerminator(int lineIndex)
        {
            lock (_lock)
            {
                var lineRange = _lineRanges[lineIndex];
                return _textSource.GetText(lineRange.Offset + lineRange.Length, lineRange.TotalLength - lineRange.Length);
            }
        }

        public int GetLineLength(int lineIndex)
        {
            lock (_lock)
            {
                return _lineRanges[lineIndex].Length;
            }
        }

        public int GetTotalLineLength(int lineIndex)
        {
            lock (_lock)
            {
                return _lineRanges[lineIndex].TotalLength;
            }
        }

        public string GetText()
        {
            lock (_lock)
            {
                return _textSource.Text;
            }
        }

        public void Update(DocumentChangeEventArgs e)
        {
            lock (_lock)
            {
                _lineCount = _document.Lines.Count;

                if (e != null && e.OffsetChangeMap != null && _lineRanges != null && _lineCount == _lineRanges.Length)
                {
                    // it's a single-line change
                    // update the offsets usign the OffsetChangeMap
                    RecalculateOffsets(e);
                }
                else
                {
                    // recompute all the line ranges
                    // based in the document lines
                    RecomputeAllLineRanges(e);
                }

                _textSource = _document.CreateSnapshot();
            }
        }

        private void RecalculateOffsets(DocumentChangeEventArgs e)
        {
            var changedLine = _document.GetLineByOffset(e.Offset);
            int lineIndex = changedLine.LineNumber - 1;

            _lineRanges[lineIndex].Offset = changedLine.Offset;
            _lineRanges[lineIndex].Length = changedLine.Length;
            _lineRanges[lineIndex].TotalLength = changedLine.TotalLength;

            for (int i = lineIndex + 1; i < _lineCount; i++)
            {
                _lineRanges[i].Offset = e.OffsetChangeMap.GetNewOffset(_lineRanges[i].Offset);
            }
        }

        private void RecomputeAllLineRanges(DocumentChangeEventArgs e)
        {
            Array.Resize(ref _lineRanges, _lineCount);

            int currentLineIndex = (e != null) ?
                _document.GetLineByOffset(e.Offset).LineNumber - 1 : 0;
            var currentLine = _document.GetLineByNumber(currentLineIndex + 1);

            while (currentLine != null)
            {
                _lineRanges[currentLineIndex].Offset = currentLine.Offset;
                _lineRanges[currentLineIndex].Length = currentLine.Length;
                _lineRanges[currentLineIndex].TotalLength = currentLine.TotalLength;
                currentLine = currentLine.NextLine;
                currentLineIndex++;
            }
        }

        struct LineRange
        {
            public int Offset;
            public int Length;
            public int TotalLength;
        }
    }
}
