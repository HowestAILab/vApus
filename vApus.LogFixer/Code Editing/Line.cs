/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System.Collections.Generic;
using System.Drawing;
using System.Text;
using vApus.Stresstest;

namespace vApus.LogFixer
{
    public class Lines : List<Line>
    {
        public Lines()
        {
        }

        public Lines(int count)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (Line l in this)
                sb.AppendLine(l.ToString());

            return sb.ToString().Trim();
        }
    }

    public class Line
    {
        public readonly Lines Parent;
        private readonly bool _fileNotFoundComment;
        private readonly bool _hasLogEntry;

        public string Comment;
        public LogEntry LogEntry;

        private Line(Lines parent)
        {
            Parent = parent;
        }

        public Line(Lines parent, string comment, bool fileNotFoundComment = false)
            : this(parent)
        {
            Comment = comment;
            _fileNotFoundComment = fileNotFoundComment;
        }

        public Line(Lines parent, LogEntry logEntry)
            : this(parent)
        {
            LogEntry = logEntry;
            _hasLogEntry = true;
        }

        public LexicalResult LexicalResult
        {
            get
            {
                if (_hasLogEntry)
                    return LogEntry.LexicalResult;
                else if (_fileNotFoundComment)
                    return LexicalResult.Error;
                return LexicalResult.OK;
            }
        }

        public Color LineColor
        {
            get
            {
                if (_hasLogEntry)
                    return LogEntry.LexicalResult == LexicalResult.OK ? Color.DarkGreen : Color.DarkRed;
                else if (_fileNotFoundComment)
                    return Color.DarkRed;
                return Color.DarkGray;
            }
        }

        public override string ToString()
        {
            if (_hasLogEntry)
                return LogEntry.LogEntryString;
            return Comment;
        }
    }
}