namespace LexisNexisTakeHomeTest.Models
{
    public class Recipe
    {
        public Recipe()
        {
            Title = string.Empty;
            Description = string.Empty;
            Ingredients = new List<Ingredient>();
            Instructions = new List<Instruction>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[]? Image { get; set; }
        public IList<Ingredient> Ingredients { get; set; }
        public IList<Instruction> Instructions { get; set; }
    }
}
