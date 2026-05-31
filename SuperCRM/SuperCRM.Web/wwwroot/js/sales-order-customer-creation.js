(function () {
    function qs(id) { return document.getElementById(id); }

    function setValue(id, value) {
        const el = qs(id);
        if (el) el.value = value ?? '';
    }

    function setSelectValue(id, value) {
        const el = qs(id);
        if (el) el.value = value == null ? '' : value.toString();
    }

    function getProp(obj, camel, pascal) {
        if (!obj) return null;
        return obj[camel] ?? obj[pascal] ?? null;
    }

    function toggleBusinessAddress() {
        const same = qs('IsBusinessAddressSameAsPersonal');
        const fields = qs('businessAddressFields');
        if (!same || !fields) return;
        fields.style.display = same.checked ? 'none' : '';
    }

    function toggleBusinessType() {
        const selected = document.querySelector('input[name="BusinessType"]:checked')?.value || '1';
        document.querySelectorAll('.ltd-required-label').forEach(function (label) {
            if (selected === '2') label.classList.add('required-label');
            else label.classList.remove('required-label');
        });
    }

    function setCustomerButtonMode(mode) {
        // mode: new | existingUnsaved | saved
        const btnCreate = qs('btnCreateSalesOrder');
        const btnSave = qs('btnSaveCustomer');
        const btnUpdate = qs('btnUpdateCustomer');

        if (btnSave) btnSave.style.display = mode === 'new' ? '' : 'none';
        if (btnUpdate) btnUpdate.style.display = mode === 'new' ? 'none' : '';
        if (btnCreate) btnCreate.disabled = mode !== 'saved';
    }

    async function loadCitiesForAddress(prefix, countryId, cityId) {
        const country = qs(prefix + '_CountryId');
        const region = qs(prefix + '_RegionId');
        const city = qs(prefix + '_CityId');
        const cityText = qs(prefix + '_City');

        if (!country || !region || !city) return;

        if (countryId) {
            country.value = countryId;
        }

        city.innerHTML = '<option value="">-- Select City --</option>';

        if (!country.value) return;

        const regionResponse =
            await fetch('/SalesOrder/GetAnyRegionByCountry?countryId=' + encodeURIComponent(country.value));

        const regionData =
            await regionResponse.json();

        if (!regionData.regionId) return;

        region.value = regionData.regionId;

        const cityResponse =
            await fetch('/SalesOrder/GetCitiesByRegion?regionId=' + encodeURIComponent(regionData.regionId));

        const cities =
            await cityResponse.json();

        cities.forEach(function (item) {
            const option = document.createElement('option');
            option.value = item.id;
            option.text = item.name;
            city.appendChild(option);
        });

        if (cityId) {
            city.value = cityId;
        }

        const selected = city.options[city.selectedIndex];
        if (cityText) {
            cityText.value = selected && selected.value ? selected.text : '';
        }
    }

    async function loadCustomerForSalesOrder(customerId, displayText) {
        const url = '/SalesOrder/GetCustomerForSalesOrder?customerId=' + encodeURIComponent(customerId);
        const response = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
        const data = await response.json();

        const success = data.success ?? data.Success;
        if (!success) {
            alert(data.message || data.Message || 'Customer could not be loaded.');
            return;
        }

        const customer = data.customer || data.Customer;
        const personalAddress = data.personalAddress || data.PersonalAddress;
        const business = data.business || data.Business;
        const businessAddress = data.businessAddress || data.BusinessAddress;
        const bank = data.bankAccount || data.BankAccount;

        setValue('ExistingCustomerId', customerId);

        setValue('Customer_FirstName', getProp(customer, 'firstName', 'FirstName'));
        setValue('Customer_LastName', getProp(customer, 'lastName', 'LastName'));
        setValue('Customer_DisplayName', getProp(customer, 'displayName', 'DisplayName'));
        setValue('Customer_Email', getProp(customer, 'email', 'Email'));
        setValue('Customer_AlternativeEmail', getProp(customer, 'alternativeEmail', 'AlternativeEmail'));
        setValue('Customer_Phone', getProp(customer, 'phone', 'Phone'));
        setValue('Customer_Mobile', getProp(customer, 'mobile', 'Mobile'));

        setValue('PersonalAddress_HouseNo', getProp(personalAddress, 'houseNo', 'HouseNo'));
        setValue('PersonalAddress_RoadName', getProp(personalAddress, 'roadName', 'RoadName'));
        setValue('PersonalAddress_PostCode', getProp(personalAddress, 'postCode', 'PostCode'));
        setValue('PersonalAddress_City', getProp(personalAddress, 'city', 'City'));

        //setSelectValue('PersonalAddress_CountryId', getProp(personalAddress, 'countryId', 'CountryId'));
        //setSelectValue('PersonalAddress_RegionId', getProp(personalAddress, 'regionId', 'RegionId'));

        await loadCitiesForAddress(
            'PersonalAddress',
            getProp(personalAddress, 'countryId', 'CountryId'),
            getProp(personalAddress, 'cityId', 'CityId')
        );

        setValue('PersonalAddress_AddressLine', getProp(personalAddress, 'addressLine', 'AddressLine'));

        const businessType = getProp(business, 'businessType', 'BusinessType');
        if (businessType) {
            const businessTypeRadio = document.querySelector('input[name="BusinessType"][value="' + businessType + '"]');
            if (businessTypeRadio) {
                businessTypeRadio.checked = true;
                businessTypeRadio.dispatchEvent(new Event('change'));
            }
        }

        setValue('Business_BusinessName', getProp(business, 'businessName', 'BusinessName'));
        setValue('Business_BusinessEmail', getProp(business, 'businessEmail', 'BusinessEmail'));
        setValue('Business_TradingName', getProp(business, 'tradingName', 'TradingName'));
        setValue('Business_RegistrationNo', getProp(business, 'registrationNo', 'RegistrationNo'));
        setValue('Business_ContactPersonName', getProp(business, 'contactPersonName', 'ContactPersonName'));
        setValue('Business_ContactPersonPhone', getProp(business, 'contactPersonPhone', 'ContactPersonPhone'));

        setValue('BusinessAddress_HouseNo', getProp(businessAddress, 'houseNo', 'HouseNo'));
        setValue('BusinessAddress_RoadName', getProp(businessAddress, 'roadName', 'RoadName'));
        setValue('BusinessAddress_PostCode', getProp(businessAddress, 'postCode', 'PostCode'));
        setValue('BusinessAddress_City', getProp(businessAddress, 'city', 'City'));

        await loadCitiesForAddress(
            'BusinessAddress',
            getProp(businessAddress, 'countryId', 'CountryId'),
            getProp(businessAddress, 'cityId', 'CityId')
        );

        //setSelectValue('BusinessAddress_CountryId', getProp(businessAddress, 'countryId', 'CountryId'));
        //setSelectValue('BusinessAddress_RegionId', getProp(businessAddress, 'regionId', 'RegionId'));

        setValue('BusinessAddress_AddressLine', getProp(businessAddress, 'addressLine', 'AddressLine'));

        setValue('BankAccount_BankName', getProp(bank, 'bankName', 'BankName'));
        setValue('BankAccount_AccountName', getProp(bank, 'accountName', 'AccountName'));
        setValue('BankAccount_AccountNumber', getProp(bank, 'accountNumber', 'AccountNumber'));
        setValue('BankAccount_SortCode', getProp(bank, 'sortCode', 'SortCode'));

        const selectedBox = qs('selectedCustomerBox');
        if (selectedBox) {
            selectedBox.textContent = 'Selected existing customer: ' + displayText;
            selectedBox.classList.remove('d-none');
        }

        // Customer is selected and loaded, but must be updated before Create Sales Order is enabled.
        setCustomerButtonMode('existingUnsaved');
    }

    async function loadMyCustomers() {
        const box = qs('customerSearchResults');
        const keyword = qs('customerSearchKeyword');

        if (!box) return;

        if (keyword) {
            keyword.value = '';
        }

        box.innerHTML = '<div class="text-muted">Loading your customers...</div>';

        const response = await fetch('/SalesOrder/GetMyCustomers', {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });

        const items = await response.json();

        //renderCustomerSearchResults(items, box);

        if (!items || items.length === 0) {
            box.innerHTML = '<div class="alert alert-info">No customer found. Please create a new customer below.</div>';
            return;
        }

        let html = '<div class="table-responsive"><table class="table table-bordered table-sm"><thead><tr>' +
            '<th>Code</th><th>Name</th><th>Email</th><th>Phone</th><th>Mobile</th><th>Action</th></tr></thead><tbody>';

        items.forEach(function (x) {
            const customerId = x.customerId || x.CustomerId;
            const code = x.customerCode || x.CustomerCode || '';
            const name = x.displayName || x.DisplayName || '';
            const email = x.email || x.Email || '';
            const phone = x.phone || x.Phone || '';
            const mobile = x.mobile || x.Mobile || '';
            const text = (code ? code + ' - ' : '') + name;

            html += '<tr>' +
                '<td>' + code + '</td>' +
                '<td>' + name + '</td>' +
                '<td>' + email + '</td>' +
                '<td>' + phone + '</td>' +
                '<td>' + mobile + '</td>' +
                '<td><button type="button" class="btn btn-sm btn-success select-customer" ' +
                'data-id="' + customerId + '" data-text="' + text + '">Select</button></td>' +
                '</tr>';
        });

        html += '</tbody></table></div>';
        box.innerHTML = html;


    }

    async function searchCustomers() {
        const keyword = qs('customerSearchKeyword')?.value || '';
        const box = qs('customerSearchResults');
        if (!box) return;

        if (keyword.trim().length < 2) {
            box.innerHTML = '<div class="alert alert-warning">Please enter at least 2 characters.</div>';
            return;
        }

        box.innerHTML = '<div class="text-muted">Searching...</div>';
        const url = '/SalesOrder/SearchCustomers?keyword=' + encodeURIComponent(keyword);
        const response = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
        const items = await response.json();

        if (!items || items.length === 0) {
            box.innerHTML = '<div class="alert alert-info">No customer found. Please create a new customer below.</div>';
            return;
        }

        let html = '<div class="table-responsive"><table class="table table-bordered table-sm"><thead><tr>' +
            '<th>Code</th><th>Name</th><th>Email</th><th>Phone</th><th>Mobile</th><th>Action</th></tr></thead><tbody>';

        items.forEach(function (x) {
            const customerId = x.customerId || x.CustomerId;
            const code = x.customerCode || x.CustomerCode || '';
            const name = x.displayName || x.DisplayName || '';
            const email = x.email || x.Email || '';
            const phone = x.phone || x.Phone || '';
            const mobile = x.mobile || x.Mobile || '';
            const text = (code ? code + ' - ' : '') + name;

            html += '<tr>' +
                '<td>' + code + '</td>' +
                '<td>' + name + '</td>' +
                '<td>' + email + '</td>' +
                '<td>' + phone + '</td>' +
                '<td>' + mobile + '</td>' +
                '<td><button type="button" class="btn btn-sm btn-success select-customer" ' +
                'data-id="' + customerId + '" data-text="' + text + '">Select</button></td>' +
                '</tr>';
        });

        html += '</tbody></table></div>';
        box.innerHTML = html;
    }

    const customerSearchModal = qs('customerSearchModal');

    if (customerSearchModal) {
        customerSearchModal.addEventListener('shown.bs.modal', async function () {
            await loadMyCustomers();
        });
    }

    function showCustomerValidationError(message) {
        let box = document.getElementById('clientValidationMessage');

        if (!box) {
            alert(message);
            return;
        }

        box.innerHTML =
            '<div class="alert alert-danger alert-dismissible fade show" role="alert">' +
            message +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
            '</div>';

        window.scrollTo({ top: 0, behavior: 'smooth' });
    }

    //async function validateCustomerDuplicateAsync() {
    //    const email = document.getElementById('Customer_Email')?.value?.trim() || '';
    //    const mobile = document.getElementById('Customer_Mobile')?.value?.trim() || '';
    //    const existingCustomerId = document.getElementById('ExistingCustomerId')?.value || '';

    //    if (!email && !mobile) {
    //        return true;
    //    }

    //    const url =
    //        '/SalesOrder/CheckCustomerDuplicate'
    //        + '?email=' + encodeURIComponent(email)
    //        + '&mobile=' + encodeURIComponent(mobile)
    //        + '&excludeCustomerId=' + encodeURIComponent(existingCustomerId);

    //    const response = await fetch(url, {
    //        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    //    });

    //    const result = await response.json();

    //    if (result.emailExists) {
    //        showCustomerValidationError('Email already exists. Please use a different email.');
    //        return false;
    //    }

    //    if (result.mobileExists) {
    //        showCustomerValidationError('Mobile number already exists. Please use a different mobile number.');
    //        return false;
    //    }

    //    return true;
    //}

    async function validateCustomerDuplicateAsync() {
        const email = qs('Customer_Email')?.value?.trim() || '';
        const mobile = qs('Customer_Mobile')?.value?.trim() || '';
        const sortCode = qs('BankAccount_SortCode')?.value?.trim() || '';
        const accountNumber = qs('BankAccount_AccountNumber')?.value?.trim() || '';

        const existingCustomerId = qs('ExistingCustomerId')?.value || '';
        const existingBankAccountId = qs('SelectedCustomerBankAccountId')?.value || '';

        const url =
            '/SalesOrder/CheckCustomerDuplicateForOrder'
            + '?email=' + encodeURIComponent(email)
            + '&mobile=' + encodeURIComponent(mobile)
            + '&sortCode=' + encodeURIComponent(sortCode)
            + '&accountNumber=' + encodeURIComponent(accountNumber)
            + '&excludeCustomerId=' + encodeURIComponent(existingCustomerId)
            + '&excludeBankAccountId=' + encodeURIComponent(existingBankAccountId);

        const response = await fetch(url, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });

        const result = await response.json();

        if (result.emailExists) {
            showCustomerValidationError('Email already exists.');
            return false;
        }

        if (result.mobileExists) {
            showCustomerValidationError('Mobile number already exists.');
            return false;
        }

        if (result.bankAccountExists) {
            showCustomerValidationError('Bank account already exists for this Sort Code and Account Number.');
            return false;
        }

        return true;
    }

    function validateSalesOrderCustomerForm() {
        const isBusinessFlow = document.getElementById('IsBusinessFlow')?.value === 'True'
            || document.getElementById('IsBusinessFlow')?.value === 'true';

        const hasResidential = document.getElementById('HasResidentialProduct')?.value === 'True'
            || document.getElementById('HasResidentialProduct')?.value === 'true';

        const sameAddress = document.getElementById('IsBusinessAddressSameAsPersonal')?.checked === true;

        const businessType = document.querySelector('input[name="BusinessType"]:checked')?.value || '1';

        function value(id) {
            return document.getElementById(id)?.value?.trim() || '';
        }

        function validateAddress(prefix, title) {
            if (!value(prefix + '_HouseNo')) {
                showCustomerValidationError(title + ': House No is required.');
                return false;
            }

            if (!value(prefix + '_PostCode')) {
                showCustomerValidationError(title + ': Post Code is required.');
                return false;
            }

            if (!value(prefix + '_CountryId')) {
                showCustomerValidationError(title + ': Country is required.');
                return false;
            }

            if (!value(prefix + '_CityId')) {
                showCustomerValidationError(title + ': City is required.');
                return false;
            }

            return true;
        }

        if (!value('Customer_FirstName')) {
            showCustomerValidationError('First name is required.');
            return false;
        }

        if (!value('Customer_LastName')) {
            showCustomerValidationError('Last name is required.');
            return false;
        }

        if (!value('Customer_Email')) {
            showCustomerValidationError('Email is required.');
            return false;
        }

        if (!value('Customer_Mobile')) {
            showCustomerValidationError('Mobile is required.');
            return false;
        }

        if (hasResidential) {
            if (!validateAddress('PersonalAddress', 'Home Address')) {
                return false;
            }
        }

        if (isBusinessFlow) {
            if (!value('Business_BusinessName')) {
                showCustomerValidationError('Business name is required.');
                return false;
            }

            if (!value('Business_BusinessEmail')) {
                showCustomerValidationError('Business email is required.');
                return false;
            }

            if (businessType === '2') {
                if (!value('Business_RegistrationNo')) {
                    showCustomerValidationError('Registration number is required for LTD company.');
                    return false;
                }

                if (!value('Business_ContactPersonName')) {
                    showCustomerValidationError('Contact person is required for LTD company.');
                    return false;
                }

                if (!value('Business_ContactPersonPhone')) {
                    showCustomerValidationError('Contact person phone number is required for LTD company.');
                    return false;
                }
            }

            if (!sameAddress) {
                if (!validateAddress('BusinessAddress', 'Business Address')) {
                    return false;
                }
            }
        }

        return true;
    }

    document.addEventListener('DOMContentLoaded', function () {


        const customerForm = document.getElementById('salesOrderCustomerForm');

        let isSubmittingAfterValidation = false;

        if (customerForm) {
            customerForm.addEventListener('submit', async function (e) {

                if (isSubmittingAfterValidation) {
                    return true;
                }

                e.preventDefault();

                if (!validateSalesOrderCustomerForm()) {
                    return false;
                }

                const duplicateOk = await validateCustomerDuplicateAsync();
                if (!duplicateOk) {
                    return false;
                }

                const submitter = e.submitter;

                if (submitter && submitter.formAction) {
                    customerForm.action = submitter.formAction;
                }

                if (submitter && submitter.formMethod) {
                    customerForm.method = submitter.formMethod;
                }

                isSubmittingAfterValidation = true;
                customerForm.submit();
            });
        }

        toggleBusinessAddress();
        toggleBusinessType();

        const same = qs('IsBusinessAddressSameAsPersonal');
        if (same) same.addEventListener('change', toggleBusinessAddress);

        document.querySelectorAll('.business-type-radio').forEach(function (r) {
            r.addEventListener('change', toggleBusinessType);
        });

        const searchButton = qs('btnCustomerSearch');
        if (searchButton) searchButton.addEventListener('click', searchCustomers);

        const keyword = qs('customerSearchKeyword');
        if (keyword) {
            keyword.addEventListener('keydown', function (e) {
                if (e.key === 'Enter') {
                    e.preventDefault();
                    searchCustomers();
                }
            });
        }

        document.addEventListener('click', async function (e) {
            const btn = e.target.closest('.select-customer');
            if (!btn) return;

            qs('ExistingCustomerId').value = btn.dataset.id;
            await loadCustomerForSalesOrder(btn.dataset.id, btn.dataset.text);

            const modalEl = qs('customerSearchModal');
            if (window.bootstrap && modalEl) {
                const modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                modal.hide();
            }
        });
    });
})();
