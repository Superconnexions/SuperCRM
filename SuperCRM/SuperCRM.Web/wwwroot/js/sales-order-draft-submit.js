/* This script converts selected product UI values into Lines[index] hidden fields.
   Normal products: Quantity = 1.
   Rolls: each selected roll variant creates one draft line with entered quantity.
   Installment fields are also posted for selected products. */

function buildSalesOrderDraftLines() {
    const container = document.getElementById('draftLineContainer');
    if (!container) return;

    container.innerHTML = '';
    let index = 0;

    document.querySelectorAll('.sales-order-product-item').forEach(function (item) {
        const productCheck = item.querySelector('.product-check');
        if (!productCheck || !productCheck.checked) return;

        const productId = productCheck.value;
        const providerSelect = item.querySelector('.provider-select');
        const variantSelect = item.querySelector('.variant-select');
        
        
        const salePriceInput =
            document.getElementById('product_total_' + productId)
            || item.querySelector('.price-input');

        //alert('ProductId: ' + productId + ' Price: ' + salePriceInput?.value);

        const installmentData = getInstallmentData(item, productId);

        const rollRows = item.querySelectorAll('.roll-variant-row');

        if (rollRows.length > 0) {
            rollRows.forEach(function (row) {
                const rollCheck = row.querySelector('.roll-variant-check');
                if (!rollCheck || !rollCheck.checked) return;

                const qty = parseInt(row.querySelector('.roll-qty')?.value || '0') || 0;
                if (qty <= 0) return;

                const price = parseFloat(row.querySelector('.roll-unit-price')?.value || '0') || 0;

                // For debugging: log product and price info for each selected roll variant
                console.log('Saving Product:', productId, 'SalePrice:', salePriceInput?.value);

                appendDraftLine(
                    container,
                    index++,
                    productId,
                    rollCheck.value,
                    providerSelect?.value || '',
                    qty,
                    price,
                    installmentData
                );
            });
        } else {
            //alert('ProductId2: ' + productId + ' Price2: ' + salePriceInput?.value);
            appendDraftLine(
                container,
                index++,
                productId,
                variantSelect?.value || '',
                providerSelect?.value || '',
                1,
                parseFloat(salePriceInput?.value || '0') || 0,
                installmentData
            );
        }
    });
}

function getInstallmentData(item, productId) {
    const selectedPaymentOption =
        item.querySelector('input[name="PaymentMode_' + productId + '"]:checked')?.value || '';

    return {
        IsInstallmentSelected: selectedPaymentOption === 'Installment',
        DownPaymentAmount: item.querySelector('.down-payment-amount')?.value || '',
        NoOfInstallment: item.querySelector('.no-of-installment')?.value || '',
        MonthlyInstallmentAmount: item.querySelector('.monthly-installment-amount')?.value || '',
        FirstInstallmentDate: item.querySelector('.first-installment-date')?.value || ''
    };
}

function appendDraftLine(
    container,
    index,
    productId,
    productVariantId,
    providerProductId,
    quantity,
    salePrice,
    installmentData
) {
    const fields = {
        ProductId: productId,
        ProductVariantId: productVariantId,
        ProviderProductId: providerProductId,
        Quantity: quantity,
        SalePrice: salePrice,

        IsInstallmentSelected: installmentData?.IsInstallmentSelected || false,
        DownPaymentAmount: installmentData?.DownPaymentAmount || '',
        NoOfInstallment: installmentData?.NoOfInstallment || '',
        MonthlyInstallmentAmount: installmentData?.MonthlyInstallmentAmount || '',
        FirstInstallmentDate: installmentData?.FirstInstallmentDate || ''
    };

    Object.keys(fields).forEach(function (key) {
        const input = document.createElement('input');
        input.type = 'hidden';
        input.name = `Lines[${index}].${key}`;
        input.value = fields[key] ?? '';
        container.appendChild(input);
    });
}

function validateSalesOrderSelection() {

    let validationMessage = '';

    const productItems =
        document.querySelectorAll('.sales-order-product-item');

    for (const item of productItems) {

        const productCheck =
            item.querySelector('.product-check');

        if (!productCheck || !productCheck.checked) {
            continue;
        }

        const productName =
            item.querySelector('.product-title')?.innerText?.trim()
            || 'Selected Product';

        const providerSelect =
            item.querySelector('.provider-select');

        const variantSelect =
            item.querySelector('.variant-select');

        const salePriceInput =
            item.querySelector('.price-input');

        // ====================================================
        // Validation-1
        // Provider Product Validation
        // ====================================================

        if (providerSelect) {

            const providerValue =
                providerSelect.value || '';

            if (!providerValue) {

                alert('Please select Provider for ' + productName);
                return false;
            }
        }

        if (variantSelect) {

            const variantValue =
                variantSelect.value || '';

            if (!variantValue) {

                alert('Please select Package for ' + productName);
                return false;
            }
        }

        const totalPrice =
            parseFloat(salePriceInput?.value || '0') || 0;

        if (totalPrice <= 0) {

            alert('Total Price must be greater than zero for ' + productName);
            return false;
        }

        // ====================================================
        // Validation-2
        // Rolls Quantity Validation
        // ====================================================

        const rollRows =
            item.querySelectorAll('.roll-variant-row');

        if (rollRows.length > 0) {

            let hasQty = false;

            for (const row of rollRows) {

                const rollCheck =
                    row.querySelector('.roll-variant-check');

                if (!rollCheck || !rollCheck.checked) {
                    continue;
                }

                const qty =
                    parseInt(row.querySelector('.roll-qty')?.value || '0') || 0;

                if (qty <= 0) {

                    alert('Quantity must be greater than zero for ' + productName);
                    return false;
                }

                hasQty = true;
            }

            if (!hasQty) {

                alert('Please select at least one roll/package for ' + productName);
                return false;
            }
        }

        // ====================================================
        // Validation-3
        // Installment Validation
        // ====================================================

        const installmentRadio =
            item.querySelector('input[name^="PaymentMode_"][value="Installment"]:checked');

        if (installmentRadio) {

            const downPayment =
                parseFloat(item.querySelector('.down-payment-amount')?.value || '0') || 0;

            const noOfInstallment =
                parseInt(item.querySelector('.no-of-installment')?.value || '0') || 0;

            const monthlyInstallment =
                parseFloat(item.querySelector('.monthly-installment-amount')?.value || '0') || 0;

            if (noOfInstallment <= 0) {

                alert('No of Installment must be greater than zero for ' + productName);
                return false;
            }

            if (monthlyInstallment <= 0) {

                alert('Monthly Installment must be greater than zero for ' + productName);
                return false;
            }

            const installmentTotal =
                (monthlyInstallment * noOfInstallment) + downPayment;

            if (Math.abs(installmentTotal - totalPrice) > 0.01) {

                alert(
                    'Installment calculation mismatch for '
                    + productName
                    + '. Total Price should equal '
                    + 'Down Payment + (No Of Installment × Monthly Installment).'
                );

                return false;
            }
        }
    }

    return true;
}

document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('salesOrderProductForm');
    if (!form) return;

    form.addEventListener('submit', function (e) {

        if (!validateSalesOrderSelection()) {
            e.preventDefault();
            return false;
        }

        buildSalesOrderDraftLines();
    });
});