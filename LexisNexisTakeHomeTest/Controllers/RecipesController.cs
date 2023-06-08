using LexisNexisTakeHomeTest.Data;
using LexisNexisTakeHomeTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LexisNexisTakeHomeTest.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipesController"/> class.
        /// </summary>
        /// <param name="context">The application's DbContext.</param>
        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of all recipes.
        /// </summary>
        /// <returns>A view with the list of recipes.</returns>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Recipes.ToListAsync());
        }

        /// <summary>
        /// Displays the details of a specific recipe.
        /// </summary>
        /// <param name="id">The ID of the recipe to display.</param>
        /// <returns>A view with the recipe details, or NotFound if the ID is not valid.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Instructions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        /// <summary>
        /// Displays a form for editing an existing recipe.
        /// </summary>
        /// <param name="id">The ID of the recipe to edit.</param>
        /// <returns>A view with the recipe edit form, or NotFound if the ID is not valid.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Instructions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            // Sort the Instructions by StepNumber
            recipe.Instructions = recipe.Instructions.OrderBy(i => i.StepNumber).ToList();

            ViewBag.Title = "Edit Recipe";
            ViewBag.FormAction = "Edit";
            return View("CreateEdit", recipe);
        }

        /// <summary>
        /// Displays a form for creating a new recipe.
        /// </summary>
        /// <returns>A view with the recipe creation form.</returns>
        public IActionResult Create()
        {
            var newRecipe = new Recipe();

            ViewBag.Title = "Create New Recipe";
            ViewBag.FormAction = "Create";
            return View("CreateEdit", newRecipe);
        }

        /// <summary>
        /// Creates or updates a recipe.
        /// </summary>
        /// <param name="recipe">The recipe to create or update.</param>
        /// <returns>A RedirectToAction to Index if successful, or the same view with validation errors if not.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEdit(Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                await ProcessImageUploadAsync(recipe);

                if (recipe.Id == 0)
                {
                    // New Recipe
                    _context.Add(recipe);
                }
                else
                {
                    // Update Existing Recipe
                    try
                    {
                        await UpdateExistingRecipeAsync(recipe);
                    }
                    catch (Exception)
                    {
                        return NotFound();
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(recipe);
        }


        /// <summary>
        /// Deletes a recipe by its ID.
        /// </summary>
        /// <param name="id">The ID of the recipe to delete.</param>
        /// <returns>A JsonResult with a success status and a message.</returns>
        public JsonResult Delete(int id)
        {
            Recipe? recipe = _context.Recipes.SingleOrDefault(r => r.Id == id);

            if (recipe == null)
            {
                return Json(new { success = false, message = "Recipe not found." });
            }

            _context.Recipes.Remove(recipe);
            _context.SaveChanges();

            return Json(new { success = true, message = "Recipe deleted successfully." });
        }

        #region Helper Methods

        /// <summary>
        /// Checks if a recipe with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the recipe to check.</param>
        /// <returns>true if the recipe exists, false otherwise.</returns>
        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }

        /// <summary>
        /// Asynchronously processes an image uploaded as part of the recipe data, if one exists.
        /// </summary>
        /// <param name="recipe">The recipe model object to which the uploaded image data should be attached.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task ProcessImageUploadAsync(Recipe recipe)
        {
            if (Request.Form.Files.Count > 0)
            {
                var uploadedImage = Request.Form.Files.FirstOrDefault(file => file.Name == "Image");
                if (uploadedImage != null && uploadedImage.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await uploadedImage.CopyToAsync(ms);
                        recipe.Image = ms.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously updates an existing recipe in the database, based on the provided recipe model.
        /// This includes removing any ingredients or instructions that have been removed in the updated model.
        /// </summary>
        /// <param name="recipe">The updated recipe model, which should be used to update the existing recipe in the database.</param>
        /// <exception cref="Exception">Thrown when the recipe to be updated does not exist in the database.</exception>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task UpdateExistingRecipeAsync(Recipe recipe)
        {
            var existingRecipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Instructions)
                .FirstOrDefaultAsync(r => r.Id == recipe.Id);

            if (existingRecipe == null)
            {
                throw new Exception("Recipe not found.");
            }

            if (recipe.Image == null)
            {
                recipe.Image = existingRecipe.Image;
            }

            var deletedInstructions = existingRecipe.Instructions
                .Where(existingInstruction => !recipe.Instructions.Any(updatedInstruction => updatedInstruction.Id == existingInstruction.Id))
                .ToList();

            _context.Instructions.RemoveRange(deletedInstructions);

            var deletedIngredients = existingRecipe.Ingredients
                .Where(existingIngredient => !recipe.Ingredients.Any(updatedIngredient => updatedIngredient.Id == existingIngredient.Id))
                .ToList();

            _context.Ingredients.RemoveRange(deletedIngredients);

            _context.Entry(existingRecipe).CurrentValues.SetValues(recipe);
            existingRecipe.Instructions = recipe.Instructions;
            existingRecipe.Ingredients = recipe.Ingredients;
        }

        #endregion
    }
}
