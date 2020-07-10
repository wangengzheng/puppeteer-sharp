using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace puppeteer_sharp.Models.Exercise
{
    public class ExerciseModel
    {
        public string Id { get; set; }

        public string Question { get; set; }

        public string Analysis { get; set; }

        public string Answer { get; set; }

        public string Code { get; set; }

        public string ExerciseTypeId { get; set; }

        public string QuestionHtml2Image { get; set; }

        public string AnalysisHtml2Image { get; set; }

        public string AnswerHtml2Image { get; set; }

        public int Hard { get; set; }

        public int Options { get; set; }

        public string QuestionSummary { get; set; }

        public int ShareLevel { get; set; }

        public List<ExerciseOptionModel> ExerciseOptions { get; set; } = Enumerable.Empty<ExerciseOptionModel>().ToList();
    }
}
