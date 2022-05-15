using System.Net;
using System.Text.RegularExpressions;

namespace homeautomation_service.Helper
{
    internal class Parser : IHTMLParse
    {
        private object _dataObj;
        private readonly int _interval;
        private  string _url;
        private IHTMLParseResponse _response;
        Timer _stateTimer ;

        public Parser(object dataObj, int intervalSec, string url)
        {

            _dataObj = dataObj;
            _interval = intervalSec;
            _url = url;
        }

        public void CyclicParseFromUrl(IHTMLParseResponse response)
        {
            _response = response;
            _stateTimer = new(ParseFromUrl, null, 0, 1000 * _interval);
        }
        private void ParseFromUrl(object state)
        {
            try
            {
                using var client = new WebClient();
                client.Headers.Add("User-Agent", "C#");
                string content = client.DownloadString(_url);
                //string content = client.GetStringAsync(_url).ToString();

                //var _fileContent = System.IO.File.ReadAllText(_filePath);
                if (content != null)
                {
                    foreach (var prop in _dataObj.GetType().GetProperties())
                    {
                        try
                        {
                            object[] attrs = prop.GetCustomAttributes(true);
                            foreach (object attr in attrs)
                            {
                                if (attr is HTMLParserAttribute htmlAttr)
                                {
                                    string pattern = @"(?<=" + htmlAttr.Name + @")(?:.*)(?<=value=\"")(.*)(?=\"" )";
                                    Regex rg = new Regex(pattern);
                                    MatchCollection matched = rg.Matches(content);
                                    if (matched.Count > 0)
                                    {
                                        if (matched[0].Groups.Count > 1)
                                        {
                                            if (prop.PropertyType == typeof(string))
                                            {
                                                prop.SetValue(_dataObj, matched[0].Groups[1].Value.ToString());
                                            }
                                            if (prop.PropertyType == typeof(bool))
                                            {
                                                prop.SetValue(_dataObj, Convert.ToBoolean(matched[0].Groups[1].Value.Replace(".", ",")));
                                            }
                                            if (prop.PropertyType == typeof(double))
                                            {
                                                prop.SetValue(_dataObj, Convert.ToDouble(matched[0].Groups[1].Value.Replace(".", ",")));
                                            }
                                            if (prop.PropertyType == typeof(int))
                                            {
                                                prop.SetValue(_dataObj, Convert.ToInt32(matched[0].Groups[1].Value.Replace(".", ",")));
                                            }
                                        }
                                    }
                                    ;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            ;
                        }
                    }
                }
                _response.ReceiveParserResponse(_dataObj);
            }
            catch (Exception)
            {

                
            }
            
        }
    }

    public interface IHTMLParse
    {
        public void CyclicParseFromUrl(IHTMLParseResponse response);
    }
    public interface IHTMLParseResponse
    {
        public void ReceiveParserResponse(dynamic result);
    }

    internal class HTMLParserAttribute : Attribute
    {
        public string Name { get; set; }
    }
}