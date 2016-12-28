using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ever_Afters.common.Enums;
using Ever_Afters.common.Listeners;
using Ever_Afters.common.Models;

namespace Ever_Afters.common.Core
{
    public class MathEngine : InputChangedListener
    {
        #region SingleTon

        #region Fields & Properties
        private bool vplong = false;

        private const int RenderEngineLeadTime = 40; //In Ms
        public IVisualisationHandler Screen { get; set; }
        public IMathRequestHandler Database { get; set; }

        public MathProblem CurrentProblem { get; private set; }

        private Random _random;

        public Random Generator => _random ?? (_random = new Random());

        #endregion

        private static MathEngine _engine;

        public static MathEngine CurrentInstance => _engine ?? (_engine = new MathEngine(new Skel_Db(), new Skel_Screen()));

        public MathEngine(IMathRequestHandler db, IVisualisationHandler screen)
        {
            Database = db;
            Screen = screen;

            //Make sure that the resources directory exists
            if (!Directory.Exists(Engine.ResourcePath)) Directory.CreateDirectory(Engine.ResourcePath);
        }

        #endregion

        #region Core

        #region Generators
        
        private MathProblem GenerateRandomEquation()
        {
            //1. Generate two random input numbers
            MathProblem problem = new MathProblem
            {
                input1 = GenerateLowInteger(),
                input2 = GenerateLowInteger()
            };

            //2. Generate a random term
            do
            {
                int enumCount = Enum.GetNames(typeof(MathTerm)).Length;
                int answer_rand = Generator.Next(0, enumCount);
                problem.Term = (MathTerm) answer_rand;
            } while (problem.Term == MathTerm.EQUALS || problem.Term == MathTerm.NOTEQUALS);

            //3. Recalculate the generated question -> The answer has to be an positive integer.
            int? answer = CalculateAnswerProblem(problem);
            if (!answer.HasValue || answer.Value < 0) return GenerateRandomEquation();
            problem.output = answer.Value;
            return problem;
        }

        private int GenerateLowInteger()
        {
            //We generate a random devisor, from 0 to 100 in order to divide with 1, creating a hyperbole.
            //In this way, we have a higher chance to have a low integer as outcome. This to simplify the equations for the user.
            decimal divisor = Generator.Next(50) + 1;
            decimal hyperbole = 1 / divisor;
            decimal result = Math.Ceiling(hyperbole * 15);

            return Convert.ToInt32(result);
        }

        #endregion

        #region Calculators

        public int? CalculateAnswerProblem(MathProblem problem)
        {
            int? result = null;

            switch (problem.Term)
            {
                case MathTerm.PLUS:
                    result = problem.input1 + problem.input2;
                    break;
                case MathTerm.MINUS:
                    result = problem.input1 - problem.input2;
                    break;
                case MathTerm.TIMES:
                    result = problem.input1 * problem.input2;
                    break;
                case MathTerm.DIVIDE:
                    double r = (double)problem.input1 / (double)problem.input2;
                    double r_to_int = Convert.ToDouble(Convert.ToInt32(r));
                    if (r - r_to_int == 0) result = Convert.ToInt32(r_to_int);
                    break;
            }

            return result;
        }

        private List<MathTerm> FindCorrectTerms(MathProblem problem)
        {
            List<MathTerm> answers = new List<MathTerm>();

            int? correctSolution = CalculateAnswerProblem(problem);
            if (!correctSolution.HasValue) return answers;
            foreach (MathTerm term in Enum.GetValues(typeof(MathTerm)))
            {
                MathProblem newProblem = problem;
                newProblem.Term = term;
                int? newsolution = CalculateAnswerProblem(newProblem);

                if(newsolution.HasValue && newsolution.Value == correctSolution.Value)
                    answers.Add(term);
            }

            return answers;
        }

        private List<int> FindCorrectAnswersInput1(MathProblem problem)
        {
            List<int> answers = new List<int>();

            int? correctSolution = CalculateAnswerProblem(problem);
            if (!correctSolution.HasValue) return answers;
            for (int i = 0; i <= 10; i++)
            {
                MathProblem newProblem = problem;
                newProblem.input1 = i;
                int? newSolution = CalculateAnswerProblem(newProblem);

                if(newSolution.HasValue && newSolution.Value == correctSolution.Value)
                    answers.Add(i);
            }

            return answers;
        }

        private List<int> FindCorrectAnswersInput2(MathProblem problem)
        {
            List<int> answers = new List<int>();

            int? correctSolution = CalculateAnswerProblem(problem);
            if (!correctSolution.HasValue) return answers;
            for (int i = 0; i <= 10; i++)
            {
                MathProblem newProblem = problem;
                newProblem.input2 = i;
                int? newSolution = CalculateAnswerProblem(newProblem);

                if (newSolution.HasValue && newSolution.Value == correctSolution.Value)
                    answers.Add(i);
            }

            return answers;
        }

        #endregion

        #region Logic

        public void StartupMathEngine()
        {
            //1. Activate the vplong.
            vplong = true;

            //2. Start the beginvideo
            TimeSpan waitTime = TriggerVideo(MathVideos.BEGIN);

            //3. Kickstart the engine
            var asynctask = new Task(async () =>
            {
                await Ignite(waitTime.TotalSeconds);
            });
            asynctask.Start();
        }

        public void ShutdownMathEngine()
        {
            //1. Delete the background video
            Engine.CurrentEngine.OnQueueClearRequest(true);

            //2. De-activate the vplong.
            vplong = false;
        }

        private async Task Ignite(double wait = 0)
        {
            //1. Wait for the video to complete
            TimeSpan waitTime = TimeSpan.FromSeconds(wait);

            //2. Give Mathproblem
            await Task.Delay(waitTime);
            await ShowMathProblem();
        }

        private async Task ShowMathProblem()
        {
            //1. Generate a valid equation.
            MathProblem problem = GenerateRandomEquation();

            //2. Generate a question type
            double mpt_rand = Generator.NextDouble();

            //3. Determine Answer           
            if (mpt_rand > 0.9)
            {
                //a. Set question type
                problem.Type = MathProblemType.TermExpected;

                //b. Determine the term that needs to be eliminated
                List<MathTerm> answer = new List<MathTerm>();
                double elim_rand = Generator.NextDouble();
                if (elim_rand > 0.7)
                {
                    //Eliminate the = sign.
                    answer.Add(MathTerm.EQUALS);
                    problem.Question = problem.input1 + MathProblem.TermString(problem.Term) + problem.input2 + " ? " +
                                       problem.output;
                } else
                {
                    //Eliminate the + - * / signs
                    problem.Question = problem.input1 + " ? " + problem.input2 + " = " + problem.output;
                    answer = FindCorrectTerms(problem);
                }

                //c. Lookup the answer cube
                foreach (MathTerm t in answer)
                    foreach (Tag tag in Database.GiveTermTag(t))
                        problem.ExpectedAnswer.Add(tag);
                

            } else if (mpt_rand > 0.45)
            {
                //a. Set question type
                problem.Type = MathProblemType.AnswerExpected;

                //b. Eliminate the answer
                problem.Question = problem.input1 + MathProblem.TermString(problem.Term) + problem.input2 + " = ?";

                //c. Lookup the answer cube
                problem.ExpectedAnswer = Database.GiveNumberTag(problem.output);

            } else
            {
                //a. Set question type
                problem.Type = MathProblemType.QuestionExpected;

                //b. Determine the input that needs to be eliminated
                List<int> answer;
                double elim_rand = Generator.NextDouble();
                if (elim_rand > 0.5)
                {
                    //Eliminate the first input.
                    problem.Question = " ? " + MathProblem.TermString(problem.Term) + problem.input2 + " = " +
                                       problem.output;
                    answer = FindCorrectAnswersInput1(problem);
                } else
                {
                    //Eliminate the second input.
                    problem.Question = problem.input1 + MathProblem.TermString(problem.Term) + " ? = " +
                                       problem.output;
                    answer = FindCorrectAnswersInput2(problem);
                }

                //c. Lookup the answer cube
                foreach(int i in answer)
                    foreach(Tag tag in Database.GiveNumberTag(i))
                        problem.ExpectedAnswer.Add(tag);
            }

            //4. Check if the rendered question is possible to recreate with given blocks. (aka, is a tag found for asked answer.
            if (problem.ExpectedAnswer != null)
            {
                //5. Listen for the Expected answer
                CurrentProblem = problem;

                //6. Send the answer to the screen if allowed
                if(vplong) Screen.OverlayManager(problem.Question);
            }
            else await ShowMathProblem(); //Keep retrying.
        }

        #endregion

        #region Movie

        private TimeSpan TriggerVideo(MathVideos vid)
        {
            if (!vplong) return TimeSpan.Zero;

            //1. Get the physical Video
            Video toPlay = Database.GiveVideo(vid);
            if(toPlay == null) return TimeSpan.Zero;

            //2. Issue Video with priority
            ClearAndPlay(toPlay);

            //3. Dummy return
            return TimeSpan.Zero;
        }

        private void ClearAndPlay(Video toPlay)
        {
            //1. Clear the current queue
            Engine.CurrentEngine.OnQueueClearRequest(true);

            //2. Issue the requested video to play first
            Queue.AddToQueue(toPlay, QueuePosition.Priority);

            //3. Issue the wait video
            Queue.AddToQueue(Database.GiveVideo(MathVideos.MID));

            //4. Kick the engine
            Engine.CurrentEngine.VideoStarted();
        }

        #endregion

        #endregion

        #region InputChangedListenerInterface

        public void OnTagAdded(Sensors sensor, string TagIdentifier)
        {
            //Log to console
            Debug.WriteLine("Tag added: " + TagIdentifier);

            //1. Check if an answer is expected
            if (CurrentProblem != null)
            {
                //2. Check if given answer is expected
                if ((from a in CurrentProblem.ExpectedAnswer where a.name.Equals(TagIdentifier) select a.name).Any())
                {
                    //3a. Issue the 'well done' video
                    TimeSpan waitTime = TriggerVideo(MathVideos.END_GOOD);

                    //4. Issue the next math question
                    CurrentProblem = null;
                    Task.Run(async () =>
                    {
                        await Task.Delay(waitTime);
                        if(CurrentProblem == null) StartupMathEngine(); //If no other question has been asked, ask again.
                    });
                }
                else
                {
                    //3b. Issue the 'oh no' video 
                    TriggerVideo(MathVideos.END_BAD);
                }
            }
            else
            {
                StartupMathEngine(); //Ask another question
            }
        }

        public void OnTagRemoved(Sensors sensor)
        {
            
        }

        public bool OnQueueClearRequest(bool force)
        {
            if (!force) return false;
            CurrentProblem = null;
            return true;
        }

        #endregion
    }
}
