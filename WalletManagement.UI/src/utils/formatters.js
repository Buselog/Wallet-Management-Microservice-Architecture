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

export const formatCurrency = (amount, currencyCode) => {
    const currentLang = i18n.language === 'tr' ? 'tr-TR' : 'en-US';
    return new Intl.NumberFormat(currentLang, {
        style: 'currency',
        currency: currencyCode,
        minimumFractionDigits: 2
    }).format(amount);
};