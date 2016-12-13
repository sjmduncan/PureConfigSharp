using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace PureConfigNet
{
    public partial class PureConfig
    {
        enum ConfigType
        {
            Decimal, String, Quantity, Bool
        }
        class ConfigData
        {
            public ConfigType type;
            public object data;
        }

        Dictionary<string, ConfigData> config;
        protected bool ParseFile(System.IO.TextReader lines)
        {
            Dictionary<string, TokenDef[]> tokenisers= CreateTokenizers();
            List<List<Token>> tokens = new List<List<Token>>();
            string line;
            while((line = lines.ReadLine()) != null)
            {
                JoinMultiLines(ref line, lines);

                if(line?.Length > 0)
                    tokens.Add(ParseLine(line, tokenisers));
            }
            //TODO: ExpandScope (indentation)
            //TODO: Resolve References

            // By now all scopes should be fully specified (i.e. indentation replaced with dot notation)
            // and all references should be resolved to their groups/values with values overwritten appropriately.

            config = TokenListToDict(tokens);
            return true; //TODO
        }

        
        private Dictionary<string, ConfigData> TokenListToDict(List<List<Token>> tokens)
        {
            Dictionary<string, ConfigData> dict = new Dictionary<string, ConfigData>();
            foreach(List<Token> line in tokens)
            {
                if(line.Count < 2)
                    continue;
                ConfigData val = new ConfigData();
                string fullKey = "";
                Token lastToken = null;

                foreach(Token t in line)
                {
                    switch(t.tokenType)
                    {
                        case TokenType.KEY:
                            if(fullKey == "")
                                fullKey += t.data;
                            else
                                fullKey += "." + t.data;
                            break;
                        case TokenType.KEYQQ:
                            if(fullKey == "")
                                fullKey += t.data.Substring(1, t.data.Length-2);
                            else
                                fullKey += "." + t.data.Substring(1, t.data.Length-2);
                            break;
                        case TokenType.KEYQ:
                            if(fullKey == "")
                                fullKey += t.data.Substring(1, t.data.Length-2);
                            else
                                fullKey += "." + t.data.Substring(1, t.data.Length-2);
                            break;
                        case TokenType.BOOLFALSE:
                            val.data = false;
                            val.type = ConfigType.Bool;
                            break;
                        case TokenType.BOOLTRUE:
                            val.data = true;
                            val.type = ConfigType.Bool;
                            break;
                        case TokenType.STRING:
                            val.data = t.data;
                            val.type = ConfigType.String;
                            break;
                        case TokenType.STRINGQ:
                            val.data = t.data.Substring(1, t.data.Length-2);
                            val.type = ConfigType.String;
                            break;
                        case TokenType.STRINGQQ:
                            val.data = t.data.Substring(1, t.data.Length-2);
                            val.type = ConfigType.String;
                            break;
                        case TokenType.NUMBER:
                            double d;
                            if(!double.TryParse(t.data, out d))
                                throw new Exception("invalid number: " + t.data);
                            val.data = d;
                            val.type = ConfigType.Decimal;
                            break;
                        case TokenType.QUANTITY:
                            var m = Regex.Match(t.data, @"-?[0-9]*\.*[0-9]+(?=[A-Z])|[A-z]{1,2}");
                            Quantity q = new Quantity();
                            double.TryParse(m.Value, out q.val);
                            m=m.NextMatch();
                            q.unit = m.Value;
                            val.data = q;
                            val.type = ConfigType.Quantity;
                            break;
                    }
                    if(t.tokenType != TokenType.WHITESPACE)
                        lastToken = t;
                }
                if(lastToken?.tokenType == TokenType.ASSIGN)
                {
                    val.type = ConfigType.String;
                    val.data = "";
                }

                dict.Add(fullKey, val);
            }
            return dict;
        }

        private ConfigData TryGetVal(string id)
        {
            ConfigData data;
            if(config.TryGetValue(id, out data))
                return data;
            else
                throw new Exception("Key '" + id + "' does not exist");
        }

        private List<Token> ParseLine(string line, Dictionary<string, TokenDef[]> tokenisersList)
        {
            bool assignRHS = false, referenceRHS = false;

            string lineRight = line;
            List<Token> tokens = new List<Token>();
            Token newToken = null;
            while(lineRight.Length > 0)
            {
                TokenDef[] tokenisers;
                tokenisersList.TryGetValue("WHITESPACEANDCOMMENTS", out tokenisers);
                //Always check for whitespace and comments first
                foreach(TokenDef t in tokenisers)
                    if(t.Match(lineRight, out newToken))
                        break;
                // Only check for value-types after the assignment token
                if(newToken == null && assignRHS) 
                {
                    tokenisersList.TryGetValue("VALUES", out tokenisers);
                    foreach(TokenDef t in tokenisers)
                        if(t.Match(lineRight, out newToken))
                            break;
                }
                // Only check for key tokens after reference tokens
                else if(newToken == null && referenceRHS)
                {
                    tokenisersList.TryGetValue("KEY", out tokenisers);
                    foreach(TokenDef t in tokenisers)
                        if(t.Match(lineRight, out newToken))
                            break;

                }

                // Check for assignment or reference tokens
                if(newToken == null)
                {
                    tokenisersList.TryGetValue("ASSIGNANDREF", out tokenisers);
                    foreach(TokenDef t in tokenisers)
                    {
                        if(t.Match(lineRight, out newToken))
                        {
                            assignRHS = newToken.tokenType == TokenType.ASSIGN;
                            referenceRHS = newToken.tokenType == TokenType.REFERENCE;
                            break;
                        }
                    }
                }

                // Check for key tokens
                if(newToken == null)
                {
                    tokenisersList.TryGetValue("KEY", out tokenisers);
                    foreach(TokenDef t in tokenisers)
                        if(t.Match(lineRight, out newToken))
                            break;
                }

                if(newToken == null)
                    throw new Exception("\"" + lineRight + "\" does not match any tokens");

                lineRight = lineRight.Substring(newToken.match.Length);
                tokens.Add(newToken);
            }
            return tokens;
        }

        private void JoinMultiLines(ref string line, System.IO.TextReader lines)
        {
            /// Join lines at '\' characters pefore tokenising
            while(line.EndsWith(@"\"))
            {
                string linepart = lines.ReadLine();
                line = line.Substring(0, line.Length-1);
                if(linepart != null)
                    line = line + linepart;
            }
        }
        private enum TokenType
        {
                ASSIGN,REFERENCE,WHITESPACE,STRING,STRINGQQ,STRINGQ,COMMENT,NUMBER,QUANTITY,KEY,KEYQ,KEYQQ,ARRAY,BOOLTRUE,BOOLFALSE
        }
        private Dictionary<string, TokenDef[]> CreateTokenizers()
        {
            Dictionary<string, TokenDef[]> tokenisers = new Dictionary<string, TokenDef[]>();

            TokenDef[] comWhite = new TokenDef[]
            {
                new TokenDef(TokenType.WHITESPACE,      @"\s+"),
                new TokenDef(TokenType.COMMENT,         @"#.*$")
            };
            TokenDef[] key = new TokenDef[]
            {
                new TokenDef(TokenType.KEYQ,           @"'[^""\\]*(?:\\.[^""\\]*)*'"),
                new TokenDef(TokenType.KEYQQ,           @"""[^""\\]*(?:\\.[^""\\]*)*"""),
                new TokenDef(TokenType.KEY,           @"[^#=\n]+(?==)|[^#=\n]+(?=\s=)")
                
            };

            TokenDef[] assignRef = new TokenDef[]
            {
                
                new TokenDef(TokenType.ASSIGN,          @"="),
                new TokenDef(TokenType.REFERENCE,       @"=>")
            };

            TokenDef[] val = new TokenDef[]
            {
                new TokenDef(TokenType.BOOLTRUE,        @"true(?!\S)"),
                new TokenDef(TokenType.BOOLFALSE,       @"false(?!\S)"),
                new TokenDef(TokenType.STRINGQ,         @"'[^""\\]*(?:\\.[^""\\]*)*'"),
                new TokenDef(TokenType.STRINGQQ,        @"""[^""\\]*(?:\\.[^""\\]*)*"""),
                new TokenDef(TokenType.NUMBER,          @"-?[0-9]*\.?[0-9]+(?!\S)\s*$"),
                new TokenDef(TokenType.NUMBER,          @"-?[0-9]+\.?[0-9]*(?!\S)\s*$"),
                new TokenDef(TokenType.QUANTITY,        @"-?[0-9]*\.?[0-9]+[A-z]{1,2}(?!\S)"),

                new TokenDef(TokenType.STRING,          @"[^#]+|[^#]+(?=\s)")
            };

            tokenisers.Add("WHITESPACEANDCOMMENTS", comWhite);
            tokenisers.Add("KEY", key);
            tokenisers.Add("ASSIGNANDREF", assignRef);
            tokenisers.Add("VALUES", val);

            return tokenisers;
        }
        private class TokenDef
        {
            public readonly Regex matcher;
            public readonly TokenType tokenType;
            public TokenDef(TokenType type, string regex)
            {
                matcher = new Regex(@"^" + regex, RegexOptions.IgnoreCase);
                tokenType = type;
            }

            internal bool Match(string textData, out Token token)
            {
                Match m = matcher.Match(textData);
                if(m.Success)
                {
                    token = new Token(m.Value.Trim(), m.Value, tokenType);
                    return true;
                }
                token = null;
                return false;
            }
        }

        private class Token
        {
            public readonly TokenType tokenType;
            public readonly string data;
            public readonly string match;
            public Token(string data, string matchstr, TokenType type)
            {
                this.data = data;
                this.tokenType = type;
                this.match = matchstr;
            }
        }
    }
}