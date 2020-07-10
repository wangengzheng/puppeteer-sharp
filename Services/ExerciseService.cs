using Dapper;
using puppeteer_sharp.Models;
using puppeteer_sharp.Models.Exercise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace puppeteer_sharp.Services
{
    public class ExerciseService
    {
        OosHelper oosHelper;
        public ExerciseService(OosHelper oosHelper)
        {
            this.oosHelper = oosHelper;
        }

        public async Task<ExerciseModel> GetById(string exerciseId)
        {
            var sql = @"select 
                a.id,
                a.question,
                a.analysis,
                a.answer,
                a.`code`,
                a.questionHtml2Image,
                a.analysisHtml2Image,
                a.answerHtml2Image,
                a.exerciseTypeId,
                a.hard,
                a.`options`,
                a.questionSummary,
                a.shareLevel,
                b.exerciseId,
                b.orderBy,
                b.content,
                b.audio,
                b.image,
                b.id optionId
                from k_exercise as a
                left join k_exercise_option as b on b.exerciseId=a.id
                where a.id=@exerciseId
                order by b.orderBy";

            List<ExerciseModel> result = new List<ExerciseModel>();
            using (var conn = oosHelper.GetOpenConnection())
            {               

                Dictionary<string, ExerciseModel> dict = new Dictionary<string, ExerciseModel>();

                result = (await conn.QueryAsync<ExerciseModel, ExerciseOptionModel, ExerciseModel>(sql, (q, s) => {

                    ExerciseModel exercise;

                    if (!dict.TryGetValue(q.Id, out exercise))
                    {
                        exercise = q;
                        dict.Add(q.Id, exercise);
                    }

                    if (s != null)
                    {
                        exercise.ExerciseOptions.Add(s);
                    }

                    return exercise;
                }, new { exerciseId }, splitOn: "exerciseId")).Distinct().ToList();

            }

            return result.FirstOrDefault(b=>b.Id==exerciseId);
        }

    }
}
