using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    public static class AutomatonSearch
    {
        private enum State
        {
            Start,          
            Hour0,         
            Hour1,        
            Hour0x,       
            Hour2x,        
            Colon1,     
            Minute1,       
            Minute2,      
            Colon2,      
            Second1,    
            Second2,        
            Dead
        }

        public static List<SearchResult> FindTimeOccurrences(string text)
        {
            var results = new List<SearchResult>();
            if (string.IsNullOrEmpty(text)) return results;

            int line = 1, column = 1;
            int offset = 0;
            State state = State.Start;
            int startOffset = -1, startLine = 0, startColumn = 0;
            var buffer = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                State nextState = Transition(state, c);

                if (state == State.Start && nextState != State.Dead)
                {
                    startOffset = offset;
                    startLine = line;
                    startColumn = column;
                    buffer.Clear();
                }

                if (nextState != State.Dead)
                {
                    buffer.Append(c);
                }

                if (nextState == State.Second2)
                {
                    results.Add(new SearchResult
                    {
                        FoundText = buffer.ToString(),
                        Offset = startOffset,
                        Length = buffer.Length,
                        Line = startLine,
                        Column = startColumn,
                        Position = $"строка {startLine}, столбец {startColumn}"
                    });
                    nextState = State.Start;
                    buffer.Clear();
                }
                else if (nextState == State.Dead && state != State.Start)
                {
                    nextState = Transition(State.Start, c);
                    buffer.Clear();
                    if (nextState != State.Dead)
                    {
                        startOffset = offset;
                        startLine = line;
                        startColumn = column;
                        buffer.Append(c);
                    }
                }

                state = nextState;

                offset++;
                if (c == '\n')
                {
                    line++;
                    column = 1;
                }
                else
                {
                    column++;
                }
            }

            return results;
        }

        private static State Transition(State s, char c)
        {
            switch (s)
            {
                case State.Start:
                    if (c == '0' || c == '1') return State.Hour0;
                    if (c == '2') return State.Hour1;
                    return State.Dead;

                case State.Hour0:
                    if (char.IsDigit(c)) return State.Hour0x;
                    return State.Dead;

                case State.Hour1:
                    if (c >= '0' && c <= '3') return State.Hour2x;
                    return State.Dead;

                case State.Hour0x:
                case State.Hour2x:
                    if (c == ':') return State.Colon1;
                    return State.Dead;

                case State.Colon1:
                    if (c >= '0' && c <= '5') return State.Minute1;
                    return State.Dead;

                case State.Minute1:
                    if (char.IsDigit(c)) return State.Minute2;
                    return State.Dead;

                case State.Minute2:
                    if (c == ':') return State.Colon2;
                    return State.Dead;

                case State.Colon2:
                    if (c >= '0' && c <= '5') return State.Second1;
                    return State.Dead;

                case State.Second1:
                    if (char.IsDigit(c)) return State.Second2;
                    return State.Dead;

                case State.Second2:
                   
                    return State.Dead;

                default:
                    return State.Dead;
            }
        }
    }
}