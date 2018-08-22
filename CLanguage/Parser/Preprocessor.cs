using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CLanguage.Parser
{
    public class Preprocessor
    {
		Report report;
        List<File> _files;
        List<Chunk> _chunks;
        Position _pos;
		Dictionary<string, Chunk> simpleDefines;

		char[] ident;
		int identLength;

        public Report Report => report;

        public bool Passthrough { get; }

        public Preprocessor (Report report = null, bool passthrough = false)
        {
            this.report = report ?? new Report ();
            Passthrough = passthrough;
            _files = new List<File>();
            _chunks = new List<Chunk>();
            //_log = log;
            _pos = new Position();
			simpleDefines = new Dictionary<string, Chunk> ();
			ident = new char[255];
			identLength = 0;
        }

        public Preprocessor (string name, string code, Report report = null, bool passthrough = false)
            : this (report, passthrough)
        {
            AddCode (name, code);
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

            public string Data { get { return File.Content.Substring(StartIndex, Length); } }

			public char this[int index] { get { return File.Content[StartIndex + index]; } }

			public string Substring (int startIndex, int length)
			{
				return File.Content.Substring (StartIndex + startIndex, length);
			}

			public Chunk Subchunk (int startIndex)
			{
				var length = Length - startIndex;
				return Subchunk (startIndex, length);
			}

			public Chunk Subchunk (int startIndex, int length)
			{
				if (length < 0) {
					length = 0;
				}
				return new Chunk {
					File = File,
					StartIndex = StartIndex + startIndex,
					Length = length,
					Line = Line,
				};
			}

			public int IndexOf (string substring)
			{
				if (string.IsNullOrEmpty (substring)) return -1;
				var n = substring.Length;
				if (n > Length) return -1;
				for (var i = 0; i <= Length - n; i++) {
					var j = 0;
					var match = true;
					while (match && j < n) {
						match = File.Content [StartIndex + i + j] == substring [j];
						j++;
					}
					if (match) {
						return i;
					}
				}
				return -1;
			}

			public override string ToString ()
			{
				return Data;
			}
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

		bool StringMatchesIdent (string s)
		{
			if (s.Length != identLength) return false;
			for (var i = 0; i < identLength; i++) {
				if (s[i] != ident[i]) return false;
			}
			return true;
		}

        void Process(File file)
        {
			identLength = 0;

            var p = 0;
            var numChars = file.Content.Length;
            var state = ProcessState.Normal;
            var line = 1;

            var chunk = new Chunk()
            {
                File = file,
                StartIndex = p
            };

            if (Passthrough) {
                // Skip processing whitespace, etc.
                chunk.Length = numChars;
                chunk.Line = 1;
                _chunks.Add (chunk);
                return;
            }

            Action NewChunk = () => {
				identLength = 0;
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
						if ((identLength < ident.Length) && (char.IsLetterOrDigit (ch) || ch == '_')) {
							ident[identLength] = ch;
							identLength++;
						}
						else {

							if (identLength > 0) {
								var defChunk = simpleDefines.Where (x => StringMatchesIdent (x.Key)).Select (x => x.Value).FirstOrDefault ();
								if (defChunk != null) {
									chunk.Length -= identLength;
									NewChunk ();
									chunk = defChunk;
									NewChunk ();
								}
							}

							identLength = 0;
						}

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

        void ExecuteDirective (Chunk chunk)
		{
			var n = chunk.Length;
			var defineIndex = chunk.IndexOf ("#define");
			if (defineIndex >= 0) {
				var p = defineIndex + 7;
				var idStart = p;
				while (idStart < n && char.IsWhiteSpace (chunk[idStart])) {
					idStart++;
				}
				if (idStart >= n) return;
				var idEnd = idStart;
				while (idEnd < n && (char.IsLetterOrDigit (chunk[idEnd]) || chunk[idEnd] == '_')) {
					idEnd++;
				}
				var idLength = idEnd - idStart;
				if (idLength <= 0) return;
				var id = chunk.Substring (idStart, idEnd - idStart);
				if (idEnd < n) {
					if (chunk[idEnd] == '(') {
						report.Error (2001, "Macros with parameters are not supported");
						simpleDefines[id] = chunk.Subchunk (idEnd);
					}
					else {
						simpleDefines[id] = chunk.Subchunk (idEnd);
					}
				}
				else {
					simpleDefines[id] = chunk.Subchunk (idEnd);
				}
			}
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

        public void AddDiskFile (string path, string content)
        {
            try
            {
                var file = new File()
                {
                    Path = path,
                    Content = content,
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

        public int CurrentPosition => _pos.ChunkIndex < _chunks.Count ?
                                            _pos.Index + _chunks[_pos.ChunkIndex].StartIndex :
                                            _chunks[_chunks.Count-1].StartIndex + _chunks[_chunks.Count - 1].Length;
        public string CurrentFilePath => _pos.ChunkIndex < _chunks.Count ?
                                            _chunks[_pos.ChunkIndex].File.Path : null;

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
				return -1;
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
