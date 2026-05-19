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
        setSelectValue('PersonalAddress_CountryId', getProp(personalAddress, 'countryId', 'CountryId'));
        setSelectValue('PersonalAddress_RegionId', getProp(personalAddress, 'regionId', 'RegionId'));
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
        setSelectValue('BusinessAddress_CountryId', getProp(businessAddress, 'countryId', 'CountryId'));
        setSelectValue('BusinessAddress_RegionId', getProp(businessAddress, 'regionId', 'RegionId'));
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

    document.addEventListener('DOMContentLoaded', function () {
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
