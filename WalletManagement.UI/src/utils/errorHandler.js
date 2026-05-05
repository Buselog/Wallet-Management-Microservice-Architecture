export const handleApiError = (error, form, setGeneralError) => {
    const responseData = error.response?.data;
    const errorMsg = responseData?.Message || responseData?.message;
    const status = error.response?.status;

    if (errorMsg) {
        if (status === 400 && (errorMsg.includes('|') || errorMsg.includes(':'))) {
            const parts = errorMsg.split(' | ');
            const fields = [];

            const propertyMap = {
                'FirstName': 'firstName', 'LastName': 'lastName',
                'Email': 'email', 'PhoneNumber': 'phoneNumber', 'Password': 'password'
            };

            parts.forEach(part => {
                if (part.includes(':')) {
                    const [property, msg] = part.split(':').map(s => s.trim());
                    const fieldName = propertyMap[property] || property.charAt(0).toLowerCase() + property.slice(1);
                    fields.push({ name: fieldName, errors: [msg] });
                } else {
                    setGeneralError(part);
                }
            });
            if (fields.length > 0) form.setFields(fields);
        } else {
            setGeneralError(errorMsg);
        }
    } else {
        setGeneralError(t('unexpected_error_msg'));
    }
};
