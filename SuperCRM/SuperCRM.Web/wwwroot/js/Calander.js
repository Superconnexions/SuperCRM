
document.addEventListener('click', function (e) {
    const btn = e.target.closest('.crm-date-btn');
    if (!btn) return;

    const wrapper = btn.closest('.crm-date-wrap');
    const input = wrapper ? wrapper.querySelector('input[type="date"]') : null;

    if (!input) return;

    if (typeof input.showPicker === 'function') {
        input.showPicker();
    } else {
        input.focus();
        input.click();
    }
});


