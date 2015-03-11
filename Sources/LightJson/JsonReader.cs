﻿using System;
using System.IO;
using System.Text;

namespace LightJson
{
	public sealed class JsonReader
	{
		private TextReader reader;

		private long line;
		private long column;

		private JsonReader(TextReader reader)
		{
			this.reader = reader;
			this.line = 1;
			this.column = 1;
		}

		private char Peek()
		{
			var next = reader.Peek();

			if (next == -1)
			{
				// TODO: Add message.
				throw new EndOfStreamException();
			}
			else
			{
				return (char)next;
			}
		}

		private char Read()
		{
			var next = reader.Read();

			if (next == -1)
			{
				// TODO: Add message.
				throw new EndOfStreamException();
			}
			else
			{
				if (next == '\n')
				{
					line += 1;
					column = 1;
				}
				else
				{
					column += 1;
				}
				return (char)next;
			}
		}

		private void SkipWhitespaces()
		{
			while (char.IsWhiteSpace(Peek()))
			{
				Read();
			}
		}

		private void AssertNextChar(char next)
		{
			if (Read() != next)
			{
				throw new JsonParseException(
					string.Format("Expecting '{0}' on line {1}.", next, this.line),
					this.line,
					this.column
				);
			}
		}

		private void AssertString(string s)
		{
			for (var i = 0; i < s.Length; i += 1)
			{
				if (char.ToLower(s[i]) != char.ToLower(Read()))
				{
					throw new JsonParseException(
						string.Format("Expecting '{0}' on line {1}", s, this.line),
						this.line,
						this.column
					);
				}
			}
		}

		private string ReadJsonKey()
		{
			return ReadString();
		}

		private JsonValue ReadJsonValue()
		{
			SkipWhitespaces();

			if (char.IsNumber(Peek()))
			{
				return ReadNumber();
			}

			switch (char.ToLower(Peek()))
			{
				case '{':
					return ReadObject();

				case '[':
					return ReadArray();

				case '"':
					return ReadString();

				case '-':
					return ReadNumber();

				case 't':
				case 'f':
					return ReadBoolean();

				case 'n':
					return ReadNull();

				default:
					throw new JsonParseException(
						string.Format("Unexpected character on line {0}, column {1}", line, column),
						this.line,
						this.column
					);
			}
		}

		private JsonValue ReadNull()
		{
			AssertString("null");
			return JsonValue.Null;
		}

		private JsonValue ReadBoolean()
		{
			switch (char.ToLower(Peek()))
			{
				case 't':
					AssertString("true");
					return true;

				case 'f':
					AssertString("false");
					return false;

				default:
					throw new JsonParseException(
						string.Format("Expecting boolean on line {0}", line),
						this.line,
						this.column
					);
			}
		}

		private void ReadDigits(StringBuilder builder)
		{
			if (char.IsNumber(Peek()))
			{
				while (char.IsNumber(Peek()))
				{
					builder.Append(Read());
				}
			}
			else
			{
				throw new JsonParseException(
					string.Format("Expecting digit on line {0}, column {1}", line, column),
					this.line,
					this.column
				);
			}
		}

		private JsonValue ReadNumber()
		{
			var builder = new StringBuilder();

			if (Peek() == '-')
			{
				builder.Append(Read());
			}

			if (Peek() == '0')
			{
				builder.Append(Read());
			}
			else
			{
				ReadDigits(builder);
			}

			if (Peek() == '.')
			{
				builder.Append(Read());
				ReadDigits(builder);
			}

			if (char.ToLower(Peek()) == 'e')
			{
				builder.Append(Read());
				var next = Peek();
				if (next == '+' || next == '-')
				{
					builder.Append(Read());
				}
				ReadDigits(builder);
			}

			return double.Parse(builder.ToString());
		}

		private string ReadString()
		{
			var builder = new StringBuilder();

			AssertNextChar('"');

			while (true)
			{
				var c = Read();

				if (c == '\\')
				{
					c = Read();

					switch (char.ToLower(c))
					{
						case '"':  // "
						case '\\': // \
						case '/':  // /
							builder.Append(c);
							break;
						case 'b':
							builder.Append('\b');
							break;
						case 'f':
							builder.Append('\f');
							break;
						case 'n':
							builder.Append('\n');
							break;
						case 'r':
							builder.Append('\r');
							break;
						case 't':
							builder.Append('\t');
							break;
						case 'u':
							builder.Append(ReadUnicodeLiteral());
							break;
						default:
							throw new JsonParseException(
								string.Format("Unexpected string escape character on line {0}", this.line),
								this.line,
								this.column
							);
					}
				}
				else if (c == '"')
				{
					break;
				}
				else
				{
					if (char.IsControl(c))
					{
						throw new JsonParseException(
							string.Format("Control character in string literal on line {0}", this.line),
							this.line,
							this.column
						);
					}
					else
					{
						builder.Append(c);
					}
				}
			}

			return builder.ToString();
		}

		private string ReadUnicodeLiteral()
		{
			throw new NotImplementedException();
		}

		private JsonObject ReadObject()
		{
			var jsonObject = new JsonObject();

			AssertNextChar('{');

			SkipWhitespaces();
			if (Peek() == '}')
			{
				Read();
			}
			else
			{
				while (true)
				{
					SkipWhitespaces();
					var key = ReadJsonKey();

					if (jsonObject.Contains(key))
					{
						throw new JsonParseException(
							string.Format("Duplicate object key on line {0}", this.line),
							this.line,
							this.column
						);
					}

					SkipWhitespaces();
					AssertNextChar(':');

					SkipWhitespaces();
					var value = ReadJsonValue();

					jsonObject.Add(key, value);

					SkipWhitespaces();
					var next = Read();
					if (next == '}')
					{
						break;
					}
					else if (next == ',')
					{
						continue;
					}
					else
					{
						throw new JsonParseException(
							string.Format("Expecting ',' or '}}' on line {0}", this.line),
							this.line,
							this.column
						);
					}
				}
			}

			return jsonObject;
		}

		private JsonArray ReadArray()
		{
			var jsonArray = new JsonArray();

			AssertNextChar('[');

			SkipWhitespaces();
			if (Peek() == ']')
			{
				Read();
			}
			else
			{
				while (true)
				{
					SkipWhitespaces();
					var value = ReadJsonValue();

					jsonArray.Add(value);

					SkipWhitespaces();
					var next = Read();
					if (next == ']')
					{
						break;
					}
					else if (next == ',')
					{
						continue;
					}
					else
					{
						throw new JsonParseException(
							string.Format("Expecting ',' or ']' on line {0} ", this.line),
							this.line,
							this.column
						);
					}
				}
			}

			return jsonArray;
		}

		private JsonValue Parse()
		{
			using (this.reader)
			{
				SkipWhitespaces();
				return ReadJsonValue();
			}
		}

		public static JsonValue Parse(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}

			return new JsonReader(reader).Parse();
		}

		public static JsonValue Parse(string source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			return new JsonReader(new StringReader(source)).Parse();
		}

		public static JsonValue ParseFile(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}

			return new JsonReader(new StreamReader(path)).Parse();
		}
	}
}