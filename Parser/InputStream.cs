namespace Nebula.Parser
{
    public class InputStream
    {
        private int Pos { get; set; }
        private int Line { get; set; }
        private int Col { get; set; }
        private string Input { get; set; }
        private char[] InputArray { get; set; }
        public InputStream(string input)
        {
            Pos = 0;
            Line = 1;
            Col = 0;
            Input = input;
            InputArray = input.ToCharArray();
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
            throw new System.Exception($"{msg} {Line}:{Col}");
        }
    }
}