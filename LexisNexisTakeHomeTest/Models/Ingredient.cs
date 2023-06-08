namespace LexisNexisTakeHomeTest.Models
{
    public class Ingredient
    {
        public Ingredient()
        {
            Name = string.Empty;
            Amount = string.Empty;
        }

        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public Recipe? Recipe { get; set; }
    }
}
