namespace AjSharp.Templates
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class Preprocessor
    {
        private TextReader reader;
        private TextWriter writer;

        public Preprocessor(TextReader reader, TextWriter writer)
        {
            this.reader = reader;
            this.writer = writer;
        }

        public void Process()
        {
            bool inmultiline = false;

            for (string line = this.reader.ReadLine(); line != null; line = this.reader.ReadLine())
            {
                string trimline = line.Trim();

                if (inmultiline)
                {
                    if (trimline.StartsWith("@}"))
                    {
                        inmultiline = false;
                        line = line.Substring(line.IndexOf("@}") + 2);

                        if (!string.IsNullOrEmpty(line.Trim()))
                            this.PrintCodeLine(line);
                    }
                    else
                    {
                        this.PrintCodeLine(line);
                    }
                }
                else
                {
                    if (trimline.StartsWith("@{"))
                    {
                        inmultiline = true;
                        line = line.Substring(line.IndexOf("@{") + 2);

                        if (!string.IsNullOrEmpty(line.Trim()))
                            this.PrintCodeLine(line);
                    }
                    else if (trimline.StartsWith("@"))
                    {
                        line = line.Substring(line.IndexOf("@") + 1);
                        this.PrintCodeLine(line);
                    }
                    else
                        this.PrintLine(line);
                }
            }

            this.reader.Close();
            this.writer.Flush();
        }

        private void PrintCodeLine(string code)
        {
            this.writer.WriteLine(code);
        }

        private void PrintLine(string line)
        {
            this.writer.WriteLine(string.Format("PrintLine(\"{0}\");", line));
        }
    }
}
