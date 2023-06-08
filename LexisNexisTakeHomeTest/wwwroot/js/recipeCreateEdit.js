// This code block executes when the page is ready
$(document).ready(function () {
    // Attaches a click event handler to the "Add Ingredient" button
    $("#addIngredientBtn").on("click", function () {
        // Retrieves the values from the "new ingredient" input fields
        let newAmount = $("#newIngredientAmount").val();
        let newName = $("#newIngredientName").val();

        // Checks if both input fields contain values (not just whitespace)
        if (newAmount.trim() !== "" && newName.trim() !== "") {
            // Counts the current number of rows in the ingredients table
            let ingredientsRowCount = $("#ingredientsTable tr").length;

            // Constructs a new row with the input field values
            let newRow = `<tr class="ingredient-row">
                            <td>
                                <!-- Hidden input for the ingredient ID, default to 0 -->
                                <input type="hidden" name="Ingredients[${ingredientsRowCount}].Id" value="0" />
                                <!-- Input for the ingredient amount -->
                                <input type="text" name="Ingredients[${ingredientsRowCount}].Amount" value="${newAmount}" class="form-control" placeholder="Amount" />
                            </td>
                            <td>
                                <!-- Input for the ingredient name -->
                                <input type="text" name="Ingredients[${ingredientsRowCount}].Name" value="${newName}" class="form-control" placeholder="Ingredient name" />
                            </td>
                            <td>
                                <!-- Button to delete the ingredient row -->
                                <button type="button" class="btn btn-danger delete-ingredient">Delete</button>
                            </td>
                        </tr>`;

            // Appends the new row to the ingredients table
            $("#ingredientsTable").append(newRow);

            // Clears the input fields
            $("#newIngredientAmount").val("");
            $("#newIngredientName").val("");
        }
    });

    // Same process as above but for instructions
    $("#addInstructionBtn").on("click", function () {
        let newDescription = $("#newInstructionDescription").val();

        if (newDescription.trim() !== "") {
            let stepNumber = $("#instructionsTable tr").length + 1;
            let instructionsRowCount = $("#instructionsTable tr").length;
            let newRow = `<tr draggable="true">
                            <td>
                                <input type="hidden" name="Instructions[${instructionsRowCount}].Id" value="0" />
                                <input type="hidden" name="Instructions[${instructionsRowCount}].StepNumber" value="${stepNumber}" />
                                <span class="step-number">${stepNumber}</span>
                            </td>
                            <td>
                                <textarea name="Instructions[${instructionsRowCount}].Description" class="form-control" placeholder="Instruction description">${newDescription}</textarea>
                            </td>
                            <td>
                                <button type="button" class="btn btn-danger delete-instruction">Delete</button>
                            </td>
                        </tr>`;

            $("#instructionsTable").append(newRow);

            // Clear the input field
            $("#newInstructionDescription").val("");
        }
    });

    // Attaches a click event handler to delete buttons in the instructions table
    $("#instructionsTable").on("click", ".delete-instruction", function () {
        // Deletes the row that contains the clicked delete button
        $(this).closest("tr").remove();
        // Calls the function to renumber the steps after a row is deleted
        renumberSteps();
    });

    // Same as above but for the ingredients table
    $("#ingredientsTable").on("click", ".delete-ingredient", function () {
        $(this).closest("tr").remove();
    });

    // Attaches a change event handler to the image input field
    $("#Image").on("change", function () {
        let currentImageContainer = $("#current-image-container");

        // Checks if a file is selected
        if (this.files && this.files[0]) {
            let reader = new FileReader();

            reader.onload = function (e) {
                // Creates a new image element with the selected image
                let imageElement = $("<img>")
                    .attr("src", e.target.result)
                    .attr("alt", "Recipe Image")
                    .addClass("img-thumbnail")
                    .addClass("mb-3");

                // If the image container exists, replace its contents with the new image
                // Otherwise, create a new image container and append it after the image input field
                if (currentImageContainer.length) {
                    currentImageContainer.empty().append(imageElement);
                } else {
                    let formGroupElement = $("<div>")
                        .addClass("form-group")
                        .attr("id", "current-image-container");

                    let labelElement = $("<label>").text("Current Image");
                    formGroupElement.append(labelElement).append(imageElement);

                    let uploadImageElement = $("#Image").closest(".form-group");
                    uploadImageElement.after(formGroupElement);
                }
            };

            // Reads the selected image as a Data URL
            reader.readAsDataURL(this.files[0]);
        }
    });

    // Call the function to attach the drag and drop events when the page is ready
    bindDragAndDropEvents();

    // Also, bind the events when a new instruction or ingredient is added
    $("#addInstructionBtn, #addIngredientBtn").on("click", function () {
        bindDragAndDropEvents();
    });
});

// This function renumbers the steps in the instructions table
function renumberSteps() {
    $("#instructionsTable tr").each(function (index) {
        // The index is zero-based, so we add 1 to get the step number
        let stepNumber = index + 1;
        // Updates the step number and the names of the input fields
        $(this).find("input[type=hidden][name$='StepNumber']").attr("name", `Instructions[${index}].StepNumber`).val(stepNumber);
        $(this).find("input[type=hidden][name$='Id']").attr("name", `Instructions[${index}].Id`);
        $(this).find("textarea[name$='Description']").attr("name", `Instructions[${index}].Description`);
        $(this).find(".step-number").text(stepNumber);
    });
}

// This function attaches drag and drop events to the instruction rows
function bindDragAndDropEvents() {
    var draggedElement;

    // First unbind existing drag and drop events to prevent duplication
    $("tr[draggable=true]").off("dragstart dragover drop");

    // On dragstart, store the dragged element and its HTML
    $("tr[draggable=true]").on("dragstart", function (event) {
        draggedElement = this;
        event.originalEvent.dataTransfer.setData('text/html', this.outerHTML);
    });

    // On dragover, allow the drop and store the index of the element being dragged over
    $("tr[draggable=true]").on("dragover", function (event) {
        event.preventDefault();
    });

    // On drop, replace the element being dragged over with the dragged element
    $("tr[draggable=true]").on("drop", function (event) {
        event.preventDefault();
        if (draggedElement !== this) {
            $(this).before(draggedElement);
            renumberSteps();
            bindDragAndDropEvents();
        }
    });
}