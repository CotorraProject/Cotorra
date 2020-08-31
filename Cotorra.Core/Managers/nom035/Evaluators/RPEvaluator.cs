using Cotorra.Schema;
using Cotorra.Schema.nom035;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cotorra.Core.Managers.nom035
{
    public class RPEvaluator
    {
        private readonly Dictionary<int, int[]> PointsPerQuestion = new Dictionary<int, int[]>();
        readonly int[] Answers1Array = new int[] { 0, 1, 2, 3, 4 };
        readonly int[] Answers2Array = new int[] { 4, 3, 2, 1, 0 };
        readonly int[] ItemsBlock1 = new int[] { 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33 };
        readonly int[] ItemsBlock2 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46 };

        const string NULL_OR_DISCRIMINATE = "Nulo o despreciable";
        const string LOW = "Bajo";
        const string MEDIUM = "Medio";
        const string HIGH = "Alto";
        const string VERY_HIGH = "Muy Alto";

        const int NULL_OR_DISCRIMINATE_INT = 0;
        const int LOW_INT = 1;
        const int MEDIUM_INT = 2;
        const int HIGH_INT = 3;
        const int VERY_HIGH_INT = 4;

        List<Category> categories = new List<Category>();
        
        public RPEvaluator()
        {
            PopulatePointsData();
            PopulateCategories();
        }


        public NOMSurveyReply ProcessFinalization(NOMSurveyReply answeredSurvey, List<NOMAnswer> answers,
             List<NOMEvaluationQuestion> aTSQuestions, List<NOMEvaluationCategory> categories, List<NOMEvaluationDomain> domains)
        {
            NOMSurveyCategoryResult category1Result = new NOMSurveyCategoryResult();
            NOMSurveyCategoryResult category2Result = new NOMSurveyCategoryResult();
            NOMSurveyCategoryResult category3Result = new NOMSurveyCategoryResult();
            NOMSurveyCategoryResult category4Result = new NOMSurveyCategoryResult();

            NOMSurveyDomainResult domain1Result = new NOMSurveyDomainResult();
            NOMSurveyDomainResult domain2Result = new NOMSurveyDomainResult();
            NOMSurveyDomainResult domain3Result = new NOMSurveyDomainResult();
            NOMSurveyDomainResult domain4Result = new NOMSurveyDomainResult();
            NOMSurveyDomainResult domain5Result = new NOMSurveyDomainResult();
            NOMSurveyDomainResult domain6Result = new NOMSurveyDomainResult();
            NOMSurveyDomainResult domain7Result = new NOMSurveyDomainResult();
            NOMSurveyDomainResult domain8Result = new NOMSurveyDomainResult();


            answeredSurvey.NOMSurveyCategoryResult = new List<NOMSurveyCategoryResult>();
            answeredSurvey.NOMSurveyDomainResult = new List<NOMSurveyDomainResult>();


            answeredSurvey.NOMSurveyCategoryResult.Add(category1Result);
            answeredSurvey.NOMSurveyCategoryResult.Add(category2Result);
            answeredSurvey.NOMSurveyCategoryResult.Add(category3Result);
            answeredSurvey.NOMSurveyCategoryResult.Add(category4Result);

            answeredSurvey.NOMSurveyDomainResult.Add(domain1Result);
            answeredSurvey.NOMSurveyDomainResult.Add(domain2Result);
            answeredSurvey.NOMSurveyDomainResult.Add(domain3Result);
            answeredSurvey.NOMSurveyDomainResult.Add(domain4Result);
            answeredSurvey.NOMSurveyDomainResult.Add(domain5Result);
            answeredSurvey.NOMSurveyDomainResult.Add(domain6Result);
            answeredSurvey.NOMSurveyDomainResult.Add(domain7Result);
            answeredSurvey.NOMSurveyDomainResult.Add(domain8Result);
             
            var firstCategoryID = categories.FirstOrDefault(x => x.Number == 1).ID;
            var secondCategoryID = categories.FirstOrDefault(x => x.Number == 2).ID;
            var thirdCategoryID = categories.FirstOrDefault(x => x.Number == 3).ID;
            var fourthCategoryID = categories.FirstOrDefault(x => x.Number == 4).ID;

            var firstDomainID = domains.FirstOrDefault(x => x.Number == 1).ID;
            var secondDomainID = domains.FirstOrDefault(x => x.Number == 2).ID;
            var thirdDomainID = domains.FirstOrDefault(x => x.Number == 3).ID;
            var fourthDomainID = domains.FirstOrDefault(x => x.Number == 4).ID;
            var fifthDomainID = domains.FirstOrDefault(x => x.Number == 5).ID;
            var sixthDomainID = domains.FirstOrDefault(x => x.Number == 6).ID;
            var seventhDomainID = domains.FirstOrDefault(x => x.Number == 7).ID;
            var eigthDomainID = domains.FirstOrDefault(x => x.Number == 8).ID;

            AssignPoints(answers, aTSQuestions, firstCategoryID, secondCategoryID, thirdCategoryID,
                fourthCategoryID, firstDomainID, secondDomainID, thirdDomainID, fourthDomainID,
                fifthDomainID, sixthDomainID, seventhDomainID, eigthDomainID, category1Result, category2Result, category3Result, category4Result,
                    domain1Result, domain2Result, domain3Result, domain4Result, domain5Result, domain6Result,
                    domain7Result, domain8Result, answeredSurvey);

            return answeredSurvey;
        }

        private void AssignPoints(List<NOMAnswer> answers, List<NOMEvaluationQuestion> aTSQuestions, Guid firstCategoryID, Guid secondCategoryID, Guid thirdCategoryID, Guid fourthCategoryID, Guid firstDomainID, Guid secondDomainID,
             Guid thirdDomainID, Guid fourthDomainID, Guid fifthDomainID, Guid sixthDomainID,
             Guid seventhDomainID, Guid eigthDomainID, NOMSurveyCategoryResult category1Result, NOMSurveyCategoryResult category2Result,   NOMSurveyCategoryResult category3Result, NOMSurveyCategoryResult category4Result, NOMSurveyDomainResult domain1Result,
            NOMSurveyDomainResult domain2Result, NOMSurveyDomainResult domain3Result, NOMSurveyDomainResult domain4Result, NOMSurveyDomainResult domain5Result, NOMSurveyDomainResult domain6Result,
            NOMSurveyDomainResult domain7Result, NOMSurveyDomainResult domain8Result, NOMSurveyReply answeredSurvey)
        {
            var totalPoints = 0;
            var category1Points = 0;
            var category2Points = 0;
            var category3Points = 0;
            var category4Points = 0;
            var domain1Points = 0;
            var domain2Points = 0;
            var domain3Points = 0;
            var domain4Points = 0;
            var domain5Points = 0;
            var domain6Points = 0;
            var domain7Points = 0;
            var domain8Points = 0;

            //answers.ForEach(answer =>
            Parallel.ForEach(answers, answer =>
            {
                var question = aTSQuestions.FirstOrDefault(x => x.ID == answer.NOMEvaluationQuestionID);
                var questionPoints = GetPoints(question.Number, answer.Value);
                Interlocked.Add(ref totalPoints, questionPoints);
                if (question.NOMEvaluationCategoryID == firstCategoryID)
                {
                    Interlocked.Add(ref category1Points, questionPoints);
                }
                if (question.NOMEvaluationCategoryID == secondCategoryID)
                {
                    Interlocked.Add(ref category2Points, questionPoints);
                }
                if (question.NOMEvaluationCategoryID == thirdCategoryID)
                {
                    Interlocked.Add(ref category3Points, questionPoints);
                }
                if (question.NOMEvaluationCategoryID == fourthCategoryID)
                {
                    Interlocked.Add(ref category4Points, questionPoints);
                }
                if (question.NOMEvaluationDomainID == firstDomainID)
                {
                    Interlocked.Add(ref domain1Points, questionPoints);
                }
                if (question.NOMEvaluationDomainID == secondDomainID)
                {
                    Interlocked.Add(ref domain2Points, questionPoints);
                }
                if (question.NOMEvaluationDomainID == thirdDomainID)
                {
                    Interlocked.Add(ref domain3Points, questionPoints);
                }
                if (question.NOMEvaluationDomainID == fourthDomainID)
                {
                    Interlocked.Add(ref domain4Points, questionPoints);
                }
                if (question.NOMEvaluationDomainID == fifthDomainID)
                {
                    Interlocked.Add(ref domain5Points, questionPoints);
                }
                if (question.NOMEvaluationDomainID == sixthDomainID)
                {
                    Interlocked.Add(ref domain6Points, questionPoints);
                }
                if (question.NOMEvaluationDomainID == seventhDomainID)
                {
                    Interlocked.Add(ref domain7Points, questionPoints);
                }
                if (question.NOMEvaluationDomainID == eigthDomainID)
                {
                    Interlocked.Add(ref domain8Points, questionPoints);
                }
            });
            SetValuesForResults(firstCategoryID, secondCategoryID, thirdCategoryID, fourthCategoryID, firstDomainID, secondDomainID, thirdDomainID, fourthDomainID, fifthDomainID, sixthDomainID, seventhDomainID, eigthDomainID, category1Result, category2Result, category3Result, category4Result, domain1Result, domain2Result, domain3Result, domain4Result, domain5Result, domain6Result, domain7Result, domain8Result, category1Points, category2Points, category3Points, category4Points, domain1Points, domain2Points, domain3Points, domain4Points, domain5Points, domain6Points, domain7Points, domain8Points);

            answeredSurvey.Result = totalPoints;
        }

        private static void SetValuesForResults(Guid firstCategoryID, Guid secondCategoryID, Guid thirdCategoryID, Guid fourthCategoryID, Guid firstDomainID, Guid secondDomainID, Guid thirdDomainID, Guid fourthDomainID, Guid fifthDomainID, Guid sixthDomainID, Guid seventhDomainID, Guid eigthDomainID, NOMSurveyCategoryResult category1Result, NOMSurveyCategoryResult category2Result, NOMSurveyCategoryResult category3Result, NOMSurveyCategoryResult category4Result, NOMSurveyDomainResult domain1Result, NOMSurveyDomainResult domain2Result, NOMSurveyDomainResult domain3Result, NOMSurveyDomainResult domain4Result, NOMSurveyDomainResult domain5Result, NOMSurveyDomainResult domain6Result, NOMSurveyDomainResult domain7Result, NOMSurveyDomainResult domain8Result, int category1Points, int category2Points, int category3Points, int category4Points, int domain1Points, int domain2Points, int domain3Points, int domain4Points, int domain5Points, int domain6Points, int domain7Points, int domain8Points)
        {
            category1Result.Result = category1Points;
            category1Result.NOMEvaluationCategoryID = firstCategoryID;
            category2Result.Result = category2Points;
            category2Result.NOMEvaluationCategoryID = secondCategoryID;
            category3Result.Result = category3Points;
            category3Result.NOMEvaluationCategoryID = thirdCategoryID;
            category4Result.Result = category4Points;
            category4Result.NOMEvaluationCategoryID = fourthCategoryID;

            domain1Result.Result = domain1Points;
            domain1Result.NOMEvaluationDomainID = firstDomainID;
            domain2Result.Result = domain2Points;
            domain2Result.NOMEvaluationDomainID = secondDomainID;

            domain3Result.Result = domain3Points;
            domain3Result.NOMEvaluationDomainID = thirdDomainID;
            domain4Result.Result = domain4Points;
            domain4Result.NOMEvaluationDomainID = fourthDomainID;
            domain5Result.Result = domain5Points;
            domain5Result.NOMEvaluationDomainID = fifthDomainID;
            domain6Result.Result = domain6Points;
            domain6Result.NOMEvaluationDomainID = sixthDomainID;
            domain7Result.Result = domain7Points;
            domain7Result.NOMEvaluationDomainID = seventhDomainID;
            domain8Result.Result = domain8Points;
            domain8Result.NOMEvaluationDomainID = eigthDomainID;
        }

        public int GetPoints(int questionNumber, int value)
        {

            PointsPerQuestion.TryGetValue(questionNumber, out int[] values);
            return values[value];
        }

        private void PopulatePointsData()
        {
            foreach (var item in ItemsBlock1)
            {
                PointsPerQuestion.Add(item, Answers1Array);
            }

            foreach (var item in ItemsBlock2)
            {
                PointsPerQuestion.Add(item, Answers2Array);
            }
        }

        private List<Category> PopulateCategories()
        {
            Category workEnvironment = new Category()
            {
                Description = "Ambiente de trabajo",
                Domains = new List<Domain>()
                {
                    new Domain()
                    {
                        Description = "Condiciones en el ambiente de trabajo",
                        Dimenssions = new List<Dimenssion>()
                        {
                            new Dimenssion()
                            {
                                Description = "Condiciones peligrosas e inseguras",
                                Items = new int[]{ 2 }
                            },
                             new Dimenssion()
                            {
                                Description = "Condiciones deficientes e insalubres",
                                Items = new int[]{ 1 }
                            },
                             new Dimenssion()
                            {
                                Description = "Trabajos peligrosos",
                                Items = new int[]{ 3 }
                            }
                        },
                        Ranges = new List<RangeEvaluation>()
                        {
                            new RangeEvaluation()
                            {
                                LowBoundary = 0,
                                HighBoundary = 3,
                                Result = NULL_OR_DISCRIMINATE_INT,
                                ResultDescription = NULL_OR_DISCRIMINATE
                            },
                             new RangeEvaluation()
                            {
                                LowBoundary = 2,
                                HighBoundary = 5,
                                Result = LOW_INT,
                                ResultDescription = LOW
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 4,
                                HighBoundary = 7,
                                Result = MEDIUM_INT,
                                ResultDescription = MEDIUM
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 6,
                                HighBoundary = 9,
                                Result = HIGH_INT,
                                ResultDescription = HIGH
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 8,
                                HighBoundary = int.MaxValue,
                                Result = VERY_HIGH_INT,
                                ResultDescription = VERY_HIGH
                            },
                        }
                    }, 
                },
                Ranges = new List<RangeEvaluation>()
                        {
                            new RangeEvaluation()
                            {
                                LowBoundary = 0,
                                HighBoundary = 3,
                                Result = NULL_OR_DISCRIMINATE_INT,
                                ResultDescription = NULL_OR_DISCRIMINATE
                            },
                             new RangeEvaluation()
                            {
                                LowBoundary = 2,
                                HighBoundary = 5,
                                Result = LOW_INT,
                                ResultDescription = LOW
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 4,
                                HighBoundary = 7,
                                Result = MEDIUM_INT,
                                ResultDescription = MEDIUM
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 6,
                                HighBoundary = 9,
                                Result = HIGH_INT,
                                ResultDescription = HIGH
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 9,
                                HighBoundary = int.MaxValue,
                                Result = VERY_HIGH_INT,
                                ResultDescription = VERY_HIGH
                            },
                        }

            };

            Category innerActivityFactors = new Category()
            {
                Description = "Factores propios de la actividad",
                Domains = new List<Domain>()
                {
                 new Domain()
                    {
                        Description = "Carga de trabajo",
                        Dimenssions = new List<Dimenssion>()
                        {
                            new Dimenssion()
                            {
                                Description = "Cargas cuantitativas",
                                Items = new int[]{ 4 , 9 }
                            },
                            new Dimenssion()
                            {
                                Description = "Ritmos de trabajo acelerado",
                                Items = new int[]{ 5, 6 }
                            },
                            new Dimenssion()
                            {
                                Description = "Carga mental",
                                Items = new int[]{ 7, 8 }
                            },
                            new Dimenssion()
                            {
                                Description = "Cargas psicológicas emocionales",
                                Items = new int[]{ 41, 42, 43 }
                            },
                            new Dimenssion()
                            {
                                Description = "Cargas de alta responsabilidad",
                                Items = new int[]{ 10, 11 }
                            },
                             new Dimenssion()
                            {
                                Description = "Cargas contradictorias o inconsistentes",
                                Items = new int[]{ 12, 13 }
                            },
                             
                        },
                        Ranges = new List<RangeEvaluation>()
                        {
                            new RangeEvaluation()
                            {
                                LowBoundary = 0,
                                HighBoundary = 12,
                                Result = NULL_OR_DISCRIMINATE_INT,
                                ResultDescription = NULL_OR_DISCRIMINATE
                            },
                             new RangeEvaluation()
                            {
                                LowBoundary = 11,
                                HighBoundary = 16,
                                Result = LOW_INT,
                                ResultDescription = LOW
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 15,
                                HighBoundary = 20,
                                Result = MEDIUM_INT,
                                ResultDescription = MEDIUM
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 19,
                                HighBoundary = 24,
                                Result = HIGH_INT,
                                ResultDescription = HIGH
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 23,
                                HighBoundary = int.MaxValue,
                                Result = VERY_HIGH_INT,
                                ResultDescription = VERY_HIGH
                            },
                        }

                 },
                 new Domain()
                    {
                        Description = "Falta de control sobre el trabajo",
                        Dimenssions = new List<Dimenssion>()
                        {
                            new Dimenssion()
                            {
                                Description = "Falta de control y autonomía sobre el trabajo",
                                Items = new int[]{ 20, 21, 22 }
                            },
                            new Dimenssion()
                            {
                                Description = "Limitada o nula posibilidad de desarrollo",
                                Items = new int[]{ 18, 19 }
                            },
                            new Dimenssion()
                            {
                                Description = "Limitada o inexistente capacitación",
                                Items = new int[]{ 26, 27 }
                            },

                        },
                        Ranges = new List<RangeEvaluation>()
                        {
                            new RangeEvaluation()
                            {
                                LowBoundary = 0,
                                HighBoundary = 5,
                                Result = NULL_OR_DISCRIMINATE_INT,
                                ResultDescription = NULL_OR_DISCRIMINATE
                            },
                             new RangeEvaluation()
                            {
                                LowBoundary = 4,
                                HighBoundary = 8,
                                Result = LOW_INT,
                                ResultDescription = LOW
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 7,
                                HighBoundary = 11,
                                Result = MEDIUM_INT,
                                ResultDescription = MEDIUM
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 10,
                                HighBoundary = 14,
                                Result = HIGH_INT,
                                ResultDescription = HIGH
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 13,
                                HighBoundary = int.MaxValue,
                                Result = VERY_HIGH_INT,
                                ResultDescription = VERY_HIGH
                            },
                        }
                    }
                 },
                Ranges = new List<RangeEvaluation>()
                        {
                            new RangeEvaluation()
                            {
                                LowBoundary = 0,
                                HighBoundary = 10,
                                Result = NULL_OR_DISCRIMINATE_INT,
                                ResultDescription = NULL_OR_DISCRIMINATE
                            },
                             new RangeEvaluation()
                            {
                                LowBoundary = 9,
                                HighBoundary = 20,
                                Result = LOW_INT,
                                ResultDescription = LOW
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 19,
                                HighBoundary = 30,
                                Result = MEDIUM_INT,
                                ResultDescription = MEDIUM
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 29,
                                HighBoundary = 40,
                                Result = HIGH_INT,
                                ResultDescription = HIGH
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 40,
                                HighBoundary = int.MaxValue,
                                Result = VERY_HIGH_INT,
                                ResultDescription = VERY_HIGH
                            },
                        }
            };

            Category workTimeOrganization = new Category()
            {
                Description = "Organización del tiempo de trabajo",
                Domains = new List<Domain>()
                {
                    new Domain()
                    {
                        Description = "Jornada de trabajo",
                        Dimenssions = new List<Dimenssion>()
                            {
                                new Dimenssion()
                                {
                                    Description = "Jornadas de trabajo extensas",
                                    Items = new int[]{ 14 , 15 }
                                },
                               
                            },
                        Ranges = new List<RangeEvaluation>()
                        {
                            new RangeEvaluation()
                            {
                                LowBoundary = 0,
                                HighBoundary = 1,
                                Result = NULL_OR_DISCRIMINATE_INT,
                                ResultDescription = NULL_OR_DISCRIMINATE
                            },
                             new RangeEvaluation()
                            {
                                LowBoundary = 0,
                                HighBoundary = 2,
                                Result = LOW_INT,
                                ResultDescription = LOW
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 1,
                                HighBoundary = 4,
                                Result = MEDIUM_INT,
                                ResultDescription = MEDIUM
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 3,
                                HighBoundary = 6,
                                Result = HIGH_INT,
                                ResultDescription = HIGH
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 5,
                                HighBoundary = int.MaxValue,
                                Result = VERY_HIGH_INT,
                                ResultDescription = VERY_HIGH
                            },
                        }
                    },
                    new Domain()
                    {
                        Description = "Interferencia en la relación trabajo-familia",
                        Dimenssions = new List<Dimenssion>()
                        {
                             new Dimenssion()
                                {
                                    Description = "Influencia del trabajo fuera del centro laboral",
                                    Items = new int[]{ 16 }
                                },
                              new Dimenssion()
                                {
                                    Description = "Influencia de las responsabilidades familiares",
                                    Items = new int[]{ 17 }
                                },
                        },
                        Ranges = new List<RangeEvaluation>()
                        {
                            new RangeEvaluation()
                            {
                                LowBoundary = 0,
                                HighBoundary = 1,
                                Result = NULL_OR_DISCRIMINATE_INT,
                                ResultDescription = NULL_OR_DISCRIMINATE
                            },
                             new RangeEvaluation()
                            {
                                LowBoundary = 0,
                                HighBoundary = 2,
                                Result = LOW_INT,
                                ResultDescription = LOW
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 1,
                                HighBoundary = 4,
                                Result = MEDIUM_INT,
                                ResultDescription = MEDIUM
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 3,
                                HighBoundary = 6,
                                Result = HIGH_INT,
                                ResultDescription = HIGH
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 5,
                                HighBoundary = int.MaxValue,
                                Result = VERY_HIGH_INT,
                                ResultDescription = VERY_HIGH
                            },
                        }
                    }
                },
                Ranges = new List<RangeEvaluation>()
                        {
                            new RangeEvaluation()
                            {
                                LowBoundary = 0,
                                HighBoundary = 4,
                                Result = NULL_OR_DISCRIMINATE_INT,
                                ResultDescription = NULL_OR_DISCRIMINATE
                            },
                             new RangeEvaluation()
                            {
                                LowBoundary = 3,
                                HighBoundary = 6,
                                Result = LOW_INT,
                                ResultDescription = LOW
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 5,
                                HighBoundary = 9,
                                Result = MEDIUM_INT,
                                ResultDescription = MEDIUM
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 8,
                                HighBoundary = 12,
                                Result = HIGH_INT,
                                ResultDescription = HIGH
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 11,
                                HighBoundary = int.MaxValue,
                                Result = VERY_HIGH_INT,
                                ResultDescription = VERY_HIGH
                            },
                        }
            };

            Category leaderShipAndWorkRelations = new Category()
            {
                Description = "Liderazgo y relaciones en el trabajo",
                Domains = new List<Domain>()
                {
                    new Domain()
                    {
                        Description = "Liderazgo",
                        Dimenssions = new List<Dimenssion>()
                        {
                            new Dimenssion()
                            {
                                Description = "Escasa claridad de funciones",
                                 Items = new int[]{ 23,24,25 }
                            },
                             new Dimenssion()
                            {
                                Description = "Características del liderazgo",
                                 Items = new int[]{28 , 29 }
                            }
                        },
                        Ranges = new List<RangeEvaluation>()
                        {
                            new RangeEvaluation()
                            {
                                LowBoundary = 0,
                                HighBoundary = 3,
                                Result = NULL_OR_DISCRIMINATE_INT,
                                ResultDescription = NULL_OR_DISCRIMINATE
                            },
                             new RangeEvaluation()
                            {
                                LowBoundary = 2,
                                HighBoundary = 5,
                                Result = LOW_INT,
                                ResultDescription = LOW
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 4,
                                HighBoundary = 8,
                                Result = MEDIUM_INT,
                                ResultDescription = MEDIUM
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 7,
                                HighBoundary = 11,
                                Result = HIGH_INT,
                                ResultDescription = HIGH
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 10,
                                HighBoundary = int.MaxValue,
                                Result = VERY_HIGH_INT,
                                ResultDescription = VERY_HIGH
                            },
                        }
                    },
                    new Domain()
                    {
                        Description = "Relaciones en el trabajo",
                        Dimenssions = new List<Dimenssion>()
                        {
                              new Dimenssion()
                                {
                                    Description = "Relaciones sociales en el trabajo",
                                     Items = new int[]{ 30, 31, 32 }
                                },
                              new Dimenssion()
                              {
                                    Description = "Deficiente relación con los colaboradores que supervisa",
                                    Items = new int[]{ 44, 45, 46 }
                              }
                        },
                        Ranges = new List<RangeEvaluation>()
                        {
                            new RangeEvaluation()
                            {
                                LowBoundary = 0,
                                HighBoundary = 5,
                                Result = NULL_OR_DISCRIMINATE_INT,
                                ResultDescription = NULL_OR_DISCRIMINATE
                            },
                             new RangeEvaluation()
                            {
                                LowBoundary = 4,
                                HighBoundary = 8,
                                Result = LOW_INT,
                                ResultDescription = LOW
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 7,
                                HighBoundary = 11,
                                Result = MEDIUM_INT,
                                ResultDescription = MEDIUM
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 10,
                                HighBoundary = 14,
                                Result = HIGH_INT,
                                ResultDescription = HIGH
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 13,
                                HighBoundary = int.MaxValue,
                                Result = VERY_HIGH_INT,
                                ResultDescription = VERY_HIGH
                            },
                        }
                    },
                    new Domain()
                    {
                        Description = "Violencia",
                        Dimenssions = new List<Dimenssion>()
                        {
                              new Dimenssion()
                                {
                                    Description = "Violencia laboral",
                                     Items = new int[]{ 33, 34, 35, 36, 37, 38, 39, 40 }
                                }
                        },

                        Ranges = new List<RangeEvaluation>()
                        {
                            new RangeEvaluation()
                            {
                                LowBoundary = 0,
                                HighBoundary = 7,
                                Result = NULL_OR_DISCRIMINATE_INT,
                                ResultDescription = NULL_OR_DISCRIMINATE
                            },
                             new RangeEvaluation()
                            {
                                LowBoundary = 6,
                                HighBoundary = 10,
                                Result = LOW_INT,
                                ResultDescription = LOW
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 9,
                                HighBoundary = 13,
                                Result = MEDIUM_INT,
                                ResultDescription = MEDIUM
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 12,
                                HighBoundary = 16,
                                Result = HIGH_INT,
                                ResultDescription = HIGH
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 15,
                                HighBoundary = int.MaxValue,
                                Result = VERY_HIGH_INT,
                                ResultDescription = VERY_HIGH
                            },
                        }
                    }
                },
                Ranges = new List<RangeEvaluation>()
                        {
                            new RangeEvaluation()
                            {
                                LowBoundary = 0,
                                HighBoundary = 10,
                                Result = NULL_OR_DISCRIMINATE_INT,
                                ResultDescription = NULL_OR_DISCRIMINATE
                            },
                             new RangeEvaluation()
                            {
                                LowBoundary = 9,
                                HighBoundary = 18,
                                Result = LOW_INT,
                                ResultDescription = LOW
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 17,
                                HighBoundary = 28,
                                Result = MEDIUM_INT,
                                ResultDescription = MEDIUM
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 27,
                                HighBoundary = 38,
                                Result = HIGH_INT,
                                ResultDescription = HIGH
                            },
                            new RangeEvaluation()
                            {
                                LowBoundary = 37,
                                HighBoundary = int.MaxValue,
                                Result = VERY_HIGH_INT,
                                ResultDescription = VERY_HIGH
                            },
                        }
            };

            return new List<Category>() { workEnvironment, innerActivityFactors, workTimeOrganization, leaderShipAndWorkRelations };
        }
    }

    /*
     * 
     * Obtener las respuestas
     * Obtener a que categoria dominio y dimension corresponde la respuesta
     * Obtener los puntos de acuerdo al diccionario GetPoints()
     * sumar puntos por dominio
     * Tomar el resultado por rango de la suma por dominio
     * Sumar puntos por categoria
     * Tomar el resultado por rango de la suma de la categoria
     * Sumar todos los puntos 
     * Tomar el resultado por rango del cuestionario
     */


    public class Category
    {
        public string Description { get; set; }

        public List<Domain> Domains { get; set; }

        public List<RangeEvaluation> Ranges { get; set; }

    }

    public class Domain
    {
        public string Description { get; set; }

        public List<Dimenssion> Dimenssions { get; set; }

        public List<RangeEvaluation> Ranges { get; set; }

    }

    public class Dimenssion
    {
        public string Description { get; set; }

        public int[] Items { get; set; }

    }

    public class RangeEvaluation
    {
        public int Result { get; set; }
        public string ResultDescription { get; set; }
        public int LowBoundary { get; set; }
        public int HighBoundary { get; set; }
    }
}
