using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Json.Serialization;

namespace StoryMaker
{
    /// <summary>
    /// Represents the possible scores for a rubric criterion.
    /// </summary>
    public enum CriteriaScore
    {
        Weak = 1,
        BelowAverage = 2,
        Solid = 3,
        Strong = 4,
        Excellent = 5
    }

    /// <summary>
    /// Abstract base record for rubric criteria, holding a score and explanation.
    /// </summary>
    public abstract record Criteria(CriteriaScore Score, string Explanation);

    /// <summary>
    /// Represents the relevance criterion.
    /// </summary>
    public record Relevance(CriteriaScore Score, string Explanation) : Criteria(Score, Explanation);

    /// <summary>
    /// Represents the ownership criterion.
    /// </summary>
    public record Ownership(CriteriaScore Score, string Explanation) : Criteria(Score, Explanation);

    /// <summary>
    /// Represents the complexity criterion.
    /// </summary>
    public record Complexity(CriteriaScore Score, string Explanation) : Criteria(Score, Explanation);

    /// <summary>
    /// Represents the influence criterion.
    /// </summary>
    public record Influence(CriteriaScore Score, string Explanation) : Criteria(Score, Explanation);

    /// <summary>
    /// Represents the outcome criterion.
    /// </summary>
    public record Outcome(CriteriaScore Score, string Explanation) : Criteria(Score, Explanation);

    /// <summary>
    /// Represents the reflection criterion.
    /// </summary>
    public record Reflection(CriteriaScore Score, string Explanation) : Criteria(Score, Explanation);

    /// <summary>
    /// Represents the overall story score, including all rubric criteria and areas for improvement.
    /// </summary>
    public record StoryScore(
        Relevance Relevance,
        Ownership Ownership,
        Complexity Complexity,
        Influence Influence,
        Outcome Outcome,
        Reflection Reflection,
        string[] AreasForImprovement)
    {
        /// <summary>
        /// JSON schema for the StoryScore object.
        /// </summary>
        public static readonly JsonNode Schema = new JsonSerializerOptions(JsonSerializerOptions.Default)
        {
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        }.GetJsonSchemaAsNode(
            typeof(StoryScore),
            new JsonSchemaExporterOptions
            {
                TreatNullObliviousAsNonNullable = true,
                TransformSchemaNode = (context, schema) =>
                {
                    Console.WriteLine(schema);
                    return schema;
                }
            });

        /// <summary>
        /// Gets the total score by summing all criteria scores.
        /// </summary>
        [JsonIgnore]
        public int TotalScore => (int)Relevance.Score + (int)Ownership.Score + (int)Complexity.Score + (int)Influence.Score + (int)Outcome.Score + (int)Reflection.Score;

        /// <summary>
        /// Gets the percentage score out of 100.
        /// </summary>
        [JsonIgnore]
        public int PercentageScore => (int)((TotalScore / 30.0) * 100);

        /// <summary>
        /// Gets the overall criteria score.
        /// </summary>
        [JsonIgnore]
        public CriteriaScore Overall => TotalScore switch
        {
            >= 1 and <= 6 => CriteriaScore.Weak,
            >= 7 and <= 12 => CriteriaScore.BelowAverage,
            >= 13 and <= 18 => CriteriaScore.Solid,
            >= 19 and <= 24 => CriteriaScore.Strong,
            >= 25 and <= 30 => CriteriaScore.Excellent,
            _ => CriteriaScore.Weak
        };
    }

    /// <summary>
    /// Represents a prompt for scoring a story.
    /// </summary>
    public record StoryScorePrompt(string StoryText)
    {
        private readonly string prompt = @$"""
Please critically evaluate the following career story using the STAR format rubric. Rate each of the listed dimensions on a scale from 1 (Weak) to 5 (Excellent), and provide a short explanation for each rating. At the end, provide a total score out of 30 and suggest 1–2 areas for improvement. Format the response as JSON using the specific schema.

Rubric Dimensions
•	Situation Clarity: Is the context clear and relevant?
•	Task Definition: Is the individual’s goal or responsibility clearly stated?
•	Action Specificity: Are the actions clearly described and owned by the person?
•	Result Impact: Are the outcomes measurable, observable, and tied to the actions?
•	Relevance to Target Role: Does the story align with common expectations for engineering leadership roles?
•	Complexity, Influence, or Ambiguity: Does the story involve scale, ambiguity, influencing others, or navigating challenges?

JSON Schema
{StoryScore.Schema}

Career Story
{StoryText}
""";

        /// <summary>
        /// Returns the full prompt string.
        /// </summary>
        public override string ToString() => prompt;
    }

    /// <summary>
    /// Interface for evaluating a story and returning a StoryScore.
    /// </summary>
    public interface IStoryEvaluator
    {
        /// <summary>
        /// Evaluates the given story text and returns a StoryScore.
        /// </summary>
        /// <param name="storyText">The story to evaluate.</param>
        /// <returns>The evaluated StoryScore, or null if evaluation fails.</returns>
        Task<StoryScore?> Evaluate(string storyText);
    }

    /// <summary>
    /// Evaluates stories using a chat model and logger.
    /// </summary>
    public class StoryEvaluator(IChatManager chatManager, ILogger<StoryEvaluator> logger) : IStoryEvaluator
    {
        /// <summary>
        /// Evaluates a story using the provided chat manager and logger, returning the raw JSON response.
        /// </summary>
        /// <param name="chatManager">The chat manager to use.</param>
        /// <param name="logger">The logger to use.</param>
        /// <param name="storyText">The story to evaluate.</param>
        /// <returns>The raw JSON response wrapped in a Try monad.</returns>
        public static async Task<Try<string>> EvaluateAsync(IChatManager chatManager, ILogger logger, string storyText)
        {
            try
            {
                var prompt = new StoryScorePrompt(storyText).ToString();
                
                List<ChatMessage> messages = [
                    new SystemMessage("You are an expert in evaluating career stories using the STAR format rubric. Your task is to provide a detailed evaluation of the story provided in the next message. Your standards are high, you will be critical yet fair."),
                    new UserMessage(prompt)
                ];

                logger.LogDebug("Prompt: {prompt}", prompt);

                var chatClient = chatManager.GetChatClient("gpt-4o");
                var result = await chatClient.GetFormattedResponseAsync(messages, new JsonSchema(StoryScore.Schema, "story_score", true));
                var response = result.Items.Select(c => c.Text).FirstOrDefault();
                
                logger.LogDebug("Response: {response}", response);
                
                var finalResponse = response ?? throw new InvalidOperationException("No response received from chat client");
                return Try<string>.Succ(finalResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error evaluating story: {storyText}", storyText);
                return Try<string>.Fail(ex);
            }
        }

        /// <summary>
        /// Legacy method for backward compatibility. Use EvaluateAsync for better functional patterns.
        /// </summary>
        /// <param name="chatManager">The chat manager to use.</param>
        /// <param name="logger">The logger to use.</param>
        /// <param name="storyText">The story to evaluate.</param>
        /// <returns>The raw JSON response as a string, or null if evaluation fails.</returns>
        public static async Task<string?> Evaluate(IChatManager chatManager, ILogger logger, string storyText)
        {
            var result = await EvaluateAsync(chatManager, logger, storyText);
            return result.Match(
                Succ: response => response,
                Fail: _ => null
            );
        }

        /// <summary>
        /// Parses a JSON string into a StoryScore object using functional patterns.
        /// </summary>
        /// <param name="json">The JSON string to parse.</param>
        /// <returns>The parsed StoryScore wrapped in an Option type.</returns>
        public static Option<StoryScore> ParseOption(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return None;

            try
            {
                var score = JsonDocument.Parse(json).Deserialize<StoryScore>();
                return Optional(score).Where(s => s != null).Map(s => s!);
            }
            catch
            {
                return None;
            }
        }

        /// <summary>
        /// Legacy parse method for backward compatibility.
        /// </summary>
        /// <param name="json">The JSON string to parse.</param>
        /// <returns>The parsed StoryScore, or null if parsing fails.</returns>
        public static StoryScore? Parse(string? json) =>
            ParseOption(json).Match(
                Some: score => score,
                None: () => null
            );

        /// <summary>
        /// Evaluates a story and returns a StoryScore using functional patterns.
        /// </summary>
        /// <param name="storyText">The story to evaluate.</param>
        /// <returns>The evaluated StoryScore wrapped in an Option type.</returns>
        public async Task<Option<StoryScore>> EvaluateAsync(string storyText)
        {
            var result = await EvaluateAsync(chatManager, logger, storyText);
            return result.Match(
                Succ: json => ParseOption(json),
                Fail: _ => None
            );
        }

        /// <summary>
        /// Legacy evaluate method for backward compatibility.
        /// </summary>
        /// <param name="storyText">The story to evaluate.</param>
        /// <returns>The evaluated StoryScore, or null if evaluation fails.</returns>
        public async Task<StoryScore?> Evaluate(string storyText) => 
            (await EvaluateAsync(storyText)).Match(
                Some: score => score,
                None: () => null
            );
    }
}
