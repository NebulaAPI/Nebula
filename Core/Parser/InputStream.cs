using System;
using System.Collections.Generic;
using System.IO;

namespace Nebula.Core.Parser
{
    public class InputStream
    {
        public int Pos { get; set; }
        public int Line { get; set; }
        public int Col { get; set; }
        private string Input { get; set; }
        private char[] InputArray { get; set; }
        public string[] Lines { get; set; }
        public string FileName { get; set; }

        public InputStream()
        {
            Pos = 0;
            Line = 1;
            Col = 0;
        }

        public InputStream(string input) : this()
        {
            Input = input;
            InputArray = input.ToCharArray();
            Lines = new string[] {};
        }

        public InputStream(FileInfo inputFile)
        {
            Input = inputFile.OpenText().ReadToEnd();
            InputArray = Input.ToCharArray();
            Lines = Input.Split('\n');
            FileName = inputFile.FullName;
        }

        /// <summary>
        /// Get the next character in the stream
        /// </summary>
        /// <returns></returns>
        public char Next()
        {
            var ch = InputArray[Pos++];
            if (ch.ToString() == "\n")
            {
                Line++;
                Col = 0;
            }
            else
            {
                Col++;
            }
            return ch;
        }

        /// <summary>
        /// Resets the pointer back to the beginning of the stream
        /// </summary>
        public void Reset()
        {
            Pos = 0;
            Line = 1;
            Col = 0;
        }

        /// <summary>
        /// Return the character at the current position
        /// </summary>
        /// <returns></returns>
        public char Peek()
        {
            return InputArray[Pos];
        }

        /// <summary>
        /// Check if we have reached the end of the stream
        /// </summary>
        /// <returns></returns>
        public bool Eof()
        {
            return Pos == Input.Length;
        }

        /// <summary>
        /// Throw an exception indicating an error parsing at the current line and column
        /// </summary>
        /// <param name="msg"></param>
        public void Error(string msg)
        {
            var err = new [] {
                FileName + ": ",
                Lines[Line],
                new String(' ', Col) + "^",
                $"[{Line}:{Col}] {msg}"
            };
            
            throw new System.Exception(string.Join("\n", err));
        }
    }
}