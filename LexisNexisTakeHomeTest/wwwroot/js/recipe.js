$(document).ready(function () {
    // Delete recipe functionality
    $(".delete-recipe").on("click", function () {
        let recipeId = $(this).data("recipe-id");

        if (confirm("Are you sure you want to delete this recipe?")) {
            $.ajax({
                url: "/Recipes/Delete/" + recipeId,
                method: "POST",
                success: function () {
                    location.reload();
                },
                error: function () {
                    alert("An error occurred while deleting the recipe. Please try again.");
                }
            });
        }
    });

    // Search functionality
    $("#searchInput").on("keyup", function () {
        const searchText = $(this).val().toLowerCase();

        $("table tbody tr").each(function () {
            const recipeTitle = $(this).find("td:first").text().toLowerCase();
            const recipeDescription = $(this).find("td:nth-child(2)").text().toLowerCase();

            if (recipeTitle.includes(searchText) || recipeDescription.includes(searchText)) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    });
});
