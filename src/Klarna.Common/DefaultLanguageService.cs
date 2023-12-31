﻿using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using EPiServer.DataAbstraction;
using EPiServer.Globalization;

namespace Klarna.Common
{
    public class DefaultLanguageService : ILanguageService
    {
        private readonly ILanguageBranchRepository _languageBranchRepository;

        public DefaultLanguageService(ILanguageBranchRepository languageBranchRepository)
        {
            _languageBranchRepository = languageBranchRepository;
        }

        public CultureInfo GetPreferredCulture()
        {
            var languages = _languageBranchRepository.ListEnabled();
            var currentLanguage = ContentLanguage.PreferredCulture;

            if (languages.Any(x => x.Culture.Name == Thread.CurrentThread.CurrentCulture.Name))
            {
                currentLanguage = Thread.CurrentThread.CurrentCulture;
            }
            else if (languages.Any(x => x.Culture.Name == Thread.CurrentThread.CurrentCulture.Parent.Name))
            {
                currentLanguage = Thread.CurrentThread.CurrentCulture.Parent;
            }

            return currentLanguage;
        }

        /// <summary>
        /// Supported languages and locales: https://docs.klarna.com/klarna-checkout/in-depth-knowledge/available-languages/
        /// </summary>
        public string ConvertToLocale(string languageId)
        {
            var supportedLanguages = new[] {"de", "nl", "fi", "it", "pl", "pt", "es"};

            var languageCode = languageId.Substring(0, 2);

            if (supportedLanguages.Contains(languageCode))
            {
                return languageCode;
            }

            if (languageId.StartsWith("no"))
            {
                return "nb";
            }

            if (languageId.StartsWith("sv") && !languageId.Equals("sv-FI", StringComparison.InvariantCultureIgnoreCase))
            {
                return "sv";
            }

            return languageId;
        }
    }
}
