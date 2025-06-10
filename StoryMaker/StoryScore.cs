using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Json.Serialization;

namespace StoryMaker
{
    public enum CriteriaScore
    {
        Weak = 1,
        BelowAverage = 2,
        Solid = 3,
        Strong = 4,
        Excellent = 5
    }

    public abstract record Criteria(CriteriaScore Score, string Explanation);
    public record Relevance(CriteriaScore Score, string Explanation) : Criteria(Score, Explanation);
    public record Ownership(CriteriaScore Score, string Explanation) : Criteria(Score, Explanation);
    public record Complexity(CriteriaScore Score, string Explanation) : Criteria(Score, Explanation);
    public record Influence(CriteriaScore Score, string Explanation) : Criteria(Score, Explanation);
    public record Outcome(CriteriaScore Score, string Explanation) : Criteria(Score, Explanation);
    public record Reflection(CriteriaScore Score, string Explanation) : Criteria(Score, Explanation);


    public record StoryScore(Relevance Relevance, Ownership Ownership, Complexity Complexity, Influence Influence, Outcome Outcome, Reflection Reflection, string[] AreasForImprovment)
    {
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

        [JsonIgnore]
        public int TotalScore => (int)Relevance.Score + (int)Ownership.Score + (int)Complexity.Score + (int)Influence.Score + (int)Outcome.Score + (int)Reflection.Score;

        [JsonIgnore]
        public int PercentageScore => (int)((TotalScore / 30.0) * 100);
    }

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

        public override string ToString() => prompt;
    }


    public class StoryEvaluator(OpenAIClient client, ILogger logger)
    {
        public static async Task<string?> Evaluate(OpenAIClient client, ILogger logger, string storyText)
        {
            // Create a prompt using StoryScorePrompt
            var prompt = new StoryScorePrompt(storyText).ToString();

            List<ChatMessage> messages = [
                new SystemChatMessage("You are an expert in evaluating career stories using the STAR format rubric. Your task is to provide a detailed evaluation of the story provided in the next message. Your standards are high, you will be critical yet fair."),
                new UserChatMessage(prompt)
                ];

            logger.LogInformation("Prompt: {prompt}", prompt);

            //OpenAIClient client = new (Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

            var chatClient = client.GetChatClient("gpt-4o");

            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "story_score",
                    jsonSchema: BinaryData.FromString(StoryScore.Schema.ToString()),
                    jsonSchemaIsStrict: true),
                
            };

            try
            {
                // Send the prompt to the OpenAI API and get the response
                var result = await chatClient.CompleteChatAsync(messages, options);


                return string.Concat(result.Value.Content.Select(c => c.Text));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error evaluating story");
                return null;
            }
        }

        public static StoryScore? Parse(string? json)
        {
            if (json == null)
            {
                return null;
            }

            var scoreJson = JsonDocument.Parse(json);
            var storyScore = scoreJson.Deserialize<StoryScore>();

            return storyScore;
        }

        public async Task<StoryScore?> Evaluate(string storyText) => Parse(await Evaluate(client, logger, storyText));
    }

}
