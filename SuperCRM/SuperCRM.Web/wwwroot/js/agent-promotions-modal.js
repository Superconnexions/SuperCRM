(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        const trigger = document.getElementById('agentPromotionAlert');
        const modalElement = document.getElementById('activePromotionsModal');
        const body = document.getElementById('activePromotionsModalBody');
        if (!trigger || !modalElement || !body || !window.bootstrap) return;
        const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
        let loading = false;
        trigger.addEventListener('click', async function (event) {
            event.preventDefault();
            modal.show();
            if (loading) return;
            loading = true;
            body.innerHTML = '<div class="promotion-items-loading">Loading active promotions...</div>';
            try {
                const response = await fetch(trigger.dataset.modalUrl, {
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                });
                if (response.redirected || !response.ok) throw new Error('Unable to load promotions. Please check your session and try again.');
                body.innerHTML = await response.text();
                initializePromotionItems(body);
            } catch (error) {
                body.innerHTML = '<div class="alert alert-danger m-3"></div>';
                body.firstElementChild.textContent = error.message || 'Unable to load promotions. Close and reopen to retry.';
            } finally { loading = false; }
        });
    });


    function initializePromotionItems(root) {
        const buttons = root.querySelectorAll('.btn-view-promotion-items');
        const section = root.querySelector('[data-promotion-items-section]');
        const container = root.querySelector('[data-promotion-items-container]');
        const token = root.querySelector(
            '.promotion-ajax-token-form input[name="__RequestVerificationToken"]');

        if (!section || !container || !token) {
            return;
        }

        buttons.forEach(function (button) {
            button.addEventListener('click', async function () {
                const promotionId = button.dataset.promotionId;

                if (!promotionId) {
                    return;
                }

                const originalText = button.textContent;

                try {
                    setButtonsBusy(buttons, true);
                    button.textContent = 'Loading...';

                    selectPromotionRow(root, null);
                    section.classList.remove('d-none');
                    container.innerHTML =
                        '<div class="promotion-items-loading">Loading promotion items...</div>';

                    const formData = new FormData();
                    formData.append('__RequestVerificationToken', token.value);
                    formData.append('promotionId', promotionId);

                    const response = await fetch(root.dataset.itemsUrl, {
                        method: 'POST',
                        headers: {
                            'X-Requested-With': 'XMLHttpRequest'
                        },
                        body: formData
                    });

                    if (!response.ok) {
                        const message = await response.text();
                        throw new Error(message || 'Unable to load promotion items.');
                    }

                    container.innerHTML = await response.text();

                    selectPromotionRow(root, promotionId);
                    markPromotionAsViewed(promotionId, root);

                    updateHeaderNotification(
                        response.headers.get('X-Active-Promotion-Count'),
                        response.headers.get('X-New-Promotion-Count'));

                    section.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
                catch (error) {
                    container.innerHTML =
                        '<div class="alert alert-danger mb-0">' +
                        escapeHtml(error.message || 'Unable to load promotion items.') +
                        '</div>';
                }
                finally {
                    setButtonsBusy(buttons, false);
                    button.textContent = originalText;
                }
            });
        });
    }

    function selectPromotionRow(root, promotionId) {
        root.querySelectorAll('[data-promotion-row]').forEach(function (row) {
            const selected = row.dataset.promotionRow === promotionId;
            row.classList.toggle('promotion-row-selected', selected);
            row.setAttribute('aria-selected', selected ? 'true' : 'false');
            const button = row.querySelector('.btn-view-promotion-items');
            if (button) button.setAttribute('aria-pressed', selected ? 'true' : 'false');
        });
    }

    function setButtonsBusy(buttons, isBusy) {
        buttons.forEach(function (button) {
            button.disabled = isBusy;
        });
    }

    function markPromotionAsViewed(promotionId, root) {
        const row = root.querySelector(
            '[data-promotion-row="' + promotionId + '"]');

        if (!row) {
            return;
        }

        row.classList.remove('promotion-row-new');

        const badge = row.querySelector('.promotion-new-row-badge');

        if (badge) {
            badge.remove();
        }
    }

    function updateHeaderNotification(activeCountValue, newCountValue) {
        const alert = document.getElementById('agentPromotionAlert');
        const count = document.getElementById('agentPromotionCount');
        const newBadge = document.getElementById('agentPromotionNewBadge');

        if (!alert || !count || !newBadge) {
            return;
        }

        const activeCount = Number.parseInt(activeCountValue || '0', 10) || 0;
        const newCount = Number.parseInt(newCountValue || '0', 10) || 0;

        count.textContent = activeCount.toString();

        alert.classList.remove(
            'promotion-state-zero',
            'promotion-state-active',
            'promotion-state-new');

        if (activeCount === 0) {
            alert.classList.add('promotion-state-zero');
            newBadge.classList.add('d-none');
            return;
        }

        if (newCount > 0) {
            alert.classList.add('promotion-state-new');
            newBadge.classList.remove('d-none');
        }
        else {
            alert.classList.add('promotion-state-active');
            newBadge.classList.add('d-none');
        }
    }

    function escapeHtml(value) {
        const div = document.createElement('div');
        div.textContent = value;
        return div.innerHTML;
    }
})();
