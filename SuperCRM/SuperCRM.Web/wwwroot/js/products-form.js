document.addEventListener("DOMContentLoaded", function () {
    const addButton = document.getElementById("add-image-row");
    const rowsContainer = document.getElementById("product-image-rows");
    const template = document.getElementById("product-image-row-template");

    if (addButton && rowsContainer && template) {
        addButton.addEventListener("click", function () {
            const index = rowsContainer.querySelectorAll(".product-image-row").length;
            const html = template.innerHTML.replaceAll("__index__", index.toString());
            rowsContainer.insertAdjacentHTML("beforeend", html);
        });
    }

    document.addEventListener("change", function (event) {
        if (!event.target.classList.contains("product-image-primary")) {
            return;
        }

        if (!event.target.checked) {
            return;
        }

        document.querySelectorAll(".product-image-primary").forEach(function (checkbox) {
            if (checkbox !== event.target) {
                checkbox.checked = false;
            }
        });
    });
});
