using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CLanguage
{
    public class Preprocessor
    {
        //Log _log;
        List<File> _files;
        List<Chunk> _chunks;
        Position _pos;

        public Preprocessor()
        {
            _files = new List<File>();
            _chunks = new List<Chunk>();
            //_log = log;
            _pos = new Position();
        }

        class File
        {
            public string Path;
            public string Content;
        }

        class Chunk
        {
            public File File;
            public int StartIndex;
            public int Length;
            public int Line;
            //public int Column;
            public string Data { get { return File.Content.Substring(StartIndex, Length); } }
        }

        class Position
        {
            public int ChunkIndex;
            public int Index;
        }

        enum ProcessState
        {
            Normal,
            Preprocessor,
        }

        void Process(File file)
        {
            var p = 0;
            var numChars = file.Content.Length;
            var state = ProcessState.Normal;
            var line = 1;

            var chunk = new Chunk()
            {
                File = file,
                StartIndex = p
            };

            Action NewChunk = () => {
                if (chunk != null && chunk.Length > 0)
                {
                    _chunks.Add(chunk);
                }
                chunk = new Chunk()
                {
                    File = file,
                    StartIndex = p,
                    Length = 0,
                    Line = line
                };
            };

            Func<char, bool> OnlyWhiteSince = (sentinel) => {
                var i = p - 1;
                while (i >= 0) {
                    var wc = file.Content[i];
                    if (wc == sentinel) return true;
                    if (!char.IsWhiteSpace(wc)) return false;
                    i--;
                }
                return false;
            };

            NewChunk();

            while (p < numChars)
            {
                var ch = file.Content[p];

                if (ch == '\n')
                {
                    line++;
                }

                if (state == ProcessState.Normal)
                {
                    if (ch == '#' && OnlyWhiteSince('\n'))
                    {
                        NewChunk();
                        state = ProcessState.Preprocessor;
                    }
                    else
                    {
                        chunk.Length++;
                        p++;
                    }
                }
                else if (state == ProcessState.Preprocessor)
                {
                    if (ch == '\n')
                    {
                        if (OnlyWhiteSince('\\'))
                        {
                            chunk.Length++;
                            p++;
                        }
                        else
                        {
                            ExecuteDirective(chunk);
                            chunk = null; // Discard it
                            NewChunk();
                            state = ProcessState.Normal;
                        }
                    }
                    else
                    {
                        chunk.Length++;
                        p++;
                    }
                }
            }

            NewChunk(); // Consume the last chunk
        }

        void ExecuteDirective(Chunk chunk)
        {
            Console.WriteLine("PP: " + chunk.Data);
        }

        public void AddCode(string name, string code)
        {
            var file = new File()
            {
                Path = name,
                Content = code
            };
            _files.Add(file);
            Process(file);
        }

        public void AddDiskFile(string filename)
        {
            try
            {
                var file = new File()
                {
                    Path = filename,
                    Content = System.IO.File.ReadAllText(filename)
                };
                _files.Add(file);
                Process(file);
            }
            catch (System.IO.IOException ioex)
            {
                Console.WriteLine(ioex);
                //_log.Error(Error.FromException(ioex));
            }
        }

        public int CurrentPosition
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Reads a single character and advances the CurrentPosition.
        /// </summary>
        /// <returns>The character read or -1 if there is no more input</returns>
        public int Read()
        {
            if (_pos.ChunkIndex >= _chunks.Count) return -1;

            var chunk = _chunks[_pos.ChunkIndex];

            if (_pos.Index >= chunk.Length) return -1;

            var ch = chunk.File.Content[_pos.Index + chunk.StartIndex];

            _pos.Index++;
            if (_pos.Index >= chunk.Length)
            {
                _pos.Index = 0;
                _pos.ChunkIndex++;
            }

            return ch;
        }

        public int Peek()
        {
            var chunk = _chunks[_pos.ChunkIndex];

            if (_pos.Index + 1 < chunk.Length)
            {
                return chunk.File.Content[_pos.Index + chunk.StartIndex];
            }
            else
            {
                throw new ApplicationException("OMFG we're screwed");
            }
        }

        public void Dump(TextWriter o, bool showLineNumbers)
        {
            if (showLineNumbers)
            {
                foreach (var c in _chunks)
                {
                    o.Write(c.Line + ": ");
                    o.WriteLine(c.Data);
                }
            }
            else
            {
                foreach (var c in _chunks)
                {
                    o.Write(c.Data);
                }
            }
        }

        
    }
}
