using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;

using Dx29.Services;

namespace Dx29.Web.Services
{
    public partial class LocalizationService
    {
        public event EventHandler CultureChanged;

        public LocalizationService(IHttpClientFactory httpClientFactory, ILogService logService)
        {
            logService.Info("LocalizationService ctor");
            Culture = new CultureInfo("en-US");
            Language = Culture.TwoLetterISOLanguageName;

            HttpClient = httpClientFactory.CreateClient("Dx29.Web.API");
        }

        public HttpClient HttpClient { get; }
        public CultureInfo Culture { get; private set; }

        public Dictionary<string, string> Literals { get; private set; }

        public string Language { get; private set; }

        public string this[string key] => TryGetLiteral(key);
        public string this[string key, params object[] args] => Localize(key, args);
        public string this[double value] => Localize(value);
        public string this[DateTime value] => Localize(value);
        public string this[object value] => Localize(value);

        private string Localize(string key)
        {
            return TryGetLiteral(key);
        }
        private string Localize(string key, params object[] args)
        {
            return String.Format(Culture, TryGetLiteral(key), args);
        }
        private string Localize(string key, params string[] args)
        {
            return String.Format(TryGetLiteral(key), args);
        }
        private string Localize(double value) => value.ToString(Culture);
        private string Localize(DateTime value) => value.ToString(Culture);
        private string Localize(object value) => String.Format(Culture, "{0}", value);
    }
}
