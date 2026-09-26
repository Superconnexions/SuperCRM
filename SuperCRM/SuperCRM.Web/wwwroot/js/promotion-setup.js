(function () {

    const body =
        document.getElementById(
            'promotionItemsBody');

    const template =
        document.getElementById(
            'promotionProductTemplate');

    const addButton =
        document.getElementById(
            'btnAddPromotionItem');

    if (!body || !template) {
        return;
    }

    function escapeHtml(value) {
        return String(value ?? '')
            .replace(
                /[&<>"']/g,
                function (char) {
                    return {
                        '&': '&amp;',
                        '<': '&lt;',
                        '>': '&gt;',
                        '"': '&quot;',
                        "'": '&#39;'
                    }[char];
                });
    }

    function buildProductOptions() {

        const options =
            Array.from(template.options)
                .map(function (option) {

                    return `
                        <option
                            value="${escapeHtml(option.value)}"
                            data-base-id="${escapeHtml(option.dataset.baseId)}"
                            data-standard="${escapeHtml(option.dataset.standard)}">
                            ${escapeHtml(option.text)}
                        </option>`;
                })
                .join('');

        return `
            <option value="">
                -- Select Product --
            </option>
            ${options}`;
    }

    function reindexRows() {

        body.querySelectorAll('tr')
            .forEach(function (row, index) {

                row.querySelectorAll('[name]')
                    .forEach(function (input) {

                        input.name =
                            input.name.replace(
                                /Items\[\d+\]/,
                                `Items[${index}]`);
                    });
            });
    }

    function calculateRow(row) {

        const standard =
            parseFloat(
                row.querySelector(
                    '.standard')?.value)
            || 0;

        const promotionType =
            row.querySelector(
                '.promo-type')?.value;

        const percentage =
            row.querySelector(
                '.percent');

        const promotionAmount =
            row.querySelector(
                '.promo-amount');

        const finalCommission =
            row.querySelector(
                '.final');

        if (!percentage
            || !promotionAmount
            || !finalCommission) {
            return;
        }

        if (promotionType === '2') {

            percentage.readOnly = false;
            promotionAmount.readOnly = true;

            const percentageValue =
                parseFloat(
                    percentage.value)
                || 0;

            promotionAmount.value =
                (
                    standard
                    * percentageValue
                    / 100
                )
                .toFixed(2);
        }
        else {

            percentage.readOnly = true;
            percentage.value = '';

            promotionAmount.readOnly = false;
        }

        const additionalAmount =
            parseFloat(
                promotionAmount.value)
            || 0;

        finalCommission.value =
            (
                standard
                + additionalAmount
            )
            .toFixed(2);
    }

    function initializeRow(row) {

        row.querySelector(
            '.product-select')
            ?.addEventListener(
                'change',
                function (event) {

                    const selected =
                        event.target
                            .selectedOptions[0];

                    row.querySelector(
                        '.base-id')
                        .value =
                        selected?.dataset.baseId
                        || '';

                    row.querySelector(
                        '.standard')
                        .value =
                        selected?.dataset.standard
                        || '0';

                    calculateRow(row);
                });

        row.querySelector(
            '.promo-type')
            ?.addEventListener(
                'change',
                function () {
                    calculateRow(row);
                });

        row.querySelector(
            '.percent')
            ?.addEventListener(
                'input',
                function () {
                    calculateRow(row);
                });

        row.querySelector(
            '.promo-amount')
            ?.addEventListener(
                'input',
                function () {
                    calculateRow(row);
                });

        row.querySelector(
            '.remove-row')
            ?.addEventListener(
                'click',
                function () {

                    row.remove();

                    reindexRows();
                });

        calculateRow(row);
    }

    addButton?.addEventListener(
        'click',
        function () {

            const index =
                body.children.length;

            const row =
                document.createElement(
                    'tr');

            row.className =
                'promotion-item-row';

            row.innerHTML = `
                <td>
                    <input
                        type="hidden"
                        name="Items[${index}].PromotionItemId" />

                    <input
                        type="hidden"
                        class="base-id"
                        name="Items[${index}].ProductBaseCommissionId" />

                    <select
                        class="form-select product-select"
                        name="Items[${index}].ProductId">

                        ${buildProductOptions()}

                    </select>
                </td>

                <td>
                    <input
                        class="form-control text-end standard"
                        name="Items[${index}].StandardCommission"
                        readonly
                        value="0" />
                </td>

                <td>
                    <select
                        class="form-select promo-type"
                        name="Items[${index}].PromotionType">

                        <option value="1">
                            FixedAmount
                        </option>

                        <option value="2">
                            Percentage
                        </option>

                    </select>
                </td>

                <td>
                    <input
                        class="form-control text-end percent"
                        name="Items[${index}].PromotionPercentage" />
                </td>

                <td>
                    <input
                        class="form-control text-end promo-amount"
                        name="Items[${index}].PromotionAmount" />
                </td>

                <td>
                    <input
                        class="form-control text-end final"
                        name="Items[${index}].FinalCommissionAmount"
                        readonly />
                </td>

                <td class="text-center">

                    <!--
                        ASP.NET Core checkbox binding:
                        false hidden value + checked true value.
                        New PromotionItem is Active by default.
                    -->
                    <input
                        type="hidden"
                        name="Items[${index}].IsActive"
                        value="false" />

                    <input
                        type="checkbox"
                        class="form-check-input"
                        name="Items[${index}].IsActive"
                        value="true"
                        checked />

                </td>

                <td class="text-center">
                    <button
                        type="button"
                        class="btn btn-outline-danger btn-sm remove-row">
                        Remove
                    </button>
                </td>`;

            body.appendChild(row);

            initializeRow(row);
        });

    body.querySelectorAll('tr')
        .forEach(function (row) {
            initializeRow(row);
        });

})();
