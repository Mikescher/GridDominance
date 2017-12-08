using System;
using System.Text;

namespace MonoSAMFramework.Portable.DebugTools
{
	public static class CompactJsonFormatter
	{
		private const string INDENT_STRING = "  ";
		public static string FormatJSON(string str, int maxIndent)
		{
			var indent = 0;
			var quoted = false;
			var sb = new StringBuilder();
			var last = ' ';
			for (var i = 0; i < str.Length; i++)
			{
				var ch = str[i];
				switch (ch)
				{
					case '\r':
					case '\n':
						break;
					case '{':
					case '[':
						sb.Append(ch);
						last = ch;
						if (!quoted)
						{
							indent++;
							if (indent >= maxIndent) break;
							sb.AppendLine();
							for (int ix = 0; ix < indent; ix++) sb.Append(INDENT_STRING);
							last = ' ';
						}
						break;
					case '}':
					case ']':
						if (!quoted)
						{
							indent--;
							if (indent + 1 >= maxIndent) { sb.Append(ch); break; }
							sb.AppendLine();
							for (int ix = 0; ix < indent; ix++) sb.Append(INDENT_STRING);
						}
						sb.Append(ch);
						last = ch;
						break;
					case '"':
						sb.Append(ch);
						last = ch;
						bool escaped = false;
						var index = i;
						while (index > 0 && str[--index] == '\\')
							escaped = !escaped;
						if (!escaped)
							quoted = !quoted;
						break;
					case ',':
						sb.Append(ch);
						last = ch;
						if (!quoted)
						{
							if (indent >= maxIndent) { sb.Append(' '); last = ' '; break; }
							sb.AppendLine();
							for (int ix = 0; ix < indent; ix++) sb.Append(INDENT_STRING);
						}
						break;
					case ':':
						sb.Append(ch);
						last = ch;
						if (!quoted) { sb.Append(" "); last = ' '; }
						break;
					case ' ':
					case '\t':
						if (quoted)
						{
							sb.Append(ch);
							last = ch;
						}
						else if (last != ' ')
						{
							sb.Append(' ');
							last = ' ';
						}
						break;
					default:
						sb.Append(ch);
						last = ch;
						break;
				}
			}
			return sb.ToString();
		}


		public static string CompressJson(string str, int compressionLevel)
		{
			var indent = 0;
			var quoted = false;
			var sb = new StringBuilder();
			var last = ' ';
			var compress = 0;
			for (var i = 0; i < str.Length; i++)
			{
				var ch = str[i];
				switch (ch)
				{
					case '\r':
					case '\n':
						break;
					case '{':
					case '[':

						sb.Append(ch);
						last = ch;
						if (!quoted)
						{
							if (compress == 0 && GetJsonDepth(str, i) <= compressionLevel)
								compress = 1;
							else if (compress > 0)
								compress++;

							indent++;
							if (compress > 0) break;
							sb.AppendLine();
							for (int ix = 0; ix < indent; ix++) sb.Append(INDENT_STRING);
							last = ' ';
						}
						break;
					case '}':
					case ']':
						if (!quoted)
						{
							indent--;
							if (compress > 0) { compress--; sb.Append(ch); break; }
							compress--;
							sb.AppendLine();
							for (int ix = 0; ix < indent; ix++) sb.Append(INDENT_STRING);
						}
						sb.Append(ch);
						last = ch;
						break;
					case '"':
						sb.Append(ch);
						last = ch;
						bool escaped = false;
						var index = i;
						while (index > 0 && str[--index] == '\\')
							escaped = !escaped;
						if (!escaped)
							quoted = !quoted;
						break;
					case ',':
						sb.Append(ch);
						last = ch;
						if (!quoted)
						{
							if (compress > 0) { sb.Append(' '); last = ' '; break; }
							sb.AppendLine();
							for (int ix = 0; ix < indent; ix++) sb.Append(INDENT_STRING);
						}
						break;
					case ':':
						sb.Append(ch);
						last = ch;
						if (!quoted) { sb.Append(" "); last = ' '; }
						break;
					case ' ':
					case '\t':
						if (quoted)
						{
							sb.Append(ch);
							last = ch;
						}
						else if (last != ' ')
						{
							sb.Append(' ');
							last = ' ';
						}
						break;
					default:
						sb.Append(ch);
						last = ch;
						break;
				}
			}
			return sb.ToString();
		}

		public static int GetJsonDepth(string str, int i)
		{
			var maxindent = 0;
			var indent = 0;
			var quoted = false;
			for (; i < str.Length; i++)
			{
				var ch = str[i];
				switch (ch)
				{
					case '{':
					case '[':
						if (!quoted)
						{
							indent++;
							maxindent = Math.Max(indent, maxindent);
						}
						break;

					case '}':
					case ']':
						if (!quoted)
						{
							indent--;
							if (indent <= 0) return maxindent;
						}
						break;

					case '"':
						bool escaped = false;
						var index = i;
						while (index > 0 && str[--index] == '\\')
							escaped = !escaped;
						if (!escaped)
							quoted = !quoted;
						break;

					default:
						break;
				}
			}
			return maxindent;
		}
	}
}
