/**
 * Reusable SweetAlert2 Utility Functions
 */
const Alert = {
    // 1. Success Alert
    success: (title, text = '') => {
        return Swal.fire({ title, text, icon: 'success' });
    },

    // 2. Error Alert
    error: (title, text = '') => {
        return Swal.fire({ title, text, icon: 'error' });
    },

    // 3. Info / Warning Alert
    info: (title, text = '', icon = 'info') => {
        return Swal.fire({ title, text, icon });
    },

    // 4. Confirm Dialog (Returns a promise resolving to true or false)
    confirm: async (title, text = 'You cannot undo this action.') => {
        const result = await Swal.fire({
            title,
            text,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, proceed',
            cancelButtonText: 'Cancel'
        });
        return result.isConfirmed;
    }
};

/**
 * Global Event Listener (Handles clicks automatically via data attributes)
 */
document.addEventListener('click', async (event) => {
    // Find if the clicked element (or its parent) has a data-swal attribute
    const button = event.target.closest('[data-swal]');
    if (!button) return;

    event.preventDefault();

    const type = button.getAttribute('data-swal'); // success, error, info, confirm
    const title = button.getAttribute('data-title') || 'Notice';
    const text = button.getAttribute('data-text') || '';

    // Trigger correct alert based on data-swal type
    if (type === 'success') {
        Alert.success(title, text);
    } else if (type === 'error') {
        Alert.error(title, text);
    } else if (type === 'info' || type === 'warning') {
        Alert.info(title, text, type);
    } else if (type === 'confirm') {
        const confirmed = await Alert.confirm(title, text);
        if (confirmed) {
            // Custom behavior: Execute form submission if it's a delete button inside a form
            if (button.type === 'submit') {
                button.closest('form').submit();
            } else {
                // Trigger a custom success message or execute standard JavaScript action
                Alert.success('Action Confirmed', 'The operation proceeded successfully.');
            }
        }
    }
});
