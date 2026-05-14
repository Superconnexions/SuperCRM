(function () {
    function toNumber(value) {
        const parsed = parseFloat(value);
        return isNaN(parsed) ? 0 : parsed;
    }

    function recalculatePrice(productItem) {
        const basePriceType = parseInt(productItem.dataset.basePriceType || '0');
        const basePrice = toNumber(productItem.dataset.basePrice || '0');
        const priceInput = productItem.querySelector('.price-input');
        if (!priceInput) return;

        // 1 = SimplePrice, 2 = OpenPrice, 3 = VariantPrice
        if (basePriceType === 1) {
            priceInput.value = basePrice.toFixed(2);
            return;
        }

        if (basePriceType === 2) {
            priceInput.value = '0.00';
            return;
        }

        if (basePriceType === 3) {
            const selectedVariant = productItem.querySelector('.variant-select option:checked');
            if (selectedVariant && selectedVariant.value) {
                priceInput.value = toNumber(selectedVariant.dataset.price || '0').toFixed(2);
                return;
            }

            const checkedVariant = productItem.querySelector('input[name^="VariantCheckbox_"]:checked');
            if (checkedVariant) {
                priceInput.value = toNumber(checkedVariant.dataset.price || '0').toFixed(2);
                return;
            }

            priceInput.value = '0.00';
        }
    }

    document.querySelectorAll('.order-product-item').forEach(function (productItem) {
        recalculatePrice(productItem);

        productItem.querySelectorAll('.variant-select, input[name^="VariantCheckbox_"]').forEach(function (input) {
            input.addEventListener('change', function () {
                recalculatePrice(productItem);
            });
        });

        productItem.querySelectorAll('input[name^="PaymentMode_"]').forEach(function (radio) {
            radio.addEventListener('change', function () {
                const fields = productItem.querySelector('.installment-fields');
                if (!fields) return;
                fields.style.display = this.value === 'Installment' && this.checked ? '' : 'none';
            });
        });
    });
})();
