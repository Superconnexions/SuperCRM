(function () {
    const state = {
        customerId: null,
        activeTab: '#customer-basic-tab',
        changed: false
    };

    function qs(selector, root) {
        return (root || document).querySelector(selector);
    }

    function qsa(selector, root) {
        return Array.from((root || document).querySelectorAll(selector));
    }

    function escapeHtml(value) {
        const div = document.createElement('div');
        div.textContent = value == null ? '' : value.toString();
        return div.innerHTML;
    }

    function showMessage(containerId, message, success) {
        const box = document.getElementById(containerId);
        if (!box) return;
        box.innerHTML = '<div class="alert alert-' + (success ? 'success' : 'danger') + ' alert-dismissible fade show" role="alert">' +
            escapeHtml(message || (success ? 'Saved successfully.' : 'Unable to save.')) +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button></div>';
    }

    function clearErrors(form) {
        qsa('.is-invalid', form).forEach(x => x.classList.remove('is-invalid'));
        qsa('[data-error-for]', form).forEach(x => x.textContent = '');
    }

    function applyErrors(form, errors) {
        if (!errors) return;
        Object.keys(errors).forEach(function (key) {
            const messages = errors[key] || [];
            const input = qs('[name="' + key + '"]', form);
            const error = qs('[data-error-for="' + key + '"]', form);
            if (input) input.classList.add('is-invalid');
            if (error) error.textContent = messages.join(' ');
        });
    }

    async function parseJson(response) {
        const contentType = response.headers.get('content-type') || '';
        if (!contentType.includes('application/json')) {
            throw new Error(await response.text() || 'Unexpected server response.');
        }
        return await response.json();
    }

    async function loadCustomerEdit(customerId, activeTab) {
        const modalElement = document.getElementById('customerEditModal');
        const body = document.getElementById('customerEditModalBody');
        if (!modalElement || !body) return;

        state.customerId = customerId;
        state.activeTab = activeTab || state.activeTab || '#customer-basic-tab';
        body.innerHTML = '<div class="py-5 text-center"><div class="spinner-border text-primary" role="status"></div><div class="mt-2 text-muted">Loading customer...</div></div>';

        const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
        modal.show();

        try {
            const response = await fetch('/SalesOrder/GetCustomerEdit?customerId=' + encodeURIComponent(customerId), {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });

            if (!response.ok) {
                throw new Error(await response.text() || 'Customer could not be loaded.');
            }

            body.innerHTML = await response.text();
            initializeLoadedContent();
            activateTab(state.activeTab);
        } catch (error) {
            body.innerHTML = '<div class="alert alert-danger">' + escapeHtml(error.message) + '</div>';
        }
    }

    function activateTab(target) {
        const trigger = qs('[data-bs-target="' + target + '"]', document.getElementById('customerEditModalBody'));
        if (trigger) bootstrap.Tab.getOrCreateInstance(trigger).show();
    }

    function initializeLoadedContent() {
        const body = document.getElementById('customerEditModalBody');
        if (!body) return;

        qsa('[data-bs-toggle="tab"]', body).forEach(function (button) {
            button.addEventListener('shown.bs.tab', function () {
                state.activeTab = button.getAttribute('data-bs-target');
            });
        });

        toggleLimitedFields();
    }

    async function submitAjaxForm(form, messageBoxId, activeTab) {
        clearErrors(form);
        const submitButton = qs('[type="submit"]', form);
        if (submitButton) submitButton.disabled = true;

        try {
            const response = await fetch(form.action, {
                method: 'POST',
                body: new FormData(form),
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });
            const result = await parseJson(response);
            const success = result.success ?? result.Success;
            const message = result.message ?? result.Message;
            const errors = result.errors ?? result.Errors;

            if (!success) {
                applyErrors(form, errors);
                showMessage(messageBoxId, message, false);
                return false;
            }

            state.changed = true;
            await loadCustomerEdit(state.customerId, activeTab);
            showMessage(messageBoxId, message, true);
            return true;
        } catch (error) {
            showMessage(messageBoxId, error.message, false);
            return false;
        } finally {
            if (submitButton) submitButton.disabled = false;
        }
    }

    function resetAddressForm() {
        const form = document.getElementById('customerAddressForm');
        if (!form) return;
        form.reset();
        clearErrors(form);
        qs('#editAddress_CustomerAddressId', form).value = '';
        qs('#editAddress_AddressType', form).value = '1';
        qs('#editAddress_CityId', form).innerHTML = '<option value="">-- Select City --</option>';
        qs('#editAddress_City', form).value = '';
        document.getElementById('customerAddressEditTitle').textContent = 'Add Address';
        const message = document.getElementById('customerAddressFormMessage');
        if (message) message.innerHTML = '';
    }

    async function loadCities(countryId, regionId, cityId) {
        const region = document.getElementById('editAddress_RegionId');
        const city = document.getElementById('editAddress_CityId');
        if (!region || !city) return;

        city.innerHTML = '<option value="">-- Select City --</option>';
        if (!countryId) {
            region.value = '';
            return;
        }

        let selectedRegionId = regionId;
        if (!selectedRegionId) {
            const regionResponse = await fetch('/SalesOrder/GetAnyRegionByCountry?countryId=' + encodeURIComponent(countryId));
            const regionData = await regionResponse.json();
            selectedRegionId = regionData.regionId || regionData.RegionId;
        }

        if (!selectedRegionId) return;
        region.value = selectedRegionId.toString();

        const cityResponse = await fetch('/SalesOrder/GetCitiesByRegion?regionId=' + encodeURIComponent(selectedRegionId));
        const cities = await cityResponse.json();
        (cities || []).forEach(function (item) {
            const option = document.createElement('option');
            option.value = item.id ?? item.Id;
            option.textContent = item.name ?? item.Name;
            city.appendChild(option);
        });

        if (cityId) city.value = cityId.toString();
        setCityText();
    }

    async function loadCitiesForAddress(prefix, countryId, cityId) {

        const country = qs(prefix + '_CountryId');
        const city = qs(prefix + '_CityId');
        const cityText = qs(prefix + '_City');

        if (!country || !city)
            return;

        if (countryId)
            country.value = countryId;

        city.innerHTML = '<option value="">-- Select City --</option>';

        if (!country.value)
            return;

        // Region is always "Any" (RegionId = 1)
        const cityResponse =
            await fetch('/SalesOrder/GetCitiesByRegion?regionId=1');

        const cities = await cityResponse.json();

        cities.forEach(function (item) {

            if (item.countryId != country.value)
                return;

            const option = document.createElement('option');

            option.value = item.id;
            option.text = item.name;

            city.appendChild(option);

        });

        if (cityId)
            city.value = cityId;

        const selected = city.options[city.selectedIndex];

        if (cityText)
            cityText.value = selected && selected.value
                ? selected.text
                : '';
    }

    function setCityText() {
        const city = document.getElementById('editAddress_CityId');
        const cityText = document.getElementById('editAddress_City');
        if (!city || !cityText) return;
        const selected = city.options[city.selectedIndex];
        cityText.value = selected && selected.value ? selected.text : '';
    }

    async function editAddress(row) {
        resetAddressForm();
        document.getElementById('customerAddressEditTitle').textContent = 'Edit Address';
        document.getElementById('editAddress_CustomerAddressId').value = row.dataset.addressId || '';
        document.getElementById('editAddress_AddressType').value = row.dataset.addressType || '1';
        document.getElementById('editAddress_AddressLine').value = row.dataset.addressLine || '';
        document.getElementById('editAddress_HouseNo').value = row.dataset.houseNo || '';
        document.getElementById('editAddress_RoadName').value = row.dataset.roadName || '';
        document.getElementById('editAddress_PostCode').value = row.dataset.postCode || '';
        document.getElementById('editAddress_CountryId').value = row.dataset.countryId || '';
        document.getElementById('editAddress_RegionId').value = row.dataset.regionId || '';
        document.getElementById('editAddress_City').value = row.dataset.city || '';
        document.getElementById('editAddress_IsDefault').checked = row.dataset.isDefault === 'true';

        await loadCities(row.dataset.countryId, row.dataset.regionId, row.dataset.cityId);
        bootstrap.Modal.getOrCreateInstance(document.getElementById('customerAddressEditModal')).show();
    }

    function toggleLimitedFields() {
        const select = document.getElementById('editBusiness_BusinessType');
        if (!select) return;
        const isLimited = select.value === '2';
        qsa('.ltd-business-field label').forEach(function (label) {
            label.classList.toggle('required-label', isLimited);
        });
    }

    document.addEventListener('click', async function (event) {
        const editCustomerButton = event.target.closest('.btn-customer-view-edit');
        if (editCustomerButton) {
            state.changed = false;
            state.activeTab = '#customer-basic-tab';
            await loadCustomerEdit(editCustomerButton.dataset.customerId, state.activeTab);
            return;
        }

        if (event.target.closest('#btnAddCustomerAddress')) {
            resetAddressForm();
            bootstrap.Modal.getOrCreateInstance(document.getElementById('customerAddressEditModal')).show();
            return;
        }

        const editAddressButton = event.target.closest('.btn-edit-customer-address');
        if (editAddressButton) {
            await editAddress(editAddressButton.closest('tr'));
        }
    });

    document.addEventListener('change', async function (event) {
        if (event.target.id === 'editAddress_CountryId') {
            await loadCities(event.target.value, null, null);
        }
        if (event.target.id === 'editAddress_CityId') setCityText();
        if (event.target.id === 'editBusiness_BusinessType') toggleLimitedFields();
    });

    document.addEventListener('submit', async function (event) {
        const form = event.target;
        if (form.id === 'customerBasicForm') {
            event.preventDefault();
            await submitAjaxForm(form, 'customerBasicMessage', '#customer-basic-tab');
        } else if (form.id === 'customerBusinessForm') {
            event.preventDefault();
            await submitAjaxForm(form, 'customerBusinessMessage', '#customer-business-tab');
        } else if (form.id === 'customerBankForm') {
            event.preventDefault();
            await submitAjaxForm(form, 'customerBankMessage', '#customer-bank-tab');
        } else if (form.id === 'customerAddressForm') {
            event.preventDefault();
            clearErrors(form);
            const submitButton = qs('[type="submit"]', form);
            if (submitButton) submitButton.disabled = true;
            try {
                const response = await fetch(form.action, {
                    method: 'POST',
                    body: new FormData(form),
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                });
                const result = await parseJson(response);
                const success = result.success ?? result.Success;
                if (!success) {
                    applyErrors(form, result.errors ?? result.Errors);
                    showMessage('customerAddressFormMessage', result.message ?? result.Message, false);
                    return;
                }
                state.changed = true;
                bootstrap.Modal.getInstance(document.getElementById('customerAddressEditModal'))?.hide();
                await loadCustomerEdit(state.customerId, '#customer-address-tab');
                showMessage('customerAddressMessage', result.message ?? result.Message, true);
            } catch (error) {
                showMessage('customerAddressFormMessage', error.message, false);
            } finally {
                if (submitButton) submitButton.disabled = false;
            }
        }
    });

    document.addEventListener('hidden.bs.modal', function (event) {
        if (event.target.id === 'customerAddressEditModal') {
            document.body.classList.add('modal-open');
            return;
        }
        if (event.target.id === 'customerEditModal' && state.changed) {
            window.location.reload();
        }
    });
})();
