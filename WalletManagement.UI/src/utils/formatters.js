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
        const currentLang = i18n.language === 'tr' ? 'tr-TR' : 'en-US';
        return (0).toLocaleString(currentLang, {
            style: 'currency',
            currency: currencyCode,
            minimumFractionDigits: 0,
            maximumFractionDigits: 0,
        }).replace(/\d/g, '').trim();
    } catch (e) {
        return currencyCode; 
    }
};

export const formatCurrency = (amount, currencyCode) => {
    const currentLang = i18n.language === 'tr' ? 'tr-TR' : 'en-US';
    return new Intl.NumberFormat(currentLang, {
        style: 'currency',
        currency: currencyCode,
        minimumFractionDigits: 2
    }).format(amount);
};