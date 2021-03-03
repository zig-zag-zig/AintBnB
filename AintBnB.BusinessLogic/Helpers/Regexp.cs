﻿using System.Text.RegularExpressions;

namespace AintBnB.BusinessLogic.Helpers
{
    public static class Regexp
    {
        public static Regex onlyLettersOneSpaceOrDash = new Regex(@"^(?=.{2,}$)([A-Za-z\u00C0-\u00D6\u00D8-\u00f6\u00f8-\u00ff]+([\s-]?[A-Za-z\u00C0-\u00D6\u00D8-\u00f6\u00f8-\u00ff]+){0,})$");
        public static Regex onlyLettersNumbersOneSpaceOrDash = new Regex(@"^(?=.{2,}$)([A-Za-z\u00C0-\u00D6\u00D8-\u00f6\u00f8-\u00ff0-9]([\s-]?[A-Za-z\u00C0-\u00D6\u00D8-\u00f6\u00f8-\u00ff0-9]+)*)$");
        public static Regex onlyNumbersFollowedByAnOptionalLetter = new Regex(@"^[1-9]+[0-9]*[A-Za-z]?$");
        public static Regex zipCodeFormatsOfTheWorld = new Regex(@"^(?=.{2,10}$)([A-Za-z\u00C0-\u00D6\u00D8-\u00f6\u00f8-\u00ff0-9]([\s-]?[A-Za-z\u00C0-\u00D6\u00D8-\u00f6\u00f8-\u00ff0-9]+)*)$");
    }
}