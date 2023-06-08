namespace LexisNexisTakeHomeTest.Models
{
    public class Instruction
    {
        public Instruction()
        {
            Description = string.Empty;
        }

        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int StepNumber { get; set; }
        public string Description { get; set; }
        public Recipe? Recipe { get; set; }
    }
}
