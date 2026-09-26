(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        const buttons = document.querySelectorAll('.btn-view-promotion-items');
        const section = document.getElementById('promotionItemsSection');
        const container = document.getElementById('promotionItemsContainer');
        const token = document.querySelector(
            '#promotionAjaxTokenForm input[name="__RequestVerificationToken"]');

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

                    section.classList.remove('d-none');
                    container.innerHTML =
                        '<div class="promotion-items-loading">Loading promotion items...</div>';

                    const formData = new FormData();
                    formData.append('__RequestVerificationToken', token.value);
                    formData.append('promotionId', promotionId);

                    const response = await fetch('/AgentPromotions/PromotionItems', {
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

                    markPromotionAsViewed(promotionId);

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
    });

    function setButtonsBusy(buttons, isBusy) {
        buttons.forEach(function (button) {
            button.disabled = isBusy;
        });
    }

    function markPromotionAsViewed(promotionId) {
        const row = document.querySelector(
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
