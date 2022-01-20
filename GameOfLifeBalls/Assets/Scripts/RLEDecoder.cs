using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RLEPattern
{
    string COMMENT_PREFIX = "#";

    char LINE_ENDING = '\n';

    string PATTERN_END = "!";

    private RLEParams rleParams;
    private List<RLEComment> comments = new List<RLEComment>();
    public List<RLEPart> Parts { get; private set; }
    private string rleString;

    public int Width { get { return rleParams.Width; } }
    public int Height { get { return rleParams.Height; } }

    public RLEPattern(string rleString)
    {
        this.rleString = rleString;

        string[] lines = rleString.Split(
            new string[] { "\r\n", "\r", "\n" },
            System.StringSplitOptions.None
        );
        ParseComments(lines);
        string[] relevantLines = lines.Where(line => !line.StartsWith(COMMENT_PREFIX)).ToArray();
        if (relevantLines.Length < 2)
        {
            MonoBehaviour.print("Error! Need 2 relevant lines!");
            return;
        }
        rleParams = new RLEParams(relevantLines[0]);
        string parts = "";
        for (int index = 1; index < relevantLines.Length; index += 1)
        {
            parts += relevantLines[index];
        }
        ParseParts(parts);
    }

    private void ParseComments(string[] lines)
    {
        string[] commentLines = lines.Where(line => line.StartsWith(COMMENT_PREFIX)).ToArray();
        for (int index = 0; index < commentLines.Length; index += 1)
        {
            string line = commentLines[index];
            string commentType = line.Substring(1, 1);
            string comment = line.Substring(3);
            comments.Add(new RLEComment(comment, commentType));
        }
    }

    private void ParseParts(string patternLine)
    {
        int index = 0;
        int tagLength = 1;
        Parts = new List<RLEPart>();
        while (index < patternLine.Length)
        {
            if (patternLine.Substring(index, tagLength) == " ")
            {
                index += 1;
                continue;
            }
            if (patternLine.Substring(index, tagLength) == PATTERN_END)
            {
                break;
            }
            int number = 1;
            int numberLength = FindNumberLength(patternLine.Substring(index));
            if (numberLength > 0)
            {
                number = System.Int32.Parse(patternLine.Substring(index, numberLength));
            }
            string tag = patternLine.Substring(index + numberLength, tagLength);
            Parts.Add(new RLEPart(number, tag));
            index += numberLength + tagLength;
        }
    }

    private int FindNumberLength(string part)
    {
        int numberLength = 0;
        for (int index = 0; index < part.Length; index += 1)
        {
            string current = part.Substring(index, 1);
            bool parseSuccesful = System.Int32.TryParse(current, out int result);
            if (!parseSuccesful)
            {
                numberLength = index;
                break;
            }
        }
        return numberLength;
    }

    public override string ToString()
    {
        return $@"
--Pattern--
{rleString}

--Params--
Width: {rleParams.Width}
Height: {rleParams.Height}
Rule: {rleParams.Rule}

--Comments--
{string.Join("\n", comments)}

--Parts--
{string.Join("\n", Parts)}
";
    }

}

public class RLEParams
{
    private const string PARAM_X = "x";
    private const string PARAM_Y = "y";
    private const string PARAM_RULE = "rule";
    private const char PARAM_SEPARATOR = ',';

    public int Width { get; private set; }
    public int Height { get; private set; }
    public string Rule { get; private set; }
    private string paramLine = "";

    public RLEParams(string paramLine)
    {
        this.paramLine = paramLine;
        ParseLine();
    }

    private void ParseLine()
    {
        string[] patternParams = paramLine.Split(PARAM_SEPARATOR);
        foreach (string param in patternParams)
        {
            string[] parts = param.Split('=');
            if (parts.Length < 2)
            {
                MonoBehaviour.print($"Param error! {param}");
                continue;
            }
            string paramKey = parts[0].Trim().ToLower();
            string paramValue = parts[1].Trim().ToLower();
            int paramValueInt = 0;
            if (paramKey == PARAM_X || paramKey == PARAM_Y)
            {
                bool parseSuccesful = System.Int32.TryParse(paramValue, out int outInt);
                if (!parseSuccesful)
                {
                    MonoBehaviour.print($"Error! Couldn't parse int from {paramKey} = {paramValue}!");
                    continue;
                }
                paramValueInt = outInt;
                if (paramKey == PARAM_X)
                {
                    Width = paramValueInt;
                }
                else if (paramKey == PARAM_Y)
                {
                    Height = paramValueInt;
                }
            }
            if (paramKey == PARAM_RULE)
            {
                Rule = paramValue;
            }
        }
    }
}

public class RLEPart
{
    public int RunCount { get; private set; }
    public RLETag Tag { get; private set; }
    private string tag = "";

    private Dictionary<string, RLETag> tagTypes = new Dictionary<string, RLETag>{
        {"$", RLETag.NewLine},
        {"b", RLETag.DeadCell},
        {"o", RLETag.AliveCell},
    };


    public RLEPart(int runCount, string tag)
    {
        RunCount = runCount;
        this.tag = tag;
        Tag = tagTypes[tag];
    }

    public override string ToString()
    {
        return $"{RunCount}: {Tag} ({tag})";
    }
}

public enum RLETag
{
    NewLine,
    DeadCell,
    AliveCell
}

public class RLEComment
{
    string value = "";
    RLECommentType type;

    string commentType = "";

    private Dictionary<string, RLECommentType> commentTypes = new Dictionary<string, RLECommentType>{
        {"C", RLECommentType.Comment},
        {"c", RLECommentType.Comment},
        {"N", RLECommentType.Name},
        {"O", RLECommentType.DateAndAuthor},
        {"P", RLECommentType.PositionLife32},
        {"R", RLECommentType.PositionXLife},
        {"r", RLECommentType.Rules}
    };

    public RLEComment(string value, string commentType)
    {
        this.value = value;
        this.commentType = commentType;
        type = commentTypes[commentType];
    }

    public override string ToString()
    {
        return $"{type} ({commentType}): {value}";
    }
}

public enum RLECommentType
{
    None,
    Comment,
    Name,
    DateAndAuthor,
    PositionLife32,
    PositionXLife,
    Rules
}