using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ever_Afters.common.Core;
using Ever_Afters.common.Listeners;
using Ever_Afters.common.Models;

namespace Ever_Afters.common.DatabaseLayer
{
    public class MathPackDatapool : IMathRequestHandler
    {
        #region Singleton
        int _id = Int32.MaxValue;

        private static MathPackDatapool _pool;

        public static MathPackDatapool CurrentInstance => _pool ?? (_pool = new MathPackDatapool());

        #endregion

        public List<Tag> GiveTermTag(MathTerm term)
        {
            //1. Resolve the term
            List<String> ids = ResolveSymbol(term);

            //2. Check if an answer came
            if (ids.Count == 0) return null;

            //3. Convert to tags
            return (from i in ids select GetTagFromIdentifier(i)).ToList();
        }

        public List<Tag> GiveNumberTag(int value)
        {
            //1. Resolve the term
            List<String> ids = ResolveSymbol(value);

            //2. Check if an answer came
            if (ids.Count == 0) return null;

            //3. Convert to tags
            return (from i in ids select GetTagFromIdentifier(i)).ToList();
        }

        public Video GiveVideo(MathVideos video)
        {
            string p = Engine.ResourcePath;
            switch (video)
            {
                case MathVideos.BEGIN:
                    return new Video() {BasePath = Path.Combine(p, "MATH_BEGIN.mp4"), BaseStartsOnScreen = true, id = 0, OffScreenEndingPath = null, OnScreenEndingPath = null, Request_TAG = "", TAGS = new List<string>()};
                case MathVideos.MID:
                    return new Video() { BasePath = Path.Combine(p, "MATH_WAIT.mp4"), BaseStartsOnScreen = true, id = 0, OffScreenEndingPath = null, OnScreenEndingPath = null, Request_TAG = "", TAGS = new List<string>() };
                case MathVideos.END_BAD:
                    return new Video() { BasePath = Path.Combine(p, "MATH_END_BAD.mp4"), BaseStartsOnScreen = true, id = 0, OffScreenEndingPath = null, OnScreenEndingPath = null, Request_TAG = "", TAGS = new List<string>() };
                case MathVideos.END_GOOD:
                    return new Video() { BasePath = Path.Combine(p, "MATH_END_GOOD.mp4"), BaseStartsOnScreen = true, id = 0, OffScreenEndingPath = null, OnScreenEndingPath = null, Request_TAG = "", TAGS = new List<string>() };
            }
            return null;
        }

        #region Database

        private Tag GetTagFromIdentifier(String TagIdentifier)
        {
            return new Tag() {id = _id--, name = TagIdentifier};
        }

        public static List<String> ResolveSymbol(MathTerm term)
        {
            List<String> response = new List<String>();
            switch (term)
            {
                case MathTerm.PLUS:
                    response.Add("804e1a721bee04");
                    break;
                case MathTerm.MINUS:
                    response.Add("814e1a721c0304");
                    break;
                case MathTerm.TIMES:
                    response.Add("814e1a721d3304");
                    break;
                case MathTerm.DIVIDE:
                    response.Add("814e1a721d0504");
                    break;
                case MathTerm.EQUALS:
                    response.Add("814e1a721a1004");
                    break;
                case MathTerm.NOTEQUALS:
                    response.Add("814e1a721d1304");
                    break;
            }
            return response;
        }

        public static List<String> ResolveSymbol(int number)
        {
            List<String> response = new List<String>();
            switch (number)
            {
                case 0:
                    response.Add("814e1a721b0004");
                    response.Add("804e1a721dcc04");
                    break;
                case 1:
                    response.Add("804e1a721c9204");
                    response.Add("804e1a4ad79d04");
                    break;
                case 2:
                    response.Add("804e1a721c9304");
                    response.Add("804e1a4ad7ab04");
                    break;
                case 3:
                    response.Add("804e1a4ac2b204");
                    response.Add("804e1a4ad7e004");
                    break;
                case 4:
                    response.Add("804e1a4ac2c004");
                    response.Add("804e1a721c7304");
                    break;
                case 5:
                    response.Add("804e1a721c7404");
                    response.Add("804e1a4ad77d04");
                    break;
                case 6:
                    response.Add("804e1a721ca404");
                    response.Add("804e1a4ad7ee04");
                    response.Add("804e1a4ac2df04");
                    response.Add("814e1a721d1404");
                    break;
                case 7:
                    response.Add("804e1a721bef04");
                    response.Add("814e1a721c2104");
                    break;
                case 8:
                    response.Add("804e1a4ac2c104");
                    response.Add("804e1a721dbe04");
                    break;
                case 9:
                    response.Add("814e1a721b2004");
                    response.Add("814e1a721d0604");
                    break;
                case 10:
                    response.Add("804e1a4ac2b304");
                    response.Add("814e1a721d3404");
                    break;
            }
            return response;
        }
        
        #endregion
    }
}
