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
        const salePriceInput = item.querySelector('.product-price');

        const installmentData = getInstallmentData(item, productId);

        const rollRows = item.querySelectorAll('.roll-variant-row');

        if (rollRows.length > 0) {
            rollRows.forEach(function (row) {
                const rollCheck = row.querySelector('.roll-variant-check');
                if (!rollCheck || !rollCheck.checked) return;

                const qty = parseInt(row.querySelector('.roll-qty')?.value || '0') || 0;
                if (qty <= 0) return;

                const price = parseFloat(row.querySelector('.roll-unit-price')?.value || '0') || 0;

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

document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('salesOrderProductForm');
    if (!form) return;

    form.addEventListener('submit', function () {
        buildSalesOrderDraftLines();
    });
});