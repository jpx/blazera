using System.Collections.Generic;

using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class MessageBuilder
    {
        #region Classes

        public class CInfo
        {
            public Vector2f MaxDimension;
            public Font Font;
            public uint Size;
            public Text.Styles Styles;

            public CInfo(Vector2f maxDimension, Font font, uint size, Text.Styles styles)
            {
                MaxDimension = maxDimension;
                Font = font;
                Size = size;
                Styles = styles;
            }
        }

        #endregion Classes

        #region Members

        string Message;
        List<string> FormattedMessage;

        CInfo Info;

        List<string> Words;

        #endregion

        public MessageBuilder(CInfo info)
        {
            FormattedMessage = new List<string>();

            Info = info;
        }

        public List<string> GetFormattedMessage(string message)
        {
            Message = message;

            Words = new List<string>(Message.Split(' '));

            FormatMessage();

            return FormattedMessage;
        }

        bool FormatMessage()
        {
            FormattedMessage.Clear();

            if (Words.Count <= 0)
                return false;

            Text text = new Text(string.Empty, Info.Font, Info.Size);
            text.Style = Info.Styles;
            int wordCount = 0;

            text.DisplayedString += Words[wordCount++];
            if (text.GetRect().Height > Info.MaxDimension.Y)
                return false;

            if (text.GetRect().Width > Info.MaxDimension.X)
                return CutWord(0);

            while (wordCount < Words.Count)
            {
                text.DisplayedString += (text.DisplayedString == string.Empty ? "" : " ") + Words[wordCount];

                if (text.GetRect().Width > Info.MaxDimension.X)
                {
                    text.DisplayedString =
                        text.DisplayedString.Substring(
                        0,
                        text.DisplayedString.Length - (Words[wordCount].Length + (text.DisplayedString.Length <= Words[wordCount].Length ? 0 : 1)));
                    text.DisplayedString += "\n" + Words[wordCount];

                    if (text.GetRect().Height > Info.MaxDimension.Y)
                    {
                        text.DisplayedString = text.DisplayedString.Substring(
                            0,
                            text.DisplayedString.Length - (Words[wordCount].Length + 1));

                        FormattedMessage.Add(text.DisplayedString);

                        text.DisplayedString = string.Empty;
                        continue;
                    }
                }

                if (text.GetRect().Width > Info.MaxDimension.X)
                    return CutWord(wordCount);

                ++wordCount;
            }

            if (text.DisplayedString != string.Empty)
                FormattedMessage.Add(text.DisplayedString);

            return true;
        }

        /// <summary>
        /// Cut a word that does not fit in one row placing a '-' at the limit.
        /// </summary>
        /// <param name="wordIndex">Index of word that must be cut.</param>
        /// <returns>True if the operation is successful.</returns>
        bool CutWord(int wordIndex)
        {
            string word = Words[wordIndex];
            Text text = new Text(word, Info.Font, Info.Size);
            text.Style = Info.Styles;

            int count = 0;
            while (text.GetRect().Width > Info.MaxDimension.X)
            {
                if (count > 0)
                    text.DisplayedString = text.DisplayedString.Substring(0, text.DisplayedString.Length - 1);

                text.DisplayedString = text.DisplayedString.Substring(0, word.Length - ++count);
                text.DisplayedString += "-";
            }

            Words.Insert(wordIndex, text.DisplayedString);
            Words[wordIndex + 1] = word.Substring(word.Length - count);

            FormatMessage();

            return true;
        }
    }
}
