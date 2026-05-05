import i18n from 'i18next';

export const getCurrencyName = (currencyCode) => {
    try {
        const currentLang = i18n.language || 'tr';
        const displayNames = new Intl.DisplayNames([currentLang], { type: 'currency' });
        return displayNames.of(currencyCode);
    } catch (e) {
        return currencyCode;
    }
};

export const getCurrencySymbol = (currencyCode) => {
    try {
        const checkLang = currencyCode === 'TRY' ? 'tr-TR' : (i18n.language === 'tr' ? 'tr-TR' : 'en-US');
        return (0).toLocaleString(checkLang, {
            style: 'currency',
            currency: currencyCode,
            minimumFractionDigits: 0,
            maximumFractionDigits: 0,
        }).replace(/\d/g, '').trim();
    } catch (e) {
        return currencyCode;
    }
};

export const formatNumber = (amount) => {
    const currentLang = i18n.language === 'tr' ? 'tr-TR' : 'en-US';
    return new Intl.NumberFormat(currentLang, {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
    }).format(amount);
};