using System.Text.Json;
using StoryMaker;
using Xunit;

namespace StoryGenTests
{
    public class StoryScoreTests
    {
        [Theory]
        [InlineData(CriteriaScore.Weak, "Weak")]
        [InlineData(CriteriaScore.BelowAverage, "BelowAverage")]
        [InlineData(CriteriaScore.Solid, "Solid")]
        [InlineData(CriteriaScore.Strong, "Strong")]
        [InlineData(CriteriaScore.Excellent, "Excellent")]
        public void Criteria_Records_StoreValues(CriteriaScore score, string expectedName)
        {
            var explanation = "Test explanation";
            Assert.Equal(score, new Relevance(score, explanation).Score);
            Assert.Equal(score, new Ownership(score, explanation).Score);
            Assert.Equal(score, new Complexity(score, explanation).Score);
            Assert.Equal(score, new Influence(score, explanation).Score);
            Assert.Equal(score, new Outcome(score, explanation).Score);
            Assert.Equal(score, new Reflection(score, explanation).Score);
        }

        [Fact]
        public void Criteria_Equality_Works()
        {
            var a = new Relevance(CriteriaScore.Strong, "A");
            var b = new Relevance(CriteriaScore.Strong, "A");
            var c = new Relevance(CriteriaScore.Weak, "B");
            Assert.Equal(a, b);
            Assert.NotEqual(a, c);
        }

        [Fact]
        public void StoryScore_TotalScore_ComputesSum()
        {
            var score = new StoryScore(
                new Relevance(CriteriaScore.Excellent, ""),
                new Ownership(CriteriaScore.Strong, ""),
                new Complexity(CriteriaScore.Solid, ""),
                new Influence(CriteriaScore.BelowAverage, ""),
                new Outcome(CriteriaScore.Weak, ""),
                new Reflection(CriteriaScore.Strong, ""),
                new[] { "Be more specific" }
            );
            // 5 + 4 + 3 + 2 + 1 + 4 = 19
            Assert.Equal(19, score.TotalScore);
        }


        [Fact]
        public void StoryEvaluator_Parse_ReturnsNullOnNullInput()
        {
            Assert.Null(StoryEvaluator.Parse(null));
        }

        [Fact]
        public void StoryEvaluator_Parse_DeserializesValidJson()
        {
            var json = """
            {
                "Relevance": { "Score": 5, "Explanation": "Very relevant" },
                "Ownership": { "Score": 4, "Explanation": "Good ownership" },
                "Complexity": { "Score": 3, "Explanation": "Some complexity" },
                "Influence": { "Score": 2, "Explanation": "Limited influence" },
                "Outcome": { "Score": 1, "Explanation": "Weak outcome" },
                "Reflection": { "Score": 4, "Explanation": "Good reflection" },
                "AreasForImprovment": [ "Clarify results" ]
            }
            """;
            var result = StoryEvaluator.Parse(json);
            Assert.NotNull(result);
            Assert.Equal(CriteriaScore.Excellent, result.Relevance.Score);
            Assert.Equal("Very relevant", result.Relevance.Explanation);
            Assert.Equal(19, result.TotalScore);
            Assert.Contains("Clarify results", result.AreasForImprovment);
        }

        [Fact]
        public void StoryEvaluator_Parse_ThrowsOnMalformedJson()
        {
            var badJson = "{ this is not valid json }";
            Assert.ThrowsAny<JsonException>(() => StoryEvaluator.Parse(badJson));
        }

        [Fact]
        public void StoryScorePrompt_ToString_ContainsStoryText()
        {
            var story = "Once upon a time";
            var prompt = new StoryScorePrompt(story);
            var promptString = prompt.ToString();
            Assert.Contains(story, promptString);
            Assert.Contains("Rubric Dimensions", promptString);
            Assert.Contains("JSON Schema", promptString);
        }

        [Fact]
        public void StoryScorePrompt_ToString_ContainsNonNullSchema()
        {

            Assert.StartsWith("{\r\n  \"type\": \"object\",", StoryScore.Schema.ToString());
        }

        [Fact]
        public void StoryScore_Schema_DisallowsAdditionalProperties()
        {
            var schemaJson = StoryScore.Schema.ToString();
            Assert.Contains(@"""additionalProperties"": false", schemaJson);
        }

        [Fact]
        public void StoryScore_Schema_DoesNotContainTotalScore()
        {
            var schemaJson = StoryScore.Schema.ToString();
            Assert.DoesNotContain("TotalScore", schemaJson);
        }
    }

}